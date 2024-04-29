namespace WebApi.Entities;

using System.Text.Json.Serialization;

public class Book
{
    public int Id { get; set; }
    public string? Title { get; set; }

    public Book()
    {
        Id = 0; // Default value for Id
        Title = "No Book Found"; // Default value for Title
    }
}