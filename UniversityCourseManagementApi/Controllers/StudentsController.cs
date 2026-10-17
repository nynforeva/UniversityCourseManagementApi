using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Students;
using UniversityCourseManagementApi.Services;

namespace UniversityCourseManagementApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly StudentService _service;

    public StudentsController(StudentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ReturnResult<List<StudentDto>>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ReturnResult<StudentDto>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    public async Task<IActionResult> Create(StudentCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById),
            new { id = result.Id },
            ReturnResult<StudentDto>.Success(
                result, 201, HttpContext.TraceIdentifier));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, StudentUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(ReturnResult<StudentDto>.Success(
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