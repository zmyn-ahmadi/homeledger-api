using System.Text;
using HomeLedger.API.Middleware;
using HomeLedger.API.Persistence;
using HomeLedger.API.Services.Categories;
using HomeLedger.API.Services.Token;
using HomeLedger.API.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

// Controllers
builder
    .Services.AddControllers(options =>
    {
      options.ReturnHttpNotAcceptable = true;
    })
    .AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc(
      "v1",
      new OpenApiInfo
      {
        Title = "HomeLedger API",
        Version = "v1",
        Description =
              "HomeLedger API provides a complete backend for personal finance management.",
        Contact = new OpenApiContact { Email = "zmnahm17@gmail.com", Name = "zmyn ahmadi" },
      }
  );
});

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
builder
    .Services.AddAuthentication(options =>
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

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
      };
    });

// ----------------------------------------------
// Unit of work
// ----------------------------------------------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ----------------------------------------------
// User Service
// ----------------------------------------------
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// ----------------------------------------------
// Jwt Service
// ----------------------------------------------
builder.Services.AddScoped<JwtService>();

// ----------------------------------------------
// DbContext Service
// ----------------------------------------------
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is not configured");

builder.Services.AddDbContext<HomeLedgerDbContext>(options =>
    options
        .UseNpgsql(connectionString)
        .UseLazyLoadingProxies()
        .LogTo(Console.WriteLine, LogLevel.Debug)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

// ----------------------------------------------
// SuperUser Policy
// ----------------------------------------------
builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy("SuperUserOnly", policy => policy.RequireClaim("IsSuperUser", "True", "true"));

// ----------------------------------------------
// Cors Policy
// ----------------------------------------------
builder.Services.AddCors(options =>
{
  options.AddPolicy(
      "AllowFrontend",
      policy =>
          policy
              .WithOrigins("http://192.168.0.19:3000", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
  );
});

// ----------------------------------------------
// AutoMapper
// ----------------------------------------------
builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeLedger API v1");
    options.RoutePrefix = "swagger";
  });
}

app.UseMiddleware<CustomNotAcceptableMiddleware>();

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
