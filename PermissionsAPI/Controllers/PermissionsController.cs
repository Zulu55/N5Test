using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using PermissionsAPI.Data;
using PermissionsAPI.DTOs;
using PermissionsAPI.Models;

namespace PermissionsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionsController : ControllerBase
{
    private readonly PermissionsContext _context;
    private readonly IElasticClient _elasticClient;
    private readonly IProducer<Null, string> _producer;

    public PermissionsController(PermissionsContext context, IElasticClient elasticClient, IProducer<Null, string> producer)
    {
        _context = context;
        _elasticClient = elasticClient;
        _producer = producer;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestPermission(PermissionDto dto)
    {
        var permission = new Permission
        {
            EmployeeName = dto.EmployeeName,
            PermissionTypeId = dto.PermissionTypeId,
            Date = dto.Date
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        await _elasticClient.IndexDocumentAsync(permission);
        await _producer.ProduceAsync("permissions", new Message<Null, string> { Value = "request" });

        return Ok(permission);
    }

    [HttpPut("modify/{id}")]
    public async Task<IActionResult> ModifyPermission(int id, PermissionDto dto)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null) return NotFound();

        permission.EmployeeName = dto.EmployeeName;
        permission.PermissionTypeId = dto.PermissionTypeId;
        permission.Date = dto.Date;

        await _context.SaveChangesAsync();

        await _elasticClient.IndexDocumentAsync(permission);
        await _producer.ProduceAsync("permissions", new Message<Null, string> { Value = "modify" });

        return Ok(permission);
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await _context.Permissions.ToListAsync();
        await _producer.ProduceAsync("permissions", new Message<Null, string> { Value = "get" });
        return Ok(permissions);
    }
}