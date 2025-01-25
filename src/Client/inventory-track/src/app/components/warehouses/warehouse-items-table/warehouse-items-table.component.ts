import { Component, Input, OnInit } from '@angular/core';
import { WarehouseStateResponseDto } from '../../../models/dto/warehouse/warehouse-state-response-dto';
import { InventoryItemResponseDto } from '../../../models/dto/inventory-item/inventory-item-response-dto';
import { mockWarehouseState } from '../mock-warehouses-state';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-warehouse-items-table',
  templateUrl: './warehouse-items-table.component.html',
  styleUrls: ['./warehouse-items-table.component.css'],
  standalone: true,
  imports: [HeaderComponent, FooterComponent, CommonModule],
})
export class WarehouseItemsTableComponent implements OnInit {
  @Input() warehouseState: WarehouseStateResponseDto = mockWarehouseState; // Используем моковый объект по умолчанию

  inventoryItems: {
    name: string;
    uniqueCode: string;
    quantity: number;
    supplier: string;
    estimatedValue: number;
    expirationDate: Date;
  }[] = [];

  ngOnInit(): void {
    if (this.warehouseState) {
      this.processItems();
    }
  }

  processItems(): void {
    this.inventoryItems = this.warehouseState.inventoryItems.map((item) => {
      const warehouseDetail = item.warehouseDetails.find(
        (wd) => wd.warehouseId === this.warehouseState.id
      );

      return {
        name: item.name,
        uniqueCode: item.uniqueCode,
        quantity: warehouseDetail?.quantity || 0,
        supplier: item.supplier.name,
        estimatedValue: item.estimatedValue,
        expirationDate: item.expirationDate,
      };
    });
  }
}
