import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { InventoryItemService } from '../../../services/inventory-item.service';
import { WriteOffRequestService } from '../../../services/writeoff-request.service';
import { WriteOffReasonService } from '../../../services/writeoff-reason.service';
import { TokenService } from '../../../services/token.service';
import { UserService } from '../../../services/user.service';
import { InventoryItemResponseDto } from '../../../models/dto/inventory-item/inventory-item-response-dto';
import { WriteOffReasonResponseDto } from '../../../models/dto/writeoff-reason/writeoff-reason-response-dto';
import { CreateWriteOffRequestDto } from '../../../models/dto/writeoff-request/create-writeoff-request-dto';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BackButtonComponent } from "../../shared/back-button/back-button.component";
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
  items: InventoryItemResponseDto[] = [];
  writeOffReasons: WriteOffReasonResponseDto[] = [];
  selectedReasonId: string = '';
  anotherReason: string = '';
  selectedItemId: string = '';
  quantity: number = 1;
  maxQuantity: number = 1;
  errorMessage: string | null = null;

  constructor(
    private inventoryItemService: InventoryItemService,
    private writeOffRequestService: WriteOffRequestService,
    private writeOffReasonService: WriteOffReasonService,
    private tokenService: TokenService,
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUserWarehouse();
  }

  /** 📌 Загружаем ID склада пользователя */
  loadUserWarehouse(): void {
    const userId = this.tokenService.getUserId();
    if (!userId) {
      this.errorMessage = "❌ Ошибка: пользователь не найден.";
      return;
    }

    this.userService.getById(userId).subscribe({
      next: (user: UserResponseDTO) => {
        if (user.warehouseId) {
          this.warehouseId = user.warehouseId;
          this.loadInventoryItems();
          this.loadWriteOffReasons();
        } else {
          this.errorMessage = "❌ Ошибка: у пользователя нет склада.";
        }
      },
      error: () => {
        this.errorMessage = "❌ Ошибка загрузки данных пользователя.";
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
        this.errorMessage = "❌ Ошибка загрузки списка товаров.";
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
        this.errorMessage = "❌ Ошибка загрузки причин списания.";
      },
    });
  }

  /** 📌 Обновляет максимальное количество списания при смене товара */
  updateMaxQuantity(): void {
    const selectedItem = this.items.find(item => item.id === this.selectedItemId);
    this.maxQuantity = selectedItem ? selectedItem.quantity : 1;
    if (this.quantity > this.maxQuantity) {
      this.quantity = this.maxQuantity;
    }
  }

  /** 📌 Отправка запроса на списание */
  submitWriteOffRequest(form: NgForm): void {
    if (form.invalid || this.quantity > this.maxQuantity) {
      this.errorMessage = "⚠️ Заполните все обязательные поля и укажите корректное количество!";
      return;
    }

    const request: CreateWriteOffRequestDto = {
      itemId: this.selectedItemId,
      warehouseId: this.warehouseId,
      quantity: this.quantity,
      reasonId: this.selectedReasonId,
      anotherReason: this.selectedReasonId === 'other' ? this.anotherReason : undefined
    };

    this.writeOffRequestService.create(request).subscribe({
      next: () => {
        alert('✅ Запрос на списание успешно создан!');
        this.router.navigate(['/']);
      },
      error: () => {
        this.errorMessage = "❌ Ошибка при создании запроса.";
      },
    });
  }

  /** 📌 Метод отмены и возврата */
  cancel(): void {
    this.router.navigate(['/']);
  }
}
