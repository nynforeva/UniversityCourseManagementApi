using AutoMapper;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Courses;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Services;

public class CourseService
{
    private readonly ICourseRepository _courses;
    private readonly ITeacherRepository _teachers;
    private readonly IMapper _mapper;
    private readonly ILogger<CourseService> _logger;

    public CourseService(
        ICourseRepository courses,
        ITeacherRepository teachers,
        IMapper mapper,
        ILogger<CourseService> logger)
    {
        _courses = courses;
        _teachers = teachers;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<CourseDto>> GetAllAsync(
        string? name, int? teacherId)
    {
        var courses = await _courses.FilterAsync(name, teacherId);
        return _mapper.Map<List<CourseDto>>(courses);
    }

    public async Task<CourseDto> GetByIdAsync(int id)
    {
        var course = await _courses.GetByIdAsync(id);

        if (course == null)
        {
            _logger.LogWarning("Course {Id} not found", id);
            throw new AppException(404, "COURSE_NOT_FOUND",
                "Course not found");
        }

        return _mapper.Map<CourseDto>(course);
    }

    public async Task<CourseDto> CreateAsync(CourseCreateDto dto)
    {
        if (await _teachers.GetByIdAsync(dto.TeacherId) == null)
            throw new AppException(404, "TEACHER_NOT_FOUND",
                "Teacher not found");

        var course = _mapper.Map<Course>(dto);
        course.CreatedAt = DateTime.UtcNow;

        await _courses.AddAsync(course);

        _logger.LogInformation("Course {Id} created", course.Id);

        return _mapper.Map<CourseDto>(course);
    }

    public async Task<CourseDto> UpdateAsync(
        int id, CourseUpdateDto dto)
    {
        var course = await _courses.GetByIdAsync(id);

        if (course == null)
        {
            _logger.LogWarning("Course {Id} not found", id);
            throw new AppException(404, "COURSE_NOT_FOUND",
                "Course not found");
        }

        if (await _teachers.GetByIdAsync(dto.TeacherId) == null)
            throw new AppException(404, "TEACHER_NOT_FOUND",
                "Teacher not found");

        _mapper.Map(dto, course);

        await _courses.UpdateAsync(course);

        _logger.LogInformation("Course {Id} updated", id);

        return _mapper.Map<CourseDto>(course);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _courses.GetByIdAsync(id);

        if (course == null)
        {
            _logger.LogWarning("Course {Id} not found", id);
            throw new AppException(404, "COURSE_NOT_FOUND",
                "Course not found");
        }

        await _courses.DeleteAsync(course);

        _logger.LogInformation("Course {Id} deleted", id);

        return true;
    }
}