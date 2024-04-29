namespace WebApi.Services;

using AutoMapper;
using BCrypt.Net;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Repositories;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAll();
    Task<Book> GetById(int id);
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

    public async Task<IEnumerable<Book>> GetAll()
    {
        return await _bookRepository.GetAll();
    }

    public async Task<Book> GetById(int id)
    {
        var book = await _bookRepository.GetById(id);

        if (book == null)
            throw new KeyNotFoundException("Book not found");

        return book;
    }

}