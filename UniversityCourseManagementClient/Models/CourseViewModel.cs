
namespace UniversityCourseManagementClient.Models;

public class CourseViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Credits { get; set; }
    public int TeacherId { get; set; }
    public DateTime CreatedAt { get; set; }
}
