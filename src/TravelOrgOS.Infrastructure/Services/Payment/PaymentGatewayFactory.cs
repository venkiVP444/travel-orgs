using Microsoft.Extensions.Configuration;

namespace TravelOrgOS.Infrastructure.Services.PaymentGateways;

public interface IPaymentGatewayFactory
{
    IPaymentGatewayService GetGateway(string? providerName = null);
}

public class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly IEnumerable<IPaymentGatewayService> _gateways;
    private readonly string _defaultProvider;

    public PaymentGatewayFactory(IEnumerable<IPaymentGatewayService> gateways, IConfiguration configuration)
    {
        _gateways = gateways;
        _defaultProvider = configuration["PaymentGateway:DefaultProvider"] ?? "Mock";
    }

    public IPaymentGatewayService GetGateway(string? providerName = null)
    {
        var targetProvider = string.IsNullOrWhiteSpace(providerName) ? _defaultProvider : providerName.Trim();

        var gateway = _gateways.FirstOrDefault(g => g.ProviderName.Equals(targetProvider, StringComparison.OrdinalIgnoreCase));
        
        if (gateway == null)
        {
            // Fallback to Mock gateway if requested provider is unknown
            gateway = _gateways.FirstOrDefault(g => g.ProviderName.Equals("Mock", StringComparison.OrdinalIgnoreCase));
        }

        return gateway ?? throw new InvalidOperationException($"No payment gateway registered for provider '{targetProvider}'.");
    }
}
