
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace UniversityCourseManagementTests;

public class ApiSecurityTests
{
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri(TestSettings.BaseUrl),
        Timeout = TimeSpan.FromSeconds(15)
    };

    private static async Task<string> LoginAsync(
        string userName, string password)
    {
        using var response = await Client.PostAsJsonAsync(
            "api/auth/login",
            new { userName, password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var root = document.RootElement;

        if (root.TryGetProperty("result", out var result) &&
            result.ValueKind == JsonValueKind.Object)
        {
            root = result;
        }

        string? token = null;

        if (root.TryGetProperty("accessToken", out var accessToken))
            token = accessToken.GetString();
        else if (root.TryGetProperty("token", out var tokenValue))
            token = tokenValue.GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        return token!;
    }

    private static async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        string? token = null,
        object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        if (body != null)
            request.Content = JsonContent.Create(body);

        return await Client.SendAsync(request);
    }

    private static async Task<string> AdminTokenAsync()
    {
        return await LoginAsync(
            TestSettings.AdminUser,
            TestSettings.AdminPassword);
    }

    private static async Task<string> StudentTokenAsync()
    {
        return await LoginAsync(
            TestSettings.StudentUser,
            TestSettings.StudentPassword);
    }

    private static async Task<string> TeacherTokenAsync()
    {
        return await LoginAsync(
            TestSettings.TeacherUser,
            TestSettings.TeacherPassword);
    }

    [Fact]
    public async Task Test01_OldCoursesApiWorks()
    {
        using var response = await SendAsync(
            HttpMethod.Get, "api/courses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Test02_VersionOneWorks()
    {
        using var response = await SendAsync(
            HttpMethod.Get, "api/v1/courses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Test03_VersionTwoWorks()
    {
        using var response = await SendAsync(
            HttpMethod.Get, "api/v2/courses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Test04_VersionTwoContainsExtendedFields()
    {
        using var response = await SendAsync(
            HttpMethod.Get, "api/v2/courses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var courses = document.RootElement.GetProperty("result");

        Assert.Equal(JsonValueKind.Array, courses.ValueKind);
        Assert.True(courses.GetArrayLength() > 0);

        var course = courses[0];

        Assert.True(course.TryGetProperty("teacherName", out _));
        Assert.True(course.TryGetProperty("studentCount", out _));
    }

    [Fact]
    public async Task Test05_UnauthorizedUserCannotAccessProfile()
    {
        using var response = await SendAsync(
            HttpMethod.Get, "api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test06_InvalidTokenIsRejected()
    {
        using var response = await SendAsync(
            HttpMethod.Get,
            "api/auth/me",
            "invalid.jwt.token");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test07_StudentCanLogin()
    {
        var token = await StudentTokenAsync();

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Test08_TeacherCanLogin()
    {
        var token = await TeacherTokenAsync();

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Test09_AdminCanLogin()
    {
        var token = await AdminTokenAsync();

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Test10_StudentCanReadOwnProfile()
    {
        var token = await StudentTokenAsync();

        using var response = await SendAsync(
            HttpMethod.Get, "api/auth/me", token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Test11_AdminCanViewUsers()
    {
        var token = await AdminTokenAsync();

        using var response = await SendAsync(
            HttpMethod.Get, "api/users", token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Test12_StudentCannotViewUsers()
    {
        var token = await StudentTokenAsync();

        using var response = await SendAsync(
            HttpMethod.Get, "api/users", token);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Test13_StudentCanSeeOwnEnrollment()
    {
        var token = await StudentTokenAsync();

        using var response = await SendAsync(
            HttpMethod.Get,
            $"api/enrollments/{TestSettings.OwnEnrollmentId}",
            token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Test14_StudentCannotSeeAnotherEnrollment()
    {
        var token = await StudentTokenAsync();

        using var response = await SendAsync(
            HttpMethod.Get,
            $"api/enrollments/{TestSettings.OtherEnrollmentId}",
            token);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Test15_TeacherCanUpdateOwnCourseGrade()
    {
        var token = await TeacherTokenAsync();

        using var response = await SendAsync(
            HttpMethod.Put,
            $"api/enrollments/{TestSettings.TeacherOwnEnrollmentId}",
            token,
            new { grade = 95 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Test16_TeacherCannotUpdateAnotherCourseGrade()
    {
        var token = await TeacherTokenAsync();

        using var response = await SendAsync(
            HttpMethod.Put,
            $"api/enrollments/{TestSettings.TeacherOtherEnrollmentId}",
            token,
            new { grade = 50 });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Test17_StudentCannotCreateEnrollment()
    {
        var token = await StudentTokenAsync();

        using var response = await SendAsync(
            HttpMethod.Post,
            "api/enrollments",
            token,
            new { studentId = 1, courseId = 1 });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Test18_AnonymousCannotCreateEnrollment()
    {
        using var response = await SendAsync(
            HttpMethod.Post,
            "api/enrollments",
            null,
            new { studentId = 1, courseId = 1 });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test19_StudentCannotDeleteEnrollment()
    {
        var token = await StudentTokenAsync();

        using var response = await SendAsync(
            HttpMethod.Delete,
            $"api/enrollments/{TestSettings.OwnEnrollmentId}",
            token);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Test20_TeacherCanReadOwnEnrollment()
    {
        var token = await TeacherTokenAsync();

        using var response = await SendAsync(
            HttpMethod.Get,
            $"api/enrollments/{TestSettings.TeacherOwnEnrollmentId}",
            token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
