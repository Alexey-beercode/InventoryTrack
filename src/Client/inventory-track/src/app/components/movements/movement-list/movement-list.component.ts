import { Component, OnInit } from '@angular/core';
import { MovementRequestResponseDto } from '../../../models/dto/movement-request/movement-request-response-dto';
import { MovementRequestService } from '../../../services/movement-request.service';
import { InventoryItemService } from '../../../services/inventory-item.service';
import { WarehouseService } from '../../../services/warehouse.service';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { TokenService } from '../../../services/token.service';
import { HeaderComponent } from "../../shared/header/header.component";
import { LoadingSpinnerComponent } from "../../shared/loading-spinner/loading-spinner.component";
import { FooterComponent } from "../../shared/footer/footer.component";
import { ErrorMessageComponent } from "../../shared/error/error.component";
import { NgForOf, NgIf } from "@angular/common";
import { MovementRequestStatus } from '../../../models/dto/movement-request/enums/movement-request-status.enum';
import { ChangeStatusDto } from '../../../models/dto/movement-request/change-status-dto';
import {InventoryDocumentService} from "../../../services/inventory-document.service";

@Component({
  selector: 'app-movement-list',
  templateUrl: './movement-list.component.html',
  styleUrls: ['./movement-list.component.css'],
  standalone: true,
  imports: [
    HeaderComponent,
    LoadingSpinnerComponent,
    FooterComponent,
    ErrorMessageComponent,
    NgIf,
    NgForOf
  ]
})
export class MovementListComponent implements OnInit {
  movements: MovementRequestResponseDto[] = [];
  items: Record<string, string> = {}; // Хранение itemId -> itemName
  warehouses: Record<string, string> = {}; // Хранение warehouseId -> warehouseName
  isLoading = false;
  errorMessage: string | null = null;
  user: UserResponseDTO | null = null;
  userRoles: string[] = [];
  showUploadModal = false;
  selectedRequestId: string | null = null;
  selectedFile: File | null = null;
  isAccountant = false;
  companyId: string | null = null;
  private userReceived = false;
  private companyIdReceived = false;
  generatedDocumentFile: File | null = null;
  isDocumentGenerated = false;
  isGeneratingDocument = false;




  constructor(
    private movementService: MovementRequestService,
    private itemService: InventoryItemService,
    private warehouseService: WarehouseService,
    private inventoryDocumentService:InventoryDocumentService,
  ) {}

  ngOnInit(): void {}

  onCompanyIdReceived(companyId: string): void {
    this.companyId = companyId;
    this.companyIdReceived = true;

    this.tryLoadAccountantMovements();
  }

  private tryLoadAccountantMovements(): void {
    if (this.isAccountant && this.userReceived && this.companyIdReceived) {
      this.loadMovementsForAccountant();
    } else if (!this.isAccountant && this.userReceived) {
      this.loadMovements();
    }
  }

  onUserReceived(user: UserResponseDTO | null): void {
    if (!user) {
      this.errorMessage = "Пользовательские данные не получены.";
      return;
    }

    this.user = user;
    this.userRoles = this.user.roles.map(r => r.name);
    this.isAccountant = this.userRoles.includes('Бухгалтер');
    this.userReceived = true;

    this.tryLoadAccountantMovements();
  }

  generateDocument(): void {
    if (!this.selectedRequestId) return;

    const movement = this.movements.find(m => m.id === this.selectedRequestId);
    if (!movement) {
      this.errorMessage = 'Перемещение не найдено.';
      return;
    }

    const dto = {
      isWriteOff: false,
      quantity: movement.quantity,
      warehouseId: movement.destinationWarehouseId,
      sourceWarehouseId: movement.sourceWarehouseId,
      inventoryItemId: movement.itemId
    };

    this.isGeneratingDocument = true;
    this.inventoryDocumentService.generateInventoryDocument(dto).subscribe({
      next: (file) => {
        const today = new Date().toLocaleDateString('ru-RU');
        const fileName = `ТТН ${today}.xls`;
        const generatedFile = new File([file], fileName, { type: 'application/vnd.ms-excel' });
        this.generatedDocumentFile = generatedFile;
        this.isDocumentGenerated = true;
        this.isGeneratingDocument = false;
      },
      error: () => {
        this.errorMessage = 'Ошибка генерации документа';
        this.isGeneratingDocument = false;
      }
    });
  }



