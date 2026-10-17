using AutoMapper;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Enrollments;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Services;

public class EnrollmentService
{
    private readonly IEnrollmentRepository _enrollments;
    private readonly IStudentRepository _students;
    private readonly ICourseRepository _courses;
    private readonly IMapper _mapper;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(
        IEnrollmentRepository enrollments,
        IStudentRepository students,
        ICourseRepository courses,
        IMapper mapper,
        ILogger<EnrollmentService> logger)
    {
        _enrollments = enrollments;
        _students = students;
        _courses = courses;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<EnrollmentDto>> GetAllAsync()
    {
        var enrollments = await _enrollments.GetAllAsync();
        return _mapper.Map<List<EnrollmentDto>>(enrollments);
    }

    public async Task<EnrollmentDto> GetByIdAsync(int id)
    {
        var enrollment = await _enrollments.GetByIdAsync(id);

        if (enrollment == null)
        {
            _logger.LogWarning("Enrollment {Id} not found", id);
            throw new AppException(404, "ENROLLMENT_NOT_FOUND",
                "Enrollment not found");
        }

        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<EnrollmentDto> CreateAsync(
        EnrollmentCreateDto dto)
    {
        if (await _students.GetByIdAsync(dto.StudentId) == null)
            throw new AppException(404, "STUDENT_NOT_FOUND",
                "Student not found");

        if (await _courses.GetByIdAsync(dto.CourseId) == null)
            throw new AppException(404, "COURSE_NOT_FOUND",
                "Course not found");

        if (await _enrollments.ExistsAsync(
            dto.StudentId, dto.CourseId))
            throw new AppException(409, "ALREADY_ENROLLED",
                "Student is already enrolled in this course");

        var enrollment = _mapper.Map<Enrollment>(dto);
        enrollment.EnrollmentDate = DateTime.UtcNow;

        await _enrollments.AddAsync(enrollment);

        _logger.LogInformation(
            "Student {StudentId} enrolled in course {CourseId}",
            dto.StudentId, dto.CourseId);

        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<EnrollmentDto> UpdateAsync(
        int id, EnrollmentUpdateDto dto)
    {
        var enrollment = await _enrollments.GetByIdAsync(id);

        if (enrollment == null)
        {
            _logger.LogWarning("Enrollment {Id} not found", id);
            throw new AppException(404, "ENROLLMENT_NOT_FOUND",
                "Enrollment not found");
        }

        _mapper.Map(dto, enrollment);

        await _enrollments.UpdateAsync(enrollment);

        _logger.LogInformation(
            "Enrollment {Id} grade updated", id);

        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var enrollment = await _enrollments.GetByIdAsync(id);

        if (enrollment == null)
        {
            _logger.LogWarning("Enrollment {Id} not found", id);
            throw new AppException(404, "ENROLLMENT_NOT_FOUND",
                "Enrollment not found");
        }

        await _enrollments.DeleteAsync(enrollment);

        _logger.LogInformation("Enrollment {Id} deleted", id);

        return true;
    }
}