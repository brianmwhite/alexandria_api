namespace alexandria.api.Services;

using AutoMapper;
using BCrypt.Net;
using alexandria.api.Entities;
using alexandria.api.Helpers;
using alexandria.api.Repositories;

public interface IBookService
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;
    Task<IEnumerable<Book>> GetAll(int page_number, int page_size);
    Task<Book> GetById(int id);
    Task<IEnumerable<Book>> Search(string query, int page_number, int page_size);
    Task<IEnumerable<Book>> GetBySeries(int id, int page_number, int page_size);
    Task<IEnumerable<Book>> GetByAuthor(int id, int page_number, int page_size);
}

public class BookService(
    IBookRepository bookRepository,
    IMapper mapper) : IBookService
{
    private IBookRepository _bookRepository = bookRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<Book>> GetAll(int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        return await _bookRepository.GetAll(page_number, page_size);
    }

    public async Task<Book> GetById(int id)
    {
        var book = await _bookRepository.GetById(id);

        if (book == null)
            throw new KeyNotFoundException("Book not found");

        return book;
    }

    public async Task<IEnumerable<Book>> Search(string query, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        return await _bookRepository.Search(query, page_number, page_size);
    }

    public async Task<IEnumerable<Book>> GetBySeries(int id, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        return await _bookRepository.GetBySeriesId(id, page_number, page_size);
    }

    public async Task<IEnumerable<Book>> GetByAuthor(int id, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        return await _bookRepository.GetByAuthorId(id, page_number, page_size);
    }


}