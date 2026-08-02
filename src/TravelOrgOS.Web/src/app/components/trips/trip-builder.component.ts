import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Trip, ItineraryDay, TripHotel, TripVehicle, TripVendor, TripMeal } from '../../models/models';

@Component({
  selector: 'app-trip-builder',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="trip-builder-page">
      <div class="page-header">
        <div>
          <h2>{{ isEdit ? 'Edit Trip Package' : 'Multi-Step Trip Builder' }}</h2>
          <p class="subtitle">Design detailed itineraries, hotels, transport, pricing, and publish</p>
        </div>
        <div class="header-actions">
          <button (click)="saveDraft()" class="btn btn-outline">
            <i class="fa-solid fa-floppy-disk"></i> Save Draft
          </button>
          <button (click)="publishTrip()" class="btn btn-success">
            <i class="fa-solid fa-paper-plane"></i> Publish Trip
          </button>
        </div>
      </div>

      <!-- 10-STEP STEPPER HEADER -->
      <div class="stepper-header card">
        <div *ngFor="let s of steps; let i = index" 
             (click)="goToStep(i + 1)"
             class="stepper-item" 
             [class.active]="currentStep === i + 1">
          <span class="step-num">{{ i + 1 }}</span>
          <span class="step-name">{{ s }}</span>
        </div>
      </div>

      <!-- STEP CONTENT BODY -->
      <div class="card builder-body-card">

        <!-- STEP 1: BASIC INFORMATION -->
        <div *ngIf="currentStep === 1">
          <h3>Step 1: Basic Trip Information</h3>
          <div class="form-row">
            <div class="form-group col">
              <label class="form-label">Trip Code *</label>
              <input type="text" [(ngModel)]="trip.tripCode" class="form-control" placeholder="e.g. KER-2026-001" required>
            </div>
            <div class="form-group col">
              <label class="form-label">Trip Title *</label>
              <input type="text" [(ngModel)]="trip.tripName" class="form-control" placeholder="e.g. Kerala Backwaters Escape" required>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group col">
              <label class="form-label">Primary Destination *</label>
              <input type="text" [(ngModel)]="trip.destination" class="form-control" placeholder="e.g. Kerala, India" required>
            </div>
            <div class="form-group col">
              <label class="form-label">Trip Category / Type</label>
              <select [(ngModel)]="trip.tripType" class="form-control">
                <option [ngValue]="1">Leisure</option>
                <option [ngValue]="2">Adventure</option>
                <option [ngValue]="3">Family</option>
                <option [ngValue]="4">Pilgrimage</option>
                <option [ngValue]="5">Corporate</option>
                <option [ngValue]="7">Group Tour</option>
                <option [ngValue]="8">Weekend Getaway</option>
              </select>
            </div>
          </div>

          <div class="form-group">
            <label class="form-label">Short Description</label>
            <input type="text" [(ngModel)]="trip.shortDescription" class="form-control" placeholder="Brief tagline shown on trip card">
          </div>

          <div class="form-group">
            <label class="form-label">Full Tour Description</label>
            <textarea [(ngModel)]="trip.description" class="form-control" rows="4" placeholder="Detailed tour highlight description..."></textarea>
          </div>

          <div class="form-group">
            <label class="form-label">Cover Image URL</label>
            <input type="text" [(ngModel)]="trip.coverImageUrl" class="form-control" placeholder="https://images.unsplash.com/...">
          </div>
        </div>

        <!-- STEP 2: DATES & CAPACITY -->
        <div *ngIf="currentStep === 2">
          <h3>Step 2: Dates, Location & Seat Capacity</h3>
          <div class="form-row">
            <div class="form-group col">
              <label class="form-label">Start Date *</label>
              <input type="date" [(ngModel)]="trip.startDate" class="form-control" required>
            </div>
            <div class="form-group col">
              <label class="form-label">End Date *</label>
              <input type="date" [(ngModel)]="trip.endDate" class="form-control" required>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group col">
              <label class="form-label">Duration (Days)</label>
              <input type="number" [(ngModel)]="trip.durationDays" class="form-control">
            </div>
            <div class="form-group col">
              <label class="form-label">Duration (Nights)</label>
              <input type="number" [(ngModel)]="trip.durationNights" class="form-control">
            </div>
          </div>

          <div class="form-row">
            <div class="form-group col">
              <label class="form-label">Start Location (Airport/City)</label>
              <input type="text" [(ngModel)]="trip.startLocation" class="form-control" placeholder="e.g. Cochin Airport (COK)">
            </div>
            <div class="form-group col">
              <label class="form-label">End Location</label>
              <input type="text" [(ngModel)]="trip.endLocation" class="form-control" placeholder="e.g. Cochin Airport (COK)">
            </div>
          </div>

          <div class="form-row">
            <div class="form-group col">
              <label class="form-label">Total Passenger Capacity *</label>
              <input type="number" [(ngModel)]="trip.totalCapacity" class="form-control" required>
            </div>
            <div class="form-group col">
              <label class="form-label">Available Seats</label>
              <input type="number" [(ngModel)]="trip.availableSeats" class="form-control">
            </div>
          </div>
        </div>

        <!-- STEP 3: ITINERARY -->
        <div *ngIf="currentStep === 3">
          <div class="step-title-row">
            <h3>Step 3: Day-by-Day Itinerary</h3>
            <button (click)="addItineraryDay()" class="btn btn-sm btn-outline"><i class="fa-solid fa-plus"></i> Add Day</button>
          </div>

          <div *ngFor="let day of trip.itineraryDays; let i = index" class="itinerary-box">
            <h4>Day {{ i + 1 }}</h4>
            <div class="form-row">
              <div class="form-group col">
                <label class="form-label">Day Title</label>
                <input type="text" [(ngModel)]="day.title" class="form-control" placeholder="Title of day activities">
              </div>
              <div class="form-group col">
                <label class="form-label">Location</label>
                <input type="text" [(ngModel)]="day.location" class="form-control">
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Activities & Schedule</label>
              <textarea [(ngModel)]="day.description" class="form-control" rows="2"></textarea>
            </div>
          </div>
        </div>

        <!-- STEP 4: HOTELS -->
        <div *ngIf="currentStep === 4">
          <div class="step-title-row">
            <h3>Step 4: Hotel Accommodations</h3>
            <button (click)="addHotelRow()" class="btn btn-sm btn-outline"><i class="fa-solid fa-plus"></i> Add Hotel</button>
          </div>

          <div *ngFor="let h of trip.hotels; let i = index" class="itinerary-box">
            <div class="form-row">
              <div class="form-group col">
                <label class="form-label">Hotel</label>
                <select [(ngModel)]="h.hotelId" class="form-control">
                  <option *ngFor="let mh of masterHotels" [value]="mh.id">{{ mh.hotelName }} ({{ mh.location }})</option>
                </select>
              </div>
              <div class="form-group col">
                <label class="form-label">Room Type</label>
                <input type="text" [(ngModel)]="h.roomType" class="form-control" placeholder="e.g. Deluxe Villa">
              </div>
              <div class="form-group col">
                <label class="form-label">Room Count</label>
                <input type="number" [(ngModel)]="h.roomCount" class="form-control">
              </div>
            </div>
          </div>
        </div>

        <!-- STEP 5: TRANSPORT -->
        <div *ngIf="currentStep === 5">
          <div class="step-title-row">
            <h3>Step 5: Vehicles & Transport Fleet</h3>
            <button (click)="addVehicleRow()" class="btn btn-sm btn-outline"><i class="fa-solid fa-plus"></i> Add Vehicle</button>
          </div>

          <div *ngFor="let v of trip.vehicles; let i = index" class="itinerary-box">
            <div class="form-row">
              <div class="form-group col">
                <label class="form-label">Vehicle</label>
                <select [(ngModel)]="v.vehicleId" class="form-control">
                  <option *ngFor="let mv of masterVehicles" [value]="mv.id">{{ mv.vehicleName }} ({{ mv.vehicleType }})</option>
                </select>
              </div>
              <div class="form-group col">
                <label class="form-label">Notes</label>
                <input type="text" [(ngModel)]="v.notes" class="form-control" placeholder="Dedicated bus for all transfers">
              </div>
            </div>
          </div>
        </div>

        <!-- STEP 6: MEALS -->
        <div *ngIf="currentStep === 6">
          <h3>Step 6: Meal Plan & Dietary Options</h3>
          <p class="sub-text mb-3">Configure included dining and dietary choices (Veg, Non-Veg, Jain, Vegan)</p>
          <div class="form-group">
            <label class="form-label">Meal Inclusions Summary</label>
            <input type="text" [(ngModel)]="mealSummaryText" class="form-control" placeholder="e.g. Daily Resort Breakfast + Houseboat Sadhya Lunch Included">
          </div>
        </div>

        <!-- STEP 7: VENDORS -->
        <div *ngIf="currentStep === 7">
          <div class="step-title-row">
            <h3>Step 7: Vendors & Partners</h3>
            <button (click)="addVendorRow()" class="btn btn-sm btn-outline"><i class="fa-solid fa-plus"></i> Add Vendor</button>
          </div>
          <div *ngFor="let v of trip.vendors; let i = index" class="itinerary-box">
            <div class="form-row">
              <div class="form-group col">
                <label class="form-label">Vendor Partner</label>
                <select [(ngModel)]="v.vendorId" class="form-control">
                  <option *ngFor="let mv of masterVendors" [value]="mv.id">{{ mv.vendorName }}</option>
                </select>
              </div>
              <div class="form-group col">
                <label class="form-label">Contract Amount ($)</label>
                <input type="number" [(ngModel)]="v.contractAmount" class="form-control">
              </div>
            </div>
          </div>
        </div>

        <!-- STEP 8: PRICING -->
        <div *ngIf="currentStep === 8">
          <h3>Step 8: Pricing & Deposit Policy</h3>
          <div class="form-row">
            <div class="form-group col">
              <label class="form-label">Base Price Per Passenger ($) *</label>
              <input type="number" [(ngModel)]="trip.basePrice" class="form-control" required>
            </div>
            <div class="form-group col">
              <label class="form-label">Currency</label>
              <input type="text" [(ngModel)]="trip.currency" class="form-control" value="USD">
            </div>
          </div>
        </div>

        <!-- STEP 9: PREVIEW -->
        <div *ngIf="currentStep === 9">
          <h3>Step 9: Trip Package Preview</h3>
          <div class="preview-card card">
            <img [src]="trip.coverImageUrl" class="preview-img">
            <h4>{{ trip.tripName }} ({{ trip.tripCode }})</h4>
            <p><strong>Destination:</strong> {{ trip.destination }} | <strong>Price:</strong> \${{ trip.basePrice }}</p>
            <p>{{ trip.shortDescription }}</p>
          </div>
        </div>

        <!-- STEP 10: PUBLISH -->
        <div *ngIf="currentStep === 10">
          <h3>Step 10: Ready to Publish</h3>
          <p>Your trip package is fully configured! Click below to publish it to your Branded Traveller Portal.</p>
          <button (click)="publishTrip()" class="btn btn-primary btn-lg mt-3">
            <i class="fa-solid fa-globe"></i> Publish to Traveller Portal Now
          </button>
        </div>

        <!-- NAVIGATION FOOTER -->
        <div class="builder-footer">
          <button *ngIf="currentStep > 1" (click)="prevStep()" class="btn btn-secondary">Previous</button>
          <button *ngIf="currentStep < 10" (click)="nextStep()" class="btn btn-primary">Next Step</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    .header-actions { display: flex; gap: 12px; }
    .stepper-header { display: flex; overflow-x: auto; gap: 10px; padding: 14px; margin-bottom: 24px; }
    .stepper-item { display: flex; align-items: center; gap: 6px; padding: 8px 14px; border-radius: 20px; font-size: 0.8rem; font-weight: 700; color: #64748B; cursor: pointer; white-space: nowrap; }
    .stepper-item.active { background: #E3F2FD; color: #1E88E5; }
    .step-num { width: 20px; height: 20px; border-radius: 50%; background: #CBD5E1; color: white; display: flex; align-items: center; justify-content: center; font-size: 0.72rem; }
    .stepper-item.active .step-num { background: #1E88E5; }

    .builder-body-card { padding: 32px; }
    .step-title-row { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
    .itinerary-box { background: #F8FAFC; border: 1px solid #E2E8F0; padding: 16px; border-radius: 10px; margin-bottom: 16px; }
    .form-row { display: flex; gap: 16px; }
    .col { flex: 1; }
    .preview-card { padding: 20px; max-width: 500px; }
    .preview-img { width: 100%; height: 200px; object-fit: cover; border-radius: 10px; margin-bottom: 12px; }
    .builder-footer { display: flex; justify-content: space-between; margin-top: 32px; padding-top: 20px; border-top: 1px solid #E2E8F0; }
  `]
})
export class TripBuilderComponent implements OnInit {
  steps = [
    'Basic Info', 'Dates & Capacity', 'Itinerary', 'Hotels',
    'Transport', 'Meals', 'Vendors', 'Pricing', 'Preview', 'Publish'
  ];
  currentStep = 1;
  isEdit = false;
  tripId = '';

  trip: any = {
    tripCode: '', tripName: '', destination: '', shortDescription: '', description: '',
    startDate: '', endDate: '', durationDays: 5, durationNights: 4, tripType: 1,
    totalCapacity: 20, availableSeats: 20, basePrice: 850, currency: 'USD',
    coverImageUrl: 'https://images.unsplash.com/photo-1602216056096-3b40cc0c9944?w=800&fit=crop',
    itineraryDays: [], hotels: [], vehicles: [], vendors: [], meals: []
  };

  masterHotels: any[] = [];
  masterVehicles: any[] = [];
  masterVendors: any[] = [];
  mealSummaryText = 'Daily Breakfast & Traditional Sadhya Lunch Included';

  constructor(
    private apiService: ApiService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.tripId = this.route.snapshot.paramMap.get('id') || '';
    if (this.tripId && this.tripId !== 'new') {
      this.isEdit = true;
      this.loadTripDetails();
    } else {
      this.addItineraryDay();
    }

    this.apiService.getHotels().subscribe(h => this.masterHotels = h);
    this.apiService.getVehicles().subscribe(v => this.masterVehicles = v);
    this.apiService.getVendors().subscribe(v => this.masterVendors = v);
  }

  loadTripDetails(): void {
    this.apiService.getTrip(this.tripId).subscribe(t => {
      this.trip = { ...t };
      if (t.startDate) this.trip.startDate = t.startDate.substring(0, 10);
      if (t.endDate) this.trip.endDate = t.endDate.substring(0, 10);
    });
  }

  goToStep(step: number): void {
    this.currentStep = step;
  }

  nextStep(): void {
    if (this.currentStep < 10) this.currentStep++;
  }

  prevStep(): void {
    if (this.currentStep > 1) this.currentStep--;
  }

  addItineraryDay(): void {
    this.trip.itineraryDays.push({
      dayNumber: this.trip.itineraryDays.length + 1,
      title: 'Day Title',
      description: 'Day activities details...'
    });
  }

  addHotelRow(): void {
    this.trip.hotels.push({ hotelId: this.masterHotels[0]?.id || '', roomType: 'Standard Double', roomCount: 1 });
  }

  addVehicleRow(): void {
    this.trip.vehicles.push({ vehicleId: this.masterVehicles[0]?.id || '', notes: 'AC Coach' });
  }

  addVendorRow(): void {
    this.trip.vendors.push({ vendorId: this.masterVendors[0]?.id || '', contractAmount: 500 });
  }

  saveDraft(): void {
    if (this.isEdit) {
      this.apiService.updateTrip(this.tripId, this.trip).subscribe(() => {
        alert('Trip package saved as draft!');
      });
    } else {
      this.apiService.createTrip(this.trip).subscribe(t => {
        alert('Trip package created as draft!');
        this.router.navigate(['/trips']);
      });
    }
  }

  publishTrip(): void {
    if (!this.trip.id) {
      this.apiService.createTrip(this.trip).subscribe(t => {
        this.apiService.publishTrip(t.id).subscribe(() => {
          alert('Trip package published to Traveller Portal!');
          this.router.navigate(['/trips']);
        });
      });
    } else {
      this.apiService.publishTrip(this.trip.id).subscribe(() => {
        alert('Trip package published to Traveller Portal!');
        this.router.navigate(['/trips']);
      });
    }
  }
}
