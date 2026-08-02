using System.ComponentModel.DataAnnotations;
using TravelOrgOS.Domain.Enums;

namespace TravelOrgOS.Api.DTOs;

// --- AUTH DTOs ---
public record LoginRequestDto([Required, EmailAddress] string Email, [Required] string Password);

public record LoginResponseDto(
    string Token,
    Guid UserId,
    string FullName,
    string Email,
    UserRole Role,
    Guid? OrganizationId,
    string? OrganizationName,
    string? OrganizationSlug,
    string? PrimaryColor,
    string? SecondaryColor,
    string? LogoUrl
);

// --- ORGANIZATION DTOs ---
public record OrganizationDto(
    Guid Id,
    string Name,
    string Slug,
    string? LegalName,
    string? LogoUrl,
    string PrimaryColor,
    string SecondaryColor,
    string? WelcomeMessage,
    string Email,
    string Phone,
    string? Website,
    string? Address,
    string? City,
    string? Country,
    string? BusinessHours,
    string? Description,
    string? FacebookUrl,
    string? InstagramUrl,
    string? LinkedInUrl,
    string? WhatsAppNumber,
    bool Status
);

public record UpdateOrganizationDto(
    string Name,
    string? LegalName,
    string? LogoUrl,
    string PrimaryColor,
    string SecondaryColor,
    string? WelcomeMessage,
    string Email,
    string Phone,
    string? Website,
    string? Address,
    string? City,
    string? Country,
    string? Description,
    string? WhatsAppNumber
);

// --- TRAVELLER DTOs ---
public record TravellerDto(
    Guid Id,
    Guid OrganizationId,
    string FirstName,
    string LastName,
    string Email,
    string MobileNumber,
    DateTime? DateOfBirth,
    string? Gender,
    string? Nationality,
    string? PassportNumber,
    DateTime? PassportExpiry,
    string? EmergencyContactName,
    string? EmergencyContactNumber,
    string? Address,
    string? City,
    string? Country,
    string? Notes,
    bool Status,
    DateTime CreatedAt
);

public record CreateTravellerDto(
    [Required] string FirstName,
    [Required] string LastName,
    [Required, EmailAddress] string Email,
    [Required] string MobileNumber,
    DateTime? DateOfBirth,
    string? Gender,
    string? Nationality,
    string? PassportNumber,
    DateTime? PassportExpiry,
    string? EmergencyContactName,
    string? EmergencyContactNumber,
    string? Address,
    string? City,
    string? Country,
    string? Notes
);

public record TravellerCsvRowDto(
    string FirstName,
    string LastName,
    string Email,
    string MobileNumber,
    string? DateOfBirth,
    string? Gender,
    string? Nationality,
    string? PassportNumber,
    string? EmergencyContactName,
    string? EmergencyContactNumber,
    string? City,
    string? Country
);

public record CsvImportSummaryDto(
    int TotalRecords,
    int Successful,
    int Failed,
    int Duplicate,
    List<string> ValidationErrors
);

// --- TRIP DTOs ---
public record TripDto(
    Guid Id,
    Guid OrganizationId,
    string TripCode,
    string TripName,
    string ShortDescription,
    string Description,
    string Destination,
    string StartLocation,
    string EndLocation,
    string? ViaRoute,
    DateTime StartDate,
    DateTime EndDate,
    int DurationDays,
    int DurationNights,
    TripType TripType,
    TripStatus Status,
    TripVisibility Visibility,
    string CoverImageUrl,
    decimal BasePrice,
    string Currency,
    int TotalCapacity,
    int AvailableSeats,
    int BookedSeats,
    int MinimumTravellers,
    int MaximumTravellers,
    string? HostGuide,
    string? DriverName,
    decimal EstimatedCost,
    decimal GrossRevenue,
    decimal NetProfit,
    string? ContactPerson,
    string? ContactNumber,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    List<ItineraryDayDto> ItineraryDays,
    List<TripHotelDto> Hotels,
    List<TripVehicleDto> Vehicles,
    List<TripVendorDto> Vendors,
    List<TripMealDto> Meals
);

public record CreateTripDto(
    [Required] string TripCode,
    [Required] string TripName,
    string ShortDescription,
    string Description,
    [Required] string Destination,
    string StartLocation,
    string EndLocation,
    string? ViaRoute,
    DateTime StartDate,
    DateTime EndDate,
    int DurationDays,
    int DurationNights,
    TripType TripType,
    TripVisibility Visibility,
    string CoverImageUrl,
    decimal BasePrice,
    string Currency,
    int TotalCapacity,
    int AvailableSeats,
    int MinimumTravellers,
    int MaximumTravellers,
    string? HostGuide,
    string? DriverName,
    decimal EstimatedCost,
    string? ContactPerson,
    string? ContactNumber
);

public record ItineraryDayDto(
    Guid? Id,
    int DayNumber,
    DateTime? Date,
    string Title,
    string Description,
    string? Location,
    string? Activities,
    string? StartTime,
    string? EndTime,
    string? Notes
);

