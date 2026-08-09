import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-guides',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="guides-page">
      <div class="page-header">
        <div class="header-left">
          <h2>Guide Management</h2>
          <p class="subtitle">Assign certifications, tracking experience, and manage schedules for tour operators</p>
        </div>
        <button (click)="openDrawer()" class="btn btn-primary">
          <i class="fa-solid fa-plus"></i> Add Tour Guide
        </button>
      </div>

      <div class="filter-bar card mb-4">
        <div class="search-box">
          <i class="fa-solid fa-magnifying-glass"></i>
          <input type="text" [(ngModel)]="search" (ngModelChange)="loadGuides()" placeholder="Search guides by name, language, expertise..." class="form-control">
        </div>
      </div>

      <div class="card table-card">
        <table class="table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Contact Info</th>
              <th>Languages</th>
              <th>Experience</th>
              <th>License No.</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let g of guides">
              <td>
                <div class="guide-profile">
                  <div class="avatar">{{ g.name.charAt(0) }}</div>
                  <div>
                    <span class="guide-name">{{ g.name }}</span>
                    <span class="specialization">{{ g.specializations }}</span>
                  </div>
                </div>
              </td>
              <td>
                <div class="contact-details">
                  <span><i class="fa-regular fa-envelope"></i> {{ g.email }}</span>
                  <span><i class="fa-solid fa-mobile-screen-button"></i> {{ g.phone }}</span>
                </div>
              </td>
              <td>{{ g.languages }}</td>
              <td>{{ g.experienceYears }} Years</td>
              <td><span class="badge badge-outline">{{ g.licenseNumber || 'None' }}</span></td>
              <td>
                <span class="status-indicator" [class.active]="g.status" [class.inactive]="!g.status">
                  {{ g.status ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td>
                <div class="table-actions">
                  <button (click)="openDrawer(g)" class="btn btn-icon" title="Edit Guide"><i class="fa-solid fa-pencil"></i></button>
                  <button (click)="toggleStatus(g)" class="btn btn-icon" [title]="g.status ? 'Deactivate' : 'Activate'">
                    <i class="fa-solid" [class.fa-ban]="g.status" [class.fa-check-circle]="!g.status"></i>
                  </button>
                </div>
              </td>
            </tr>
            <tr *ngIf="guides.length === 0">
              <td colspan="7" class="empty-state">
                <i class="fa-regular fa-folder-open"></i>
                <p>No tour guides registered yet. Register your first guide to assign them to itineraries.</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- SIDE DRAWER FOR CREATE/EDIT -->
      <div class="drawer-overlay" [class.open]="drawerOpen" (click)="closeDrawer()">
        <div class="drawer" (click)="$event.stopPropagation()">
          <div class="drawer-header">
            <h3>{{ editMode ? 'Modify Guide Details' : 'Register New Guide' }}</h3>
            <button (click)="closeDrawer()" class="btn-close"><i class="fa-solid fa-xmark"></i></button>
          </div>
          
          <div class="drawer-body">
            <form (ngSubmit)="saveGuide()">
              <div class="form-group">
                <label class="form-label">Full Name *</label>
                <input type="text" [(ngModel)]="form.name" name="name" class="form-control" required>
              </div>

              <div class="form-row">
                <div class="form-group col">
                  <label class="form-label">Email Address *</label>
                  <input type="email" [(ngModel)]="form.email" name="email" class="form-control" required [readonly]="editMode">
                </div>
                <div class="form-group col">
                  <label class="form-label">Phone Number *</label>
                  <input type="text" [(ngModel)]="form.phone" name="phone" class="form-control" required>
                </div>
              </div>

              <div class="form-row">
                <div class="form-group col">
                  <label class="form-label">Languages Spoken (comma-separated)</label>
                  <input type="text" [(ngModel)]="form.languages" name="languages" class="form-control" placeholder="Kannada, Tamil, English">
                </div>
                <div class="form-group col">
                  <label class="form-label">Experience Years</label>
                  <input type="number" [(ngModel)]="form.experienceYears" name="experienceYears" class="form-control">
                </div>
              </div>

              <div class="form-group">
                <label class="form-label">License Number / Certification ID</label>
                <input type="text" [(ngModel)]="form.licenseNumber" name="licenseNumber" class="form-control">
              </div>

              <div class="form-group">
                <label class="form-label">Specializations / Areas of Expertise</label>
                <input type="text" [(ngModel)]="form.specializations" name="specializations" class="form-control" placeholder="Adventure, Historical, Cultural tours">
              </div>

              <div class="form-group">
                <label class="form-label">Internal Coordinator Notes</label>
                <textarea [(ngModel)]="form.notes" name="notes" class="form-control" rows="3"></textarea>
              </div>

              <div class="drawer-actions">
                <button type="button" (click)="closeDrawer()" class="btn btn-outline">Cancel</button>
                <button type="submit" class="btn btn-primary">Save Profile</button>
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
    .filter-bar { padding: 16px; }
    .search-box { display: flex; align-items: center; gap: 10px; background: #F8FAFC; border: 1px solid #E2E8F0; border-radius: 8px; padding: 8px 14px; width: 100%; max-width: 450px; }
    .search-box i { color: #94A3B8; }
    .search-box input { border: none; background: transparent; outline: none; width: 100%; font-size: 0.9rem; }
    .table-card { border-radius: 12px; overflow: hidden; }
    .guide-profile { display: flex; align-items: center; gap: 12px; }
    .avatar { width: 36px; height: 36px; border-radius: 50%; background: #E0F2FE; color: #0369A1; font-weight: 700; display: flex; align-items: center; justify-content: center; }
    .guide-name { font-weight: 700; color: #0F172A; display: block; }
    .specialization { font-size: 0.75rem; color: #64748B; }
    .contact-details { display: flex; flex-direction: column; font-size: 0.8rem; color: #64748B; gap: 2px; }
    .contact-details i { width: 14px; }
    .badge-outline { border: 1px solid #CBD5E1; color: #475569; padding: 4px 8px; border-radius: 6px; font-size: 0.75rem; }
    .status-indicator { padding: 4px 8px; border-radius: 12px; font-size: 0.75rem; font-weight: 700; display: inline-block; }
    .status-indicator.active { background: #DCFCE7; color: #15803D; }
    .status-indicator.inactive { background: #F1F5F9; color: #475569; }
    .table-actions { display: flex; gap: 4px; }
    .btn-icon { width: 34px; height: 34px; border-radius: 6px; display: flex; align-items: center; justify-content: center; background: #F1F5F9; color: #475569; border: none; cursor: pointer; }
    .btn-icon:hover { background: #E2E8F0; color: #0F172A; }
    .empty-state { text-align: center; padding: 48px; color: #94A3B8; }
    .empty-state i { font-size: 2.5rem; margin-bottom: 12px; color: #CBD5E1; }
    .drawer-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(15, 23, 42, 0.4); opacity: 0; pointer-events: none; transition: opacity 0.3s ease; z-index: 1000; }
    .drawer-overlay.open { opacity: 1; pointer-events: auto; }
    .drawer { position: fixed; top: 0; right: -400px; width: 400px; height: 100vh; background: white; box-shadow: -4px 0 20px rgba(0, 0, 0, 0.1); transition: right 0.3s cubic-bezier(0.4, 0, 0.2, 1); display: flex; flex-direction: column; }
    .drawer-overlay.open .drawer { right: 0; }
    .drawer-header { display: flex; justify-content: space-between; align-items: center; padding: 20px; border-bottom: 1px solid #E2E8F0; }
    .btn-close { background: transparent; border: none; font-size: 1.2rem; cursor: pointer; color: #64748B; }
    .drawer-body { padding: 20px; overflow-y: auto; flex-grow: 1; }
    .form-row { display: flex; gap: 12px; }
    .col { flex: 1; }
    .drawer-actions { display: flex; justify-content: flex-end; gap: 10px; margin-top: 24px; padding-top: 16px; border-top: 1px solid #E2E8F0; }
  `]
})
export class GuidesComponent implements OnInit {
  guides: any[] = [];
  search = '';
  drawerOpen = false;
  editMode = false;
  selectedGuideId: string | null = null;

  form = {
    name: '',
    email: '',
    phone: '',
    languages: '',
    specializations: '',
    experienceYears: 0,
    licenseNumber: '',
    notes: ''
  };

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadGuides();
  }

  loadGuides(): void {
    this.apiService.getGuides(this.search).subscribe(res => this.guides = res);
  }

  openDrawer(g?: any): void {
    this.editMode = !!g;
    this.selectedGuideId = g ? g.id : null;
    this.drawerOpen = true;

    if (g) {
      this.form = {
        name: g.name,
        email: g.email,
        phone: g.phone,
        languages: g.languages,
        specializations: g.specializations,
        experienceYears: g.experienceYears,
        licenseNumber: g.licenseNumber || '',
        notes: g.notes || ''
      };
    } else {
      this.form = {
        name: '',
        email: '',
        phone: '',
        languages: 'Kannada, English',
        specializations: '',
        experienceYears: 2,
        licenseNumber: '',
        notes: ''
      };
    }
  }

  closeDrawer(): void {
    this.drawerOpen = false;
  }

  saveGuide(): void {
    const action = this.editMode && this.selectedGuideId
      ? this.apiService.updateGuide(this.selectedGuideId, this.form)
      : this.apiService.createGuide(this.form);

    action.subscribe({
      next: () => {
        this.loadGuides();
        this.closeDrawer();
      },
      error: (err) => alert(err.error || 'Failed to save guide.')
    });
  }

  toggleStatus(g: any): void {
    this.apiService.toggleGuideStatus(g.id).subscribe(() => this.loadGuides());
  }
}
