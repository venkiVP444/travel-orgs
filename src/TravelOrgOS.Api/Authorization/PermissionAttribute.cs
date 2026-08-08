using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TravelOrgOS.Domain.Enums;

namespace TravelOrgOS.Api.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequiresPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public string Permission { get; }

    public RequiresPermissionAttribute(string permission)
    {
        Permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
        if (!Enum.TryParse<UserRole>(roleClaim, out var role))
        {
            context.Result = new ForbidResult();
            return;
        }

        if (!HasPermission(role, Permission))
        {
            context.Result = new ForbidResult();
            return;
        }
    }

    private static bool HasPermission(UserRole role, string permission)
    {
        // PlatformAdmin and OrganizationOwner have all permissions
        if (role == UserRole.PlatformAdmin || role == UserRole.OrganizationOwner)
        {
            return true;
        }

        return permission switch
        {
            "Trip.View" => true,
            "Trip.Create" or "Trip.Edit" or "Trip.Publish" => role == UserRole.OrganizationAdmin || role == UserRole.TripCoordinator,
            "Booking.View" => role == UserRole.OrganizationAdmin || role == UserRole.TripCoordinator || role == UserRole.FinanceUser,
            "Booking.Create" or "Booking.Cancel" => role == UserRole.OrganizationAdmin || role == UserRole.TripCoordinator,
            "Payment.View" or "Finance.View" => role == UserRole.FinanceUser || role == UserRole.OrganizationAdmin,
            "Payment.Record" or "Payment.Refund" => role == UserRole.FinanceUser || role == UserRole.OrganizationAdmin,
            "Vendor.Manage" or "Vehicle.Manage" or "Guide.Manage" => role == UserRole.OrganizationAdmin || role == UserRole.TripCoordinator,
            "Team.View" => role == UserRole.OrganizationAdmin || role == UserRole.FinanceUser || role == UserRole.TripCoordinator,
            "Team.Manage" => role == UserRole.OrganizationAdmin,
            "Marketing.Manage" => role == UserRole.OrganizationAdmin || role == UserRole.TripCoordinator,
            "Subscription.View" => role == UserRole.OrganizationAdmin || role == UserRole.FinanceUser,
            "Subscription.Manage" => false,
            _ => false
        };
    }
}
