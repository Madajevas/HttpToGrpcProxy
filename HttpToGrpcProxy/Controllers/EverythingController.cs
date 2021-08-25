using HttpToGrpcProxy.Services;

using Microsoft.AspNetCore.Mvc;

namespace HttpToGrpcProxy.Controllers;
[ApiController]
public class EverythingController : ControllerBase
{
    private readonly ProxyService proxy;
    private readonly ILogger<EverythingController> _logger;

    public EverythingController(ProxyService proxy, ILogger<EverythingController> logger)
    {
        this.proxy = proxy;
        _logger = logger;
    }

    [Route("{**route}")]
    [HttpGet, HttpPost, HttpDelete, HttpHead, HttpOptions, HttpPatch, HttpPut]
    public IActionResult Everything(string route)
    {
        return Ok(new { Route = route, Method = Request.Method });
    }
}
