using Microsoft.AspNetCore.Mvc;

namespace HttpToGrpcProxy.Controllers;
[ApiController]
public class EverythingController : ControllerBase
{
    private readonly ILogger<EverythingController> _logger;

    public EverythingController(ILogger<EverythingController> logger)
    {
        _logger = logger;
    }

    [Route("{**route}")]
    [HttpGet, HttpPost, HttpDelete, HttpHead, HttpOptions, HttpPatch, HttpPut]
    public IActionResult Everything(string route)
    {
        return Ok(new { Route = route, Method = Request.Method });
    }
}
