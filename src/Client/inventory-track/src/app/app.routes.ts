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
import {
  CreateInventoryItemComponent
} from "./components/inventory-items/create-inventory-item/create-inventory-item.component";
import {
  CreateMovementRequestComponent
} from "./components/movements/create-movement-request/create-movement-request.component";
import {WarehouseViewComponent} from "./components/warehouses/warehouse-view/warehouse-view.component";
import {WriteOffCreateComponent} from "./components/write-offs/ writeoff-create/writeoff-create.component";
import {MyRequestsComponent} from "./components/my-requests/my-requests.component";

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
  {path:'warehouse/:id' , component:WarehouseItemsTableComponent, canActivate: [AuthGuard]},
  {path:'create-item',component:CreateInventoryItemComponent,canActivate: [AuthGuard] },
  {path: 'create-movement' , component: CreateMovementRequestComponent, canActivate : [AuthGuard]},
  {path: 'warehouse-view' , component: WarehouseViewComponent, canActivate: [AuthGuard]},
  {path:'create-write-off' , component: WriteOffCreateComponent , canActivate: [AuthGuard]},
  {path: 'my-requests',component: MyRequestsComponent,canActivate:[AuthGuard]},
];
