namespace PermissionsAPI.Models;

public class Permission
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = null!;
    public int PermissionTypeId { get; set; }
    public DateTime Date { get; set; }
    public PermissionType PermissionType { get; set; } = null!;
}