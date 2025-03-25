import { Component, OnInit } from '@angular/core';
import { SupplierService } from '../../../services/supplier.service';
import { SupplierResponseDto } from '../../../models/dto/supplier/supplier-response-dto';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { BackButtonComponent } from '../../shared/back-button/back-button.component';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { CreateSupplierComponent } from '../create-supplier/create-supplier.component';
import {CommonModule} from "@angular/common";

@Component({
  selector: 'app-supplier-list',
  templateUrl: 'suppliers-list.component.html',
  styleUrls: ['suppliers-list.component.css'],
  standalone: true,
  imports: [HeaderComponent, FooterComponent, BackButtonComponent, LoadingSpinnerComponent, CreateSupplierComponent,CommonModule],
})
export class SupplierListComponent implements OnInit {
  companyId: string | null = null; // companyId получаем из HeaderComponent
  suppliers: SupplierResponseDto[] = [];
  isLoading = false;
  showCreateModal = false;
  showDeleteConfirmModal = false;
  selectedSupplierId: string | null = null;
  selectedSupplierName: string = '';

  constructor(private supplierService: SupplierService) {}

  ngOnInit(): void {
    // companyId будет получен через событие
  }

  onCompanyIdReceived(companyId: string): void {
    this.companyId = companyId
    console.log(companyId);
    this.loadSuppliers();
  }
  openDeleteConfirmModal(id: string, name: string) {
    this.selectedSupplierId = id;
    this.selectedSupplierName = name;
    this.showDeleteConfirmModal = true;
  }

  deleteSupplier() {
    if (!this.selectedSupplierId) return;

    this.supplierService.deleteSupplier(this.selectedSupplierId).subscribe({
      next: () => {
        this.loadSuppliers();
        this.showDeleteConfirmModal = false;
      },
      error: err => {
        console.error('Ошибка при удалении:', err);
      }
    });
  }

  loadSuppliers(): void {
    if (!this.companyId) return;

    this.isLoading = true;
    this.supplierService.getByCompanyId(this.companyId).subscribe({
      next: (data) => {
        this.suppliers = data;
        this.isLoading = false;
      },
      error: () => {
        this.suppliers = [];
        this.isLoading = false;
      },
    });
  }

  openCreateModal(): void {
    this.showCreateModal = true;
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
    this.loadSuppliers(); // Обновляем список после создания контрагента
  }
}
