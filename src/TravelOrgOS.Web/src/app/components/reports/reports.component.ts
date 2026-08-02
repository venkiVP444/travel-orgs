import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="reports-page">
      <div class="page-header">
        <h2>Export Operational & Financial Reports</h2>
        <p class="subtitle">Download real-time CSV exports for bookings, revenue, travellers, and outstanding balances</p>
      </div>

      <div class="reports-grid">
        <div class="card report-card">
          <div class="report-icon icon-blue"><i class="fa-solid fa-receipt"></i></div>
          <h3>Bookings Master Report</h3>
          <p>Export all passenger booking records, reference codes, contact info, and payment statuses.</p>
          <a href="http://localhost:5100/api/reports/bookings/export" target="_blank" class="btn btn-primary btn-block">
            <i class="fa-solid fa-download"></i> Export Bookings CSV
          </a>
        </div>

        <div class="card report-card">
          <div class="report-icon icon-green"><i class="fa-solid fa-users"></i></div>
          <h3>Travellers Directory Export</h3>
          <p>Export full customer list with passports, emergency contacts, nationalities, and dates of birth.</p>
          <a href="http://localhost:5100/api/reports/travellers/export" target="_blank" class="btn btn-primary btn-block">
            <i class="fa-solid fa-download"></i> Export Travellers CSV
          </a>
        </div>

        <div class="card report-card">
          <div class="report-icon icon-purple"><i class="fa-solid fa-sack-dollar"></i></div>
          <h3>Revenue & Payments Ledger</h3>
          <p>Export all transaction logs, credit card payments, deposit receipts, and transaction references.</p>
          <a href="http://localhost:5100/api/reports/revenue/export" target="_blank" class="btn btn-primary btn-block">
            <i class="fa-solid fa-download"></i> Export Revenue CSV
          </a>
        </div>

        <div class="card report-card">
          <div class="report-icon icon-orange"><i class="fa-solid fa-wallet"></i></div>
          <h3>Outstanding Balances Report</h3>
          <p>Export list of customers with pending balances for automated follow-up.</p>
          <a href="http://localhost:5100/api/reports/outstanding/export" target="_blank" class="btn btn-primary btn-block">
            <i class="fa-solid fa-download"></i> Export Balances CSV
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header { margin-bottom: 28px; }
    .reports-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(260px, 1fr)); gap: 24px; }
    .report-card { text-align: center; padding: 28px; display: flex; flex-direction: column; align-items: center; }
    .report-icon { width: 60px; height: 60px; border-radius: 16px; display: flex; align-items: center; justify-content: center; font-size: 1.6rem; margin-bottom: 16px; }
    .icon-blue { background: #E3F2FD; color: #1E88E5; }
    .icon-green { background: #DCFCE7; color: #16A34A; }
    .icon-purple { background: #F3E8FF; color: #9333EA; }
    .icon-orange { background: #FFEDD5; color: #EA580C; }
    .report-card h3 { font-size: 1.2rem; color: #0F172A; margin-bottom: 8px; }
    .report-card p { font-size: 0.88rem; color: #64748B; margin-bottom: 24px; flex-grow: 1; }
    .btn-block { width: 100%; }
  `]
})
export class ReportsComponent {}
