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
import {FormsModule} from "@angular/forms";

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
    ErrorMessageComponent,
    FormsModule
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
  filter = {
    search: '',
    priceMin: null as number | null,
    priceMax: null as number | null,
    quantityMin: null as number | null,
    quantityMax: null as number | null,
    sortBy: '',
  };

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
        this.errorMessage = 'Ошибка: ID склада не найден.';
      }
    });
  }
  filteredItems() {
    let result = [...this.inventoryItems];

    const search = this.filter.search?.toLowerCase().trim();

    if (search) {
      result = result.filter(item =>
        item.name.toLowerCase().includes(search) ||
        item.supplier.toLowerCase().includes(search)
      );
    }

    if (this.filter.priceMin != null)
      result = result.filter(item => item.estimatedValue >= this.filter.priceMin!);

    if (this.filter.priceMax != null)
      result = result.filter(item => item.estimatedValue <= this.filter.priceMax!);

    if (this.filter.quantityMin != null)
      result = result.filter(item => item.quantity >= this.filter.quantityMin!);

    if (this.filter.quantityMax != null)
      result = result.filter(item => item.quantity <= this.filter.quantityMax!);

    switch (this.filter.sortBy) {
      case 'priceAsc':
        result.sort((a, b) => a.estimatedValue - b.estimatedValue);
        break;
      case 'priceDesc':
        result.sort((a, b) => b.estimatedValue - a.estimatedValue);
        break;
      case 'quantityAsc':
        result.sort((a, b) => a.quantity - b.quantity);
        break;
      case 'quantityDesc':
        result.sort((a, b) => b.quantity - a.quantity);
        break;
    }

    return result;
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
          this.errorMessage = 'Склад не найден.';
          this.isLoading = false;
          return;
        }

        this.warehouseState = state;
        this.processItems();
        this.isLoading = false;
      },
      error: (err) => {
        console.error("❌ Ошибка загрузки склада:", err);
        this.errorMessage = 'Ошибка загрузки состояния склада.';
        this.isLoading = false;
      }
    });
  }

  /** 📌 Обрабатываем полученные данные и заполняем `inventoryItems` */
  processItems(): void {
    if (!this.warehouseState) {
      this.errorMessage = 'Данные склада не загружены.';
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
      this.errorMessage = 'В этом складе пока нет материальных ценностей.';
    }
  }
}
