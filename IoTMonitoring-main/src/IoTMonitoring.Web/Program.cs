using IoTMonitoring.Infrastructure;
using IoTMonitoring.Application;
using IoTMonitoring.API.MinimalApi;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// Database Configuration
builder.Services.AddInfrastructure(builder.Configuration);

// Application Services
builder.Services.AddApplication();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "IoT Monitoring API",
        Version = "v1",
        Description = "API RESTful para gerenciamento de dispositivos IoT",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Leonardo CJS",
            Email = "contato@iotmonitoring.com"
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IoT Monitoring API V1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

// MVC Routes (Custom Routes)
app.MapControllerRoute(
    name: "devices",
    pattern: "dispositivos/{action=Index}/{id?}",
    defaults: new { controller = "Devices" });

app.MapControllerRoute(
    name: "sensordata",
    pattern: "sensores/{action=Index}/{id?}",
    defaults: new { controller = "SensorData" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Minimal API Endpoints
app.MapDevicesEndpoints();
app.MapSensorDataEndpoints();

// API Controllers
app.MapControllers();

app.Run();
