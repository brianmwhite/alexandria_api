namespace alexandria.api.Services;

using AutoMapper;
using BCrypt.Net;
using alexandria.api.Entities;
using alexandria.api.Helpers;
using alexandria.api.Repositories;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAll(int page_number, int page_size);
    Task<Book> GetById(int id);
    Task<IEnumerable<Book>> Search(string query, int page_number, int page_size);
}

public class BookService : IBookService
{
    private IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public BookService(
        IBookRepository bookRepository,
        IMapper mapper)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Book>> GetAll(int page_number = 1, int page_size = 10)
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

    public async Task<IEnumerable<Book>> Search(string query, int page_number = 1, int page_size = 10)
    {
        return await _bookRepository.Search(query, page_number, page_size);
    }

}