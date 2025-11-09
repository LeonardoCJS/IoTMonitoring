using System.ComponentModel.DataAnnotations;

namespace IoTMonitoring.Web.ViewModels
{
    public class SensorDataViewModel
    {
        public int Id { get; set; }

        [Display(Name = "ID do Dispositivo")]
        public string DeviceId { get; set; } = string.Empty;

        [Display(Name = "Nome do Dispositivo")]
        public string DeviceName { get; set; } = string.Empty;

        [Display(Name = "Tipo de Sensor")]
        public string SensorType { get; set; } = string.Empty;

        [Display(Name = "Valor")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Value { get; set; }

        [Display(Name = "Data/Hora")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime Timestamp { get; set; }

        [Display(Name = "Unidade")]
        public string Unit { get; set; } = string.Empty;
    }

    public class CreateSensorDataViewModel
    {
        [Required(ErrorMessage = "O ID do dispositivo é obrigatório")]
        [Display(Name = "ID do Dispositivo")]
        public string DeviceId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de sensor é obrigatório")]
        [StringLength(100, ErrorMessage = "O tipo de sensor deve ter no máximo 100 caracteres")]
        [Display(Name = "Tipo de Sensor")]
        public string SensorType { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(-999999.99, 999999.99, ErrorMessage = "O valor deve estar entre -999999.99 e 999999.99")]
        [Display(Name = "Valor")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "A unidade é obrigatória")]
        [StringLength(50, ErrorMessage = "A unidade deve ter no máximo 50 caracteres")]
        [Display(Name = "Unidade")]
        public string Unit { get; set; } = string.Empty;
    }

    public class SensorDataSearchViewModel
    {
        [Display(Name = "ID do Dispositivo")]
        public string? DeviceId { get; set; }

        [Display(Name = "Data Inicial")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Data Final")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Tipo de Sensor")]
        public string? SensorType { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Timestamp";
        public bool SortDescending { get; set; } = true;
    }
}
