using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelOrgOS.Domain.Enums;

namespace TravelOrgOS.Domain.Entities;

public class Organization
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MaxLength(200)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Slug { get; set; } = string.Empty;
    [MaxLength(200)] public string? LegalName { get; set; }
    [MaxLength(500)] public string? LogoUrl { get; set; }
    [MaxLength(30)] public string PrimaryColor { get; set; } = "#1E88E5";
    [MaxLength(30)] public string SecondaryColor { get; set; } = "#0D47A1";
    [MaxLength(500)] public string? WelcomeMessage { get; set; } = "Welcome to our Travel Experiences!";
    [MaxLength(200)] public string Email { get; set; } = string.Empty;
    [MaxLength(50)] public string Phone { get; set; } = string.Empty;
    [MaxLength(200)] public string? Website { get; set; }
    [MaxLength(500)] public string? Address { get; set; }
    [MaxLength(100)] public string? City { get; set; }
    [MaxLength(100)] public string? Country { get; set; }
    [MaxLength(200)] public string? BusinessHours { get; set; }
    public string? Description { get; set; }
    [MaxLength(200)] public string? FacebookUrl { get; set; }
    [MaxLength(200)] public string? InstagramUrl { get; set; }
    [MaxLength(200)] public string? LinkedInUrl { get; set; }
    [MaxLength(50)] public string? WhatsAppNumber { get; set; }
    [MaxLength(15)] public string? GSTIN { get; set; }
    [MaxLength(100)] public string? State { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<OrganizationUser> Users { get; set; } = new List<OrganizationUser>();
    public ICollection<Traveller> Travellers { get; set; } = new List<Traveller>();
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();
    public ICollection<Guide> Guides { get; set; } = new List<Guide>();
    public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
}

public class OrganizationUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    [Required, MaxLength(150)] public string FullName { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Email { get; set; } = string.Empty;
    [Required] public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}

