using System.ComponentModel.DataAnnotations;
using UniversityCourseManagementApi.Validation;

namespace UniversityCourseManagementApi.DTO.Students;

public class StudentUpdateDto
{
    [Required]
    [NotBlank]
    [MaxLength(100)]
    public string FirstName { get; set; } = "";

    [Required]
    [NotBlank]
    [MaxLength(100)]
    public string LastName { get; set; } = "";

    [Required]
    [NotBlank]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = "";

    [Required]
    public DateTime? BirthDate { get; set; }
}