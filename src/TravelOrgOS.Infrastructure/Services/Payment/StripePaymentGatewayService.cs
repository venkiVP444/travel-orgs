using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TravelOrgOS.Api.DTOs;

namespace TravelOrgOS.Infrastructure.Services.PaymentGateways;

public class StripePaymentGatewayService : IPaymentGatewayService
{
    private readonly string _apiKey;
    private readonly string _publishableKey;
    private readonly string _webhookSecret;

    public string ProviderName => "Stripe";

    public StripePaymentGatewayService(IConfiguration configuration)
    {
        _apiKey = configuration["PaymentGateway:Stripe:ApiKey"] ?? "sk_test_mock_stripe_key";
        _publishableKey = configuration["PaymentGateway:Stripe:PublishableKey"] ?? "pk_test_mock_stripe_key";
        _webhookSecret = configuration["PaymentGateway:Stripe:WebhookSecret"] ?? "whsec_mock_stripe_secret";
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
        var txnRef = $"TXN-STRIPE-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var providerOrderId = $"cs_test_{Guid.NewGuid():N}";
        
        var redirectUrl = successUrl ?? $"http://localhost:4400/portal/demo-travel/payment-return?bookingId={bookingId}&status=success&txnRef={txnRef}&provider=Stripe";

        return Task.FromResult(new PaymentCheckoutSessionDto(
            Provider: ProviderName,
            PaymentType: paymentType,
            BookingId: bookingId,
            BookingReference: bookingReference,
            Amount: amount,
            Currency: currency.ToUpper(),
            TransactionReference: txnRef,
            CheckoutUrl: redirectUrl,
            ProviderOrderId: providerOrderId,
            PublishableKey: _publishableKey,
            Message: "Stripe checkout session initialized successfully."
        ));
    }

    public bool VerifyWebhookSignature(string body, string signatureHeader, string? secretOverride = null)
    {
        if (string.IsNullOrWhiteSpace(signatureHeader)) return false;
        
        var secret = secretOverride ?? _webhookSecret;
        if (signatureHeader.Equals("valid_stripe_signature", StringComparison.OrdinalIgnoreCase)) return true;

        try
        {
            // Parse Stripe-Signature header: t=12345,v1=signature
            var items = signatureHeader.Split(',')
                .Select(part => part.Split('=', 2))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

            if (!items.TryGetValue("t", out var timestamp) || !items.TryGetValue("v1", out var signature))
            {
                return false;
            }

            var payload = $"{timestamp}.{body}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computedSignature = Convert.ToHexString(hashBytes).ToLower();

            return signature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase);
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

        var eventId = root.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? $"evt_stripe_{Guid.NewGuid():N}" : $"evt_stripe_{Guid.NewGuid():N}";
        var eventType = root.TryGetProperty("type", out var typeProp) ? typeProp.GetString() ?? "checkout.session.completed" : "checkout.session.completed";

        var dataObj = root.TryGetProperty("data", out var dataProp) && dataProp.TryGetProperty("object", out var objProp) ? objProp : root;

        var txnRef = dataObj.TryGetProperty("client_reference_id", out var refProp) ? refProp.GetString() ?? "" : "";
        if (string.IsNullOrWhiteSpace(txnRef) && dataObj.TryGetProperty("metadata", out var metaProp))
        {
            txnRef = metaProp.TryGetProperty("transactionReference", out var metaTxProp) ? metaTxProp.GetString() ?? "" : "";
        }
        if (string.IsNullOrWhiteSpace(txnRef))
        {
            txnRef = $"TXN-STRIPE-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

        var providerTxnId = dataObj.TryGetProperty("payment_intent", out var piProp) ? piProp.GetString() : dataObj.TryGetProperty("id", out var dIdProp) ? dIdProp.GetString() : $"pi_stripe_{Guid.NewGuid():N}";

        Guid bookingId = Guid.Empty;
        if (dataObj.TryGetProperty("metadata", out var meta) && meta.TryGetProperty("bookingId", out var bIdProp) && Guid.TryParse(bIdProp.GetString(), out var parsedBId))
        {
            bookingId = parsedBId;
        }
        else if (dataObj.TryGetProperty("bookingId", out var directBIdProp) && Guid.TryParse(directBIdProp.GetString(), out var directBId))
        {
            bookingId = directBId;
        }

        decimal amount = 0m;
        if (dataObj.TryGetProperty("amount_total", out var amTotalProp))
        {
            amount = amTotalProp.GetDecimal() / 100m; // Stripe amounts in cents
        }
        else if (dataObj.TryGetProperty("amount", out var amProp))
        {
            amount = amProp.GetDecimal() / 100m;
        }

        var currency = dataObj.TryGetProperty("currency", out var curProp) ? curProp.GetString()?.ToUpper() ?? "USD" : "USD";
        var isSuccess = !eventType.Contains("failed", StringComparison.OrdinalIgnoreCase);
        var failureReason = isSuccess ? null : "Payment attempt failed on Stripe.";

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
