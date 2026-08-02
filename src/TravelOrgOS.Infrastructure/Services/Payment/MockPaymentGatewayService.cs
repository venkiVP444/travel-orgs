using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TravelOrgOS.Api.DTOs;

namespace TravelOrgOS.Infrastructure.Services.PaymentGateways;

public class MockPaymentGatewayService : IPaymentGatewayService
{
    public string ProviderName => "Mock";

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
        var txnRef = $"TXN-MOCK-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var mockCheckoutUrl = successUrl ?? $"http://localhost:4400/portal/demo-travel/payment-return?bookingId={bookingId}&status=success&txnRef={txnRef}";

        return Task.FromResult(new PaymentCheckoutSessionDto(
            Provider: ProviderName,
            PaymentType: paymentType,
            BookingId: bookingId,
            BookingReference: bookingReference,
            Amount: amount,
            Currency: currency,
            TransactionReference: txnRef,
            CheckoutUrl: mockCheckoutUrl,
            ProviderOrderId: $"mock_order_{Guid.NewGuid():N}",
            PublishableKey: "mock_pk_test_12345",
            Message: "Mock payment session initialized successfully."
        ));
    }

    public bool VerifyWebhookSignature(string body, string signatureHeader, string? secretOverride = null)
    {
        if (string.IsNullOrWhiteSpace(signatureHeader)) return false;
        if (signatureHeader.Equals("valid_mock_signature", StringComparison.OrdinalIgnoreCase)) return true;

        // Verify SHA256 HMAC for test payloads if signature header follows mock-signature format
        var secret = secretOverride ?? "mock_webhook_secret";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(body))).ToLower();
        
        return signatureHeader.Equals(hash, StringComparison.OrdinalIgnoreCase) || signatureHeader.Contains(hash, StringComparison.OrdinalIgnoreCase);
    }

    public PaymentWebhookEvent ParseWebhookEvent(string body)
    {
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        var eventId = root.TryGetProperty("eventId", out var evProp) ? evProp.GetString() ?? $"evt_mock_{Guid.NewGuid():N}" : $"evt_mock_{Guid.NewGuid():N}";
        var txnRef = root.TryGetProperty("transactionReference", out var txProp) ? txProp.GetString() ?? "TXN-MOCK-UNKNOWN" : "TXN-MOCK-UNKNOWN";
        var providerTxnId = root.TryGetProperty("providerTransactionId", out var ptxProp) ? ptxProp.GetString() : $"pi_mock_{Guid.NewGuid():N}";
        var bookingIdStr = root.TryGetProperty("bookingId", out var bkProp) ? bkProp.GetString() : null;
        var bookingId = Guid.TryParse(bookingIdStr, out var bId) ? bId : Guid.Empty;
        var amount = root.TryGetProperty("amount", out var amProp) ? amProp.GetDecimal() : 0m;
        var currency = root.TryGetProperty("currency", out var curProp) ? curProp.GetString() ?? "USD" : "USD";
        var paymentType = root.TryGetProperty("paymentType", out var ptProp) ? ptProp.GetString() ?? "Full" : "Full";
        var isSuccess = !root.TryGetProperty("status", out var stProp) || !stProp.GetString()!.Equals("failed", StringComparison.OrdinalIgnoreCase);
        var failureReason = root.TryGetProperty("failureReason", out var failProp) ? failProp.GetString() : null;

        return new PaymentWebhookEvent(
            Provider: ProviderName,
            EventId: eventId,
            TransactionReference: txnRef,
            ProviderTransactionId: providerTxnId,
            BookingId: bookingId,
            Amount: amount,
            Currency: currency,
            PaymentType: paymentType,
            IsSuccess: isSuccess,
            FailureReason: failureReason,
            RawBody: body
        );
    }
}
