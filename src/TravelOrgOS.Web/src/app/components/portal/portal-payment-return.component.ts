import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-portal-payment-return',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="payment-return-wrapper">
      <div class="container">
        <div class="card status-card text-center" *ngIf="loading">
          <div class="spinner-border text-primary my-4" role="status"></div>
          <h3>Verifying Payment Status...</h3>
          <p class="text-muted">Communicating with backend payment gateway ledger...</p>
        </div>

        <div class="card status-card text-center" *ngIf="!loading && booking">
          <div class="status-icon" [class.success]="isVerifiedPaid" [class.partial]="isPartialPaid" [class.failed]="isFailed">
            <i *ngIf="isVerifiedPaid" class="fa-solid fa-circle-check"></i>
            <i *ngIf="isPartialPaid" class="fa-solid fa-clock"></i>
            <i *ngIf="isFailed" class="fa-solid fa-circle-xmark"></i>
          </div>

          <h2 class="mt-3">{{ getStatusTitle() }}</h2>
          <p class="subtitle">{{ getStatusSubtitle() }}</p>

          <div class="booking-details-box my-4">
            <div class="detail-row">
              <span>Booking Reference:</span>
              <strong>{{ booking.bookingReference }}</strong>
            </div>
            <div class="detail-row">
              <span>Payment Gateway:</span>
              <strong>{{ provider || 'Verified Gateway' }}</strong>
            </div>
            <div class="detail-row">
              <span>Total Package Price:</span>
              <strong>\${{ booking.totalAmount | number:'1.2-2' }}</strong>
            </div>
            <div class="detail-row">
              <span>Amount Paid:</span>
              <strong class="text-success">\${{ booking.paidAmount | number:'1.2-2' }}</strong>
            </div>
            <div class="detail-row">
              <span>Outstanding Balance:</span>
              <strong class="text-danger">\${{ booking.balanceAmount | number:'1.2-2' }}</strong>
            </div>
          </div>

          <div class="actions">
            <a [routerLink]="['/portal', slug]" class="btn btn-primary btn-lg">
              <i class="fa-solid fa-house"></i> Return to Portal Home
            </a>
          </div>
        </div>

        <div class="card status-card text-center" *ngIf="!loading && !booking && errorMessage">
          <div class="status-icon failed">
            <i class="fa-solid fa-triangle-exclamation"></i>
          </div>
          <h2>Payment Verification Error</h2>
          <p class="text-danger my-3">{{ errorMessage }}</p>
          <a [routerLink]="['/portal', slug]" class="btn btn-secondary">Back to Portal</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .payment-return-wrapper { background: #F8FAFC; min-height: 100vh; padding: 60px 20px; }
    .container { max-width: 600px; margin: 0 auto; }
    .status-card { padding: 40px; border-radius: 16px; background: white; box-shadow: 0 10px 25px -5px rgba(0,0,0,0.05); }
    .status-icon { font-size: 4rem; margin-bottom: 10px; }
    .status-icon.success { color: #16A34A; }
    .status-icon.partial { color: #D97706; }
    .status-icon.failed { color: #DC2626; }
    .subtitle { color: #64748B; font-size: 1rem; }

    .booking-details-box { background: #F1F5F9; border-radius: 12px; padding: 20px; text-align: left; }
    .detail-row { display: flex; justify-content: space-between; margin-bottom: 10px; font-size: 0.95rem; }
    .detail-row:last-child { margin-bottom: 0; }
    .actions { display: flex; justify-content: center; gap: 12px; }
  `]
})
export class PortalPaymentReturnComponent implements OnInit {
  slug = 'demo-travel';
  bookingId = '';
  provider = '';
  loading = true;
  booking: any = null;
  errorMessage = '';

  get isVerifiedPaid(): boolean {
    return this.booking?.paymentStatus === 3 || this.booking?.balanceAmount === 0;
  }

  get isPartialPaid(): boolean {
    return this.booking?.paymentStatus === 2 || (this.booking?.paidAmount > 0 && this.booking?.balanceAmount > 0);
  }

  get isFailed(): boolean {
    return !this.isVerifiedPaid && !this.isPartialPaid;
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.slug = this.route.snapshot.paramMap.get('organizationSlug') || 'demo-travel';
    
    this.route.queryParams.subscribe((params) => {
      this.bookingId = params['bookingId'] || '';
      this.provider = params['provider'] || '';

      if (!this.bookingId) {
        this.loading = false;
        this.errorMessage = 'No booking ID provided in return parameters.';
        return;
      }

      this.verifyPaymentStatus();
    });
  }

  verifyPaymentStatus(): void {
    this.apiService.getBookingPaymentStatus(this.bookingId).subscribe({
      next: (res) => {
        this.loading = false;
        this.booking = res;
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = 'Unable to fetch verified payment status from backend.';
      }
    });
  }

  getStatusTitle(): string {
    if (this.isVerifiedPaid) return 'Payment Successful!';
    if (this.isPartialPaid) return 'Deposit Received - Seats Reserved!';
    return 'Payment Pending Verification';
  }

  getStatusSubtitle(): string {
    if (this.isVerifiedPaid) return 'Your trip booking is fully paid and confirmed.';
    if (this.isPartialPaid) return 'Your 30% deposit has been recorded. Remaining balance can be paid prior to departure.';
    return 'We are verifying your payment with the payment gateway provider.';
  }
}
