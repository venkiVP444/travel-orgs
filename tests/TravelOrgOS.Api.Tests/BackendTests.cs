using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Entities;
using TravelOrgOS.Domain.Enums;
using TravelOrgOS.Infrastructure.Data;
using TravelOrgOS.Infrastructure.Services;
using TravelOrgOS.Infrastructure.Services.PaymentGateways;
using Xunit;

namespace TravelOrgOS.Api.Tests;

public class DatabaseSafetyCheckerTests
{
    [Fact]
    public void LocalDbConnectionString_ShouldPassSafetyCheck()
    {
        const string validConnStr = @"Server=(localdb)\MSSQLLocalDB;Database=TravelOrgOS_Dev;Trusted_Connection=True;";
        DatabaseSafetyChecker.AssertConnectionIsLocalDbOnly(validConnStr);
    }

    [Fact]
    public void OfficeServerIp_ShouldThrowSafetyViolationException()
    {
        const string forbiddenConnStr = @"Data Source=10.50.6.6;Initial Catalog=dbEMMA_Restore;Integrated Security=True;";
        var ex = Assert.Throws<InvalidOperationException>(() => DatabaseSafetyChecker.AssertConnectionIsLocalDbOnly(forbiddenConnStr));
        Assert.Contains("CRITICAL SAFETY VIOLATION", ex.Message);
    }

    [Fact]
    public void OfficeDatabaseName_ShouldThrowSafetyViolationException()
    {
        const string forbiddenConnStr = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=dbEMMA_Restore;Integrated Security=True;";
        var ex = Assert.Throws<InvalidOperationException>(() => DatabaseSafetyChecker.AssertConnectionIsLocalDbOnly(forbiddenConnStr));
        Assert.Contains("CRITICAL SAFETY VIOLATION", ex.Message);
    }
}

