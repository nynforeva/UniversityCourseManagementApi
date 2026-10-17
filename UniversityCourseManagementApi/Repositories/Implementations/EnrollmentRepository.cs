using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Repositories.Implementations;

public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(int studentId, int courseId)
    {
        return await _context.Enrollments.AnyAsync(e =>
            e.StudentId == studentId && e.CourseId == courseId);
    }
}