using AutoMapper;
using IoTMonitoring.Application.DTOs;
using IoTMonitoring.Application.Services;
using IoTMonitoring.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IoTMonitoring.Web.Controllers;

public class SensorDataController : Controller
{
    private readonly ISensorDataService _sensorDataService;
    private readonly IMapper _mapper;
    private readonly ILogger<SensorDataController> _logger;

    public SensorDataController(ISensorDataService sensorDataService, IMapper mapper, ILogger<SensorDataController> logger)
    {
        _sensorDataService = sensorDataService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IActionResult> Index(SensorDataSearchViewModel search)
    {
        if (string.IsNullOrWhiteSpace(search.DeviceId))
        {
            return View(new PaginatedViewModel<SensorDataViewModel>());
        }

        try
        {
            var data = await _sensorDataService.GetSensorDataByDeviceAsync(search.DeviceId, search.StartDate, search.EndDate);

            if (!string.IsNullOrWhiteSpace(search.SensorType))
            {
                data = data.Where(x => x.SensorType.Contains(search.SensorType, StringComparison.OrdinalIgnoreCase));
            }

            var ordered = search.SortBy.ToLowerInvariant() switch
            {
                "value" => search.SortDescending ? data.OrderByDescending(x => x.Value) : data.OrderBy(x => x.Value),
                "sensortype" => search.SortDescending ? data.OrderByDescending(x => x.SensorType) : data.OrderBy(x => x.SensorType),
                _ => search.SortDescending ? data.OrderByDescending(x => x.Timestamp) : data.OrderBy(x => x.Timestamp),
            };

            var totalCount = ordered.Count();
            var pageItems = ordered.Skip((search.PageNumber - 1) * search.PageSize).Take(search.PageSize).ToList();
            var vmItems = _mapper.Map<List<SensorDataViewModel>>(pageItems);

            ViewBag.Search = search;
            return View(new PaginatedViewModel<SensorDataViewModel>(
                vmItems,
                totalCount,
                search.PageNumber,
                search.PageSize,
                search.SortBy,
                search.SortDescending));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar dados de sensores");
            TempData["ErrorMessage"] = "Erro ao consultar dados de sensores.";
            return View(new PaginatedViewModel<SensorDataViewModel>());
        }
    }

    public IActionResult Create()
    {
        return View(new CreateSensorDataViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSensorDataViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        try
        {
            var dto = _mapper.Map<CreateSensorDataDto>(viewModel);
            await _sensorDataService.AddSensorDataAsync(dto);
            TempData["SuccessMessage"] = "Leitura de sensor cadastrada com sucesso.";
            return RedirectToAction(nameof(Index), new { deviceId = viewModel.DeviceId });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar leitura de sensor");
            ModelState.AddModelError(string.Empty, "Erro inesperado ao cadastrar leitura.");
            return View(viewModel);
        }
    }
}
