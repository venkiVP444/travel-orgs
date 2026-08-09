using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Entities;
using TravelOrgOS.Domain.Enums;
using TravelOrgOS.Infrastructure.Data;
using TravelOrgOS.Infrastructure.Services.PaymentGateways;

namespace TravelOrgOS.Infrastructure.Services;

public interface IBookingService
{
    Task<List<BookingDto>> GetBookingsAsync(Guid orgId, string? search = null);
    Task<BookingDto?> GetBookingByIdAsync(Guid orgId, Guid id);
    Task<BookingDto?> GetBookingByReferenceAsync(string reference, Guid? orgId = null);
    Task<BookingDto> CreateBookingAsync(Guid orgId, CreateBookingDto dto, Guid? userId = null);
    Task<bool> ConfirmBookingAsync(Guid orgId, Guid bookingId);
    Task<bool> CancelBookingAsync(Guid orgId, Guid bookingId);
    Task<BookingDto?> RecordPaymentAsync(Guid orgId, Guid bookingId, RecordPaymentDto dto);
    Task<PaymentCheckoutSessionDto> InitiatePaymentSessionAsync(Guid orgId, InitiatePaymentSessionDto dto);
    Task<bool> ProcessGatewayPaymentWebhookAsync(PaymentWebhookEvent webhookEvent);
    Task<BookingDto?> GetBookingByIdForPortalAsync(Guid id);
}

public class BookingService : IBookingService
{
    private readonly TravelOrgOSDbContext _context;
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly ITaxService _taxService;

    public BookingService(TravelOrgOSDbContext context, IPaymentGatewayFactory gatewayFactory, ITaxService taxService)
    {
        _context = context;
        _gatewayFactory = gatewayFactory;
        _taxService = taxService;
    }

    public async Task<List<BookingDto>> GetBookingsAsync(Guid orgId, string? search = null)
    {
        var query = _context.Bookings
            .Include(b => b.Trip)
            .Include(b => b.BookingTravellers).ThenInclude(bt => bt.Traveller)
            .Include(b => b.Payments)
            .Where(b => b.OrganizationId == orgId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(b =>
                b.BookingReference.ToLower().Contains(s) ||
                b.Trip!.TripName.ToLower().Contains(s) ||
                b.ContactEmail.ToLower().Contains(s));
        }

        var bookings = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return bookings.Select(b => MapToDto(b)).ToList();
    }

