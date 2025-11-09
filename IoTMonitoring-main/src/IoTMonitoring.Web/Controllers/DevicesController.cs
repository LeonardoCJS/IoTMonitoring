using Microsoft.AspNetCore.Mvc;
using IoTMonitoring.Web.ViewModels;
using IoTMonitoring.Application.Services;
using IoTMonitoring.Application.DTOs;
using IoTMonitoring.Domain.Entities;
using AutoMapper;

namespace IoTMonitoring.Web.Controllers
{
    public class DevicesController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly ISensorDataService _sensorDataService;
        private readonly IMapper _mapper;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(
            IDeviceService deviceService,
            ISensorDataService sensorDataService,
            IMapper mapper,
            ILogger<DevicesController> logger)
        {
            _deviceService = deviceService;
            _sensorDataService = sensorDataService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string sortBy = "Name", bool sortDescending = false)
        {
            try
            {
                var devices = await _deviceService.GetAllDevicesAsync();
                var deviceViewModels = _mapper.Map<List<DeviceViewModel>>(devices);

                var sortedDevices = sortBy.ToLower() switch
                {
                    "name" => sortDescending 
                        ? deviceViewModels.OrderByDescending(d => d.Name) 
                        : deviceViewModels.OrderBy(d => d.Name),
                    "status" => sortDescending 
                        ? deviceViewModels.OrderByDescending(d => d.Status) 
                        : deviceViewModels.OrderBy(d => d.Status),
                    "lastseen" => sortDescending 
                        ? deviceViewModels.OrderByDescending(d => d.LastSeen) 
                        : deviceViewModels.OrderBy(d => d.LastSeen),
                    _ => deviceViewModels.OrderBy(d => d.Name)
                };

                var totalCount = deviceViewModels.Count;
                var pagedDevices = sortedDevices
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginatedViewModel = new PaginatedViewModel<DeviceViewModel>(
                    pagedDevices, 
                    totalCount, 
                    pageNumber, 
                    pageSize, 
                    sortBy, 
                    sortDescending);

                return View(paginatedViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de dispositivos");
                TempData["ErrorMessage"] = "Erro ao carregar lista de dispositivos.";
                return View(new PaginatedViewModel<DeviceViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                if (device == null)
                {
                    TempData["ErrorMessage"] = "Dispositivo não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = _mapper.Map<DeviceViewModel>(device);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do dispositivo {DeviceId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar detalhes do dispositivo.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View(new CreateDeviceViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDeviceViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var createDto = _mapper.Map<CreateDeviceDto>(viewModel);
                var createdDevice = await _deviceService.CreateDeviceAsync(createDto);

                TempData["SuccessMessage"] = $"Dispositivo '{createdDevice.Name}' criado com sucesso!";
                return RedirectToAction(nameof(Details), new { id = createdDevice.Id });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Falha na validação ao criar dispositivo");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar dispositivo");
                ModelState.AddModelError(string.Empty, "Erro ao criar dispositivo. Tente novamente.");
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                if (device == null)
                {
                    TempData["ErrorMessage"] = "Dispositivo não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = _mapper.Map<EditDeviceViewModel>(device);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dispositivo para edição {DeviceId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar dispositivo.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditDeviceViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                if (device == null)
                {
                    TempData["ErrorMessage"] = "Dispositivo não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                if (Enum.TryParse<DeviceStatus>(viewModel.Status, out var status))
                {
                    await _deviceService.UpdateDeviceStatusAsync(device.DeviceId, status);
                }

                TempData["SuccessMessage"] = "Dispositivo atualizado com sucesso!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar dispositivo {DeviceId}", id);
                ModelState.AddModelError(string.Empty, "Erro ao atualizar dispositivo. Tente novamente.");
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                if (device == null)
                {
                    TempData["ErrorMessage"] = "Dispositivo não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = _mapper.Map<DeleteDeviceViewModel>(device);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dispositivo para exclusão {DeviceId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar dispositivo.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _deviceService.DeleteDeviceAsync(id);
                if (!result)
                {
                    TempData["ErrorMessage"] = "Dispositivo não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Dispositivo excluído com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir dispositivo {DeviceId}", id);
                TempData["ErrorMessage"] = "Erro ao excluir dispositivo. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
