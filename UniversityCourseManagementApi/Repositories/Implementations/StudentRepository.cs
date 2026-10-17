using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Repositories.Implementations;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetWithCoursesAsync(int id)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        return await _context.Students.AnyAsync(s =>
            s.Email == email &&
            (!excludeId.HasValue || s.Id != excludeId.Value));
    }
}
