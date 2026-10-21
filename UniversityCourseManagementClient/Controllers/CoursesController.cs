
using Microsoft.AspNetCore.Mvc;
using UniversityCourseManagementClient.Services;

namespace UniversityCourseManagementClient.Controllers;

public class CoursesController : Controller
{
    private readonly CourseApiService _service;

    public CoursesController(CourseApiService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var (courses, error) = await _service.GetCoursesAsync();

        ViewBag.Error = error;

        return View(courses);
    }

    public async Task<IActionResult> Details(int id)
    {
        var (course, error) = await _service.GetCourseAsync(id);

        ViewBag.Error = error;

        return View(course);
    }
}
