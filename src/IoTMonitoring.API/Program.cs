using IoTMonitoring.API.HealthChecks;
using IoTMonitoring.API.Middleware;
using IoTMonitoring.API.MinimalApi;
using IoTMonitoring.Application;
using IoTMonitoring.Infrastructure;
using IoTMonitoring.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IoT Monitoring API",
        Version = "v1",
        Description = """
            API RESTful para monitoramento de dispositivos IoT e seus dados de sensores.

            ## Autenticação
            Endpoints de escrita requerem autenticação JWT. Obtenha o token em **POST /api/auth/login** e use o botão **Authorize** acima.

            ## Funcionalidades
            - **Dispositivos**: cadastro, consulta, atualização de status e exclusão.
            - **Dados de Sensores**: registro individual ou em lote, consulta por dispositivo com filtros de data.
            - **Alertas (MongoDB)**: registro e consulta de alertas de sensores críticos via NoSQL.
            - **Paginação e Ordenação**: todos os endpoints de listagem suportam paginação e filtros via query string.
            - **HATEOAS**: respostas incluem links hipermídia para navegação entre recursos.

            ## Códigos de Status
            | Código | Descrição |
            |--------|-----------|
            | 200 | Requisição processada com sucesso |
            | 201 | Recurso criado com sucesso |
            | 204 | Operação realizada, sem conteúdo de retorno |
            | 400 | Dados inválidos na requisição |
            | 401 | Não autenticado — token JWT ausente ou inválido |
            | 404 | Recurso não encontrado |
            | 500 | Erro interno no servidor |
            """,
        Contact = new OpenApiContact
        {
            Name = "IoT Monitoring Team",
            Email = "leonardocarvalhosantos14@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Token JWT. Informe: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    var apiXml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(apiXml))
        c.IncludeXmlComments(apiXml);

    var appXml = Path.Combine(AppContext.BaseDirectory, "IoTMonitoring.Application.xml");
    if (File.Exists(appXml))
        c.IncludeXmlComments(appXml);
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpClient("external-health");

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .AddCheck<ExternalServiceHealthCheck>("external_service");

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddConsoleExporter();
    });

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

app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IoT Monitoring API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "IoT Monitoring API";
        c.DisplayRequestDuration();
        c.DefaultModelsExpandDepth(2);
        c.DefaultModelExpandDepth(2);
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapDevicesEndpoints();
app.MapSensorDataEndpoints();
app.MapAlertsEndpoints();
app.MapControllers();

app.Run();
public partial class Program;
