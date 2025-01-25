import { Component } from '@angular/core';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { CommonModule } from '@angular/common';
import { EditUserComponent } from '../edit-user/edit-user.component';
import {BackButtonComponent} from "../../shared/back-button/back-button.component";

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.css'],
  standalone: true,
  imports: [EditUserComponent, HeaderComponent, FooterComponent, CommonModule, BackButtonComponent],
})
export class UserDetailsComponent {
  user: UserResponseDTO | null = null; // Может быть null
  showEditModal = false;

  onUserReceived(user: UserResponseDTO): void {
    this.user = user; // Устанавливаем объект пользователя
  }

  openEditModal(): void {
    if (this.user) {
      this.showEditModal = true; // Открываем модалку только если user не null
    }
  }

  closeEditModal(): void {
    this.showEditModal = false; // Закрываем модалку
  }

  onRefresh(): void {
    // Здесь можно реализовать обновление данных пользователя
  }
}
