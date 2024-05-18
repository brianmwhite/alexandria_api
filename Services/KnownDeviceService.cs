using alexandria.api.Entities;
using alexandria.api.Models;
using alexandria.api.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace alexandria.api.Services;

public interface IKnownDeviceService
{
    const int DefaultPageNumber = 1;
    const int DefaultPageSize = 10;
    Task<PagedResult<KnownDeviceModel>> GetAll(int page_number = DefaultPageNumber, int page_size = DefaultPageSize);
    Task<PagedResult<KnownDeviceModel>> Search(string query, int page_number = DefaultPageNumber, int page_size = DefaultPageSize);
    Task<PagedResult<KnownDeviceModel>> GetById(long id);
    Task Create(KnownDeviceModel model);
    Task Update(long id, KnownDeviceModel model);
    Task Delete(long id);
    Task<KnownDeviceModel?> DetectDevice();
}
public class KnownDeviceService : IKnownDeviceService
{
    private readonly AppDataContext _context;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public KnownDeviceService(AppDataContext context, IMapper mapper, IFileService fileService)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
    }

    public async Task<PagedResult<KnownDeviceModel>> GetAll(int page_number = IKnownDeviceService.DefaultPageNumber, int page_size = IKnownDeviceService.DefaultPageSize)
    {
        var knownDevices = await _context.KnownDevices
            .Skip((page_number - 1) * page_size)
            .Take(page_size)
            .ToListAsync();
        var totalCount = await _context.KnownDevices.CountAsync();
        var devices = _mapper.Map<List<KnownDeviceModel>>(knownDevices);
        return new PagedResult<KnownDeviceModel> { Data = devices, TotalCount = totalCount };
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.

    public async Task<PagedResult<KnownDeviceModel>> Search(string query, int page_number = IKnownDeviceService.DefaultPageNumber, int page_size = IKnownDeviceService.DefaultPageSize)
    {
        query = query.ToLower();

        var knownDevices = await _context.KnownDevices
            .Where(x => x.DeviceName.ToLower().Contains(query) || x.Vendor.ToLower().Contains(query))
            .Skip((page_number - 1) * page_size)
            .Take(page_size)
            .ToListAsync();

        var totalCount = await _context.KnownDevices
            .Where(x => x.DeviceName.ToLower().Contains(query) || x.Vendor.ToLower().Contains(query))
            .CountAsync();

        var devices = _mapper.Map<List<KnownDeviceModel>>(knownDevices);

        return new PagedResult<KnownDeviceModel> { Data = devices, TotalCount = totalCount };
    }

#pragma warning restore CS8602 // Dereference of a possibly null reference.

    public async Task<PagedResult<KnownDeviceModel>> GetById(long id)
    {
        var knownDevice = await _context.KnownDevices.FindAsync(id);
        var device = _mapper.Map<KnownDeviceModel>(knownDevice);
        return new PagedResult<KnownDeviceModel> { Data = [device], TotalCount = 1 };
    }

    public async Task Create(KnownDeviceModel model)
    {
        var knownDevice = _mapper.Map<KnownDevice>(model);

        if (model.DeviceTypeId != null)
        {
            var deviceType = await _context.DeviceTypes.FindAsync(model.DeviceTypeId);
            knownDevice.DeviceType = deviceType;
        }

        _context.KnownDevices.Add(knownDevice);
        await _context.SaveChangesAsync();
    }

    public async Task Update(long id, KnownDeviceModel model)
    {
        var knownDevice = await _context.KnownDevices.FindAsync(id);
        if (knownDevice == null) return;

        knownDevice = _mapper.Map(model, knownDevice);

        if (model.DeviceTypeId != null)
        {
            var deviceType = await _context.DeviceTypes.FindAsync(model.DeviceTypeId);
            knownDevice.DeviceType = deviceType;
        }

        _context.KnownDevices.Update(knownDevice);

        await _context.SaveChangesAsync();
    }

    public async Task Delete(long id)
    {
        var knownDevice = await _context.KnownDevices.FindAsync(id);

        if (knownDevice == null) return;

        _context.KnownDevices.Remove(knownDevice);
        await _context.SaveChangesAsync();
    }

    public async Task<KnownDeviceModel?> DetectDevice()
    {
        var usbDevice = await _fileService.CheckUSBDeviceInformation();

        if (usbDevice == null) return null;
        if (usbDevice.First<USBDevice>() == null) return null;

        var vendor = usbDevice.First<USBDevice>().Vendor ?? null;
        var serial = usbDevice.First<USBDevice>().Serial ?? null;

        if (vendor == null || serial == null) return null;

        var knownDevice = await _context.KnownDevices
            .Where(x => x.Vendor == vendor && x.SerialNumber == serial)
            .FirstOrDefaultAsync();

        if (knownDevice == null) return null;

        var knownDeviceModel = _mapper.Map<KnownDeviceModel>(knownDevice);
        knownDeviceModel.USBDeviceInfo = usbDevice.First<USBDevice>();

        return knownDeviceModel;
    }
}