public class Traveller
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(50)] public string MobileNumber { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    [MaxLength(20)] public string? Gender { get; set; }
    [MaxLength(100)] public string? Nationality { get; set; }
    [MaxLength(50)] public string? PassportNumber { get; set; }
    public DateTime? PassportExpiry { get; set; }
    [MaxLength(150)] public string? EmergencyContactName { get; set; }
    [MaxLength(50)] public string? EmergencyContactNumber { get; set; }
    [MaxLength(500)] public string? Address { get; set; }
    [MaxLength(100)] public string? City { get; set; }
    [MaxLength(100)] public string? Country { get; set; }
    public string? Notes { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<BookingTraveller> Bookings { get; set; } = new List<BookingTraveller>();
}

public class Trip
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    [Required, MaxLength(50)] public string TripCode { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string TripName { get; set; } = string.Empty;
    [MaxLength(500)] public string ShortDescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Destination { get; set; } = string.Empty;
    [MaxLength(200)] public string StartLocation { get; set; } = string.Empty;
    [MaxLength(200)] public string EndLocation { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public int DurationNights { get; set; }
    public TripType TripType { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Draft;
    public TripVisibility Visibility { get; set; } = TripVisibility.Public;
    [MaxLength(500)] public string CoverImageUrl { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18,2)")] public decimal BasePrice { get; set; }
    [MaxLength(10)] public string Currency { get; set; } = "USD";
    public int TotalCapacity { get; set; }
    public int AvailableSeats { get; set; }
    public int MinimumTravellers { get; set; } = 1;
    public int MaximumTravellers { get; set; }
    [MaxLength(150)] public string? HostGuide { get; set; }
    [MaxLength(250)] public string? ViaRoute { get; set; }
    [MaxLength(150)] public string? DriverName { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal EstimatedCost { get; set; }
    [MaxLength(150)] public string? ContactPerson { get; set; }
    [MaxLength(50)] public string? ContactNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<TripItineraryDay> ItineraryDays { get; set; } = new List<TripItineraryDay>();
    public ICollection<TripHotel> TripHotels { get; set; } = new List<TripHotel>();
    public ICollection<TripVehicle> TripVehicles { get; set; } = new List<TripVehicle>();
    public ICollection<TripVendor> TripVendors { get; set; } = new List<TripVendor>();
    public ICollection<TripMeal> TripMeals { get; set; } = new List<TripMeal>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<TripGuide> TripGuides { get; set; } = new List<TripGuide>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}

public class TripItineraryDay
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }
    public int DayNumber { get; set; }
    public DateTime? Date { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [MaxLength(200)] public string? Location { get; set; }
    [MaxLength(1000)] public string? Activities { get; set; }
    [MaxLength(20)] public string? StartTime { get; set; }
    [MaxLength(20)] public string? EndTime { get; set; }
    public string? Notes { get; set; }
}

public class Hotel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    [Required, MaxLength(200)] public string HotelName { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Location { get; set; } = string.Empty;
    [MaxLength(500)] public string? Address { get; set; }
    [MaxLength(50)] public string? ContactNumber { get; set; }
    [MaxLength(150)] public string? ContactPerson { get; set; }
    [MaxLength(100)] public string? DefaultRoomType { get; set; }
    public string? Notes { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TripHotel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    [MaxLength(100)] public string RoomType { get; set; } = "Standard Double";
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int RoomCount { get; set; } = 1;
    public string? Notes { get; set; }
}

public class Vehicle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    [Required, MaxLength(200)] public string VehicleName { get; set; } = string.Empty;
    [Required, MaxLength(50)] public string VehicleType { get; set; } = "Bus"; // Bus, MiniBus, Van, Car, TempoTraveller
    [MaxLength(50)] public string? RegistrationNumber { get; set; }
    public int Capacity { get; set; } = 20;
    [MaxLength(150)] public string? DriverName { get; set; }
    [MaxLength(50)] public string? DriverPhone { get; set; }
    [MaxLength(150)] public string? VendorName { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TripVehicle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public string? Notes { get; set; }
}

public class Vendor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    [Required, MaxLength(200)] public string VendorName { get; set; } = string.Empty;
    public VendorType VendorType { get; set; }
    [MaxLength(150)] public string? ContactPerson { get; set; }
    [MaxLength(50)] public string? Phone { get; set; }
    [MaxLength(200)] public string? Email { get; set; }
    [MaxLength(500)] public string? Address { get; set; }
    public string? Notes { get; set; }
    public bool Status { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TripVendor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid VendorId { get; set; }
    public Vendor? Vendor { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal ContractAmount { get; set; }
    public string? ServiceDescription { get; set; }
}

public class TripMeal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }
    public MealType MealType { get; set; }
    public MealOption MealOption { get; set; }
    [MaxLength(200)] public string? Description { get; set; }
    [MaxLength(200)] public string? DietaryOptions { get; set; } // Veg, Non-Veg, Jain, Vegan
}

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid? BookedByUserId { get; set; }
    [Required, MaxLength(50)] public string BookingReference { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public int NumberOfTravellers { get; set; } = 1;
    [Column(TypeName = "decimal(18,2)")] public decimal TotalAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal PaidAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal BalanceAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal TaxableAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal GstPercentage { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal CGST { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal SGST { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal IGST { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal TotalTax { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;
    [MaxLength(200)] public string ContactEmail { get; set; } = string.Empty;
    [MaxLength(50)] public string ContactPhone { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<BookingTraveller> BookingTravellers { get; set; } = new List<BookingTraveller>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class BookingTraveller
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }
    public Guid TravellerId { get; set; }
    public Traveller? Traveller { get; set; }
    [MaxLength(50)] public string RoomPreference { get; set; } = "Single";
    [MaxLength(100)] public string DietaryPreference { get; set; } = "Regular";
}

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Amount { get; set; }
    [Required, MaxLength(50)] public string PaymentMethod { get; set; } = "Mock Card"; // Mock Card, Stripe, Razorpay, Pay Later, Bank Transfer
    [Required, MaxLength(100)] public string TransactionReference { get; set; } = string.Empty;
    [MaxLength(50)] public string Provider { get; set; } = "Mock"; // Stripe, Razorpay, Mock
    [MaxLength(100)] public string? ProviderTransactionId { get; set; }
    [MaxLength(100)] public string? ProviderEventId { get; set; }
    [MaxLength(10)] public string Currency { get; set; } = "USD";
    [MaxLength(50)] public string PaymentType { get; set; } = "Full"; // Full, Deposit, Balance
    public PaymentStatus Status { get; set; } = PaymentStatus.Paid;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? Notes { get; set; }
}

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Guid? UserId { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    [Required] public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    [Required, MaxLength(100)] public string User { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Action { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Entity { get; set; } = string.Empty;
    [MaxLength(100)] public string? EntityId { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class Guide
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(50)] public string Phone { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Email { get; set; } = string.Empty;
    [MaxLength(200)] public string Languages { get; set; } = string.Empty;
    [MaxLength(500)] public string Specializations { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    [MaxLength(100)] public string? LicenseNumber { get; set; }
    public bool Status { get; set; } = true;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<TripGuide> TripGuides { get; set; } = new List<TripGuide>();
}

public class TripGuide
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid GuideId { get; set; }
    public Guide? Guide { get; set; }
    public string? Notes { get; set; }
}

public class Campaign
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    [Required, MaxLength(200)] public string Name { get; set; } = string.Empty;
    public CampaignType Type { get; set; } = CampaignType.Email;
    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;
    public string Subject { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public string TargetSegmentQuery { get; set; } = string.Empty;
    public DateTime? ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<CampaignRecipient> Recipients { get; set; } = new List<CampaignRecipient>();
}

public class CampaignRecipient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public Campaign? Campaign { get; set; }
    public Guid TravellerId { get; set; }
    public Traveller? Traveller { get; set; }
    [MaxLength(50)] public string Status { get; set; } = "Pending";
    public string? ErrorMessage { get; set; }
    public DateTime? ActionAt { get; set; }
}

public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public Guid? TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid? BookingId { get; set; }
    public Booking? Booking { get; set; }
    [Required, MaxLength(100)] public string SenderName { get; set; } = string.Empty;
    [Required] public string MessageText { get; set; } = string.Empty;
    public MessageType MessageType { get; set; } = MessageType.Text;
    [MaxLength(500)] public string? AttachmentUrl { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class SubscriptionQuota
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public SubscriptionTier Tier { get; set; } = SubscriptionTier.Starter;
    public int MaxActiveTrips { get; set; } = 5;
    public int MaxTeamMembers { get; set; } = 3;
    public int MaxBookingsPerMonth { get; set; } = 20;
    public bool Status { get; set; } = true;
    public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(30);
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

