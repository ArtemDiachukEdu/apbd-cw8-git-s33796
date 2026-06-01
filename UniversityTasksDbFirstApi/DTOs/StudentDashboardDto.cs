namespace UniversityTasksDbFirstApi.DTOs;

public class StudentDashboardDto
{
    public int StudentId { get; set; }

    public string IndexNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public bool IsActive { get; set; }

    public List<EnrollmentSummaryDto> Enrollments { get; set; } = [];

    public List<SubmissionSummaryDto> Submissions { get; set; } = [];
}

public class EnrollmentSummaryDto
{
    public int EnrollmentId { get; set; }

    public int CourseId { get; set; }

    public string CourseCode { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public DateOnly EnrolledAt { get; set; }

    public string Status { get; set; } = null!;
}

public class SubmissionSummaryDto
{
    public int SubmissionId { get; set; }

    public int AssignmentId { get; set; }

    public string AssignmentTitle { get; set; } = null!;

    public string RepositoryUrl { get; set; } = null!;

    public DateTime SubmittedAt { get; set; }

    public string Status { get; set; } = null!;

    public int? Score { get; set; }

    public string? Feedback { get; set; }
}
