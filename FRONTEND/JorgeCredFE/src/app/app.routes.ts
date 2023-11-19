import { Routes } from '@angular/router';
import { LoginComponent } from './views/login/login.component';
import { DashboardComponent } from './views/dashboard/dashboard.component';

export const routes: Routes = [
  {
    component: DashboardComponent,
    path: 'dashboard'
  },
  {
    component: LoginComponent,
    path: 'login'
  },
  {
    component: LoginComponent,
    path: ''
  }
];
