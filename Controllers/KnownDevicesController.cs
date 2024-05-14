namespace alexandria.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using alexandria.api.Services;
using alexandria.api.Entities;
using alexandria.api.Models;

[ApiController]
[Route("[controller]")]
public class KnownDevicesController : ControllerBase
{
    private IKnownDeviceService _knownDeviceService;

    public KnownDevicesController(IKnownDeviceService knownDeviceService)
    {
        _knownDeviceService = knownDeviceService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var knownDevices = _knownDeviceService.GetAll();
        return Ok(knownDevices);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var knownDevice = _knownDeviceService.GetById(id);
        return Ok(knownDevice);
    }

    [HttpPost]
    public IActionResult Create([FromBody] KnownDeviceModel model)
    {
        _knownDeviceService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] KnownDeviceModel model)
    {
        _knownDeviceService.Update(id, model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _knownDeviceService.Delete(id);
        return Ok();
    }
}