using Microsoft.AspNetCore.Mvc;
using WebApi.Entity;

namespace WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class ItemController(ItemContext dbContext) : ControllerBase
{
    private readonly ItemContext _dbContext = dbContext;

    public ActionResult<IEnumerable<Item>> GetAll()
    {
        var items = _dbContext.Items.ToList();
        return items;
    }
}