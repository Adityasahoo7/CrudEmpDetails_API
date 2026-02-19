using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Repository;
using CrudDemoPratice.Repository.Implementation;
using CrudDemoPratice.Repository.Interface;
using CrudDemoPratice.Service.AI.Implementation;
using CrudDemoPratice.Service.AI.Interface;
using CrudDemoPratice.Service.Implementation;
using CrudDemoPratice.Service.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//ADD DI for Repository and Service Layers

builder.Services.AddScoped<IEmployeeRepo, EmployeeRepo>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
//builder.Services.AddScoped<>
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAIQueryInterpreter, AIQueryInterpreter>();
builder.Services.AddScoped<IAISearchService, AISearchService>();

builder.Services.AddHttpClient<IAIQueryInterpreter, AIQueryInterpreter>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.Timeout = TimeSpan.FromMinutes(10); // Ollama can be slow first time
});


//Add /Register config file/Appetting.json
var provider = builder.Services.BuildServiceProvider();
var config = provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<AppDBContext>(item => item.UseSqlServer(config.GetConnectionString("Dbconn")));


builder.Services.AddControllers();

// ---------------- CORS ----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200"
                
                    )  // Angular origin
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


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
app.UseCors("AllowAngularApp");
app.UseAuthorization();

app.MapControllers();

app.Run();
