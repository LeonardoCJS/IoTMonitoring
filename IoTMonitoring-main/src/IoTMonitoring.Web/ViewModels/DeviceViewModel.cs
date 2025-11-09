using System.ComponentModel.DataAnnotations;

namespace IoTMonitoring.Web.ViewModels
{
    public class DeviceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do dispositivo é obrigatório")]
        [StringLength(100, ErrorMessage = "O ID do dispositivo deve ter no máximo 100 caracteres")]
        [Display(Name = "ID do Dispositivo")]
        public string DeviceId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres")]
        [Display(Name = "Nome do Dispositivo")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O status é obrigatório")]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "A localização deve ter no máximo 300 caracteres")]
        [Display(Name = "Localização")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Última Visualização")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime LastSeen { get; set; }

        [Display(Name = "Criado em")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime CreatedAt { get; set; }

        public List RecentSensorData { get; set; } = new List();
    }

    public class CreateDeviceViewModel
    {
        [Required(ErrorMessage = "O ID do dispositivo é obrigatório")]
        [StringLength(100, ErrorMessage = "O ID do dispositivo deve ter no máximo 100 caracteres")]
        [Display(Name = "ID do Dispositivo")]
        [RegularExpression(@"^[a-zA-Z0-9\-_]+$", ErrorMessage = "O ID deve conter apenas letras, números, hífens e underscores")]
        public string DeviceId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres")]
        [Display(Name = "Nome do Dispositivo")]
        public string Name { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "A localização deve ter no máximo 300 caracteres")]
        [Display(Name = "Localização")]
        public string Location { get; set; } = string.Empty;
    }

    public class EditDeviceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres")]
        [Display(Name = "Nome do Dispositivo")]
        public string Name { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "A localização deve ter no máximo 300 caracteres")]
        [Display(Name = "Localização")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "O status é obrigatório")]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;
    }

    public class DeleteDeviceViewModel
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime LastSeen { get; set; }
    }
}
