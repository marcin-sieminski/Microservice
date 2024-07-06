using Microsoft.AspNetCore.Mvc;
using WebApi.Entity;

namespace WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class ItemController(ItemContext dbContext) : ControllerBase
{
    private readonly ItemContext _dbContext = dbContext;

    [HttpGet]
    public ActionResult<IEnumerable<Item>> GetAll()
    {
        var items = _dbContext.Items.ToList();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public ActionResult<Item> Get(int id)
    {
        var item = _dbContext.Items.FirstOrDefault(x => x.Id == id);
        if (item is null)
        {
            return NotFound();
        }
        return Ok(item);
    }
}