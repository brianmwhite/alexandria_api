// ❯ lsblk --output vendor,model,serial,name,path,label,mountpoint,uuid,fsavail,fssize,fsused,fsuse% --include 8 --json
// {
//    "blockdevices": [
//       {"vendor":"Kindle  ", "model":"Internal_Storage", "serial":"G000S109740403H0", "name":"sdb", "path":"/dev/sdb", "label":"Kindle", "mountpoint":"/media/usb0", "uuid":"6578-753B", "fsavail":"27.1G", "fssize":"27.3G", "fsused":"228M", "fsuse%":"1%"}
//    ]
// }
namespace alexandria.api.Models;

public class USBDevice
{
    public string? Vendor { get; set; }
    public string? Model { get; set; }
    public string? Serial { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
    public string? Label { get; set; }
    public string? Mountpoint { get; set; }
    public string? UUID { get; set; }
    public string? FSavail { get; set; }
    public string? FSsize { get; set; }
    public string? FSused { get; set; }
    public string? FSuse { get; set; }
}
