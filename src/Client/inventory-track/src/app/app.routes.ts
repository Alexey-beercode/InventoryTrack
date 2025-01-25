import { Routes } from '@angular/router';
import { LoginComponent } from "./components/login/login.component";
import {RegisterComponent} from "./components/register/register.component";
import {HomeComponent} from "./components/home/home.component";
import {UserManagementComponent} from "./components/users/user-managment/user-managment.component";
import {AuthGuard} from "./guards/auth.guard";
import {SupplierListComponent} from "./components/suppliers/suppliers-list/suppliers-list.component";
import {WarehouseStatesListComponent} from "./components/warehouses/warehouses-list/warehouses-list.component";
import {UserDetailsComponent} from "./components/users/user-details/user-details.component";
import {ReportPageComponent} from "./components/reports/report-page/report-page.component";
import {WriteOffListComponent} from "./components/write-offs/writeoff-list/writeoff-list.component";
import {MovementListComponent} from "./components/movements/movement-list/movement-list.component";
import {
  WarehouseItemsTableComponent
} from "./components/warehouses/warehouse-items-table/warehouse-items-table.component";

export const routes: Routes = [
  { path: '', component: HomeComponent,canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {path: 'home', component: HomeComponent ,canActivate: [AuthGuard] },
  {path:'workers',component:UserManagementComponent,canActivate: [AuthGuard] },
  {path:'suppliers',component:SupplierListComponent,canActivate: [AuthGuard] },
  {path: 'warehouses',component:WarehouseStatesListComponent,canActivate: [AuthGuard] },
  { path: 'user-details', component: UserDetailsComponent, canActivate :[AuthGuard] },
  {path: 'reports', component:ReportPageComponent,canActivate :[AuthGuard]},
  {path: 'write-offs',component : WriteOffListComponent,canActivate : [AuthGuard]},
  {path:'movements',component:MovementListComponent, canActivate: [AuthGuard]},
  {path:'warehouse/:id' , component:WarehouseItemsTableComponent, canActivate: [AuthGuard]}
];
