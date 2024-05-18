namespace alexandria.api.Helpers;

using alexandria.api.Entities;
using alexandria.api.Models;
using AutoMapper;

public class AuthorsResolver : IValueResolver<BookEntity, BookModel, List<AuthorModel>?>
{
    public List<AuthorModel>? Resolve(BookEntity source, BookModel dest, List<AuthorModel>? destMember, ResolutionContext context)
    {
        var authors = new List<AuthorModel>();
        if (!string.IsNullOrEmpty(source.AuthorsWithId))
        {
            var authorPart = source.AuthorsWithId.Split('|');
            var numberOfParts = authorPart.Length / 2;
            for (var i = 0; i < numberOfParts; i++)
            {
                authors.Add(new AuthorModel
                {
                    Id = int.Parse(authorPart[i * 2 + 1]),
                    Name = authorPart[i * 2]
                });
            }
        }
        return authors;
    }
}

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<BookEntity, BookModel>()
            .ForMember(dest => dest.AuthorList, opt => opt.MapFrom<AuthorsResolver>());

        CreateMap<DeviceType, DeviceTypeModel>();

        CreateMap<DeviceTypeModel, DeviceType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<KnownDevice, KnownDeviceModel>()
            .ForMember(dest => dest.DeviceTypeId, opt => opt.MapFrom(src => src.DeviceType != null ? src.DeviceType.Id : (long?)null))
            .ForMember(dest => dest.FormatList, opt => opt.Ignore())
            .ForMember(dest => dest.USBDeviceInfo, opt => opt.Ignore());

        CreateMap<KnownDeviceModel, KnownDevice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DeviceType, opt => opt.Ignore());

    }
}

