import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Booking } from '../../models/models';

@Component({
  selector: 'app-portal-booking-success',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="success-wrapper">
      <div class="card success-card" *ngIf="booking">
        <div class="success-icon">
          <i class="fa-solid fa-circle-check"></i>
        </div>
        <h2>Booking Confirmed!</h2>
        <p class="reference-code">Reference #: <strong>{{ booking.bookingReference }}</strong></p>
        <p class="subtitle">Thank you for booking with us. A confirmation email has been dispatched to <strong>{{ booking.contactEmail }}</strong>.</p>

        <div class="details-box">
          <div class="detail-row">
            <span>Trip Package:</span>
            <strong>{{ booking.tripName }}</strong>
          </div>
          <div class="detail-row">
            <span>Passengers:</span>
            <strong>{{ booking.numberOfTravellers }} Person(s)</strong>
          </div>
          <div class="detail-row">
            <span>Total Price:</span>
            <strong>\${{ booking.totalAmount | number:'1.2-2' }}</strong>
          </div>
          <div class="detail-row">
            <span>Amount Paid:</span>
            <strong class="text-success">\${{ booking.paidAmount | number:'1.2-2' }}</strong>
          </div>
          <div class="detail-row" *ngIf="booking.balanceAmount > 0">
            <span>Outstanding Balance:</span>
            <strong class="text-warning">\${{ booking.balanceAmount | number:'1.2-2' }}</strong>
          </div>
        </div>

        <div class="card-actions">
          <a [routerLink]="['/portal', slug]" class="btn btn-primary btn-block">
            <i class="fa-solid fa-house"></i> Return to Organization Portal
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .success-wrapper { min-height: 100vh; background: #F8FAFC; display: flex; align-items: center; justify-content: center; padding: 20px; }
    .success-card { max-width: 500px; width: 100%; text-align: center; padding: 40px; }
    .success-icon { font-size: 4rem; color: #16A34A; margin-bottom: 16px; }
    .success-card h2 { font-size: 1.8rem; color: #0F172A; }
    .reference-code { font-size: 1.2rem; color: #1E88E5; margin: 8px 0; font-weight: 700; }
    .subtitle { font-size: 0.9rem; color: #64748B; margin-bottom: 24px; }
    .details-box { background: #F8FAFC; border: 1px solid #E2E8F0; padding: 20px; border-radius: 12px; margin-bottom: 28px; text-align: left; }
    .detail-row { display: flex; justify-content: space-between; margin-bottom: 8px; font-size: 0.92rem; }
    .text-success { color: #16A34A; }
    .text-warning { color: #D97706; }
    .btn-block { width: 100%; text-align: center; }
  `]
})
export class PortalBookingSuccessComponent implements OnInit {
  slug = '';
  bookingRef = '';
  booking: Booking | null = null;

  constructor(private route: ActivatedRoute, private apiService: ApiService) {}

  ngOnInit(): void {
    this.slug = this.route.snapshot.paramMap.get('organizationSlug') || 'demo-travel';
    this.bookingRef = this.route.snapshot.paramMap.get('bookingId') || '';

    this.apiService.getBookingByReference(this.bookingRef).subscribe({
      next: (b) => this.booking = b
    });
  }
}
