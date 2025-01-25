import { Component, OnInit } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { ReportResponseDto } from '../../../models/dto/report/report-response-dto';
import { ReportType} from '../../../models/dto/report/enums/report-type';
import { DateSelect} from '../../../models/dto/report/enums/data-select';
import { CreateCommand } from '../../../models/dto/report/create-command';
import { HeaderComponent } from '../../shared/header/header.component';
import { FooterComponent } from '../../shared/footer/footer.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoadingSpinnerComponent } from '../../shared/loading-spinner/loading-spinner.component';
import {EnumLabelPipe} from "../../../pipes/enum-label.pipe";
import {DataPipe} from "../../../pipes/data-pipe";
import {ErrorMessageComponent} from "../../shared/error/error.component";

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
  reports: ReportResponseDto[] = [];
  isLoading = false;
  showModal = false;
  reportTypes = Object.values(ReportType).filter(value => typeof value === 'number');
  dateSelectOptions = Object.values(DateSelect).filter(value => typeof value === 'number');
  selectedReportType: ReportType = ReportType.StockState;
  selectedDateSelect: DateSelect = DateSelect.Month;
  pageNumber = 1;
  pageSize = 10;
  totalReports = 0;
  companyId: string = ''; // Получаем из хедера
  errorMessage: string='';

  constructor(private reportService: ReportService) {}

  ngOnInit(): void {
    this.loadReports();
  }
  downloadReport(reportId: string,reportName : string): void {
    this.reportService.getById(reportId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `report-${reportId}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        this.errorMessage = 'Не удалось скачать файл';
      }
    });
  }



  loadReports(): void {
    this.isLoading = true;
    this.reportService
      .getPaginated({
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
      })
      .subscribe({
        next: (data) => {
          this.reports = data;
          this.isLoading = false;
        },
        error: () => {
          this.reports = [];
          this.isLoading = false;
        },
      });
  }

  openModal(): void {
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }

  createReport(): void {
    if (!this.companyId) {
      alert('Компания не выбрана');
      return;
    }

    const command: CreateCommand = {
      reportType: Number(this.selectedReportType),
      dateSelect: Number(this.selectedDateSelect),
      companyId: this.companyId,
    };

    this.isLoading = true;
    this.reportService.createReport(command).subscribe({
      next: () => {
        this.showModal = false;
        this.loadReports();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }
}
