using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Courses;
using UniversityCourseManagementApi.Services;

namespace UniversityCourseManagementApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly CourseService _service;

    public CoursesController(CourseService service)
    {
        _service = service;
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] int? teacherId)
    {
        var result = await _service.GetAllAsync(name, teacherId);
        return Ok(ReturnResult<List<CourseDto>>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ReturnResult<CourseDto>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create(CourseCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById),
            new { id = result.Id },
            ReturnResult<CourseDto>.Success(
                result, 201, HttpContext.TraceIdentifier));
    }
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, CourseUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(ReturnResult<CourseDto>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return Ok(ReturnResult<bool>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }
}