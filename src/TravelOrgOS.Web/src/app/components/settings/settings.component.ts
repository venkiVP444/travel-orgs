import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Organization } from '../../models/models';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="settings-page">
      <div class="page-header">
        <h2>Organization Branding & Settings</h2>
        <p class="subtitle">Customize your organization logo, primary/secondary colors, and Traveller Portal branding</p>
      </div>

      <div class="card settings-card" *ngIf="org">
        <form (ngSubmit)="saveSettings()">
          <h3 class="section-title"><i class="fa-solid fa-palette"></i> Brand Identity & Colors</h3>
          
          <div class="form-row">
            <div class="form-group col">
              <label class="form-label">Organization Name *</label>
              <input type="text" [(ngModel)]="org.name" name="name" class="form-control" required>
            </div>
            <div class="form-group col">
              <label class="form-label">Portal Slug (URL)</label>
              <input type="text" [(ngModel)]="org.slug" name="slug" class="form-control" readonly>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group col">
              <label class="form-label">Primary Brand Color</label>
              <div class="color-picker-row">
                <input type="color" [(ngModel)]="org.primaryColor" name="primaryColor" class="color-picker">
                <input type="text" [(ngModel)]="org.primaryColor" name="primaryColorText" class="form-control">
              </div>
            </div>
            <div class="form-group col">
              <label class="form-label">Secondary Brand Color</label>
              <div class="color-picker-row">
                <input type="color" [(ngModel)]="org.secondaryColor" name="secondaryColor" class="color-picker">
                <input type="text" [(ngModel)]="org.secondaryColor" name="secondaryColorText" class="form-control">
              </div>
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">Organization Logo URL</label>
            <input type="text" [(ngModel)]="org.logoUrl" name="logoUrl" class="form-control" placeholder="https://...">
          </div>

          <div class="form-group">
            <label class="form-label">Portal Welcome Message</label>
            <input type="text" [(ngModel)]="org.welcomeMessage" name="welcomeMessage" class="form-control">
          </div>

          <h3 class="section-title mt-4"><i class="fa-solid fa-building"></i> Contact & Business Information</h3>

          <div class="form-row">
            <div class="form-group col">
              <label class="form-label">Public Email *</label>
              <input type="email" [(ngModel)]="org.email" name="email" class="form-control" required>
            </div>
            <div class="form-group col">
              <label class="form-label">Phone / WhatsApp Number *</label>
              <input type="text" [(ngModel)]="org.phone" name="phone" class="form-control" required>
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">Physical Address</label>
            <input type="text" [(ngModel)]="org.address" name="address" class="form-control">
          </div>

          <div class="form-actions mt-4">
            <button type="submit" [disabled]="saving" class="btn btn-primary btn-lg">
              <i *ngIf="saving" class="fa-solid fa-spinner fa-spin"></i>
              <span *ngIf="!saving">Save Branding Settings</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .page-header { margin-bottom: 24px; }
    .settings-card { padding: 32px; max-width: 800px; }
    .section-title { font-size: 1.1rem; color: #0F172A; margin-bottom: 16px; padding-bottom: 8px; border-bottom: 1px solid #E2E8F0; display: flex; align-items: center; gap: 8px; }
    .form-row { display: flex; gap: 16px; }
    .col { flex: 1; }
    .color-picker-row { display: flex; gap: 10px; align-items: center; }
    .color-picker { width: 44px; height: 42px; padding: 2px; border-radius: 8px; border: 1px solid #CBD5E1; cursor: pointer; }
    .form-actions { display: flex; justify-content: flex-end; }
  `]
})
export class SettingsComponent implements OnInit {
  org: Organization | null = null;
  saving = false;

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.apiService.getOrganizationMe().subscribe(o => this.org = o);
  }

  saveSettings(): void {
    if (!this.org) return;
    this.saving = true;

    this.apiService.updateOrganizationMe(this.org).subscribe({
      next: (updated) => {
        this.org = updated;
        this.saving = false;
        alert('Branding settings saved successfully! Your Traveller Portal has been updated.');
      },
      error: () => this.saving = false
    });
  }
}
