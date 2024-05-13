using System.Runtime.InteropServices;
using alexandria.api.Entities;
using alexandria.api.Helpers;

namespace alexandria.api.Services;

public interface IKnownDeviceService
{
    IEnumerable<KnownDevice> GetAll();
    KnownDevice GetById(long id);
    void Create(KnownDevice model);
    void Update(long id, KnownDevice model);
    void Delete(long id);
}

public class KnownDeviceService : IKnownDeviceService
{
    private readonly AppDataContext _context;

    public KnownDeviceService(AppDataContext context)
    {
        _context = context;
    }

    public IEnumerable<KnownDevice> GetAll()
    {
        return _context.KnownDevices.ToList();
    }

    public KnownDevice GetById(long id)
    {
        return _context.KnownDevices.Find(id);
    }

    public void Create(KnownDevice model)
    {
        var knownDevice = new KnownDevice()
        {
            DeviceName = model.DeviceName,
            EbookDirectory = model.EbookDirectory,
            SavePathTemplate = model.SavePathTemplate,
            Vendor = model.Vendor,
            SerialNumber = model.SerialNumber
        };

        _context.KnownDevices.Add(knownDevice);
        _context.SaveChanges();
    }

    public void Update(long id, KnownDevice model)
    {
        var knownDevice = _context.KnownDevices.Find(id);

        if (knownDevice == null) return;

        knownDevice.DeviceName = model.DeviceName;
        knownDevice.EbookDirectory = model.EbookDirectory;
        knownDevice.SavePathTemplate = model.SavePathTemplate;
        knownDevice.Vendor = model.Vendor;
        knownDevice.SerialNumber = model.SerialNumber;

        _context.KnownDevices.Update(knownDevice);
        _context.SaveChanges();
    }

    public void Delete(long id)
    {
        var knownDevice = _context.KnownDevices.Find(id);

        if (knownDevice == null) return;

        _context.KnownDevices.Remove(knownDevice);
        _context.SaveChanges();
    }
}