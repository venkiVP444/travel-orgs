import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { UserSession } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  currentSession = signal<UserSession | null>(this.getStoredSession());

  constructor(private router: Router) {}

  private getStoredSession(): UserSession | null {
    const raw = localStorage.getItem('tos_session');
    if (!raw) return null;
    try {
      return JSON.parse(raw);
    } catch {
      return null;
    }
  }

  setSession(session: UserSession): void {
    localStorage.setItem('tos_session', JSON.stringify(session));
    this.currentSession.set(session);
  }

  logout(): void {
    localStorage.removeItem('tos_session');
    this.currentSession.set(null);
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return this.currentSession() !== null;
  }
}
