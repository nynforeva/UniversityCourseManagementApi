using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Repositories.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<List<Course>> FilterAsync(string? name, int? teacherId);
}