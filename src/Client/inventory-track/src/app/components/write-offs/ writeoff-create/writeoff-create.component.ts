import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { InventoryItemService } from '../../../services/inventory-item.service';
import { WriteOffRequestService } from '../../../services/writeoff-request.service';
import { WriteOffReasonService } from '../../../services/writeoff-reason.service';
import { BatchService } from '../../../services/batch.service';
import { TokenService } from '../../../services/token.service';
import { UserService } from '../../../services/user.service';
import { InventoryItemResponseDto } from '../../../models/dto/inventory-item/inventory-item-response-dto';
import { WriteOffReasonResponseDto } from '../../../models/dto/writeoff-reason/writeoff-reason-response-dto';
import { CreateWriteOffRequestDto } from '../../../models/dto/writeoff-request/create-writeoff-request-dto';
import { CreateBatchWriteOffRequestDto } from '../../../models/dto/writeoff-request/create-batch-writeoff-request-dto';
import { BatchInfoDto } from '../../../models/dto/inventory-item/batch-info-dto';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BackButtonComponent } from '../../shared/back-button/back-button.component';
import { ErrorMessageComponent } from '../../shared/error/error.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-writeoff-create',
  templateUrl: './writeoff-create.component.html',
  styleUrls: ['./writeoff-create.component.css'],
  standalone: true,
  imports: [
    HeaderComponent,
    FooterComponent,
    CommonModule,
    BackButtonComponent,
    ErrorMessageComponent,
    FormsModule,
  ],
})
export class WriteOffCreateComponent implements OnInit {
  warehouseId: string = '';
  companyId: string = '';
  userId: string = '';
  items: InventoryItemResponseDto[] = [];
  writeOffReasons: WriteOffReasonResponseDto[] = [];
  selectedReasonId: string = '';
  anotherReason: string = '';
  selectedItemId: string = '';
  quantity: number = 1;
  maxQuantity: number = 1;
  errorMessage: string | null = null;

  // 🆕 Поддержка партий
  selectedItemName: string = '';
  availableBatches: BatchInfoDto[] = [];
  showBatchSelection = false;
  selectedBatchNumber: string = '';
  writeOffMode: 'individual' | 'batch' = 'individual';
  isLoadingBatches = false;

  constructor(
    private inventoryItemService: InventoryItemService,
    private writeOffRequestService: WriteOffRequestService,
    private writeOffReasonService: WriteOffReasonService,
    private batchService: BatchService,
    private tokenService: TokenService,
    private userService: UserService,
    private router: Router
  ) {}

  onCompanyIdReceived(companyId: string) {
    this.companyId = companyId;
    console.log("📌 Получен companyId из HeaderComponent:", this.companyId);
  }

  ngOnInit(): void {
    this.loadUserWarehouseAndCompany();
  }

  /** 📌 Загружаем ID склада и компании пользователя */
  loadUserWarehouseAndCompany(): void {
    const userId = this.tokenService.getUserId();
    if (!userId) {
      this.errorMessage = "Ошибка: пользователь не найден.";
      return;
    }

    this.userId = userId;

    this.userService.getById(userId).subscribe({
      next: (user: UserResponseDTO) => {
        if (user.warehouseId) {
          this.warehouseId = user.warehouseId;
          this.loadInventoryItems();
          this.loadWriteOffReasons();
        } else {
          this.errorMessage = "Ошибка: у пользователя нет склада.";
        }
      },
      error: () => {
        this.errorMessage = "Ошибка загрузки данных пользователя.";
      }
    });
  }

  /** 📌 Загружаем материальные ценности склада */
  loadInventoryItems(): void {
    if (!this.warehouseId) return;

    this.inventoryItemService.getInventoryItemsByWarehouse(this.warehouseId).subscribe({
      next: (data) => {
        this.items = data;
      },
      error: () => {
        this.errorMessage = "Ошибка загрузки списка товаров.";
      },
    });
  }

  /** 📌 Загружаем базовые причины списания */
  loadWriteOffReasons(): void {
    this.writeOffReasonService.getAll().subscribe({
      next: (data) => {
        this.writeOffReasons = data;
      },
      error: () => {
        this.errorMessage = "Ошибка загрузки причин списания.";
      },
    });
  }

  /** 📌 Обновляет максимальное количество списания при смене товара */
  updateMaxQuantity(): void {
    const selectedItem = this.items.find(item => item.id === this.selectedItemId);
    if (selectedItem) {
      this.maxQuantity = selectedItem.quantity;
      this.selectedItemName = selectedItem.name;

      if (this.quantity > this.maxQuantity) {
        this.quantity = this.maxQuantity;
      }

      // 🆕 Сбрасываем режим партий при смене товара
      this.showBatchSelection = false;
      this.availableBatches = [];
      this.selectedBatchNumber = '';
      this.writeOffMode = 'individual';
    }
  }

