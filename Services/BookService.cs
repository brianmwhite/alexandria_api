namespace alexandria.api.Services;

using AutoMapper;
using alexandria.api.Entities;
using alexandria.api.Repositories;

public interface IBookService
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;
    Task<PagedResult<Book>> GetAll(int page_number, int page_size);
    Task<PagedResult<Book>> GetById(int id);
    Task<PagedResult<Book>> Search(string query, int page_number, int page_size);
    Task<PagedResult<Book>> GetBySeries(int id, int page_number, int page_size);
    Task<PagedResult<Book>> GetByAuthor(int id, int page_number, int page_size);
}

public class BookService(
    IBookRepository bookRepository,
    IMapper mapper) : IBookService
{
    private IBookRepository _bookRepository = bookRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<PagedResult<Book>> GetAll(int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        var pagedResultEntity = await _bookRepository.GetAll(page_number, page_size);
        var books = _mapper.Map<IEnumerable<Book>>(pagedResultEntity.Data);
        return new PagedResult<Book> { Data = books, TotalCount = pagedResultEntity.TotalCount };
    }

    public async Task<PagedResult<Book>> GetById(int id)
    {
        var pagedResultEntity = await _bookRepository.GetById(id) ?? throw new KeyNotFoundException("Book not found");
        var books = _mapper.Map<IEnumerable<Book>>(pagedResultEntity.Data);
        return new PagedResult<Book> { Data = books, TotalCount = pagedResultEntity.TotalCount };
    }

    public async Task<PagedResult<Book>> Search(string query, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        var pagedResultEntity = await _bookRepository.Search(query, page_number, page_size);
        var books = _mapper.Map<IEnumerable<Book>>(pagedResultEntity.Data);
        return new PagedResult<Book> { Data = books, TotalCount = pagedResultEntity.TotalCount };
    }

    public async Task<PagedResult<Book>> GetBySeries(int id, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        var pagedResultEntity = await _bookRepository.GetBySeriesId(id, page_number, page_size);
        var books = _mapper.Map<IEnumerable<Book>>(pagedResultEntity.Data);
        return new PagedResult<Book> { Data = books, TotalCount = pagedResultEntity.TotalCount };
    }

    public async Task<PagedResult<Book>> GetByAuthor(int id, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        var pagedResultEntity = await _bookRepository.GetByAuthorId(id, page_number, page_size);
        var books = _mapper.Map<IEnumerable<Book>>(pagedResultEntity.Data);
        return new PagedResult<Book> { Data = books, TotalCount = pagedResultEntity.TotalCount };
    }


}