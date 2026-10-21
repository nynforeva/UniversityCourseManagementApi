
using System.ComponentModel.DataAnnotations;

namespace UniversityCourseManagementApi.DTO.Users;

public class LinkProfileDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProfileId { get; set; }
}
