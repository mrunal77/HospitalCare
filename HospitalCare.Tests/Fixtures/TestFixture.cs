using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HospitalCare.Tests.Fixtures;

public class TestFixture
{
    public Mock<T> GetMock<T>() where T : class
    {
        return new Mock<T>();
    }

    public ControllerContext CreateControllerContextWithClaims(IEnumerable<Claim>? claims = null, string? scheme = "Bearer", string? token = "test-token")
    {
        claims ??= new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Email, "test@example.com"),
            new(ClaimTypes.GivenName, "Test"),
            new(ClaimTypes.Surname, "User"),
            new(ClaimTypes.Role, "Admin")
        };

        var claimsIdentity = new ClaimsIdentity(claims, scheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };
    }

    public ControllerContext CreateControllerContextWithoutAuth()
    {
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    public static T? GetControllerResult<T>(ActionResult<T> result)
    {
        if (result.Result is ObjectResult objectResult)
        {
            return (T?)objectResult.Value;
        }
        return result.Value;
    }

    public static ObjectResult? GetObjectResult(ActionResult result)
    {
        if (result is ObjectResult objectResult)
        {
            return objectResult;
        }
        if (result is NotFoundObjectResult notFoundResult)
        {
            return notFoundResult;
        }
        if (result is BadRequestObjectResult badRequestResult)
        {
            return badRequestResult;
        }
        if (result is UnauthorizedObjectResult unauthorizedResult)
        {
            return unauthorizedResult;
        }
        return null;
    }
}