public record TripHotelDto(
    Guid? Id,
    Guid HotelId,
    string HotelName,
    string Location,
    string RoomType,
    DateTime CheckIn,
    DateTime CheckOut,
    int RoomCount,
    string? Notes
);

public record TripVehicleDto(
    Guid? Id,
    Guid VehicleId,
    string VehicleName,
    string VehicleType,
    int Capacity,
    string? DriverName,
    string? DriverPhone,
    string? Notes
);

public record TripVendorDto(
    Guid? Id,
    Guid VendorId,
    string VendorName,
    VendorType VendorType,
    decimal ContractAmount,
    string? ServiceDescription
);

public record TripMealDto(
    Guid? Id,
    MealType MealType,
    MealOption MealOption,
    string? Description,
    string? DietaryOptions
);

// --- BOOKING DTOs ---
public record BookingDto(
    Guid Id,
    Guid OrganizationId,
    Guid TripId,
    string TripName,
    string TripCode,
    string BookingReference,
    DateTime BookingDate,
    int NumberOfTravellers,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal BalanceAmount,
    PaymentStatus PaymentStatus,
    BookingStatus BookingStatus,
    string ContactEmail,
    string ContactPhone,
    string? SpecialRequests,
    List<BookingTravellerDto> Travellers,
    List<PaymentDto> Payments
);

public record BookingTravellerDto(
    Guid TravellerId,
    string FullName,
    string Email,
    string MobileNumber,
    string RoomPreference,
    string DietaryPreference
);

public record CreateBookingDto(
    [Required] Guid TripId,
    [Required] int NumberOfTravellers,
    [Required, EmailAddress] string ContactEmail,
    [Required] string ContactPhone,
    string? SpecialRequests,
    [Required] List<CreateBookingTravellerDto> Travellers,
    [Required] string PaymentType,
    decimal AmountToPay
);

public record CreateBookingTravellerDto(
    string FirstName,
    string LastName,
    string Email,
    string MobileNumber,
    string? RoomPreference,
    string? DietaryPreference
);

public record PaymentDto(
    Guid Id,
    Guid BookingId,
    decimal Amount,
    string PaymentMethod,
    string TransactionReference,
    string Provider,
    string? ProviderTransactionId,
    string? ProviderEventId,
    string Currency,
    string PaymentType,
    PaymentStatus Status,
    DateTime PaymentDate,
    DateTime? CompletedAt,
    string? FailureReason,
    string? Notes
);

public record InitiatePaymentSessionDto(
    [Required] Guid BookingId,
    [Required] string Provider, // Stripe, Razorpay, Mock
    [Required] string PaymentType, // Full, Deposit, Balance
    decimal? AmountToPay,
    string? SuccessUrl,
    string? CancelUrl
);

public record PaymentCheckoutSessionDto(
    string Provider,
    string PaymentType,
    Guid BookingId,
    string BookingReference,
    decimal Amount,
    string Currency,
    string TransactionReference,
    string? CheckoutUrl,
    string? ProviderOrderId,
    string? PublishableKey,
    string Message
);

public record PaymentWebhookEvent(
    string Provider,
    string EventId,
    string TransactionReference,
    string? ProviderTransactionId,
    Guid BookingId,
    decimal Amount,
    string Currency,
    string PaymentType,
    bool IsSuccess,
    string? FailureReason,
    string RawBody
);

public record RecordPaymentDto(
    [Required] decimal Amount,
    [Required] string PaymentMethod,
    string? TransactionReference,
    string? Notes
);

// --- DASHBOARD DTOs ---
public record DashboardSummaryDto(
    int TotalTrips,
    int ActiveTrips,
    int TotalTravellers,
    int TotalBookings,
    int ConfirmedBookings,
    decimal TotalRevenue,
    decimal PendingPayments,
    decimal OutstandingBalance,
    List<UpcomingTripDto> UpcomingTrips,
    List<RecentBookingDto> RecentBookings,
    List<BookingTrendDto> BookingTrends
);

public record UpcomingTripDto(
    Guid Id,
    string TripCode,
    string TripName,
    string Destination,
    DateTime StartDate,
    int TotalCapacity,
    int BookedSeats,
    int AvailableSeats,
    TripStatus Status
);

public record RecentBookingDto(
    Guid Id,
    string BookingReference,
    string TripName,
    string CustomerName,
    int Passengers,
    decimal TotalAmount,
    PaymentStatus PaymentStatus,
    BookingStatus BookingStatus,
    DateTime BookingDate
);

public record BookingTrendDto(
    string MonthLabel,
    int BookingCount,
    decimal Revenue
);

// --- REPORT DTOs ---
public record ReportExportFilterDto(
    DateTime? StartDate,
    DateTime? EndDate,
    Guid? TripId,
    string? Status
);

public record RazorpayVerificationDto(
    [Required] Guid BookingId,
    [Required] string OrderId,
    [Required] string PaymentId,
    [Required] string Signature,
    [Required] string TransactionReference,
    [Required] string PaymentType,
    [Required] decimal Amount
);
