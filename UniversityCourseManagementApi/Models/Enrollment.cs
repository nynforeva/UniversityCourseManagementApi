using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.Models;

public class Enrollment
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public Student Student { get; set; } = null!;

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    [Range(0, 100)]
    public int? Grade { get; set; }
}