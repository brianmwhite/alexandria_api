namespace alexandria.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using alexandria.api.Services;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int limit = 10)
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _bookService.GetById(id);
        return Ok(book);
    }
}