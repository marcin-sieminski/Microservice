using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Services.Models;

namespace Services.Services;

public class ItemService(ItemContext context, IMapper mapper) : IItemService
{
    public async Task<List<ItemModel>> GetAll()
    {
        var itemEntities = await context.Items.ToListAsync();
        var itemModels = mapper.Map<List<ItemModel>>(itemEntities);
        return itemModels;
    }

    public async Task<ItemModel?> GetById(int id)
    {
        var itemEntity = await context.Items.FirstOrDefaultAsync(x => x.Id == id);
        return itemEntity is null ? null : mapper.Map<ItemModel>(itemEntity);
    }

    public async Task<ItemModel?> Create(ItemModel item)
    {
        var itemEntity = mapper.Map<Item>(item);
        itemEntity.CreatedAt = DateTime.UtcNow;
        itemEntity.Version = 1;
        context.Items.Add(itemEntity);
        await context.SaveChangesAsync();
        return mapper.Map<ItemModel>(itemEntity);
    }

    public async Task<ItemModel?> Update(int id, ItemModel item)
    {
        var itemEntity = await context.Items.FindAsync(id);
        if (itemEntity is null)
        {
            return null;
        }
        itemEntity.Name = item.Name;
        itemEntity.Description = item.Description;
        itemEntity.Version += 1;
        await context.SaveChangesAsync();
        return mapper.Map<ItemModel>(itemEntity);
    }

    public async Task<bool> Delete(int id)
    {
        if (await context.Items.FindAsync(id) is not Item item) return false;
        context.Items.Remove(item);
        await context.SaveChangesAsync();
        return true;
    }
}