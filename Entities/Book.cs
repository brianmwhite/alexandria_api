namespace alexandria.api.Entities;

using System.Text.Json.Serialization;

public class Author
{
    public long? Id { get; set; }
    public string? Name { get; set; }
}

public class Book
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Authors { get; set; }
    public List<Author>? AuthorList { get; set; }
    public string? SeriesInfo { get; set; }
    public long? SeriesId { get; set; }
    public DateTime? DateAdded { get; set; }
    public DateTime? PublicationDate { get; set; }
    public bool HasMobi { get; set; }
    public bool HasAzw3 { get; set; }
    public bool HasEpub { get; set; }

    public Book()
    {
        Id = 0;
        Title = "No Book Found";
        Authors = "Unknown";

        SeriesInfo = null;

        DateAdded = null;
        PublicationDate = null;
    }
}