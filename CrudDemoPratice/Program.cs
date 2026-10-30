using Azure.Identity;
using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Repository;
using CrudDemoPratice.Repository.Implementation;
using CrudDemoPratice.Repository.Interface;
using CrudDemoPratice.Service.AI.Implementation;
using CrudDemoPratice.Service.AI.Interface;
using CrudDemoPratice.Service.Implementation;
using CrudDemoPratice.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------- AZURE KEY VAULT ----------------
// KeyVaultName comes from appsettings.json, environment variables or user secrets.
var keyVaultName = builder.Configuration["KeyVaultName"];
if (string.IsNullOrWhiteSpace(keyVaultName))
    throw new InvalidOperationException(
        "KeyVaultName is not configured. Add it to appsettings.json, environment variables or user secrets.");

builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());

// Reads a required setting and fails at startup with a clear message if it is missing.
string GetRequired(string key)
{
    var value = builder.Configuration[key];
    if (string.IsNullOrWhiteSpace(value))
        throw new InvalidOperationException(
            $"Required setting '{key}' was not found. Add a secret named '{key}' to Key Vault '{keyVaultName}'.");
    return value;
}

// Secrets expected in Key Vault (flat names, exactly as written):
//   connstrlocal, jwt-key, jwt-Issuer, jwt-Audience, groq-apikey
var connStr = GetRequired("connstrAzurev3");
//var connStr = GetRequired("connstrlocal");
var jwtKey = GetRequired("jwt-key");
var jwtIssuer = GetRequired("jwt-Issuer");
var jwtAudience = GetRequired("jwt-Audience");

// AIQueryInterpreter reads this itself; checking here makes a missing key fail at startup,
// not on the first request.
_ = GetRequired("groq-apikey");

// ---------------- DATABASE ----------------
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(connStr));

// ---------------- DI: REPOSITORY & SERVICE LAYERS ----------------
builder.Services.AddScoped<IEmployeeRepo, EmployeeRepo>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISalaryPredictionService, SalaryPredictionService>();
builder.Services.AddScoped<IAISearchService, AISearchService>();

// Typed HttpClient (this single call also registers IAIQueryInterpreter)
builder.Services.AddHttpClient<IAIQueryInterpreter, AIQueryInterpreter>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.Timeout = TimeSpan.FromMinutes(10);
});

// ---------------- JWT AUTHENTICATION ----------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero // No buffer; strict expiry
        };
    });

builder.Services.AddAuthorization();

// ---------------- CONTROLLERS ----------------
builder.Services.AddControllers();

// ---------------- CORS ----------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ---------------- SWAGGER (with JWT support) ----------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ContactAppNLayer.Api", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste only the JWT token. Swagger adds the 'Bearer ' prefix automatically."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// ---------------- HTTP REQUEST PIPELINE ----------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngularApp");

app.UseAuthentication(); // must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();