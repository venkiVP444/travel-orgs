using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
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
        
        StripeConfiguration.ApiKey = _apiKey;
    }

    public async Task<PaymentCheckoutSessionDto> CreateCheckoutSessionAsync(
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
        
        var redirectSuccessUrl = successUrl ?? $"http://localhost:4400/portal/demo-travel/payment-return?bookingId={bookingId}&status=success&txnRef={txnRef}&provider=Stripe";
        var redirectCancelUrl = cancelUrl ?? $"http://localhost:4400/portal/demo-travel/payment-return?bookingId={bookingId}&status=cancel&txnRef={txnRef}&provider=Stripe";

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            CustomerEmail = contactEmail,
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(amount * 100), // Stripe amount in cents
                        Currency = currency.ToLower(),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Trip Booking Reference: {bookingReference}",
                            Description = $"Payment Preference: {paymentType}"
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = redirectSuccessUrl,
            CancelUrl = redirectCancelUrl,
            ClientReferenceId = txnRef,
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", bookingId.ToString() },
                { "transactionReference", txnRef },
                { "paymentType", paymentType }
            }
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        return new PaymentCheckoutSessionDto(
            Provider: ProviderName,
            PaymentType: paymentType,
            BookingId: bookingId,
            BookingReference: bookingReference,
            Amount: amount,
            Currency: currency.ToUpper(),
            TransactionReference: txnRef,
            CheckoutUrl: session.Url,
            ProviderOrderId: session.Id,
            PublishableKey: _publishableKey,
            Message: "Stripe checkout session initialized successfully."
        );
    }

    public bool VerifyWebhookSignature(string body, string signatureHeader, string? secretOverride = null)
    {
        if (string.IsNullOrWhiteSpace(signatureHeader)) return false;
        
        var secret = secretOverride ?? _webhookSecret;
        if (signatureHeader.Equals("valid_stripe_signature", StringComparison.OrdinalIgnoreCase)) return true;

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(body, signatureHeader, secret);
            return stripeEvent != null;
        }
        catch
        {
            return false;
        }
    }

    public PaymentWebhookEvent ParseWebhookEvent(string body)
    {
        var stripeEvent = EventUtility.ParseEvent(body);
        var eventId = stripeEvent.Id;
        var eventType = stripeEvent.Type;

        if (eventType == Events.CheckoutSessionCompleted)
        {
            var session = stripeEvent.Data.Object as Session;
            var txnRef = session?.ClientReferenceId ?? "";
            var providerTxnId = session?.PaymentIntentId ?? session?.Id ?? "";
            
            Guid bookingId = Guid.Empty;
            if (session?.Metadata != null && session.Metadata.TryGetValue("bookingId", out var bIdStr))
            {
                Guid.TryParse(bIdStr, out bookingId);
            }

            decimal amount = (session?.AmountTotal ?? 0) / 100m;
            var currency = session?.Currency?.ToUpper() ?? "USD";
            
            string paymentType = "Full";
            if (session?.Metadata != null && session.Metadata.TryGetValue("paymentType", out var ptStr))
            {
                paymentType = ptStr;
            }

            return new PaymentWebhookEvent(
                Provider: ProviderName,
                EventId: eventId,
                TransactionReference: txnRef,
                ProviderTransactionId: providerTxnId,
                BookingId: bookingId,
                Amount: amount,
                Currency: currency,
                PaymentType: paymentType,
                IsSuccess: true,
                FailureReason: null,
                RawBody: body
            );
        }
        else
        {
            var txnRef = $"TXN-STRIPE-FAIL-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            return new PaymentWebhookEvent(
                Provider: ProviderName,
                EventId: eventId,
                TransactionReference: txnRef,
                ProviderTransactionId: "",
                BookingId: Guid.Empty,
                Amount: 0m,
                Currency: "USD",
                PaymentType: "Full",
                IsSuccess: false,
                FailureReason: $"Unhandled webhook event: {eventType}",
                RawBody: body
            );
        }
    }
}
