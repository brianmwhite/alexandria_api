namespace alexandria.api.Services;

using AutoMapper;
using alexandria.api.Helpers;
using alexandria.api.Repositories;
using alexandria.api.Models;
using System.Linq;
using alexandria.api.Controllers;

public interface IBookService
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;
    Task<PagedResult<BookModel>> GetAll(int page_number, int page_size);
    Task<PagedResult<BookModel>> GetById(int id);
    Task TransferBookFiles(IEnumerable<long> bookIds, long knownDeviceId);
    Task<PagedResult<BookModel>> Search(string query, int page_number, int page_size);
    Task<PagedResult<BookModel>> GetBySeries(int id, int page_number, int page_size);
    Task<PagedResult<BookModel>> GetByAuthor(int id, int page_number, int page_size);
    Task<IEnumerable<BookFormatModel>> GetBooksWithPrioritizedFormat(List<long> bookIds, List<string> formats);
}

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IFileService _fileService;
    private readonly IKnownDeviceService _knownDeviceService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    private string _bookFilesDirectory;
    private string _deviceMountDirectory;


    public BookService(
        IBookRepository bookRepository, IFileService fileService, IKnownDeviceService knownDeviceService,
        IMapper mapper, IConfiguration configuration)
    {
        _bookRepository = bookRepository;
        _fileService = fileService;
        _knownDeviceService = knownDeviceService;
        _mapper = mapper;
        _configuration = configuration;

        _bookFilesDirectory = _configuration["BookFilesDirectory"] ?? throw new ConfigurationException("BookFilesDirectory");
        _deviceMountDirectory = _configuration["DeviceMountDirectory"] ?? throw new ConfigurationException("DeviceMountDirectory");
    }

    public async Task<PagedResult<BookModel>> GetAll(int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        var pagedResultEntity = await _bookRepository.GetAll(page_number, page_size);
        var books = _mapper.Map<IEnumerable<BookModel>>(pagedResultEntity.Data);
        return new PagedResult<BookModel> { Data = books, TotalCount = pagedResultEntity.TotalCount };
    }

    public async Task<PagedResult<BookModel>> GetById(int id)
    {
        var pagedResultEntity = await _bookRepository.GetById(id) ?? throw new KeyNotFoundException("Book not found");
        var books = _mapper.Map<IEnumerable<BookModel>>(pagedResultEntity.Data);
        return new PagedResult<BookModel> { Data = books, TotalCount = pagedResultEntity.TotalCount };
    }

    public async Task TransferBookFiles(IEnumerable<long> bookIds, long knownDeviceId)
    {
        var detectedDevice = await _knownDeviceService.DetectDevice();

        if (detectedDevice?.KnownDevice?.Id != knownDeviceId)
            throw new KeyNotFoundException("Device expected does not match detected device");
        if (detectedDevice == null)
            throw new KeyNotFoundException("Device not found");
        if (detectedDevice.DeviceState != DetectedDevice.DeviceStateEnum.MOUNTED)
            throw new KeyNotFoundException("Device not mounted");
        if (detectedDevice.MatchedState != DetectedDevice.MatchedStateEnum.MATCHED_KNOWN)
            throw new KeyNotFoundException("Device not matched");
        if (detectedDevice?.USBDeviceInfo?.Mountpoint == null)
            throw new KeyNotFoundException("Device does not have a mountpoint");
        if (detectedDevice?.KnownDevice?.EbookDirectory == null)
            throw new KeyNotFoundException("Device does not have an ebook directory");
        if (detectedDevice?.KnownDevice?.FormatList == null || detectedDevice.KnownDevice.FormatList.Count == 0)
            throw new KeyNotFoundException("Device does not have any supported formats");

        var books = await GetBooksWithPrioritizedFormat(bookIds.ToList(), detectedDevice.KnownDevice.FormatList);

        foreach (var book in books)
        {
            if (book.SupportedFormat)
            {
                var bookSourceFilePath = Path.Combine(
                    _bookFilesDirectory,
                    book.BookFilePath
                );

                var bookFileName = Path.GetFileName(book.BookFilePath);

                var deviceTargetFilePath = Path.Combine(
                    detectedDevice.USBDeviceInfo.Mountpoint,
                    detectedDevice.KnownDevice.EbookDirectory
                );

                _fileService.CopyFile(bookSourceFilePath, deviceTargetFilePath, bookFileName);
            }
        }
    }

    public async Task<PagedResult<BookModel>> Search(string query, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        var pagedResultEntity = await _bookRepository.Search(query, page_number, page_size);
        var books = _mapper.Map<IEnumerable<BookModel>>(pagedResultEntity.Data);
        return new PagedResult<BookModel> { Data = books, TotalCount = pagedResultEntity.TotalCount };
    }

    public async Task<PagedResult<BookModel>> GetBySeries(int id, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        var pagedResultEntity = await _bookRepository.GetBySeriesId(id, page_number, page_size);
        var books = _mapper.Map<IEnumerable<BookModel>>(pagedResultEntity.Data);
        return new PagedResult<BookModel> { Data = books, TotalCount = pagedResultEntity.TotalCount };
    }

    public async Task<PagedResult<BookModel>> GetByAuthor(int id, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        var pagedResultEntity = await _bookRepository.GetByAuthorId(id, page_number, page_size);
        var books = _mapper.Map<IEnumerable<BookModel>>(pagedResultEntity.Data);
        return new PagedResult<BookModel> { Data = books, TotalCount = pagedResultEntity.TotalCount };
    }

    public async Task<IEnumerable<BookFormatModel>> GetBooksWithPrioritizedFormat(List<long> bookIds, List<string> formats)
    {
        if (bookIds == null || bookIds.Count == 0)
        {
            throw new KeyNotFoundException("No books selected");
        }
        if (formats == null || formats.Count == 0)
        {
            throw new KeyNotFoundException("No books selected or device does not have any supported formats");
        }
        var books = await _bookRepository.GetBooksByFormat(bookIds, formats);
        return books;
    }
}