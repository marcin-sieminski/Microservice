using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using WebApi.Entity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connection = builder.Configuration.GetConnectionString("ItemsConnection");
var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connection)
{
    Username = Environment.GetEnvironmentVariable("ITEMS_DB_USERNAME"),
    Password = Environment.GetEnvironmentVariable("ITEMS_DB_PASSWORD")
};
builder.Services.AddDbContext<ItemContext>(options => options.UseNpgsql(connectionStringBuilder.ConnectionString));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
