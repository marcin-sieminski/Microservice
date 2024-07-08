using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entity;
using WebApi.Model;

namespace WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class ItemController(ItemContext dbContext, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Item>> GetAll()
    {
        var itemsDb = dbContext.Items.ToList();
        var itemsDto = mapper.Map<List<ItemDto>>(itemsDb);
        return Ok(itemsDto);
    }

    [HttpGet("{id}")]
    public ActionResult<Item> Get(int id)
    {
        var itemDb = dbContext.Items.FirstOrDefault(x => x.Id == id);
        if (itemDb is null)
        {
            return NotFound();
        }
        var item = mapper.Map<ItemDto>(itemDb);
        return Ok(item);
    }
}