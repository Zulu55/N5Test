using Microsoft.EntityFrameworkCore;
using PermissionsAPI.Data;

namespace PermissionsAPI.Tests;

public static class DbContextFactory
{
    public static PermissionsContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<PermissionsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PermissionsContext(options);
    }
}