/*
import { Component, OnInit } from '@angular/core';
import { MovementRequestResponseDto } from '../../../models/dto/movement-request/movement-request-response-dto';
import { MovementRequestService } from '../../../services/movement-request.service';
import { InventoryItemService } from '../../../services/inventory-item.service';
import { InventoryItemResponseDto } from '../../../models/dto/inventory-item/inventory-item-response-dto';
import {LoadingSpinnerComponent} from "../../shared/loading-spinner/loading-spinner.component";
import {HeaderComponent} from "../../shared/header/header.component";
import {NgForOf, NgIf} from "@angular/common";
import {MovementModalComponent} from "../movement-modal/movement-modal.component";
import {FooterComponent} from "../../shared/footer/footer.component";

@Component({
  selector: 'app-movement-list',
  templateUrl: './movement-list.component.html',
  styleUrls: ['./movement-list.component.css'],
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    HeaderComponent,
    NgIf,
    NgForOf,
    MovementModalComponent,
    FooterComponent
  ]
})
export class MovementListComponent implements OnInit {
  movements: MovementRequestResponseDto[] = [];
  items: Record<string, string> = {}; // Хранение пар itemId -> itemName
  isLoading = false;
  showMovementModal = false;
  companyId: string = '';

  constructor(
    private movementService: MovementRequestService,
    private itemService: InventoryItemService
  ) {}

  ngOnInit(): void {
    this.loadMovements();
  }

  onCompanyIdReceived(companyId: string): void {
    if (companyId) {
      this.companyId = companyId;
    }
  }

  loadMovements(): void {
    this.isLoading = true;
    this.movementService.getByStatus('Processing').subscribe({
      next: (data) => {
        this.movements = data;
        this.loadItemNames(); // Подгружаем имена items
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Ошибка загрузки перемещений:', err);
        this.isLoading = false;
      },
    });
  }

  loadItemNames(): void {
    const itemIds = Array.from(new Set(this.movements.map((m) => m.itemId)));
    itemIds.forEach((itemId) => {
      this.itemService.getInventoryItemById(itemId).subscribe({
        next: (item: InventoryItemResponseDto) => {
          this.items[itemId] = item.name;
        },
        error: (err) => console.error(`Ошибка загрузки имени item ${itemId}:`, err),
      });
    });
  }

  getItemName(itemId: string): string {
    return this.items[itemId] || 'Загрузка...';
  }

  openMovementModal(): void {
    this.showMovementModal = true;
  }

  closeMovementModal(): void {
    this.showMovementModal = false;
    this.loadMovements(); // Перезагрузить список после создания
  }
}
*/
import { Component, OnInit } from '@angular/core';
import { MovementRequestResponseDto } from '../../../models/dto/movement-request/movement-request-response-dto';
import { mockMovements } from './mock-movements'; // Импорт моковых данных
import { InventoryItemService } from '../../../services/inventory-item.service';
import { InventoryItemResponseDto } from '../../../models/dto/inventory-item/inventory-item-response-dto';
import {HeaderComponent} from "../../shared/header/header.component";
import {LoadingSpinnerComponent} from "../../shared/loading-spinner/loading-spinner.component";
import {MovementModalComponent} from "../movement-modal/movement-modal.component";
import {FooterComponent} from "../../shared/footer/footer.component";
import {NgForOf, NgIf} from "@angular/common";

@Component({
  selector: 'app-movement-list',
  templateUrl: './movement-list.component.html',
  styleUrls: ['./movement-list.component.css'],
  standalone: true,
  imports: [
    HeaderComponent,
    LoadingSpinnerComponent,
    MovementModalComponent,
    FooterComponent,
    NgIf,
    NgForOf
  ]
})
export class MovementListComponent implements OnInit {
  movements: MovementRequestResponseDto[] = [];
  items: Record<string, string> = {}; // Хранение пар itemId -> itemName
  isLoading = false;
  showMovementModal = false;
  companyId: string = '';

  constructor(private itemService: InventoryItemService) {}

  ngOnInit(): void {
    this.loadMovements();
  }

  loadMovements(): void {
    // Вместо реального запроса используем моковые данные
    this.movements = mockMovements;
    this.loadItemNames(); // Подгружаем имена items
  }

  loadItemNames(): void {
    const itemIds = Array.from(new Set(this.movements.map((m) => m.itemId)));
    itemIds.forEach((itemId) => {
      // Используем моковые данные для подгрузки
      const mockItem: InventoryItemResponseDto = {
        id: itemId,
        name: `Item ${itemId}`,
        uniqueCode: `Unique-${itemId}`,
        quantity: 100,
        estimatedValue: 500,
        expirationDate: new Date('2025-12-31'),
        supplier: {
          id: 'supplier-1',
          name: 'Supplier Mock',
          phoneNumber: '123456789',
          postalAddress: 'Mock Address',
          accountNumber: '12345',
          companyId: 'company-1',
        },
        deliveryDate: new Date(),
        documentId: 'doc-1',
        status: { value: 'Available', name: 'Доступен' },
        warehouseDetails: [
          { warehouseId: 'wh-1', warehouseName: 'Mock Warehouse 1', quantity: 50 },
        ],
        documentInfo: { id: 'doc-1', fileName: 'mock-doc.pdf', fileType: 'application/pdf' },
      };
      this.items[itemId] = mockItem.name;
    });
  }

  getItemName(itemId: string): string {
    return this.items[itemId] || 'Загрузка...';
  }

  openMovementModal(): void {
    this.showMovementModal = true;
  }

  closeMovementModal(): void {
    this.showMovementModal = false;
    this.loadMovements(); // Перезагрузить список после создания
  }
}

