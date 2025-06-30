using System.Text;
using Domain.Entities;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.AutoMapper;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(InfrastructureProfile));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBaseRepository<Product, int>, ProductRepository>();
builder.Services.AddScoped<IBaseRepository<Category, int>, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
// builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
// builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisCache");
    options.InstanceName = "products_";
});


// builder.Services.AddHttpContextAccessor();

// builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
//     {
//         config.Password.RequiredLength = 4;
//         config.Password.RequireDigit = false;
//         config.Password.RequireNonAlphanumeric = false;
//         config.Password.RequireUppercase = false;
//         config.Password.RequireLowercase = false;
//     })
//     .AddEntityFrameworkStores<DataContext>()
//     .AddDefaultTokenProviders();

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

// }).AddJwtBearer(o =>
// {
//     var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
//     o.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = builder.Configuration["Jwt:Issuer"],
//         ValidAudience = builder.Configuration["Jwt:Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey(key),
//         ClockSkew = TimeSpan.Zero
//     };
// });

// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "My API",
//         Version = "v1",
//         Description = "Описание вашего API",
//         Contact = new OpenApiContact
//         {
//             Name = "Umar Nizomov",
//             Email = "umarnizomov@gmail.com",
//         }
//     });

//     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Name = "Authorization",
//         Type = SecuritySchemeType.Http,
//         Scheme = "Bearer",
//         BearerFormat = "JWT",
//         In = ParameterLocation.Header,
//         Description = "Введите JWT токен: Bearer {your token}"
//     });

//     options.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 }
//             },
//             Array.Empty<string>()
//         }
//     });
// });

// builder.Services.AddHangfire(config =>
//     config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
// builder.Services.AddHangfireServer();

// builder.Services.AddHangfire(config =>
//     config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
// builder.Services.AddHangfireServer();



// builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// await using var scope = app.Services.CreateAsyncScope();
// var services = scope.ServiceProvider;
// var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
// var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
// await DefaultRoles.SeedAsync(roleManager);
// await DefaultUser.SeedAsync(userManager);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
