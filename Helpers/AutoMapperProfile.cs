namespace alexandria.api.Helpers;

using alexandria.api.Entities;
using AutoMapper;

public class AuthorsResolver : IValueResolver<BookEntity, Book, List<Author>?>
{
    public List<Author>? Resolve(BookEntity source, Book dest, List<Author>? destMember, ResolutionContext context)
    {
        var authors = new List<Author>();
        if (!string.IsNullOrEmpty(source.AuthorsWithId))
        {
            var authorPart = source.AuthorsWithId.Split('|');
            var numberOfParts = authorPart.Length / 2;
            for (var i = 0; i < numberOfParts; i++)
            {
                authors.Add(new Author
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
        CreateMap<BookEntity, Book>()
            .ForMember(dest => dest.AuthorList, opt => opt.MapFrom<AuthorsResolver>());
    }
}

