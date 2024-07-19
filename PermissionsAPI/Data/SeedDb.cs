using Microsoft.EntityFrameworkCore;
using PermissionsAPI.Models;

namespace PermissionsAPI.Data;

public class SeedDb
{
    private readonly PermissionsContext _context;

    public SeedDb(PermissionsContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckPermissionTypesAsync();
        await CheckPermissionsAsync();
    }

    private async Task CheckPermissionTypesAsync()
    {
        if (!_context.PermissionTypes.Any())
        {
            _context.PermissionTypes.Add(new PermissionType { Description = "Read" });
            _context.PermissionTypes.Add(new PermissionType { Description = "Write" });
            _context.PermissionTypes.Add(new PermissionType { Description = "Modify" });
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckPermissionsAsync()
    {
        if (!_context.Permissions.Any())
        {
            var permissionType = await _context.PermissionTypes.FirstOrDefaultAsync();
            _context.Permissions.Add(new Permission { Date = DateTime.UtcNow, EmployeeName = "Juan Zuluaga", PermissionType = permissionType! });
            _context.Permissions.Add(new Permission { Date = DateTime.UtcNow, EmployeeName = "Joe Biden", PermissionType = permissionType! });
            _context.Permissions.Add(new Permission { Date = DateTime.UtcNow, EmployeeName = "Donald Trump", PermissionType = permissionType! });
            await _context.SaveChangesAsync();
        }
    }
}