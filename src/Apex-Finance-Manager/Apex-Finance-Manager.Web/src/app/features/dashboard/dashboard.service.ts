import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AuthService } from '../auth/auth.service';

// Define the shape of your data
export interface Transaction {
  id: number;
  description: string;
  category: string;
  date: string;
  amount: number;
  type: 'income' | 'expense';
}

export interface DashboardData {
  totalBalance: number;
  monthlySpend: number;
  monthlyBudgetLimit: number;
  savingsGoal: number;
  userFirstName: string;
  userLastName: string;
  recentTransactions: Transaction[];
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
    private authService = inject(AuthService); // Injected
  private apiUrl = 'https://localhost:7001/api/dashboard'; // Replace with your actual ASP.NET API URL

  getDashboardData(): Observable<DashboardData> {
    // In a real app, use the http client:
    // return this.http.get<DashboardData>(this.apiUrl);

    // For now, return mock data instantly
    const mockData: DashboardData = {
      totalBalance: 142500.00,
      monthlySpend: 12340.50,
      monthlyBudgetLimit: 16500.00,
      savingsGoal: 45000.00,
      userFirstName: this.authService.currentUserFirstName(),
      userLastName: this.authService.currentUserLastName(),

      recentTransactions: [
        { id: 1, description: 'Dubai Mall - Nike', category: 'Shopping', date: '2026-01-14', amount: 850.00, type: 'expense' },
        { id: 2, description: 'Salary Deposit', category: 'Income', date: '2026-01-01', amount: 35000.00, type: 'income' },
        // ... more transactions
      ]
    };
    
    // Use 'of(mockData)' to return the mock data wrapped in an Observable immediately
    return of(mockData); 
  }
}
