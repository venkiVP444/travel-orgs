import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="app-container">
      <!-- SIDEBAR -->
      <aside class="sidebar">
        <div class="sidebar-brand">
          <div class="logo-box">
            <i class="fa-solid fa-plane-departure"></i>
          </div>
          <div class="brand-text">
            <h2>TravelOrgOS</h2>
            <span class="version">Enterprise SaaS</span>
          </div>
        </div>

        <div class="org-card" *ngIf="session">
          <img [src]="session.logoUrl || 'https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=100&fit=crop'" class="org-logo">
          <div class="org-info">
            <p class="org-name">{{ session.organizationName || 'Travel Agency' }}</p>
            <span class="org-slug">/portal/{{ session.organizationSlug || 'demo-travel' }}</span>
          </div>
        </div>

        <nav class="nav-menu">
          <a routerLink="/dashboard" routerLinkActive="active" class="nav-item">
            <i class="fa-solid fa-chart-line"></i> Dashboard
          </a>
          <a routerLink="/trips" routerLinkActive="active" class="nav-item">
            <i class="fa-solid fa-route"></i> Trip Management
          </a>
          <a routerLink="/travellers" routerLinkActive="active" class="nav-item">
            <i class="fa-solid fa-users"></i> Travellers
          </a>
          <a routerLink="/guides" routerLinkActive="active" class="nav-item">
            <i class="fa-solid fa-user-tie"></i> Guides
          </a>
          <a routerLink="/bookings" routerLinkActive="active" class="nav-item">
            <i class="fa-solid fa-receipt"></i> Bookings
          </a>
          <a routerLink="/team" routerLinkActive="active" class="nav-item">
            <i class="fa-solid fa-users-gear"></i> Team Management
          </a>
          <a routerLink="/marketing" routerLinkActive="active" class="nav-item">
            <i class="fa-solid fa-bullhorn"></i> Marketing Campaigns
          </a>
          <a routerLink="/subscriptions" routerLinkActive="active" class="nav-item">
            <i class="fa-solid fa-credit-card"></i> Subscriptions
          </a>
          <a routerLink="/reports" routerLinkActive="active" class="nav-item">
            <i class="fa-solid fa-file-export"></i> Reports
          </a>
          <a routerLink="/settings" routerLinkActive="active" class="nav-item">
            <i class="fa-solid fa-sliders"></i> Branding & Settings
          </a>
        </nav>

        <div class="sidebar-portal-cta" *ngIf="session?.organizationSlug">
          <a [href]="'/portal/' + session?.organizationSlug" target="_blank" class="btn btn-primary btn-sm btn-block">
            <i class="fa-solid fa-arrow-up-right-from-square"></i> Open Branded Portal
          </a>
        </div>

        <div class="user-profile" *ngIf="session">
          <div class="user-avatar">
            {{ session.fullName.charAt(0) }}
          </div>
          <div class="user-details">
            <p class="user-name">{{ session.fullName }}</p>
            <span class="user-role">{{ getRoleName(session.role) }}</span>
          </div>
          <button (click)="logout()" class="logout-btn" title="Logout">
            <i class="fa-solid fa-right-from-bracket"></i>
          </button>
        </div>
      </aside>

      <!-- MAIN CONTENT -->
      <main class="main-content">
        <header class="topbar">
          <div class="topbar-left">
            <span class="breadcrumb-app">TravelOrgOS</span> / <span class="page-title">Operations Console</span>
          </div>

          <div class="topbar-actions">
            <button (click)="resetDemo()" class="btn btn-outline btn-sm" title="Reset Demo Data">
              <i class="fa-solid fa-rotate-left"></i> Reset Demo Data
            </button>

            <a [href]="'/portal/' + (session?.organizationSlug || 'demo-travel')" target="_blank" class="btn btn-outline btn-sm">
              <i class="fa-solid fa-mobile-screen"></i> Mobile Portal
            </a>
          </div>
        </header>

        <div class="content-body">
          <router-outlet></router-outlet>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .app-container {
      display: flex;
      min-height: 100vh;
    }
    .sidebar {
      width: 260px;
      background: #0F172A;
      color: #F8FAFC;
      display: flex;
      flex-direction: column;
      padding: 20px;
      flex-shrink: 0;
    }
    .sidebar-brand {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 24px;
    }
    .logo-box {
      width: 42px;
      height: 42px;
      background: linear-gradient(135deg, #1E88E5, #0D47A1);
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.2rem;
      color: white;
    }
    .brand-text h2 {
      font-size: 1.2rem;
      color: white;
    }
    .version {
      font-size: 0.7rem;
      color: #94A3B8;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }
    .org-card {
      background: #1E293B;
      border-radius: 12px;
      padding: 12px;
      display: flex;
      align-items: center;
      gap: 10px;
      margin-bottom: 24px;
      border: 1px solid #334155;
    }
    .org-logo {
      width: 36px;
      height: 36px;
      border-radius: 8px;
      object-fit: cover;
    }
    .org-name {
      font-size: 0.85rem;
      font-weight: 700;
      color: white;
    }
    .org-slug {
      font-size: 0.72rem;
      color: #94A3B8;
    }
    .nav-menu {
      display: flex;
      flex-direction: column;
      gap: 6px;
      flex-grow: 1;
    }
    .nav-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 10px 14px;
      border-radius: 8px;
      font-size: 0.88rem;
      font-weight: 600;
      color: #94A3B8;
      transition: all 0.2s ease;
    }
    .nav-item:hover, .nav-item.active {
      background: #1E88E5;
      color: white;
    }
    .sidebar-portal-cta {
      margin-bottom: 20px;
    }
    .btn-block { width: 100%; }
    .user-profile {
      display: flex;
      align-items: center;
      gap: 10px;
      padding-top: 16px;
      border-top: 1px solid #334155;
    }
    .user-avatar {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      background: #3B82F6;
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 700;
    }
    .user-details { flex-grow: 1; }
    .user-name { font-size: 0.82rem; font-weight: 700; }
    .user-role { font-size: 0.7rem; color: #94A3B8; }
    .logout-btn {
      background: transparent;
      border: none;
      color: #94A3B8;
      font-size: 1rem;
      cursor: pointer;
      padding: 6px;
    }
    .logout-btn:hover { color: #EF4444; }

    .main-content {
      flex-grow: 1;
      display: flex;
      flex-direction: column;
      background: #F1F5F9;
    }
    .topbar {
      height: 64px;
      background: white;
      border-bottom: 1px solid #E2E8F0;
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 28px;
    }
    .breadcrumb-app { color: #64748B; }
    .page-title { font-weight: 700; color: #0F172A; }
    .topbar-actions { display: flex; gap: 12px; }
    .content-body {
      padding: 28px;
      flex-grow: 1;
      overflow-y: auto;
    }
  `]
})
export class AdminLayoutComponent implements OnInit {
  session: any = null;

  constructor(
    private authService: AuthService,
    private apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.session = this.authService.currentSession();
  }

  logout(): void {
    this.authService.logout();
  }

  resetDemo(): void {
    if (confirm('Reset demo data to initial realistic state?')) {
      this.apiService.resetDemoData().subscribe({
        next: () => {
          alert('Demo data successfully reset!');
          window.location.reload();
        }
      });
    }
  }

  getRoleName(role: number): string {
    const roles: { [key: number]: string } = {
      1: 'Platform Admin',
      2: 'Org Owner',
      3: 'Org Admin',
      4: 'Trip Coordinator',
      5: 'Finance User',
      6: 'Traveller'
    };
    return roles[role] || 'User';
  }
}
