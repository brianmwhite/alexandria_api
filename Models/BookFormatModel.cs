namespace alexandria.api.Models;

public class BookFormatModel
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Authors { get; set; }
    public bool SupportedFormat { get; set; }
    public string BookFilePath { get; set; }

    public BookFormatModel()
    {
        Id = 0;
        Title = "No Book Found";
        Authors = "No Authors Found";
        SupportedFormat = false;
        BookFilePath = "";
    }
}