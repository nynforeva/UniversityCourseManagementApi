using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.Models;

public class Student
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = "";

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = "";

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = "";

    public DateTime BirthDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}