using Naijaput.Api.Extension;
using Naijaput.Api.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureLibrary(builder.Configuration);
builder.Services.ConfigureDb(builder.Configuration);
builder.Services.ConfigureServices(builder.Configuration);
var app = builder.Build();
Seeder.SeedData(app).Wait();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
