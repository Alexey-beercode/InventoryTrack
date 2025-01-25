import { Component, OnInit } from '@angular/core';
import { WarehouseStateResponseDto } from '../../../models/dto/warehouse/warehouse-state-response-dto';
import { WarehouseService } from '../../../services/warehouse.service';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { CreateWarehouseComponent } from '../create-warehouse/create-warehouse.component';
import { CommonModule } from '@angular/common';
import {RouterLink} from "@angular/router";

@Component({
  selector: 'app-warehouse-states-list',
  templateUrl: './warehouses-list.component.html',
  styleUrls: ['./warehouses-list.component.css'],
  standalone: true,
  imports: [HeaderComponent, FooterComponent, LoadingSpinnerComponent, CreateWarehouseComponent, CommonModule, RouterLink],
})
export class WarehouseStatesListComponent implements OnInit {
  warehouseStates: WarehouseStateResponseDto[] = [];
  isLoading = false;
  companyId: string = ''; // Убираем `null`, чтобы работать с пустой строкой
  userId: string = ''; // Убираем `null`, чтобы работать с пустой строкой
  showCreateModal = false;

  constructor(private warehouseService: WarehouseService) {}

  ngOnInit(): void {}

  onCompanyIdReceived(companyId: string): void {
    if (companyId) {
      this.companyId = companyId;
      this.loadWarehouseStates();
    }
  }

  onUserReceived(user: { id: string } | null): void {
    if (user?.id) {
      this.userId = user.id;
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

  openCreateModal(): void {
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
    this.loadWarehouseStates();
  }
}
