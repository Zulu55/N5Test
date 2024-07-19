namespace PermissionsAPI.DTOs;

public class PermissionDto
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = null!;
    public int PermissionTypeId { get; set; }
    public DateTime Date { get; set; }
}