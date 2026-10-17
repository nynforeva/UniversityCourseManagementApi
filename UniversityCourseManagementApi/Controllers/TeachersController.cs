using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Teachers;
using UniversityCourseManagementApi.Services;

namespace UniversityCourseManagementApi.Controllers;

[ApiController]
[Route("api/teachers")]
public class TeachersController : ControllerBase
{
    private readonly TeacherService _service;

    public TeachersController(TeacherService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ReturnResult<List<TeacherDto>>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ReturnResult<TeacherDto>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    public async Task<IActionResult> Create(TeacherCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById),
            new { id = result.Id },
            ReturnResult<TeacherDto>.Success(
                result, 201, HttpContext.TraceIdentifier));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, TeacherUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(ReturnResult<TeacherDto>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return Ok(ReturnResult<bool>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }
}
