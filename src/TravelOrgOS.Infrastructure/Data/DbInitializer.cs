using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Domain.Entities;
using TravelOrgOS.Domain.Enums;

namespace TravelOrgOS.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(TravelOrgOSDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Organizations.AnyAsync())
        {
            return; // Data already seeded
        }

        // 1. Create Organization
        var org = new Organization
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Demo Travel Experiences",
            Slug = "demo-travel",
            LegalName = "Demo Travel Experiences Private Limited",
            LogoUrl = "https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=200&h=200&fit=crop",
            PrimaryColor = "#1E88E5",
            SecondaryColor = "#0D47A1",
            WelcomeMessage = "Explore stunning destinations with Demo Travel Experiences!",
            Email = "contact@demo-travel.com",
            Phone = "+1 (555) 019-2831",
            Website = "https://demo-travel.com",
            Address = "100 Innovation Way, Suite 400",
            City = "San Francisco",
            Country = "USA",
            BusinessHours = "Mon - Fri: 9:00 AM - 6:00 PM PST",
            Description = "Premier group tour operator delivering authentic cultural, luxury, and adventure journeys.",
            FacebookUrl = "https://facebook.com/demotravel",
            InstagramUrl = "https://instagram.com/demotravel",
            LinkedInUrl = "https://linkedin.com/company/demotravel",
            WhatsAppNumber = "+15550192831",
            Status = true,
            CreatedAt = DateTime.UtcNow.AddDays(-60)
        };

        context.Organizations.Add(org);

        // 2. Create Users (PasswordHash for 'Demo@123' is a BCrypt or Simple Hash string)
        const string defaultPasswordHash = "AQAAAAIAAYagAAAAEG4z/Demo123HashedKeyForDevelopmentMode=";

        var users = new List<OrganizationUser>
        {
            new OrganizationUser
            {
                Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                OrganizationId = null,
                FullName = "System Platform Admin",
                Email = "admin@travelorgos.com",
                PasswordHash = defaultPasswordHash,
                Role = UserRole.PlatformAdmin
            },
            new OrganizationUser
            {
                Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                OrganizationId = org.Id,
                FullName = "Sarah Jenkins",
                Email = "owner@demo-travel.com",
                PasswordHash = defaultPasswordHash,
                Role = UserRole.OrganizationOwner
            },
            new OrganizationUser
            {
                Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                OrganizationId = org.Id,
                FullName = "Marcus Vance",
                Email = "manager@demo-travel.com",
                PasswordHash = defaultPasswordHash,
                Role = UserRole.OrganizationAdmin
            },
            new OrganizationUser
            {
                Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                OrganizationId = org.Id,
                FullName = "Elena Rostova",
                Email = "finance@demo-travel.com",
                PasswordHash = defaultPasswordHash,
                Role = UserRole.FinanceUser
            },
            new OrganizationUser
            {
                Id = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                OrganizationId = org.Id,
                FullName = "David Miller",
                Email = "traveller@demo-travel.com",
                PasswordHash = defaultPasswordHash,
                Role = UserRole.Traveller
            }
        };

        context.OrganizationUsers.AddRange(users);

        // 3. Create 10 Travellers
        var travellers = new List<Traveller>
        {
            new Traveller { Id = Guid.NewGuid(), OrganizationId = org.Id, FirstName = "David", LastName = "Miller", Email = "traveller@demo-travel.com", MobileNumber = "+1-555-0101", DateOfBirth = new DateTime(1988, 5, 12), Gender = "Male", Nationality = "American", PassportNumber = "US9876543", EmergencyContactName = "Rachel Miller", EmergencyContactNumber = "+1-555-0102", City = "San Jose", Country = "USA" },
            new Traveller { Id = Guid.NewGuid(), OrganizationId = org.Id, FirstName = "Alice", LastName = "Smith", Email = "alice.smith@example.com", MobileNumber = "+1-555-0103", DateOfBirth = new DateTime(1992, 8, 24), Gender = "Female", Nationality = "Canadian", PassportNumber = "CA1234567", EmergencyContactName = "John Smith", EmergencyContactNumber = "+1-555-0104", City = "Toronto", Country = "Canada" },
            new Traveller { Id = Guid.NewGuid(), OrganizationId = org.Id, FirstName = "Robert", LastName = "Taylor", Email = "robert.t@example.com", MobileNumber = "+44-20-7946-0912", DateOfBirth = new DateTime(1984, 11, 3), Gender = "Male", Nationality = "British", PassportNumber = "UK9988776", EmergencyContactName = "Claire Taylor", EmergencyContactNumber = "+44-20-7946-0913", City = "London", Country = "UK" },
            new Traveller { Id = Guid.NewGuid(), OrganizationId = org.Id, FirstName = "Sophia", LastName = "Garcia", Email = "sophia.g@example.com", MobileNumber = "+1-555-0105", DateOfBirth = new DateTime(1995, 2, 18), Gender = "Female", Nationality = "American", PassportNumber = "US1122334", EmergencyContactName = "Carlos Garcia", EmergencyContactNumber = "+1-555-0106", City = "Austin", Country = "USA" },
            new Traveller { Id = Guid.NewGuid(), OrganizationId = org.Id, FirstName = "Michael", LastName = "Brown", Email = "mbrown@example.com", MobileNumber = "+1-555-0107", DateOfBirth = new DateTime(1990, 7, 30), Gender = "Male", Nationality = "American", PassportNumber = "US4455667", EmergencyContactName = "Laura Brown", EmergencyContactNumber = "+1-555-0108", City = "Denver", Country = "USA" },
            new Traveller { Id = Guid.NewGuid(), OrganizationId = org.Id, FirstName = "Emma", LastName = "Wilson", Email = "emma.w@example.com", MobileNumber = "+61-2-9374-0000", DateOfBirth = new DateTime(1993, 12, 5), Gender = "Female", Nationality = "Australian", PassportNumber = "AU7788990", EmergencyContactName = "Peter Wilson", EmergencyContactNumber = "+61-2-9374-0001", City = "Sydney", Country = "Australia" },
            new Traveller { Id = Guid.NewGuid(), OrganizationId = org.Id, FirstName = "James", LastName = "Anderson", Email = "j.anderson@example.com", MobileNumber = "+1-555-0109", DateOfBirth = new DateTime(1982, 4, 15), Gender = "Male", Nationality = "American", PassportNumber = "US5566778", EmergencyContactName = "Karen Anderson", EmergencyContactNumber = "+1-555-0110", City = "Seattle", Country = "USA" },
            new Traveller { Id = Guid.NewGuid(), OrganizationId = org.Id, FirstName = "Olivia", LastName = "Martinez", Email = "olivia.m@example.com", MobileNumber = "+1-555-0111", DateOfBirth = new DateTime(1997, 9, 21), Gender = "Female", Nationality = "American", PassportNumber = "US8899001", EmergencyContactName = "Luis Martinez", EmergencyContactNumber = "+1-555-0112", City = "Miami", Country = "USA" },
            new Traveller { Id = Guid.NewGuid(), OrganizationId = org.Id, FirstName = "Daniel", LastName = "Thomas", Email = "d.thomas@example.com", MobileNumber = "+1-555-0113", DateOfBirth = new DateTime(1987, 1, 9), Gender = "Male", Nationality = "American", PassportNumber = "US2233445", EmergencyContactName = "Sarah Thomas", EmergencyContactNumber = "+1-555-0114", City = "Chicago", Country = "USA" },
            new Traveller { Id = Guid.NewGuid(), OrganizationId = org.Id, FirstName = "Isabella", LastName = "White", Email = "i.white@example.com", MobileNumber = "+1-555-0115", DateOfBirth = new DateTime(1994, 6, 14), Gender = "Female", Nationality = "American", PassportNumber = "US6677889", EmergencyContactName = "James White", EmergencyContactNumber = "+1-555-0116", City = "Boston", Country = "USA" }
        };

        context.Travellers.AddRange(travellers);

        // 4. Create Master Hotels, Vehicles, Vendors
        var hotel1 = new Hotel { Id = Guid.NewGuid(), OrganizationId = org.Id, HotelName = "Kumarakom Lake Resort", Location = "Kerala", Address = "Kumarakom, Kottayam", ContactNumber = "+91-481-2524800", ContactPerson = "Rajesh Nair", DefaultRoomType = "Luxury Heritage Villa" };
        var hotel2 = new Hotel { Id = Guid.NewGuid(), OrganizationId = org.Id, HotelName = "Coorg Wilderness Resort", Location = "Coorg", Address = "Virajpet Road, Madikeri", ContactNumber = "+91-827-2220000", ContactPerson = "Sunil Rao", DefaultRoomType = "Valley View Suite" };
        var hotel3 = new Hotel { Id = Guid.NewGuid(), OrganizationId = org.Id, HotelName = "Jaipur Palace & Spa", Location = "Jaipur", Address = "MI Road, Jaipur", ContactNumber = "+91-141-2371111", ContactPerson = "Vikram Singh", DefaultRoomType = "Royal Suite" };
        context.Hotels.AddRange(hotel1, hotel2, hotel3);

        var vehicle1 = new Vehicle { Id = Guid.NewGuid(), OrganizationId = org.Id, VehicleName = "Royal Luxury Coach", VehicleType = "Bus", RegistrationNumber = "KA-01-EQ-9988", Capacity = 40, DriverName = "Suresh Kumar", DriverPhone = "+91-9845012345", VendorName = "Starline Fleet" };
        var vehicle2 = new Vehicle { Id = Guid.NewGuid(), OrganizationId = org.Id, VehicleName = "Comfort Tempo Cruiser", VehicleType = "TempoTraveller", RegistrationNumber = "KA-05-MH-4455", Capacity = 15, DriverName = "Anil Verma", DriverPhone = "+91-9880199887", VendorName = "Starline Fleet" };
        context.Vehicles.AddRange(vehicle1, vehicle2);

        var vendor1 = new Vendor { Id = Guid.NewGuid(), OrganizationId = org.Id, VendorName = "Starline Fleet Services", VendorType = VendorType.Transport, ContactPerson = "Karan Patel", Phone = "+91-9800011223", Email = "booking@starlinefleet.com" };
        var vendor2 = new Vendor { Id = Guid.NewGuid(), OrganizationId = org.Id, VendorName = "Coastal Culinary Caterers", VendorType = VendorType.Restaurant, ContactPerson = "Chef George", Phone = "+91-9877766554", Email = "events@coastalculinary.com" };
        context.Vendors.AddRange(vendor1, vendor2);

        // 5. Create 5 Realistic Trips
        var trip1 = new Trip
        {
            Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
            OrganizationId = org.Id,
            TripCode = "KER-2026-001",
            TripName = "Kerala Backwaters & Houseboat Escape",
            ShortDescription = "Immerse in lush coconut groves, serene backwater cruises, and authentic Ayurvedic wellness in God's Own Country.",
            Description = "Experience five unforgettable days traversing Alleppey's iconic emerald houseboats, Kumarakom's tranquil bird sanctuary, and Cochin's historic Portuguese heritage quarter. Includes luxury resort stays, daily traditional sadhya banquets, and sunset cruises.",
            Destination = "Kerala, India",
            StartLocation = "Cochin International Airport (COK)",
            EndLocation = "Cochin International Airport (COK)",
            StartDate = DateTime.UtcNow.AddDays(15),
            EndDate = DateTime.UtcNow.AddDays(20),
            DurationDays = 5,
            DurationNights = 4,
            TripType = TripType.Leisure,
            Status = TripStatus.RegistrationOpen,
            Visibility = TripVisibility.Public,
            CoverImageUrl = "https://images.unsplash.com/photo-1602216056096-3b40cc0c9944?w=800&fit=crop",
            BasePrice = 850.00m,
            Currency = "USD",
            TotalCapacity = 20,
            AvailableSeats = 16,
            MinimumTravellers = 4,
            MaximumTravellers = 20,
            HostGuide = "Anand Krishnan",
            ViaRoute = "Cochin - Munnar - Alleppey - Kumarakom",
            DriverName = "Ramesh Kumar",
            EstimatedCost = 2400.00m,
            ContactPerson = "Sarah Jenkins",
            ContactNumber = "+1 (555) 019-2831",
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            PublishedAt = DateTime.UtcNow.AddDays(-20)
        };

        var trip2 = new Trip
        {
            Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"),
            OrganizationId = org.Id,
            TripCode = "CRG-2026-002",
            TripName = "Coorg Coffee Estate & Wilderness Weekend",
            ShortDescription = "Unwind among mist-covered mountains, aromatic coffee plantations, and cascading waterfalls in the Scotland of India.",
            Description = "A refreshing 3-day retreat featuring private estate walks, coffee cupping sessions, campfire evenings under star-lit skies, and river rafting at Dubare Elephant Camp.",
            Destination = "Coorg, Karnataka",
            StartLocation = "Bangalore Airport (BLR)",
            EndLocation = "Bangalore Airport (BLR)",
            StartDate = DateTime.UtcNow.AddDays(25),
            EndDate = DateTime.UtcNow.AddDays(28),
            DurationDays = 3,
            DurationNights = 2,
            TripType = TripType.WeekendGetaway,
            Status = TripStatus.RegistrationOpen,
            Visibility = TripVisibility.Public,
            CoverImageUrl = "https://images.unsplash.com/photo-1596402184320-417e7178b2cd?w=800&fit=crop",
            BasePrice = 350.00m,
            Currency = "USD",
            TotalCapacity = 15,
            AvailableSeats = 11,
            MinimumTravellers = 5,
            MaximumTravellers = 15,
            HostGuide = "Priya Hegde",
            ViaRoute = "Bangalore - Mysore - Madikeri",
            DriverName = "Suresh Gowda",
            EstimatedCost = 900.00m,
            ContactPerson = "Marcus Vance",
            ContactNumber = "+1 (555) 019-2831",
            CreatedAt = DateTime.UtcNow.AddDays(-25),
            PublishedAt = DateTime.UtcNow.AddDays(-15)
        };

        var trip3 = new Trip
        {
            Id = Guid.Parse("b3333333-3333-3333-3333-333333333333"),
            OrganizationId = org.Id,
            TripCode = "RAJ-2026-003",
            TripName = "Rajasthan Heritage & Royal Forts Odyssey",
            ShortDescription = "Step back into royal history with magnificent forts, vibrant bazaars, desert safaris, and palatial hospitality.",
            Description = "A grand 7-day tour through Jaipur's Pink City, Udaipur's romantic lakes, and Jaisalmer's golden sand dunes. Enjoy folk dance performances, camel treks, and dining in historic fort courtyards.",
            Destination = "Jaipur & Udaipur, Rajasthan",
            StartLocation = "Jaipur Airport (JAI)",
            EndLocation = "Udaipur Airport (UDR)",
            StartDate = DateTime.UtcNow.AddDays(40),
            EndDate = DateTime.UtcNow.AddDays(47),
            DurationDays = 7,
            DurationNights = 6,
            TripType = TripType.GroupTour,
            Status = TripStatus.Published,
            Visibility = TripVisibility.Public,
            CoverImageUrl = "https://images.unsplash.com/photo-1599661046289-e31897846e41?w=800&fit=crop",
            BasePrice = 1200.00m,
            Currency = "USD",
            TotalCapacity = 25,
            AvailableSeats = 25,
            MinimumTravellers = 8,
            MaximumTravellers = 25,
            HostGuide = "Rohan Sharma",
            ViaRoute = "Jaipur - Ajmer - Jodhpur - Udaipur",
            DriverName = "Mahipal Singh",
            EstimatedCost = 6500.00m,
            ContactPerson = "Sarah Jenkins",
            ContactNumber = "+1 (555) 019-2831",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            PublishedAt = DateTime.UtcNow.AddDays(-5)
        };

        var trip4 = new Trip
        {
            Id = Guid.Parse("b4444444-4444-4444-4444-444444444444"),
            OrganizationId = org.Id,
            TripCode = "GOA-2026-004",
            TripName = "Goa Sun, Sea & Portuguese Heritage Escape",
            ShortDescription = "Relax on sun-drenched golden beaches, sample fresh seafood, and explore UNESCO-listed Latin Quarter architecture.",
            Description = "4 days of coastal bliss! Includes private yacht sunset sail, Latin Quarter heritage walk, spice plantation feast, and water sports at Palolem Beach.",
            Destination = "Goa, India",
            StartLocation = "Goa Airport (GOI)",
            EndLocation = "Goa Airport (GOI)",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(14),
            DurationDays = 4,
            DurationNights = 3,
            TripType = TripType.Leisure,
            Status = TripStatus.AlmostFull,
            Visibility = TripVisibility.Public,
            CoverImageUrl = "https://images.unsplash.com/photo-1512343879784-a960bf40e7f2?w=800&fit=crop",
            BasePrice = 450.00m,
            Currency = "USD",
            TotalCapacity = 12,
            AvailableSeats = 2,
            MinimumTravellers = 4,
            MaximumTravellers = 12,
            HostGuide = "Francis D'Souza",
            ViaRoute = "Panaji - Old Goa - Calangute - Palolem",
            DriverName = "Anthony Fernandes",
            EstimatedCost = 2100.00m,
            ContactPerson = "Marcus Vance",
            ContactNumber = "+1 (555) 019-2831",
            CreatedAt = DateTime.UtcNow.AddDays(-40),
            PublishedAt = DateTime.UtcNow.AddDays(-30)
        };

        var trip5 = new Trip
        {
            Id = Guid.Parse("b5555555-5555-5555-5555-555555555555"),
            OrganizationId = org.Id,
            TripCode = "HIM-2026-005",
            TripName = "Himalayan Explorer & Valley Trek",
            ShortDescription = "Conquer high-altitude mountain passes, ancient monasteries, and breathtaking alpine landscapes in Manali & Solang.",
            Description = "An extraordinary 8-day Himalayan expedition designed for adventure seekers. Includes guided trekking, paragliding over alpine valleys, campfires, and monastery visits.",
            Destination = "Manali & Solang Valley, Himachal Pradesh",
            StartLocation = "Chandigarh Airport (IXC)",
            EndLocation = "Chandigarh Airport (IXC)",
            StartDate = DateTime.UtcNow.AddDays(60),
            EndDate = DateTime.UtcNow.AddDays(68),
            DurationDays = 8,
            DurationNights = 7,
            TripType = TripType.Adventure,
            Status = TripStatus.Draft,
            Visibility = TripVisibility.Private,
            CoverImageUrl = "https://images.unsplash.com/photo-1506744038136-46273834b3fb?w=800&fit=crop",
            BasePrice = 1500.00m,
            Currency = "USD",
            TotalCapacity = 15,
            AvailableSeats = 15,
            MinimumTravellers = 6,
            MaximumTravellers = 15,
            HostGuide = "Tenzing Norbu",
            ContactPerson = "Sarah Jenkins",
            ContactNumber = "+1 (555) 019-2831",
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };

        var trip6 = new Trip
        {
            Id = Guid.Parse("b6666666-6666-6666-6666-666666666666"),
            OrganizationId = org.Id,
            TripCode = "NEP-2026-006",
            TripName = "Nepal Annapurna Circuit Trek",
            ShortDescription = "Trek through rhododendron forests, sacred Hindu temples, and high-altitude Himalayan passes.",
            Description = "An epic 12-day alpine trek through the legendary Annapurna Sanctuary. Includes tea house stays, Thorong La Pass crossing, and Pokhara lakeside relaxation.",
            Destination = "Pokhara & Annapurna, Nepal",
            StartLocation = "Kathmandu Airport (KTM)",
            EndLocation = "Kathmandu Airport (KTM)",
            ViaRoute = "Kathmandu - Besisahar - Thorong La Pass - Pokhara",
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(10),
            DurationDays = 12,
            DurationNights = 11,
            TripType = TripType.Adventure,
            Status = TripStatus.InProgress,
            Visibility = TripVisibility.Public,
            CoverImageUrl = "https://images.unsplash.com/photo-1544735716-392fe2489ffa?w=800&fit=crop",
            BasePrice = 1800.00m,
            Currency = "USD",
            TotalCapacity = 10,
            AvailableSeats = 0,
            MinimumTravellers = 4,
            MaximumTravellers = 10,
            HostGuide = "Pemba Sherpa",
            DriverName = "Bikram Rai",
            EstimatedCost = 5200.00m,
            ContactPerson = "Sarah Jenkins",
            ContactNumber = "+1 (555) 019-2831",
            CreatedAt = DateTime.UtcNow.AddDays(-45),
            PublishedAt = DateTime.UtcNow.AddDays(-40)
        };

        var trip7 = new Trip
        {
            Id = Guid.Parse("b7777777-7777-7777-7777-777777777777"),
            OrganizationId = org.Id,
            TripCode = "BAL-2026-007",
            TripName = "Bali Cultural & Temple Sanctuary Retreat",
            ShortDescription = "Rejuvenate with rice terrace yoga, sacred water temples, and cliffside sunsets in tropical Bali.",
            Description = "A soul-enriching 7-day Bali retreat featuring luxury Ubud resort stays, Tanah Lot temple visits, and Balinese cooking workshops.",
            Destination = "Ubud & Seminyak, Bali",
            StartLocation = "Ngurah Rai Airport (DPS)",
            EndLocation = "Ngurah Rai Airport (DPS)",
            ViaRoute = "Denpasar - Ubud - Kintamani - Seminyak",
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow.AddDays(-23),
            DurationDays = 7,
            DurationNights = 6,
            TripType = TripType.Leisure,
            Status = TripStatus.Completed,
            Visibility = TripVisibility.Public,
            CoverImageUrl = "https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=800&fit=crop",
            BasePrice = 950.00m,
            Currency = "USD",
            TotalCapacity = 16,
            AvailableSeats = 0,
            MinimumTravellers = 6,
            MaximumTravellers = 16,
            HostGuide = "Wayan Sudarta",
            DriverName = "I Made Ketut",
            EstimatedCost = 6000.00m,
            ContactPerson = "Marcus Vance",
            ContactNumber = "+1 (555) 019-2831",
            CreatedAt = DateTime.UtcNow.AddDays(-90),
            PublishedAt = DateTime.UtcNow.AddDays(-80)
        };

        var trip8 = new Trip
        {
            Id = Guid.Parse("b8888888-8888-8888-8888-888888888888"),
            OrganizationId = org.Id,
            TripCode = "EGY-2026-008",
            TripName = "Egypt Pyramids & Nile Luxury River Cruise",
            ShortDescription = "Unravel ancient mysteries at the Pyramids of Giza, Valley of the Kings, and Karnak Temple.",
            Description = "A 9-day voyage through ancient civilization. Includes a 4-night luxury Nile River cruise, private Egyptologist tour guide, and Khan el-Khalili bazaar walk.",
            Destination = "Cairo & Luxor, Egypt",
            StartLocation = "Cairo International Airport (CAI)",
            EndLocation = "Cairo International Airport (CAI)",
            ViaRoute = "Cairo - Giza - Aswan - Luxor",
            StartDate = DateTime.UtcNow.AddDays(18),
            EndDate = DateTime.UtcNow.AddDays(27),
            DurationDays = 9,
            DurationNights = 8,
            TripType = TripType.GroupTour,
            Status = TripStatus.FullyBooked,
            Visibility = TripVisibility.Public,
            CoverImageUrl = "https://images.unsplash.com/photo-1503177119275-0aa32b3a9368?w=800&fit=crop",
            BasePrice = 2100.00m,
            Currency = "USD",
            TotalCapacity = 20,
            AvailableSeats = 0,
            MinimumTravellers = 8,
            MaximumTravellers = 20,
            HostGuide = "Ahmed Al-Mansoor",
            DriverName = "Tarek Hassan",
            EstimatedCost = 14000.00m,
            ContactPerson = "Sarah Jenkins",
            ContactNumber = "+1 (555) 019-2831",
            CreatedAt = DateTime.UtcNow.AddDays(-50),
            PublishedAt = DateTime.UtcNow.AddDays(-40)
        };

        var trip9 = new Trip
        {
            Id = Guid.Parse("b9999999-9999-9999-9999-999999999999"),
            OrganizationId = org.Id,
            TripCode = "SL-2026-009",
            TripName = "Sri Lanka Wild Safari & Hill Country",
            ShortDescription = "Spot leopards in Yala, tour tea plantations in Nuwara Eliya, and climb Sigiriya Rock Fortress.",
            Description = "6 days of wild adventure and island beauty across Sri Lanka's cultural triangle and southern coastline.",
            Destination = "Kandy & Yala, Sri Lanka",
            StartLocation = "Colombo Airport (CMB)",
            EndLocation = "Colombo Airport (CMB)",
            ViaRoute = "Colombo - Kandy - Nuwara Eliya - Yala",
            StartDate = DateTime.UtcNow.AddDays(75),
            EndDate = DateTime.UtcNow.AddDays(81),
            DurationDays = 6,
            DurationNights = 5,
            TripType = TripType.Adventure,
            Status = TripStatus.Cancelled,
            Visibility = TripVisibility.Public,
            CoverImageUrl = "https://images.unsplash.com/photo-1586861635167-e5223aadc9fe?w=800&fit=crop",
            BasePrice = 780.00m,
            Currency = "USD",
            TotalCapacity = 15,
            AvailableSeats = 15,
            MinimumTravellers = 4,
            MaximumTravellers = 15,
            HostGuide = "Chaminda Perera",
            DriverName = "Sunil Fernando",
            EstimatedCost = 1800.00m,
            ContactPerson = "Marcus Vance",
            ContactNumber = "+1 (555) 019-2831",
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            PublishedAt = DateTime.UtcNow.AddDays(-15)
        };

        var trip10 = new Trip
        {
            Id = Guid.Parse("b1010101-1010-1010-1010-101010101010"),
            OrganizationId = org.Id,
            TripCode = "JPN-2026-010",
            TripName = "Japan Cherry Blossom & Bullet Train Expedition",
            ShortDescription = "Experience Tokyo neon lights, Kyoto shrines, Mount Fuji views, and Shinkansen bullet train rides.",
            Description = "An extraordinary 10-day cultural immersive journey across Japan during spring blossom season. Includes tea ceremonies, ryokan hot spring stays, and gourmet ramen tours.",
            Destination = "Tokyo & Kyoto, Japan",
            StartLocation = "Tokyo Haneda Airport (HND)",
            EndLocation = "Osaka Kansai Airport (KIX)",
            ViaRoute = "Tokyo - Hakone - Kyoto - Osaka",
            StartDate = DateTime.UtcNow.AddDays(50),
            EndDate = DateTime.UtcNow.AddDays(60),
            DurationDays = 10,
            DurationNights = 9,
            TripType = TripType.GroupTour,
            Status = TripStatus.Published,
            Visibility = TripVisibility.Public,
            CoverImageUrl = "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?w=800&fit=crop",
            BasePrice = 2850.00m,
            Currency = "USD",
            TotalCapacity = 25,
            AvailableSeats = 18,
            MinimumTravellers = 10,
            MaximumTravellers = 25,
            HostGuide = "Kenji Sato",
            DriverName = "Takahashi Tanaka",
            EstimatedCost = 18000.00m,
            ContactPerson = "Sarah Jenkins",
            ContactNumber = "+1 (555) 019-2831",
            CreatedAt = DateTime.UtcNow.AddDays(-12),
            PublishedAt = DateTime.UtcNow.AddDays(-8)
        };

        context.Trips.AddRange(trip1, trip2, trip3, trip4, trip5, trip6, trip7, trip8, trip9, trip10);

        // 6. Itinerary Days for Trip 1 (Kerala)
        var trip1Days = new List<TripItineraryDay>
        {
            new TripItineraryDay { Id = Guid.NewGuid(), TripId = trip1.Id, DayNumber = 1, Title = "Arrival in Cochin & Fort Kochi Heritage Walk", Description = "Welcome meeting at Cochin Airport followed by private transfer to Fort Kochi. Afternoon guided walk past Dutch Palace and Chinese Fishing Nets.", Location = "Fort Kochi", Activities = "Airport Pickup, Heritage Walk, Sunset Tea", StartTime = "10:00 AM", EndTime = "06:00 PM" },
            new TripItineraryDay { Id = Guid.NewGuid(), TripId = trip1.Id, DayNumber = 2, Title = "Drive to Alleppey & Houseboat Boarding", Description = "Board luxury private houseboat at Punnamada Jetty. Enjoy traditional Kerala lunch while cruising narrow canals.", Location = "Alleppey Backwaters", Activities = "Houseboat Cruise, Village Visit, Kerala Sadhya Lunch", StartTime = "09:00 AM", EndTime = "07:00 PM" },
            new TripItineraryDay { Id = Guid.NewGuid(), TripId = trip1.Id, DayNumber = 3, Title = "Kumarakom Bird Sanctuary & Resort Relaxation", Description = "Check in to Kumarakom Lake Resort. Afternoon visit to Kumarakom Bird Sanctuary and Ayurvedic herbal massage.", Location = "Kumarakom", Activities = "Bird Watching, Ayurvedic Massage, Infinity Pool Sunset", StartTime = "08:30 AM", EndTime = "06:30 PM" },
            new TripItineraryDay { Id = Guid.NewGuid(), TripId = trip1.Id, DayNumber = 4, Title = "Spice Plantation Tour & Cultural Kathakali Evening", Description = "Explore organic cardamom, pepper, and vanilla plantations. Evening authentic Kathakali dance performance.", Location = "Kumarakom & Cochin", Activities = "Spice Tour, Kathakali Show, Farewell Banquet", StartTime = "09:00 AM", EndTime = "09:00 PM" },
            new TripItineraryDay { Id = Guid.NewGuid(), TripId = trip1.Id, DayNumber = 5, Title = "Souvenir Shopping & Airport Departure", Description = "Morning breakfast and shopping at local handicraft market. Transfer to Cochin Airport for onward flights.", Location = "Cochin Airport", Activities = "Shopping, Airport Transfer", StartTime = "09:00 AM", EndTime = "01:00 PM" }
        };
        context.TripItineraryDays.AddRange(trip1Days);

        // Trip Hotels, Vehicles, Meals & Vendors
        context.TripHotels.Add(new TripHotel { Id = Guid.NewGuid(), TripId = trip1.Id, HotelId = hotel1.Id, RoomType = "Luxury Heritage Villa", CheckIn = trip1.StartDate.AddDays(2), CheckOut = trip1.StartDate.AddDays(4), RoomCount = 10, Notes = "Lakefront villas reserved" });
        context.TripVehicles.Add(new TripVehicle { Id = Guid.NewGuid(), TripId = trip1.Id, VehicleId = vehicle1.Id, Notes = "AC Volvo Coach for all land transfers" });
        context.TripVendors.Add(new TripVendor { Id = Guid.NewGuid(), TripId = trip1.Id, VendorId = vendor1.Id, ContractAmount = 1200.00m, ServiceDescription = "5-day dedicated coach transport" });

        context.TripMeals.AddRange(new List<TripMeal>
        {
            new TripMeal { Id = Guid.NewGuid(), TripId = trip1.Id, MealType = MealType.Breakfast, MealOption = MealOption.Included, Description = "Daily Resort Buffet Breakfast", DietaryOptions = "Veg, Non-Veg, Jain, Vegan" },
            new TripMeal { Id = Guid.NewGuid(), TripId = trip1.Id, MealType = MealType.Lunch, MealOption = MealOption.Included, Description = "Houseboat Traditional Sadhya Served on Banana Leaf", DietaryOptions = "Veg, Non-Veg" },
            new TripMeal { Id = Guid.NewGuid(), TripId = trip1.Id, MealType = MealType.Dinner, MealOption = MealOption.Included, Description = "Seafood Special Dinner", DietaryOptions = "Veg, Non-Veg, Custom" }
        });

        // 7. Seed Existing Bookings for Trip 1 and Trip 2
        var booking1 = new Booking
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            TripId = trip1.Id,
            BookedByUserId = users[4].Id,
            BookingReference = "BK-KER-9821",
            BookingDate = DateTime.UtcNow.AddDays(-10),
            NumberOfTravellers = 2,
            TotalAmount = 1700.00m,
            PaidAmount = 1700.00m,
            BalanceAmount = 0.00m,
            PaymentStatus = PaymentStatus.Paid,
            BookingStatus = BookingStatus.Confirmed,
            ContactEmail = "traveller@demo-travel.com",
            ContactPhone = "+1-555-0101",
            SpecialRequests = "Vegetarian meals preferred on houseboat."
        };

        var booking2 = new Booking
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            TripId = trip1.Id,
            BookedByUserId = null,
            BookingReference = "BK-KER-4412",
            BookingDate = DateTime.UtcNow.AddDays(-5),
            NumberOfTravellers = 2,
            TotalAmount = 1700.00m,
            PaidAmount = 500.00m,
            BalanceAmount = 1200.00m,
            PaymentStatus = PaymentStatus.PartiallyPaid,
            BookingStatus = BookingStatus.Confirmed,
            ContactEmail = "alice.smith@example.com",
            ContactPhone = "+1-555-0103"
        };

        var booking3 = new Booking
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            TripId = trip2.Id,
            BookedByUserId = null,
            BookingReference = "BK-CRG-7719",
            BookingDate = DateTime.UtcNow.AddDays(-3),
            NumberOfTravellers = 4,
            TotalAmount = 1400.00m,
            PaidAmount = 1400.00m,
            BalanceAmount = 0.00m,
            PaymentStatus = PaymentStatus.Paid,
            BookingStatus = BookingStatus.Confirmed,
            ContactEmail = "sophia.g@example.com",
            ContactPhone = "+1-555-0105"
        };

        context.Bookings.AddRange(booking1, booking2, booking3);

        // Booking Travellers
        context.BookingTravellers.Add(new BookingTraveller { Id = Guid.NewGuid(), BookingId = booking1.Id, TravellerId = travellers[0].Id, RoomPreference = "Double", DietaryPreference = "Vegetarian" });
        context.BookingTravellers.Add(new BookingTraveller { Id = Guid.NewGuid(), BookingId = booking1.Id, TravellerId = travellers[1].Id, RoomPreference = "Double", DietaryPreference = "Vegetarian" });

        // Payments
        context.Payments.Add(new Payment { Id = Guid.NewGuid(), OrganizationId = org.Id, BookingId = booking1.Id, Amount = 1700.00m, PaymentMethod = "Credit Card", TransactionReference = "TXN-99881122", Status = PaymentStatus.Paid, PaymentDate = DateTime.UtcNow.AddDays(-10) });
        context.Payments.Add(new Payment { Id = Guid.NewGuid(), OrganizationId = org.Id, BookingId = booking2.Id, Amount = 500.00m, PaymentMethod = "Mock Deposit", TransactionReference = "TXN-55443322", Status = PaymentStatus.Paid, PaymentDate = DateTime.UtcNow.AddDays(-5) });
        context.Payments.Add(new Payment { Id = Guid.NewGuid(), OrganizationId = org.Id, BookingId = booking3.Id, Amount = 1400.00m, PaymentMethod = "Mock Card", TransactionReference = "TXN-77665544", Status = PaymentStatus.Paid, PaymentDate = DateTime.UtcNow.AddDays(-3) });

        // Notifications
        context.Notifications.Add(new Notification { Id = Guid.NewGuid(), OrganizationId = org.Id, Title = "New Booking Received", Message = "Booking BK-KER-9821 has been confirmed for Kerala Backwaters Escape.", Type = NotificationType.BookingConfirmed, IsRead = false, CreatedAt = DateTime.UtcNow.AddDays(-10) });
        context.Notifications.Add(new Notification { Id = Guid.NewGuid(), OrganizationId = org.Id, Title = "Partial Payment Recorded", Message = "Deposit payment of $500 received for Booking BK-KER-4412.", Type = NotificationType.PaymentReceived, IsRead = false, CreatedAt = DateTime.UtcNow.AddDays(-5) });

        // Audit Logs
        context.AuditLogs.Add(new AuditLog { Id = Guid.NewGuid(), OrganizationId = org.Id, User = "owner@demo-travel.com", Action = "TripPublished", Entity = "Trip", EntityId = trip1.Id.ToString(), Details = "Kerala Backwaters Escape trip published to Traveller Portal.", Timestamp = DateTime.UtcNow.AddDays(-20) });
        context.AuditLogs.Add(new AuditLog { Id = Guid.NewGuid(), OrganizationId = org.Id, User = "traveller@demo-travel.com", Action = "BookingCreated", Entity = "Booking", EntityId = booking1.Id.ToString(), Details = "Booking BK-KER-9821 created via Branded Traveller Portal.", Timestamp = DateTime.UtcNow.AddDays(-10) });

        await context.SaveChangesAsync();
    }
}
