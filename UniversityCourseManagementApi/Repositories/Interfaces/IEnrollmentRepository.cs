using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Repositories.Interfaces;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<bool> ExistsAsync(int studentId, int courseId);
}