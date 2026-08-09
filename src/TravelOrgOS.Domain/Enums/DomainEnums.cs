namespace TravelOrgOS.Domain.Enums;

public enum UserRole
{
    PlatformAdmin = 1,
    OrganizationOwner = 2,
    OrganizationAdmin = 3,
    TripCoordinator = 4,
    FinanceUser = 5,
    Traveller = 6
}

public enum TripType
{
    Leisure = 1,
    Adventure = 2,
    Family = 3,
    Pilgrimage = 4,
    Corporate = 5,
    Honeymoon = 6,
    GroupTour = 7,
    WeekendGetaway = 8,
    Custom = 9
}

public enum TripStatus
{
    Draft = 1,
    Published = 2,
    RegistrationOpen = 3,
    AlmostFull = 4,
    FullyBooked = 5,
    InProgress = 6,
    Completed = 7,
    Cancelled = 8
}

public enum TripVisibility
{
    Private = 1,
    OrganizationOnly = 2,
    Public = 3
}

public enum MealType
{
    Breakfast = 1,
    Lunch = 2,
    Dinner = 3,
    Snacks = 4
}

public enum MealOption
{
    Included = 1,
    Optional = 2,
    NotIncluded = 3
}

public enum VendorType
{
    Hotel = 1,
    Transport = 2,
    Restaurant = 3,
    Guide = 4,
    Activity = 5,
    Other = 6
}

public enum BookingStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Completed = 4
}

public enum PaymentStatus
{
    Pending = 1,
    PartiallyPaid = 2,
    Paid = 3,
    Refunded = 4
}

public enum NotificationType
{
    BookingCreated = 1,
    BookingConfirmed = 2,
    PaymentReceived = 3,
    PaymentPending = 4,
    TripPublished = 5,
    TripUpdated = 6,
    TripCancelled = 7,
    TripReminder = 8
}

public enum CampaignStatus
{
    Draft = 1,
    Scheduled = 2,
    Sending = 3,
    Sent = 4,
    Cancelled = 5,
    Failed = 6
}

public enum CampaignType
{
    Email = 1,
    SMS = 2,
    WhatsApp = 3
}

public enum SubscriptionTier
{
    Starter = 1,
    Growth = 2,
    Business = 3,
    Enterprise = 4
}

public enum MessageType
{
    Text = 1,
    System = 2
}

