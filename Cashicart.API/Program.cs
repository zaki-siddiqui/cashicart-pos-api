using Cashicart.Common.Interfaces;
using Cashicart.Infrastructure.Data;
using Cashicart.Infrastructure.Repositories;
using Cashicart.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MediatR;
using Serilog;
using Microsoft.Extensions.Logging;
using System.Text;
using Cashicart.Application;
using FluentValidation; // Added
using Cashicart.Application.Features.Products.Commands;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using Asp.Versioning; // Added
using Asp.Versioning.ApiExplorer;
using Cashicart.Common.Options;
using Cashicart.API.Middleware;
using Cashicart.Application.Common.Options;
using Cashicart.Application.Features.Products.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc
.WriteTo.Console()
    .WriteTo.File("logs/cashicart-.log", rollingInterval: RollingInterval.Day));

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null); // Disable camelCase for consistency

var apiVersioning = builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

apiVersioning.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});



// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommand>(); // Register validators from the application assembly

// Add DbContext
builder.Services.AddDbContext<CashicartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MediatR with centralized marker
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(ApplicationMarker))));

// Add Unit of Work and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

// Add Payment Service
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Add logging
builder.Services.AddLogging(builder =>
{
    builder.AddSerilog(dispose: true);
});

// Add Authentication
var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"];
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["Jwt:Audience"];
var key = Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(key))
{
    throw new ArgumentException("JWT configuration (Issuer, Audience, or Key) is missing or empty in environment variables or appsettings.json.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var userId = context.Principal?.FindFirst("sub")?.Value;
                if (string.IsNullOrEmpty(userId))
                    context.Fail("Invalid user ID in token.");
                return Task.CompletedTask;
            }
        };
    });

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Manager", policy => policy.RequireRole("Manager"));
});

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cashicart API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[] { }
        }
    });
});

builder.Services.Configure<ImageUploadOptions>(builder.Configuration.GetSection("ImageUpload"));

//builder.Services.Configure<SupportedLanguageOptions>(
//    builder.Configuration.GetSection("SupportedLanguages"));

builder.Services.Configure<LocalizationOptions>(
    builder.Configuration.GetSection("Localization"));

builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCommandValidator).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cashicart API V1");
        c.RoutePrefix = string.Empty;
    });
}


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "An unhandled exception occurred.");

        context.Response.StatusCode = exception is ValidationException ? 400 : 500;
        await context.Response.WriteAsJsonAsync(new
        {
            Error = exception?.Message,
            StackTrace = context.Response.StatusCode == 500 ? exception?.StackTrace : null
        });
    });
});

app.UseMiddleware<ApiExceptionMiddleware>();

app.UseStaticFiles();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();