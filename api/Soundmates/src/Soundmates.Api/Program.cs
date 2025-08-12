using Microsoft.EntityFrameworkCore;
using Soundmates.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// TODO add identity

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Soundmates API");
    });
}
app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
