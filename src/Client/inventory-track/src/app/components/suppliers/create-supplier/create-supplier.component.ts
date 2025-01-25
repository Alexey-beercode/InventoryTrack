import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { SupplierService } from '../../../services/supplier.service';
import { CreateSupplierDto } from '../../../models/dto/supplier/create-supplier-dto';
import { ErrorMessageComponent } from '../../shared/error/error.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-supplier',
  templateUrl: './create-supplier.component.html',
  styleUrls: ['./create-supplier.component.css'],
  standalone: true,
  imports: [FormsModule, ErrorMessageComponent, CommonModule],
})
export class CreateSupplierComponent {
  @Input() companyId!: string; // Получаем companyId от родителя
  @Output() close = new EventEmitter<void>();
  @Output() refreshList = new EventEmitter<void>();

  newSupplier: CreateSupplierDto = {
    name: '',
    phoneNumber: '',
    postalAddress: '',
    accountNumber: '',
    companyId: '',
  };

  submitted = false;
  errorMessage: string | null = null;

  constructor(private supplierService: SupplierService) {}

  createSupplier(form: NgForm): void {
    this.submitted = true;
    if (!form.valid) return;

    this.newSupplier.companyId = this.companyId; // Присваиваем companyId перед отправкой
    console.log("компания :" + this.newSupplier.companyId);
    this.supplierService.createSupplier(this.newSupplier).subscribe({
      next: () => {
        this.refreshList.emit(); // Обновляем список
        this.close.emit(); // Закрываем модалку
      },
      error: (error) => {
        this.errorMessage = 'Ошибка при создании контрагента.';
        console.error(error);
      },
    });
  }


  closeModal(): void {
    this.close.emit();
  }
}
