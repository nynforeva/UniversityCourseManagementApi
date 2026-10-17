using AutoMapper;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.DTO.Students;
using UniversityCourseManagementApi.DTO.Teachers;
using UniversityCourseManagementApi.DTO.Courses;
using UniversityCourseManagementApi.DTO.Enrollments;

namespace UniversityCourseManagementApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentDto>()
            .ForMember(d => d.Courses, o => o.MapFrom(s =>
                s.Enrollments.Select(e => new StudentCourseDto
                {
                    CourseId = e.CourseId,
                    CourseName = e.Course.Name,
                    Grade = e.Grade
                })));

        CreateMap<StudentCreateDto, Student>()
            .ForMember(d => d.BirthDate,
                o => o.MapFrom(s => s.BirthDate!.Value));

        CreateMap<StudentUpdateDto, Student>()
            .ForMember(d => d.BirthDate,
                o => o.MapFrom(s => s.BirthDate!.Value));

        CreateMap<Teacher, TeacherDto>();
        CreateMap<TeacherCreateDto, Teacher>();
        CreateMap<TeacherUpdateDto, Teacher>();

        CreateMap<Course, CourseDto>();
        CreateMap<CourseCreateDto, Course>();
        CreateMap<CourseUpdateDto, Course>();

        CreateMap<Enrollment, EnrollmentDto>();
        CreateMap<EnrollmentCreateDto, Enrollment>();
        CreateMap<EnrollmentUpdateDto, Enrollment>();
    }
}