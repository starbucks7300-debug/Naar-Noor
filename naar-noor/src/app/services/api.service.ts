import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MenuItem {
  id: string;
  name: string;
  description: string;
  price: number;
  category: string;
  isVegetarian: boolean;
  isVegan: boolean;
  isGlutenFree: boolean;
  isAvailable: boolean;
  imageUrl: string | null;
  sortOrder: number;
}

export interface Chef {
  id: string;
  name: string;
  title: string;
  bio: string;
  imageUrl: string | null;
  specialty: string;
  sortOrder: number;
}

export interface Review {
  id: string;
  customerName: string;
  rating: number;
  comment: string;
  source: string | null;
  createdAt: string;
}

export interface CreateReservationRequest {
  customerName: string;
  email: string;
  phoneNumber: string;
  reservationDate: string;
  reservationTime: string;
  partySize: number;
  specialRequests?: string;
}

export interface CreateReservationResponse {
  id: string;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api';

  getMenu(category?: string): Observable<MenuItem[]> {
    const url = category
      ? `${this.base}/menu?category=${encodeURIComponent(category)}`
      : `${this.base}/menu`;
    return this.http.get<MenuItem[]>(url);
  }

  getChefs(): Observable<Chef[]> {
    return this.http.get<Chef[]>(`${this.base}/chefs`);
  }

  getReviews(): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.base}/reviews`);
  }

  createReservation(data: CreateReservationRequest): Observable<CreateReservationResponse> {
    return this.http.post<CreateReservationResponse>(`${this.base}/reservations`, data);
  }
}
