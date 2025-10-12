namespace IoTMonitoring.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; } = null!;
        public DbSet<SensorData> SensorData { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.DeviceId).IsUnique();
                entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Location).HasMaxLength(300);
                entity.Property(e => e.Status).IsRequired().HasConversion<string>();
                entity.Property(e => e.LastSeen).IsRequired();

                // Relationship
                entity.HasMany(e => e.SensorData)
                      .WithOne(e => e.Device)
                      .HasForeignKey(e => e.DeviceId)
                      .HasPrincipalKey(e => e.DeviceId);
            });

            modelBuilder.Entity<SensorData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SensorType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Value).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Timestamp).IsRequired();

                entity.HasIndex(e => new { e.DeviceId, e.Timestamp });
            });
        }
    }
}