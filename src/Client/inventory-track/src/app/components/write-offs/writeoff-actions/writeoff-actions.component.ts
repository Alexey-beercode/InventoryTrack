import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { WriteOffRequestResponseDto } from '../../../models/dto/writeoff-request/writeoff-request-response-dto';
import { WriteOffRequestService } from '../../../services/writeoff-request.service';
import { WarehouseService } from '../../../services/warehouse.service';
import { WarehouseResponseDto } from '../../../models/dto/warehouse/warehouse-response-dto';
import { RequestStatus } from "../../../models/dto/writeoff-request/enums/request-status.enum";
import { CommonModule } from "@angular/common";
import { ErrorMessageComponent } from "../../shared/error/error.component";
import {DataPipe} from "../../../pipes/data-pipe";
import {InventoryDocumentService} from "../../../services/inventory-document.service";
import {InventoryItemService} from "../../../services/inventory-item.service";

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
  @Input() showBatchInfo: boolean = false;

  documentFile!: File;
  warehouseName: string = 'Загрузка...';
  errorMessage: string | null = null;
  showUploadModal = false;
  generatedDocumentFile: File | null = null;
  selectedFile: File | null = null;
  isDocumentGenerated = false;
  isGeneratingDocument = false;


  constructor(
    private writeOffRequestService: WriteOffRequestService,
    private warehouseService: WarehouseService,
    private inventoryDocumentService: InventoryDocumentService,
    private inventoryItemService: InventoryItemService
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


/*  approve(): void {
    if (!this.documentFile) {
      this.errorMessage = "Выберите документ для прикрепления.";
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
            this.errorMessage = `Ошибка при загрузке документов: ${err.message}`;
          }
        });
      },
      error: (err) => {
        console.error("❌ Ошибка при одобрении заявки:", err);
        this.errorMessage = `Ошибка при одобрении заявки: ${err.message}`;
      }
    });
  }*/

  /** 📌 Отклонение заявки */
  reject(): void {
    this.writeOffRequestService.reject(this.writeOffRequest.id, this.userId).subscribe({
      next: () => {
        this.errorMessage = null;
        this.reload.emit();
      },
      error: (err) => {
        console.error('❌ Ошибка при отклонении заявки:', err);
        this.errorMessage = "Ошибка при отклонении заявки. Попробуйте позже.";
      }
    });
  }

  openUploadModal(): void {
    this.selectedFile = null;
    this.generatedDocumentFile = null;
    this.isDocumentGenerated = false;
    this.showUploadModal = true;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  generateDocument(): void {
    this.isGeneratingDocument = true;

    const dto = {
      isWriteOff: true,
      quantity: this.writeOffRequest.quantity,
      destinationWarehouseId: this.writeOffRequest.warehouseId,
      inventoryItemId: this.writeOffRequest.itemId,
      sourceWarehouseId:this.writeOffRequest.warehouseId
    };

    this.inventoryDocumentService.generateInventoryDocument(dto).subscribe({
      next: (blob) => {
        const today = new Date().toLocaleDateString('ru-RU'); // формат: дд.мм.гггг
        const fileName = `Акт о списании ${today}.docx`;
        const file = new File([blob], fileName, { type: blob.type });

        this.generatedDocumentFile = file;
        this.isDocumentGenerated = true;
        this.isGeneratingDocument = false;
      },
    error: () => {
        this.errorMessage = 'Ошибка генерации документа';
        this.isGeneratingDocument = false;
      }
    });
  }


  confirmUpload(): void {
    const fileToUpload = this.selectedFile || this.generatedDocumentFile;
    if (!fileToUpload) {
      this.errorMessage = 'Не выбран файл для загрузки';
      return;
    }

    // 📤 Сначала загружаем документ и получаем documentId
    this.inventoryItemService.uploadDocument(fileToUpload).subscribe({
      next: (documentId: string) => {
        // ✅ После загрузки вызываем approve с documentId
        this.writeOffRequestService.approve(this.writeOffRequest.id, this.userId, documentId).subscribe({
          next: () => {
            this.errorMessage = null;
            this.showUploadModal = false;
            this.reload.emit();
          },
          error: (err) => {
            console.error('❌ Ошибка при утверждении заявки:', err);
            this.errorMessage = 'Ошибка при утверждении заявки';
          }
        });
      },
      error: (err) => {
        console.error('❌ Ошибка при загрузке документа:', err);
        this.errorMessage = 'Ошибка при загрузке документа';
      }
    });
  }


  downloadDocument(documentId: string): void {
    this.inventoryDocumentService.downloadDocument(documentId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(
          new Blob([blob], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' }) // DOCX
        );
        const link = document.createElement('a');
        link.href = url;
        link.download = `writeoff-${documentId}.docx`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.errorMessage = 'Ошибка при скачивании документа';
      }
    });
  }


  protected readonly RequestStatus = RequestStatus;
}
