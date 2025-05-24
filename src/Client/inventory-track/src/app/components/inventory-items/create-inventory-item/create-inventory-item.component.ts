// src/app/components/inventory-item/create-inventory-item/create-inventory-item.component.ts (ОБНОВЛЕННЫЙ)
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { InventoryItemService } from '../../../services/inventory-item.service';
import { SupplierService } from '../../../services/supplier.service';
import { BatchService } from '../../../services/batch.service';
import { CreateInventoryItemDto } from '../../../models/dto/inventory-item/create-inventory-item-dto';
import { SupplierResponseDto } from '../../../models/dto/supplier/supplier-response-dto';
import { CommonModule } from '@angular/common';
import { ErrorMessageComponent } from '../../shared/error/error.component';
import { BackButtonComponent } from '../../shared/back-button/back-button.component';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { Router } from "@angular/router";

@Component({
  selector: 'app-create-inventory-item',
  templateUrl: './create-inventory-item.component.html',
  styleUrls: ['./create-inventory-item.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, ErrorMessageComponent, BackButtonComponent, HeaderComponent, FooterComponent],
})
export class CreateInventoryItemComponent implements OnInit {
  newInventoryItem: CreateInventoryItemDto = {
    name: '',
    uniqueCode: '',
    quantity: 1,
    estimatedValue: 0,
    expirationDate: '',
    supplierId: '',
    warehouseId: '',
    deliveryDate: '',
    documentId: '',
    // 🆕 Новые поля для ТТН
    batchNumber: '',
    measureUnit: 'шт',
    vatRate: 0,
    placesCount: 1,
    cargoWeight: 0,
    notes: ''
  };

  documentFile: File | null = null;
  suppliers: SupplierResponseDto[] = [];
  warehouseId: string = '';
  errorMessage: string | null = null;
  showDateError = false;
  showBatchNumberSuggestion = false;

  constructor(
    private inventoryItemService: InventoryItemService,
    private supplierService: SupplierService,
    private batchService: BatchService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSuppliers();
    this.generateBatchNumber(); // Автогенерация номера партии
  }

  /** 📌 Получаем объект пользователя из `userEmitter` и устанавливаем склад */
  onUserReceived(user: UserResponseDTO): void {
    if (!user || !user.warehouseId) {
      this.errorMessage = "У пользователя нет закрепленного склада.";
      return;
    }

    this.warehouseId = user.warehouseId;
    this.newInventoryItem.warehouseId = this.warehouseId;
  }

  /** 📌 Загружаем список поставщиков */
  loadSuppliers(): void {
    this.supplierService.getAllSuppliers().subscribe({
      next: (data) => (this.suppliers = data),
      error: () => (this.errorMessage = 'Ошибка загрузки поставщиков.'),
    });
  }

  /** 🆕 Автогенерация номера партии */
  generateBatchNumber(): void {
    const today = new Date();
    this.newInventoryItem.batchNumber = this.batchService.generateBatchNumber(today, 1);
    this.showBatchNumberSuggestion = true;
  }

  /** 🆕 Обновление номера партии при изменении даты поставки */
  onDeliveryDateChange(): void {
    if (this.newInventoryItem.deliveryDate) {
      const deliveryDate = new Date(this.newInventoryItem.deliveryDate);
      const suggestedBatch = this.batchService.generateBatchNumber(deliveryDate, 1);

      if (!this.newInventoryItem.batchNumber || this.showBatchNumberSuggestion) {
        this.newInventoryItem.batchNumber = suggestedBatch;
      }
    }
  }

  /** 🆕 Валидация номера партии */
  validateBatchNumber(): boolean {
    if (!this.newInventoryItem.batchNumber) return true; // Опционально

    return this.batchService.isValidBatchNumber(this.newInventoryItem.batchNumber);
  }

  /** 📌 Обработчик выбора файла */
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.documentFile = input.files[0];
    }
  }

  /** 📌 Создаёт материальную ценность */
  createInventoryItem(form: NgForm): void {
    this.showDateError = false;
    this.errorMessage = null;

    // Валидация дат
    if (
      this.isDateInPast(this.newInventoryItem.expirationDate) ||
      this.isDateInPast(this.newInventoryItem.deliveryDate)
    ) {
      this.showDateError = true;
      return;
    }

    // Валидация формы
    if (form.invalid) {
      this.errorMessage = 'Заполните все обязательные поля!';
      return;
    }

    // Валидация склада
    if (!this.newInventoryItem.warehouseId) {
      this.errorMessage = 'Нельзя создать ценность без склада!';
      return;
    }

    // 🆕 Валидация номера партии
    if (this.newInventoryItem.batchNumber && !this.validateBatchNumber()) {
      this.errorMessage = 'Некорректный формат номера партии! Используйте формат: YYYY-MM-DD-XXXX';
      return;
    }

    // 🆕 Валидация числовых полей
    if (this.newInventoryItem.vatRate && (this.newInventoryItem.vatRate < 0 || this.newInventoryItem.vatRate > 100)) {
      this.errorMessage = 'Ставка НДС должна быть от 0 до 100%';
      return;
    }

    if (this.newInventoryItem.placesCount && this.newInventoryItem.placesCount < 1) {
      this.errorMessage = 'Количество грузовых мест должно быть больше 0';
      return;
    }

    if (this.newInventoryItem.cargoWeight && this.newInventoryItem.cargoWeight < 0) {
      this.errorMessage = 'Масса груза не может быть отрицательной';
      return;
    }

    if (this.documentFile) {
      this.uploadDocumentAndCreateItem(form);
    } else {
      this.createItem(form);
    }
  }

  private uploadDocumentAndCreateItem(form: NgForm): void {
    if (!this.documentFile) {
      this.createItem(form);
      return;
    }

    this.inventoryItemService.uploadDocument(this.documentFile).subscribe({
      next: (documentId: string) => {
        console.log('✅ Документ загружен, ID:', documentId);
        this.newInventoryItem.documentId = documentId;
        this.createItem(form);
      },
      error: (error) => {
        console.error('❌ Ошибка загрузки документа:', error);
        this.errorMessage = 'Ошибка загрузки документа.';
      },
    });
  }

  /** 📌 Создание элемента */
  private createItem(form: NgForm): void {
    this.inventoryItemService.createInventoryItem(this.newInventoryItem).subscribe({
      next: (createdItem) => {
        console.log('✅ Материальная ценность создана:', createdItem);
        this.errorMessage = null;
        form.resetForm();
        this.resetNewInventoryItem();
        this.router.navigate(['/'])
      },
      error: (error) => {
        console.error('❌ Ошибка создания:', error);
        this.errorMessage = 'Ошибка создания. Проверьте введённые данные.';
      },
    });
  }

  /** 📌 Сброс DTO после создания */
  private resetNewInventoryItem(): void {
    this.newInventoryItem = {
      name: '',
      uniqueCode: '',
      quantity: 1,
      estimatedValue: 0,
      expirationDate: '',
      supplierId: '',
      warehouseId: this.warehouseId,
      deliveryDate: '',
      documentId: '',
      // 🆕 Сброс новых полей
      batchNumber: '',
      measureUnit: 'шт',
      vatRate: 0,
      placesCount: 1,
      cargoWeight: 0,
      notes: ''
    };
    this.generateBatchNumber(); // Новый номер партии
  }

  isDateInPast(dateStr: string): boolean {
    if (!dateStr) return true;

    const inputDate = new Date(dateStr);
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    return inputDate < today;
  }

  /** 📌 Отмена и возврат назад */
  cancel(): void {
    window.history.back();
  }
}
