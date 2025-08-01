using System.Text;
using Domain.Entities;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.AutoMapper;
using Infrastructure.Data;
using Infrastructure.Data.Seader;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultures = new[] { "ru", "tg" };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ru");
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(InfrastructureProfile));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBaseRepository<Product, int>, ProductRepository>();
builder.Services.AddScoped<IBaseRepository<Category, int>, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IBaseRepository<Order, int>, OrderRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IBaseRepository<OrderItem, int>, OrderItemRepository>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IBaseRepository<CartItem, int>, CartItemRepository>();
builder.Services.AddScoped<IBaseRepositoryWithInclude<OrderItem, int>, OrderItemRepository>();
builder.Services.AddScoped<IBaseRepositoryWithInclude<CartItem, int>, CartItemRepository>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IBaseRepository<Review, int>, ReviewRepository>();




builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisCache");
    options.InstanceName = "myapp_";
});



builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
    {
        config.Password.RequiredLength = 8; // минимум 8 символов
        config.Password.RequireDigit = true; // хотя бы одна цифра
        config.Password.RequireNonAlphanumeric = true; // спецсимвол (например, !@#)
        config.Password.RequireUppercase = true; // хотя бы одна заглавная буква
        config.Password.RequireLowercase = true; // хотя бы одна строчная буква

    })
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "Описание вашего API",
        Contact = new OpenApiContact
        {
            Name = "Umar Nizomov",
            Email = "umarnizomov@gmail.com",
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// builder.Services.AddHangfire(config =>
//     config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
// builder.Services.AddHangfireServer();

// builder.Services.AddHangfire(config =>
//     config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
// builder.Services.AddHangfireServer();




builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


await using var scope = app.Services.CreateAsyncScope();
var services = scope.ServiceProvider;
var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
await DefaultRoles.SeedAsync(roleManager);
await DefaultUser.SeedAsync(userManager);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // обязательно
app.UseHttpsRedirection();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseRouting();
app.UseAuthentication();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
