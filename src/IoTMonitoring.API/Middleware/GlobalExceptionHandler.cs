using System.Net;
using System.Text.Json;

namespace IoTMonitoring.API.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção não tratada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title) = exception switch
            {
                ArgumentException => (HttpStatusCode.BadRequest, "Requisição inválida"),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Recurso não encontrado"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Não autorizado"),
                _ => (HttpStatusCode.InternalServerError, "Erro interno no servidor")
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var problem = new
            {
                type = $"https://httpstatuses.io/{(int)statusCode}",
                title,
                status = (int)statusCode,
                detail = exception.Message,
                instance = context.Request.Path.Value
            };

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(problem, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            );
        }
    }
}
