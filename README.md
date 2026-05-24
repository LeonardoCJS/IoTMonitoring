# IoT Monitoring System — .NET 8

Sistema de monitoramento de dispositivos IoT desenvolvido com Clean Architecture em .NET 8, cobrindo as Sprints 1, 2, 3 e 4.

---

## Integrantes

| Nome | RM |
|------|----|
| Leonardo Carvalho Santos | RM560380 |

---

## Visão Geral

O sistema permite cadastrar dispositivos IoT, ingerir leituras de sensores, emitir alertas e consultar dados históricos com filtros, paginação e ordenação. A API segue os princípios REST com suporte a HATEOAS, autenticação JWT e persistência híbrida (Oracle + MongoDB).

---

## Arquitetura

O projeto adota **Clean Architecture** com separação estrita de responsabilidades:

```
IoTMonitoring-main/
├── src/
│   ├── IoTMonitoring.Domain          # Entidades e interfaces (sem dependências externas)
│   ├── IoTMonitoring.Application     # Serviços, DTOs e regras de negócio
│   ├── IoTMonitoring.Infrastructure  # Repositórios EF Core (Oracle) + MongoDB
│   ├── IoTMonitoring.API             # API REST + Minimal API + Auth JWT
│   └── IoTMonitoring.Web             # Dashboard MVC
└── tests/
    ├── IoTMonitoring.Application.UnitTests      # Testes unitários (xUnit + Moq)
    └── IoTMonitoring.API.IntegrationTests       # Testes de integração (WebApplicationFactory)
```

### Diagrama de Camadas

```
┌─────────────────────────────────────────────┐
│              API / Web (Apresentação)        │
│  Controllers │ Minimal API │ Middleware      │
└────────────────────┬────────────────────────┘
                     │ usa
┌────────────────────▼────────────────────────┐
│              Application                    │
│  Services │ DTOs │ Interfaces               │
└────────────────────┬────────────────────────┘
                     │ usa
┌────────────────────▼────────────────────────┐
│                Domain                       │
│  Entidades │ Interfaces de Repositório      │
└────────────────────┬────────────────────────┘
                     │ implementado por
┌────────────────────▼────────────────────────┐
│             Infrastructure                  │
│  EF Core (Oracle) │ MongoDB.Driver          │
└─────────────────────────────────────────────┘
```

### Princípios aplicados
- **Clean Architecture**: dependências sempre apontam para o centro (Domain)
- **SOLID**: cada classe tem responsabilidade única, interfaces segregadas
- **Repository Pattern**: abstrações `IDeviceRepository`, `ISensorDataRepository`, `ISensorAlertRepository`
- **Injeção de Dependência**: registrada via `AddInfrastructure()` e `AddApplication()`
- **Tratamento de Exceções Global**: middleware `GlobalExceptionHandler` captura e formata erros

---

## Funcionalidades por Sprint

### Sprint 1 — Base e Arquitetura
- Estrutura Clean Architecture com 5 projetos
- Entidades `Device` e `SensorData` com EF Core + Oracle
- Migrações aplicadas
- CRUD completo via Controllers REST (`/api/devices`, `/api/sensordata`)
- Dashboard MVC

### Sprint 2 — Monitoramento e Testes
- Health Checks em `/health` (banco de dados + serviço externo)
- Logging estruturado com Serilog (console + arquivo `logs/log-*.txt`)
- Tracing e métricas com OpenTelemetry
- Testes unitários (xUnit + Moq) com padrão AAA
- Testes de integração com `WebApplicationFactory`

### Sprint 3 — API Avançada
- Minimal API versionada (`/api/v1/devices`, `/api/v1/sensordata`)
- Paginação, ordenação e filtros em todos os endpoints de listagem
- HATEOAS com links hipermídia nas respostas
- Endpoint de busca avançada (`/search`) com query parameters

### Sprint 4 — Consolidação Final
- **Autenticação JWT** (`POST /api/auth/login`) com roles (`Admin`, `User`)
- **Autorização**: endpoints de escrita (POST, PATCH, DELETE) protegidos por `RequireAuthorization()`
- **MongoDB** integrado para alertas de sensores (`/api/v1/alerts`)
- **Global Exception Handler** middleware com respostas no formato `application/problem+json`
- **Swagger completo**: descrições, exemplos, tipos de resposta e autenticação Bearer

---

## Tecnologias

| Categoria | Tecnologia |
|-----------|-----------|
| Framework | .NET 8, ASP.NET Core 8 |
| ORM relacional | Entity Framework Core 8 + Oracle |
| NoSQL | MongoDB.Driver 2.28 |
| Autenticação | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| Documentação | Swashbuckle (Swagger / OpenAPI) |
| Logging | Serilog (console + arquivo) |
| Observabilidade | OpenTelemetry (tracing + métricas) |
| Mapeamento | AutoMapper |
| Testes | xUnit, Moq, WebApplicationFactory, EF InMemory |

