using AutoMapper;
using DataAccess;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Models;
using Microsoft.EntityFrameworkCore;

namespace MinimalWebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapGets(this WebApplication app)
    {
        app.MapGet("/", () => "Hello World!").ExcludeFromDescription();

        app.MapGet("/item", async Task<Results<Ok<IEnumerable<ItemModel>>, NotFound>> ([FromServices] ItemContext dbContext, IMapper mapper) =>
        {
            var itemsDb = await dbContext.Items.ToListAsync();
            if (!itemsDb.Any())
            {
                return TypedResults.NotFound();
            }
            var items = mapper.Map<IEnumerable<ItemModel>>(itemsDb);
            return TypedResults.Ok(items);
        })
        .WithName("GetItems")
        .Produces<IEnumerable<ItemModel>>();
        
        app.MapGet("/item/{id:int}", async Task<Results<Ok<ItemModel>, NotFound>> ([FromServices] ItemContext dbContext, IMapper mapper, [FromRoute] int id) =>
        {
            var itemDb = await dbContext.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (itemDb is null)
            {
                return TypedResults.NotFound();
            }
            var item = mapper.Map<ItemModel>(itemDb);
            return TypedResults.Ok(item);
        })
        .WithName("GetItemById")
        .Produces<IEnumerable<ItemModel>>()
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
