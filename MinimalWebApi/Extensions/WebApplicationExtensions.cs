using AutoMapper;
using DataAccess;
using DataAccess.Entities;
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

        app.MapGet("item", async Task<Results<Ok<IEnumerable<ItemModel>>, NotFound>> ([FromServices] ItemContext dbContext, IMapper mapper) =>
        {
            var itemsEntity = await dbContext.Items.ToListAsync();
            if (!itemsEntity.Any())
            {
                return TypedResults.NotFound();
            }
            var items = mapper.Map<IEnumerable<ItemModel>>(itemsEntity);
            return TypedResults.Ok(items);
        })
        .WithName("GetItems")
        .Produces<IEnumerable<ItemModel>>();

        app.MapGet("/item/{id:int}", async Task<Results<Ok<ItemModel>, NotFound>> ([FromServices] ItemContext dbContext, IMapper mapper, [FromRoute] int id) =>
        {
            var itemEntity = await dbContext.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (itemEntity is null)
            {
                return TypedResults.NotFound();
            }
            var item = mapper.Map<ItemModel>(itemEntity);
            return TypedResults.Ok(item);
        })
        .WithName("GetItemById")
        .Produces<IEnumerable<ItemModel>>()
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    public static WebApplication MapPosts(this WebApplication app)
    {
        app.MapPost("item", async ([FromBody] ItemModel item, [FromServices] ItemContext dbContext, IMapper mapper) =>
        {
            var itemEntity = mapper.Map<Item>(item);
            itemEntity.CreatedAt = DateTime.UtcNow;
            itemEntity.Version = 1;
            dbContext.Items.Add(itemEntity);
            await dbContext.SaveChangesAsync();
            return Results.Created($"item/{item.Id}", mapper.Map<ItemModel>(itemEntity));
        })
          .Produces<ItemModel>(StatusCodes.Status201Created);

        return app;
    }
}
