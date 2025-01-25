import { Component, Input, Output, EventEmitter } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-edit-user',
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.css'],
  standalone: true,
  imports: [FormsModule],
})
export class EditUserComponent {
  @Input() user!: UserResponseDTO;
  @Output() closeModal = new EventEmitter<void>(); // Переименован Output
  @Output() refresh = new EventEmitter<void>();

  constructor(private userService: UserService) {}

  updateUser(): void {
    if (!this.user) return;

    this.userService
      .update({
        id: this.user.id,
        firstName: this.user.firstName,
        lastName: this.user.lastName,
      })
      .subscribe({
        next: () => {
          this.refresh.emit();
          this.closeModal.emit(); // Используем переименованный Output
        },
        error: (error) => console.error('Ошибка обновления пользователя:', error),
      });
  }

  handleClose(): void {
    this.closeModal.emit(); // Используем переименованный Output
  }

  onBackdropClick(event: MouseEvent): void {
    this.handleClose(); // Закрываем модалку при клике на фон
  }
}
