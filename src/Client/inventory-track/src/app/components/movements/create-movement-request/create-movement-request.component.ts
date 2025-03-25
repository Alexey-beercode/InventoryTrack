import { Component, OnInit } from '@angular/core';
import { InventoryItemService } from '../../../services/inventory-item.service';
import { MovementRequestService } from '../../../services/movement-request.service';
import { WarehouseService } from '../../../services/warehouse.service';
import { InventoryItemResponseDto } from '../../../models/dto/inventory-item/inventory-item-response-dto';
import { WarehouseResponseDto } from '../../../models/dto/warehouse/warehouse-response-dto';
import { CreateMovementRequestDto } from '../../../models/dto/movement-request/create-movement-request-dto';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { TokenService } from '../../../services/token.service';
import { Router } from '@angular/router';
import { HeaderComponent } from '../../shared/header/header.component';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { ErrorMessageComponent } from '../../shared/error/error.component';
import { BackButtonComponent } from '../../shared/back-button/back-button.component';
import { NgIf, NgForOf } from '@angular/common';
import { FormsModule } from "@angular/forms";

@Component({
  selector: 'app-create-movement-request',
  templateUrl: './create-movement-request.component.html',
  styleUrls: ['./create-movement-request.component.css'],
  standalone: true,
  imports: [
    HeaderComponent,
    LoadingSpinnerComponent,
    FooterComponent,
    ErrorMessageComponent,
    BackButtonComponent,
    NgIf,
    NgForOf,
    FormsModule
  ]
})
export class CreateMovementRequestComponent implements OnInit {
  companyId: string = ''; // Получаем из хедера
  warehouses: WarehouseResponseDto[] = []; // Список складов компании
  selectedSourceWarehouse: WarehouseResponseDto | null = null;
  destinationWarehouse: WarehouseResponseDto | null = null;
  items: InventoryItemResponseDto[] = []; // Товары выбранного склада
  selectedItem: InventoryItemResponseDto | null = null;
  transferQuantity: number | null = null;
  isLoading = false;
  errorMessage: string | null = null;
  user: UserResponseDTO | null = null;

  constructor(
    private inventoryItemService: InventoryItemService,
    private movementRequestService: MovementRequestService,
    private warehouseService: WarehouseService,
    private tokenService: TokenService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  /** 📌 Получаем данные пользователя и companyId из хедера */
  onUserReceived(user: UserResponseDTO | null): void {
    if (!user) {
      this.errorMessage = 'Ошибка получения данных пользователя.';
      return;
    }
    this.user = user;
    this.loadDestinationWarehouse(user.warehouseId);
  }

  onCompanyIdReceived(companyId: string): void {
    this.companyId = companyId;
    this.loadWarehouses(companyId);
  }

  /** 📌 Загружаем склады компании, исключая склад назначения */
  loadWarehouses(companyId: string): void {
    this.isLoading = true;
    this.warehouseService.getWarehousesByCompany(companyId).subscribe({
      next: (warehouses) => {
        this.warehouses = warehouses.filter(w => w.id !== this.user?.warehouseId);
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Ошибка загрузки складов компании.';
        this.isLoading = false;
      }
    });
  }

  /** 📌 Загружаем склад назначения */
  loadDestinationWarehouse(warehouseId: string): void {
    this.warehouseService.getWarehouseById(warehouseId).subscribe({
      next: (warehouse) => {
        this.destinationWarehouse = warehouse;
      },
      error: () => {
        this.errorMessage = 'Ошибка загрузки склада назначения.';
      }
    });
  }

  /** 📌 Загружаем товары со склада */
  loadItems(): void {
    if (!this.selectedSourceWarehouse) return;

    this.isLoading = true;
    this.inventoryItemService.getInventoryItemsByWarehouse(this.selectedSourceWarehouse.id).subscribe({
      next: (items) => {
        this.items = items.filter(item => item.warehouseDetails?.some(wd => wd.warehouseId === this.selectedSourceWarehouse!.id && wd.quantity > 0));
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Ошибка загрузки товаров со склада.';
        this.isLoading = false;
      }
    });
  }

  getMaxQuantity(item: InventoryItemResponseDto, warehouseId: string): number {
    if (!item || !warehouseId) {
      console.error('Ошибка: item или warehouseId отсутствует!');
      return 0;
    }

    console.log(`🔍 Проверяем товар: ${item.name}`);
    console.log(`🔍 Ищем количество для склада ID: ${warehouseId}`);

    // Ищем склад в `warehouseDetails`
    const warehouseDetail = item.warehouseDetails?.find(
      wd => wd.warehouseId.trim().toLowerCase() === warehouseId.trim().toLowerCase()
    );

    if (!warehouseDetail) {
      console.warn(`⚠️ Товар ${item.name} отсутствует на складе!`);
      return 0;
    }

    console.log(`✅ Найден склад: ${warehouseDetail.warehouseName}, Количество: ${warehouseDetail.quantity}`);
    return warehouseDetail.quantity;
  }


  /** 📌 Создаем запрос на перемещение */
  createMovementRequest(): void {
    if (!this.selectedItem || !this.transferQuantity || this.transferQuantity < 1 || !this.selectedSourceWarehouse || !this.destinationWarehouse) {
      this.errorMessage = 'Все поля должны быть заполнены!';
      return;
    }

    const maxQuantity = this.getMaxQuantity(this.selectedItem, this.selectedSourceWarehouse.id);

    if (this.transferQuantity > maxQuantity) {
      this.errorMessage = `Нельзя переместить больше, чем ${maxQuantity} шт.`;
      return;
    }

    const dto: CreateMovementRequestDto = {
      itemId: this.selectedItem.id,
      sourceWarehouseId: this.selectedSourceWarehouse.id,
      destinationWarehouseId: this.destinationWarehouse.id, // Теперь используем название склада назначения
      quantity: this.transferQuantity,
    };

    this.isLoading = true;
    this.movementRequestService.create(dto).subscribe({
      next: () => {
        this.router.navigate(['/']); // Перенаправляем после успешного создания
      },
      error: () => {
        this.errorMessage = 'Ошибка создания перемещения.';
        this.isLoading = false;
      }
    });
  }
}
