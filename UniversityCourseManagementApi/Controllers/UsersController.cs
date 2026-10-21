
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityCourseManagementApi.Data;
using UniversityCourseManagementApi.DTO.Users;
using UniversityCourseManagementApi.Models;

namespace UniversityCourseManagementApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Policy = "AdminOnly")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userManager.Users
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.StudentId,
                u.TeacherId
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRole(
        string id, AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound(new { Message = "User not found" });

        if (!await _roleManager.RoleExistsAsync(dto.Role))
            return BadRequest(new { Message = "Role not found" });

        if (await _userManager.IsInRoleAsync(user, dto.Role))
            return Conflict(new { Message = "User already has this role" });

        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Count > 0)
            return Conflict(new { Message = "User already has a role" });

        var result = await _userManager.AddToRoleAsync(user, dto.Role);

        if (!result.Succeeded)
            return BadRequest(new
            {
                Errors = result.Errors.Select(e => e.Description)
            });

        return Ok(new
        {
            Message = "Role assigned successfully",
            UserId = user.Id,
            Role = dto.Role
        });
    }

    [HttpPut("{id}/student")]
    public async Task<IActionResult> LinkStudent(
        string id, LinkProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound(new { Message = "User not found" });

        if (!await _userManager.IsInRoleAsync(user, "Student") ||
            await _userManager.IsInRoleAsync(user, "Admin") ||
            await _userManager.IsInRoleAsync(user, "Teacher"))
            return BadRequest(new { Message = "Invalid user role" });

        if (!await _context.Students.AnyAsync(s => s.Id == dto.ProfileId))
            return NotFound(new { Message = "Student not found" });

        if (await _context.Users.AnyAsync(u =>
            u.StudentId == dto.ProfileId && u.Id != id))
            return Conflict(new { Message = "Student already linked" });

        user.StudentId = dto.ProfileId;
        user.TeacherId = null;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(new
            {
                Errors = result.Errors.Select(e => e.Description)
            });

        return Ok(new
        {
            Message = "Student linked successfully",
            UserId = user.Id,
            StudentId = user.StudentId
        });
    }

    [HttpPut("{id}/teacher")]
    public async Task<IActionResult> LinkTeacher(
        string id, LinkProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound(new { Message = "User not found" });

        if (!await _userManager.IsInRoleAsync(user, "Teacher") ||
            await _userManager.IsInRoleAsync(user, "Admin") ||
            await _userManager.IsInRoleAsync(user, "Student"))
            return BadRequest(new { Message = "Invalid user role" });

        if (!await _context.Teachers.AnyAsync(t => t.Id == dto.ProfileId))
            return NotFound(new { Message = "Teacher not found" });

        if (await _context.Users.AnyAsync(u =>
            u.TeacherId == dto.ProfileId && u.Id != id))
            return Conflict(new { Message = "Teacher already linked" });

        user.TeacherId = dto.ProfileId;
        user.StudentId = null;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(new
            {
                Errors = result.Errors.Select(e => e.Description)
            });

        return Ok(new
        {
            Message = "Teacher linked successfully",
            UserId = user.Id,
            TeacherId = user.TeacherId
        });
    }
}