  openUploadModal(requestId: string): void {
    this.selectedRequestId = requestId;
    this.showUploadModal = true;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  confirmUpload(): void {
    const fileToUpload = this.selectedFile || this.generatedDocumentFile;
    if (!fileToUpload || !this.selectedRequestId) return;

    this.itemService.uploadDocument(fileToUpload).subscribe({
      next: (documentId) => {
        this.movementService.addDocumentToRequest(documentId, this.selectedRequestId!).subscribe({
          next: () => {
            this.movementService.finalApprove(this.selectedRequestId!).subscribe({
              next: () => {
                this.tryLoadAccountantMovements();
                this.resetModal();
              },
              error: () => this.errorMessage = 'Ошибка при финальном утверждении'
            });
          },
          error: () => this.errorMessage = 'Ошибка при прикреплении документа'
        });
      },
      error: () => this.errorMessage = 'Ошибка загрузки документа'
    });
  }

  resetModal(): void {
    this.showUploadModal = false;
    this.selectedFile = null;
    this.generatedDocumentFile = null;
    this.selectedRequestId = null;
    this.isDocumentGenerated = false;
  }



  /** 📌 Загружаем все перемещения */
  loadMovements(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.movementService.getByWarehouse(this.user?.warehouseId!).subscribe({
      next: (data) => {
        this.movements = data;
        this.loadItemNames();
        this.loadWarehouseNames();
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Ошибка загрузки перемещений.';
        this.isLoading = false;
      }
    });
  }

  /** 📌 Подгружаем названия товаров */
  loadItemNames(): void {
    const itemIds = new Set(this.movements.map((m) => m.itemId));
    itemIds.forEach((itemId) => {
      this.itemService.getInventoryItemById(itemId).subscribe({
        next: (item) => this.items[itemId] = item.name,
        error: () => this.errorMessage = `Ошибка загрузки товара.`
      });
    });
  }

  /** 📌 Подгружаем названия складов */
  loadWarehouseNames(): void {
    const warehouseIds = new Set(
      this.movements.flatMap(m => [m.sourceWarehouseId, m.destinationWarehouseId])
    );

    warehouseIds.forEach((warehouseId) => {
      if (!this.warehouses[warehouseId]) {
        this.warehouseService.getWarehouseById(warehouseId).subscribe({
          next: (warehouse) => this.warehouses[warehouseId] = warehouse.name,
          error: () => this.errorMessage = `Ошибка загрузки склада.`
        });
      }
    });
  }

  /** 📌 Возвращает название товара */
  getItemName(itemId: string): string {
    return this.items[itemId] || 'Загрузка...';
  }

  /** 📌 Возвращает название склада */
  getWarehouseName(warehouseId: string): string {
    return this.warehouses[warehouseId] || 'Загрузка...';
  }

  /** ✅ Одобрить перемещение (только `Processing`) */
  approveMovement(movementId: string): void {
    if (!this.user) {
      this.errorMessage = 'Ошибка: пользователь не найден!';
      return;
    }

    const dto: ChangeStatusDto = {
      userId: this.user.id,
      requestId: movementId
    };

    this.movementService.approve(dto).subscribe({
      next: () => {
        if (this.isAccountant) {
          this.tryLoadAccountantMovements();
        } else {
          this.loadMovements();
        }
      },
      error: () => alert("❌ Ошибка при одобрении перемещения!")
    });
  }

  /** ❌ Отклонить перемещение (только `Processing`) */
  rejectMovement(movementId: string): void {
    if (!this.user) {
      this.errorMessage = 'Ошибка: пользователь не найден!';
      return;
    }

    const dto: ChangeStatusDto = {
      userId: this.user.id,
      requestId: movementId
    };

    this.movementService.reject(dto).subscribe({
      next: () => {
        if (this.isAccountant) {
          this.tryLoadAccountantMovements();
        } else {
          this.loadMovements();
        }
      },
      error: () => alert("❌ Ошибка при отклонении перемещения!")
    });
  }
  loadMovementsForAccountant(): void {
    if (!this.companyId) {
      this.errorMessage = 'Компания не определена.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.warehouseService.getWarehousesByCompany(this.companyId).subscribe({
      next: (warehouses) => {
        const warehouseIds = warehouses.map(w => w.id);
        const requests = warehouseIds.map(id => this.movementService.getByWarehouse(id));

        Promise.all(requests.map(r => r.toPromise()))
          .then(results => {
            const allMovements = results.flat();
            const uniqueMap = new Map<string, MovementRequestResponseDto>();

            for (const m of allMovements) {
              uniqueMap.set(m!.id, m!);
            }

            this.movements = Array.from(uniqueMap.values());
            this.loadItemNames();
            this.loadWarehouseNames();
          })
          .catch(() => {
            this.errorMessage = 'Ошибка загрузки перемещений бухгалтера.';
          })
          .finally(() => {
            this.isLoading = false;
          });
      },
      error: () => {
        this.errorMessage = 'Ошибка загрузки складов компании.';
        this.isLoading = false;
      }
    });
  }
  downloadDocument(documentId: string): void {
    this.inventoryDocumentService.downloadDocument(documentId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(
          new Blob([blob], { type: 'application/vnd.ms-excel' }) // 📌 MIME для .xls
        );
        const link = document.createElement('a');
        link.href = url;
        link.download = `movement-document-${documentId}.xls`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.errorMessage = 'Ошибка при скачивании документа';
      }
    });
  }

  protected readonly MovementRequestStatus = MovementRequestStatus;
}
