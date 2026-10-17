namespace UniversityCourseManagementApi.DTO.Enrollments;

public class EnrollmentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public int? Grade { get; set; }
}