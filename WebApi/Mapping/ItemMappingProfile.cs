using AutoMapper;
using WebApi.Entity;
using WebApi.Model;

namespace WebApi.Mapping;

public class ItemMappingProfile : Profile
{
    public ItemMappingProfile()
    {
        CreateMap<Item, ItemDto>();
    }
}