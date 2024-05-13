namespace alexandria.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using alexandria.api.Services;
using alexandria.api.Entities;

[ApiController]
[Route("[controller]")]
public class DeviceTypesController : ControllerBase
{
    private IDeviceTypeService _deviceTypeService;

    public DeviceTypesController(IDeviceTypeService deviceTypeService)
    {
        _deviceTypeService = deviceTypeService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var deviceTypes = _deviceTypeService.GetAll();
        return Ok(deviceTypes);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var deviceType = _deviceTypeService.GetById(id);
        return Ok(deviceType);
    }

    [HttpPost]
    public IActionResult Create([FromBody] DeviceType model)
    {
        _deviceTypeService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] DeviceType model)
    {
        _deviceTypeService.Update(id, model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _deviceTypeService.Delete(id);
        return Ok();
    }
}