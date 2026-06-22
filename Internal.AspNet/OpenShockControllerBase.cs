using Microsoft.AspNetCore.Mvc;
using OpenShock.Internal.Common.Problems;

namespace OpenShock.Internal.Common;

public class OpenShockControllerBase : ControllerBase
{
    [NonAction]
    public ObjectResult Problem(OpenShockProblem problem) => problem.ToObjectResult(HttpContext);
}
