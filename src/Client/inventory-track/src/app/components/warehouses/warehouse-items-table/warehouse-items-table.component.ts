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
import { FormsModule } from "@angular/forms";

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
  warehouseId: string = '';
  warehouseState: WarehouseStateResponseDto | null = null;
  inventoryItems: {
    name: string;
    uniqueCode: string;
    quantity: number;
    supplier: string;
    estimatedValue: number;
    expirationDate: Date;
    // 🆕 Новые поля для ТТН и партий
    batchNumber?: string;
    measureUnit?: string;
    vatRate?: number;
    placesCount?: number;
    cargoWeight?: number;
    notes?: string;
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
    // 🆕 Новые фильтры
    batchNumber: '',
    measureUnit: '',
  };
  private _filteredItems: any[] = [];
  private _lastFilterState: any = {};

  // 🆕 Управление отображением колонок
  showAdvancedColumns = false;

  constructor(
    private route: ActivatedRoute,
    private warehouseService: WarehouseService
  ) {}

  ngOnInit(): void {
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
        item.supplier.toLowerCase().includes(search) ||
        (item.batchNumber && item.batchNumber.toLowerCase().includes(search))
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

    // 🆕 Фильтр по номеру партии
    if (this.filter.batchNumber) {
      const batchSearch = this.filter.batchNumber.toLowerCase();
      result = result.filter(item =>
        item.batchNumber && item.batchNumber.toLowerCase().includes(batchSearch)
      );
    }

    // 🆕 Фильтр по единице измерения
    if (this.filter.measureUnit) {
      result = result.filter(item => item.measureUnit === this.filter.measureUnit);
    }

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
      // 🆕 Сортировка по партиям
      case 'batchAsc':
        result.sort((a, b) => (a.batchNumber || '').localeCompare(b.batchNumber || ''));
        break;
      case 'batchDesc':
        result.sort((a, b) => (b.batchNumber || '').localeCompare(a.batchNumber || ''));
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
        // 🆕 Новые поля
        batchNumber: item.batchNumber,
        measureUnit: item.measureUnit || 'шт',
        vatRate: item.vatRate,
        placesCount: item.placesCount,
        cargoWeight: item.cargoWeight,
        notes: item.notes
      };
    });

    console.log("📋 Итоговый список товаров:", this.inventoryItems);

    if (this.inventoryItems.length === 0) {
      this.errorMessage = 'В этом складе пока нет материальных ценностей.';
    }
  }

  // 🆕 Получить уникальные единицы измерения для фильтра
  getUniqueMeasureUnits(): string[] {
    const units = this.inventoryItems
      .map(item => item.measureUnit)
      .filter((unit, index, arr) => unit && arr.indexOf(unit) === index);
    return units as string[];
  }

  // 🆕 Переключение расширенного режима
  toggleAdvancedView(): void {
    this.showAdvancedColumns = !this.showAdvancedColumns;
  }

  getFilteredItems() {
    // Проверяем, изменились ли фильтры
    const currentFilterState = JSON.stringify(this.filter);
    if (currentFilterState !== JSON.stringify(this._lastFilterState)) {
      this._filteredItems = this.filteredItems();
      this._lastFilterState = { ...this.filter };
    }
    return this._filteredItems;
  }

  getFilteredItemsCount(): number {
    return this.getFilteredItems().length;
  }

// Методы для отображения значений
  getVatRateDisplay(vatRate?: number): string {
    return vatRate ? (vatRate + '%') : '0%';
  }

  getPlacesCountDisplay(placesCount?: number): string {
    return placesCount ? placesCount.toString() : '1';
  }

  getCargoWeightDisplay(cargoWeight?: number): string {
    return cargoWeight ? cargoWeight.toString() : '—';
  }

  getTruncatedNotes(notes: string): string {
    return notes.length > 20 ? (notes.substring(0, 20) + '...') : notes;
  }

// Проверяем, нужно ли показывать сводку по партиям
  shouldShowBatchSummary(): boolean {
    const batches = this.getBatchGroups();
    return batches.size > 1;
  }

// Получаем данные для сводки по партиям
  getBatchSummaryData(): Array<{batchNumber: string, itemsCount: number, totalQuantity: number}> {
    const batches = this.getBatchGroups();
    const result: Array<{batchNumber: string, itemsCount: number, totalQuantity: number}> = [];

    batches.forEach((items, batchNumber) => {
      const totalQuantity = items.reduce((sum, item) => sum + item.quantity, 0);
      result.push({
        batchNumber: batchNumber,
        itemsCount: items.length,
        totalQuantity: totalQuantity
      });
    });

    return result;
  }

// Приватный метод для группировки по партиям
  private getBatchGroups(): Map<string, any[]> {
    const batchGroups = new Map<string, any[]>();

    this.getFilteredItems().forEach(item => {
      const batchKey = item.batchNumber || 'Без партии';
      if (!batchGroups.has(batchKey)) {
        batchGroups.set(batchKey, []);
      }
      batchGroups.get(batchKey)!.push(item);
    });

    return batchGroups;
  }

  isExpiringSoon(expirationDate: Date): boolean {
    const today = new Date();
    const expDate = new Date(expirationDate);
    const daysUntilExpiry = Math.ceil((expDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));

    return daysUntilExpiry <= 30 && daysUntilExpiry >= 0;
  }
}
