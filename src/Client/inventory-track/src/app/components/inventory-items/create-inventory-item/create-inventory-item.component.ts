import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { InventoryItemService } from '../../../services/inventory-item.service';
import { SupplierService } from '../../../services/supplier.service';
import { CreateInventoryItemDto } from '../../../models/dto/inventory-item/create-inventory-item-dto';
import { SupplierResponseDto } from '../../../models/dto/supplier/supplier-response-dto';
import { CommonModule } from '@angular/common';
import { ErrorMessageComponent } from '../../shared/error/error.component';
import { BackButtonComponent } from '../../shared/back-button/back-button.component';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import {Router} from "@angular/router";

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
    documentId: ''
  };

  documentFile: File | null = null;
  suppliers: SupplierResponseDto[] = [];
  warehouseId: string = '';
  errorMessage: string | null = null;
  showDateError = false;


  constructor(
    private inventoryItemService: InventoryItemService,
    private supplierService: SupplierService,
    private router:Router
  ) {}

  ngOnInit(): void {
    this.loadSuppliers();
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

    if (
      this.isDateInPast(this.newInventoryItem.expirationDate) ||
      this.isDateInPast(this.newInventoryItem.deliveryDate)
    ) {
      this.showDateError = true;
      return;
    }
    if (form.invalid) {
      this.errorMessage = 'Заполните все обязательные поля!';
      return;
    }

    if (!this.newInventoryItem.warehouseId) {
      this.errorMessage = 'Нельзя создать ценность без склада!';
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
        // ✅ Обнуляем DTO после создания
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
      warehouseId: this.warehouseId, // Оставляем склад
      deliveryDate: '',
      documentId: ''
    };
  }

  isDateInPast(dateStr: string): boolean {
    if (!dateStr) return true;

    const inputDate = new Date(dateStr);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // сбрасываем время

    return inputDate < today;
  }


  /** 📌 Отмена и возврат назад */
  cancel(): void {
    window.history.back();
  }
}
