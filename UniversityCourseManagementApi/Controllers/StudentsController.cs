
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Students;
using UniversityCourseManagementApi.Models;
using UniversityCourseManagementApi.Services;

namespace UniversityCourseManagementApi.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly StudentService _service;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentsController(
        StudentService service,
        UserManager<ApplicationUser> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();

        return Ok(ReturnResult<List<StudentDto>>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId ?? "");

            if (user == null || user.StudentId != id)
                return Forbid();
        }

        var result = await _service.GetByIdAsync(id);

        return Ok(ReturnResult<StudentDto>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(StudentCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);

        return StatusCode(201,
            ReturnResult<StudentDto>.Success(
                result, 201, HttpContext.TraceIdentifier));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(
        int id, StudentUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        return Ok(ReturnResult<StudentDto>.Success(
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
