import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CreateWarehouseDto } from '../../../models/dto/warehouse/create-warehouse-dto';
import { WarehouseService } from '../../../services/warehouse.service';
import { UserService } from '../../../services/user.service';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { NgForOf, NgIf } from '@angular/common';

@Component({
  selector: 'app-create-warehouse',
  templateUrl: './create-warehouse.component.html',
  styleUrls: ['./create-warehouse.component.css'],
  standalone: true,
  imports: [FormsModule, NgIf, NgForOf],
})
export class CreateWarehouseComponent implements OnInit {
  @Input() companyId!: string;
  @Input() userId!: string;
  @Output() close = new EventEmitter<void>();
  @Output() refreshList = new EventEmitter<void>();

  users: UserResponseDTO[] = [];
  filteredUsers: UserResponseDTO[] = [];
  assignManager: boolean = false; // Флаг: добавлять ли начальника склада
  noSuitableUsers: boolean = false; // Флаг: нет подходящих пользователей

  newWarehouse: CreateWarehouseDto = {
    name: '',
    type: 0,
    location: '',
    companyId: '',
    responsiblePersonId: null,
    accountantId: this.userId,
  };

  constructor(private warehouseService: WarehouseService, private userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    if (!this.companyId) return;
    this.userService.getByCompanyId(this.companyId).subscribe({
      next: (users) => {
        this.users = users;
        this.filterUsersByType(); // Фильтруем при загрузке
      },
      error: () => {
        console.error('Ошибка загрузки пользователей.');
      },
    });
  }

  filterUsersByType(): void {
    const warehouseType = Number(this.newWarehouse.type);
    this.filteredUsers = this.users.filter(user =>
      warehouseType === 0
        ? user.roles.some(role => role.name === 'Начальник подразделения') // Производственный склад
        : user.roles.some(role => role.name === 'Начальник склада') // Внутренний склад
    );

    this.noSuitableUsers = this.filteredUsers.length === 0;

    // Если список пуст, автоматически ставим userId
    if (this.noSuitableUsers || !this.assignManager) {
      this.newWarehouse.responsiblePersonId = this.userId;
    } else {
      this.newWarehouse.responsiblePersonId = null; // Даем возможность выбрать
    }
  }

  createWarehouse(form: NgForm): void {
    if (!form.valid) return;

    this.newWarehouse.companyId = this.companyId;
    this.newWarehouse.type = Number(this.newWarehouse.type);
    this.newWarehouse.responsiblePersonId = (this.assignManager && !this.noSuitableUsers)
      ? this.newWarehouse.responsiblePersonId
      : this.userId;

    this.newWarehouse.accountantId = this.userId;

    this.warehouseService.createWarehouse(this.newWarehouse).subscribe({
      next: () => {
        // Получаем склад по ответственному лицу
        this.warehouseService.getWarehousesByResponsiblePerson(this.newWarehouse.responsiblePersonId!)
          .subscribe({
            next: (warehouses) => {
              if (warehouses.length > 0) {
                const createdWarehouse = warehouses[warehouses.length - 1]; // Берем последний созданный склад

                this.userService.addUserToWarehouse({
                  userId: this.newWarehouse.responsiblePersonId!,
                  warehouseId: createdWarehouse.id
                }).subscribe({
                  next: () => {
                    this.refreshList.emit();
                    this.close.emit();
                  },
                  error: (err) => {
                    console.error('Ошибка назначения пользователя на склад:', err);
                  }
                });
              } else {
                console.error('Не удалось найти созданный склад.');
                this.refreshList.emit();
                this.close.emit();
              }
            },
            error: (err) => {
              console.error('Ошибка получения склада по ответственному пользователю:', err);
              this.refreshList.emit();
              this.close.emit();
            }
          });
      },
      error: (err) => {
        console.error('Ошибка при создании склада:', err);
      },
    });
  }



  closeModal(): void {
    this.close.emit();
  }
}
