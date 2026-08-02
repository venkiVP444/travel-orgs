import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, GridOptions, RowClickedEvent } from 'ag-grid-community';
import { ApiService } from '../../services/api.service';
import { Trip, TripStatus } from '../../models/models';

@Component({
  selector: 'app-trips-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, AgGridModule],
  template: `
    <div class="trips-management-page">
      <!-- PAGE HEADER -->
      <div class="page-header">
        <div>
          <h2>Trip Management</h2>
          <p class="subtitle">Complete operational grid for tour packages, routes, seat capacity, guides, drivers, income & expenses</p>
        </div>
        <div class="header-actions">
          <a routerLink="/trips/new" class="btn btn-primary btn-lg">
            <i class="fa-solid fa-wand-magic-sparkles"></i> Launch Trip Builder
          </a>
        </div>
      </div>

      <!-- STATUS FILTER TABS BAR -->
      <div class="card tabs-card mb-4">
        <div class="filter-tabs">
          <button (click)="filterByStatus(null)" class="tab-btn" [class.active]="selectedTab === null">
            All Trips <span class="tab-count">{{ trips.length }}</span>
          </button>
          <button (click)="filterByStatus(3)" class="tab-btn" [class.active]="selectedTab === 3">
            Registration Open <span class="tab-count">{{ getCountByStatus(3) }}</span>
          </button>
          <button (click)="filterByStatus(2)" class="tab-btn" [class.active]="selectedTab === 2">
            Published <span class="tab-count">{{ getCountByStatus(2) }}</span>
          </button>
          <button (click)="filterByStatus(4)" class="tab-btn" [class.active]="selectedTab === 4">
            Almost Full <span class="tab-count">{{ getCountByStatus(4) }}</span>
          </button>
          <button (click)="filterByStatus(5)" class="tab-btn" [class.active]="selectedTab === 5">
            Fully Booked <span class="tab-count">{{ getCountByStatus(5) }}</span>
          </button>
          <button (click)="filterByStatus(1)" class="tab-btn" [class.active]="selectedTab === 1">
            Draft <span class="tab-count">{{ getCountByStatus(1) }}</span>
          </button>
          <button (click)="filterByStatus(7)" class="tab-btn" [class.active]="selectedTab === 7">
            Completed <span class="tab-count">{{ getCountByStatus(7) }}</span>
          </button>
          <button (click)="filterByStatus(8)" class="tab-btn" [class.active]="selectedTab === 8">
            Cancelled <span class="tab-count">{{ getCountByStatus(8) }}</span>
          </button>
        </div>
      </div>

      <!-- SEARCH & SUMMARY ROW -->
      <div class="card search-card mb-4">
        <div class="search-row">
          <div class="search-box">
            <i class="fa-solid fa-magnifying-glass search-icon"></i>
            <input 
              type="text" 
              [(ngModel)]="searchQuery" 
              (input)="onQuickFilterChanged()" 
              class="form-control search-input" 
              placeholder="Search by trip code, title, origin, destination, guide, or driver..."
            />
          </div>

          <div class="financial-summary-pills">
            <div class="summary-pill">
              <span class="label">Total Income</span>
              <span class="val text-success">\${{ totalIncome | number:'1.2-2' }}</span>
            </div>
            <div class="summary-pill">
              <span class="label">Total Expense</span>
              <span class="val text-danger">\${{ totalExpense | number:'1.2-2' }}</span>
            </div>
            <div class="summary-pill highlight">
              <span class="label">Net Margin</span>
              <span class="val text-primary">\${{ (totalIncome - totalExpense) | number:'1.2-2' }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- AG GRID TRIP MANAGEMENT TABLE -->
      <div class="card grid-card">
        <div class="grid-instructions">
          <i class="fa-solid fa-circle-info"></i> Click any row to view & edit trip details. Use column headers to sort (ASC/DESC) or filter data.
        </div>

        <ag-grid-angular
          style="width: 100%; height: 580px;"
          class="ag-theme-alpine tos-ag-grid"
          [gridOptions]="gridOptions"
          [rowData]="filteredTrips"
          [columnDefs]="columnDefs"
          [defaultColDef]="defaultColDef"
          [pagination]="true"
          [paginationPageSize]="15"
          (rowClicked)="onRowClicked($event)"
          (gridReady)="onGridReady($event)"
        >
        </ag-grid-angular>
      </div>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
    .tabs-card { padding: 8px 16px; margin-bottom: 20px; }
    .filter-tabs { display: flex; gap: 8px; overflow-x: auto; padding: 4px 0; }
    .tab-btn { background: none; border: none; padding: 10px 16px; border-radius: 12px; font-weight: 700; font-size: 0.85rem; color: #64748B; cursor: pointer; display: flex; align-items: center; gap: 8px; white-space: nowrap; transition: all 0.2s ease; }
    .tab-btn.active { background: #E3F2FD; color: #1E88E5; }
    .tab-count { background: #E2E8F0; color: #334155; padding: 2px 8px; border-radius: 12px; font-size: 0.75rem; }
    .tab-btn.active .tab-count { background: #1E88E5; color: white; }

    .search-card { padding: 16px 24px; margin-bottom: 20px; }
    .search-row { display: flex; justify-content: space-between; align-items: center; gap: 20px; }
    .search-box { flex-grow: 1; position: relative; display: flex; align-items: center; max-width: 500px; }
    .search-icon { position: absolute; left: 16px; color: #94A3B8; }
    .search-input { padding-left: 44px; height: 44px; border-radius: 10px; }

    .financial-summary-pills { display: flex; gap: 16px; }
    .summary-pill { background: #F8FAFC; border: 1px solid #E2E8F0; padding: 6px 14px; border-radius: 10px; display: flex; flex-direction: column; align-items: flex-end; }
    .summary-pill.highlight { background: #F0F9FF; border-color: #BAE6FD; }
    .summary-pill .label { font-size: 0.72rem; font-weight: 700; color: #64748B; text-transform: uppercase; }
    .summary-pill .val { font-size: 1rem; font-weight: 800; }
    .text-success { color: #16A34A; }
    .text-danger { color: #DC2626; }
    .text-primary { color: #1E88E5; }

    .grid-card { padding: 20px; overflow: hidden; }
    .grid-instructions { font-size: 0.82rem; color: #64748B; background: #F1F5F9; padding: 10px 16px; border-radius: 8px; margin-bottom: 16px; display: flex; align-items: center; gap: 8px; }

    ::ng-deep .ag-theme-alpine {
      --ag-header-background-color: #F8FAFC;
      --ag-header-foreground-color: #0F172A;
      --ag-row-hover-color: #F0F9FF;
      --ag-selected-row-background-color: #E0F2FE;
      --ag-font-size: 13px;
      --ag-font-family: 'Plus Jakarta Sans', sans-serif;
    }
    ::ng-deep .ag-row { cursor: pointer; }
    .badge-draft { background: #F1F5F9; color: #475569; padding: 4px 10px; border-radius: 12px; font-weight: 700; font-size: 0.75rem; }
    .badge-success { background: #DCFCE7; color: #16A34A; padding: 4px 10px; border-radius: 12px; font-weight: 700; font-size: 0.75rem; }
    .badge-warning { background: #FEF3C7; color: #D97706; padding: 4px 10px; border-radius: 12px; font-weight: 700; font-size: 0.75rem; }
    .badge-danger { background: #FEE2E2; color: #DC2626; padding: 4px 10px; border-radius: 12px; font-weight: 700; font-size: 0.75rem; }
  `]
})
export class TripsListComponent implements OnInit {
  trips: Trip[] = [];
  filteredTrips: Trip[] = [];
  selectedTab: number | null = null;
  searchQuery = '';
  gridApi: any;

