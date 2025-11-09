using Microsoft.AspNetCore.Mvc;
using IoTMonitoring.Web.ViewModels;
using IoTMonitoring.Application.Services;
using AutoMapper;

namespace IoTMonitoring.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly ISensorDataService _sensorDataService;
        private readonly IMapper _mapper;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IDeviceService deviceService,
            ISensorDataService sensorDataService,
            IMapper mapper,
            ILogger<HomeController> logger)
        {
            _deviceService = deviceService;
            _sensorDataService = sensorDataService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var devices = await _deviceService.GetAllDevicesAsync();
                
                var viewModel = new HomeViewModel
                {
                    TotalDevices = devices.Count,
                    OnlineDevices = devices.Count(d => d.Status == "Online"),
                    OfflineDevices = devices.Count(d => d.Status == "Offline"),
                    ErrorDevices = devices.Count(d => d.Status == "Error"),
                    RecentDevices = _mapper.Map<List<DeviceViewModel>>(devices.Take(5).ToList())
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dashboard");
                return View(new HomeViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
