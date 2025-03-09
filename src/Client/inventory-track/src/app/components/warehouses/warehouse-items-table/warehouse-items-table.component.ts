import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { WarehouseStateResponseDto } from '../../../models/dto/warehouse/warehouse-state-response-dto';
import { WarehouseService } from '../../../services/warehouse.service';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { CommonModule } from '@angular/common';
import { BackButtonComponent } from "../../shared/back-button/back-button.component";
import { LoadingSpinnerComponent } from "../../shared/loading-spinner/loading-spinner.component";
import { ErrorMessageComponent } from "../../shared/error/error.component";

@Component({
  selector: 'app-warehouse-items-table',
  templateUrl: './warehouse-items-table.component.html',
  styleUrls: ['./warehouse-items-table.component.css'],
  standalone: true,
  imports: [
    HeaderComponent,
    FooterComponent,
    CommonModule,
    BackButtonComponent,
    LoadingSpinnerComponent,
    ErrorMessageComponent
  ],
})
export class WarehouseItemsTableComponent implements OnInit {
  warehouseId: string = ''; // ID склада получаем из URL
  warehouseState: WarehouseStateResponseDto | null = null;
  inventoryItems: {
    name: string;
    uniqueCode: string;
    quantity: number;
    supplier: string;
    estimatedValue: number;
    expirationDate: Date;
  }[] = [];

  isLoading = false;
  errorMessage: string | null = null;

  constructor(
    private route: ActivatedRoute, // Достаем параметры из URL
    private warehouseService: WarehouseService
  ) {}

  ngOnInit(): void {
    // Получаем warehouseId из параметров URL
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.warehouseId = id;
        console.log(`✅ Получен warehouseId из URL: ${this.warehouseId}`);
        this.loadWarehouseState();
      } else {
        console.warn("⚠️ warehouseId отсутствует в URL");
        this.errorMessage = '❌ Ошибка: ID склада не найден.';
      }
    });
  }

  /** 📌 Загружаем состояние склада */
  loadWarehouseState(): void {
    if (!this.warehouseId) return;

    console.log(`🔄 Загружаем данные для склада ID: ${this.warehouseId}`);
    this.isLoading = true;
    this.errorMessage = null;

    this.warehouseService.getWarehouseStateById(this.warehouseId).subscribe({
      next: (state) => {
        console.log("✅ Данные склада загружены:", state);

        if (!state) {
          this.errorMessage = '❌ Склад не найден.';
          this.isLoading = false;
          return;
        }

        this.warehouseState = state;
        this.processItems();
        this.isLoading = false;
      },
      error: (err) => {
        console.error("❌ Ошибка загрузки склада:", err);
        this.errorMessage = '❌ Ошибка загрузки состояния склада.';
        this.isLoading = false;
      }
    });
  }

  /** 📌 Обрабатываем полученные данные и заполняем `inventoryItems` */
  processItems(): void {
    if (!this.warehouseState) {
      this.errorMessage = '❌ Данные склада не загружены.';
      return;
    }

    console.log("📦 Обрабатываем товары склада...");
    this.inventoryItems = this.warehouseState.inventoryItems.map((item) => {
      const warehouseDetail = item.warehouseDetails.find(
        (wd) => wd.warehouseId === this.warehouseState!.id
      );

      return {
        name: item.name,
        uniqueCode: item.uniqueCode,
        quantity: warehouseDetail?.quantity ?? 0,
        supplier: item.supplier?.name || 'Неизвестно',
        estimatedValue: item.estimatedValue,
        expirationDate: item.expirationDate,
      };
    });

    console.log("📋 Итоговый список товаров:", this.inventoryItems);

    if (this.inventoryItems.length === 0) {
      this.errorMessage = '❌ В этом складе пока нет материальных ценностей.';
    }
  }
}
