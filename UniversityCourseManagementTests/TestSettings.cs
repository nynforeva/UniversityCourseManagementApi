
namespace UniversityCourseManagementTests;

public static class TestSettings
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("TEST_API_URL")
        ?? "http://localhost:5132/";

    public static string AdminUser =>
        Environment.GetEnvironmentVariable("TEST_ADMIN_USER")
        ?? "admin1";

    public static string AdminPassword =>
        Required("TEST_ADMIN_PASSWORD");

    public static string StudentUser =>
        Environment.GetEnvironmentVariable("TEST_STUDENT_USER")
        ?? "student1";

    public static string StudentPassword =>
        Required("TEST_STUDENT_PASSWORD");

    public static string TeacherUser =>
        Environment.GetEnvironmentVariable("TEST_TEACHER_USER")
        ?? "teacher1";

    public static string TeacherPassword =>
        Required("TEST_TEACHER_PASSWORD");

    public static int OwnEnrollmentId =>
        RequiredInt("TEST_OWN_ENROLLMENT_ID");

    public static int OtherEnrollmentId =>
        RequiredInt("TEST_OTHER_ENROLLMENT_ID");

    public static int TeacherOwnEnrollmentId =>
        RequiredInt("TEST_TEACHER_OWN_ENROLLMENT_ID");

    public static int TeacherOtherEnrollmentId =>
        RequiredInt("TEST_TEACHER_OTHER_ENROLLMENT_ID");

    private static string Required(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Missing environment variable: {name}");
        }

        return value;
    }

    private static int RequiredInt(string name)
    {
        var value = Required(name);

        if (!int.TryParse(value, out var number) || number <= 0)
        {
            throw new InvalidOperationException(
                $"Invalid value for {name}");
        }

        return number;
    }
}