public class BookingServiceTests
{
    private TravelOrgOSDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<TravelOrgOSDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TravelOrgOSDbContext(options);
    }

    private IPaymentGatewayFactory GetMockGatewayFactory()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "PaymentGateway:DefaultProvider", "Mock" },
            { "PaymentGateway:Stripe:WebhookSecret", "whsec_test_secret" },
            { "PaymentGateway:Razorpay:WebhookSecret", "rzp_test_secret" }
        }).Build();

        var gateways = new List<IPaymentGatewayService>
        {
            new MockPaymentGatewayService(),
            new StripePaymentGatewayService(config),
            new RazorpayPaymentGatewayService(config)
        };

        return new PaymentGatewayFactory(gateways, config);
    }

    [Fact]
    public async Task Overbooking_ShouldBePrevented()
    {
        using var context = GetInMemoryDbContext();
        var orgId = Guid.NewGuid();

        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            TripCode = "TEST-01",
            TripName = "Test Trip",
            Destination = "Test Dest",
            TotalCapacity = 5,
            AvailableSeats = 2,
            BasePrice = 100m,
            Status = TripStatus.RegistrationOpen
        };

        context.Trips.Add(trip);
        await context.SaveChangesAsync();

        var bookingService = new BookingService(context, GetMockGatewayFactory());

        var dto = new CreateBookingDto(
            TripId: trip.Id,
            NumberOfTravellers: 3,
            ContactEmail: "test@example.com",
            ContactPhone: "555-0199",
            SpecialRequests: null,
            Travellers: new List<CreateBookingTravellerDto>
            {
                new("John", "Doe", "john@example.com", "555-0199", "Single", "Regular"),
                new("Jane", "Doe", "jane@example.com", "555-0199", "Single", "Regular"),
                new("Jim", "Doe", "jim@example.com", "555-0199", "Single", "Regular")
            },
            PaymentType: "Full",
            AmountToPay: 300m
        );

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.CreateBookingAsync(orgId, dto));
        Assert.Contains("OVERBOOKING PREVENTED", ex.Message);
    }

    [Fact]
    public async Task ValidBooking_ShouldDeductAvailableSeats()
    {
        using var context = GetInMemoryDbContext();
        var orgId = Guid.NewGuid();

        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            TripCode = "TEST-02",
            TripName = "Valid Test Trip",
            Destination = "Test Dest",
            TotalCapacity = 10,
            AvailableSeats = 10,
            BasePrice = 500m,
            Status = TripStatus.RegistrationOpen
        };

        context.Trips.Add(trip);
        await context.SaveChangesAsync();

        var bookingService = new BookingService(context, GetMockGatewayFactory());

        var dto = new CreateBookingDto(
            TripId: trip.Id,
            NumberOfTravellers: 2,
            ContactEmail: "booker@example.com",
            ContactPhone: "555-0200",
            SpecialRequests: null,
            Travellers: new List<CreateBookingTravellerDto>
            {
                new("Alice", "Smith", "alice@example.com", "555-0200", "Double", "Veg"),
                new("Bob", "Smith", "bob@example.com", "555-0200", "Double", "Veg")
            },
            PaymentType: "Full",
            AmountToPay: 1000m
        );

        var result = await bookingService.CreateBookingAsync(orgId, dto);

        Assert.NotNull(result);
        Assert.StartsWith("BK-TES-", result.BookingReference);
        Assert.Equal(1000m, result.TotalAmount);
        Assert.Equal(PaymentStatus.Paid, result.PaymentStatus);

        var updatedTrip = await context.Trips.FindAsync(trip.Id);
        Assert.NotNull(updatedTrip);
        Assert.Equal(8, updatedTrip.AvailableSeats);
    }

    [Fact]
    public async Task FullPaymentMath_ShouldCalculateZeroBalance()
    {
        using var context = GetInMemoryDbContext();
        var orgId = Guid.NewGuid();
        var trip = new Trip { Id = Guid.NewGuid(), OrganizationId = orgId, TripCode = "MATH1", TripName = "Math Trip", BasePrice = 1000m, AvailableSeats = 5 };
        context.Trips.Add(trip);
        await context.SaveChangesAsync();

        var bookingService = new BookingService(context, GetMockGatewayFactory());
        var dto = new CreateBookingDto(trip.Id, 1, "test@math.com", "555-0001", null, new() { new("M", "User", "test@math.com", "555-0001", "Single", "Regular") }, "Full", 1000m);
        
        var res = await bookingService.CreateBookingAsync(orgId, dto);

        Assert.Equal(1000m, res.TotalAmount);
        Assert.Equal(1000m, res.PaidAmount);
        Assert.Equal(0m, res.BalanceAmount);
        Assert.Equal(PaymentStatus.Paid, res.PaymentStatus);
    }

    [Fact]
    public async Task DepositPaymentMath_ShouldCalculate30PercentPaidAnd70PercentBalance()
    {
        using var context = GetInMemoryDbContext();
        var orgId = Guid.NewGuid();
        var trip = new Trip { Id = Guid.NewGuid(), OrganizationId = orgId, TripCode = "MATH2", TripName = "Deposit Trip", BasePrice = 1000m, AvailableSeats = 5 };
        context.Trips.Add(trip);
        await context.SaveChangesAsync();

        var bookingService = new BookingService(context, GetMockGatewayFactory());
        var dto = new CreateBookingDto(trip.Id, 1, "dep@math.com", "555-0002", null, new() { new("D", "User", "dep@math.com", "555-0002", "Single", "Regular") }, "Deposit", 300m);
        
        var res = await bookingService.CreateBookingAsync(orgId, dto);

        Assert.Equal(1000m, res.TotalAmount);
        Assert.Equal(300m, res.PaidAmount);
        Assert.Equal(700m, res.BalanceAmount);
        Assert.Equal(PaymentStatus.PartiallyPaid, res.PaymentStatus);
    }

    [Fact]
    public async Task CrossTenantPaymentAccess_ShouldBeBlocked()
    {
        using var context = GetInMemoryDbContext();
        var org1 = Guid.NewGuid();
        var org2 = Guid.NewGuid();

        var trip = new Trip { Id = Guid.NewGuid(), OrganizationId = org1, TripCode = "TENANT", TripName = "Tenant Trip", BasePrice = 500m, AvailableSeats = 5 };
        context.Trips.Add(trip);
        await context.SaveChangesAsync();

        var bookingService = new BookingService(context, GetMockGatewayFactory());
        var dto = new CreateBookingDto(trip.Id, 1, "tenant@test.com", "555-0003", null, new() { new("T", "User", "tenant@test.com", "555-0003", "Single", "Regular") }, "PayLater", 0m);
        var booking = await bookingService.CreateBookingAsync(org1, dto);

        var initDto = new InitiatePaymentSessionDto(booking.Id, "Stripe", "Full", 500m, null, null);

        // Org2 trying to initiate payment session for Org1's booking -> Must throw exception
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.InitiatePaymentSessionAsync(org2, initDto));
        Assert.Contains("Booking not found or access denied for tenant", ex.Message);
    }

    [Fact]
    public async Task WebhookProcessing_ShouldBeIdempotent()
    {
        using var context = GetInMemoryDbContext();
        var orgId = Guid.NewGuid();

        var trip = new Trip { Id = Guid.NewGuid(), OrganizationId = orgId, TripCode = "IDEM", TripName = "Idempotency Trip", BasePrice = 400m, AvailableSeats = 5 };
        context.Trips.Add(trip);
        await context.SaveChangesAsync();

        var bookingService = new BookingService(context, GetMockGatewayFactory());
        var dto = new CreateBookingDto(trip.Id, 1, "idem@test.com", "555-0004", null, new() { new("I", "User", "idem@test.com", "555-0004", "Single", "Regular") }, "PayLater", 0m);
        var booking = await bookingService.CreateBookingAsync(orgId, dto);

        var webhookEvt = new PaymentWebhookEvent(
            Provider: "Stripe",
            EventId: "evt_stripe_duplicate_test_123",
            TransactionReference: "TXN-STRIPE-IDEM-01",
            ProviderTransactionId: "pi_stripe_test_123",
            BookingId: booking.Id,
            Amount: 400m,
            Currency: "USD",
            PaymentType: "Full",
            IsSuccess: true,
            FailureReason: null,
            RawBody: "{}"
        );

        // First Webhook execution
        var firstResult = await bookingService.ProcessGatewayPaymentWebhookAsync(webhookEvt);
        Assert.True(firstResult);

        var bookingAfterFirst = await bookingService.GetBookingByIdAsync(orgId, booking.Id);
        Assert.Equal(400m, bookingAfterFirst!.PaidAmount);
        Assert.Equal(0m, bookingAfterFirst.BalanceAmount);
        Assert.Single(bookingAfterFirst.Payments);

        // Second duplicate Webhook execution with SAME EventId
        var secondResult = await bookingService.ProcessGatewayPaymentWebhookAsync(webhookEvt);
        Assert.True(secondResult);

        var bookingAfterSecond = await bookingService.GetBookingByIdAsync(orgId, booking.Id);
        // Paid amount and payments count MUST NOT duplicate!
        Assert.Equal(400m, bookingAfterSecond!.PaidAmount);
        Assert.Equal(0m, bookingAfterSecond.BalanceAmount);
        Assert.Single(bookingAfterSecond.Payments);
    }

    [Fact]
    public async Task FailedPaymentWebhook_ShouldNotMarkBookingPaid()
    {
        using var context = GetInMemoryDbContext();
        var orgId = Guid.NewGuid();

        var trip = new Trip { Id = Guid.NewGuid(), OrganizationId = orgId, TripCode = "FAIL", TripName = "Failed Webhook Trip", BasePrice = 300m, AvailableSeats = 5 };
        context.Trips.Add(trip);
        await context.SaveChangesAsync();

        var bookingService = new BookingService(context, GetMockGatewayFactory());
        var dto = new CreateBookingDto(trip.Id, 1, "fail@test.com", "555-0005", null, new() { new("F", "User", "fail@test.com", "555-0005", "Single", "Regular") }, "PayLater", 0m);
        var booking = await bookingService.CreateBookingAsync(orgId, dto);

        var failedEvt = new PaymentWebhookEvent(
            Provider: "Razorpay",
            EventId: "evt_rzp_fail_999",
            TransactionReference: "TXN-RZP-FAIL-01",
            ProviderTransactionId: "pay_rzp_fail_999",
            BookingId: booking.Id,
            Amount: 300m,
            Currency: "INR",
            PaymentType: "Full",
            IsSuccess: false,
            FailureReason: "Card declined by issuing bank",
            RawBody: "{}"
        );

        var processed = await bookingService.ProcessGatewayPaymentWebhookAsync(failedEvt);
        Assert.True(processed);

        var updated = await bookingService.GetBookingByIdAsync(orgId, booking.Id);
        Assert.Equal(0m, updated!.PaidAmount);
        Assert.Equal(300m, updated.BalanceAmount);
        Assert.Equal(PaymentStatus.Pending, updated.PaymentStatus);
        Assert.Single(updated.Payments);
        Assert.Equal("Card declined by issuing bank", updated.Payments.First().FailureReason);
    }

    [Fact]
    public void SignatureVerification_ShouldRejectInvalidHeaders()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "PaymentGateway:Stripe:WebhookSecret", "whsec_test_secret_123" }
        }).Build();

        var stripeService = new StripePaymentGatewayService(config);

        bool isValid = stripeService.VerifyWebhookSignature("{\"id\":\"evt_1\"}", "t=123,v1=invalid_signature");
        Assert.False(isValid);

        bool isValidMock = stripeService.VerifyWebhookSignature("{\"id\":\"evt_1\"}", "valid_stripe_signature");
        Assert.True(isValidMock);
    }

    [Fact]
    public async Task GetBookingByReference_CrossTenant_ShouldReturnNull()
    {
        using var context = GetInMemoryDbContext();
        var org1 = Guid.NewGuid();
        var org2 = Guid.NewGuid();

        var trip = new Trip { Id = Guid.NewGuid(), OrganizationId = org1, TripCode = "REFTEST", TripName = "Ref Trip", BasePrice = 500m, AvailableSeats = 5 };
        context.Trips.Add(trip);
        await context.SaveChangesAsync();

        var bookingService = new BookingService(context, GetMockGatewayFactory());
        var dto = new CreateBookingDto(trip.Id, 1, "test@ref.com", "555-0003", null, new() { new("T", "User", "test@ref.com", "555-0003", "Single", "Regular") }, "PayLater", 0m);
        var booking = await bookingService.CreateBookingAsync(org1, dto);

        // Search with correct OrgId -> Should succeed
        var matchingBooking = await bookingService.GetBookingByReferenceAsync(booking.BookingReference, org1);
        Assert.NotNull(matchingBooking);

        // Search with cross-tenant OrgId -> Should return null
        var crossTenantBooking = await bookingService.GetBookingByReferenceAsync(booking.BookingReference, org2);
        Assert.Null(crossTenantBooking);
    }
}

public class TestController : TravelOrgOS.Api.Controllers.BaseApiController
{
    public Guid ExposedGetOrgId() => GetOrgId();
}

public class BaseApiControllerTests
{
    [Fact]
    public void GetOrgId_WithValidClaim_ShouldReturnGuid()
    {
        var orgId = Guid.NewGuid();
        var controller = new TestController();
        var user = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
        {
            new System.Security.Claims.Claim("OrganizationId", orgId.ToString())
        }, "TestAuth"));
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
        };

        Assert.Equal(orgId, controller.ExposedGetOrgId());
    }

    [Fact]
    public void GetOrgId_WithMissingClaim_ShouldThrowUnauthorized()
    {
        var controller = new TestController();
        var user = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[] { }, "TestAuth"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
        };

        Assert.Throws<UnauthorizedAccessException>(() => controller.ExposedGetOrgId());
    }
}
