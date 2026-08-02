import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Trip } from '../../models/models';

@Component({
  selector: 'app-portal-trip-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="portal-detail-wrapper" *ngIf="trip">
      <!-- COVER BANNER -->
      <div class="cover-hero">
        <img [src]="trip.coverImageUrl" [alt]="trip.tripName" class="hero-img">
        <div class="hero-overlay">
          <a [routerLink]="['/portal', slug]" class="back-link"><i class="fa-solid fa-arrow-left"></i> Back to All Trips</a>
          <h1 class="hero-title">{{ trip.tripName }}</h1>
          <p class="hero-sub"><i class="fa-solid fa-location-dot"></i> {{ trip.destination }} &bull; {{ trip.durationDays }} Days / {{ trip.durationNights }} Nights</p>
        </div>
      </div>

      <div class="portal-container">
        <div class="detail-grid">
          <!-- MAIN ITINERARY CONTENT -->
          <div class="main-info">
            <div class="card info-card">
              <h3>Trip Highlights & Overview</h3>
              <p class="description-text">{{ trip.description }}</p>
            </div>

            <!-- ITINERARY DAYS -->
            <div class="card info-card mt-4">
              <h3><i class="fa-solid fa-calendar-days"></i> Day-by-Day Itinerary</h3>

              <div class="itinerary-timeline">
                <div *ngFor="let day of trip.itineraryDays" class="timeline-item">
                  <div class="day-badge">Day {{ day.dayNumber }}</div>
                  <div class="day-content">
                    <h4>{{ day.title }}</h4>
                    <p class="location-tag" *ngIf="day.location"><i class="fa-solid fa-map-pin"></i> {{ day.location }}</p>
                    <p class="day-desc">{{ day.description }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- BOOKING ACTION CARD -->
          <div class="sidebar-booking">
            <div class="card booking-sticky-card">
              <span class="price-label">Price per passenger</span>
              <div class="price-amount">\${{ trip.basePrice | number:'1.0-0' }} <span>{{ trip.currency }}</span></div>

              <div class="availability-status">
                <i class="fa-solid fa-chair text-success"></i> <strong>{{ trip.availableSeats }} seats remaining</strong>
              </div>

              <a [routerLink]="['/portal', slug, 'book', trip.id]" class="btn btn-primary btn-block btn-lg mt-3">
                Book This Trip Now
              </a>

              <div class="contact-info mt-4">
                <p><strong>Have Questions?</strong></p>
                <p><i class="fa-solid fa-phone"></i> {{ trip.contactNumber || '+1-555-019-2831' }}</p>
                <p><i class="fa-solid fa-user-guide"></i> Host Guide: {{ trip.hostGuide || 'Dedicated Tour Escort' }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .portal-detail-wrapper { background: #F8FAFC; min-height: 100vh; }
    .cover-hero { position: relative; height: 360px; }
    .hero-img { width: 100%; height: 100%; object-fit: cover; }
    .hero-overlay { position: absolute; top: 0; left: 0; right: 0; bottom: 0; background: linear-gradient(to top, rgba(15, 23, 42, 0.9), rgba(15, 23, 42, 0.3)); color: white; padding: 40px 20px; display: flex; flex-direction: column; justify-content: space-between; max-width: 1000px; margin: 0 auto; }
    .back-link { color: white; font-weight: 600; font-size: 0.9rem; }
    .hero-title { font-size: 2.2rem; font-weight: 800; }
    .hero-sub { font-size: 1rem; opacity: 0.9; }

    .portal-container { max-width: 1000px; margin: 0 auto; padding: 32px 20px; }
    .detail-grid { display: grid; grid-template-columns: 1fr 340px; gap: 28px; }
    @media (max-width: 768px) { .detail-grid { grid-template-columns: 1fr; } }

    .info-card { padding: 28px; }
    .description-text { color: #334155; line-height: 1.7; margin-top: 12px; }
    .mt-4 { margin-top: 24px; }

    .itinerary-timeline { margin-top: 20px; display: flex; flex-direction: column; gap: 20px; }
    .timeline-item { display: flex; gap: 16px; position: relative; }
    .day-badge { background: #1E88E5; color: white; font-weight: 800; font-size: 0.8rem; padding: 6px 12px; border-radius: 20px; height: max-content; }
    .day-content h4 { font-size: 1.1rem; color: #0F172A; }
    .location-tag { font-size: 0.8rem; color: #64748B; margin: 2px 0 6px; }
    .day-desc { font-size: 0.9rem; color: #475569; }

    .booking-sticky-card { position: sticky; top: 30px; padding: 28px; }
    .price-label { font-size: 0.8rem; text-transform: uppercase; color: #64748B; font-weight: 700; }
    .price-amount { font-size: 2.2rem; font-weight: 800; color: #0F172A; }
    .price-amount span { font-size: 1rem; color: #64748B; font-weight: 600; }
    .availability-status { margin-top: 12px; font-size: 0.9rem; }
    .text-success { color: #16A34A; }
    .btn-block { width: 100%; text-align: center; }
    .contact-info { font-size: 0.85rem; color: #64748B; border-top: 1px solid #E2E8F0; padding-top: 16px; }
  `]
})
export class PortalTripDetailComponent implements OnInit {
  slug = '';
  tripId = '';
  trip: Trip | null = null;

  constructor(private route: ActivatedRoute, private apiService: ApiService) {}

  ngOnInit(): void {
    this.slug = this.route.snapshot.paramMap.get('organizationSlug') || 'demo-travel';
    this.tripId = this.route.snapshot.paramMap.get('tripId') || '';

    this.apiService.getPortalTrip(this.slug, this.tripId).subscribe({
      next: (t) => this.trip = t
    });
  }
}
