import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Organization, Trip } from '../../models/models';

@Component({
  selector: 'app-portal-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="portal-wrapper" *ngIf="org">
      <!-- BRANDED HEADER -->
      <header class="portal-header" [style.background]="'linear-gradient(135deg, ' + org.primaryColor + ', ' + org.secondaryColor + ')'">
        <div class="header-content">
          <img [src]="org.logoUrl || 'https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=100&fit=crop'" class="portal-logo">
          <h1 class="org-name">{{ org.name }}</h1>
          <p class="welcome-msg">{{ org.welcomeMessage }}</p>
        </div>
      </header>

      <!-- PORTAL BODY -->
      <main class="portal-container">
        <div class="section-header">
          <h2>Upcoming Published Trips</h2>
          <p>Browse and book authentic travel experiences</p>
        </div>

        <div class="portal-trips-grid">
          <div *ngFor="let t of trips" class="portal-trip-card">
            <div class="card-img-box">
              <img [src]="t.coverImageUrl" [alt]="t.tripName" class="card-img">
              <span class="price-tag">\${{ t.basePrice | number:'1.0-0' }} {{ t.currency }}</span>
            </div>

            <div class="card-content">
              <span class="trip-badge">{{ t.durationDays }} Days / {{ t.durationNights }} Nights</span>
              <h3 class="trip-title">{{ t.tripName }}</h3>
              <p class="destination"><i class="fa-solid fa-location-dot"></i> {{ t.destination }}</p>

              <div class="seats-info">
                <span><i class="fa-solid fa-users"></i> {{ t.availableSeats }} seats remaining</span>
              </div>

              <div class="card-actions">
                <a [routerLink]="['/portal', slug, 'trips', t.id]" class="btn btn-outline btn-block">
                  View Details & Itinerary
                </a>
                <a [routerLink]="['/portal', slug, 'book', t.id]" class="btn btn-primary btn-block">
                  Book Now
                </a>
              </div>
            </div>
          </div>
        </div>

        <div *ngIf="trips.length === 0" class="empty-state">
          <i class="fa-solid fa-compass empty-icon"></i>
          <p>No trips currently published for this organization.</p>
        </div>
      </main>

      <!-- FOOTER -->
      <footer class="portal-footer">
        <p>{{ org.name }} &bull; {{ org.phone }} &bull; {{ org.email }}</p>
        <p class="powered-by">Powered by <strong>TravelOrgOS</strong></p>
      </footer>
    </div>
  `,
  styles: [`
    .portal-wrapper { min-height: 100vh; background: #F8FAFC; display: flex; flex-direction: column; }
    .portal-header { color: white; padding: 40px 20px; text-align: center; border-bottom-left-radius: 24px; border-bottom-right-radius: 24px; }
    .portal-logo { width: 70px; height: 70px; border-radius: 18px; object-fit: cover; border: 3px solid white; box-shadow: 0 8px 16px rgba(0,0,0,0.2); margin-bottom: 12px; }
    .org-name { font-size: 1.8rem; font-weight: 800; }
    .welcome-msg { font-size: 0.95rem; opacity: 0.9; margin-top: 6px; }

    .portal-container { max-width: 900px; margin: 0 auto; padding: 32px 20px; flex-grow: 1; width: 100%; }
    .section-header { text-align: center; margin-bottom: 28px; }
    .section-header h2 { font-size: 1.5rem; color: #0F172A; }

    .portal-trips-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 24px; }
    .portal-trip-card { background: white; border-radius: 16px; border: 1px solid #E2E8F0; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.05); display: flex; flex-direction: column; }
    .card-img-box { position: relative; height: 190px; }
    .card-img { width: 100%; height: 100%; object-fit: cover; }
    .price-tag { position: absolute; bottom: 12px; right: 12px; background: rgba(15, 23, 42, 0.85); backdrop-filter: blur(4px); color: white; font-weight: 800; padding: 4px 12px; border-radius: 20px; font-size: 1.05rem; }

    .card-content { padding: 20px; flex-grow: 1; display: flex; flex-direction: column; }
    .trip-badge { font-size: 0.75rem; font-weight: 700; color: #1E88E5; background: #E3F2FD; padding: 2px 10px; border-radius: 12px; align-self: flex-start; margin-bottom: 8px; }
    .trip-title { font-size: 1.2rem; color: #0F172A; margin-bottom: 4px; }
    .destination { font-size: 0.85rem; color: #64748B; margin-bottom: 16px; }
    .seats-info { font-size: 0.82rem; font-weight: 600; color: #16A34A; margin-bottom: 20px; }

    .card-actions { display: flex; flex-direction: column; gap: 8px; margin-top: auto; }
    .btn-block { width: 100%; text-align: center; }

    .portal-footer { background: #0F172A; color: #94A3B8; text-align: center; padding: 24px; font-size: 0.85rem; margin-top: 40px; }
    .powered-by { font-size: 0.78rem; margin-top: 6px; }
    .empty-state { text-align: center; padding: 60px 20px; color: #64748B; }
    .empty-icon { font-size: 3rem; margin-bottom: 12px; color: #CBD5E1; }
  `]
})
export class PortalHomeComponent implements OnInit {
  slug = '';
  org: Organization | null = null;
  trips: Trip[] = [];

  constructor(private route: ActivatedRoute, private apiService: ApiService) {}

  ngOnInit(): void {
    this.slug = this.route.snapshot.paramMap.get('organizationSlug') || 'demo-travel';
    this.apiService.getPortalTrips(this.slug).subscribe({
      next: (data) => {
        this.org = data.organization;
        this.trips = data.trips;
      }
    });
  }
}