  totalIncome = 0;
  totalExpense = 0;

  defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    floatingFilter: true,
    resizable: true,
    suppressHeaderMenuButton: false
  };

  columnDefs: ColDef[] = [
    {
      headerName: 'Trip ID',
      field: 'tripCode',
      width: 140,
      pinned: 'left',
      cellRenderer: (params: any) => `
        <span style="font-weight: 800; color: #1E88E5; background: #E3F2FD; padding: 4px 8px; border-radius: 6px;">
          ${params.value || ''}
        </span>
      `
    },
    {
      headerName: 'Trip Name',
      field: 'tripName',
      width: 240,
      pinned: 'left',
      cellRenderer: (params: any) => `
        <div style="font-weight: 700; color: #0F172A; line-height: 1.2; padding-top: 4px;">
          ${params.value || ''}
        </div>
      `
    },
    {
      headerName: 'Period',
      width: 190,
      valueGetter: (params: any) => {
        if (!params.data.startDate) return '';
        const start = new Date(params.data.startDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
        const end = new Date(params.data.endDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
        return `${start} - ${end}`;
      }
    },
    {
      headerName: 'Days / Nights',
      width: 130,
      valueGetter: (params: any) => `${params.data.durationDays || 0}D / ${params.data.durationNights || 0}N`
    },
    {
      headerName: 'Origin',
      field: 'startLocation',
      width: 160
    },
    {
      headerName: 'Destination',
      field: 'destination',
      width: 160
    },
    {
      headerName: 'Via / Route',
      field: 'viaRoute',
      width: 200,
      valueFormatter: (params: any) => params.value || 'Direct'
    },
    {
      headerName: 'Total Seats',
      field: 'totalCapacity',
      width: 110,
      type: 'numericColumn'
    },
    {
      headerName: 'Booked',
      field: 'bookedSeats',
      width: 110,
      type: 'numericColumn',
      valueGetter: (params: any) => Math.max(0, (params.data.totalCapacity || 0) - (params.data.availableSeats || 0)),
      cellRenderer: (params: any) => `<strong style="color: #16A34A;">${params.value}</strong>`
    },
    {
      headerName: 'Not Booked',
      field: 'availableSeats',
      width: 120,
      type: 'numericColumn',
      cellRenderer: (params: any) => `<strong style="color: #E11D48;">${params.value}</strong>`
    },
    {
      headerName: 'Status',
      field: 'status',
      width: 150,
      cellRenderer: (params: any) => {
        const text = this.getStatusText(params.value);
        let badgeClass = 'badge-draft';
        if (params.value === 2 || params.value === 3) badgeClass = 'badge-success';
        if (params.value === 4) badgeClass = 'badge-warning';
        if (params.value === 5 || params.value === 8) badgeClass = 'badge-danger';
        return `<span class="${badgeClass}">${text}</span>`;
      }
    },
    {
      headerName: 'Guide / Escort',
      field: 'hostGuide',
      width: 150,
      valueFormatter: (params: any) => params.value || 'Assigned Guide'
    },
    {
      headerName: 'Driver',
      field: 'driverName',
      width: 140,
      valueFormatter: (params: any) => params.value || 'Fleet Driver'
    },
    {
      headerName: 'Income ($)',
      field: 'grossRevenue',
      width: 130,
      type: 'numericColumn',
      valueGetter: (params: any) => {
        const booked = Math.max(0, (params.data.totalCapacity || 0) - (params.data.availableSeats || 0));
        return booked * (params.data.basePrice || 0);
      },
      valueFormatter: (params: any) => `$${(params.value || 0).toLocaleString()}`
    },
    {
      headerName: 'Expense ($)',
      field: 'estimatedCost',
      width: 130,
      type: 'numericColumn',
      valueFormatter: (params: any) => `$${(params.value || 0).toLocaleString()}`
    },
    {
      headerName: 'Net Margin ($)',
      width: 140,
      type: 'numericColumn',
      valueGetter: (params: any) => {
        const booked = Math.max(0, (params.data.totalCapacity || 0) - (params.data.availableSeats || 0));
        const income = booked * (params.data.basePrice || 0);
        const expense = params.data.estimatedCost || 0;
        return income - expense;
      },
      cellRenderer: (params: any) => {
        const val = params.value || 0;
        const color = val >= 0 ? '#16A34A' : '#DC2626';
        return `<strong style="color: ${color};">$${val.toLocaleString()}</strong>`;
      }
    }
  ];

  gridOptions: GridOptions = {
    rowHeight: 48,
    headerHeight: 44,
    animateRows: true
  };

  constructor(private apiService: ApiService, private router: Router) {}

  ngOnInit(): void {
    this.loadTrips();
  }

  loadTrips(): void {
    this.apiService.getTrips().subscribe({
      next: (data) => {
        this.trips = data;
        this.applyFilters();
      }
    });
  }

  filterByStatus(status: number | null): void {
    this.selectedTab = status;
    this.applyFilters();
  }

  applyFilters(): void {
    let result = [...this.trips];

    if (this.selectedTab !== null) {
      result = result.filter(t => t.status === this.selectedTab);
    }

    this.filteredTrips = result;
    this.calculateTotals(result);

    if (this.gridApi) {
      this.gridApi.setGridOption('rowData', this.filteredTrips);
    }
  }

  calculateTotals(trips: Trip[]): void {
    this.totalIncome = trips.reduce((acc, t) => {
      const booked = Math.max(0, t.totalCapacity - t.availableSeats);
      return acc + (booked * t.basePrice);
    }, 0);

    this.totalExpense = trips.reduce((acc, t) => acc + (t.estimatedCost || 0), 0);
  }

  getCountByStatus(status: number): number {
    return this.trips.filter(t => t.status === status).length;
  }

  onQuickFilterChanged(): void {
    if (this.gridApi) {
      this.gridApi.setGridOption('quickFilterText', this.searchQuery);
    }
  }

  onGridReady(params: any): void {
    this.gridApi = params.api;
  }

  onRowClicked(event: RowClickedEvent): void {
    if (event.data && event.data.id) {
      this.router.navigate(['/trips', event.data.id]);
    }
  }

  getStatusText(status: number): string {
    const map: { [key: number]: string } = {
      1: 'Draft', 2: 'Published', 3: 'Registration Open',
      4: 'Almost Full', 5: 'Fully Booked', 6: 'In Progress', 7: 'Completed', 8: 'Cancelled'
    };
    return map[status] || 'Draft';
  }
}
