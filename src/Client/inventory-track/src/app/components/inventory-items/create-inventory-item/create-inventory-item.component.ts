import { Component, OnInit } from '@angular/core';
import {FormsModule, NgForm} from '@angular/forms';
import { InventoryItemService } from '../../../services/inventory-item.service';
import { SupplierService } from '../../../services/supplier.service';
import { CreateInventoryItemDto } from '../../../models/dto/inventory-item/create-inventory-item-dto';
import { SupplierResponseDto } from '../../../models/dto/supplier/supplier-response-dto';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { CommonModule } from '@angular/common';
import { BackButtonComponent } from "../../shared/back-button/back-button.component";
import { ErrorMessageComponent } from '../../shared/error/error.component';

@Component({
  selector: 'app-create-inventory-item',
  templateUrl: './create-inventory-item.component.html',
  styleUrls: ['./create-inventory-item.component.css'],
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
export class CreateInventoryItemComponent implements OnInit {
  newInventoryItem: CreateInventoryItemDto = {
    name: '',
    uniqueCode: '',
    quantity: 1,
    estimatedValue: 0,
    expirationDate: '',  // Дата как строка `yyyy-MM-dd`
    supplierId: '',
    warehouseId: '',
    deliveryDate: '',
    documentFile: null as any,
  };

  suppliers: SupplierResponseDto[] = [];
  warehouseId: string = '';
  errorMessage: string | null = null;

  constructor(
    private inventoryItemService: InventoryItemService,
    private supplierService: SupplierService
  ) {}

  ngOnInit(): void {
    this.loadSuppliers();
    this.initDefaultDates();
  }

  /** 📌 Инициализируем даты в `yyyy-MM-dd` формате для `ngModel` */
  private initDefaultDates(): void {
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(today.getDate() + 1);

    this.newInventoryItem.expirationDate = this.formatDateForInput(tomorrow);
    this.newInventoryItem.deliveryDate = this.formatDateForInput(today);
  }

  /** 📌 Получаем ID склада из хедера */
  onUserReceived(warehouseId: string): void {
    if (warehouseId) {
      this.warehouseId = warehouseId;
      this.newInventoryItem.warehouseId = warehouseId;
    } else {
      this.errorMessage = "❌ Не удалось получить склад пользователя.";
    }
  }

  /** 📌 Загружаем список поставщиков */
  loadSuppliers(): void {
    this.supplierService.getAllSuppliers().subscribe({
      next: (data) => (this.suppliers = data),
      error: () => (this.errorMessage = 'Ошибка загрузки поставщиков.'),
    });
  }

  /** 📌 Обработчик выбора файла */
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.newInventoryItem.documentFile = input.files[0];
    } else {
      this.errorMessage = '⚠️ Выберите файл!';
    }
  }

  /** 📌 Преобразуем `Date` → `yyyy-MM-dd` (для `<input type="date">`) */
  private formatDateForInput(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  /** 📌 Создаёт материальную ценность */
  createInventoryItem(form: NgForm): void {
    console.log('🔥 Кнопка "Сохранить" нажата');

    if (form.invalid) {
      console.warn('⚠️ Форма невалидна, отправка отменена');
      this.errorMessage = '⚠️ Заполните все обязательные поля!';
      return;
    }

    this.errorMessage = null;

    // 🔹 Преобразуем строки `yyyy-MM-dd` в `Date`
    const expirationDate = new Date(this.newInventoryItem.expirationDate);
    const deliveryDate = new Date(this.newInventoryItem.deliveryDate);

    console.log('📆 Даты перед отправкой:');
    console.log('   📌 expirationDate:', expirationDate.toISOString());
    console.log('   📌 deliveryDate:', deliveryDate.toISOString());

    console.log('📤 Отправляем данные:', {
      name: this.newInventoryItem.name,
      uniqueCode: this.newInventoryItem.uniqueCode,
      quantity: this.newInventoryItem.quantity,
      estimatedValue: this.newInventoryItem.estimatedValue,
      expirationDate: expirationDate,
      deliveryDate: deliveryDate,
      supplierId: this.newInventoryItem.supplierId,
      warehouseId: this.newInventoryItem.warehouseId,
      documentFile: this.newInventoryItem.documentFile,
    });

    const formData = new FormData();
    formData.append('name', this.newInventoryItem.name.trim());
    formData.append('uniqueCode', this.newInventoryItem.uniqueCode.trim());
    formData.append('quantity', this.newInventoryItem.quantity.toString());
    formData.append('estimatedValue', this.newInventoryItem.estimatedValue.toString());
    formData.append('expirationDate', expirationDate.toISOString());
    formData.append('deliveryDate', deliveryDate.toISOString());
    formData.append('supplierId', this.newInventoryItem.supplierId);
    formData.append('warehouseId', this.newInventoryItem.warehouseId);
    formData.append('documentFile', this.newInventoryItem.documentFile);

    this.inventoryItemService.createInventoryItem(formData).subscribe({
      next: () => {
        console.log('✅ Материальная ценность успешно добавлена!');
        alert('✅ Материальная ценность успешно добавлена!');
        form.resetForm();
        this.initDefaultDates(); // Сбрасываем даты после отправки
      },
      error: (error) => {
        console.error('❌ Ошибка создания:', error);
        this.errorMessage = '❌ Ошибка создания. Проверьте введённые данные.';
      },
    });
  }

  cancel(): void {
    window.history.back();
  }
}
