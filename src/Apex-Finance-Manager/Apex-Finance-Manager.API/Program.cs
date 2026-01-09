using Apex_Finance_Manager.Data.DBContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Add Services to the container ---
// Controllers are used for the standard MVC pattern (like AuthController)
builder.Services.AddControllers();

// Configure Swagger/OpenAPI (using Swashbuckle 10.0.1 compatible setup)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Apex Finance Manager API",
        Version = "v1",
        Description = "API Documentation for the PFM Tool"
    });
});

// Configure Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// --- 2. Configure the HTTP request pipeline ---

if (app.Environment.IsDevelopment())
{
    // Enable the Swagger UI in development mode
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Apex Finance API v1");
        options.RoutePrefix = "swagger"; // Makes the URL: https://localhost:XXXX/swagger
    });
}

app.UseHttpsRedirection();

// Use Authorization middleware
app.UseAuthorization();

// Map the Controller routes (essential for AuthController to work)
app.MapControllers();

// Run the application
app.Run();
