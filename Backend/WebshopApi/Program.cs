using ApplicationDbContext;
using Scalar.AspNetCore;
using Service;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSingleton(new DatabaseConnectie(connectionString));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddOpenApi();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapControllers();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/db-test", async (DatabaseConnectie dbService) =>
{
    try
    {
        await dbService.TestConnectionAsync();
        return Results.Ok(new { status = "Database verbinding succesvol!" });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database fout: {ex.Message}");
    }
});

app.Run();