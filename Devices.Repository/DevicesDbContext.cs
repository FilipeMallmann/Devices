using System;
using Microsoft.EntityFrameworkCore;
using Devices.Domain;

namespace Devices.Infrastructure.Db
{
    public class DevicesDbContext : DbContext
    {
        public DevicesDbContext(DbContextOptions<DevicesDbContext> options)
            : base(options)
        {
        }

        public DbSet<DeviceModel> Devices { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeviceModel>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired().HasMaxLength(100);
                b.Property(x => x.Brand).IsRequired().HasMaxLength(100);
                b.Property(x => x.CreationTime).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}