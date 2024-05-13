namespace alexandria.api.Entities
{
    public class KnownDevice
    {
        public long Id { get; set; }
        public required string DeviceName { get; set; }
        public required string EbookDirectory { get; set; }
        public required string SavePathTemplate { get; set; }
        public string? Vendor { get; set; }
        // public string? ProductId { get; set; }
        public string? SerialNumber { get; set; }
    }
}
