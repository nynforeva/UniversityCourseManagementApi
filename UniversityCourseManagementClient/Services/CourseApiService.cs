
using System.Net.Http.Json;
using System.Text.Json;
using UniversityCourseManagementClient.Models;

namespace UniversityCourseManagementClient.Services;

public class CourseApiService
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<CourseApiService> _logger;

    public CourseApiService(
        IHttpClientFactory factory,
        ILogger<CourseApiService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<(List<CourseViewModel> Courses, string? Error)> GetCoursesAsync()
    {
        try
        {
            var client = _factory.CreateClient("UniversityApi");

            using var response = await client.GetAsync("api/courses");

            if (!response.IsSuccessStatusCode)
            {
                return (new List<CourseViewModel>(),
                    $"API returned HTTP {(int)response.StatusCode}");
            }

            var data = await response.Content
                .ReadFromJsonAsync<ApiResponse<List<CourseViewModel>>>();

            if (data == null || !data.IsSuccess || data.Result == null)
            {
                return (new List<CourseViewModel>(),
                    data?.ErrorMessage ?? "Invalid API response");
            }

            return (data.Result, null);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Cannot connect to University API");

            return (new List<CourseViewModel>(),
                "Cannot connect to University API");
        }
        catch (TaskCanceledException)
        {
            return (new List<CourseViewModel>(),
                "University API request timed out");
        }
        catch (JsonException)
        {
            return (new List<CourseViewModel>(),
                "Invalid JSON received from API");
        }
    }

    public async Task<(CourseViewModel? Course, string? Error)> GetCourseAsync(int id)
    {
        try
        {
            var client = _factory.CreateClient("UniversityApi");

            using var response = await client.GetAsync($"api/courses/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return (null, "Course not found");

            if (!response.IsSuccessStatusCode)
                return (null, $"API returned HTTP {(int)response.StatusCode}");

            var data = await response.Content
                .ReadFromJsonAsync<ApiResponse<CourseViewModel>>();

            if (data == null || !data.IsSuccess || data.Result == null)
                return (null, data?.ErrorMessage ?? "Invalid API response");

            return (data.Result, null);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Cannot connect to University API");

            return (null, "Cannot connect to University API");
        }
        catch (TaskCanceledException)
        {
            return (null, "University API request timed out");
        }
        catch (JsonException)
        {
            return (null, "Invalid JSON received from API");
        }
    }
}
