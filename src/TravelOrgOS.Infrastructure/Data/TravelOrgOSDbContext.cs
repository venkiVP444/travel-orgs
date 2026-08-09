using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Domain.Entities;

namespace TravelOrgOS.Infrastructure.Data;

public class TravelOrgOSDbContext : DbContext
{
    public TravelOrgOSDbContext(DbContextOptions<TravelOrgOSDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationUser> OrganizationUsers => Set<OrganizationUser>();
    public DbSet<Traveller> Travellers => Set<Traveller>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<TripItineraryDay> TripItineraryDays => Set<TripItineraryDay>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<TripHotel> TripHotels => Set<TripHotel>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<TripVehicle> TripVehicles => Set<TripVehicle>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<TripVendor> TripVendors => Set<TripVendor>();
    public DbSet<TripMeal> TripMeals => Set<TripMeal>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingTraveller> BookingTravellers => Set<BookingTraveller>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Guide> Guides => Set<Guide>();
    public DbSet<TripGuide> TripGuides => Set<TripGuide>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignRecipient> CampaignRecipients => Set<CampaignRecipient>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<SubscriptionQuota> SubscriptionQuotas => Set<SubscriptionQuota>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraints
        modelBuilder.Entity<Organization>()
            .HasIndex(o => o.Slug)
            .IsUnique();

        modelBuilder.Entity<OrganizationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Trip>()
            .HasIndex(t => new { t.OrganizationId, t.TripCode })
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.BookingReference)
            .IsUnique();

        // Foreign Key Relationships
        modelBuilder.Entity<OrganizationUser>()
            .HasOne(u => u.Organization)
            .WithMany(o => o.Users)
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Traveller>()
            .HasOne(t => t.Organization)
            .WithMany(o => o.Travellers)
            .HasForeignKey(t => t.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Trip>()
            .HasOne(t => t.Organization)
            .WithMany(o => o.Trips)
            .HasForeignKey(t => t.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Trip)
            .WithMany(t => t.Bookings)
            .HasForeignKey(b => b.TripId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BookingTraveller>()
            .HasOne(bt => bt.Booking)
            .WithMany(b => b.BookingTravellers)
            .HasForeignKey(bt => bt.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookingTraveller>()
            .HasOne(bt => bt.Traveller)
            .WithMany(t => t.Bookings)
            .HasForeignKey(bt => bt.TravellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Booking)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.TransactionReference);

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.ProviderEventId);

        // Guide Constraints & Relations
        modelBuilder.Entity<Guide>()
            .HasIndex(g => new { g.OrganizationId, g.Email })
            .IsUnique();

        modelBuilder.Entity<Guide>()
            .HasOne(g => g.Organization)
            .WithMany(o => o.Guides)
            .HasForeignKey(g => g.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TripGuide>()
            .HasOne(tg => tg.Trip)
            .WithMany(t => t.TripGuides)
            .HasForeignKey(tg => tg.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TripGuide>()
            .HasOne(tg => tg.Guide)
            .WithMany(g => g.TripGuides)
            .HasForeignKey(tg => tg.GuideId)
            .OnDelete(DeleteBehavior.Restrict);

        // Campaign Relations
        modelBuilder.Entity<Campaign>()
            .HasOne(c => c.Organization)
            .WithMany(o => o.Campaigns)
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CampaignRecipient>()
            .HasOne(cr => cr.Campaign)
            .WithMany(c => c.Recipients)
            .HasForeignKey(cr => cr.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CampaignRecipient>()
            .HasOne(cr => cr.Traveller)
            .WithMany()
            .HasForeignKey(cr => cr.TravellerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Chat Relations
        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.Organization)
            .WithMany()
            .HasForeignKey(cm => cm.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.Trip)
            .WithMany(t => t.ChatMessages)
            .HasForeignKey(cm => cm.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.Booking)
            .WithMany()
            .HasForeignKey(cm => cm.BookingId)
            .OnDelete(DeleteBehavior.NoAction);

        // Subscription Relations
        modelBuilder.Entity<SubscriptionQuota>()
            .HasOne(sq => sq.Organization)
            .WithOne()
            .HasForeignKey<SubscriptionQuota>(sq => sq.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
