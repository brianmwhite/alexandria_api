namespace WebApi.Entities;

using System.Text.Json.Serialization;

public class Book
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Authors { get; set; }
    public string? Series { get; set; }
    public int SeriesIndex { get; set; }
    public DateTime? LastModified { get; set; }
    public string? MobiFullPath { get; set; }
    public string? Azw3FullPath { get; set; }
    public string? EpubFullPath { get; set; }

    public Book()
    {
        Id = 0;
        Title = "No Book Found";
        Title = "Unknown";
        Series = null;
        SeriesIndex = 1;
        LastModified = null;
        MobiFullPath = null;
        Azw3FullPath = null;
        EpubFullPath = null;
    }
}