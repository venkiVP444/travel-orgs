import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Trip } from '../../models/models';

@Component({
  selector: 'app-portal-booking',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="portal-booking-wrapper" *ngIf="trip">
      <div class="booking-container">
        <div class="booking-header">
          <a [routerLink]="['/portal', slug, 'trips', trip.id]" class="back-link"><i class="fa-solid fa-arrow-left"></i> Back to Trip Details</a>
          <h2>Complete Your Booking</h2>
          <p class="subtitle">{{ trip.tripName }} ({{ trip.tripCode }})</p>
        </div>

        <div class="booking-grid">
          <!-- BOOKING FORM -->
          <div class="card booking-form-card">
            <form (ngSubmit)="submitBooking()">
              <!-- STEP 1: PASSENGERS COUNT -->
              <div class="form-section">
                <h3><i class="fa-solid fa-users"></i> 1. Number of Passengers</h3>
                <div class="form-group">
                  <label class="form-label">Select Passengers Count</label>
                  <select [(ngModel)]="passengerCount" (change)="onCountChange()" name="passengerCount" class="form-control">
                    <option [ngValue]="1">1 Passenger</option>
                    <option [ngValue]="2">2 Passengers</option>
                    <option [ngValue]="3">3 Passengers</option>
                    <option [ngValue]="4">4 Passengers</option>
                  </select>
                </div>
              </div>

              <!-- STEP 2: CONTACT INFORMATION -->
              <div class="form-section">
                <h3><i class="fa-solid fa-envelope"></i> 2. Primary Contact Details</h3>
                <div class="form-row">
                  <div class="form-group col">
                    <label class="form-label">Contact Email *</label>
                    <input type="email" [(ngModel)]="contactEmail" name="contactEmail" class="form-control" placeholder="john@example.com" required>
                  </div>
                  <div class="form-group col">
                    <label class="form-label">Contact Mobile Number *</label>
                    <input type="text" [(ngModel)]="contactPhone" name="contactPhone" class="form-control" placeholder="+1-555-0199" required>
                  </div>
                </div>
              </div>

              <!-- STEP 3: PASSENGER NAMES -->
              <div class="form-section">
                <h3><i class="fa-solid fa-id-card"></i> 3. Passenger Details</h3>
                <div *ngFor="let p of passengers; let i = index" class="passenger-box">
                  <h4>Passenger {{ i + 1 }}</h4>
                  <div class="form-row">
                    <div class="form-group col">
                      <label class="form-label">First Name *</label>
                      <input type="text" [(ngModel)]="p.firstName" [name]="'fn_' + i" class="form-control" required>
                    </div>
                    <div class="form-group col">
                      <label class="form-label">Last Name *</label>
                      <input type="text" [(ngModel)]="p.lastName" [name]="'ln_' + i" class="form-control" required>
                    </div>
                  </div>
                </div>
              </div>

              <!-- STEP 4: PAYMENT PREFERENCE & GATEWAY -->
              <div class="form-section">
                <h3><i class="fa-solid fa-credit-card"></i> 4. Payment Plan & Gateway</h3>

                <label class="form-label font-weight-bold">Select Payment Schedule:</label>
                <div class="payment-options mb-3">
                  <label class="payment-card" [class.selected]="paymentType === 'Full'">
                    <input type="radio" [(ngModel)]="paymentType" name="paymentType" value="Full">
                    <div class="option-info">
                      <strong>Pay Full Amount Now</strong>
                      <p>Instant booking confirmation (\${{ calculateTotal() | number:'1.2-2' }})</p>
                    </div>
                  </label>

                  <label class="payment-card" [class.selected]="paymentType === 'Deposit'">
                    <input type="radio" [(ngModel)]="paymentType" name="paymentType" value="Deposit">
                    <div class="option-info">
                      <strong>Pay 30% Deposit Now</strong>
                      <p>Pay \${{ (calculateTotal() * 0.3) | number:'1.2-2' }} deposit to hold seats</p>
                    </div>
                  </label>

                  <label class="payment-card" [class.selected]="paymentType === 'PayLater'">
                    <input type="radio" [(ngModel)]="paymentType" name="paymentType" value="PayLater">
                    <div class="option-info">
                      <strong>Reserve & Pay Later</strong>
                      <p>Pay via bank transfer or invoice prior to trip departure</p>
                    </div>
                  </label>
                </div>

                <div *ngIf="paymentType !== 'PayLater'" class="gateway-selection mt-3">
                  <label class="form-label font-weight-bold">Select Payment Method / Gateway:</label>
                  <div class="gateway-options">
                    <label class="gateway-card" [class.selected]="paymentProvider === 'Stripe'">
                      <input type="radio" [(ngModel)]="paymentProvider" name="paymentProvider" value="Stripe">
                      <div class="gateway-info">
                        <i class="fa-brands fa-stripe text-primary fa-lg"></i>
                        <strong>Stripe (Credit / Debit Card)</strong>
                      </div>
                    </label>

                    <label class="gateway-card" [class.selected]="paymentProvider === 'Razorpay'">
                      <input type="radio" [(ngModel)]="paymentProvider" name="paymentProvider" value="Razorpay">
                      <div class="gateway-info">
                        <i class="fa-solid fa-bolt text-warning fa-lg"></i>
                        <strong>Razorpay (UPI / NetBanking / Cards)</strong>
                      </div>
                    </label>

                    <label class="gateway-card" [class.selected]="paymentProvider === 'Mock'">
                      <input type="radio" [(ngModel)]="paymentProvider" name="paymentProvider" value="Mock">
                      <div class="gateway-info">
                        <i class="fa-solid fa-flask text-success fa-lg"></i>
                        <strong>Instant Mock Payment (Demo Mode)</strong>
                      </div>
                    </label>
                  </div>
                </div>
              </div>

              <div *ngIf="errorMessage" class="error-banner mt-3">
                <i class="fa-solid fa-triangle-exclamation"></i> {{ errorMessage }}
              </div>

              <button type="submit" [disabled]="submitting" class="btn btn-primary btn-lg btn-block mt-4">
                <i *ngIf="submitting" class="fa-solid fa-spinner fa-spin"></i>
                <span *ngIf="!submitting">Confirm & Process Booking</span>
              </button>
            </form>
          </div>

          <!-- SUMMARY CARD -->
          <div class="summary-card card">
            <h3>Trip Order Summary</h3>
            <div class="summary-row">
              <span>Package Price:</span>
              <strong>\${{ trip.basePrice }} x {{ passengerCount }}</strong>
            </div>
            <div class="summary-row total-row">
              <span>Total Price:</span>
              <strong>\${{ calculateTotal() | number:'1.2-2' }}</strong>
            </div>

            <div class="summary-seats">
              <i class="fa-solid fa-circle-check text-success"></i> Instant Seat Reservation Guaranteed
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .portal-booking-wrapper { background: #F8FAFC; min-height: 100vh; padding: 40px 20px; }
    .booking-container { max-width: 960px; margin: 0 auto; }
    .booking-header { margin-bottom: 24px; }
    .back-link { color: #1E88E5; font-weight: 600; font-size: 0.9rem; }
    .subtitle { color: #64748B; font-size: 1rem; }

    .booking-grid { display: grid; grid-template-columns: 1fr 320px; gap: 24px; }
    @media (max-width: 768px) { .booking-grid { grid-template-columns: 1fr; } }

    .booking-form-card { padding: 32px; }
    .form-section { margin-bottom: 28px; padding-bottom: 20px; border-bottom: 1px solid #E2E8F0; }
    .form-section h3 { font-size: 1.1rem; color: #0F172A; margin-bottom: 16px; display: flex; align-items: center; gap: 8px; }
    .form-row { display: flex; gap: 16px; }
    .col { flex: 1; }

    .passenger-box { background: #F8FAFC; padding: 16px; border-radius: 10px; border: 1px solid #E2E8F0; margin-bottom: 12px; }
    .passenger-box h4 { font-size: 0.95rem; color: #334155; margin-bottom: 10px; }

    .payment-options, .gateway-options { display: flex; flex-direction: column; gap: 12px; }
    .payment-card, .gateway-card { display: flex; align-items: center; gap: 12px; padding: 14px 16px; border: 2px solid #E2E8F0; border-radius: 12px; cursor: pointer; transition: all 0.2s ease; }
    .payment-card.selected, .gateway-card.selected { border-color: #1E88E5; background: #E3F2FD; }
    .gateway-info { display: flex; align-items: center; gap: 10px; font-size: 0.95rem; }

    .summary-card { padding: 24px; position: sticky; top: 30px; height: max-content; }
    .summary-row { display: flex; justify-content: space-between; margin-bottom: 12px; font-size: 0.95rem; }
    .total-row { border-top: 2px solid #E2E8F0; padding-top: 12px; font-size: 1.2rem; }
    .summary-seats { font-size: 0.85rem; color: #16A34A; margin-top: 16px; font-weight: 600; }
    .btn-block { width: 100%; text-align: center; }
    .error-banner { background: #FEE2E2; color: #B91C1C; padding: 10px; border-radius: 8px; font-size: 0.85rem; }
  `]
})
export class PortalBookingComponent implements OnInit {
  slug = '';
  tripId = '';
  trip: Trip | null = null;

  passengerCount = 1;
  contactEmail = '';
  contactPhone = '';
  paymentType = 'Full';
  paymentProvider = 'Mock';
  passengers: any[] = [];
  submitting = false;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.slug = this.route.snapshot.paramMap.get('organizationSlug') || 'demo-travel';
    this.tripId = this.route.snapshot.paramMap.get('tripId') || '';

    this.apiService.getPortalTrip(this.slug, this.tripId).subscribe({
      next: (t) => {
        this.trip = t;
        this.onCountChange();
      }
    });
  }

  onCountChange(): void {
    this.passengers = [];
    for (let i = 0; i < this.passengerCount; i++) {
      this.passengers.push({
        firstName: i === 0 ? 'David' : 'Passenger',
        lastName: i === 0 ? 'Miller' : `${i + 1}`,
        email: this.contactEmail || 'david.miller@example.com',
        mobileNumber: this.contactPhone || '+1-555-0199'
      });
    }
  }

  calculateTotal(): number {
    return (this.trip?.basePrice || 0) * this.passengerCount;
  }

  submitBooking(): void {
    if (!this.contactEmail || !this.contactPhone) {
      this.errorMessage = 'Please enter contact email and mobile number.';
      return;
    }

    this.submitting = true;
    this.errorMessage = '';

    const amountToPay = this.paymentType === 'Deposit' ? (this.calculateTotal() * 0.3) : this.calculateTotal();

    const payload = {
      tripId: this.tripId,
      numberOfTravellers: this.passengerCount,
      contactEmail: this.contactEmail,
      contactPhone: this.contactPhone,
      paymentType: this.paymentType,
      amountToPay: amountToPay,
      travellers: this.passengers
    };

    this.apiService.createPortalBooking(this.slug, payload).subscribe({
      next: (booking) => {
        if (this.paymentType === 'PayLater') {
          this.submitting = false;
          this.router.navigate(['/portal', this.slug, 'booking-success', booking.bookingReference]);
          return;
        }

        // Initiate Gateway Session
        this.apiService.initiatePortalPayment(this.slug, booking.id, this.paymentProvider, this.paymentType, amountToPay).subscribe({
          next: (session) => {
            this.submitting = false;
            if (session.checkoutUrl && this.paymentProvider !== 'Mock') {
              window.location.href = session.checkoutUrl;
            } else {
              // Direct navigation to return status page for mock gateway or instant verification
              this.router.navigate(['/portal', this.slug, 'payment-return'], {
                queryParams: { bookingId: booking.id, status: 'success', txnRef: session.transactionReference, provider: this.paymentProvider }
              });
            }
          },
          error: (err) => {
            this.submitting = false;
            this.router.navigate(['/portal', this.slug, 'booking-success', booking.bookingReference]);
          }
        });
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = err.error?.message || 'Failed to process booking. Please try again.';
      }
    });
  }
}
