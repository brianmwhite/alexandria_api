namespace alexandria.api.Models;

public class BookModel
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Authors { get; set; }
    public List<AuthorModel>? AuthorList { get; set; }
    public string? SeriesInfo { get; set; }
    public long? SeriesId { get; set; }
    public DateTime? DateAdded { get; set; }
    public DateTime? PublicationDate { get; set; }
    public bool HasMobi { get; set; }
    public bool HasAzw3 { get; set; }
    public bool HasEpub { get; set; }

    public BookModel()
    {
        Id = 0;
        Title = "No Book Found";
        Authors = "Unknown";

        SeriesInfo = null;

        DateAdded = null;
        PublicationDate = null;
    }
}