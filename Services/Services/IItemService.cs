using Services.Models;

namespace Services.Services;

public interface IItemService
{
    Task<List<ItemModel>> GetAll();
    Task<ItemModel?> GetById(int id);
    Task<ItemModel?> Create(ItemModel item);
    Task<ItemModel?> Update(int id, ItemModel item);
    Task<bool> Delete(int id);
}