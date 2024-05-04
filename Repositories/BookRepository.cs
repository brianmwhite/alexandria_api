namespace alexandria.api.Repositories;

using Dapper;
using alexandria.api.Entities;
using alexandria.api.Helpers;

public interface IBookRepository
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;
    Task<IEnumerable<BookEntity>> GetAll(int page_number = DefaultPageNumber, int page_size = DefaultPageSize);
    Task<IEnumerable<BookEntity>> GetBySeriesId(int id, int page_number = DefaultPageNumber, int page_size = DefaultPageSize);
    Task<IEnumerable<BookEntity>> GetByAuthorId(int id, int page_number = DefaultPageNumber, int page_size = DefaultPageSize);
    Task<BookEntity> GetById(int id);
    Task<IEnumerable<BookEntity>> Search(string query, int page_number = DefaultPageNumber, int page_size = DefaultPageSize);
}

public class BookRepository(DataContext context) : IBookRepository
{
    private const string book_query = """
    WITH book_info AS (
    SELECT b.id AS book_id, 
           b.title, 
           GROUP_CONCAT(a.name, ', ') AS Authors, 
           GROUP_CONCAT(a.name || '|' || a.id, '|') AS AuthorsWithId, 
           s.name AS series_names,
		   bsl_min.series as seriesId,
           b.series_index,
		   b.pubdate,
		   b.last_modified,
		   b.timestamp,
		   b.path
    FROM books AS b
    LEFT JOIN books_authors_link AS bal ON b.id = bal.book
    LEFT JOIN authors AS a ON bal.author = a.id
    LEFT JOIN (
        SELECT book, MIN(series) as series
        FROM books_series_link as bsl
        GROUP BY book
    ) AS bsl_min ON b.id = bsl_min.book
    LEFT JOIN series AS s ON bsl_min.series = s.id
    {0}	GROUP BY b.id
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
        bi.Authors, 
		bi.AuthorsWithId,
        bi.series_index as SeriesIndex,
		bi.series_names as Series,
		bi.seriesId as SeriesId,
        COALESCE(bi.series_names || ' [' || (CASE WHEN bi.series_index = CAST(bi.series_index AS INTEGER) THEN CAST(bi.series_index AS INTEGER) ELSE bi.series_index END) || ']', '') as SeriesInfo,
		datetime(bi.timestamp) as DateAdded,
		datetime(bi.pubdate) as PublicationDate,
        bi.path || '/' || fi.mobi_name || '.mobi' AS MobiFullPath,
        bi.path || '/' || fi.azw3_name || '.azw3' AS Azw3FullPath,
        bi.path || '/' || fi.epub_name || '.epub' AS EpubFullPath
    FROM book_info AS bi
    LEFT JOIN format_info AS fi ON bi.book_id = fi.book
   	{1}
    LIMIT @page_size OFFSET @page_size * (@page_number - 1)
""";
    private const string book_query_search_where_clause = """
    WHERE (b.title LIKE COALESCE('%' || @title || '%', b.title) OR 
       a.name LIKE COALESCE('%' || @author || '%', a.name) OR 
       s.name LIKE COALESCE('%' || @series || '%', s.name))
""";
    private const string book_query_id_where_clause = "WHERE b.id = @id";
    private const string book_query_series_id_where_clause = "WHERE s.id = @id";
    private const string book_query_author_id_where_clause = "WHERE a.id = @id";
    private const string book_query_search_order_by_clause = "ORDER by bi.timestamp DESC";
    private const string book_query_series_order_by_clause = "ORDER BY bi.series_index ASC";
    private const string book_query_author_order_by_clause = "ORDER BY bi.pubdate DESC";

    private DataContext _context = context;

    public async Task<IEnumerable<BookEntity>> GetAll(int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        using var connection = _context.CreateConnection();
        var sql = string.Format(book_query, "", book_query_search_order_by_clause);
        return await connection.QueryAsync<BookEntity>(sql, new { page_number, page_size });
    }

    public async Task<BookEntity> GetById(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = string.Format(book_query, book_query_id_where_clause, "");
        var result = await connection.QuerySingleOrDefaultAsync<BookEntity>(sql, new { id, page_size = IBookRepository.DefaultPageSize, page_number = IBookRepository.DefaultPageNumber });
        return result ?? new BookEntity();
    }

    public async Task<IEnumerable<BookEntity>> GetBySeriesId(int id, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        using var connection = _context.CreateConnection();
        var sql = string.Format(book_query, book_query_series_id_where_clause, book_query_series_order_by_clause);
        return await connection.QueryAsync<BookEntity>(sql, new { id, page_number, page_size });
    }

    public async Task<IEnumerable<BookEntity>> GetByAuthorId(int id, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        using var connection = _context.CreateConnection();
        var sql = string.Format(book_query, book_query_author_id_where_clause, book_query_author_order_by_clause);
        return await connection.QueryAsync<BookEntity>(sql, new { id, page_number, page_size });
    }

    public async Task<IEnumerable<BookEntity>> Search(string query, int page_number = IBookRepository.DefaultPageNumber, int page_size = IBookRepository.DefaultPageSize)
    {
        using var connection = _context.CreateConnection();
        var sql = string.Format(book_query, book_query_search_where_clause, book_query_search_order_by_clause);
        return await connection.QueryAsync<BookEntity>(sql, new { title = query, author = query, series = query, page_size, page_number });
    }
}