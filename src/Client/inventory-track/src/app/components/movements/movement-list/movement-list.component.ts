import { Component, OnInit } from '@angular/core';
import { MovementRequestResponseDto } from '../../../models/dto/movement-request/movement-request-response-dto';
import { MovementRequestService } from '../../../services/movement-request.service';
import { InventoryItemService } from '../../../services/inventory-item.service';
import { WarehouseService } from '../../../services/warehouse.service';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { TokenService } from '../../../services/token.service';
import { HeaderComponent } from "../../shared/header/header.component";
import { LoadingSpinnerComponent } from "../../shared/loading-spinner/loading-spinner.component";
import { FooterComponent } from "../../shared/footer/footer.component";
import { ErrorMessageComponent } from "../../shared/error/error.component";
import { NgForOf, NgIf } from "@angular/common";
import { MovementRequestStatus } from '../../../models/dto/movement-request/enums/movement-request-status.enum';
import { ChangeStatusDto } from '../../../models/dto/movement-request/change-status-dto';

@Component({
  selector: 'app-movement-list',
  templateUrl: './movement-list.component.html',
  styleUrls: ['./movement-list.component.css'],
  standalone: true,
  imports: [
    HeaderComponent,
    LoadingSpinnerComponent,
    FooterComponent,
    ErrorMessageComponent,
    NgIf,
    NgForOf
  ]
})
export class MovementListComponent implements OnInit {
  movements: MovementRequestResponseDto[] = [];
  items: Record<string, string> = {}; // Хранение itemId -> itemName
  warehouses: Record<string, string> = {}; // Хранение warehouseId -> warehouseName
  isLoading = false;
  errorMessage: string | null = null;
  user: UserResponseDTO | null = null;
  userRoles: string[] = [];

  constructor(
    private movementService: MovementRequestService,
    private itemService: InventoryItemService,
    private warehouseService: WarehouseService,
    private tokenService: TokenService
  ) {}

  ngOnInit(): void {}

  onUserReceived(user: UserResponseDTO | null): void {
    if (!user) {
      this.errorMessage = "⚠️ Пользовательские данные не получены.";
      return;
    }

    this.user = user;
    this.userRoles = this.tokenService.getUserRoles();
    this.loadMovements();
  }

  /** 📌 Загружаем все перемещения */
  loadMovements(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.movementService.getByWarehouse(this.user?.warehouseId!).subscribe({
      next: (data) => {
        this.movements = data;
        this.loadItemNames();
        this.loadWarehouseNames();
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = '❌ Ошибка загрузки перемещений.';
        this.isLoading = false;
      }
    });
  }

  /** 📌 Подгружаем названия товаров */
  loadItemNames(): void {
    const itemIds = new Set(this.movements.map((m) => m.itemId));
    itemIds.forEach((itemId) => {
      this.itemService.getInventoryItemById(itemId).subscribe({
        next: (item) => this.items[itemId] = item.name,
        error: () => this.errorMessage = `❌ Ошибка загрузки товара.`
      });
    });
  }

  /** 📌 Подгружаем названия складов */
  loadWarehouseNames(): void {
    const warehouseIds = new Set(
      this.movements.flatMap(m => [m.sourceWarehouseId, m.destinationWarehouseId])
    );

    warehouseIds.forEach((warehouseId) => {
      if (!this.warehouses[warehouseId]) {
        this.warehouseService.getWarehouseById(warehouseId).subscribe({
          next: (warehouse) => this.warehouses[warehouseId] = warehouse.name,
          error: () => this.errorMessage = `❌ Ошибка загрузки склада.`
        });
      }
    });
  }

  /** 📌 Возвращает название товара */
  getItemName(itemId: string): string {
    return this.items[itemId] || 'Загрузка...';
  }

  /** 📌 Возвращает название склада */
  getWarehouseName(warehouseId: string): string {
    return this.warehouses[warehouseId] || 'Загрузка...';
  }

  /** ✅ Одобрить перемещение (только `Processing`) */
  approveMovement(movementId: string): void {
    if (!this.user) {
      this.errorMessage = '❌ Ошибка: пользователь не найден!';
      return;
    }

    const dto: ChangeStatusDto = {
      userId: this.user.id,
      requestId: movementId
    };

    this.movementService.approve(dto).subscribe({
      next: () => {
        alert("✅ Перемещение одобрено!");
        this.loadMovements();
      },
      error: () => alert("❌ Ошибка при одобрении перемещения!")
    });
  }

  /** ❌ Отклонить перемещение (только `Processing`) */
  rejectMovement(movementId: string): void {
    if (!this.user) {
      this.errorMessage = '❌ Ошибка: пользователь не найден!';
      return;
    }

    const dto: ChangeStatusDto = {
      userId: this.user.id,
      requestId: movementId
    };

    this.movementService.reject(dto).subscribe({
      next: () => {
        alert("❌ Перемещение отклонено!");
        this.loadMovements();
      },
      error: () => alert("❌ Ошибка при отклонении перемещения!")
    });
  }

  protected readonly MovementRequestStatus = MovementRequestStatus;
}
