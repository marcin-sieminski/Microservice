using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Models;

namespace WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class ItemController(ItemContext dbContext, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemModel>>> GetAll()
    {
        var itemsDb = await dbContext.Items.ToListAsync();
        var itemsDto = mapper.Map<List<ItemModel>>(itemsDb);
        return Ok(itemsDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> Get(int id)
    {
        var itemDb = await dbContext.Items.FirstOrDefaultAsync(x => x.Id == id);
        if (itemDb is null)
        {
            return NotFound();
        }
        var item = mapper.Map<ItemModel>(itemDb);
        return Ok(item);
    }
}