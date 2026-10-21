
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.DTO.Courses;

namespace UniversityCourseManagementApi.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "v2")]
[Route("api/v2/courses")]
public class CoursesV2Controller : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CoursesV2Controller(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _context.Courses
            .AsNoTracking()
            .Select(c => new CourseV2Dto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Credits = c.Credits,
                TeacherId = c.TeacherId,
                TeacherName = c.Teacher.FirstName + " " + c.Teacher.LastName,
                StudentCount = c.Enrollments.Count,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(ReturnResult<List<CourseV2Dto>>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseV2Dto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Credits = c.Credits,
                TeacherId = c.TeacherId,
                TeacherName = c.Teacher.FirstName + " " + c.Teacher.LastName,
                StudentCount = c.Enrollments.Count,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (course == null)
            return NotFound();

        return Ok(ReturnResult<CourseV2Dto>.Success(
            course, 200, HttpContext.TraceIdentifier));
    }
}
