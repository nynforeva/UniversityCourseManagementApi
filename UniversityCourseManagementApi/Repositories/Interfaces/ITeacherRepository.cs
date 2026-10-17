using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Repositories.Interfaces;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}