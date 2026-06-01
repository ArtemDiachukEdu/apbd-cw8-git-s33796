using Microsoft.AspNetCore.Mvc;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Services;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _submissionService;

    public SubmissionsController(SubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    public async Task<ActionResult<SubmissionDto>> Create(
        [FromBody] CreateSubmissionDto dto,
        CancellationToken cancellationToken = default)
    {
        var (result, error, statusCode) = await _submissionService.CreateAsync(dto, cancellationToken);

        if (error is not null)
        {
            return StatusCode(statusCode, new { message = error });
        }

        return StatusCode(statusCode, result);
    }

    [HttpPut("{idSubmission}/grade")]
    public async Task<ActionResult<SubmissionDto>> Grade(
        int idSubmission,
        [FromBody] GradeSubmissionDto dto,
        CancellationToken cancellationToken = default)
    {
        var (result, error, statusCode) = await _submissionService.GradeAsync(
            idSubmission,
            dto,
            cancellationToken);

        if (error is not null)
        {
            return StatusCode(statusCode, new { message = error });
        }

        return Ok(result);
    }

    [HttpDelete("{idSubmission}")]
    public async Task<IActionResult> Delete(
        int idSubmission,
        CancellationToken cancellationToken = default)
    {
        var (error, statusCode) = await _submissionService.DeleteAsync(idSubmission, cancellationToken);

        if (error is not null)
        {
            return StatusCode(statusCode, new { message = error });
        }

        return NoContent();
    }
}
