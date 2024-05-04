namespace alexandria.api.Entities;

using System.Text.Json.Serialization;

public class BookEntity
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Authors { get; set; }
    public string? AuthorsWithId { get; set; }
    public string? Series { get; set; }
    public long SeriesIndex { get; set; }
    public string? SeriesInfo { get; set; }
    public long? SeriesId { get; set; }
    public DateTime? DateAdded { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? MobiFullPath { get; set; }
    public string? Azw3FullPath { get; set; }
    public string? EpubFullPath { get; set; }
}