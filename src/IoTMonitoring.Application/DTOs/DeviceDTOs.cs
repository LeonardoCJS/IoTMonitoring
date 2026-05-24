namespace IoTMonitoring.Application.DTOs
{
    /// <summary>Representa um dispositivo IoT registrado no sistema.</summary>
    public class DeviceDto
    {
        /// <summary>Identificador interno do registro no banco de dados.</summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>Identificador único do dispositivo (ex: número de série ou UUID).</summary>
        /// <example>DEVICE-001</example>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Nome amigável do dispositivo.</summary>
        /// <example>Sensor de Temperatura - Sala A</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>Status atual do dispositivo. Valores possíveis: Online, Offline, Error.</summary>
        /// <example>Online</example>
        public string Status { get; set; } = string.Empty;

        /// <summary>Localização física onde o dispositivo está instalado.</summary>
        /// <example>Galpão Norte - Prateleira 3</example>
        public string Location { get; set; } = string.Empty;

        /// <summary>Data e hora da última leitura recebida do dispositivo (UTC).</summary>
        /// <example>2024-06-15T14:30:00Z</example>
        public DateTime LastSeen { get; set; }
    }

    /// <summary>Dados necessários para cadastrar um novo dispositivo IoT.</summary>
    public class CreateDeviceDto
    {
        /// <summary>Identificador único do dispositivo. Deve ser único no sistema.</summary>
        /// <example>DEVICE-001</example>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Nome amigável para identificar o dispositivo.</summary>
        /// <example>Sensor de Temperatura - Sala A</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>Localização física do dispositivo.</summary>
        /// <example>Galpão Norte - Prateleira 3</example>
        public string Location { get; set; } = string.Empty;
    }

    /// <summary>Representa uma leitura de sensor de um dispositivo IoT.</summary>
    public class SensorDataDto
    {
        /// <summary>Identificador interno do registro no banco de dados.</summary>
        /// <example>42</example>
        public int Id { get; set; }

        /// <summary>Identificador do dispositivo que gerou a leitura.</summary>
        /// <example>DEVICE-001</example>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Tipo do sensor que originou a leitura (ex: Temperature, Humidity, Pressure).</summary>
        /// <example>Temperature</example>
        public string SensorType { get; set; } = string.Empty;

        /// <summary>Valor medido pelo sensor.</summary>
        /// <example>23.5</example>
        public decimal Value { get; set; }

        /// <summary>Data e hora em que a leitura foi registrada (UTC).</summary>
        /// <example>2024-06-15T14:30:00Z</example>
        public DateTime Timestamp { get; set; }

        /// <summary>Unidade de medida do valor registrado (ex: °C, %, hPa).</summary>
        /// <example>°C</example>
        public string Unit { get; set; } = string.Empty;
    }

    /// <summary>Dados necessários para registrar uma nova leitura de sensor.</summary>
    public class CreateSensorDataDto
    {
        /// <summary>Identificador do dispositivo que gerou a leitura. O dispositivo deve estar cadastrado.</summary>
        /// <example>DEVICE-001</example>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Tipo do sensor. Exemplos: Temperature, Humidity, Pressure, CO2.</summary>
        /// <example>Temperature</example>
        public string SensorType { get; set; } = string.Empty;

        /// <summary>Valor numérico medido pelo sensor.</summary>
        /// <example>23.5</example>
        public decimal Value { get; set; }

        /// <summary>Unidade de medida correspondente ao valor (ex: °C, %, hPa, ppm).</summary>
        /// <example>°C</example>
        public string Unit { get; set; } = string.Empty;
    }
}