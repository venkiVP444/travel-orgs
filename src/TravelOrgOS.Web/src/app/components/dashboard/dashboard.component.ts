import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { DashboardSummary } from '../../models/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="dashboard-page">
      <div class="page-header">
        <div>
          <h2>Executive Operations Dashboard</h2>
          <p class="subtitle">Real-time overview of trips, travellers, bookings, and revenue</p>
        </div>
        <div class="header-actions">
          <a routerLink="/trips/new" class="btn btn-primary">
            <i class="fa-solid fa-plus"></i> Create Trip
          </a>
          <a routerLink="/travellers" class="btn btn-outline">
            <i class="fa-solid fa-file-import"></i> Import Travellers
          </a>
        </div>
      </div>

      <div *ngIf="loading" class="loading-state">
        <i class="fa-solid fa-spinner fa-spin"></i> Loading Dashboard Metrics...
      </div>

      <div *ngIf="!loading && summary">
        <!-- KPI METRICS GRID -->
        <div class="kpi-grid">
          <div class="kpi-card">
            <div class="kpi-icon icon-blue"><i class="fa-solid fa-route"></i></div>
            <div class="kpi-info">
              <span class="kpi-title">Active Trips</span>
              <p class="kpi-value">{{ summary.activeTrips }} / {{ summary.totalTrips }}</p>
            </div>
          </div>

          <div class="kpi-card">
            <div class="kpi-icon icon-green"><i class="fa-solid fa-users"></i></div>
            <div class="kpi-info">
              <span class="kpi-title">Total Travellers</span>
              <p class="kpi-value">{{ summary.totalTravellers }}</p>
            </div>
          </div>

          <div class="kpi-card">
            <div class="kpi-icon icon-purple"><i class="fa-solid fa-ticket"></i></div>
            <div class="kpi-info">
              <span class="kpi-title">Confirmed Bookings</span>
              <p class="kpi-value">{{ summary.confirmedBookings }}</p>
            </div>
          </div>

          <div class="kpi-card">
            <div class="kpi-icon icon-orange"><i class="fa-solid fa-sack-dollar"></i></div>
            <div class="kpi-info">
              <span class="kpi-title">Total Revenue</span>
              <p class="kpi-value">\${{ summary.totalRevenue | number:'1.2-2' }}</p>
            </div>
          </div>
        </div>

        <!-- REVENUE & BALANCES BANNER -->
        <div class="finance-banner">
          <div class="finance-item">
            <span>Outstanding Customer Balance:</span>
            <strong>\${{ summary.outstandingBalance | number:'1.2-2' }}</strong>
          </div>
          <div class="finance-item">
            <span>Pending Payment Invoices:</span>
            <strong>\${{ summary.pendingPayments | number:'1.2-2' }}</strong>
          </div>
        </div>

        <!-- TWO COLUMN LAYOUT -->
        <div class="dashboard-columns">
          <!-- UPCOMING TRIPS -->
          <div class="card col-card">
            <div class="card-header">
              <h3><i class="fa-solid fa-plane"></i> Upcoming Scheduled Trips</h3>
              <a routerLink="/trips" class="link-sm">View All Trips</a>
            </div>

            <div class="table-container">
              <table class="tos-table">
                <thead>
                  <tr>
                    <th>Trip Code</th>
                    <th>Trip Name</th>
                    <th>Start Date</th>
                    <th>Capacity</th>
                    <th>Seats Available</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  <tr *ngFor="let t of summary.upcomingTrips">
                    <td><strong>{{ t.tripCode }}</strong></td>
                    <td>{{ t.tripName }}</td>
                    <td>{{ t.startDate | date:'mediumDate' }}</td>
                    <td>{{ t.totalCapacity }}</td>
                    <td>
                      <span [class.text-danger]="t.availableSeats <= 3" class="seat-badge">
                        {{ t.availableSeats }} left
                      </span>
                    </td>
                    <td>
                      <span class="badge badge-success">Active</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- RECENT BOOKINGS -->
          <div class="card col-card">
            <div class="card-header">
              <h3><i class="fa-solid fa-receipt"></i> Recent Bookings</h3>
              <a routerLink="/bookings" class="link-sm">View All Bookings</a>
            </div>

            <div class="table-container">
              <table class="tos-table">
                <thead>
                  <tr>
                    <th>Ref</th>
                    <th>Customer</th>
                    <th>Trip</th>
                    <th>Amount</th>
                    <th>Payment</th>
                  </tr>
                </thead>
                <tbody>
                  <tr *ngFor="let b of summary.recentBookings">
                    <td><strong>{{ b.bookingReference }}</strong></td>
                    <td>{{ b.customerName }}</td>
                    <td>{{ b.tripName }}</td>
                    <td>\${{ b.totalAmount | number:'1.2-2' }}</td>
                    <td>
                      <span class="badge" [ngClass]="{
                        'badge-success': b.paymentStatus === 3,
                        'badge-warning': b.paymentStatus === 2,
                        'badge-danger': b.paymentStatus === 1
                      }">
                        {{ getPaymentStatusLabel(b.paymentStatus) }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
    }
    .page-header h2 { font-size: 1.6rem; color: #0F172A; }
    .subtitle { color: #64748B; font-size: 0.9rem; }
    .header-actions { display: flex; gap: 12px; }

    .kpi-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 20px;
      margin-bottom: 24px;
    }
    .kpi-card {
      background: white;
      border-radius: 16px;
      padding: 20px;
      display: flex;
      align-items: center;
      gap: 16px;
      border: 1px solid #E2E8F0;
      box-shadow: 0 2px 4px rgba(0,0,0,0.04);
    }
    .kpi-icon {
      width: 52px;
      height: 52px;
      border-radius: 14px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.4rem;
    }
    .icon-blue { background: #E3F2FD; color: #1E88E5; }
    .icon-green { background: #DCFCE7; color: #16A34A; }
    .icon-purple { background: #F3E8FF; color: #9333EA; }
    .icon-orange { background: #FFEDD5; color: #EA580C; }

    .kpi-title { font-size: 0.8rem; font-weight: 600; color: #64748B; text-transform: uppercase; }
    .kpi-value { font-size: 1.5rem; font-weight: 800; color: #0F172A; }

    .finance-banner {
      background: linear-gradient(135deg, #0F172A, #1E293B);
      color: white;
      padding: 16px 24px;
      border-radius: 14px;
      display: flex;
      justify-content: space-around;
      margin-bottom: 28px;
    }
    .finance-item { display: flex; align-items: center; gap: 8px; font-size: 0.95rem; }
    .finance-item strong { color: #38BDF8; font-size: 1.1rem; }

    .dashboard-columns {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 24px;
    }
    .col-card { padding: 20px; }
    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }
    .card-header h3 { font-size: 1.1rem; color: #0F172A; display: flex; align-items: center; gap: 8px; }
    .link-sm { font-size: 0.85rem; font-weight: 600; }
    .seat-badge { font-weight: 700; }
    .text-danger { color: #DC2626; }
    .loading-state { text-align: center; padding: 40px; font-size: 1.1rem; color: #64748B; }
  `]
})
export class DashboardComponent implements OnInit {
  summary: DashboardSummary | null = null;
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.apiService.getDashboardSummary().subscribe({
      next: (data) => {
        this.summary = data;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  getPaymentStatusLabel(status: number): string {
    const map: { [key: number]: string } = { 1: 'Pending', 2: 'Partial', 3: 'Paid', 4: 'Refunded' };
    return map[status] || 'Unknown';
  }
}
