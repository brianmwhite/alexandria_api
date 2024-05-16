namespace alexandria.api.Services;

using AutoMapper;
using alexandria.api.Helpers;
using alexandria.api.Repositories;
using alexandria.api.Models;

public interface IBookService
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;
    Task<PagedResult<BookModel>> GetAll(int page_number, int page_size);
    Task<PagedResult<BookModel>> GetById(int id);
    Task TransferBookFile(long id, string format, long knownDeviceId);
    Task<PagedResult<BookModel>> Search(string query, int page_number, int page_size);
    Task<PagedResult<BookModel>> GetBySeries(int id, int page_number, int page_size);
    Task<PagedResult<BookModel>> GetByAuthor(int id, int page_number, int page_size);
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

    public async Task TransferBookFile(long bookId, string format, long knownDeviceId)
    {
        var bookFilePath = await _bookRepository.GetBookFilePath(bookId, format)
            ?? throw new KeyNotFoundException("Book does not have a file path");

        bookFilePath = Path.Combine(_bookFilesDirectory, bookFilePath);

        var knownDevice = await _knownDeviceService.GetById(knownDeviceId);
        if (knownDevice == null || knownDevice.Data == null || knownDevice.TotalCount == 0)
        {
            throw new KeyNotFoundException("Device not found");
        }
        var deviceEbookStorageDirectory = knownDevice.Data.FirstOrDefault()?.EbookDirectory
            ?? throw new KeyNotFoundException("Device does not have an EbookDirectory");

        deviceEbookStorageDirectory = Path.Combine(_deviceMountDirectory, deviceEbookStorageDirectory);

        _fileService.CopyFile(bookFilePath, deviceEbookStorageDirectory);
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


}