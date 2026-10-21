
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.DTO.Enrollments;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Services;

namespace UniversityCourseManagementApi.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly EnrollmentService _service;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EnrollmentsController(
        EnrollmentService service,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _service = service;
        _context = context;
        _userManager = userManager;
    }

    private async Task<ApplicationUser?> CurrentUserAsync()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(id))
            return null;

        return await _userManager.FindByIdAsync(id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var user = await CurrentUserAsync();

        if (user == null)
            return Unauthorized();

        var query = _context.Enrollments.AsNoTracking();

        if (!User.IsInRole("Admin"))
        {
            if (User.IsInRole("Teacher") && user.TeacherId.HasValue)
            {
                query = query.Where(e =>
                    e.Course.TeacherId == user.TeacherId.Value);
            }
            else if (User.IsInRole("Student") && user.StudentId.HasValue)
            {
                query = query.Where(e =>
                    e.StudentId == user.StudentId.Value);
            }
            else
            {
                return Forbid();
            }
        }

        var result = await query.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            StudentId = e.StudentId,
            CourseId = e.CourseId,
            EnrollmentDate = e.EnrollmentDate,
            Grade = e.Grade
        }).ToListAsync();

        return Ok(ReturnResult<List<EnrollmentDto>>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await CurrentUserAsync();

        if (user == null)
            return Unauthorized();

        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment == null)
            return NotFound();

        if (!User.IsInRole("Admin"))
        {
            bool isStudentOwner =
                User.IsInRole("Student") &&
                user.StudentId.HasValue &&
                user.StudentId.Value == enrollment.StudentId;

            bool isCourseTeacher =
                User.IsInRole("Teacher") &&
                user.TeacherId.HasValue &&
                user.TeacherId.Value == enrollment.Course.TeacherId;

            if (!isStudentOwner && !isCourseTeacher)
                return Forbid();
        }

        var result = new EnrollmentDto
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollmentDate = enrollment.EnrollmentDate,
            Grade = enrollment.Grade
        };

        return Ok(ReturnResult<EnrollmentDto>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(EnrollmentCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);

        return StatusCode(201,
            ReturnResult<EnrollmentDto>.Success(
                result, 201, HttpContext.TraceIdentifier));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> Update(
        int id, EnrollmentUpdateDto dto)
    {
        var user = await CurrentUserAsync();

        if (user == null)
            return Unauthorized();

        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment == null)
            return NotFound();

        if (!User.IsInRole("Admin"))
        {
            bool isCourseTeacher =
                User.IsInRole("Teacher") &&
                user.TeacherId.HasValue &&
                user.TeacherId.Value == enrollment.Course.TeacherId;

            if (!isCourseTeacher)
                return Forbid();
        }

        var result = await _service.UpdateAsync(id, dto);

        return Ok(ReturnResult<EnrollmentDto>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        return Ok(ReturnResult<bool>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }
}
