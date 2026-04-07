# IoT Monitoring System (.NET 8)

## Visao geral do projeto
Sistema de monitoramento de dispositivos IoT com arquitetura em camadas (Domain, Application, Infrastructure, API e Web), oferecendo:
- cadastro e gerenciamento de dispositivos;
- ingestao de leituras de sensores;
- consultas historicas com filtros;
- API REST + Minimal API;
- dashboard MVC.

## Novas funcionalidades da sprint

### 1) Monitoramento e observabilidade
- **Health Checks** em `/health` para:
  - status do banco (DbContext);
  - disponibilidade de servico externo configurado.
- **Logging estruturado com Serilog**:
  - niveis (`Information`, `Warning`, `Error`);
  - saida para console e arquivo (`logs/log-*.txt`).
- **Tracing e metricas com OpenTelemetry**:
  - instrumentacao de ASP.NET Core, chamadas HTTP e runtime;
  - exportacao para console (facil validar localmente).

### 2) Testes automatizados (padrao AAA)
- **Testes unitarios** (xUnit + Moq) para camada de aplicacao:
  - `CreateDeviceAsync_DadoPayloadValido_DeveCriarDispositivo`
  - `AddSensorDataAsync_DadoDispositivoInexistente_DeveLancarArgumentException`
- **Testes de integracao** com `WebApplicationFactory`:
  - valida endpoint de health;
  - valida fluxo HTTP de criar e consultar dispositivo.

## Estrutura de projetos
- `src/IoTMonitoring.Domain`
- `src/IoTMonitoring.Application`
- `src/IoTMonitoring.Infrastructure`
- `src/IoTMonitoring.API`
- `src/IoTMonitoring.Web`
- `tests/IoTMonitoring.Application.UnitTests`
- `tests/IoTMonitoring.API.IntegrationTests`

## Instalacao e configuracao

### Pre-requisitos
- .NET SDK 8.0+
- Oracle Database (somente para execucao normal da API/Web)

### Configuracoes importantes
Arquivo: `src/IoTMonitoring.API/appsettings.json`
- `ConnectionStrings:DefaultConnection`
- `HealthChecks:ExternalServiceUrl`
- secao `Serilog`

Arquivo: `src/IoTMonitoring.Web/appsettings.json`
- `ConnectionStrings:DefaultConnection`

### Exemplo de connection string (Oracle)
Use o formato abaixo em `ConnectionStrings:DefaultConnection` (API e Web):
`User Id=SEULOGIN;Password=SUASENHA;Data Source=oracle.fiap.com.br:1521/orcl;`

## Comandos uteis

### Restore
```bash
dotnet restore src/IoTMonitoring.API/IoTMonitoring.API.csproj
dotnet restore src/IoTMonitoring.Web/IoTMonitoring.Web.csproj
dotnet restore tests/IoTMonitoring.Application.UnitTests/IoTMonitoring.Application.UnitTests.csproj
dotnet restore tests/IoTMonitoring.API.IntegrationTests/IoTMonitoring.API.IntegrationTests.csproj
```

### Build
```bash
dotnet build src/IoTMonitoring.API/IoTMonitoring.API.csproj
dotnet build src/IoTMonitoring.Web/IoTMonitoring.Web.csproj
```

### Executar API e Web
```bash
dotnet run --project src/IoTMonitoring.API/IoTMonitoring.API.csproj
dotnet run --project src/IoTMonitoring.Web/IoTMonitoring.Web.csproj
```

### Executar testes
```bash
dotnet test tests/IoTMonitoring.Application.UnitTests/IoTMonitoring.Application.UnitTests.csproj
dotnet test tests/IoTMonitoring.API.IntegrationTests/IoTMonitoring.API.IntegrationTests.csproj
```

## Endpoints de monitoramento
- `GET /health` -> retorna saude da aplicacao, banco e servico externo.

## Endpoints funcionais principais
- API Controllers:
  - `api/devices`
  - `api/sensordata`
- Minimal API:
  - `api/v1/devices`
  - `api/v1/sensordata`
