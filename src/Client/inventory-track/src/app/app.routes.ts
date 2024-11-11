import { Routes } from '@angular/router';
import { LoginComponent } from "./components/login/login.component";
import {RegisterComponent} from "./components/register/register.component";
import {HomeComponent} from "./components/home/home.component";
import {UserManagementComponent} from "./components/user-managment/user-managment.component";

export const routes: Routes = [
  { path: '', component: LoginComponent }, // Убираем слэш перед пустым путем
  { path: 'login', component: LoginComponent }, // Убираем слэш перед 'login'
  { path: 'register', component: RegisterComponent },
  {path: 'home', component: HomeComponent },
  {path:'workers',component:UserManagementComponent}
];
