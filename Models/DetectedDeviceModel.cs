namespace alexandria.api.Models;

public class DetectedDevice
{
    public enum StateEnum
    {
        NOT_DETECTED,
        NOT_MATCHED,
        MATCHED
    }
    public StateEnum State { get; set; }
    public KnownDeviceModel? KnownDevice { get; set; }
    public DeviceTypeModel? DeviceType { get; set; }
    public USBDevice? USBDeviceInfo { get; set; }
}