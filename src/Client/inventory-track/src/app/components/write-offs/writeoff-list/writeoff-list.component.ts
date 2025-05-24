import { Component, OnInit } from '@angular/core';
import { WriteOffRequestService } from '../../../services/writeoff-request.service';
import { WarehouseService } from '../../../services/warehouse.service';
import { WriteOffRequestResponseDto } from '../../../models/dto/writeoff-request/writeoff-request-response-dto';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { RequestStatus } from '../../../models/dto/writeoff-request/enums/request-status.enum';
import { HeaderComponent } from "../../shared/header/header.component";
import { EnumLabelPipe } from "../../../pipes/enum-label.pipe";
import { LoadingSpinnerComponent } from "../../shared/loading-spinner/loading-spinner.component";
import { WriteOffActionsComponent } from "../writeoff-actions/writeoff-actions.component";
import { FooterComponent } from "../../shared/footer/footer.component";
import { CommonModule } from "@angular/common";
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-writeoff-list',
  templateUrl: './writeoff-list.component.html',
  styleUrls: ['./writeoff-list.component.css'],
  standalone: true,
  imports: [
    HeaderComponent,
    EnumLabelPipe,
    LoadingSpinnerComponent,
    WriteOffActionsComponent,
    FooterComponent,
    CommonModule
  ]
})
export class WriteOffListComponent implements OnInit {
  writeOffRequests: WriteOffRequestResponseDto[] = [];
  filterStatus: RequestStatus = RequestStatus.Requested;
  isLoading = false;
  userId!: string;
  companyId!: string;

  // 🆕 Группировка по партиям
  showGroupedView = false;
  batchGroups: Map<string, WriteOffRequestResponseDto[]> = new Map();

  // ✅ ИСПРАВЛЕНИЕ: Кешируем вычисленные данные
  batchGroupsArray: Array<{batchNumber: string, requests: WriteOffRequestResponseDto[], isBatch: boolean}> = [];
  hasBatchesToGroupCache = false;

  constructor(
    private writeOffRequestService: WriteOffRequestService,
    private warehouseService: WarehouseService
  ) {}

  ngOnInit(): void {
    // Ожидаем события от хедера
  }

  // ✅ ИСПРАВЛЕНИЕ: Получаем данные пользователя через header (userEmitter)
  onUserReceived(user: UserResponseDTO): void {
    if (user) {
      this.userId = user.id;
      console.log('📌 Получены данные пользователя через userEmitter:', { userId: this.userId });
    }
  }

  // ✅ ИСПРАВЛЕНИЕ: Получаем companyId через header (companyIdEmitter)
  onCompanyIdReceived(companyId: string): void {
    this.companyId = companyId;
    console.log('📌 Получен companyId через companyIdEmitter:', this.companyId);

    // Загружаем данные только после получения companyId
    if (this.companyId) {
      this.loadWriteOffRequestsByCompany();
    }
  }

  // ✅ ИСПРАВЛЕНИЕ: Новый метод для загрузки запросов по компании
  loadWriteOffRequestsByCompany(): void {
    if (!this.companyId) {
      console.error('❌ CompanyId не определен');
      return;
    }

    this.isLoading = true;
    console.log('🔍 Загрузка складов для компании:', this.companyId);

    // 1. Получаем все склады компании
    this.warehouseService.getWarehousesByCompany(this.companyId).subscribe({
      next: (warehouses) => {
        console.log('✅ Найдено складов:', warehouses.length);

        if (warehouses.length === 0) {
          this.writeOffRequests = [];
          this.updateCachedData();
          this.isLoading = false;
          return;
        }

        // 2. Получаем запросы для всех складов
        const requests$ = warehouses.map(warehouse =>
          this.writeOffRequestService.getByWarehouseId(warehouse.id)
        );

        forkJoin(requests$).subscribe({
          next: (allRequests) => {
            // 3. Объединяем все запросы
            this.writeOffRequests = allRequests
              .flat()
              .filter(request => request.status.name === this.getStatusString(this.filterStatus));

            console.log('✅ Всего найдено запросов:', this.writeOffRequests.length);
            this.updateCachedData();
            this.isLoading = false;
          },
          error: (error) => {
            console.error('❌ Ошибка загрузки запросов:', error);
            this.isLoading = false;
          }
        });
      },
      error: (error) => {
        console.error('❌ Ошибка загрузки складов:', error);
        this.isLoading = false;
      }
    });
  }

