using System.Text;
using HomeLedger.API.Mappers;
using HomeLedger.API.Middleware;
using HomeLedger.API.Persistence;
using HomeLedger.API.Services.BillPayments;
using HomeLedger.API.Services.Categories;
using HomeLedger.API.Services.Correlation;
using HomeLedger.API.Services.Incomes;
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

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token.",
        }
    );

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
    });
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
        .LogTo(Console.WriteLine, LogLevel.Error)
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
// Mapperly mappers
// ----------------------------------------------
builder.Services.AddSingleton<BillMapper>();
builder.Services.AddSingleton<BillPaymentMapper>();
builder.Services.AddSingleton<BudgetMapper>();
builder.Services.AddSingleton<CategoryMapper>();
builder.Services.AddSingleton<ExpenseMapper>();
builder.Services.AddSingleton<IncomeMapper>();
builder.Services.AddSingleton<UserMapper>();

// ----------------------------------------------
// Services
// ----------------------------------------------
builder.Services.AddScoped<IBillService, BillService>();
builder.Services.AddScoped<IBillPaymentService, BillPaymentService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IUserServices, UserServices>();

// ----------------------------------------------
// Allow services to access HttpContext (used for CorrelationId, etc.
// ----------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

//builder.Services.AddHttpClient<BasiqServices>();

// ----------------------------------------------
// Logging Configuration
// ----------------------------------------------

// Remove default logging providers
builder.Logging.ClearProviders();

// log output to terminal
builder.Logging.AddConsole();

// debug window
builder.Logging.AddDebug();

var app = builder.Build();
Console.WriteLine("App built...");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeLedger API v1");
        options.EnablePersistAuthorization();
        options.RoutePrefix = "swagger";
    });
}

// enforce HTTPS for all requests
app.UseHttpsRedirection();

// attach a Correlation ID to each request for tracing
app.UseCorrelationId();

app.UseMiddleware<CustomNotAcceptableMiddleware>();

app.UseCors("AllowFrontend");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
