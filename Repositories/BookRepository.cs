namespace WebApi.Repositories;

using Dapper;
using WebApi.Entities;
using WebApi.Helpers;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAll();
    Task<Book> GetById(int id);
}

public class BookRepository : IBookRepository
{
    private DataContext _context;

    public BookRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAll()
    {
        using var connection = _context.CreateConnection();
        var sql = """
            SELECT * FROM books ORDER BY last_modified DESC LIMIT 10
        """;
        return await connection.QueryAsync<Book>(sql);
    }

    public async Task<Book> GetById(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = @"
            SELECT * FROM book 
            WHERE id = @id
        ";
        var result = await connection.QuerySingleOrDefaultAsync<Book>(sql, new { id });
        return result ?? new Book();
    }
}