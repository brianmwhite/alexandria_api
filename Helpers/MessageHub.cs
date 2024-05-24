namespace alexandria.api.Helpers;

using alexandria.api.Models;
using Microsoft.AspNetCore.SignalR;

public class MessageHub : Hub
{
    public async Task NotifyUSBDeviceEvent(DetectedDevice device)
        => await Clients.All.SendAsync("USBDeviceEvent", device);
}