namespace alexandria.api.Repositories;

using Dapper;
using alexandria.api.Entities;
using alexandria.api.Helpers;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAll(int page_number = 1, int page_size = 10);
    Task<Book> GetById(int id);
    Task<IEnumerable<Book>> Search(string query, int page_number = 1, int page_size = 10);
}

public class BookRepository : IBookRepository
{
    private const string book_query = """
    WITH book_info AS (
    SELECT b.id AS book_id, 
           b.title, 
           GROUP_CONCAT(a.name, ', ') AS authors, 
           s.name AS series_names,
           b.series_index,
		   b.last_modified,
		   b.path
    FROM books AS b
    LEFT JOIN books_authors_link AS bal ON b.id = bal.book
    LEFT JOIN authors AS a ON bal.author = a.id
    LEFT JOIN (
        SELECT book, MIN(series) as series
        FROM books_series_link
        GROUP BY book
    ) AS bsl_min ON b.id = bsl_min.book
    LEFT JOIN series AS s ON bsl_min.series = s.id
    {0}
    GROUP BY b.id
    ),
    format_info AS (
        SELECT book, 
            MAX(CASE WHEN format = 'MOBI' THEN name END) AS mobi_name,
            MAX(CASE WHEN format = 'AZW3' THEN name END) AS azw3_name,
            MAX(CASE WHEN format = 'EPUB' THEN name END) AS epub_name
        FROM DATA
        WHERE format IN ('MOBI', 'AZW3', 'EPUB')
        GROUP BY book
    )
    SELECT 
	    bi.book_id as id,
        bi.title, 
        bi.authors, 
        bi.series_names as Series, 
        bi.series_index as SeriesIndex,
        datetime(bi.last_modified) as LastModified, 
        bi.path || '/' || fi.mobi_name || '.mobi' AS MobiFullPath,
        bi.path || '/' || fi.azw3_name || '.azw3' AS Azw3FullPath,
        bi.path || '/' || fi.epub_name || '.epub' AS EpubFullPath
    FROM book_info AS bi
    LEFT JOIN format_info AS fi ON bi.book_id = fi.book
   	ORDER BY bi.last_modified DESC
    LIMIT @page_size OFFSET @page_size * (@page_number - 1)
""";
    private const string book_query_search_where_clause = """
    WHERE (b.title LIKE COALESCE('%' || @title || '%', b.title) OR 
       a.name LIKE COALESCE('%' || @author || '%', a.name) OR 
       s.name LIKE COALESCE('%' || @series || '%', s.name))
""";
    private const string book_query_id_where_clause = """
    WHERE b.id = @id
""";


    private DataContext _context;

    public BookRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAll(int page_number = 1, int page_size = 10)
    {
        using var connection = _context.CreateConnection();
        var sql = string.Format(book_query, "");
        return await connection.QueryAsync<Book>(sql, new { page_number, page_size });
    }

    public async Task<Book> GetById(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = string.Format(book_query, book_query_id_where_clause);
        var result = await connection.QuerySingleOrDefaultAsync<Book>(sql, new { id, page_size = 1, page_number = 1 });
        return result ?? new Book();
    }

    public async Task<IEnumerable<Book>> Search(string query, int page_number = 1, int page_size = 10)
    {
        using var connection = _context.CreateConnection();
        var sql = string.Format(book_query, book_query_search_where_clause);
        return await connection.QueryAsync<Book>(sql, new { title = query, author = query, series = query, page_size, page_number });
    }
}