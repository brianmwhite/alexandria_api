using System.Runtime.InteropServices;
using alexandria.api.Entities;
using alexandria.api.Models;
using alexandria.api.Helpers;
using AutoMapper;

namespace alexandria.api.Services;

public interface IKnownDeviceService
{
    IEnumerable<KnownDeviceModel> GetAll();
    KnownDeviceModel GetById(long id);
    void Create(KnownDeviceModel model);
    void Update(long id, KnownDeviceModel model);
    void Delete(long id);
}
public class KnownDeviceService : IKnownDeviceService
{
    private readonly AppDataContext _context;
    private readonly IMapper _mapper;

    public KnownDeviceService(AppDataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IEnumerable<KnownDeviceModel> GetAll()
    {
        var knownDevices = _context.KnownDevices.ToList();
        return _mapper.Map<List<KnownDeviceModel>>(knownDevices);
    }

    public KnownDeviceModel GetById(long id)
    {
        var knownDevice = _context.KnownDevices.Find(id);
        if (knownDevice == null) return null;

        return _mapper.Map<KnownDeviceModel>(knownDevice);
    }

    public void Create(KnownDeviceModel model)
    {
        var knownDevice = _mapper.Map<KnownDevice>(model);
        _context.KnownDevices.Add(knownDevice);
        _context.SaveChanges();
    }

    public void Update(long id, KnownDeviceModel model)
    {
        var knownDevice = _context.KnownDevices.Find(id);
        if (knownDevice == null) return;

        _mapper.Map(model, knownDevice);
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