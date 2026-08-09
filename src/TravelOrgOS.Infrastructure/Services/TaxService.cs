using System;
using TravelOrgOS.Api.DTOs;

namespace TravelOrgOS.Infrastructure.Services;

public interface ITaxService
{
    TaxBreakdownDto CalculateGst(decimal baseAmount, string operatorState, string customerState);
}

public class TaxService : ITaxService
{
    public TaxBreakdownDto CalculateGst(decimal baseAmount, string operatorState, string customerState)
    {
        // Standard B2B travel agency service rate (SAC 9985) is 18%
        decimal gstPercentage = 18.0m;
        decimal totalTax = Math.Round(baseAmount * (gstPercentage / 100m), 2);
        
        decimal cgst = 0m;
        decimal sgst = 0m;
        decimal igst = 0m;

        if (string.IsNullOrEmpty(operatorState) || string.IsNullOrEmpty(customerState) ||
            string.Equals(operatorState.Trim(), customerState.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            // Intra-state: Split CGST (9%) & SGST (9%)
            cgst = Math.Round(totalTax / 2m, 2);
            sgst = totalTax - cgst;
        }
        else
        {
            // Inter-state: Full IGST (18%)
            igst = totalTax;
        }

        decimal grandTotal = baseAmount + totalTax;

        return new TaxBreakdownDto(
            TaxableAmount: baseAmount,
            GstPercentage: gstPercentage,
            CGST: cgst,
            SGST: sgst,
            IGST: igst,
            TotalTax: totalTax,
            GrandTotal: grandTotal
        );
    }
}
