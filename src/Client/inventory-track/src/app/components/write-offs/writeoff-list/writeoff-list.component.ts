import { Component, OnInit } from '@angular/core';
import { WriteOffRequestService } from '../../../services/writeoff-request.service';
import { WriteOffRequestResponseDto } from '../../../models/dto/writeoff-request/writeoff-request-response-dto';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { RequestStatus } from '../../../models/dto/writeoff-request/enums/request-status.enum';
import { HeaderComponent } from "../../shared/header/header.component";
import { EnumLabelPipe } from "../../../pipes/enum-label.pipe";
import { LoadingSpinnerComponent } from "../../shared/loading-spinner/loading-spinner.component";
import { WriteOffActionsComponent } from "../writeoff-actions/writeoff-actions.component";
import { FooterComponent } from "../../shared/footer/footer.component";
import { CommonModule } from "@angular/common";

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

  // 🆕 Группировка по партиям
  showGroupedView = false;
  batchGroups: Map<string, WriteOffRequestResponseDto[]> = new Map();

  constructor(private writeOffRequestService: WriteOffRequestService) {}

  ngOnInit(): void {
    // Ожидаем события от хедера
  }

  onUserReceived(user: UserResponseDTO): void {
    if (user) {
      this.userId = user.id;
      this.loadWriteOffRequests();
    }
  }

  loadWriteOffRequests(): void {
    this.isLoading = true;

    this.writeOffRequestService.getByStatus(this.filterStatus.toString()).subscribe({
      next: (data) => {
        this.writeOffRequests = data;
        this.groupRequestsByBatch(); // 🆕 Группируем по партиям
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  onStatusChange(status: RequestStatus): void {
    this.filterStatus = status;
    this.loadWriteOffRequests();
  }

  // 🆕 Группировка запросов по партиям
  groupRequestsByBatch(): void {
    this.batchGroups.clear();

    this.writeOffRequests.forEach(request => {
      const batchKey = request.batchNumber || 'individual'; // Индивидуальные запросы

      if (!this.batchGroups.has(batchKey)) {
        this.batchGroups.set(batchKey, []);
      }

      this.batchGroups.get(batchKey)!.push(request);
    });
  }

  // 🆕 Получить данные для отображения групп
  getBatchGroupsArray(): Array<{batchNumber: string, requests: WriteOffRequestResponseDto[], isBatch: boolean}> {
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

  // 🆕 Переключение режима отображения
  toggleGroupedView(): void {
    this.showGroupedView = !this.showGroupedView;
  }

  // 🆕 Проверить, есть ли партии для группировки
  hasBatchesToGroup(): boolean {
    return Array.from(this.batchGroups.values()).some(requests => requests.length > 1 && requests[0].batchNumber);
  }

  // 🆕 Получить сводную информацию о партии
  getBatchSummary(requests: WriteOffRequestResponseDto[]): {totalQuantity: number, warehousesCount: number} {
    const totalQuantity = requests.reduce((sum, req) => sum + req.quantity, 0);
    const warehousesCount = new Set(requests.map(req => req.warehouseId)).size;

    return { totalQuantity, warehousesCount };
  }

  // 🆕 Проверить статус всех запросов в партии
  getBatchStatus(requests: WriteOffRequestResponseDto[]): {allSameStatus: boolean, status: string} {
    const statuses = requests.map(req => req.status.name);
    const uniqueStatuses = [...new Set(statuses)];

    return {
      allSameStatus: uniqueStatuses.length === 1,
      status: uniqueStatuses.length === 1 ? uniqueStatuses[0] : 'Смешанный'
    };
  }

  protected readonly RequestStatus = RequestStatus;
}
