import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Traveller } from '../../models/models';

@Component({
  selector: 'app-travellers',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="travellers-page">
      <div class="page-header">
        <div>
          <h2>Traveller Directory</h2>
          <p class="subtitle">Manage organization customer profiles, passports, and contact records</p>
        </div>
        <div class="header-actions">
          <button (click)="openCsvModal()" class="btn btn-outline">
            <i class="fa-solid fa-file-csv"></i> Import CSV
          </button>
          <button (click)="openAddModal()" class="btn btn-primary">
            <i class="fa-solid fa-user-plus"></i> Add Traveller
          </button>
        </div>
      </div>

      <!-- SEARCH BAR -->
      <div class="filter-card card">
        <div class="search-box">
          <i class="fa-solid fa-magnifying-glass search-icon"></i>
          <input type="text" [(ngModel)]="searchQuery" (input)="onSearchChange()" class="form-control search-input" placeholder="Search by name, email, phone, or passport number...">
        </div>
      </div>

      <!-- TRAVELLER TABLE -->
      <div class="card table-card">
        <div class="table-container">
          <table class="tos-table">
            <thead>
              <tr>
                <th>Full Name</th>
                <th>Email</th>
                <th>Mobile Number</th>
                <th>Passport Number</th>
                <th>City / Country</th>
                <th>Emergency Contact</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let t of travellers">
                <td>
                  <div class="traveller-cell">
                    <div class="avatar">{{ t.firstName.charAt(0) }}{{ t.lastName.charAt(0) }}</div>
                    <div>
                      <strong>{{ t.firstName }} {{ t.lastName }}</strong>
                      <span class="sub-text">{{ t.gender || 'N/A' }}</span>
                    </div>
                  </div>
                </td>
                <td>{{ t.email }}</td>
                <td>{{ t.mobileNumber }}</td>
                <td>
                  <span class="passport-chip" *ngIf="t.passportNumber"><i class="fa-solid fa-passport"></i> {{ t.passportNumber }}</span>
                  <span *ngIf="!t.passportNumber" class="sub-text">None</span>
                </td>
                <td>{{ t.city || 'N/A' }}, {{ t.country || 'N/A' }}</td>
                <td>
                  <span *ngIf="t.emergencyContactName">{{ t.emergencyContactName }} ({{ t.emergencyContactNumber }})</span>
                  <span *ngIf="!t.emergencyContactName" class="sub-text">None</span>
                </td>
                <td>
                  <button (click)="editTraveller(t)" class="btn btn-sm btn-outline"><i class="fa-solid fa-pen"></i></button>
                  <button (click)="deleteTraveller(t.id)" class="btn btn-sm btn-danger"><i class="fa-solid fa-trash"></i></button>
                </td>
              </tr>
              <tr *ngIf="travellers.length === 0">
                <td colspan="7" class="empty-cell">
                  <i class="fa-solid fa-users-slash empty-icon"></i>
                  <p>No travellers found matching your search.</p>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- ADD / EDIT MODAL -->
      <div *ngIf="showModal" class="modal-overlay">
        <div class="modal-card">
          <div class="modal-header">
            <h3>{{ isEdit ? 'Edit Traveller Profile' : 'Add New Traveller' }}</h3>
            <button (click)="showModal = false" class="close-btn">&times;</button>
          </div>
          <div class="modal-body">
            <form (ngSubmit)="saveTraveller()">
              <div class="form-row">
                <div class="form-group col">
                  <label class="form-label">First Name *</label>
                  <input type="text" [(ngModel)]="form.firstName" name="firstName" class="form-control" required>
                </div>
                <div class="form-group col">
                  <label class="form-label">Last Name *</label>
                  <input type="text" [(ngModel)]="form.lastName" name="lastName" class="form-control" required>
                </div>
              </div>

              <div class="form-row">
                <div class="form-group col">
                  <label class="form-label">Email Address *</label>
                  <input type="email" [(ngModel)]="form.email" name="email" class="form-control" required>
                </div>
                <div class="form-group col">
                  <label class="form-label">Mobile Number *</label>
                  <input type="text" [(ngModel)]="form.mobileNumber" name="mobileNumber" class="form-control" required>
                </div>
              </div>

              <div class="form-row">
                <div class="form-group col">
                  <label class="form-label">Passport Number</label>
                  <input type="text" [(ngModel)]="form.passportNumber" name="passportNumber" class="form-control">
                </div>
                <div class="form-group col">
                  <label class="form-label">Nationality</label>
                  <input type="text" [(ngModel)]="form.nationality" name="nationality" class="form-control">
                </div>
              </div>

              <div class="form-row">
                <div class="form-group col">
                  <label class="form-label">Emergency Contact Name</label>
                  <input type="text" [(ngModel)]="form.emergencyContactName" name="emergencyContactName" class="form-control">
                </div>
                <div class="form-group col">
                  <label class="form-label">Emergency Contact Number</label>
                  <input type="text" [(ngModel)]="form.emergencyContactNumber" name="emergencyContactNumber" class="form-control">
                </div>
              </div>

              <div class="modal-footer">
                <button type="button" (click)="showModal = false" class="btn btn-secondary">Cancel</button>
                <button type="submit" class="btn btn-primary">Save Profile</button>
              </div>
            </form>
          </div>
        </div>
      </div>

      <!-- CSV IMPORT MODAL -->
      <div *ngIf="showCsvModal" class="modal-overlay">
        <div class="modal-card">
          <div class="modal-header">
            <h3>Import Travellers from CSV</h3>
            <button (click)="showCsvModal = false" class="close-btn">&times;</button>
          </div>
          <div class="modal-body">
            <p class="modal-sub">Upload a CSV file containing traveller records. Download the reference template below.</p>

            <div class="template-box">
              <a [href]="apiService.downloadTravellerCsvTemplateUrl()" download class="btn btn-sm btn-outline">
                <i class="fa-solid fa-download"></i> Download CSV Template
              </a>
            </div>

            <div class="file-dropzone">
              <input type="file" (change)="onFileSelected($event)" accept=".csv" class="file-input">
              <i class="fa-solid fa-cloud-arrow-up drop-icon"></i>
              <p>Click or drag CSV file here to upload</p>
              <span *ngIf="selectedFile" class="file-name">{{ selectedFile.name }}</span>
            </div>

            <div *ngIf="importSummary" class="import-summary-box">
              <h4>Import Summary Result:</h4>
              <div class="summary-stats">
                <span>Total: {{ importSummary.totalRecords }}</span>
                <span class="text-success">Success: {{ importSummary.successful }}</span>
                <span class="text-warning">Duplicates: {{ importSummary.duplicate }}</span>
                <span class="text-danger">Failed: {{ importSummary.failed }}</span>
              </div>

              <div *ngIf="importSummary.validationErrors?.length" class="error-list">
                <p><strong>Validation Errors:</strong></p>
                <ul>
                  <li *ngFor="let err of importSummary.validationErrors">{{ err }}</li>
                </ul>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button (click)="showCsvModal = false" class="btn btn-secondary">Close</button>
            <button (click)="uploadCsv()" [disabled]="!selectedFile || importing" class="btn btn-primary">
              <i *ngIf="importing" class="fa-solid fa-spinner fa-spin"></i> Process CSV Import
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    .header-actions { display: flex; gap: 12px; }
    .filter-card { margin-bottom: 20px; padding: 16px; }
    .search-box { position: relative; display: flex; align-items: center; }
    .search-icon { position: absolute; left: 16px; color: #94A3B8; }
    .search-input { padding-left: 44px; }

    .traveller-cell { display: flex; align-items: center; gap: 12px; }
    .avatar {
      width: 38px; height: 38px; border-radius: 50%; background: #E0F2FE; color: #0369A1;
      font-weight: 700; font-size: 0.85rem; display: flex; align-items: center; justify-content: center;
    }
    .sub-text { font-size: 0.78rem; color: #64748B; display: block; }
    .passport-chip { font-size: 0.8rem; background: #F1F5F9; padding: 4px 8px; border-radius: 6px; font-family: monospace; }
    .empty-cell { text-align: center; padding: 40px; color: #64748B; }
    .empty-icon { font-size: 2rem; margin-bottom: 8px; }

    .form-row { display: flex; gap: 16px; }
    .col { flex: 1; }
    .close-btn { background: none; border: none; font-size: 1.5rem; cursor: pointer; color: #64748B; }

    .template-box { margin-bottom: 16px; text-align: right; }
    .file-dropzone {
      border: 2px dashed #CBD5E1; border-radius: 12px; padding: 30px; text-align: center; position: relative; background: #F8FAFC;
    }
    .file-input { position: absolute; top:0; left:0; width:100%; height:100%; opacity: 0; cursor: pointer; }
    .drop-icon { font-size: 2.2rem; color: #1E88E5; margin-bottom: 8px; }
    .file-name { font-weight: 700; color: #1E88E5; margin-top: 8px; display: block; }
    .import-summary-box { margin-top: 20px; background: #F8FAFC; padding: 16px; border-radius: 10px; border: 1px solid #E2E8F0; }
    .summary-stats { display: flex; gap: 16px; font-weight: 700; margin: 8px 0; }
    .error-list { max-height: 120px; overflow-y: auto; font-size: 0.82rem; color: #DC2626; margin-top: 8px; }
  `]
})
export class TravellersComponent implements OnInit {
  travellers: Traveller[] = [];
  searchQuery = '';
  showModal = false;
  isEdit = false;
  selectedId = '';

  form: any = {
    firstName: '', lastName: '', email: '', mobileNumber: '',
    nationality: '', passportNumber: '', emergencyContactName: '', emergencyContactNumber: ''
  };

  showCsvModal = false;
  selectedFile: File | null = null;
  importing = false;
  importSummary: any = null;

  constructor(public apiService: ApiService) {}

  ngOnInit(): void {
    this.loadTravellers();
  }

  loadTravellers(): void {
    this.apiService.getTravellers(this.searchQuery).subscribe({
      next: (data) => this.travellers = data
    });
  }

  onSearchChange(): void {
    this.loadTravellers();
  }

  openAddModal(): void {
    this.isEdit = false;
    this.form = { firstName: '', lastName: '', email: '', mobileNumber: '', nationality: '', passportNumber: '' };
    this.showModal = true;
  }

  editTraveller(t: Traveller): void {
    this.isEdit = true;
    this.selectedId = t.id;
    this.form = { ...t };
    this.showModal = true;
  }

  saveTraveller(): void {
    if (this.isEdit) {
      this.apiService.updateTraveller(this.selectedId, this.form).subscribe({
        next: () => {
          this.showModal = false;
          this.loadTravellers();
        }
      });
    } else {
      this.apiService.createTraveller(this.form).subscribe({
        next: () => {
          this.showModal = false;
          this.loadTravellers();
        }
      });
    }
  }

  deleteTraveller(id: string): void {
    if (confirm('Are you sure you want to delete this traveller profile?')) {
      this.apiService.deleteTraveller(id).subscribe({
        next: () => this.loadTravellers()
      });
    }
  }

  openCsvModal(): void {
    this.selectedFile = null;
    this.importSummary = null;
    this.showCsvModal = true;
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  uploadCsv(): void {
    if (!this.selectedFile) return;
    this.importing = true;

    this.apiService.importTravellerCsv(this.selectedFile).subscribe({
      next: (summary) => {
        this.importing = false;
        this.importSummary = summary;
        this.loadTravellers();
      },
      error: () => this.importing = false
    });
  }
}
