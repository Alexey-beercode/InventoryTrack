import { Component, OnInit } from '@angular/core';
import { WarehouseStateResponseDto } from '../../../models/dto/warehouse/warehouse-state-response-dto';
import { WarehouseService } from '../../../services/warehouse.service';
import { UserService } from '../../../services/user.service';
import { RoleService } from '../../../services/role.service';
import { UserResponseDTO } from "../../../models/dto/user/user-response-dto";
import { UpdateWarehouseDto } from "../../../models/dto/warehouse/update-warehouse-dto";
import {RouterLink} from "@angular/router";
import {NgForOf, NgIf} from "@angular/common";
import {LoadingSpinnerComponent} from "../../shared/loading-spinner/loading-spinner.component";
import {HeaderComponent} from "../../shared/header/header.component";
import {FormsModule} from "@angular/forms";
import {FooterComponent} from "../../shared/footer/footer.component";
import {CreateWarehouseComponent} from "../create-warehouse/create-warehouse.component";

@Component({
  selector: 'app-warehouse-states-list',
  templateUrl: './warehouses-list.component.html',
  styleUrls: ['./warehouses-list.component.css'],
  standalone: true,
  imports: [
    RouterLink,
    NgIf,
    LoadingSpinnerComponent,
    NgForOf,
    HeaderComponent,
    FormsModule,
    FooterComponent,
    CreateWarehouseComponent
  ]
})
export class WarehouseStatesListComponent implements OnInit {
  warehouseStates: WarehouseStateResponseDto[] = [];
  users: UserResponseDTO[] = [];
  isLoading = false;
  companyId: string = '';
  userId: string = '';
  showCreateModal = false;
  showAssignManagerModal = false;
  showDeleteConfirmModal = false;
  selectedWarehouseId: string | null = null;
  selectedManagerId: string | null = null;
  selectedWarehouseName: string = '';
  filteredUsers: UserResponseDTO[] = [];
  errorMessage: string | null = null;

  constructor(
    private warehouseService: WarehouseService,
    private userService: UserService,
    private roleService: RoleService
  ) {
  }

  ngOnInit(): void {
  }

  onCompanyIdReceived(companyId: string): void {
    if (companyId) {
      this.companyId = companyId;
      this.loadWarehouseStates();
      this.loadUsers();
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

  loadUsers(): void {
    if (!this.companyId) return;

    this.userService.getByCompanyId(this.companyId).subscribe({
      next: (users) => {
        this.users = users;
      },
      error: () => {
        this.errorMessage = 'Ошибка загрузки пользователей';
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

  openDeleteConfirmModal(warehouseId: string, warehouseName: string): void {
    this.selectedWarehouseId = warehouseId;
    this.selectedWarehouseName = warehouseName;
    this.showDeleteConfirmModal = true;
  }

  deleteWarehouse(): void {
    if (!this.selectedWarehouseId) return;

    this.warehouseService.deleteWarehouse(this.selectedWarehouseId).subscribe({
      next: () => {
        this.loadWarehouseStates();
        this.showDeleteConfirmModal = false;
      },
      error: () => {
        this.errorMessage = 'Ошибка удаления склада.';
      },
    });
  }

  assignManager(warehouseId: string, warehouseType: string): void {

    this.selectedWarehouseId = warehouseId;
    this.selectedManagerId = null; // Очистка перед открытием
    this.filteredUsers = this.users.filter(user =>
      (warehouseType === 'Производственный' && user.roles.some(role => role.name === 'Начальник подразделения')) ||
      (warehouseType === 'Внутренний' && user.roles.some(role => role.name === 'Начальник склада'))
    );

    this.errorMessage = this.filteredUsers.length === 0 ? "Нет доступных пользователей для назначения." : null;

    this.showAssignManagerModal = true;
  }


  confirmAssignManager(): void {
    if (!this.selectedWarehouseId || !this.selectedManagerId) return;

    const warehouse = this.warehouseStates.find(w => w.id === this.selectedWarehouseId);
    if (!warehouse) return;

    const dto: UpdateWarehouseDto = {
      id: warehouse.id,
      name: warehouse.name,
      type: warehouse.type.value,
      location: warehouse.location,
      responsiblePersonId: this.selectedManagerId,
      companyId: this.companyId,
      accountantId: this.userId
    };

    this.warehouseService.updateWarehouse(dto).subscribe({
      next: () => {
        this.userService.addUserToWarehouse({
          userId: this.selectedManagerId!,
          warehouseId: this.selectedWarehouseId!
        }).subscribe({
          next: () => {
            this.loadWarehouseStates();
            this.showAssignManagerModal = false;
          },
          error: () => {
            this.errorMessage = 'Ошибка добавления пользователя к складу.';
          }
        });
      },
      error: () => {
        this.errorMessage = 'Ошибка назначения начальника склада.';
      }
    });
  }
}
