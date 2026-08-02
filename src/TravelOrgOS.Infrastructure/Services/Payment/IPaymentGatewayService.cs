using TravelOrgOS.Api.DTOs;

namespace TravelOrgOS.Infrastructure.Services.PaymentGateways;

public interface IPaymentGatewayService
{
    string ProviderName { get; }
    
    Task<PaymentCheckoutSessionDto> CreateCheckoutSessionAsync(
        Guid orgId,
        Guid bookingId,
        string bookingReference,
        decimal amount,
        string currency,
        string paymentType,
        string contactEmail,
        string? successUrl = null,
        string? cancelUrl = null);

    bool VerifyWebhookSignature(string body, string signatureHeader, string? secretOverride = null);

    PaymentWebhookEvent ParseWebhookEvent(string body);
}
