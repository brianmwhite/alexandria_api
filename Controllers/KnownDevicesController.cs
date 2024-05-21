namespace alexandria.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using alexandria.api.Services;
using alexandria.api.Models;

[ApiController]
[Route("[controller]")]
public class KnownDevicesController : ControllerBase
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;
    private IKnownDeviceService _knownDeviceService;
    private IFileService _fileService;

    public KnownDevicesController(IKnownDeviceService knownDeviceService, IFileService fileService)
    {
        _knownDeviceService = knownDeviceService;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? search, int page = DefaultPageNumber, [FromQuery] int limit = DefaultPageSize)
    {
        if (string.IsNullOrEmpty(search))
        {
            var knownDevices = await _knownDeviceService.GetAll(page, limit);
            return Ok(knownDevices);
        }
        else
        {
            var knownDevices = await _knownDeviceService.Search(search, page, limit);
            return Ok(knownDevices);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var knownDevice = await _knownDeviceService.GetById(id);
        return Ok(knownDevice);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] KnownDeviceModel model)
    {
        await _knownDeviceService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] KnownDeviceModel model)
    {
        await _knownDeviceService.Update(id, model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _knownDeviceService.Delete(id);
        return Ok();
    }

    [HttpGet("detect")]
    public async Task<IActionResult> DetectDevice()
    {
        var knownDevice = await _knownDeviceService.DetectDevice();
        return Ok(knownDevice);
    }

    //post method to unmount the usbdrive
    [HttpPost("unmount")]
    public async Task<IActionResult> UnmountDevice()
    {
        await _fileService.UnmountUSBDevice();
        return Ok();
    }
}