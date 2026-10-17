using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Enrollments;

public class EnrollmentUpdateDto
{
    [Range(0, 100)]
    public int? Grade { get; set; }
}