import { Injectable, signal, inject, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { Router } from '@angular/router';

interface AuthResponse {
  token: string;
  firstName: string;
  lastName: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  // Ensure this port matches your Visual Studio API port!
  private apiUrl = 'https://localhost:7001/api/auth'; 

  currentUserFirstName = signal<any | null>(null);
  currentUserLastName = signal<any | null>(null);

  isAuthenticated = computed(() => !!this.currentUserFirstName());

  // login(credentials: any) {
  //   return this.http.post(`${this.apiUrl}/login`, credentials).pipe(
  //     tap((user: any) => {
  //       localStorage.setItem('token', user.token);
  //       this.currentUser.set(user);
  //     })
  //   );
  // }
  login(credentials: any) {
    // We expect the API to return the AuthResponse shape including 'userName'
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        // Store the username in the signal upon successful login
        this.currentUserFirstName.set(response.firstName);
        this.currentUserLastName.set(response.lastName);
        // Store token in localStorage
        localStorage.setItem('apex_token', response.token);
      })
    );
  }

  logout() {
    this.currentUserFirstName.set(null); // Clear user data
    this.currentUserLastName.set(null);
    localStorage.removeItem('apex_token');
    this.router.navigate(['/login']);
  }
  isLoggedIn(): boolean {
    // Simple check based on the token presence
    return !!localStorage.getItem('apex_token');
  }
}
