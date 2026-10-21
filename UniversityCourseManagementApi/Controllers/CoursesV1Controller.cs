
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.Data;

namespace UniversityCourseManagementApi.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/courses")]
public class CoursesV1Controller : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CoursesV1Controller(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _context.Courses
            .AsNoTracking()
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.Credits,
                c.TeacherId,
                c.CreatedAt
            })
            .ToListAsync();

        return Ok(ReturnResult<object>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.Credits,
                c.TeacherId,
                c.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (course == null)
            return NotFound();

        return Ok(ReturnResult<object>.Success(
            course, 200, HttpContext.TraceIdentifier));
    }
}
