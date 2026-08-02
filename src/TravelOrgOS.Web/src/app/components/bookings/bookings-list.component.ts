import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Booking } from '../../models/models';

@Component({
  selector: 'app-bookings-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="bookings-page">
      <div class="page-header">
        <div>
          <h2>Booking & Payment Ledger</h2>
          <p class="subtitle">Track customer reservations, payments, balances, and passenger manifests</p>
        </div>
      </div>

      <!-- SEARCH BAR -->
      <div class="filter-card card">
        <div class="search-box">
          <i class="fa-solid fa-magnifying-glass search-icon"></i>
          <input type="text" [(ngModel)]="searchQuery" (input)="loadBookings()" class="form-control search-input" placeholder="Search by booking reference, trip name, or customer email...">
        </div>
      </div>

      <!-- BOOKINGS TABLE -->
      <div class="card table-card">
        <div class="table-container">
          <table class="tos-table">
            <thead>
              <tr>
                <th>Booking Ref</th>
                <th>Trip Package</th>
                <th>Contact Email</th>
                <th>Passengers</th>
                <th>Total Price</th>
                <th>Paid Amount</th>
                <th>Balance</th>
                <th>Payment Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let b of bookings">
                <td><strong>{{ b.bookingReference }}</strong></td>
                <td>{{ b.tripName }}</td>
                <td>{{ b.contactEmail }}</td>
                <td>{{ b.numberOfTravellers }} Pass</td>
                <td>\${{ b.totalAmount | number:'1.2-2' }}</td>
                <td class="text-success">\${{ b.paidAmount | number:'1.2-2' }}</td>
                <td [class.text-danger]="b.balanceAmount > 0">\${{ b.balanceAmount | number:'1.2-2' }}</td>
                <td>
                  <span class="badge" [ngClass]="{
                    'badge-success': b.paymentStatus === 3,
                    'badge-warning': b.paymentStatus === 2,
                    'badge-danger': b.paymentStatus === 1
                  }">
                    {{ getPaymentStatusText(b.paymentStatus) }}
                  </span>
                </td>
                <td>
                  <button *ngIf="b.balanceAmount > 0" (click)="openPaymentModal(b)" class="btn btn-sm btn-outline">
                    <i class="fa-solid fa-credit-card"></i> Pay
                  </button>
                  <button *ngIf="b.bookingStatus !== 3" (click)="cancelBooking(b.id)" class="btn btn-sm btn-danger">
                    <i class="fa-solid fa-ban"></i> Cancel
                  </button>
                </td>
              </tr>
              <tr *ngIf="bookings.length === 0">
                <td colspan="9" class="empty-cell">No bookings found.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- RECORD PAYMENT MODAL -->
      <div *ngIf="showPaymentModal" class="modal-overlay">
        <div class="modal-card">
          <div class="modal-header">
            <h3>Record Payment for {{ selectedBooking?.bookingReference }}</h3>
            <button (click)="showPaymentModal = false" class="close-btn">&times;</button>
          </div>
          <div class="modal-body">
            <p><strong>Total Amount:</strong> \${{ selectedBooking?.totalAmount }} | <strong>Remaining Balance:</strong> \${{ selectedBooking?.balanceAmount }}</p>

            <form (ngSubmit)="submitPayment()">
              <div class="form-group mt-3">
                <label class="form-label">Payment Amount ($) *</label>
                <input type="number" [(ngModel)]="paymentForm.amount" name="amount" class="form-control" required>
              </div>

              <div class="form-group">
                <label class="form-label">Payment Method *</label>
                <select [(ngModel)]="paymentForm.paymentMethod" name="paymentMethod" class="form-control">
                  <option value="Credit Card">Credit Card</option>
                  <option value="Bank Transfer">Bank Transfer</option>
                  <option value="Cash / Manual">Cash / Manual</option>
                </select>
              </div>

              <div class="modal-footer">
                <button type="button" (click)="showPaymentModal = false" class="btn btn-secondary">Cancel</button>
                <button type="submit" class="btn btn-primary">Record Payment</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header { margin-bottom: 24px; }
    .filter-card { margin-bottom: 20px; padding: 16px; }
    .search-box { position: relative; display: flex; align-items: center; }
    .search-icon { position: absolute; left: 16px; color: #94A3B8; }
    .search-input { padding-left: 44px; }
    .text-success { color: #16A34A; font-weight: 700; }
    .text-danger { color: #DC2626; font-weight: 700; }
    .empty-cell { text-align: center; padding: 40px; color: #64748B; }
    .close-btn { background: none; border: none; font-size: 1.5rem; cursor: pointer; color: #64748B; }
  `]
})
export class BookingsListComponent implements OnInit {
  bookings: Booking[] = [];
  searchQuery = '';
  showPaymentModal = false;
  selectedBooking: Booking | null = null;
  paymentForm = { amount: 0, paymentMethod: 'Credit Card' };

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.apiService.getBookings(this.searchQuery).subscribe({
      next: (data) => this.bookings = data
    });
  }

  cancelBooking(id: string): void {
    if (confirm('Cancel this booking? Available trip seats will be restored automatically.')) {
      this.apiService.cancelBooking(id).subscribe({
        next: () => this.loadBookings()
      });
    }
  }

  openPaymentModal(booking: Booking): void {
    this.selectedBooking = booking;
    this.paymentForm.amount = booking.balanceAmount;
    this.showPaymentModal = true;
  }

  submitPayment(): void {
    if (!this.selectedBooking) return;
    this.apiService.recordPayment(this.selectedBooking.id, this.paymentForm).subscribe({
      next: () => {
        this.showPaymentModal = false;
        alert('Payment recorded successfully!');
        this.loadBookings();
      }
    });
  }

  getPaymentStatusText(status: number): string {
    const map: { [key: number]: string } = { 1: 'Pending', 2: 'Partially Paid', 3: 'Paid', 4: 'Refunded' };
    return map[status] || 'Pending';
  }
}
