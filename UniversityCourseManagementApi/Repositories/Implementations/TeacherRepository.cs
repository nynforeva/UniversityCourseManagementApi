using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Repositories.Implementations;

public class TeacherRepository : Repository<Teacher>, ITeacherRepository
{
    public TeacherRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        return await _context.Teachers.AnyAsync(t =>
            t.Email == email &&
            (!excludeId.HasValue || t.Id != excludeId.Value));
    }
}