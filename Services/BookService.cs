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
    Task TransferBookFile(int id, string format);
    Task<PagedResult<BookModel>> Search(string query, int page_number, int page_size);
    Task<PagedResult<BookModel>> GetBySeries(int id, int page_number, int page_size);
    Task<PagedResult<BookModel>> GetByAuthor(int id, int page_number, int page_size);
}

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public BookService(IBookRepository bookRepository, IFileService fileService, IMapper mapper)
    {
        _bookRepository = bookRepository;
        _fileService = fileService;
        _mapper = mapper;
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

    public async Task TransferBookFile(int id, string format)
    {
        var filePath = await _bookRepository.GetBookFilePath(id, format);
        if (filePath != null)
        {
            _fileService.CopyFile(filePath);
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


}