---

## Pré-requisitos

- .NET SDK 8.0+
- Oracle Database (FIAP: `oracle.fiap.com.br:1521/orcl`)
- MongoDB (local `localhost:27017` ou Atlas)

---

## Configuração

### `src/IoTMonitoring.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=SEU_RM;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/orcl;"
  },
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "IoTMonitoringDb",
    "AlertsCollectionName": "SensorAlerts"
  },
  "Jwt": {
    "Key": "IoTMonitoring_SuperSecretKey_2024_MinLength32chars!!",
    "Issuer": "IoTMonitoring.API",
    "Audience": "IoTMonitoring.Client",
    "ExpirationHours": "24"
  },
  "Users": [
    { "Username": "admin", "Password": "admin123", "Role": "Admin" },
    { "Username": "user",  "Password": "user123",  "Role": "User"  }
  ]
}
```

> Para MongoDB Atlas, substitua `ConnectionString` pela string de conexão do Atlas.

---

## Instalação e Execução

```bash
# Restaurar dependências
dotnet restore src/IoTMonitoring.API/IoTMonitoring.API.csproj

# Executar a API
dotnet run --project src/IoTMonitoring.API/IoTMonitoring.API.csproj

# Executar o Web (dashboard MVC)
dotnet run --project src/IoTMonitoring.Web/IoTMonitoring.Web.csproj
```

Após iniciar, acesse o Swagger em: `https://localhost:{porta}/swagger`

---

## Testes

```bash
# Testes unitários (Application layer)
dotnet test tests/IoTMonitoring.Application.UnitTests/IoTMonitoring.Application.UnitTests.csproj

# Testes de integração (API)
dotnet test tests/IoTMonitoring.API.IntegrationTests/IoTMonitoring.API.IntegrationTests.csproj
```

### Cobertura de testes
- **Unitários**: `DeviceService` e `SensorDataService` com Moq (repositórios mockados)
  - Cenário de sucesso: criação de dispositivo com payload válido
  - Cenário de erro: adição de sensor para dispositivo inexistente lança `ArgumentException`
- **Integração**: fluxo HTTP completo de criação e consulta de dispositivo com banco in-memory
- **Padrão AAA**: todos os testes seguem Arrange → Act → Assert

---

## Endpoints

### Autenticação

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/auth/login` | Obtém token JWT | Público |

**Exemplo de login:**
```json
POST /api/auth/login
{ "username": "admin", "password": "admin123" }
```

### Dispositivos — Minimal API v1

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| GET | `/api/v1/devices` | Lista todos com HATEOAS | Público |
| GET | `/api/v1/devices/{id}` | Busca por ID com HATEOAS | Público |
| GET | `/api/v1/devices/search` | Busca paginada com filtros | Público |
| POST | `/api/v1/devices` | Cria dispositivo | **JWT** |
| PATCH | `/api/v1/devices/{id}/status` | Atualiza status | **JWT** |
| DELETE | `/api/v1/devices/{id}` | Remove dispositivo | **JWT** |

### Dados de Sensores — Minimal API v1

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| GET | `/api/v1/sensordata/device/{deviceId}` | Leituras por dispositivo | Público |
| GET | `/api/v1/sensordata/search` | Busca paginada com filtros | Público |
| POST | `/api/v1/sensordata` | Registra leitura | **JWT** |
| POST | `/api/v1/sensordata/bulk` | Registra lote de leituras | **JWT** |

### Alertas MongoDB — Minimal API v1

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| GET | `/api/v1/alerts/unacknowledged` | Alertas não reconhecidos | **JWT** |
| GET | `/api/v1/alerts/device/{deviceId}` | Alertas por dispositivo | **JWT** |
| POST | `/api/v1/alerts` | Cria alerta (MongoDB) | **JWT** |
| PATCH | `/api/v1/alerts/{id}/acknowledge` | Reconhece alerta | **JWT** |

### Monitoramento

| Rota | Descrição |
|------|-----------|
| `GET /health` | Status da aplicação, banco e serviço externo |
| `GET /swagger` | Documentação interativa da API |

---

## Persistência de Dados

### Oracle (EF Core)
- **Devices**: cadastro de dispositivos IoT
- **SensorData**: leituras históricas de sensores

### MongoDB
- **SensorAlerts**: alertas gerados quando sensores ultrapassam limites críticos
- Coleção: `SensorAlerts` no banco `IoTMonitoringDb`
- Padrão Repository implementado via `SensorAlertRepository`

---

## Segurança

- **JWT Bearer**: tokens com 24h de validade, assinados com HMAC-SHA256
- **Roles**: `Admin` e `User` (configuráveis no `appsettings.json`)
- **Proteção seletiva**: GET endpoints são públicos; POST/PATCH/DELETE exigem token
- **Exception Handler**: erros não tratados retornam `application/problem+json` sem expor stack traces
