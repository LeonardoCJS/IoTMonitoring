using IoTMonitoring.Infrastructure;
using IoTMonitoring.Application;
using IoTMonitoring.Web.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Database Configuration
builder.Services.AddInfrastructure(builder.Configuration);

// Application Services
builder.Services.AddApplication();

// AutoMapper
builder.Services.AddAutoMapper(typeof(WebMappingProfile));

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
    app.UseDeveloperExceptionPage();
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

app.Run();
