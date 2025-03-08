using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using RepositoryLayer.Contexts;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Retrieve connection string
var connectionString = builder.Configuration.GetConnectionString("GreetingAppDB");

Console.WriteLine($"Connection String: {connectionString}"); // Debugging output

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'GreetingAppDB' not found in appsettings.json.");
}

// Register services
builder.Services.AddControllers();

// Register DbContext
builder.Services.AddDbContext<GreetingAppContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<IGreetingBL, GreetingBL>();
builder.Services.AddScoped<IGreetingRL, GreetingRL>();

//Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure NLog
builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
