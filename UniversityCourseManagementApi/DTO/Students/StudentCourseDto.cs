namespace UniversityCourseManagementApi.DTO.Students;

public class StudentCourseDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = "";
    public int? Grade { get; set; }
}