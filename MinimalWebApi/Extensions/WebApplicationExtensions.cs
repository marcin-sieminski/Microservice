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

        app.MapGet("items", async Task<Results<Ok<IEnumerable<ItemModel>>, NotFound>> ([FromServices] ItemContext dbContext, IMapper mapper) =>
        {
            var itemsEntity = await dbContext.Items.ToListAsync();
            if (!itemsEntity.Any())
            {
                return TypedResults.NotFound();
            }
            var items = mapper.Map<IEnumerable<ItemModel>>(itemsEntity);
            return TypedResults.Ok(items);
        })
        .WithName("items")
        .Produces<IEnumerable<ItemModel>>();

        app.MapGet("/items/{id:int}", async Task<Results<Ok<ItemModel>, NotFound>> ([FromServices] ItemContext dbContext, IMapper mapper, [FromRoute] int id) =>
        {
            var itemEntity = await dbContext.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (itemEntity is null)
            {
                return TypedResults.NotFound();
            }
            var item = mapper.Map<ItemModel>(itemEntity);
            return TypedResults.Ok(item);
        })
        .WithName("items")
        .Produces<IEnumerable<ItemModel>>()
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    public static WebApplication MapPosts(this WebApplication app)
    {
        app.MapPost("items", async ([FromBody] ItemModel item, [FromServices] ItemContext dbContext, IMapper mapper) =>
        {
            var itemEntity = mapper.Map<Item>(item);
            itemEntity.CreatedAt = DateTime.UtcNow;
            itemEntity.Version = 1;
            dbContext.Items.Add(itemEntity);
            await dbContext.SaveChangesAsync();
            return Results.Created($"items/{item.Id}", mapper.Map<ItemModel>(itemEntity));
        })
        .WithName("items")
        .Accepts<ItemModel>("application/json")
        .Produces<ItemModel>(StatusCodes.Status201Created);

        return app;
    }

    public static WebApplication MapPuts(this WebApplication app)
    {
        app.MapPut("/items/{id:int}", async (
                [FromRoute] int id,
                [FromBody] ItemModel item,
                ItemContext dbContext) =>
        {
            Item? foundItem = await dbContext.Items.FindAsync(id);
            if (foundItem is null) return Results.NotFound();
            foundItem.Name = item.Name;
            foundItem.Description = item.Description;
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("items")
        .Accepts<ItemModel>("application/json")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);
        return app;
    }

    public static WebApplication MapDeletes(this WebApplication app)
    {
        app.MapDelete("items/{id:int}", async (
                [FromRoute] int id,
                ItemContext dbContext) =>
        {
            if (await dbContext.Items.FindAsync(id) is Item item)
            {
                dbContext.Items.Remove(item);
                await dbContext.SaveChangesAsync();
                return Results.NoContent();
            }
            return Results.NotFound();
        })
        .WithName("items")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);
        return app;
    }
}
