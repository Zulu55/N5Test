using Microsoft.EntityFrameworkCore;
using PermissionsAPI.Models;

namespace PermissionsAPI.Data;

public class PermissionsContext : DbContext
{
    public PermissionsContext(DbContextOptions<PermissionsContext> options) : base(options)
    {
    }

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PermissionType> PermissionTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permission>()
            .HasOne(p => p.PermissionType)
            .WithMany()
            .HasForeignKey(p => p.PermissionTypeId);

        base.OnModelCreating(modelBuilder);
    }
}