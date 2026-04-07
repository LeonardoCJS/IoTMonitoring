using IoTMonitoring.Application.Services;
using IoTMonitoring.Application.DTOs;
using IoTMonitoring.API.Models;
using IoTMonitoring.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IoTMonitoring.API.MinimalApi
{
    public static class DevicesEndpoints
    {
        public static void MapDevicesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/devices")
                .WithTags("Devices");

            group.MapGet("/search", async (
                [FromServices] IDeviceService deviceService,
                [FromQuery] string? status,
                [FromQuery] string? location,
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
                    var currentSortBy = string.IsNullOrWhiteSpace(sortBy) ? "Name" : sortBy;
                    var currentSortDescending = sortDescending ?? false;

                    var devices = await deviceService.GetAllDevicesAsync();

                    if (!string.IsNullOrEmpty(status))
                    {
                        devices = devices.Where(d => d.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    if (!string.IsNullOrEmpty(location))
                    {
                        devices = devices.Where(d => d.Location.Contains(location, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    devices = currentSortBy.ToLower() switch
                    {
                        "name" => currentSortDescending
                            ? devices.OrderByDescending(d => d.Name).ToList()
                            : devices.OrderBy(d => d.Name).ToList(),
                        "status" => currentSortDescending
                            ? devices.OrderByDescending(d => d.Status).ToList()
                            : devices.OrderBy(d => d.Status).ToList(),
                        "lastseen" => currentSortDescending
                            ? devices.OrderByDescending(d => d.LastSeen).ToList()
                            : devices.OrderBy(d => d.LastSeen).ToList(),
                        _ => devices.OrderBy(d => d.Name).ToList()
                    };

                    var totalCount = devices.Count();
                    var pagedDevices = devices
                        .Skip((currentPage - 1) * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    var result = new PagedResult<DeviceDto>(pagedDevices, totalCount, currentPage, currentPageSize, currentSortBy, currentSortDescending);

                    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                    result.Links.Add(new HateoasLink
                    {
                        Href = $"{baseUrl}/api/v1/devices/search?pageNumber={currentPage}&pageSize={currentPageSize}&sortBy={currentSortBy}&sortDescending={currentSortDescending}",
                        Rel = "self",
                        Method = "GET"
                    });

                    if (result.HasPreviousPage)
                    {
                        result.Links.Add(new HateoasLink
                        {
                            Href = $"{baseUrl}/api/v1/devices/search?pageNumber={currentPage - 1}&pageSize={currentPageSize}&sortBy={currentSortBy}&sortDescending={currentSortDescending}",
                            Rel = "previous",
                            Method = "GET"
                        });
                    }

                    if (result.HasNextPage)
                    {
                        result.Links.Add(new HateoasLink
                        {
                            Href = $"{baseUrl}/api/v1/devices/search?pageNumber={currentPage + 1}&pageSize={currentPageSize}&sortBy={currentSortBy}&sortDescending={currentSortDescending}",
                            Rel = "next",
                            Method = "GET"
                        });
                    }

                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao buscar dispositivos: {ex.Message}");
                }
            })
            .WithName("SearchDevices")
            .WithSummary("Busca dispositivos com paginação, ordenação e filtros");

            group.MapGet("/", async ([FromServices] IDeviceService deviceService, HttpContext httpContext) =>
            {
                try
                {
                    var devices = await deviceService.GetAllDevicesAsync();
                    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

                    var devicesWithLinks = devices.Select(device => new HateoasResponse<DeviceDto>(device)
                    {
                        Links = new List<HateoasLink>
                        {
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/{device.Id}", Rel = "self", Method = "GET" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/{device.Id}", Rel = "update", Method = "PUT" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/{device.Id}", Rel = "delete", Method = "DELETE" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/sensordata/device/{device.DeviceId}", Rel = "sensor-data", Method = "GET" }
                        }
                    }).ToList();

                    return Results.Ok(devicesWithLinks);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao buscar dispositivos: {ex.Message}");
                }
            })
            .WithName("GetAllDevices")
            .WithSummary("Retorna todos os dispositivos com HATEOAS");

            group.MapGet("/{id}", async (int id, [FromServices] IDeviceService deviceService, HttpContext httpContext) =>
            {
                try
                {
                    var device = await deviceService.GetDeviceByIdAsync(id);

                    if (device == null)
                        return Results.NotFound(new { message = $"Dispositivo com ID {id} não encontrado" });

                    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

                    var response = new HateoasResponse<DeviceDto>(device)
                    {
                        Links = new List<HateoasLink>
                        {
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/{id}", Rel = "self", Method = "GET" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices", Rel = "all-devices", Method = "GET" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/{id}", Rel = "update", Method = "PUT" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/{id}", Rel = "delete", Method = "DELETE" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/{id}/status", Rel = "update-status", Method = "PATCH" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/sensordata/device/{device.DeviceId}", Rel = "sensor-data", Method = "GET" }
                        }
                    };

                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao buscar dispositivo: {ex.Message}");
                }
            })
            .WithName("GetDeviceById")
            .WithSummary("Retorna um dispositivo específico com HATEOAS");

            group.MapPost("/", async ([FromBody] CreateDeviceDto createDto, [FromServices] IDeviceService deviceService, HttpContext httpContext) =>
            {
                try
                {
                    var device = await deviceService.CreateDeviceAsync(createDto);
                    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

                    var response = new HateoasResponse<DeviceDto>(device)
                    {
                        Links = new List<HateoasLink>
                        {
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/{device.Id}", Rel = "self", Method = "GET" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/{device.Id}", Rel = "update", Method = "PUT" },
                            new HateoasLink { Href = $"{baseUrl}/api/v1/devices/{device.Id}", Rel = "delete", Method = "DELETE" }
                        }
                    };

                    return Results.Created($"/api/v1/devices/{device.Id}", response);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao criar dispositivo: {ex.Message}");
                }
            })
            .WithName("CreateDevice")
            .WithSummary("Cria um novo dispositivo");

            group.MapPatch("/{id}/status", async (int id, [FromBody] string status, [FromServices] IDeviceService deviceService) =>
            {
                try
                {
                    var device = await deviceService.GetDeviceByIdAsync(id);
                    if (device == null)
                        return Results.NotFound(new { message = $"Dispositivo com ID {id} não encontrado" });

                    if (!Enum.TryParse<DeviceStatus>(status, out var deviceStatus))
                        return Results.BadRequest(new { message = "Status inválido" });

                    await deviceService.UpdateDeviceStatusAsync(device.DeviceId, deviceStatus);

                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao atualizar status: {ex.Message}");
                }
            })
            .WithName("UpdateDeviceStatus")
            .WithSummary("Atualiza o status de um dispositivo");

            group.MapDelete("/{id}", async (int id, [FromServices] IDeviceService deviceService) =>
            {
                try
                {
                    var result = await deviceService.DeleteDeviceAsync(id);

                    if (!result)
                        return Results.NotFound(new { message = $"Dispositivo com ID {id} não encontrado" });

                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Erro ao excluir dispositivo: {ex.Message}");
                }
            })
            .WithName("DeleteDevice")
            .WithSummary("Exclui um dispositivo");
        }
    }
}
