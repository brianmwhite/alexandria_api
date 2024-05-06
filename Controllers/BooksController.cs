namespace alexandria.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using alexandria.api.Services;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;
    private IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] int page = DefaultPageNumber, [FromQuery] int limit = DefaultPageSize)
    {
        if (string.IsNullOrEmpty(search))
        {
            var books = await _bookService.GetAll(page, limit);
            return Ok(books);
        }
        else
        {
            var books = await _bookService.Search(search, page, limit);
            return Ok(books);
        }
    }

    [HttpGet("series/{id}")]
    public async Task<IActionResult> Series([FromRoute] int id, [FromQuery] int page = DefaultPageNumber, [FromQuery] int limit = DefaultPageSize)
    {
        var books = await _bookService.GetBySeries(id, page, limit);
        return Ok(books);
    }

    [HttpGet("author/{id}")]
    public async Task<IActionResult> Author([FromRoute] int id, [FromQuery] int page = DefaultPageNumber, [FromQuery] int limit = DefaultPageSize)
    {
        var books = await _bookService.GetByAuthor(id, page, limit);
        return Ok(books);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _bookService.GetById(id);
        return Ok(book);
    }

    public class TransferRequest
    {
        public int Id { get; set; }
        private string _format = "";

        public string Format
        {
            get { return _format; }
            set { _format = value?.ToUpper() ?? string.Empty; }
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        await _bookService.TransferBookFile(request.Id, request.Format);
        return Ok();
    }
}