  /** 🆕 Загрузка партий для выбранного товара */
  loadBatchesForItem(): void {
    if (!this.selectedItemName) {
      this.errorMessage = "Сначала выберите товар";
      return;
    }

    this.isLoadingBatches = true;
    this.inventoryItemService.getBatchesByItemName(this.selectedItemName).subscribe({
      next: (batches) => {
        this.availableBatches = batches;
        this.showBatchSelection = true;
        this.isLoadingBatches = false;

        if (batches.length === 0) {
          this.errorMessage = "Для данного товара нет доступных партий";
        }
      },
      error: (error) => {
        console.error("❌ Ошибка загрузки партий:", error);
        this.errorMessage = "Ошибка загрузки партий товара";
        this.isLoadingBatches = false;
      }
    });
  }

  /** 🆕 Переключение режима списания */
  switchWriteOffMode(mode: 'individual' | 'batch'): void {
    this.writeOffMode = mode;
    this.showBatchSelection = false;
    this.selectedBatchNumber = '';
    this.errorMessage = null;

    if (mode === 'batch' && this.selectedItemName) {
      this.loadBatchesForItem();
    }
  }

  /** 🆕 Выбор партии для списания */
  selectBatch(batchNumber: string): void {
    this.selectedBatchNumber = batchNumber;
    const selectedBatch = this.availableBatches.find(b => b.batchNumber === batchNumber);

    if (selectedBatch) {
      console.log(`📦 Выбрана партия: ${batchNumber}, количество: ${selectedBatch.totalQuantity}`);
    }
  }

  /** 📌 Отправка запроса на списание */
  submitWriteOffRequest(form: NgForm): void {
    if (this.writeOffMode === 'individual') {
      this.submitIndividualWriteOff(form);
    } else {
      this.submitBatchWriteOff(form);
    }
  }

  /** 📌 Индивидуальное списание */
  private submitIndividualWriteOff(form: NgForm): void {
    if (form.invalid || this.quantity > this.maxQuantity) {
      this.errorMessage = "Заполните все обязательные поля и укажите корректное количество!";
      return;
    }

    const request: CreateWriteOffRequestDto = {
      itemId: this.selectedItemId,
      warehouseId: this.warehouseId,
      quantity: this.quantity,
      reasonId: this.selectedReasonId !== 'other' ? this.selectedReasonId : null,
      anotherReason: this.selectedReasonId === 'other' ? this.anotherReason : undefined,
      companyId: this.companyId,
    };

    console.log("📤 Отправляемый индивидуальный запрос:", request);

    this.writeOffRequestService.create(request).subscribe({
      next: () => {
        this.router.navigate(['/writeoff-list']);
      },
      error: (err) => {
        console.error("❌ Ошибка при создании запроса:", err);
        this.errorMessage = "Ошибка при создании запроса.";
      },
    });
  }

  /** 🆕 Списание всей партии */
  private submitBatchWriteOff(form: NgForm): void {
    if (!this.selectedBatchNumber || !this.selectedReasonId) {
      this.errorMessage = "Выберите партию и причину списания!";
      return;
    }

    const batchRequest: CreateBatchWriteOffRequestDto = {
      batchNumber: this.selectedBatchNumber,
      reasonId: this.selectedReasonId !== 'other' ? this.selectedReasonId : undefined,
      anotherReason: this.selectedReasonId === 'other' ? this.anotherReason : undefined,
      companyId: this.companyId,
      requestedByUserId: this.userId
    };

    console.log("📤 Отправляемый запрос на списание партии:", batchRequest);

    this.writeOffRequestService.createBatch(batchRequest).subscribe({
      next: (response) => {
        console.log("✅ Партия успешно отправлена на списание:", response);
        this.router.navigate(['/writeoff-list']);
      },
      error: (err) => {
        console.error("❌ Ошибка при создании запроса на партию:", err);
        this.errorMessage = "Ошибка при создании запроса на списание партии.";
      },
    });
  }

  /** 📌 Метод отмены и возврата */
  cancel(): void {
    this.router.navigate(['/']);
  }

  canSubmit(): boolean {
    // Базовые проверки
    if (!this.selectedItemId || !this.selectedReasonId) {
      return false;
    }

    // Проверка "другой причины"
    if (this.selectedReasonId === 'other' && !this.anotherReason.trim()) {
      return false;
    }

    // Проверки для индивидуального списания
    if (this.writeOffMode === 'individual') {
      return this.quantity > 0 && this.quantity <= this.maxQuantity;
    }

    // Проверки для партийного списания
    if (this.writeOffMode === 'batch') {
      return !!this.selectedBatchNumber;
    }

    return false;
  }

  // 🆕 Вспомогательные методы для шаблона (убираем сложные выражения)
  getSelectedItem(): InventoryItemResponseDto | undefined {
    return this.items.find(item => item.id === this.selectedItemId);
  }

  getSelectedBatch(): BatchInfoDto | undefined {
    return this.availableBatches.find(b => b.batchNumber === this.selectedBatchNumber);
  }

  getSelectedItemMeasureUnit(): string {
    const item = this.getSelectedItem();
    return item?.measureUnit || 'шт';
  }

  getSelectedBatchItemsCount(): number {
    const batch = this.getSelectedBatch();
    return batch?.itemsCount || 0;
  }

  getSelectedBatchTotalQuantity(): number {
    const batch = this.getSelectedBatch();
    return batch?.totalQuantity || 0;
  }
}
