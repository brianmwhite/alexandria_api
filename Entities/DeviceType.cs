namespace alexandria.api.Entities;
public class DeviceType
{
    public long Id { get; set; }
    public required string DeviceName { get; set; }
    public required string EbookDirectory { get; set; }
    public required string SavePathTemplate { get; set; }
    public required string Vendor { get; set; }
    public string? Formats { get; set; }
    public ICollection<KnownDevice> KnownDevices { get; } = [];
}