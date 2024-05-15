using alexandria.api.Entities;
using alexandria.api.Models;
using alexandria.api.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace alexandria.api.Services;

public interface IDeviceTypeService
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;
    Task<PagedResult<DeviceTypeModel>> GetAll(int page_number = DefaultPageNumber, int page_size = DefaultPageSize);
    Task<PagedResult<DeviceTypeModel>> Search(string query, int page_number = DefaultPageNumber, int page_size = DefaultPageSize);
    Task<PagedResult<DeviceTypeModel>> GetById(long id);
    Task Create(DeviceTypeModel model);
    Task Update(long id, DeviceTypeModel model);
    Task Delete(long id);
}

public class DeviceTypeService : IDeviceTypeService
{
    private readonly AppDataContext _context;
    private readonly IMapper _mapper;

    public DeviceTypeService(AppDataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<DeviceTypeModel>> GetAll(int page_number = IDeviceTypeService.DefaultPageNumber, int page_size = IDeviceTypeService.DefaultPageSize)
    {
        var deviceTypes = await _context.DeviceTypes
            .Skip((page_number - 1) * page_size)
            .Take(page_size)
            .ToListAsync();
        var totalCount = await _context.DeviceTypes.CountAsync();
        var data = _mapper.Map<List<DeviceTypeModel>>(deviceTypes);
        return new PagedResult<DeviceTypeModel> { Data = data, TotalCount = totalCount };
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.

    public async Task<PagedResult<DeviceTypeModel>> Search(string query, int page_number = IDeviceTypeService.DefaultPageNumber, int page_size = IDeviceTypeService.DefaultPageSize)
    {
        var deviceTypes = await _context.DeviceTypes
            .Where(x => x.DeviceName.ToLower().Contains(query) || x.Vendor.ToLower().Contains(query))
            .Skip((page_number - 1) * page_size)
            .Take(page_size)
            .ToListAsync();

        var totalCount = await _context.DeviceTypes
            .Where(x => x.DeviceName.ToLower().Contains(query) || x.Vendor.ToLower().Contains(query))
            .CountAsync();

        var data = _mapper.Map<List<DeviceTypeModel>>(deviceTypes);

        return new PagedResult<DeviceTypeModel> { Data = data, TotalCount = totalCount };
    }

#pragma warning restore CS8602 // Dereference of a possibly null reference.

    public async Task<PagedResult<DeviceTypeModel>> GetById(long id)
    {
        var deviceTypes = await _context.DeviceTypes.FindAsync(id);
        var data = _mapper.Map<DeviceTypeModel>(deviceTypes);
        return new PagedResult<DeviceTypeModel> { Data = [data], TotalCount = 1 };
    }

    public async Task Create(DeviceTypeModel model)
    {
        var deviceType = _mapper.Map<DeviceType>(model);
        _context.DeviceTypes.Add(deviceType);
        await _context.SaveChangesAsync();
    }

    public async Task Update(long id, DeviceTypeModel model)
    {
        var deviceType = await _context.DeviceTypes.FindAsync(id);
        if (deviceType == null) return;

        deviceType = _mapper.Map(model, deviceType);
        _context.DeviceTypes.Update(deviceType);

        await _context.SaveChangesAsync();
    }

    public async Task Delete(long id)
    {
        var deviceType = await _context.DeviceTypes.FindAsync(id);

        if (deviceType == null) return;

        _context.DeviceTypes.Remove(deviceType);
        await _context.SaveChangesAsync();
    }
}