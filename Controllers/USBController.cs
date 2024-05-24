namespace alexandria.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using alexandria.api.Services;
using System.Runtime.CompilerServices;
using alexandria.api.Helpers;
using alexandria.api.Models;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("[controller]")]
public class USBController : ControllerBase
{
    private IFileService _fileService;
    private IKnownDeviceService _knownDeviceService;
    private readonly IHubContext<MessageHub> _hubContext;

    public USBController(IFileService fileService, IKnownDeviceService knownDeviceService, IHubContext<MessageHub> hubContext)
    {
        _fileService = fileService;
        _knownDeviceService = knownDeviceService;
        _hubContext = hubContext;
    }
    public class DeviceStateModel
    {
        public string? State { get; set; }
    }
    // curl -X POST "http://HOST:PORT/usb/notify"
    [HttpPost("notify")]
    public async Task<IActionResult> NotifyDeviceEvent([FromBody] DeviceStateModel? deviceState = null)
    {
        var detectedDevice = new DetectedDevice();

        if (deviceState != null && deviceState.State != null)
        {
            if (deviceState.State == "MATCHED")
            {
                detectedDevice.State = DetectedDevice.StateEnum.MATCHED;
            }
            else if (deviceState.State == "NOT_MATCHED")
            {
                detectedDevice.State = DetectedDevice.StateEnum.NOT_MATCHED;
            }
            else
            {
                detectedDevice.State = DetectedDevice.StateEnum.NOT_DETECTED;
            }
        }
        else
        {
            detectedDevice = await _knownDeviceService.DetectDevice();
        }

        await _hubContext.Clients.All.SendAsync("USBDeviceEvent", detectedDevice);

        return Ok();
    }

    //post method to unmount the usbdrive
    [HttpPost("unmount")]
    public async Task<IActionResult> UnmountDevice()
    {
        await _fileService.UnmountUSBDevice();
        return Ok();
    }
}