import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CreateWarehouseDto } from '../../../models/dto/warehouse/create-warehouse-dto';
import { WarehouseService } from '../../../services/warehouse.service';

@Component({
  selector: 'app-create-warehouse',
  templateUrl: './create-warehouse.component.html',
  styleUrls: ['./create-warehouse.component.css'],
  standalone: true,
  imports: [FormsModule],
})
export class CreateWarehouseComponent {
  @Input() companyId!: string;
  @Input() userId!: string;
  @Output() close = new EventEmitter<void>();
  @Output() refreshList = new EventEmitter<void>();

  newWarehouse: CreateWarehouseDto = {
    name: '',
    type: 0, // Значение по умолчанию (0 для производственного склада)
    location: '',
    companyId: '',
    responsiblePersonId: '',
  };

  constructor(private warehouseService: WarehouseService) {}

  createWarehouse(form: NgForm): void {
    if (!form.valid) return;

    // Устанавливаем companyId и responsiblePersonId
    this.newWarehouse.companyId = this.companyId;
    this.newWarehouse.responsiblePersonId = this.userId;
    this.newWarehouse.type=Number(this.newWarehouse.type)

    // Отправляем запрос на создание склада
    this.warehouseService.createWarehouse(this.newWarehouse).subscribe({
      next: () => {
        this.refreshList.emit(); // Обновляем список складов
        this.close.emit(); // Закрываем модалку
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
