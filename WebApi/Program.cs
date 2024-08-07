using DataAccess;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ItemMappingProfile = Services.Mappings.ItemMappingProfile;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<ItemContext>(options => options.UseNpgsql(new NpgsqlConnectionStringBuilder(builder.Configuration.GetConnectionString("ItemsConnection"))
{
    Username = Environment.GetEnvironmentVariable("ITEMS_DB_USERNAME"),
    Password = Environment.GetEnvironmentVariable("ITEMS_DB_PASSWORD")
}.ConnectionString));
builder.Services.AddAutoMapper(typeof(ItemMappingProfile));
var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
