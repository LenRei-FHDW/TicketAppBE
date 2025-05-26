using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Serilog;
using TicketAPI.Data;
using TicketAPI.Data.Exceptions;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.Helper;
using TicketAPI.Services.Scoped;

var builder = WebApplication.CreateBuilder(args);

// Add eternal dependencies
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ITokenGenerator, JwtGenerator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddSingleton<EmailHelper>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddScoped<CategoryService>();


//Add Repositories
builder.Services.AddScoped<IRepository<Order, Guid>, Repository<Order, Guid>>();
builder.Services.AddScoped<IRepository<Product, Guid>, Repository<Product, Guid>>();
builder.Services.AddScoped<IRepository<OrderItem, Guid>, Repository<OrderItem, Guid>>();
builder.Services.AddScoped<IRepository<ShoppingCartItem, Guid>, Repository<ShoppingCartItem, Guid>>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ShoppingCartRepository>();
builder.Services.AddScoped<IOrderRepository,OrderRepository>();
builder.Services.AddScoped<IRepository<Category, Guid>, Repository<Category, Guid>>();

// DbContext
builder.Services.AddDbContext<TicketApiDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var password = builder.Configuration["ConnectionStrings:DatabasePassword"];

    var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString)
    {
        Password = password,
        SslMode = MySqlSslMode.Required,
        //SslCert = Path.Combine(Directory.GetCurrentDirectory(), "AzureRootCert.pem")
    };
    
    options.UseMySql(connectionStringBuilder.ConnectionString, ServerVersion.AutoDetect(connectionStringBuilder.ConnectionString));
});

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TicketApiDbContext>()
    .AddDefaultTokenProviders();

// JWT-Konfiguration
var jwtSettings = builder.Configuration.GetSection("Authentification:JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
var googleSettings = builder.Configuration.GetSection("Authentification:Google");

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
    })
    .AddGoogle(options =>
    {
        options.SignInScheme = "ExternalCookie";
        options.ClientId = googleSettings["ClientId"];
        options.ClientSecret = googleSettings["ClientSecret"];
    })
    .AddCookie("ExternalCookie");

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

// SeriLog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(builder.Configuration));

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("*.pfax423.store", "https://localhost:7145", "http://localhost:5246")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot/Uploads")),
    RequestPath = "/api/images"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseExceptionHandler(errorApp =>
        errorApp.Run(async context =>
        {
            context.Response.ContentType = "application/json";
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature != null)
            {
                string message;
                // Handle specific exceptions here
                if (exceptionHandlerFeature.Error is KeyNotFoundException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    message = "Key not found";
                }
                else if (exceptionHandlerFeature.Error is ForbiddenException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    message = "Forbidden";
                }
                else if (exceptionHandlerFeature.Error is SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    message = "The token expired";
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
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    
    await DbSeeder.SeedRolesAsync(roleManager);
    await DbSeeder.SeedAdminUserAsync(userManager, configuration);
}

app.Run();





