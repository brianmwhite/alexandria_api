namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

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
    public async Task<IActionResult> Search([FromQuery] string? search)
    {
        if (string.IsNullOrEmpty(search))
        {
            var books = await _bookService.GetAll();
            return Ok(books);
        }
        else
        {
            var books = await _bookService.Search(search);
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