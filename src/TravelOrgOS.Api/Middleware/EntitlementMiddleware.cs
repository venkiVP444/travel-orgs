using System;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Middleware;

public class EntitlementMiddleware
{
    private readonly RequestDelegate _next;

    public EntitlementMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";
        var method = context.Request.Method;

        // Quota rules interceptor for resource creations
        if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            var user = context.User;
            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                var orgIdClaim = user.FindFirst("OrganizationId")?.Value;
                if (Guid.TryParse(orgIdClaim, out var orgId))
                {
                    var subService = context.RequestServices.GetRequiredService<ISubscriptionService>();

                    if (path.StartsWith("/api/trips", StringComparison.OrdinalIgnoreCase))
                    {
                        var allowed = await subService.CanCreateTripAsync(orgId);
                        if (!allowed)
                        {
                            await WriteQuotaExceededResponse(context, "Active Trips Limit Exceeded", "Your current subscription tier does not allow creating any more active trips. Please upgrade to expand your limits.");
                            return;
                        }
                    }
                    else if (path.StartsWith("/api/bookings", StringComparison.OrdinalIgnoreCase))
                    {
                        var allowed = await subService.CanCreateBookingAsync(orgId);
                        if (!allowed)
                        {
                            await WriteQuotaExceededResponse(context, "Monthly Bookings Limit Exceeded", "Your current subscription tier does not allow processing any more bookings this month. Please upgrade to expand your limits.");
                            return;
                        }
                    }
                    else if (path.StartsWith("/api/team/invite", StringComparison.OrdinalIgnoreCase))
                    {
                        var allowed = await subService.CanInviteMemberAsync(orgId);
                        if (!allowed)
                        {
                            await WriteQuotaExceededResponse(context, "Team Members Limit Exceeded", "Your current subscription tier does not allow inviting any more team members. Please upgrade to expand your limits.");
                            return;
                        }
                    }
                }
            }
        }

        await _next(context);
    }

    private static async Task WriteQuotaExceededResponse(HttpContext context, string title, string message)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";

        var payload = new
        {
            ErrorCode = "SUBSCRIPTION_LIMIT_EXCEEDED",
            Title = title,
            Message = message,
            UpgradeRequired = true
        };

        var json = JsonSerializer.Serialize(payload);
        await context.Response.WriteAsync(json);
    }
}