    public async Task<BookingDto?> GetBookingByIdAsync(Guid orgId, Guid id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Trip)
            .Include(b => b.BookingTravellers).ThenInclude(bt => bt.Traveller)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.OrganizationId == orgId && b.Id == id);

        return booking == null ? null : MapToDto(booking);
    }

    public async Task<BookingDto?> GetBookingByReferenceAsync(string reference, Guid? orgId = null)
    {
        var query = _context.Bookings
            .Include(b => b.Trip)
            .Include(b => b.BookingTravellers).ThenInclude(bt => bt.Traveller)
            .Include(b => b.Payments)
            .AsQueryable();

        if (orgId.HasValue)
        {
            query = query.Where(b => b.OrganizationId == orgId.Value);
        }

        var booking = await query.FirstOrDefaultAsync(b => b.BookingReference.ToLower() == reference.ToLower());

        return booking == null ? null : MapToDto(booking);
    }

    public async Task<BookingDto> CreateBookingAsync(Guid orgId, CreateBookingDto dto, Guid? userId = null)
    {
        using var transaction = _context.Database.IsSqlServer() 
            ? await _context.Database.BeginTransactionAsync() 
            : null;

        try
        {
            Trip? trip = null;
            if (_context.Database.IsSqlServer())
            {
                trip = await _context.Trips
                    .FromSqlRaw("SELECT * FROM Trips WITH (UPDLOCK, ROWLOCK) WHERE Id = {0} AND OrganizationId = {1}", dto.TripId, orgId)
                    .FirstOrDefaultAsync();
            }
            else
            {
                trip = await _context.Trips.FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == dto.TripId);
            }

            if (trip == null)
            {
                throw new InvalidOperationException("Trip not found.");
            }

            if (trip.AvailableSeats < dto.NumberOfTravellers)
            {
                throw new InvalidOperationException($"OVERBOOKING PREVENTED: Only {trip.AvailableSeats} seat(s) available for this trip!");
            }

            // Generate Booking Reference
            var codePrefix = trip.TripCode.Length >= 3 ? trip.TripCode.Substring(0, 3) : "TRP";
            var randomNum = new Random().Next(1000, 9999);
            var bookingRef = $"BK-{codePrefix}-{randomNum}";

            // Calculate GST Tax
            decimal taxableAmount = trip.BasePrice * dto.NumberOfTravellers;
            var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == orgId);
            
            TaxBreakdownDto tax;
            if (org != null && !string.IsNullOrEmpty(org.GSTIN))
            {
                var operatorState = org.State ?? org.City ?? "Karnataka";
                var customerState = dto.BillingState ?? operatorState;
                tax = _taxService.CalculateGst(taxableAmount, operatorState, customerState);
            }
            else
            {
                tax = new TaxBreakdownDto(taxableAmount, 0m, 0m, 0m, 0m, 0m, taxableAmount);
            }
            
            decimal totalAmount = tax.GrandTotal;
            decimal paidAmount = 0m;

            PaymentStatus initialPaymentStatus = PaymentStatus.Pending;
            if (dto.PaymentType.Equals("Full", StringComparison.OrdinalIgnoreCase))
            {
                paidAmount = totalAmount;
                initialPaymentStatus = PaymentStatus.Paid;
            }
            else if (dto.PaymentType.Equals("Deposit", StringComparison.OrdinalIgnoreCase))
            {
                paidAmount = dto.AmountToPay > 0 ? dto.AmountToPay : totalAmount * 0.3m;
                initialPaymentStatus = PaymentStatus.PartiallyPaid;
            }

            decimal balanceAmount = Math.Max(0, totalAmount - paidAmount);

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                TripId = dto.TripId,
                BookedByUserId = userId,
                BookingReference = bookingRef,
                BookingDate = DateTime.UtcNow,
                NumberOfTravellers = dto.NumberOfTravellers,
                TotalAmount = totalAmount,
                PaidAmount = paidAmount,
                BalanceAmount = balanceAmount,
                TaxableAmount = tax.TaxableAmount,
                GstPercentage = tax.GstPercentage,
                CGST = tax.CGST,
                SGST = tax.SGST,
                IGST = tax.IGST,
                TotalTax = tax.TotalTax,
                PaymentStatus = initialPaymentStatus,
                BookingStatus = BookingStatus.Confirmed,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                SpecialRequests = dto.SpecialRequests,
                CreatedAt = DateTime.UtcNow
            };

            // Attach Travellers
            foreach (var tDto in dto.Travellers)
            {
                var existing = await _context.Travellers
                    .FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Email.ToLower() == tDto.Email.ToLower());

                if (existing == null)
                {
                    existing = new Traveller
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = orgId,
                        FirstName = string.IsNullOrWhiteSpace(tDto.FirstName) ? "Passenger" : tDto.FirstName.Trim(),
                        LastName = string.IsNullOrWhiteSpace(tDto.LastName) ? "Traveller" : tDto.LastName.Trim(),
                        Email = tDto.Email.Trim(),
                        MobileNumber = string.IsNullOrWhiteSpace(tDto.MobileNumber) ? dto.ContactPhone : tDto.MobileNumber.Trim(),
                        Status = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Travellers.Add(existing);
                }

                booking.BookingTravellers.Add(new BookingTraveller
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    TravellerId = existing.Id,
                    RoomPreference = tDto.RoomPreference ?? "Single",
                    DietaryPreference = tDto.DietaryPreference ?? "Regular"
                });
            }

            // Record Initial Payment if paid > 0
            if (paidAmount > 0)
            {
                booking.Payments.Add(new Payment
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = orgId,
                    BookingId = booking.Id,
                    Amount = paidAmount,
                    PaymentMethod = dto.PaymentType.Equals("Full", StringComparison.OrdinalIgnoreCase) ? "Mock Card (Full)" : "Mock Card (Deposit)",
                    TransactionReference = $"TXN-MOCK-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                    Provider = "Mock",
                    Currency = trip.Currency,
                    PaymentType = dto.PaymentType,
                    Status = PaymentStatus.Paid,
                    PaymentDate = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow,
                    Notes = "Mock Payment Processed Successfully"
                });
            }

            // Deduct available seats
            trip.AvailableSeats = Math.Max(0, trip.AvailableSeats - dto.NumberOfTravellers);
            if (trip.AvailableSeats == 0)
            {
                trip.Status = TripStatus.FullyBooked;
            }
            else if (trip.AvailableSeats <= 3)
            {
                trip.Status = TripStatus.AlmostFull;
            }

            // Create In-App Notification
            _context.Notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                Title = "New Booking Confirmed",
                Message = $"New booking {bookingRef} created for {trip.TripName} ({dto.NumberOfTravellers} traveller(s)). Amount Paid: INR {paidAmount:N2}.",
                Type = NotificationType.BookingCreated,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            // Audit Log
            _context.AuditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                User = userId.HasValue ? "AuthenticatedUser" : dto.ContactEmail,
                Action = "BookingCreated",
                Entity = "Booking",
                EntityId = booking.Id.ToString(),
                Details = $"Booking {bookingRef} created for {trip.TripName}. Available seats updated to {trip.AvailableSeats}.",
                Timestamp = DateTime.UtcNow
            });

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            return await GetBookingByIdAsync(orgId, booking.Id) ?? MapToDto(booking);
        }
        catch
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }
            throw;
        }
    }

    public async Task<bool> ConfirmBookingAsync(Guid orgId, Guid bookingId)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.OrganizationId == orgId && b.Id == bookingId);
        if (booking == null) return false;

        booking.BookingStatus = BookingStatus.Confirmed;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelBookingAsync(Guid orgId, Guid bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Trip)
            .FirstOrDefaultAsync(b => b.OrganizationId == orgId && b.Id == bookingId);

        if (booking == null) return false;

        if (booking.BookingStatus != BookingStatus.Cancelled)
        {
            booking.BookingStatus = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;

            if (booking.Trip != null)
            {
                booking.Trip.AvailableSeats += booking.NumberOfTravellers;
                if (booking.Trip.Status == TripStatus.FullyBooked || booking.Trip.Status == TripStatus.AlmostFull)
                {
                    booking.Trip.Status = TripStatus.RegistrationOpen;
                }
            }

            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<BookingDto?> RecordPaymentAsync(Guid orgId, Guid bookingId, RecordPaymentDto dto)
    {
        var booking = await _context.Bookings
            .Include(b => b.Payments)
            .Include(b => b.Trip)
            .FirstOrDefaultAsync(b => b.OrganizationId == orgId && b.Id == bookingId);

        if (booking == null) return null;

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            BookingId = booking.Id,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            TransactionReference = !string.IsNullOrWhiteSpace(dto.TransactionReference) ? dto.TransactionReference : $"TXN-MANUAL-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
            Provider = "Manual",
            Currency = booking.Trip?.Currency ?? "USD",
            PaymentType = booking.PaidAmount == 0 ? "Deposit" : "Balance",
            Status = PaymentStatus.Paid,
            PaymentDate = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow,
            Notes = dto.Notes
        };

        _context.Payments.Add(payment);

        booking.PaidAmount += dto.Amount;
        booking.BalanceAmount = Math.Max(0, booking.TotalAmount - booking.PaidAmount);

        if (booking.BalanceAmount == 0)
        {
            booking.PaymentStatus = PaymentStatus.Paid;
        }
        else if (booking.PaidAmount > 0)
        {
            booking.PaymentStatus = PaymentStatus.PartiallyPaid;
        }

        booking.UpdatedAt = DateTime.UtcNow;

        _context.Notifications.Add(new Notification
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            Title = "Payment Recorded",
            Message = $"Payment of ${dto.Amount:N2} recorded for Booking {booking.BookingReference}. Remaining Balance: ${booking.BalanceAmount:N2}.",
            Type = NotificationType.PaymentReceived,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return await GetBookingByIdAsync(orgId, bookingId);
    }

    public async Task<PaymentCheckoutSessionDto> InitiatePaymentSessionAsync(Guid orgId, InitiatePaymentSessionDto dto)
    {
        var booking = await _context.Bookings
            .Include(b => b.Trip)
            .FirstOrDefaultAsync(b => b.OrganizationId == orgId && b.Id == dto.BookingId);

        if (booking == null)
        {
            throw new InvalidOperationException("Booking not found or access denied for tenant.");
        }

        decimal amountToPay = dto.PaymentType.Equals("Full", StringComparison.OrdinalIgnoreCase)
            ? booking.TotalAmount
            : dto.PaymentType.Equals("Deposit", StringComparison.OrdinalIgnoreCase)
                ? (dto.AmountToPay.HasValue && dto.AmountToPay.Value > 0 ? dto.AmountToPay.Value : booking.TotalAmount * 0.3m)
                : booking.BalanceAmount;

        if (amountToPay <= 0)
        {
            throw new InvalidOperationException("Booking has no remaining balance to pay.");
        }

        var gateway = _gatewayFactory.GetGateway(dto.Provider);

        return await gateway.CreateCheckoutSessionAsync(
            orgId: orgId,
            bookingId: booking.Id,
            bookingReference: booking.BookingReference,
            amount: amountToPay,
            currency: booking.Trip?.Currency ?? "USD",
            paymentType: dto.PaymentType,
            contactEmail: booking.ContactEmail,
            successUrl: dto.SuccessUrl,
            cancelUrl: dto.CancelUrl
        );
    }

    public async Task<bool> ProcessGatewayPaymentWebhookAsync(PaymentWebhookEvent evt)
    {
        // 1. IDEMPOTENCY CHECK: Check if EventId already processed
        if (!string.IsNullOrWhiteSpace(evt.EventId))
        {
            var existingByEvent = await _context.Payments.AnyAsync(p => p.ProviderEventId == evt.EventId);
            if (existingByEvent)
            {
                return true; // Already processed idempotently
            }
        }

        // 2. Find Booking by BookingId or TransactionReference
        Booking? booking = null;
        if (evt.BookingId != Guid.Empty)
        {
            booking = await _context.Bookings
                .Include(b => b.Payments)
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(b => b.Id == evt.BookingId);
        }

        if (booking == null && !string.IsNullOrWhiteSpace(evt.TransactionReference))
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.TransactionReference == evt.TransactionReference);

            booking = payment?.Booking;
        }

        if (booking == null)
        {
            return false; // Booking not found for webhook
        }

        // 3. Process Webhook Event (IDEMPOTENT SAVE)
        if (evt.IsSuccess)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrganizationId = booking.OrganizationId,
                BookingId = booking.Id,
                Amount = evt.Amount > 0 ? evt.Amount : booking.TotalAmount,
                PaymentMethod = $"{evt.Provider} Online",
                TransactionReference = evt.TransactionReference,
                Provider = evt.Provider,
                ProviderTransactionId = evt.ProviderTransactionId,
                ProviderEventId = evt.EventId,
                Currency = evt.Currency,
                PaymentType = evt.PaymentType,
                Status = PaymentStatus.Paid,
                PaymentDate = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow,
                Notes = $"Verified {evt.Provider} webhook payment processing."
            };

            _context.Payments.Add(payment);

            booking.PaidAmount += payment.Amount;
            booking.BalanceAmount = Math.Max(0, booking.TotalAmount - booking.PaidAmount);

            if (booking.BalanceAmount == 0)
            {
                booking.PaymentStatus = PaymentStatus.Paid;
            }
            else if (booking.PaidAmount > 0)
            {
                booking.PaymentStatus = PaymentStatus.PartiallyPaid;
            }

            booking.UpdatedAt = DateTime.UtcNow;

            _context.Notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                OrganizationId = booking.OrganizationId,
                Title = "Payment Confirmed via Gateway",
                Message = $"Payment of {evt.Currency} ${payment.Amount:N2} confirmed via {evt.Provider} for Booking {booking.BookingReference}.",
                Type = NotificationType.PaymentReceived,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            var failedPayment = new Payment
            {
                Id = Guid.NewGuid(),
                OrganizationId = booking.OrganizationId,
                BookingId = booking.Id,
                Amount = evt.Amount,
                PaymentMethod = $"{evt.Provider} Online",
                TransactionReference = evt.TransactionReference,
                Provider = evt.Provider,
                ProviderTransactionId = evt.ProviderTransactionId,
                ProviderEventId = evt.EventId,
                Currency = evt.Currency,
                PaymentType = evt.PaymentType,
                Status = PaymentStatus.Pending,
                PaymentDate = DateTime.UtcNow,
                FailureReason = evt.FailureReason ?? "Payment gateway failed.",
                Notes = "Failed payment attempt recorded via webhook."
            };

            _context.Payments.Add(failedPayment);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BookingDto?> GetBookingByIdForPortalAsync(Guid id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Trip)
            .Include(b => b.BookingTravellers).ThenInclude(bt => bt.Traveller)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.Id == id);

        return booking == null ? null : MapToDto(booking);
    }

    private static BookingDto MapToDto(Booking b) => new(
        b.Id,
        b.OrganizationId,
        b.TripId,
        b.Trip?.TripName ?? "",
        b.Trip?.TripCode ?? "",
        b.BookingReference,
        b.BookingDate,
        b.NumberOfTravellers,
        b.TotalAmount,
        b.PaidAmount,
        b.BalanceAmount,
        b.TaxableAmount,
        b.GstPercentage,
        b.CGST,
        b.SGST,
        b.IGST,
        b.TotalTax,
        b.PaymentStatus,
        b.BookingStatus,
        b.ContactEmail,
        b.ContactPhone,
        b.SpecialRequests,
        b.BookingTravellers.Select(bt => new BookingTravellerDto(
            bt.TravellerId,
            $"{bt.Traveller?.FirstName} {bt.Traveller?.LastName}".Trim(),
            bt.Traveller?.Email ?? "",
            bt.Traveller?.MobileNumber ?? "",
            bt.RoomPreference,
            bt.DietaryPreference
        )).ToList(),
        b.Payments.Select(p => new PaymentDto(
            p.Id,
            p.BookingId,
            p.Amount,
            p.PaymentMethod,
            p.TransactionReference,
            p.Provider,
            p.ProviderTransactionId,
            p.ProviderEventId,
            p.Currency,
            p.PaymentType,
            p.Status,
            p.PaymentDate,
            p.CompletedAt,
            p.FailureReason,
            p.Notes
        )).ToList()
    );
}
