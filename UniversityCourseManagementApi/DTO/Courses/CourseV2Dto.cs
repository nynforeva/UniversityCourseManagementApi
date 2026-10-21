
namespace UniversityCourseManagementApi.DTO.Courses;

public class CourseV2Dto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Credits { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = "";
    public int StudentCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
