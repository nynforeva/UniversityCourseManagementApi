
using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Auth;

public class RegisterDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

