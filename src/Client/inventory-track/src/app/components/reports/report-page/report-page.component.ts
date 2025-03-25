import { Component, ViewChild, OnInit } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { ReportResponseDto } from '../../../models/dto/report/report-response-dto';
import { ReportType } from '../../../models/dto/report/enums/report-type';
import { CreateCommand } from '../../../models/dto/report/create-command';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import { EnumLabelPipe } from "../../../pipes/enum-label.pipe";
import { DataPipe } from "../../../pipes/data-pipe";
import { ErrorMessageComponent } from "../../shared/error/error.component";
import { DateSelect } from "../../../models/dto/report/enums/data-select";
import { UserResponseDTO } from "../../../models/dto/user/user-response-dto"; // DTO пользователя

@Component({
  selector: 'app-report-page',
  templateUrl: './report-page.component.html',
  styleUrls: ['./report-page.component.css'],
  standalone: true,
  imports: [
    HeaderComponent,
    FooterComponent,
    CommonModule,
    FormsModule,
    LoadingSpinnerComponent,
    EnumLabelPipe,
    DataPipe,
    ErrorMessageComponent
  ],
})
export class ReportPageComponent implements OnInit {
  @ViewChild(HeaderComponent) headerComponent!: HeaderComponent; // Доступ к HeaderComponent

  reports: ReportResponseDto[] = [];
  isLoading = false;
  showModal = false;
  reportTypes = Object.values(ReportType).filter(value => typeof value === 'number');
  dateSelectOptions = Object.values(DateSelect).filter(value => typeof value === 'number');
  selectedReportType: ReportType = ReportType.StockState;
  selectedDateSelect: DateSelect = DateSelect.Month;
  companyId: string | null = null;
  user: UserResponseDTO | null = null;
  errorMessage: string = '';

  constructor(private reportService: ReportService) {}

  ngOnInit(): void {
    this.getUserFromHeader();
  }

  /** 📌 Получаем пользователя из HeaderComponent */
  getUserFromHeader(): void {
    setTimeout(() => {
      if (this.headerComponent?.user) {
        this.user = this.headerComponent.user;
        this.companyId = this.headerComponent.company?.id || null;
        console.log("✅ Получен пользователь из Header:", this.user);
        console.log("✅ Получен companyId:", this.companyId);
        this.loadReports();
      } else {
        console.error("❌ Пользователь или компания не найдены в HeaderComponent");
      }
    }, 300);
  }

  /** 📌 Загружаем отчеты */
  loadReports(): void {
    if (!this.companyId) {
      console.warn("⚠️ companyId не задан, отчеты не загружаются.");
      return;
    }

    console.log("📥 Запрос отчетов для companyId:", this.companyId);
    this.isLoading = true;

    this.reportService.getByCompanyId(this.companyId).subscribe({
      next: (data) => {
        console.log("📄 Полученные отчеты:", data);
        this.reports = this.sortReportsByDate(data);
        this.isLoading = false;
      },
      error: () => {
        console.error("❌ Ошибка загрузки отчетов");
        this.reports = [];
        this.isLoading = false;
      },
    });
  }

  /** 📌 Сортировка отчетов по дате */
  sortReportsByDate(reports: ReportResponseDto[]): ReportResponseDto[] {
    return reports.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
  }

  /** 📌 Открыть модальное окно */
  openModal(): void {
    this.showModal = true;
  }

  /** 📌 Закрыть модальное окно */
  closeModal(): void {
    this.showModal = false;
  }

  /** 📌 Создать отчет */
  createReport(): void {
    if (!this.companyId || !this.user) {
      alert('Компания или пользователь не определены');
      return;
    }

    const isAccountant = this.user.roles.some(role => role.name === 'Бухгалтер');
    const warehouseId = isAccountant ? null : this.user.warehouseId; // Если не бухгалтер, добавляем warehouseId

    const command: CreateCommand = {
      reportType: Number(this.selectedReportType),
      dateSelect: Number(this.selectedDateSelect),
      companyId: this.companyId,
      warehouseId: warehouseId ?? undefined, // Добавляем warehouseId, если он есть
    };

    console.log("📤 Создание отчета:", command);

    this.isLoading = true;
    this.reportService.createReport(command).subscribe({
      next: () => {
        console.log("✅ Отчет создан успешно");
        this.showModal = false;
        this.loadReports();
      },
      error: () => {
        console.error("❌ Ошибка создания отчета");
        this.isLoading = false;
      },
    });
  }

  /** 📌 Скачивание отчета */
  downloadReport(reportId: string, reportName: string): void {
    console.log("📥 Скачивание отчета:", reportId);
    this.reportService.getById(reportId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `${reportName}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        console.log("✅ Отчет скачан:", reportName);
      },
      error: () => {
        this.errorMessage = 'Не удалось скачать файл';
        console.error("Ошибка скачивания отчета");
      }
    });
  }
}
