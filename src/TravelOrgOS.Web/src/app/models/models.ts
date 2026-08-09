export enum UserRole {
  PlatformAdmin = 1,
  OrganizationOwner = 2,
  OrganizationAdmin = 3,
  TripCoordinator = 4,
  FinanceUser = 5,
  Traveller = 6
}

export enum TripType {
  Leisure = 1,
  Adventure = 2,
  Family = 3,
  Pilgrimage = 4,
  Corporate = 5,
  Honeymoon = 6,
  GroupTour = 7,
  WeekendGetaway = 8,
  Custom = 9
}

export enum TripStatus {
  Draft = 1,
  Published = 2,
  RegistrationOpen = 3,
  AlmostFull = 4,
  FullyBooked = 5,
  InProgress = 6,
  Completed = 7,
  Cancelled = 8
}

export enum PaymentStatus {
  Pending = 1,
  PartiallyPaid = 2,
  Paid = 3,
  Refunded = 4
}

export enum BookingStatus {
  Pending = 1,
  Confirmed = 2,
  Cancelled = 3,
  Completed = 4
}

export interface UserSession {
  token: string;
  userId: string;
  fullName: string;
  email: string;
  role: UserRole;
  organizationId?: string;
  organizationName?: string;
  organizationSlug?: string;
  primaryColor?: string;
  secondaryColor?: string;
  logoUrl?: string;
}

export interface Organization {
  id: string;
  name: string;
  slug: string;
  legalName?: string;
  logoUrl?: string;
  primaryColor: string;
  secondaryColor: string;
  welcomeMessage?: string;
  email: string;
  phone: string;
  website?: string;
  address?: string;
  city?: string;
  country?: string;
  businessHours?: string;
  description?: string;
  facebookUrl?: string;
  instagramUrl?: string;
  linkedInUrl?: string;
  whatsAppNumber?: string;
  status: boolean;
}

export interface Traveller {
  id: string;
  organizationId: string;
  firstName: string;
  lastName: string;
  email: string;
  mobileNumber: string;
  dateOfBirth?: string;
  gender?: string;
  nationality?: string;
  passportNumber?: string;
  passportExpiry?: string;
  emergencyContactName?: string;
  emergencyContactNumber?: string;
  address?: string;
  city?: string;
  country?: string;
  notes?: string;
  status: boolean;
  createdAt: string;
}

export interface ItineraryDay {
  id?: string;
  dayNumber: number;
  date?: string;
  title: string;
  description: string;
  location?: string;
  activities?: string;
  startTime?: string;
  endTime?: string;
  notes?: string;
}

export interface TripHotel {
  id?: string;
  hotelId: string;
  hotelName: string;
  location: string;
  roomType: string;
  checkIn: string;
  checkOut: string;
  roomCount: number;
  notes?: string;
}

export interface TripVehicle {
  id?: string;
  vehicleId: string;
  vehicleName: string;
  vehicleType: string;
  capacity: number;
  driverName?: string;
  driverPhone?: string;
  notes?: string;
}

export interface TripVendor {
  id?: string;
  vendorId: string;
  vendorName: string;
  vendorType: number;
  contractAmount: number;
  serviceDescription?: string;
}

export interface TripMeal {
  id?: string;
  mealType: number;
  mealOption: number;
  description?: string;
  dietaryOptions?: string;
}

export interface Trip {
  id: string;
  organizationId: string;
  tripCode: string;
  tripName: string;
  shortDescription: string;
  description: string;
  destination: string;
  startLocation: string;
  endLocation: string;
  viaRoute?: string;
  startDate: string;
  endDate: string;
  durationDays: number;
  durationNights: number;
  tripType: TripType;
  status: TripStatus;
  visibility: number;
  coverImageUrl: string;
  basePrice: number;
  currency: string;
  totalCapacity: number;
  availableSeats: number;
  bookedSeats?: number;
  minimumTravellers: number;
  maximumTravellers: number;
  hostGuide?: string;
  driverName?: string;
  estimatedCost?: number;
  grossRevenue?: number;
  netProfit?: number;
  contactPerson?: string;
  contactNumber?: string;
  createdAt: string;
  publishedAt?: string;
  itineraryDays: ItineraryDay[];
  hotels: TripHotel[];
  vehicles: TripVehicle[];
  vendors: TripVendor[];
  meals: TripMeal[];
  guides?: any[];
}

export interface BookingTraveller {
  travellerId: string;
  fullName: string;
  email: string;
  mobileNumber: string;
  roomPreference: string;
  dietaryPreference: string;
}

export interface Payment {
  id: string;
  bookingId: string;
  amount: number;
  paymentMethod: string;
  transactionReference: string;
  provider?: string;
  providerTransactionId?: string;
  providerEventId?: string;
  currency?: string;
  paymentType?: string;
  status: PaymentStatus;
  paymentDate: string;
  completedAt?: string;
  failureReason?: string;
  notes?: string;
}

export interface PaymentCheckoutSession {
  provider: string;
  paymentType: string;
  bookingId: string;
  bookingReference: string;
  amount: number;
  currency: string;
  transactionReference: string;
  checkoutUrl?: string;
  providerOrderId?: string;
  publishableKey?: string;
  message: string;
}

export interface Booking {
  id: string;
  organizationId: string;
  tripId: string;
  tripName: string;
  tripCode: string;
  bookingReference: string;
  bookingDate: string;
  numberOfTravellers: number;
  totalAmount: number;
  paidAmount: number;
  balanceAmount: number;
  paymentStatus: PaymentStatus;
  bookingStatus: BookingStatus;
  contactEmail: string;
  contactPhone: string;
  specialRequests?: string;
  travellers: BookingTraveller[];
  payments: Payment[];
}

export interface DashboardSummary {
  totalTrips: number;
  activeTrips: number;
  totalTravellers: number;
  totalBookings: number;
  confirmedBookings: number;
  totalRevenue: number;
  pendingPayments: number;
  outstandingBalance: number;
  upcomingTrips: {
    id: string;
    tripCode: string;
    tripName: string;
    destination: string;
    startDate: string;
    totalCapacity: number;
    bookedSeats: number;
    availableSeats: number;
    status: TripStatus;
  }[];
  recentBookings: {
    id: string;
    bookingReference: string;
    tripName: string;
    customerName: string;
    passengers: number;
    totalAmount: number;
    paymentStatus: PaymentStatus;
    bookingStatus: BookingStatus;
    bookingDate: string;
  }[];
  bookingTrends: {
    monthLabel: string;
    bookingCount: number;
    revenue: number;
  }[];
}
