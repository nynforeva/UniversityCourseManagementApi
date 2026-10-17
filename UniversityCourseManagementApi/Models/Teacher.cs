using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.Models;

public class Teacher
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

    [Required]
    [MaxLength(150)]
    public string Department { get; set; } = "";

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}