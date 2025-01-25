import { Component, Input, Output, EventEmitter } from '@angular/core';
import { WriteOffRequestResponseDto } from '../../../models/dto/writeoff-request/writeoff-request-response-dto';
import { WriteOffRequestService } from '../../../services/writeoff-request.service';
import {HeaderComponent} from "../../shared/header/header.component";
import {DataPipe} from "../../../pipes/data-pipe";
import {FooterComponent} from "../../shared/footer/footer.component";
import {CommonModule} from "@angular/common";
import {RequestStatus} from "../../../models/dto/writeoff-request/enums/request-status.enum";

@Component({
  selector: 'app-writeoff-actions',
  templateUrl: './writeoff-actions.component.html',
  styleUrls: ['./writeoff-actions.component.css'],
  standalone: true,
  imports: [
    HeaderComponent,
    DataPipe,
    FooterComponent,
    CommonModule
  ]
})
export class WriteOffActionsComponent {
  @Input() writeOffRequest!: WriteOffRequestResponseDto;
  @Input() userId!: string; // Принимаем ID пользователя
  @Output() reload = new EventEmitter<void>();
  documentFile!: File;

  constructor(private writeOffRequestService: WriteOffRequestService) {}

  approve(): void {
    if (!this.documentFile) {
      alert('Выберите документ для прикрепления.');
      return;
    }

    const formData = new FormData();
    formData.append('Id', this.writeOffRequest.id);
    formData.append('ApprovedByUserId', this.userId); // Используем переданный userId
    formData.append('Documents', this.documentFile);

    this.writeOffRequestService.approve(formData).subscribe({
      next: () => {
        alert('Заявка одобрена');
        this.reload.emit();
      },
      error: () => alert('Ошибка при одобрении заявки'),
    });
  }

  reject(): void {
    this.writeOffRequestService.reject(this.writeOffRequest.id, this.userId).subscribe({
      next: () => {
        alert('Заявка отклонена');
        this.reload.emit();
      },
      error: () => alert('Ошибка при отклонении заявки'),
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.documentFile = input.files[0];
    }
  }

  protected readonly RequestStatus = RequestStatus;
}
