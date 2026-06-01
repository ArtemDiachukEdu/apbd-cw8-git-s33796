using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Models;

namespace UniversityTasksDbFirstApi.Services;

public class SubmissionService
{
    private readonly UniversityTasksDbContext _context;

    public SubmissionService(UniversityTasksDbContext context)
    {
        _context = context;
    }

    public async Task<(SubmissionDto? Result, string? Error, int StatusCode)> CreateAsync(
        CreateSubmissionDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.RepositoryUrl) ||
            !dto.RepositoryUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return (null, "RepositoryUrl must not be blank and must start with https://.", 400);
        }

        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == dto.StudentId, cancellationToken);

        if (student is null)
        {
            return (null, "Student not found.", 404);
        }

        if (!student.IsActive)
        {
            return (null, "Student is not active.", 400);
        }

        var assignment = await _context.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AssignmentId == dto.AssignmentId, cancellationToken);

        if (assignment is null)
        {
            return (null, "Assignment not found.", 404);
        }

        if (!assignment.IsPublished)
        {
            return (null, "Assignment is not published.", 400);
        }

        var isEnrolled = await _context.Enrollments.AnyAsync(
            e => e.StudentId == dto.StudentId &&
                 e.CourseId == assignment.CourseId &&
                 (e.Status == "Active" || e.Status == "Completed"),
            cancellationToken);

        if (!isEnrolled)
        {
            return (null, "Student is not enrolled in the course for this assignment.", 400);
        }

        var duplicateExists = await _context.Submissions.AnyAsync(
            s => s.AssignmentId == dto.AssignmentId && s.StudentId == dto.StudentId,
            cancellationToken);

        if (duplicateExists)
        {
            return (null, "A submission for this assignment already exists.", 409);
        }

        var now = DateTime.UtcNow;
        var status = assignment.IsOverdue(now) ? "Late" : "Submitted";

        var submission = new Submission
        {
            AssignmentId = dto.AssignmentId,
            StudentId = dto.StudentId,
            RepositoryUrl = dto.RepositoryUrl.Trim(),
            SubmittedAt = now,
            Status = status
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync(cancellationToken);

        return (await MapToDtoAsync(submission.SubmissionId, cancellationToken), null, 201);
    }

    public async Task<(SubmissionDto? Result, string? Error, int StatusCode)> GradeAsync(
        int submissionId,
        GradeSubmissionDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Score < 0)
        {
            return (null, "Score cannot be lower than 0.", 400);
        }

        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId, cancellationToken);

        if (submission is null)
        {
            return (null, "Submission not found.", 404);
        }

        if (dto.Score > submission.Assignment.MaxPoints)
        {
            return (null, $"Score cannot be higher than the assignment maximum of {submission.Assignment.MaxPoints}.", 400);
        }

        submission.Score = dto.Score;
        submission.Feedback = dto.Feedback;
        submission.Status = "Graded";

        await _context.SaveChangesAsync(cancellationToken);

        return (MapToDto(submission), null, 200);
    }

    public async Task<(string? Error, int StatusCode)> DeleteAsync(
        int submissionId,
        CancellationToken cancellationToken = default)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId, cancellationToken);

        if (submission is null)
        {
            return ("Submission not found.", 404);
        }

        if (submission.Status == "Graded")
        {
            return ("A graded submission cannot be deleted.", 400);
        }

        _context.Submissions.Remove(submission);
        await _context.SaveChangesAsync(cancellationToken);

        return (null, 204);
    }

    private async Task<SubmissionDto?> MapToDtoAsync(int submissionId, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions
            .AsNoTracking()
            .Include(s => s.Student)
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId, cancellationToken);

        return submission is null ? null : MapToDto(submission);
    }

    private static SubmissionDto MapToDto(Submission submission)
    {
        return new SubmissionDto
        {
            SubmissionId = submission.SubmissionId,
            Student = new StudentRefDto
            {
                StudentId = submission.Student.StudentId,
                IndexNumber = submission.Student.IndexNumber,
                FullName = submission.Student.FullName
            },
            Assignment = new AssignmentRefDto
            {
                AssignmentId = submission.Assignment.AssignmentId,
                Title = submission.Assignment.Title
            },
            RepositoryUrl = submission.RepositoryUrl,
            Status = submission.Status,
            Score = submission.Score,
            Feedback = submission.Feedback
        };
    }
}
