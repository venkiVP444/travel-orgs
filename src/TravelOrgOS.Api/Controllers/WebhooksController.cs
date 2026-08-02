using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Infrastructure.Services;
using TravelOrgOS.Infrastructure.Services.PaymentGateways;

namespace TravelOrgOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class WebhooksController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IPaymentGatewayFactory _gatewayFactory;

    public WebhooksController(IBookingService bookingService, IPaymentGatewayFactory gatewayFactory)
    {
        _bookingService = bookingService;
        _gatewayFactory = gatewayFactory;
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        var signatureHeader = Request.Headers["Stripe-Signature"].ToString();
        var stripeGateway = _gatewayFactory.GetGateway("Stripe");

        if (string.IsNullOrWhiteSpace(signatureHeader) || !stripeGateway.VerifyWebhookSignature(body, signatureHeader))
        {
            return BadRequest(new { error = "Invalid or missing Stripe signature header." });
        }

        var webhookEvent = stripeGateway.ParseWebhookEvent(body);
        var processed = await _bookingService.ProcessGatewayPaymentWebhookAsync(webhookEvent);

        if (!processed)
        {
            return BadRequest(new { error = "Failed to process payment event or target booking not found." });
        }

        return Ok(new { received = true, eventId = webhookEvent.EventId });
    }

    [HttpPost("razorpay")]
    public async Task<IActionResult> HandleRazorpayWebhook()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        var signatureHeader = Request.Headers["X-Razorpay-Signature"].ToString();
        var razorpayGateway = _gatewayFactory.GetGateway("Razorpay");

        if (string.IsNullOrWhiteSpace(signatureHeader) || !razorpayGateway.VerifyWebhookSignature(body, signatureHeader))
        {
            return BadRequest(new { error = "Invalid or missing Razorpay signature header." });
        }

        var webhookEvent = razorpayGateway.ParseWebhookEvent(body);
        var processed = await _bookingService.ProcessGatewayPaymentWebhookAsync(webhookEvent);

        if (!processed)
        {
            return BadRequest(new { error = "Failed to process payment event or target booking not found." });
        }

        return Ok(new { status = "ok", eventId = webhookEvent.EventId });
    }
}
