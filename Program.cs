using MyDotNet9Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(15);
});
builder.Services.AddTransient<IRepository, InMemoryRepository>();
var app = builder.Build();

if (app.Environment.IsDevelopment()) //this is a middleware
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseOutputCache();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();