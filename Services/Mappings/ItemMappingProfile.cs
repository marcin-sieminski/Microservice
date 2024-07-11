using AutoMapper;
using DataAccess.Entities;
using Services.Models;

namespace Services.Mappings;

public class ItemMappingProfile : Profile
{
    public ItemMappingProfile()
    {
        CreateMap<Item, ItemModel>();
        CreateMap<ItemModel, Item>();
    }
}