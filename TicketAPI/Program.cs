using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Serilog;
using TicketAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// DbContext
builder.Services.AddDbContext<TicketApiDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var password = builder.Configuration["ConnectionStrings:DatabasePassword"];

    var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString)
    {
        Password = password,
        SslMode = MySqlSslMode.Required,
        SslCert = Path.Combine(Directory.GetCurrentDirectory(), "AzureRootCert.pem")
    };
    
    options.UseMySql(connectionStringBuilder.ConnectionString, ServerVersion.AutoDetect(connectionStringBuilder.ConnectionString));
});

// SeriLog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();