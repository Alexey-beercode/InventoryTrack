import { Component, OnInit } from '@angular/core';
import {UserResponseDTO} from "../../models/dto/user/user-response-dto";
import {WriteOffRequestResponseDto} from "../../models/dto/writeoff-request/writeoff-request-response-dto";
import { MovementRequestResponseDto } from '../../models/dto/movement-request/movement-request-response-dto';
import { WriteOffRequestService } from '../../services/writeoff-request.service';
import {MovementRequestService} from "../../services/movement-request.service";
import { InventoryItemService } from '../../services/inventory-item.service';
import {WarehouseService} from "../../services/warehouse.service";
import {NgForOf, NgIf} from "@angular/common";
import {HeaderComponent} from "../shared/header/header.component";
import {FooterComponent} from "../shared/footer/footer.component";
import {DataPipe} from "../../pipes/data-pipe";

@Component({
  selector: 'app-my-requests',
  templateUrl: './my-requests.component.html',
  styleUrls: ['./my-requests.component.css'],
  imports: [
    NgIf,
    NgForOf,
    HeaderComponent,
    FooterComponent,
    DataPipe
  ],
  standalone: true
})
export class MyRequestsComponent implements OnInit {
  user: UserResponseDTO | null = null;
  userId: string = '';
  isStorageManager = false;
  isDepartmentManager = false;
  selectedTab: 'writeoffs' | 'movements' = 'writeoffs';
  errorMessage: string | null = null;
  userRoles: string[] = [];


  writeOffs: WriteOffRequestResponseDto[] = [];
  movements: MovementRequestResponseDto[] = [];

  items: Record<string, string> = {};
  warehouses: Record<string, string> = {};

  constructor(
    private writeOffService: WriteOffRequestService,
    private movementService: MovementRequestService,
    private itemService: InventoryItemService,
    private warehouseService: WarehouseService
  ) {}

  ngOnInit(): void {}

  onUserReceived(user: UserResponseDTO): void {
    this.user = user;
    this.userId = user.id;
    this.userRoles = user.roles.map(r => r.name); // 👈 напрямую из user

    this.isStorageManager = this.userRoles.includes('Начальник склада');
    this.isDepartmentManager = this.userRoles.includes('Начальник подразделения');

    if (this.isStorageManager || this.isDepartmentManager) {
      this.loadWriteOffs();
    }

    if (this.isDepartmentManager) {
      this.loadMovements();
    }
  }


  loadWriteOffs(): void {
    this.writeOffService.getByWarehouseId(this.user?.warehouseId!).subscribe({
      next: (data) => {
        this.writeOffs = data;
        this.loadWriteOffWarehouseNames(); // 👈 загружаем название склада
      },
      error: () => {
        this.errorMessage = 'Ошибка загрузки заявок на списание';
        this.writeOffs = [];
      }
    });

  }

  loadMovements(): void {
    const warehouseId = this.user?.warehouseId;
    if (!warehouseId) return;

    this.movementService.getByWarehouse(warehouseId).subscribe({
      next: (data) => {
        this.movements = data;
        this.loadItemNames();
        this.loadWarehouseNames();
      },
      error: () => console.error('Ошибка загрузки перемещений')
    });
  }

  loadItemNames(): void {
    const itemIds = new Set(this.movements.map((m) => m.itemId));
    itemIds.forEach((itemId) => {
      this.itemService.getInventoryItemById(itemId).subscribe({
        next: (item) => (this.items[itemId] = item.name),
        error: () => console.error('Ошибка загрузки товара')
      });
    });
  }

  loadWarehouseNames(): void {
    const warehouseIds = new Set(
      this.movements.flatMap((m) => [m.sourceWarehouseId, m.destinationWarehouseId])
    );

    warehouseIds.forEach((id) => {
      this.warehouseService.getWarehouseById(id).subscribe({
        next: (w) => (this.warehouses[id] = w.name),
        error: () => console.error('Ошибка загрузки склада')
      });
    });
  }

  getItemName(id: string): string {
    return this.items[id] || 'Загрузка...';
  }

  getWarehouseName(id: string): string {
    return this.warehouses[id] || 'Загрузка...';
  }

  loadWriteOffWarehouseNames(): void {
    const warehouseIds = new Set(this.writeOffs.map(w => w.warehouseId));

    warehouseIds.forEach(id => {
      if (!this.warehouses[id]) {
        this.warehouseService.getWarehouseById(id).subscribe({
          next: (w) => (this.warehouses[id] = w.name),
          error: () => console.error(`Ошибка загрузки склада: ${id}`)
        });
      }
    });
  }

}
