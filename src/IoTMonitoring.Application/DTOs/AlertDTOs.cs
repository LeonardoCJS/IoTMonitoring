namespace IoTMonitoring.Application.DTOs
{
    /// <summary>Dados para criar um novo alerta de sensor.</summary>
    public class CreateSensorAlertDto
    {
        /// <summary>Identificador do dispositivo que gerou o alerta.</summary>
        /// <example>DEVICE-001</example>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Tipo do sensor que disparou o alerta.</summary>
        /// <example>Temperature</example>
        public string SensorType { get; set; } = string.Empty;

        /// <summary>Valor medido que ultrapassou o limite.</summary>
        /// <example>95.5</example>
        public decimal Value { get; set; }

        /// <summary>Unidade de medida.</summary>
        /// <example>°C</example>
        public string Unit { get; set; } = string.Empty;

        /// <summary>Nível de criticidade do alerta. Valores: Warning, Critical.</summary>
        /// <example>Critical</example>
        public string AlertLevel { get; set; } = string.Empty;

        /// <summary>Mensagem descritiva do alerta.</summary>
        /// <example>Temperatura acima de 90°C detectada no dispositivo.</example>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>Representa um alerta de sensor armazenado no MongoDB.</summary>
    public class SensorAlertDto
    {
        /// <summary>Identificador único do alerta no MongoDB (ObjectId).</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Identificador do dispositivo.</summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Tipo do sensor.</summary>
        public string SensorType { get; set; } = string.Empty;

        /// <summary>Valor que disparou o alerta.</summary>
        public decimal Value { get; set; }

        /// <summary>Unidade de medida.</summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>Nível do alerta: Warning ou Critical.</summary>
        public string AlertLevel { get; set; } = string.Empty;

        /// <summary>Mensagem do alerta.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Data e hora de criação do alerta (UTC).</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Indica se o alerta foi reconhecido/tratado.</summary>
        public bool Acknowledged { get; set; }
    }
}
