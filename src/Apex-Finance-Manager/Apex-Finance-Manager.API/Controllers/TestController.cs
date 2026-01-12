using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Apex_Finance_Manager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [Authorize] // This is the magic attribute that protects the route
    [HttpGet("secure-data")]
    public IActionResult GetSecureData()
    {
        // Extract the user's name from the token claims to prove it works
        var userName = User.FindFirst(ClaimTypes.GivenName)?.Value;

        return Ok(new
        {
            message = $"Hello {userName}, you have accessed protected data!",
            timestamp = DateTime.UtcNow
        });
    }
}
