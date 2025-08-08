using Microsoft.EntityFrameworkCore;
using MyDotNet9Api;
using MyDotNet9Api.Utilities;
using AutoMapper;
using MyDotNet9Api.Services;
using NetTopologySuite;
using NetTopologySuite.Geometries;

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
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("name=DefaultConnection", sqlserver=> sqlserver.UseNetTopologySuite()));
builder.Services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
builder.Services.AddSingleton(provider => new MapperConfiguration(config =>
{
    var geometryFactory = provider.GetRequiredService<GeometryFactory>();
    config.AddProfile(new AutoMapperProfiles(geometryFactory));
}).CreateMapper());
builder.Services.AddTransient<IFileStorage, AzureFileStorage>();
var app = builder.Build();
    
 
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseCors();
app.UseOutputCache();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();