using IoTMonitoring.Domain.Entities;
using IoTMonitoring.Domain.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace IoTMonitoring.Infrastructure.MongoDB
{
    internal class SensorAlertDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string DeviceId { get; set; } = string.Empty;
        public string SensorType { get; set; } = string.Empty;
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string AlertLevel { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool Acknowledged { get; set; }
    }

    public class SensorAlertRepository : ISensorAlertRepository
    {
        private readonly IMongoCollection<SensorAlertDocument> _collection;

        public SensorAlertRepository(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<SensorAlertDocument>(settings.Value.AlertsCollectionName);
        }

        public async Task<IEnumerable<SensorAlert>> GetByDeviceIdAsync(string deviceId)
        {
            var docs = await _collection
                .Find(d => d.DeviceId == deviceId)
                .SortByDescending(d => d.CreatedAt)
                .ToListAsync();
            return docs.Select(MapToDomain);
        }

        public async Task<IEnumerable<SensorAlert>> GetUnacknowledgedAsync()
        {
            var docs = await _collection
                .Find(d => !d.Acknowledged)
                .SortByDescending(d => d.CreatedAt)
                .ToListAsync();
            return docs.Select(MapToDomain);
        }

        public async Task AddAsync(SensorAlert alert)
        {
            var doc = MapToDocument(alert);
            await _collection.InsertOneAsync(doc);
            alert.Id = doc.Id;
        }

        public async Task AcknowledgeAsync(string id)
        {
            var filter = Builders<SensorAlertDocument>.Filter.Eq(d => d.Id, id);
            var update = Builders<SensorAlertDocument>.Update.Set(d => d.Acknowledged, true);
            await _collection.UpdateOneAsync(filter, update);
        }

        private static SensorAlert MapToDomain(SensorAlertDocument doc) => new()
        {
            Id = doc.Id,
            DeviceId = doc.DeviceId,
            SensorType = doc.SensorType,
            Value = doc.Value,
            Unit = doc.Unit,
            AlertLevel = doc.AlertLevel,
            Message = doc.Message,
            CreatedAt = doc.CreatedAt,
            Acknowledged = doc.Acknowledged
        };

        private static SensorAlertDocument MapToDocument(SensorAlert alert) => new()
        {
            DeviceId = alert.DeviceId,
            SensorType = alert.SensorType,
            Value = alert.Value,
            Unit = alert.Unit,
            AlertLevel = alert.AlertLevel,
            Message = alert.Message,
            CreatedAt = alert.CreatedAt == default ? DateTime.UtcNow : alert.CreatedAt,
            Acknowledged = alert.Acknowledged
        };
    }
}
