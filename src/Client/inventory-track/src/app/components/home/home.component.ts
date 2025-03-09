import { Component, OnInit } from "@angular/core";
import { TokenService } from "../../services/token.service";
import { Router } from "@angular/router";
import { CommonModule, NgIf } from "@angular/common";
import { WarehouseStatesListComponent } from "../warehouses/warehouses-list/warehouses-list.component";
import { WarehouseViewComponent } from "../warehouses/warehouse-view/warehouse-view.component";
import { UserResponseDTO } from "../../models/dto/user/user-response-dto";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  imports: [
    NgIf,
    CommonModule,
    WarehouseStatesListComponent,
    WarehouseViewComponent
  ],
  standalone: true
})
export class HomeComponent implements OnInit {
  userRoles: string[] = [];
  isAccountant = false;
  user: UserResponseDTO | null = null;

  constructor(
    private tokenService: TokenService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userRoles = this.tokenService.getUserRoles();
    this.isAccountant = this.userRoles.includes("Accountant");
  }
}
