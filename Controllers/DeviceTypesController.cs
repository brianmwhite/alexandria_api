namespace alexandria.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using alexandria.api.Services;
using alexandria.api.Models;

[ApiController]
[Route("[controller]")]
public class DeviceTypesController : ControllerBase
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;
    private IDeviceTypeService _deviceTypeService;

    public DeviceTypesController(IDeviceTypeService deviceTypeService)
    {
        _deviceTypeService = deviceTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? search, int page = DefaultPageNumber, [FromQuery] int limit = DefaultPageSize)
    {
        if (string.IsNullOrEmpty(search))
        {
            var deviceTypes = await _deviceTypeService.GetAll(page, limit);
            return Ok(deviceTypes);
        }
        else
        {
            var deviceTypes = await _deviceTypeService.Search(search, page, limit);
            return Ok(deviceTypes);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var deviceTypes = await _deviceTypeService.GetById(id);
        return Ok(deviceTypes);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceTypeModel model)
    {
        await _deviceTypeService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DeviceTypeModel model)
    {
        await _deviceTypeService.Update(id, model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _deviceTypeService.Delete(id);
        return Ok();
    }
}