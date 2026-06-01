using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly UniversityTasksDbContext _context;

    public StudentsController(UniversityTasksDbContext context)
    {
        _context = context;
    }

    [HttpGet("{idStudent}/dashboard")]
    public async Task<ActionResult<StudentDashboardDto>> GetDashboard(
        int idStudent,
        CancellationToken cancellationToken = default)
    {
        var dashboard = await _context.Students
            .AsNoTracking()
            .Where(s => s.StudentId == idStudent)
            .Select(s => new StudentDashboardDto
            {
                StudentId = s.StudentId,
                IndexNumber = s.IndexNumber,
                FullName = s.FirstName + " " + s.LastName,
                IsActive = s.IsActive,
                Enrollments = s.Enrollments
                    .Select(e => new EnrollmentSummaryDto
                    {
                        EnrollmentId = e.EnrollmentId,
                        CourseId = e.CourseId,
                        CourseCode = e.Course.Code,
                        CourseName = e.Course.Name,
                        EnrolledAt = e.EnrolledAt,
                        Status = e.Status
                    })
                    .ToList(),
                Submissions = s.Submissions
                    .Select(sub => new SubmissionSummaryDto
                    {
                        SubmissionId = sub.SubmissionId,
                        AssignmentId = sub.AssignmentId,
                        AssignmentTitle = sub.Assignment.Title,
                        RepositoryUrl = sub.RepositoryUrl,
                        SubmittedAt = sub.SubmittedAt,
                        Status = sub.Status,
                        Score = sub.Score,
                        Feedback = sub.Feedback
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dashboard is null)
        {
            return NotFound();
        }

        return Ok(dashboard);
    }
}
