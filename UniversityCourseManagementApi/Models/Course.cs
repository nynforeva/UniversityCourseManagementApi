using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";

    [Range(1, 30)]
    public int Credits { get; set; }

    public int TeacherId { get; set; }

    public Teacher Teacher { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}