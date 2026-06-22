using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OpenShock.Internal.Common.Problems;

/// <summary>
/// Represents a problem
/// </summary>
public class OpenShockProblem : ProblemDetails
{
    public OpenShockProblem(string type, string title, HttpStatusCode status = HttpStatusCode.BadRequest,
        string? detail = null) : this(type, title, (int)status, detail)
    {
    }

    private OpenShockProblem(string type, string title, int status = 400, string? detail = null)
    {
        Type = type;
        Title = title;
        Detail = detail;
        Status = status;
    }

    [Obsolete("This is the exact same as title or detail if present, refer to using title in the future")]
    public string Message => Detail ?? Title!;

    [Obsolete("This is the exact same as requestId, refer to using requestId in the future")]
    public string? TraceId => RequestId;

    public string? RequestId { get; set; }

    public ObjectResult ToObjectResult(HttpContext context)
    {
        RequestId = context.TraceIdentifier;

        return new ObjectResult(this)
        {
            StatusCode = Status
        };
    }

    public Task WriteAsJsonAsync(HttpContext context, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = Status ?? StatusCodes.Status400BadRequest;
        RequestId = context.TraceIdentifier;

        return context.Response.WriteAsJsonAsync(this, jsonSerializerOptions, MediaTypeNames.Application.ProblemJson, cancellationToken);
    }
}
