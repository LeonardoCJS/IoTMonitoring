using IoTMonitoring.Application.DTOs;
using IoTMonitoring.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IoTMonitoring.API.MinimalApi
{
    public static class AlertsEndpoints
    {
        public static void MapAlertsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/alerts")
                .WithTags("Alerts (MongoDB)");

            group.MapGet("/unacknowledged", async ([FromServices] ISensorAlertService alertService) =>
            {
                try
                {
                    var alerts = await alertService.GetUnacknowledgedAsync();
                    return Results.Ok(alerts);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao buscar alertas: {ex.Message}");
                }
            })
            .WithName("GetUnacknowledgedAlerts")
            .WithSummary("Lista todos os alertas não reconhecidos")
            .WithDescription("""
                Retorna todos os alertas de sensores que ainda não foram reconhecidos (Acknowledged = false).

                Os alertas são armazenados no **MongoDB** e representam leituras que ultrapassaram limites críticos nos dispositivos IoT.

                Requer autenticação JWT.
                """)
            .Produces<IEnumerable<SensorAlertDto>>(200, "application/json")
            .ProducesProblem(500)
            .RequireAuthorization();

            group.MapGet("/device/{deviceId}", async (string deviceId, [FromServices] ISensorAlertService alertService) =>
            {
                try
                {
                    var alerts = await alertService.GetByDeviceIdAsync(deviceId);
                    return Results.Ok(alerts);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao buscar alertas: {ex.Message}");
                }
            })
            .WithName("GetAlertsByDevice")
            .WithSummary("Lista alertas de um dispositivo específico")
            .WithDescription("""
                Retorna todos os alertas registrados para o dispositivo identificado por `deviceId`.

                Os alertas ficam ordenados do mais recente para o mais antigo.
                """)
            .Produces<IEnumerable<SensorAlertDto>>(200, "application/json")
            .ProducesProblem(500)
            .RequireAuthorization();

            group.MapPost("/", async ([FromBody] CreateSensorAlertDto dto, [FromServices] ISensorAlertService alertService) =>
            {
                try
                {
                    var alert = await alertService.CreateAlertAsync(dto);
                    return Results.Created($"/api/v1/alerts/device/{alert.DeviceId}", alert);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao criar alerta: {ex.Message}");
                }
            })
            .WithName("CreateAlert")
            .WithSummary("Registra um novo alerta de sensor no MongoDB")
            .WithDescription("""
                Cria um novo alerta de sensor e persiste no **MongoDB**.

                Use este endpoint quando um sensor detectar valor fora do intervalo seguro.

                **AlertLevel válidos:** `Warning`, `Critical`

                Requer autenticação JWT.
                """)
            .Produces<SensorAlertDto>(201, "application/json")
            .Produces(400)
            .ProducesProblem(500)
            .RequireAuthorization();

            group.MapPatch("/{id}/acknowledge", async (string id, [FromServices] ISensorAlertService alertService) =>
            {
                try
                {
                    await alertService.AcknowledgeAlertAsync(id);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao reconhecer alerta: {ex.Message}");
                }
            })
            .WithName("AcknowledgeAlert")
            .WithSummary("Marca um alerta como reconhecido")
            .WithDescription("""
                Atualiza o campo `Acknowledged` para `true` no alerta identificado pelo `id` (ObjectId do MongoDB).

                Retorna **204** em caso de sucesso.

                Requer autenticação JWT.
                """)
            .Produces(204)
            .Produces(400)
            .ProducesProblem(500)
            .RequireAuthorization();
        }
    }
}
