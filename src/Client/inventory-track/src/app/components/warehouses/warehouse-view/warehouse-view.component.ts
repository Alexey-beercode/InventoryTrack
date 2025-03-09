import { Component, OnInit } from '@angular/core';
import { WarehouseStateResponseDto } from '../../../models/dto/warehouse/warehouse-state-response-dto';
import { WarehouseService } from '../../../services/warehouse.service';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from "@angular/router";
import { TokenService } from '../../../services/token.service';

@Component({
  selector: 'app-warehouse-view',
  templateUrl: './warehouse-view.component.html',
  styleUrls: ['./warehouse-view.component.css'],
  standalone: true,
  imports: [HeaderComponent, FooterComponent, LoadingSpinnerComponent, CommonModule, RouterLink],
})
export class WarehouseViewComponent implements OnInit {
  warehouseStates: WarehouseStateResponseDto[] = [];
  isLoading = false;
  companyId: string = '';
  warehouseId: string = '';
  userRoles: string[] = [];

  constructor(
    private warehouseService: WarehouseService,
    private tokenService: TokenService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  onCompanyIdReceived(companyId: string): void {
    if (companyId) {
      this.companyId = companyId;
      this.loadWarehouseStates();
    }
  }

  onUserReceived(user: { id: string, warehouseId?: string } | null): void {
    if (!user) return;

    this.warehouseId = user.warehouseId || '';
    this.userRoles = this.tokenService.getUserRoles();

    // 🚀 Если пользователь — начальник склада, перенаправляем его на страницу конкретного склада
    if (this.userRoles.includes('Warehouse Manager') && this.warehouseId) {
      this.router.navigate(['/warehouse', this.warehouseId]);
    }
  }

  loadWarehouseStates(): void {
    if (!this.companyId) return;

    this.isLoading = true;
    this.warehouseService.getWarehousesStatesByCompany(this.companyId).subscribe({
      next: (data) => {
        this.warehouseStates = data;
        this.isLoading = false;
      },
      error: () => {
        this.warehouseStates = [];
        this.isLoading = false;
      },
    });
  }
}
