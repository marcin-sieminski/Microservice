using DataAccess;
using Microsoft.EntityFrameworkCore;
using MinimalWebApi.Extensions;
using Npgsql;
using Services.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ItemContext>(options => options.UseNpgsql(new NpgsqlConnectionStringBuilder(builder.Configuration.GetConnectionString("ItemsConnection"))
{
    Username = Environment.GetEnvironmentVariable("ITEMS_DB_USERNAME"),
    Password = Environment.GetEnvironmentVariable("ITEMS_DB_PASSWORD")
}.ConnectionString));

builder.Services.AddAutoMapper(typeof(ItemMappingProfile));

var app = builder.Build();

app.MapGets()
   .MapPosts();

app.Run();
