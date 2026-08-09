import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-team',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="team-page">
      <div class="page-header">
        <div class="header-left">
          <h2>Team & Access Management</h2>
          <p class="subtitle">Invite coordinators, assign role permissions, and control operational accesses</p>
        </div>
        <button (click)="openInviteDrawer()" class="btn btn-primary">
          <i class="fa-solid fa-user-plus"></i> Invite Team Member
        </button>
      </div>

      <div class="card table-card mt-4">
        <table class="table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Assigned Role</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let m of members">
              <td>
                <div class="member-profile">
                  <div class="avatar">{{ m.fullName.charAt(0) }}</div>
                  <span class="member-name">{{ m.fullName }}</span>
                </div>
              </td>
              <td>{{ m.email }}</td>
              <td>
                <select [ngModel]="m.role" (ngModelChange)="changeRole(m, $event)" class="form-control role-select">
                  <option [value]="2">Organization Owner</option>
                  <option [value]="3">Organization Admin</option>
                  <option [value]="4">Trip Coordinator</option>
                  <option [value]="5">Finance User</option>
                </select>
              </td>
              <td>
                <span class="status-indicator" [class.active]="m.status" [class.inactive]="!m.status">
                  {{ m.status ? 'Active' : 'Pending/Inactive' }}
                </span>
              </td>
              <td>
                <button (click)="toggleStatus(m)" class="btn btn-icon" [title]="m.status ? 'Deactivate Member' : 'Activate Member'">
                  <i class="fa-solid" [class.fa-ban]="m.status" [class.fa-circle-check]="!m.status"></i>
                </button>
              </td>
            </tr>
            <tr *ngIf="members.length === 0">
              <td colspan="5" class="empty-state">
                <i class="fa-regular fa-address-book"></i>
                <p>No team members found.</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- DRAWER FOR INVITATION -->
      <div class="drawer-overlay" [class.open]="drawerOpen" (click)="closeDrawer()">
        <div class="drawer" (click)="$event.stopPropagation()">
          <div class="drawer-header">
            <h3>Invite Team Member</h3>
            <button (click)="closeDrawer()" class="btn-close"><i class="fa-solid fa-xmark"></i></button>
          </div>
          
          <div class="drawer-body">
            <form (ngSubmit)="sendInvitation()">
              <div class="form-group">
                <label class="form-label">Full Name *</label>
                <input type="text" [(ngModel)]="inviteForm.fullName" name="fullName" class="form-control" required placeholder="E.g. Rajesh Kumar">
              </div>

              <div class="form-group">
                <label class="form-label">Email Address *</label>
                <input type="email" [(ngModel)]="inviteForm.email" name="email" class="form-control" required placeholder="rajesh@agency.com">
              </div>

              <div class="form-group">
                <label class="form-label">System Access Role *</label>
                <select [(ngModel)]="inviteForm.role" name="role" class="form-control" required>
                  <option [value]="3">Organization Admin</option>
                  <option [value]="4">Trip Coordinator</option>
                  <option [value]="5">Finance User</option>
                </select>
                <small class="form-text text-muted mt-1">Select the role matching their job capability limits.</small>
              </div>

              <div class="invite-link-box card mt-3 p-3" *ngIf="generatedInviteLink">
                <p class="label mb-1">Generated Invite Link (Onboarding Mock):</p>
                <input type="text" [value]="generatedInviteLink" class="form-control mb-2" readonly>
                <small class="text-success">Copy this link and send to user to let them set their password!</small>
              </div>

              <div class="drawer-actions">
                <button type="button" (click)="closeDrawer()" class="btn btn-outline">Cancel</button>
                <button type="submit" [disabled]="loading" class="btn btn-primary">
                  <span *ngIf="!loading">Send Invitation</span>
                  <i *ngIf="loading" class="fa-solid fa-spinner fa-spin"></i>
                </button>
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
    .member-profile { display: flex; align-items: center; gap: 12px; }
    .avatar { width: 36px; height: 36px; border-radius: 50%; background: #F0FDF4; color: #166534; font-weight: 700; display: flex; align-items: center; justify-content: center; }
    .member-name { font-weight: 700; color: #0F172A; }
    .role-select { max-width: 220px; height: 38px; }
    .status-indicator { padding: 4px 8px; border-radius: 12px; font-size: 0.75rem; font-weight: 700; display: inline-block; }
    .status-indicator.active { background: #DCFCE7; color: #15803D; }
    .status-indicator.inactive { background: #FEF3C7; color: #D97706; }
    .btn-icon { width: 34px; height: 34px; border-radius: 6px; display: flex; align-items: center; justify-content: center; background: #F1F5F9; color: #475569; border: none; cursor: pointer; }
    .btn-icon:hover { background: #E2E8F0; }
    .empty-state { text-align: center; padding: 48px; color: #94A3B8; }
    .empty-state i { font-size: 2.5rem; margin-bottom: 12px; color: #CBD5E1; }
    .drawer-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(15, 23, 42, 0.4); opacity: 0; pointer-events: none; transition: opacity 0.3s ease; z-index: 1000; }
    .drawer-overlay.open { opacity: 1; pointer-events: auto; }
    .drawer { position: fixed; top: 0; right: -400px; width: 400px; height: 100vh; background: white; box-shadow: -4px 0 20px rgba(0, 0, 0, 0.1); transition: right 0.3s cubic-bezier(0.4, 0, 0.2, 1); display: flex; flex-direction: column; }
    .drawer-overlay.open .drawer { right: 0; }
    .drawer-header { display: flex; justify-content: space-between; align-items: center; padding: 20px; border-bottom: 1px solid #E2E8F0; }
    .btn-close { background: transparent; border: none; font-size: 1.2rem; cursor: pointer; color: #64748B; }
    .drawer-body { padding: 20px; overflow-y: auto; flex-grow: 1; }
    .drawer-actions { display: flex; justify-content: flex-end; gap: 10px; margin-top: 24px; padding-top: 16px; border-top: 1px solid #E2E8F0; }
    .invite-link-box { border-left: 4px solid #10B981; }
  `]
})
export class TeamComponent implements OnInit {
  members: any[] = [];
  drawerOpen = false;
  loading = false;
  generatedInviteLink = '';

  inviteForm = {
    fullName: '',
    email: '',
    role: 4
  };

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadTeam();
  }

  loadTeam(): void {
    this.apiService.getTeamMembers().subscribe(res => this.members = res);
  }

  openInviteDrawer(): void {
    this.generatedInviteLink = '';
    this.inviteForm = {
      fullName: '',
      email: '',
      role: 4
    };
    this.drawerOpen = true;
  }

  closeDrawer(): void {
    this.drawerOpen = false;
  }

  sendInvitation(): void {
    this.loading = true;
    this.apiService.inviteTeamMember(this.inviteForm).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.generatedInviteLink = res.inviteLink;
        this.loadTeam();
      },
      error: (err) => {
        this.loading = false;
        alert(err.error || 'Failed to send team invitation.');
      }
    });
  }

  toggleStatus(m: any): void {
    this.apiService.toggleTeamMemberStatus(m.id).subscribe(() => this.loadTeam());
  }

  changeRole(m: any, newRole: any): void {
    const roleId = parseInt(newRole, 10);
    this.apiService.changeTeamMemberRole(m.id, roleId).subscribe({
      next: () => this.loadTeam(),
      error: (err) => alert(err.error || 'Failed to change role.')
    });
  }
}
