using System.Collections.Generic;
using System.Linq;
using alexandria.api.Entities;
using alexandria.api.Helpers;

namespace alexandria.api.Services;

public interface IDeviceTypeService
{
    IEnumerable<DeviceType> GetAll();
    DeviceType GetById(long id);
    void Create(DeviceType model);
    void Update(long id, DeviceType model);
    void Delete(long id);
}

public class DeviceTypeService : IDeviceTypeService
{
    private readonly AppDataContext _context;

    public DeviceTypeService(AppDataContext context)
    {
        _context = context;
    }

    public IEnumerable<DeviceType> GetAll()
    {
        return _context.DeviceTypes.ToList();
    }

    public DeviceType GetById(long id)
    {
        return _context.DeviceTypes.Find(id);
    }

    public void Create(DeviceType model)
    {
        var deviceType = new DeviceType()
        {
            DeviceName = model.DeviceName,
            EbookDirectory = model.EbookDirectory,
            SavePathTemplate = model.SavePathTemplate,
            Vendor = model.Vendor,
        };

        _context.DeviceTypes.Add(deviceType);
        _context.SaveChanges();
    }

    public void Update(long id, DeviceType model)
    {
        var deviceType = _context.DeviceTypes.Find(id);

        if (deviceType == null) return;

        deviceType.DeviceName = model.DeviceName;
        deviceType.EbookDirectory = model.EbookDirectory;
        deviceType.SavePathTemplate = model.SavePathTemplate;
        deviceType.Vendor = model.Vendor;

        _context.SaveChanges();
    }

    public void Delete(long id)
    {
        var deviceType = _context.DeviceTypes.Find(id);

        if (deviceType == null) return;

        _context.DeviceTypes.Remove(deviceType);
        _context.SaveChanges();
    }
}