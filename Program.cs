using Microsoft.EntityFrameworkCore;
using MyDotNet9Api;
using MyDotNet9Api.Utilities;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetValue<string>("allowedOrigins")!.Split(",");
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod().WithExposedHeaders("total-records-count");
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60);
});
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("name=DefaultConnection"));
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();
    

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseCors();
app.UseOutputCache();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();