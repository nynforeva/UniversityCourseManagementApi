using AutoMapper;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Students;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Services;

public class StudentService
{
    private readonly IStudentRepository _students;
    private readonly IMapper _mapper;
    private readonly ILogger<StudentService> _logger;

    public StudentService(
        IStudentRepository students,
        IMapper mapper,
        ILogger<StudentService> logger)
    {
        _students = students;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<StudentDto>> GetAllAsync()
    {
        var students = await _students.GetAllAsync();
        return _mapper.Map<List<StudentDto>>(students);
    }

    public async Task<StudentDto> GetByIdAsync(int id)
    {
        var student = await _students.GetWithCoursesAsync(id);

        if (student == null)
        {
            _logger.LogWarning("Student {Id} not found", id);
            throw new AppException(404, "STUDENT_NOT_FOUND",
                "Student not found");
        }

        return _mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> CreateAsync(StudentCreateDto dto)
    {
        if (await _students.EmailExistsAsync(dto.Email))
            throw new AppException(409, "EMAIL_EXISTS",
                "Student email already exists");

        var student = _mapper.Map<Student>(dto);
        student.CreatedAt = DateTime.UtcNow;

        await _students.AddAsync(student);

        _logger.LogInformation("Student {Id} created", student.Id);

        return await GetByIdAsync(student.Id);
    }

    public async Task<StudentDto> UpdateAsync(
        int id, StudentUpdateDto dto)
    {
        var student = await _students.GetByIdAsync(id);

        if (student == null)
        {
            _logger.LogWarning("Student {Id} not found", id);
            throw new AppException(404, "STUDENT_NOT_FOUND",
                "Student not found");
        }

        if (await _students.EmailExistsAsync(dto.Email, id))
            throw new AppException(409, "EMAIL_EXISTS",
                "Student email already exists");

        _mapper.Map(dto, student);

        await _students.UpdateAsync(student);

        _logger.LogInformation("Student {Id} updated", id);

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _students.GetByIdAsync(id);

        if (student == null)
        {
            _logger.LogWarning("Student {Id} not found", id);
            throw new AppException(404, "STUDENT_NOT_FOUND",
                "Student not found");
        }

        await _students.DeleteAsync(student);

        _logger.LogInformation("Student {Id} deleted", id);

        return true;
    }
}
