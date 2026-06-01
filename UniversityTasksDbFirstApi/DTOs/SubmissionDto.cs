namespace UniversityTasksDbFirstApi.DTOs;

public class SubmissionDto
{
    public int SubmissionId { get; set; }

    public StudentRefDto Student { get; set; } = null!;

    public AssignmentRefDto Assignment { get; set; } = null!;

    public string RepositoryUrl { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int? Score { get; set; }

    public string? Feedback { get; set; }
}

public class StudentRefDto
{
    public int StudentId { get; set; }

    public string IndexNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;
}

public class AssignmentRefDto
{
    public int AssignmentId { get; set; }

    public string Title { get; set; } = null!;
}
