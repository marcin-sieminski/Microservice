using DataAccess;
using Microsoft.EntityFrameworkCore;
using MinimalWebApi.Extensions;
using Npgsql;
using Services.Mappings;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ItemContext>(options => options.UseNpgsql(new NpgsqlConnectionStringBuilder(builder.Configuration.GetConnectionString("ItemsConnection"))
{
    Username = Environment.GetEnvironmentVariable("ITEMS_DB_USERNAME"),
    Password = Environment.GetEnvironmentVariable("ITEMS_DB_PASSWORD")
}.ConnectionString));

builder.Services.AddAutoMapper(typeof(ItemMappingProfile));
builder.Services.AddScoped<IItemService, ItemService>();

var app = builder.Build();

app.MapGets()
   .MapPosts()
   .MapPuts()
   .MapDeletes();

app.Run();
