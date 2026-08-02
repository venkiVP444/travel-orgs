import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="login-wrapper">
      <div class="login-card">
        <div class="brand-header">
          <div class="brand-logo">
            <i class="fa-solid fa-plane-departure"></i>
          </div>
          <h1>TravelOrgOS</h1>
          <p class="subtitle">Travel Organization Operating System</p>
        </div>

        <form (ngSubmit)="onSubmit()" class="login-form">
          <div *ngIf="errorMessage" class="error-banner">
            <i class="fa-solid fa-triangle-exclamation"></i> {{ errorMessage }}
          </div>

          <div class="form-group">
            <label class="form-label">Email Address</label>
            <input type="email" [(ngModel)]="email" name="email" class="form-control" placeholder="name@organization.com" required>
          </div>

          <div class="form-group">
            <label class="form-label">Password</label>
            <input type="password" [(ngModel)]="password" name="password" class="form-control" placeholder="••••••••" required>
          </div>

          <button type="submit" [disabled]="loading" class="btn btn-primary btn-block">
            <i *ngIf="loading" class="fa-solid fa-spinner fa-spin"></i>
            <span *ngIf="!loading">Sign In to Dashboard</span>
          </button>
        </form>

        <div class="demo-accounts">
          <p class="demo-title">Select One-Click Demo Role:</p>
          <div class="demo-grid">
            <button (click)="fillDemo('owner@demo-travel.com')" class="demo-chip">
              <i class="fa-solid fa-user-tie"></i> Org Owner
            </button>
            <button (click)="fillDemo('manager@demo-travel.com')" class="demo-chip">
              <i class="fa-solid fa-user-gear"></i> Org Admin
            </button>
            <button (click)="fillDemo('finance@demo-travel.com')" class="demo-chip">
              <i class="fa-solid fa-wallet"></i> Finance User
            </button>
            <button (click)="fillDemo('admin@travelorgos.com')" class="demo-chip">
              <i class="fa-solid fa-shield-halved"></i> Platform Admin
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-wrapper {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #0F172A 0%, #1E293B 100%);
      padding: 20px;
    }
    .login-card {
      background: white;
      border-radius: 20px;
      padding: 40px;
      width: 100%;
      max-width: 440px;
      box-shadow: 0 20px 40px rgba(0,0,0,0.3);
    }
    .brand-header {
      text-align: center;
      margin-bottom: 28px;
    }
    .brand-logo {
      width: 60px;
      height: 60px;
      background: linear-gradient(135deg, #1E88E5, #0D47A1);
      color: white;
      border-radius: 16px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.6rem;
      margin: 0 auto 12px;
      box-shadow: 0 8px 20px rgba(30, 136, 229, 0.4);
    }
    .brand-header h1 {
      font-size: 1.8rem;
      color: #0F172A;
    }
    .subtitle {
      font-size: 0.88rem;
      color: #64748B;
      margin-top: 4px;
    }
    .error-banner {
      background: #FEE2E2;
      color: #B91C1C;
      padding: 10px 14px;
      border-radius: 8px;
      font-size: 0.85rem;
      margin-bottom: 16px;
      display: flex;
      align-items: center;
      gap: 8px;
    }
    .btn-block {
      width: 100%;
      padding: 12px;
      font-size: 0.95rem;
      margin-top: 8px;
    }
    .demo-accounts {
      margin-top: 28px;
      padding-top: 20px;
      border-top: 1px solid #E2E8F0;
    }
    .demo-title {
      font-size: 0.78rem;
      font-weight: 700;
      text-transform: uppercase;
      color: #64748B;
      margin-bottom: 10px;
    }
    .demo-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 8px;
    }
    .demo-chip {
      padding: 8px 12px;
      font-size: 0.78rem;
      font-weight: 600;
      border: 1px solid #E2E8F0;
      border-radius: 8px;
      background: #F8FAFC;
      cursor: pointer;
      display: flex;
      align-items: center;
      gap: 6px;
      transition: all 0.2s ease;
    }
    .demo-chip:hover {
      border-color: #1E88E5;
      color: #1E88E5;
      background: #E3F2FD;
    }
  `]
})
export class LoginComponent {
  email = 'owner@demo-travel.com';
  password = 'Demo@123';
  loading = false;
  errorMessage = '';

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router
  ) {}

  fillDemo(demoEmail: string): void {
    this.email = demoEmail;
    this.password = 'Demo@123';
  }

  onSubmit(): void {
    if (!this.email || !this.password) return;
    this.loading = true;
    this.errorMessage = '';

    this.apiService.login({ email: this.email, password: this.password }).subscribe({
      next: (session) => {
        this.loading = false;
        this.authService.setSession(session);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Invalid email or password.';
      }
    });
  }
}
