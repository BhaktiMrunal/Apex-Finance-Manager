import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService, DashboardData } from './dashboard.service'; // Import the service
import { AuthService } from '../auth/auth.service';
import { ChartConfiguration, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule,BaseChartDirective],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private authService = inject(AuthService); // Assuming AuthService has a logout method

  // Signals for robust state management
  dashboardData = signal<DashboardData | null>(null);
  isLoading = signal(true);
  error = signal<string | null>(null);

   // Line Chart Configuration for Spending Trends
  public lineChartData: ChartConfiguration['data'] = {
    datasets: [
      {
        data: [2500, 3200, 2800, 4500, 3900, 5200], // Example data
        label: 'Monthly Spending (AED)',
        backgroundColor: 'rgba(234, 179, 8, 0.1)', // Light Gold
        borderColor: '#eab308', // Apex Gold
        pointBackgroundColor: '#1a2b4b',
        pointBorderColor: '#fff',
        fill: 'origin',
        tension: 0.4 // Makes the line curved and premium
      }
    ],
    labels: ['Aug', 'Sep', 'Oct', 'Nov', 'Dec', 'Jan']
  };

  public lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false } // We keep it clean
    },
    scales: {
      y: { grid: { display: false } },
      x: { grid: { display: false } }
    }
  };
  
  ngOnInit(): void {
    this.fetchDashboardData();
  }

  fetchDashboardData(): void {
    this.isLoading.set(true);
    // Use the service to fetch mock or real data
    this.dashboardService.getDashboardData().subscribe({
      next: (data) => {
        this.dashboardData.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.error.set('Failed to load dashboard data.');
        this.isLoading.set(false);
      }
    });
  }

  onLogout(): void {
    // Implement your logout logic here
    this.authService.logout();
    // Navigate back to login
  }
}
