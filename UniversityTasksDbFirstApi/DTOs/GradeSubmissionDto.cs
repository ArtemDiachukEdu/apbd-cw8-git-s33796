using System.ComponentModel.DataAnnotations;

namespace UniversityTasksDbFirstApi.DTOs;

public class GradeSubmissionDto
{
    [Required]
    public int Score { get; set; }

    public string? Feedback { get; set; }
}
