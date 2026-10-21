
namespace UniversityCourseManagementApi.DTO.Auth;

public class TokenDto
{
    public string AccessToken { get; set; } = "";
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
}
