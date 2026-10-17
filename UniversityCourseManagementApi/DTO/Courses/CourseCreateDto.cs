using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Courses;

public class CourseCreateDto
{
    [Required]
    [UniversityCourseManagementApi.Validation.NotBlank]
    [MaxLength(200)]
    public string Name { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";

    [Range(1, 30)]
    public int Credits { get; set; }

    [Range(1, int.MaxValue)]
    public int TeacherId { get; set; }
}