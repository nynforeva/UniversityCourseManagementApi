
using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Users;

public class AssignRoleDto
{
    [Required]
    public string Role { get; set; } = "";
}
