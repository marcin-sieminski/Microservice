using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Models;
using Services.Services;

namespace MinimalWebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapGets(this WebApplication app)
    {
        app.MapGet("/", () => "Hello World!").ExcludeFromDescription();

        app.MapGet("items", async Task<Results<Ok<List<ItemModel>>, NotFound>> (
            [FromServices] ItemService service) =>
            {
                var items = await service.GetAll();
                if (!items.Any())
                {
                    return TypedResults.NotFound();
                }
                return TypedResults.Ok(items);
            })
        .WithName("items")
        .Produces<IEnumerable<ItemModel>>();

        app.MapGet("/items/{id:int}", async Task<Results<Ok<ItemModel>, NotFound>> (
                [FromServices] ItemService service,
                [FromRoute] int id) =>
                {
                    var item = await service.GetById(id);
                    if (item is null)
                    {
                        return TypedResults.NotFound();
                    }
                    return TypedResults.Ok(item);
                })
        .WithName("items")
        .Produces<IEnumerable<ItemModel>>()
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    public static WebApplication MapPosts(this WebApplication app)
    {
        app.MapPost("items", async (
                [FromBody] ItemModel newItem,
                [FromServices] ItemService service) =>
                {
                    var item = await service.Create(newItem);
                    return item is null ? TypedResults.BadRequest() : Results.Created($"items/{item.Id}", item);
                })
        .WithName("items")
        .Accepts<ItemModel>("application/json")
        .Produces<ItemModel>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }

    public static WebApplication MapPuts(this WebApplication app)
    {
        app.MapPut("/items/{id:int}", async (
                [FromRoute] int id,
                [FromBody] ItemModel updateItem,
                ItemService service) =>
                {
                    var item = await service.Update(id, updateItem);
                    return item is null ? Results.NotFound() : Results.NoContent();
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
                ItemService service) =>
                {
                    if (await service.Delete(id))
                    {
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
