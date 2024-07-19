using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nest;
using PermissionsAPI.Controllers;
using PermissionsAPI.Data;
using PermissionsAPI.DTOs;
using PermissionsAPI.Models;

namespace PermissionsAPI.Tests;

[TestClass]
public class PermissionsControllerTests
{
    private PermissionsContext _context = null!;
    private Mock<IElasticClient> _mockElasticClient = null!;
    private Mock<IProducer<Null, string>> _mockKafkaProducer = null!;
    private PermissionsController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = DbContextFactory.CreateInMemoryContext();
        _mockElasticClient = new Mock<IElasticClient>();
        _mockKafkaProducer = new Mock<IProducer<Null, string>>();
        _controller = new PermissionsController(_context, _mockElasticClient.Object, _mockKafkaProducer.Object);
    }

    [TestMethod]
    public async Task RequestPermission_ReturnsOk()
    {
        var permissionDto = new PermissionDto { EmployeeName = "John Doe", PermissionTypeId = 1, Date = DateTime.UtcNow };

        var result = await _controller.RequestPermission(permissionDto) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var permission = result.Value as Permission;
        Assert.IsNotNull(permission);
        Assert.AreEqual(permissionDto.EmployeeName, permission.EmployeeName);
    }

    [TestMethod]
    public async Task ModifyPermission_ReturnsOk()
    {
        var permission = new Permission { EmployeeName = "John Doe", PermissionTypeId = 1, Date = DateTime.UtcNow };
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        var permissionDto = new PermissionDto { EmployeeName = "Jane Doe", PermissionTypeId = 1, Date = DateTime.UtcNow };

        var result = await _controller.ModifyPermission(permission.Id, permissionDto) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var modifiedPermission = result.Value as Permission;
        Assert.IsNotNull(modifiedPermission);
        Assert.AreEqual(permissionDto.EmployeeName, modifiedPermission.EmployeeName);
    }

    [TestMethod]
    public async Task GetPermissions_ReturnsOk()
    {
        var permissions = new List<Permission>
    {
        new Permission { EmployeeName = "John Doe", PermissionTypeId = 1, Date = DateTime.UtcNow },
        new Permission { EmployeeName = "Jane Doe", PermissionTypeId = 2, Date = DateTime.UtcNow }
    };

        _context.Permissions.AddRange(permissions);
        await _context.SaveChangesAsync();

        var result = await _controller.GetPermissions() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var returnedPermissions = result.Value as List<Permission>;
        Assert.IsNotNull(returnedPermissions);
        Assert.AreEqual(permissions.Count, returnedPermissions.Count);
    }
}