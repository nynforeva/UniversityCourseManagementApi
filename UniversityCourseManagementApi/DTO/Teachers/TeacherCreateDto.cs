using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Teachers;

public class TeacherCreateDto
{
    [Required]
    [UniversityCourseManagementApi.Validation.NotBlank]
    [MaxLength(100)]
    public string FirstName { get; set; } = "";

    [Required]
    [UniversityCourseManagementApi.Validation.NotBlank]
    [MaxLength(100)]
    public string LastName { get; set; } = "";

    [Required]
    [UniversityCourseManagementApi.Validation.NotBlank]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [UniversityCourseManagementApi.Validation.NotBlank]
    [MaxLength(150)]
    public string Department { get; set; } = "";
}