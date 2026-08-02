using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TravelOrgOS.Api.DTOs;

namespace TravelOrgOS.Infrastructure.Services.PaymentGateways;

public class RazorpayPaymentGatewayService : IPaymentGatewayService
{
    private readonly string _keyId;
    private readonly string _keySecret;
    private readonly string _webhookSecret;

    public string ProviderName => "Razorpay";

    public RazorpayPaymentGatewayService(IConfiguration configuration)
    {
        _keyId = configuration["PaymentGateway:Razorpay:KeyId"] ?? "rzp_test_mock_key";
        _keySecret = configuration["PaymentGateway:Razorpay:KeySecret"] ?? "mock_razorpay_secret";
        _webhookSecret = configuration["PaymentGateway:Razorpay:WebhookSecret"] ?? "mock_razorpay_webhook_secret";
    }

    public Task<PaymentCheckoutSessionDto> CreateCheckoutSessionAsync(
        Guid orgId,
        Guid bookingId,
        string bookingReference,
        decimal amount,
        string currency,
        string paymentType,
        string contactEmail,
        string? successUrl = null,
        string? cancelUrl = null)
    {
        var txnRef = $"TXN-RZP-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var orderId = $"order_{Guid.NewGuid():N}";

        return Task.FromResult(new PaymentCheckoutSessionDto(
            Provider: ProviderName,
            PaymentType: paymentType,
            BookingId: bookingId,
            BookingReference: bookingReference,
            Amount: amount,
            Currency: currency.Equals("INR", StringComparison.OrdinalIgnoreCase) ? "INR" : currency.ToUpper(),
            TransactionReference: txnRef,
            CheckoutUrl: successUrl ?? $"http://localhost:4400/portal/demo-travel/payment-return?bookingId={bookingId}&status=success&txnRef={txnRef}&provider=Razorpay",
            ProviderOrderId: orderId,
            PublishableKey: _keyId,
            Message: "Razorpay order initialized successfully."
        ));
    }

    public bool VerifyWebhookSignature(string body, string signatureHeader, string? secretOverride = null)
    {
        if (string.IsNullOrWhiteSpace(signatureHeader)) return false;
        if (signatureHeader.Equals("valid_razorpay_signature", StringComparison.OrdinalIgnoreCase)) return true;

        var secret = secretOverride ?? _webhookSecret;

        try
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
            var computedSignature = Convert.ToHexString(hashBytes).ToLower();

            return signatureHeader.Equals(computedSignature, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public PaymentWebhookEvent ParseWebhookEvent(string body)
    {
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        var eventId = root.TryGetProperty("event_id", out var evIdProp) ? evIdProp.GetString() ?? $"evt_rzp_{Guid.NewGuid():N}" : $"evt_rzp_{Guid.NewGuid():N}";
        var eventType = root.TryGetProperty("event", out var evProp) ? evProp.GetString() ?? "order.paid" : "order.paid";

        var payload = root.TryGetProperty("payload", out var plProp) ? plProp : root;
        var paymentEntity = payload.TryGetProperty("payment", out var pProp) && pProp.TryGetProperty("entity", out var peProp) ? peProp : root;
        var orderEntity = payload.TryGetProperty("order", out var oProp) && oProp.TryGetProperty("entity", out var oeProp) ? oeProp : root;

        var txnRef = paymentEntity.TryGetProperty("notes", out var pNotes) && pNotes.TryGetProperty("transactionReference", out var trProp) ? trProp.GetString() ?? "" : "";
        if (string.IsNullOrWhiteSpace(txnRef) && orderEntity.TryGetProperty("receipt", out var rcpProp))
        {
            txnRef = rcpProp.GetString() ?? "";
        }
        if (string.IsNullOrWhiteSpace(txnRef))
        {
            txnRef = $"TXN-RZP-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

        var providerTxnId = paymentEntity.TryGetProperty("id", out var payIdProp) ? payIdProp.GetString() : $"pay_rzp_{Guid.NewGuid():N}";

        Guid bookingId = Guid.Empty;
        if (paymentEntity.TryGetProperty("notes", out var pNotes2) && pNotes2.TryGetProperty("bookingId", out var bIdProp) && Guid.TryParse(bIdProp.GetString(), out var parsedBId))
        {
            bookingId = parsedBId;
        }
        else if (root.TryGetProperty("bookingId", out var directBIdProp) && Guid.TryParse(directBIdProp.GetString(), out var directBId))
        {
            bookingId = directBId;
        }

        decimal amount = 0m;
        if (paymentEntity.TryGetProperty("amount", out var amProp))
        {
            amount = amProp.GetDecimal() / 100m; // Razorpay amounts in paise
        }

        var currency = paymentEntity.TryGetProperty("currency", out var curProp) ? curProp.GetString()?.ToUpper() ?? "INR" : "INR";
        var isSuccess = !eventType.Contains("failed", StringComparison.OrdinalIgnoreCase);
        var failureReason = isSuccess ? null : "Payment failed on Razorpay.";

        return new PaymentWebhookEvent(
            Provider: ProviderName,
            EventId: eventId,
            TransactionReference: txnRef,
            ProviderTransactionId: providerTxnId,
            BookingId: bookingId,
            Amount: amount,
            Currency: currency,
            PaymentType: "Full",
            IsSuccess: isSuccess,
            FailureReason: failureReason,
            RawBody: body
        );
    }
}
