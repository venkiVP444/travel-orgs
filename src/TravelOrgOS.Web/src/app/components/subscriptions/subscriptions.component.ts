import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-subscriptions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="subscriptions-page">
      <div class="page-header">
        <h2>Subscription Quotas & Entitlements</h2>
        <p class="subtitle">Monitor resource limits, track monthly booking checkouts, and upgrade your SaaS tier</p>
      </div>

      <div class="quota-grid mt-4" *ngIf="quota">
        <!-- PLAN SUMMARY CARD -->
        <div class="card summary-card">
          <div class="summary-header">
            <span class="plan-badge">{{ getTierName(quota.tier) }} Plan</span>
            <h3>Operational Usage Dashboard</h3>
          </div>
          <p class="status-text">Your subscription is active. Renewal date: <strong>{{ quota.expiryDate | date:'mediumDate' }}</strong></p>
          <div class="plan-action-row mt-4">
            <span class="small-text">Select Tier to Upgrade:</span>
            <div class="action-btn-group">
              <button (click)="upgrade(2)" class="btn btn-sm btn-outline">Upgrade to Growth</button>
              <button (click)="upgrade(3)" class="btn btn-sm btn-outline">Upgrade to Business</button>
              <button (click)="upgrade(4)" class="btn btn-sm btn-primary">Go Enterprise</button>
            </div>
          </div>
        </div>

        <!-- PROGRESS LIMITS CARDS -->
        <div class="limits-card-group">
          <!-- LIMIT CARD 1: TEAM MEMBERS -->
          <div class="card limit-card">
            <div class="limit-header">
              <span class="icon-box green"><i class="fa-solid fa-users"></i></span>
              <div>
                <h4>Team Members</h4>
                <p class="limit-usage">Usage: <strong>{{ currentTeamCount }} / {{ quota.maxTeamMembers }}</strong></p>
              </div>
            </div>
            <div class="progress-container mt-3">
              <div class="progress-bar green" [style.width.%]="getProgress(currentTeamCount, quota.maxTeamMembers)"></div>
            </div>
          </div>

          <!-- LIMIT CARD 2: ACTIVE TRIPS -->
          <div class="card limit-card">
            <div class="limit-header">
              <span class="icon-box blue"><i class="fa-solid fa-route"></i></span>
              <div>
                <h4>Active Trips</h4>
                <p class="limit-usage">Usage: <strong>{{ currentTripsCount }} / {{ quota.maxActiveTrips }}</strong></p>
              </div>
            </div>
            <div class="progress-container mt-3">
              <div class="progress-bar blue" [style.width.%]="getProgress(currentTripsCount, quota.maxActiveTrips)"></div>
            </div>
          </div>

          <!-- LIMIT CARD 3: MONTHLY BOOKINGS -->
          <div class="card limit-card">
            <div class="limit-header">
              <span class="icon-box orange"><i class="fa-solid fa-receipt"></i></span>
              <div>
                <h4>Monthly Bookings</h4>
                <p class="limit-usage">Usage: <strong>{{ currentBookingsCount }} / {{ quota.maxBookingsPerMonth }}</strong></p>
              </div>
            </div>
            <div class="progress-container mt-3">
              <div class="progress-bar orange" [style.width.%]="getProgress(currentBookingsCount, quota.maxBookingsPerMonth)"></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header { margin-bottom: 24px; }
    .subtitle { color: #64748B; font-size: 0.9rem; margin-top: 4px; }
    .quota-grid { display: flex; flex-direction: column; gap: 20px; max-width: 900px; }
    .summary-card { padding: 24px; border-left: 4px solid #1E88E5; }
    .summary-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
    .plan-badge { background: #E0F2FE; color: #0369A1; font-weight: 700; padding: 6px 12px; border-radius: 20px; font-size: 0.8rem; text-transform: uppercase; }
    .plan-action-row { display: flex; align-items: center; justify-content: space-between; padding-top: 16px; border-top: 1px solid #F1F5F9; }
    .action-btn-group { display: flex; gap: 8px; }
    .small-text { font-size: 0.8rem; color: #64748B; }
    
    .limits-card-group { display: grid; grid-template-columns: repeat(auto-fit, minmax(260px, 1fr)); gap: 16px; }
    .limit-card { padding: 20px; }
    .limit-header { display: flex; align-items: center; gap: 16px; }
    .icon-box { width: 44px; height: 44px; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-size: 1.1rem; }
    .icon-box.green { background: #DCFCE7; color: #15803D; }
    .icon-box.blue { background: #E0F2FE; color: #0369A1; }
    .icon-box.orange { background: #FEF3C7; color: #D97706; }
    .limit-card h4 { font-size: 0.95rem; font-weight: 700; color: #0F172A; }
    .limit-usage { font-size: 0.8rem; color: #64748B; margin-top: 2px; }
    .progress-container { width: 100%; height: 8px; background: #F1F5F9; border-radius: 10px; overflow: hidden; }
    .progress-bar { height: 100%; border-radius: 10px; }
    .progress-bar.green { background: #10B981; }
    .progress-bar.blue { background: #3B82F6; }
    .progress-bar.orange { background: #F59E0B; }
  `]
})
export class SubscriptionsComponent implements OnInit {
  quota: any = null;
  currentTeamCount = 1;
  currentTripsCount = 0;
  currentBookingsCount = 0;

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadQuota();
    this.loadActiveCounters();
  }

  loadQuota(): void {
    this.apiService.getQuota().subscribe(res => this.quota = res);
  }

  loadActiveCounters(): void {
    // Resolve current database record counts to populate usage meters
    this.apiService.getTeamMembers().subscribe(res => this.currentTeamCount = res.length);
    this.apiService.getTrips().subscribe(res => this.currentTripsCount = res.length);
    this.apiService.getBookings().subscribe(res => this.currentBookingsCount = res.length);
  }

  upgrade(tier: number): void {
    this.apiService.initializeQuota(tier).subscribe({
      next: (res) => {
        this.quota = res;
        alert('Plan tier updated successfully! Limits expanded.');
        this.loadActiveCounters();
      },
      error: () => alert('Failed to update subscription.')
    });
  }

  getProgress(curr: number, max: number): number {
    if (!max) return 0;
    return Math.min(100, (curr / max) * 100);
  }

  getTierName(tier: number): string {
    const tiers: { [key: number]: string } = {
      1: 'Starter',
      2: 'Growth',
      3: 'Business',
      4: 'Enterprise'
    };
    return tiers[tier] || 'Starter';
  }
}
