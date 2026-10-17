using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Repositories.Implementations;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Course>> FilterAsync(string? name, int? teacherId)
    {
        var query = _context.Courses.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }

        if (teacherId.HasValue)
        {
            query = query.Where(c => c.TeacherId == teacherId.Value);
        }

        return await query.ToListAsync();
    }
}