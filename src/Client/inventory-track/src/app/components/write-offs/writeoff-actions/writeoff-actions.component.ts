import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { WriteOffRequestResponseDto } from '../../../models/dto/writeoff-request/writeoff-request-response-dto';
import { WriteOffRequestService } from '../../../services/writeoff-request.service';
import { WarehouseService } from '../../../services/warehouse.service';
import { WarehouseResponseDto } from '../../../models/dto/warehouse/warehouse-response-dto';
import { RequestStatus } from "../../../models/dto/writeoff-request/enums/request-status.enum";
import { CommonModule } from "@angular/common";
import { ErrorMessageComponent } from "../../shared/error/error.component";
import {DataPipe} from "../../../pipes/data-pipe";

@Component({
  selector: 'app-writeoff-actions',
  templateUrl: './writeoff-actions.component.html',
  styleUrls: ['./writeoff-actions.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ErrorMessageComponent,
    DataPipe
  ]
})
export class WriteOffActionsComponent implements OnInit {
  @Input() writeOffRequest!: WriteOffRequestResponseDto;
  @Input() userId!: string;
  @Output() reload = new EventEmitter<void>();

  documentFile!: File;
  warehouseName: string = 'Загрузка...';
  errorMessage: string | null = null;

  constructor(
    private writeOffRequestService: WriteOffRequestService,
    private warehouseService: WarehouseService
  ) {}

  ngOnInit(): void {
    this.loadWarehouseName();
  }

  /** 📌 Загружаем название склада */
  loadWarehouseName(): void {
    this.warehouseService.getWarehouseById(this.writeOffRequest.warehouseId).subscribe({
      next: (warehouse: WarehouseResponseDto) => {
        this.warehouseName = warehouse.name;
      },
      error: () => {
        this.warehouseName = 'Неизвестный склад';
      }
    });
  }

  /** 📌 Обработчик выбора файла */
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.documentFile = input.files[0];
    }
  }

  approve(): void {
    if (!this.documentFile) {
      this.errorMessage = "⚠️ Выберите документ для прикрепления.";
      return;
    }

    // 1️⃣ Обновляем статус заявки
    this.writeOffRequestService.approve(this.writeOffRequest.id, this.userId).subscribe({
      next: () => {
        console.log("✅ Заявка успешно одобрена, загружаем документы...");

        // 2️⃣ Загружаем документы
        const formData = new FormData();
        formData.append("documents", this.documentFile);

        this.writeOffRequestService.uploadDocuments(this.writeOffRequest.id, formData).subscribe({
          next: () => {
            console.log("✅ Документы загружены!");
            this.errorMessage = null;
            this.reload.emit();
          },
          error: (err) => {
            console.error("❌ Ошибка при загрузке документов:", err);
            this.errorMessage = `❌ Ошибка при загрузке документов: ${err.message}`;
          }
        });
      },
      error: (err) => {
        console.error("❌ Ошибка при одобрении заявки:", err);
        this.errorMessage = `❌ Ошибка при одобрении заявки: ${err.message}`;
      }
    });
  }

  /** 📌 Отклонение заявки */
  reject(): void {
    this.writeOffRequestService.reject(this.writeOffRequest.id, this.userId).subscribe({
      next: () => {
        this.errorMessage = null;
        this.reload.emit();
      },
      error: (err) => {
        console.error('❌ Ошибка при отклонении заявки:', err);
        this.errorMessage = "❌ Ошибка при отклонении заявки. Попробуйте позже.";
      }
    });
  }

  protected readonly RequestStatus = RequestStatus;
}
