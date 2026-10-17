namespace UniversityCourseManagementApi.DTO.Students;

public class StudentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<StudentCourseDto> Courses { get; set; } = new();
}
