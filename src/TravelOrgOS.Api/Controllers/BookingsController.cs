using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IConfiguration _configuration;

    public BookingsController(IBookingService bookingService, IConfiguration configuration)
    {
        _bookingService = bookingService;
        _configuration = configuration;
    }

    private Guid GetOrgId()
    {
        var claim = User.FindFirst("OrganizationId")?.Value;
        if (Guid.TryParse(claim, out var orgId)) return orgId;
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings([FromQuery] string? search)
    {
        var bookings = await _bookingService.GetBookingsAsync(GetOrgId(), search);
        return Ok(bookings);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(GetOrgId(), id);
        if (booking == null) return NotFound();
        return Ok(booking);
    }

    [HttpGet("ref/{reference}")]
    public async Task<IActionResult> GetBookingByReference(string reference)
    {
        var booking = await _bookingService.GetBookingByReferenceAsync(reference);
        if (booking == null) return NotFound();
        return Ok(booking);
    }

    [HttpGet("{id:guid}/payment-status")]
    public async Task<IActionResult> GetBookingPaymentStatus(Guid id)
    {
        var booking = await _bookingService.GetBookingByReferenceAsync(id.ToString());
        if (booking == null)
        {
            var allBookings = await _bookingService.GetBookingsAsync(GetOrgId());
            booking = allBookings.FirstOrDefault(b => b.Id == id);
        }
        if (booking == null) return NotFound();

        return Ok(new
        {
            bookingId = booking.Id,
            bookingReference = booking.BookingReference,
            totalAmount = booking.TotalAmount,
            paidAmount = booking.PaidAmount,
            balanceAmount = booking.BalanceAmount,
            paymentStatus = booking.PaymentStatus,
            bookingStatus = booking.BookingStatus,
            payments = booking.Payments
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        try
        {
            var result = await _bookingService.CreateBookingAsync(GetOrgId(), dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("portal/{orgSlug}")]
    public async Task<IActionResult> CreatePortalBooking(string orgSlug, [FromBody] CreateBookingDto dto)
    {
        var masterDataService = HttpContext.RequestServices.GetRequiredService<IMasterDataService>();
        var org = await masterDataService.GetOrganizationBySlugAsync(orgSlug);
        if (org == null) return NotFound(new { message = "Organization not found." });

        try
        {
            var result = await _bookingService.CreateBookingAsync(org.Id, dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> InitiatePayment(Guid id, [FromBody] InitiatePaymentSessionDto dto)
    {
        try
        {
            var requestDto = dto with { BookingId = id };
            var session = await _bookingService.InitiatePaymentSessionAsync(GetOrgId(), requestDto);
            return Ok(session);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("portal/{orgSlug}/{id:guid}/pay")]
    public async Task<IActionResult> InitiatePortalPayment(string orgSlug, Guid id, [FromBody] InitiatePaymentSessionDto dto)
    {
        var masterDataService = HttpContext.RequestServices.GetRequiredService<IMasterDataService>();
        var org = await masterDataService.GetOrganizationBySlugAsync(orgSlug);
        if (org == null) return NotFound(new { message = "Organization not found." });

        try
        {
            var requestDto = dto with { BookingId = id };
            var session = await _bookingService.InitiatePaymentSessionAsync(org.Id, requestDto);
            return Ok(session);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> ConfirmBooking(Guid id)
    {
        var result = await _bookingService.ConfirmBookingAsync(GetOrgId(), id);
        if (!result) return NotFound();
        return Ok(new { message = "Booking confirmed." });
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var result = await _bookingService.CancelBookingAsync(GetOrgId(), id);
        if (!result) return NotFound();
        return Ok(new { message = "Booking cancelled and seats restored." });
    }

    [HttpPost("{id:guid}/payment")]
    public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordPaymentDto dto)
    {
        var result = await _bookingService.RecordPaymentAsync(GetOrgId(), id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("payment/verify-razorpay")]
    public async Task<IActionResult> VerifyRazorpayPayment([FromBody] RazorpayVerificationDto dto)
    {
        var secret = _configuration["PaymentGateway:Razorpay:KeySecret"] ?? "mock_razorpay_secret";
        
        var payload = $"{dto.OrderId}|{dto.PaymentId}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = Convert.ToHexString(hashBytes).ToLower();

        if (!computedSignature.Equals(dto.Signature, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Payment signature verification failed." });
        }

        var eventId = $"evt_verify_rzp_{dto.PaymentId}";
        var webhookEvent = new PaymentWebhookEvent(
            Provider: "Razorpay",
            EventId: eventId,
            TransactionReference: dto.TransactionReference,
            ProviderTransactionId: dto.PaymentId,
            BookingId: dto.BookingId,
            Amount: dto.Amount,
            Currency: "INR",
            PaymentType: dto.PaymentType,
            IsSuccess: true,
            FailureReason: null,
            RawBody: "Direct client verification"
        );

        var processed = await _bookingService.ProcessGatewayPaymentWebhookAsync(webhookEvent);
        if (!processed)
        {
            return BadRequest(new { message = "Failed to update booking ledger." });
        }

        return Ok(new { success = true });
    }
}
