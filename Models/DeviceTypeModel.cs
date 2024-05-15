namespace alexandria.api.Models;

public class DeviceTypeModel
{
    public long? Id { get; set; }
    public string? DeviceName { get; set; }
    public string? EbookDirectory { get; set; }
    public string? SavePathTemplate { get; set; }
    public string? Vendor { get; set; }
}