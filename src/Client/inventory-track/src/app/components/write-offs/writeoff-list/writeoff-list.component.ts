import { Component, OnInit } from '@angular/core';
import { WriteOffRequestService } from '../../../services/writeoff-request.service';
import { WriteOffRequestResponseDto } from '../../../models/dto/writeoff-request/writeoff-request-response-dto';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { RequestStatus } from '../../../models/dto/writeoff-request/enums/request-status.enum';
import {HeaderComponent} from "../../shared/header/header.component";
import {EnumLabelPipe} from "../../../pipes/enum-label.pipe";
import {LoadingSpinnerComponent} from "../../shared/loading-spinner/loading-spinner.component";
import {WriteOffActionsComponent} from "../writeoff-actions/writeoff-actions.component";
import {FooterComponent} from "../../shared/footer/footer.component";
import {CommonModule} from "@angular/common";
import {RequestStatusResponseDto} from "../../../models/dto/writeoff-request/request-status-response-dto";
import {DocumentInfoResponseDto} from "../../../models/dto/document/document-info-response-dto";
import {WriteOffReasonResponseDto} from "../../../models/dto/writeoff-reason/writeoff-reason-response-dto";

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
  filterStatus: RequestStatus = RequestStatus.Requested; // По умолчанию "Необработанные"
  isLoading = false;
  userId!: string; // ID текущего пользователя
  constructor(private writeOffRequestService:WriteOffRequestService) {
  }
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

    const filter = {
      status: this.filterStatus,
    };

    this.writeOffRequestService.getFiltered(filter).subscribe({
      next: (data) => {
        this.writeOffRequests = data;
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

  protected readonly RequestStatus = RequestStatus;
}
