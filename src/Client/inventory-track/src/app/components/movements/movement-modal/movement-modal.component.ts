import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { InventoryItemResponseDto } from '../../../models/dto/inventory-item/inventory-item-response-dto';
import { WarehouseDetailsDto } from '../../../models/dto/warehouse/warehouse-details-dto';
import { WarehouseResponseDto } from '../../../models/dto/warehouse/warehouse-response-dto';
import { WarehouseService } from '../../../services/warehouse.service';
import { MovementRequestService } from '../../../services/movement-request.service';
import { CreateMovementRequestDto } from '../../../models/dto/movement-request/create-movement-request-dto';
import { mockInventoryItems } from '../movement-list/mock-inventory-items';
import { FormsModule } from '@angular/forms';
import { NgForOf, NgIf } from '@angular/common';

@Component({
  selector: 'app-movement-modal',
  templateUrl: './movement-modal.component.html',
  styleUrls: ['./movement-modal.component.css'],
  standalone: true,
  imports: [FormsModule, NgIf, NgForOf],
})
export class MovementModalComponent implements OnInit {
  @Input() companyId!: string;
  @Output() close = new EventEmitter<void>();

  items: InventoryItemResponseDto[] = [];
  selectedItem: InventoryItemResponseDto | null = null;

  sourceWarehouses: WarehouseDetailsDto[] = [];
  selectedSourceWarehouse: WarehouseDetailsDto | null = null;

  destinationWarehouses: WarehouseResponseDto[] = [];
  selectedDestinationWarehouse: WarehouseResponseDto | null = null;

  transferQuantity: number | null = null;
  isLoading = false;
  errorMessage: string | null = null;

  constructor(
    private warehouseService: WarehouseService,
    private movementService: MovementRequestService
  ) {}

  ngOnInit(): void {
    this.loadItems();
    this.loadDestinationWarehouses();
  }

  loadItems(): void {
    this.isLoading = true;
    this.items = mockInventoryItems; // Используем моковые данные
    this.isLoading = false;
  }

  loadSourceWarehouses(): void {
    if (!this.selectedItem) return;
    this.sourceWarehouses = this.selectedItem.warehouseDetails.filter(
      (warehouse) => warehouse.quantity > 0
    );
  }

  loadDestinationWarehouses(): void {
    if (!this.companyId) return;
    this.warehouseService.getWarehousesByCompany(this.companyId).subscribe({
      next: (warehouses) => {
        this.destinationWarehouses = warehouses;
      },
      error: (err) => {
        console.error('Ошибка загрузки складов назначения:', err);
        this.errorMessage = 'Не удалось загрузить склады назначения.';
      },
    });
  }

  createMovementRequest(): void {
    if (
      !this.selectedItem ||
      !this.selectedSourceWarehouse ||
      !this.selectedDestinationWarehouse ||
      !this.transferQuantity
    ) {
      alert('Все поля должны быть заполнены!');
      return;
    }

    if (this.transferQuantity > this.selectedSourceWarehouse.quantity) {
      alert('Количество превышает доступное на складе источника.');
      return;
    }

    const dto: CreateMovementRequestDto = {
      itemId: this.selectedItem.id,
      sourceWarehouseId: this.selectedSourceWarehouse.warehouseId,
      destinationWarehouseId: this.selectedDestinationWarehouse.id,
      quantity: this.transferQuantity,
    };

    this.movementService.create(dto).subscribe({
      next: () => {
        alert('Перемещение успешно создано.');
        this.close.emit();
      },
      error: (err) => {
        console.error('Ошибка создания перемещения:', err);
        this.errorMessage = 'Не удалось создать перемещение.';
      },
    });
  }
  /* loadItems(): void {
    this.isLoading = true;
    this.inventoryService.getAllInventoryItems().subscribe({
      next: (items) => {
        this.items = items;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Ошибка загрузки товаров:', err);
        this.isLoading = false;
      },
    });
  }*/
}
