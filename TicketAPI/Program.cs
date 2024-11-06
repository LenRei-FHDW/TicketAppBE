using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Serilog;
using TicketAPI.Data;
using TicketAPI.Interfaces;
using TicketAPI.Models;
using TicketAPI.Services;

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

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TicketApiDbContext>()
    .AddDefaultTokenProviders();

// JWT-Konfiguration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

// SeriLog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Interfaces
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            string message;
            // Handle specific exceptions here
            if (false)
            {
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                message = "Internal Server Error";
            }
            
            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    
    await SeedRolesAsync(roleManager);
    await SeedAdminUserAsync(userManager, configuration);
}

app.Run();

async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
{
    string [] roleNames = { "Admin", "Seller", "User" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration)
{
    var adminEmail = configuration["AdminUser:Email"];
    var adminPassword = configuration["AdminUser:Password"];
    
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
 
        var newAdminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var createAdminResult = await userManager.CreateAsync(newAdminUser, adminPassword);
        
        if (createAdminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdminUser, "Admin");
        }
    }
}

