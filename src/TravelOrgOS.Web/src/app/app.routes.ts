import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { AdminLayoutComponent } from './components/admin-layout/admin-layout.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { TravellersComponent } from './components/travellers/travellers.component';
import { TripsListComponent } from './components/trips/trips-list.component';
import { TripBuilderComponent } from './components/trips/trip-builder.component';
import { BookingsListComponent } from './components/bookings/bookings-list.component';
import { ReportsComponent } from './components/reports/reports.component';
import { SettingsComponent } from './components/settings/settings.component';

import { PortalHomeComponent } from './components/portal/portal-home.component';
import { PortalTripDetailComponent } from './components/portal/portal-trip-detail.component';
import { PortalBookingComponent } from './components/portal/portal-booking.component';
import { PortalBookingSuccessComponent } from './components/portal/portal-booking-success.component';
import { PortalPaymentReturnComponent } from './components/portal/portal-payment-return.component';

import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },

  // PUBLIC TRAVELLER PORTAL ROUTES
  { path: 'portal/:organizationSlug', component: PortalHomeComponent },
  { path: 'portal/:organizationSlug/trips', component: PortalHomeComponent },
  { path: 'portal/:organizationSlug/trips/:tripId', component: PortalTripDetailComponent },
  { path: 'portal/:organizationSlug/book/:tripId', component: PortalBookingComponent },
  { path: 'portal/:organizationSlug/booking-success/:bookingId', component: PortalBookingSuccessComponent },
  { path: 'portal/:organizationSlug/payment-return', component: PortalPaymentReturnComponent },

  // ADMIN DASHBOARD ROUTES
  {
    path: '',
    component: AdminLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'trips', component: TripsListComponent },
      { path: 'trips/new', component: TripBuilderComponent },
      { path: 'trips/:id', component: TripBuilderComponent },
      { path: 'travellers', component: TravellersComponent },
      { path: 'bookings', component: BookingsListComponent },
      { path: 'reports', component: ReportsComponent },
      { path: 'settings', component: SettingsComponent }
    ]
  },

  { path: '**', redirectTo: 'login' }
];
