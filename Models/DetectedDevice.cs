namespace alexandria.api.Models;

public class DetectedDevice
{
    public enum DeviceStateEnum
    {
        NO_DEVICE_DETECTED,
        MOUNTED,
        NOT_MOUNTED,
    }
    public enum MatchedStateEnum
    {
        NOT_MATCHED,
        MATCHED_KNOWN,
        MATCHED_TYPE,
    }

    public DeviceStateEnum DeviceState { get; set; }
    public MatchedStateEnum MatchedState { get; set; }

    public KnownDeviceModel? KnownDevice { get; set; }
    public DeviceTypeModel? DeviceType { get; set; }
    public USBDevice? USBDeviceInfo { get; set; }
}