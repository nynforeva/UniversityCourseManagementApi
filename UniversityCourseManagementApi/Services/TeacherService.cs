using AutoMapper;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Teachers;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Repositories.Interfaces;

namespace UniversityCourseManagementApi.Services;

public class TeacherService
{
    private readonly ITeacherRepository _teachers;
    private readonly IMapper _mapper;
    private readonly ILogger<TeacherService> _logger;

    public TeacherService(
        ITeacherRepository teachers,
        IMapper mapper,
        ILogger<TeacherService> logger)
    {
        _teachers = teachers;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<TeacherDto>> GetAllAsync()
    {
        var teachers = await _teachers.GetAllAsync();
        return _mapper.Map<List<TeacherDto>>(teachers);
    }

    public async Task<TeacherDto> GetByIdAsync(int id)
    {
        var teacher = await _teachers.GetByIdAsync(id);

        if (teacher == null)
        {
            _logger.LogWarning("Teacher {Id} not found", id);
            throw new AppException(404, "TEACHER_NOT_FOUND",
                "Teacher not found");
        }

        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> CreateAsync(TeacherCreateDto dto)
    {
        if (await _teachers.EmailExistsAsync(dto.Email))
            throw new AppException(409, "EMAIL_EXISTS",
                "Teacher email already exists");

        var teacher = _mapper.Map<Teacher>(dto);

        await _teachers.AddAsync(teacher);

        _logger.LogInformation("Teacher {Id} created", teacher.Id);

        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> UpdateAsync(
        int id, TeacherUpdateDto dto)
    {
        var teacher = await _teachers.GetByIdAsync(id);

        if (teacher == null)
        {
            _logger.LogWarning("Teacher {Id} not found", id);
            throw new AppException(404, "TEACHER_NOT_FOUND",
                "Teacher not found");
        }

        if (await _teachers.EmailExistsAsync(dto.Email, id))
            throw new AppException(409, "EMAIL_EXISTS",
                "Teacher email already exists");

        _mapper.Map(dto, teacher);

        await _teachers.UpdateAsync(teacher);

        _logger.LogInformation("Teacher {Id} updated", id);

        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var teacher = await _teachers.GetByIdAsync(id);

        if (teacher == null)
        {
            _logger.LogWarning("Teacher {Id} not found", id);
            throw new AppException(404, "TEACHER_NOT_FOUND",
                "Teacher not found");
        }

        await _teachers.DeleteAsync(teacher);

        _logger.LogInformation("Teacher {Id} deleted", id);

        return true;
    }
}