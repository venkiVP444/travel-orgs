using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelOrgOS.Api.Controllers;

[Authorize]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected Guid GetOrgId()
    {
        var claim = User.FindFirst("OrganizationId")?.Value;
        if (Guid.TryParse(claim, out var orgId))
        {
            return orgId;
        }

        // Allow PlatformAdmin to fall back to the default demo organization ID
        if (User.IsInRole("PlatformAdmin"))
        {
            return Guid.Parse("11111111-1111-1111-1111-111111111111");
        }
        
        throw new UnauthorizedAccessException("Organization context is missing or invalid.");
    }
}
