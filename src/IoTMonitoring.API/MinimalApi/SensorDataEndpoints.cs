using IoTMonitoring.Application.Services;
using IoTMonitoring.Application.DTOs;
using IoTMonitoring.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace IoTMonitoring.API.MinimalApi
{
    public static class SensorDataEndpoints
    {
        public static void MapSensorDataEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/sensordata")
                .WithTags("SensorData");

            group.MapGet("/search", async (
                [FromServices] ISensorDataService sensorDataService,
                [FromQuery] string? deviceId,
                [FromQuery] DateTime? startDate,
                [FromQuery] DateTime? endDate,
                [FromQuery] string? sensorType,
                HttpContext httpContext,
                [FromQuery] int? pageNumber,
                [FromQuery] int? pageSize,
                [FromQuery] string? sortBy,
                [FromQuery] bool? sortDescending) =>
            {
                try
                {
                    var currentPage = pageNumber ?? 1;
                    var currentPageSize = pageSize ?? 10;
                    var currentSortBy = string.IsNullOrWhiteSpace(sortBy) ? "Timestamp" : sortBy;
                    var currentSortDescending = sortDescending ?? true;

                    if (string.IsNullOrEmpty(deviceId))
                        return Results.BadRequest(new { message = "DeviceId é obrigatório" });

                    var sensorData = await sensorDataService.GetSensorDataByDeviceAsync(deviceId, startDate, endDate);

                    if (!string.IsNullOrEmpty(sensorType))
                    {
                        sensorData = sensorData.Where(sd => sd.SensorType.Contains(sensorType, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    sensorData = currentSortBy.ToLower() switch
                    {
                        "timestamp" => currentSortDescending
                            ? sensorData.OrderByDescending(sd => sd.Timestamp).ToList()
                            : sensorData.OrderBy(sd => sd.Timestamp).ToList(),
                        "value" => currentSortDescending
                            ? sensorData.OrderByDescending(sd => sd.Value).ToList()
                            : sensorData.OrderBy(sd => sd.Value).ToList(),
                        "sensortype" => currentSortDescending
                            ? sensorData.OrderByDescending(sd => sd.SensorType).ToList()
                            : sensorData.OrderBy(sd => sd.SensorType).ToList(),
                        _ => sensorData.OrderByDescending(sd => sd.Timestamp).ToList()
                    };

                    var totalCount = sensorData.Count();
                    var pagedData = sensorData
                        .Skip((currentPage - 1) * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    var result = new PagedResult<SensorDataDto>(pagedData, totalCount, currentPage, currentPageSize, currentSortBy, currentSortDescending);

                    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                    result.Links.Add(new HateoasLink
                    {
                        Href = $"{baseUrl}/api/v1/sensordata/search?deviceId={deviceId}&pageNumber={currentPage}&pageSize={currentPageSize}",
                        Rel = "self",
                        Method = "GET"
                    });

                    if (result.HasPreviousPage)
                    {
                        result.Links.Add(new HateoasLink
                        {
                            Href = $"{baseUrl}/api/v1/sensordata/search?deviceId={deviceId}&pageNumber={currentPage - 1}&pageSize={currentPageSize}",
                            Rel = "previous",
                            Method = "GET"
                        });
                    }

                    if (result.HasNextPage)
                    {
                        result.Links.Add(new HateoasLink
                        {
                            Href = $"{baseUrl}/api/v1/sensordata/search?deviceId={deviceId}&pageNumber={currentPage + 1}&pageSize={currentPageSize}",
                            Rel = "next",
                            Method = "GET"
                        });
                    }

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao buscar dados de sensor: {ex.Message}");
                }
            })
            .WithName("SearchSensorData")
            .WithSummary("Busca dados de sensores com paginação e filtros");

            group.MapGet("/device/{deviceId}", async (
                string deviceId,
                [FromServices] ISensorDataService sensorDataService,
                [FromQuery] DateTime? startDate,
                [FromQuery] DateTime? endDate,
                HttpContext httpContext) =>
            {
                try
                {
                    var sensorData = await sensorDataService.GetSensorDataByDeviceAsync(deviceId, startDate, endDate);
                    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

                    var dataWithLinks = sensorData.Select(data => new HateoasResponse<SensorDataDto>(data)
                    {
                        Links = new List<HateoasLink>
                        {
                            new HateoasLink { Href = $"{baseUrl}/api/v1/sensordata/{data.Id}", Rel = "self", Method = "GET" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/by-deviceid/{deviceId}", Rel = "device", Method = "GET" }
                        }
                    }).ToList();

                    return Results.Ok(dataWithLinks);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao buscar dados do sensor: {ex.Message}");
                }
            })
            .WithName("GetSensorDataByDevice")
            .WithSummary("Retorna dados de sensores por dispositivo");

            group.MapPost("/", async ([FromBody] CreateSensorDataDto createDto, [FromServices] ISensorDataService sensorDataService, HttpContext httpContext) =>
            {
                try
                {
                    var sensorData = await sensorDataService.AddSensorDataAsync(createDto);
                    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

                    var response = new HateoasResponse<SensorDataDto>(sensorData)
                    {
                        Links = new List<HateoasLink>
                        {
                            new HateoasLink { Href = $"{baseUrl}/api/v1/sensordata/{sensorData.Id}", Rel = "self", Method = "GET" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/sensordata/device/{sensorData.DeviceId}", Rel = "device-data", Method = "GET" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/by-deviceid/{sensorData.DeviceId}", Rel = "device", Method = "GET" }
                        }
                    };

                    return Results.Created($"/api/v1/sensordata/{sensorData.Id}", response);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao adicionar dados do sensor: {ex.Message}");
                }
            })
            .WithName("AddSensorData")
            .WithSummary("Adiciona novos dados de sensor");

            group.MapPost("/bulk", async ([FromBody] IEnumerable<CreateSensorDataDto> createDtos, [FromServices] ISensorDataService sensorDataService) =>
            {
                try
                {
                    await sensorDataService.AddBulkSensorDataAsync(createDtos);
                    return Results.Ok(new { message = "Dados adicionados com sucesso", count = createDtos.Count() });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao adicionar dados em lote: {ex.Message}");
                }
            })
            .WithName("AddBulkSensorData")
            .WithSummary("Adiciona múltiplos dados de sensor de uma vez");
        }
    }
}