  // ✅ ИСПРАВЛЕНИЕ: Преобразуем enum в строку
  private getStatusString(status: RequestStatus): string {
    switch (status) {
      case RequestStatus.Requested: return 'Запрошено';
      case RequestStatus.Created: return 'Создано';
      case RequestStatus.Rejected: return 'Отклонено';
      default: return 'Запрошено';
    }
  }

  onStatusChange(status: RequestStatus): void {
    this.filterStatus = status;
    this.loadWriteOffRequestsByCompany();
  }

  // ✅ ИСПРАВЛЕНИЕ: Обновляем кешированные данные
  private updateCachedData(): void {
    this.groupRequestsByBatch();
    this.batchGroupsArray = this.calculateBatchGroupsArray();
    this.hasBatchesToGroupCache = this.calculateHasBatchesToGroup();
  }

  // 🆕 Группировка запросов по партиям
  private groupRequestsByBatch(): void {
    this.batchGroups.clear();

    this.writeOffRequests.forEach(request => {
      const batchKey = request.batchNumber || 'individual';

      if (!this.batchGroups.has(batchKey)) {
        this.batchGroups.set(batchKey, []);
      }

      this.batchGroups.get(batchKey)!.push(request);
    });
  }

  // ✅ ИСПРАВЛЕНИЕ: Приватный метод для вычисления групп
  private calculateBatchGroupsArray(): Array<{batchNumber: string, requests: WriteOffRequestResponseDto[], isBatch: boolean}> {
    const result: Array<{batchNumber: string, requests: WriteOffRequestResponseDto[], isBatch: boolean}> = [];

    this.batchGroups.forEach((requests, batchNumber) => {
      result.push({
        batchNumber: batchNumber,
        requests: requests,
        isBatch: batchNumber !== 'individual' && requests.length > 1
      });
    });

    // Сортируем: сначала партии, потом индивидуальные
    return result.sort((a, b) => {
      if (a.isBatch && !b.isBatch) return -1;
      if (!a.isBatch && b.isBatch) return 1;
      return a.batchNumber.localeCompare(b.batchNumber);
    });
  }

  // ✅ ИСПРАВЛЕНИЕ: Публичный геттер для шаблона
  get getBatchGroupsArray(): Array<{batchNumber: string, requests: WriteOffRequestResponseDto[], isBatch: boolean}> {
    return this.batchGroupsArray;
  }

  // 🆕 Переключение режима отображения
  toggleGroupedView(): void {
    this.showGroupedView = !this.showGroupedView;
    console.log('🔄 Переключен режим отображения:', this.showGroupedView ? 'группировка' : 'список');
  }

  // ✅ ИСПРАВЛЕНИЕ: Приватный метод для вычисления
  private calculateHasBatchesToGroup(): boolean {
    return Array.from(this.batchGroups.values()).some(requests =>
      requests.length > 1 && requests[0].batchNumber
    );
  }

  // ✅ ИСПРАВЛЕНИЕ: Публичный геттер для шаблона
  get hasBatchesToGroup(): boolean {
    return this.hasBatchesToGroupCache;
  }

  // ✅ ИСПРАВЛЕНИЕ: Метод для получения сводки партии
  getBatchSummary(requests: WriteOffRequestResponseDto[]): {totalQuantity: number, warehousesCount: number} {
    const totalQuantity = requests.reduce((sum, req) => sum + req.quantity, 0);
    const warehousesCount = new Set(requests.map(req => req.warehouseId)).size;

    return { totalQuantity, warehousesCount };
  }

  // ✅ ИСПРАВЛЕНИЕ: Метод для получения статуса партии
  getBatchStatus(requests: WriteOffRequestResponseDto[]): {allSameStatus: boolean, status: string} {
    const statuses = requests.map(req => req.status.name);
    const uniqueStatuses = [...new Set(statuses)];

    return {
      allSameStatus: uniqueStatuses.length === 1,
      status: uniqueStatuses.length === 1 ? uniqueStatuses[0] : 'Смешанный'
    };
  }

  // ✅ ИСПРАВЛЕНИЕ: Метод для перезагрузки (вызывается из дочерних компонентов)
  loadWriteOffRequests(): void {
    this.loadWriteOffRequestsByCompany();
  }

  protected readonly RequestStatus = RequestStatus;
}
