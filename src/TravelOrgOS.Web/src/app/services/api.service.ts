import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Booking,
  DashboardSummary,
  Organization,
  Traveller,
  Trip,
  ItineraryDay,
  TripHotel,
  TripVehicle,
  TripVendor,
  TripMeal,
  UserSession,
  PaymentCheckoutSession
} from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'http://localhost:5100/api';

  constructor(private http: HttpClient) {}

  private getAuthHeaders(): HttpHeaders {
    const sessionStr = localStorage.getItem('tos_session');
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    if (sessionStr) {
      const session: UserSession = JSON.parse(sessionStr);
      if (session.token) {
        headers = headers.set('Authorization', `Bearer ${session.token}`);
      }
    }
    return headers;
  }

  // AUTH
  login(credentials: { email: string; password: string }): Observable<UserSession> {
    return this.http.post<UserSession>(`${this.baseUrl}/auth/login`, credentials);
  }

  // DASHBOARD
  getDashboardSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${this.baseUrl}/dashboard`, { headers: this.getAuthHeaders() });
  }

  // TRAVELLERS
  getTravellers(search?: string): Observable<Traveller[]> {
    const url = search ? `${this.baseUrl}/travellers?search=${encodeURIComponent(search)}` : `${this.baseUrl}/travellers`;
    return this.http.get<Traveller[]>(url, { headers: this.getAuthHeaders() });
  }

  getTraveller(id: string): Observable<Traveller> {
    return this.http.get<Traveller>(`${this.baseUrl}/travellers/${id}`, { headers: this.getAuthHeaders() });
  }

  createTraveller(traveller: any): Observable<Traveller> {
    return this.http.post<Traveller>(`${this.baseUrl}/travellers`, traveller, { headers: this.getAuthHeaders() });
  }

  updateTraveller(id: string, traveller: any): Observable<Traveller> {
    return this.http.put<Traveller>(`${this.baseUrl}/travellers/${id}`, traveller, { headers: this.getAuthHeaders() });
  }

  deleteTraveller(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/travellers/${id}`, { headers: this.getAuthHeaders() });
  }

  importTravellerCsv(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    const sessionStr = localStorage.getItem('tos_session');
    let headers = new HttpHeaders();
    if (sessionStr) {
      const session: UserSession = JSON.parse(sessionStr);
      headers = headers.set('Authorization', `Bearer ${session.token}`);
    }

    return this.http.post<any>(`${this.baseUrl}/travellers/import`, formData, { headers });
  }

  downloadTravellerCsvTemplateUrl(): string {
    return `${this.baseUrl}/travellers/import/template`;
  }

  // TRIPS
  getTrips(search?: string, status?: number, publicOnly: boolean = false): Observable<Trip[]> {
    let url = `${this.baseUrl}/trips?publicOnly=${publicOnly}`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    if (status) url += `&status=${status}`;
    return this.http.get<Trip[]>(url, { headers: this.getAuthHeaders() });
  }

  getTrip(id: string): Observable<Trip> {
    return this.http.get<Trip>(`${this.baseUrl}/trips/${id}`, { headers: this.getAuthHeaders() });
  }

  createTrip(trip: any): Observable<Trip> {
    return this.http.post<Trip>(`${this.baseUrl}/trips`, trip, { headers: this.getAuthHeaders() });
  }

  updateTrip(id: string, trip: any): Observable<Trip> {
    return this.http.put<Trip>(`${this.baseUrl}/trips/${id}`, trip, { headers: this.getAuthHeaders() });
  }

  publishTrip(id: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/trips/${id}/publish`, {}, { headers: this.getAuthHeaders() });
  }

  unpublishTrip(id: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/trips/${id}/unpublish`, {}, { headers: this.getAuthHeaders() });
  }

  duplicateTrip(id: string): Observable<Trip> {
    return this.http.post<Trip>(`${this.baseUrl}/trips/${id}/duplicate`, {}, { headers: this.getAuthHeaders() });
  }

  deleteTrip(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/trips/${id}`, { headers: this.getAuthHeaders() });
  }

  // TRIP BUILDER STEPS
  saveItinerary(tripId: string, days: ItineraryDay[]): Observable<ItineraryDay[]> {
    return this.http.post<ItineraryDay[]>(`${this.baseUrl}/trips/${tripId}/itinerary`, days, { headers: this.getAuthHeaders() });
  }

  saveHotels(tripId: string, hotels: TripHotel[]): Observable<TripHotel[]> {
    return this.http.post<TripHotel[]>(`${this.baseUrl}/trips/${tripId}/hotels`, hotels, { headers: this.getAuthHeaders() });
  }

  saveVehicles(tripId: string, vehicles: TripVehicle[]): Observable<TripVehicle[]> {
    return this.http.post<TripVehicle[]>(`${this.baseUrl}/trips/${tripId}/vehicles`, vehicles, { headers: this.getAuthHeaders() });
  }

  saveVendors(tripId: string, vendors: TripVendor[]): Observable<TripVendor[]> {
    return this.http.post<TripVendor[]>(`${this.baseUrl}/trips/${tripId}/vendors`, vendors, { headers: this.getAuthHeaders() });
  }

  saveMeals(tripId: string, meals: TripMeal[]): Observable<TripMeal[]> {
    return this.http.post<TripMeal[]>(`${this.baseUrl}/trips/${tripId}/meals`, meals, { headers: this.getAuthHeaders() });
  }

  // BOOKINGS
  getBookings(search?: string): Observable<Booking[]> {
    const url = search ? `${this.baseUrl}/bookings?search=${encodeURIComponent(search)}` : `${this.baseUrl}/bookings`;
    return this.http.get<Booking[]>(url, { headers: this.getAuthHeaders() });
  }

  getBooking(id: string): Observable<Booking> {
    return this.http.get<Booking>(`${this.baseUrl}/bookings/${id}`, { headers: this.getAuthHeaders() });
  }

  getBookingByReference(reference: string): Observable<Booking> {
    return this.http.get<Booking>(`${this.baseUrl}/bookings/ref/${reference}`);
  }

  confirmBooking(id: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/bookings/${id}/confirm`, {}, { headers: this.getAuthHeaders() });
  }

  cancelBooking(id: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/bookings/${id}/cancel`, {}, { headers: this.getAuthHeaders() });
  }

  recordPayment(id: string, paymentData: any): Observable<Booking> {
    return this.http.post<Booking>(`${this.baseUrl}/bookings/${id}/payment`, paymentData, { headers: this.getAuthHeaders() });
  }

  initiatePayment(bookingId: string, provider: string, paymentType: string, amountToPay?: number): Observable<PaymentCheckoutSession> {
    return this.http.post<PaymentCheckoutSession>(`${this.baseUrl}/bookings/${bookingId}/pay`, { bookingId, provider, paymentType, amountToPay }, { headers: this.getAuthHeaders() });
  }

  initiatePortalPayment(slug: string, bookingId: string, provider: string, paymentType: string, amountToPay?: number): Observable<PaymentCheckoutSession> {
    return this.http.post<PaymentCheckoutSession>(`${this.baseUrl}/bookings/portal/${slug}/${bookingId}/pay`, { bookingId, provider, paymentType, amountToPay });
  }

  getBookingPaymentStatus(bookingId: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/bookings/${bookingId}/payment-status`);
  }

  verifyRazorpayPayment(verificationDto: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/bookings/payment/verify-razorpay`, verificationDto);
  }

  // PORTAL ENDPOINTS
  getPortalTrips(slug: string): Observable<{ organization: Organization; trips: Trip[] }> {
    return this.http.get<{ organization: Organization; trips: Trip[] }>(`${this.baseUrl}/trips/portal/${slug}`);
  }

  getPortalTrip(slug: string, tripId: string): Observable<Trip> {
    return this.http.get<Trip>(`${this.baseUrl}/trips/portal/${slug}/${tripId}`);
  }

  createPortalBooking(slug: string, bookingDto: any): Observable<Booking> {
    return this.http.post<Booking>(`${this.baseUrl}/bookings/portal/${slug}`, bookingDto);
  }

  // MASTER DATA
  getOrganizationMe(): Observable<Organization> {
    return this.http.get<Organization>(`${this.baseUrl}/organizations/me`, { headers: this.getAuthHeaders() });
  }

  updateOrganizationMe(org: any): Observable<Organization> {
    return this.http.put<Organization>(`${this.baseUrl}/organizations/me`, org, { headers: this.getAuthHeaders() });
  }

  getHotels(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/hotels`, { headers: this.getAuthHeaders() });
  }

  getVehicles(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/vehicles`, { headers: this.getAuthHeaders() });
  }

  getVendors(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/vendors`, { headers: this.getAuthHeaders() });
  }

  getNotifications(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/notifications`, { headers: this.getAuthHeaders() });
  }

  // DEMO RESET
  resetDemoData(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/demo/reset`, {}, { headers: this.getAuthHeaders() });
  }
}
