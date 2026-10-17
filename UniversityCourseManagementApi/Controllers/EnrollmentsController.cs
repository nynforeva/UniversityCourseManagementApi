using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementApi.Common;
using UniversityCourseManagementApi.DTO.Enrollments;
using UniversityCourseManagementApi.Services;

namespace UniversityCourseManagementApi.Controllers;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly EnrollmentService _service;

    public EnrollmentsController(EnrollmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ReturnResult<List<EnrollmentDto>>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ReturnResult<EnrollmentDto>.Success(
            result, 200, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    public async Task<IActionResult> Create(EnrollmentCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById),
            new { id = result.Id },
            ReturnResult<EnrollmentDto>.Success(
                result, 201, HttpContext.TraceIdentifier));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, EnrollmentUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(ReturnResult<EnrollmentDto>.Success(
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