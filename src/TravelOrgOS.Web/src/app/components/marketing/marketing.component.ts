import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-marketing',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="marketing-page">
      <div class="page-header">
        <div class="header-left">
          <h2>Marketing & Campaigns</h2>
          <p class="subtitle">Broadcast updates, promo templates, and newsletter announcements to travellers</p>
        </div>
        <button (click)="openCreateDrawer()" class="btn btn-primary">
          <i class="fa-solid fa-bullhorn"></i> Start Campaign
        </button>
      </div>

      <div class="card table-card mt-4">
        <table class="table">
          <thead>
            <tr>
              <th>Campaign Details</th>
              <th>Channel</th>
              <th>Audience Segment</th>
              <th>Status</th>
              <th>Date Sent</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let c of campaigns">
              <td>
                <div class="campaign-title">
                  <span class="name">{{ c.name }}</span>
                  <span class="subject">{{ c.subject }}</span>
                </div>
              </td>
              <td>
                <span class="channel-badge" [class.email]="c.type === 1" [class.sms]="c.type === 2">
                  <i class="fa-solid" [class.fa-envelope]="c.type === 1" [class.fa-comment-sms]="c.type === 2"></i>
                  {{ c.type === 1 ? 'Email' : 'SMS' }}
                </span>
              </td>
              <td>
                <span class="badge badge-secondary">{{ c.targetSegmentQuery }}</span>
              </td>
              <td>
                <span class="status-pill" 
                  [class.draft]="c.status === 1" 
                  [class.sending]="c.status === 3" 
                  [class.sent]="c.status === 4">
                  {{ getStatusText(c.status) }}
                </span>
              </td>
              <td>{{ c.sentAt ? (c.sentAt | date:'medium') : 'Not Sent' }}</td>
              <td>
                <button *ngIf="c.status === 1" (click)="sendCampaign(c)" class="btn btn-sm btn-outline">
                  <i class="fa-solid fa-paper-plane"></i> Send Now
                </button>
              </td>
            </tr>
            <tr *ngIf="campaigns.length === 0">
              <td colspan="6" class="empty-state">
                <i class="fa-regular fa-paper-plane"></i>
                <p>No marketing campaigns launched yet. Start a campaign to engage your customer base.</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- DRAWER FOR CREATING CAMPAIGN -->
      <div class="drawer-overlay" [class.open]="drawerOpen" (click)="closeDrawer()">
        <div class="drawer" (click)="$event.stopPropagation()">
          <div class="drawer-header">
            <h3>Start Marketing Campaign</h3>
            <button (click)="closeDrawer()" class="btn-close"><i class="fa-solid fa-xmark"></i></button>
          </div>
          
          <div class="drawer-body">
            <form (ngSubmit)="createCampaign()">
              <div class="form-group">
                <label class="form-label">Campaign Name *</label>
                <input type="text" [(ngModel)]="form.name" name="name" class="form-control" required placeholder="E.g. Diwali Weekend Special Promo">
              </div>

              <div class="form-group">
                <label class="form-label">Channel *</label>
                <select [(ngModel)]="form.type" name="type" class="form-control" required>
                  <option [value]="1">Email Newsletter</option>
                  <option [value]="2">SMS Broadcast</option>
                </select>
              </div>

              <div class="form-group">
                <label class="form-label">Audience Segment Query *</label>
                <select [(ngModel)]="form.targetSegmentQuery" name="targetSegmentQuery" class="form-control" required>
                  <option value="all">All Registered Customers</option>
                  <option value="past-travellers">Past Travellers (with Confirmed/Completed Bookings)</option>
                  <option value="inactive-customers">Inactive Customers (No Bookings)</option>
                </select>
              </div>

              <div class="form-group">
                <label class="form-label">Message Subject *</label>
                <input type="text" [(ngModel)]="form.subject" name="subject" class="form-control" required placeholder="E.g. Exclusive 15% off on Coorg packages!">
              </div>

              <div class="form-group">
                <label class="form-label">Body Content / Template *</label>
                <textarea [(ngModel)]="form.bodyTemplate" name="bodyTemplate" class="form-control" rows="8" required placeholder="Dear Traveller, We have an exclusive pack for you..."></textarea>
              </div>

              <div class="drawer-actions">
                <button type="button" (click)="closeDrawer()" class="btn btn-outline">Cancel</button>
                <button type="submit" class="btn btn-primary">Save Campaign Draft</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    .subtitle { color: #64748B; font-size: 0.9rem; margin-top: 4px; }
    .table-card { border-radius: 12px; overflow: hidden; }
    .campaign-title { display: flex; flex-direction: column; }
    .campaign-title .name { font-weight: 700; color: #0F172A; }
    .campaign-title .subject { font-size: 0.8rem; color: #64748B; margin-top: 2px; }
    .channel-badge { display: inline-flex; align-items: center; gap: 6px; padding: 4px 8px; border-radius: 6px; font-size: 0.75rem; font-weight: 700; }
    .channel-badge.email { background: #EEF2F6; color: #3B82F6; }
    .channel-badge.sms { background: #FEF3C7; color: #D97706; }
    .badge-secondary { background: #E2E8F0; color: #475569; padding: 4px 8px; border-radius: 6px; font-size: 0.75rem; }
    .status-pill { padding: 4px 8px; border-radius: 12px; font-size: 0.75rem; font-weight: 700; display: inline-block; }
    .status-pill.draft { background: #F1F5F9; color: #475569; }
    .status-pill.sending { background: #EFF6FF; color: #1D4ED8; }
    .status-pill.sent { background: #DCFCE7; color: #15803D; }
    .empty-state { text-align: center; padding: 48px; color: #94A3B8; }
    .empty-state i { font-size: 2.5rem; margin-bottom: 12px; color: #CBD5E1; }
    .drawer-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(15, 23, 42, 0.4); opacity: 0; pointer-events: none; transition: opacity 0.3s ease; z-index: 1000; }
    .drawer-overlay.open { opacity: 1; pointer-events: auto; }
    .drawer { position: fixed; top: 0; right: -400px; width: 450px; height: 100vh; background: white; box-shadow: -4px 0 20px rgba(0, 0, 0, 0.1); transition: right 0.3s cubic-bezier(0.4, 0, 0.2, 1); display: flex; flex-direction: column; }
    .drawer-overlay.open .drawer { right: 0; }
    .drawer-header { display: flex; justify-content: space-between; align-items: center; padding: 20px; border-bottom: 1px solid #E2E8F0; }
    .btn-close { background: transparent; border: none; font-size: 1.2rem; cursor: pointer; color: #64748B; }
    .drawer-body { padding: 20px; overflow-y: auto; flex-grow: 1; }
    .drawer-actions { display: flex; justify-content: flex-end; gap: 10px; margin-top: 24px; padding-top: 16px; border-top: 1px solid #E2E8F0; }
  `]
})
export class MarketingComponent implements OnInit {
  campaigns: any[] = [];
  drawerOpen = false;

  form = {
    name: '',
    type: 1,
    subject: '',
    bodyTemplate: '',
    targetSegmentQuery: 'all',
    scheduledFor: null
  };

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadCampaigns();
  }

  loadCampaigns(): void {
    this.apiService.getCampaigns().subscribe(res => this.campaigns = res);
  }

  openCreateDrawer(): void {
    this.form = {
      name: '',
      type: 1,
      subject: '',
      bodyTemplate: '',
      targetSegmentQuery: 'all',
      scheduledFor: null
    };
    this.drawerOpen = true;
  }

  closeDrawer(): void {
    this.drawerOpen = false;
  }

  createCampaign(): void {
    this.apiService.createCampaign(this.form).subscribe(() => {
      this.loadCampaigns();
      this.closeDrawer();
    });
  }

  sendCampaign(c: any): void {
    this.apiService.sendCampaign(c.id).subscribe({
      next: () => {
        this.loadCampaigns();
        alert('Campaign dispatched successfully to target audience segment.');
      },
      error: (err) => alert(err.error || 'Failed to send campaign.')
    });
  }

  getStatusText(status: number): string {
    const statuses: { [key: number]: string } = {
      1: 'Draft',
      2: 'Scheduled',
      3: 'Sending',
      4: 'Sent',
      5: 'Cancelled',
      6: 'Failed'
    };
    return statuses[status] || 'Unknown';
  }
}
