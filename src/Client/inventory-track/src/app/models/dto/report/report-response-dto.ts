// src/app/models/dto/report/report-response-dto.ts
import {ReportType} from "./enums/report-type";
import {DateSelect} from "./enums/data-select";

export interface ReportResponseDto {
  id: string;
  name: string;
  reportType: ReportType;
  dateSelect: DateSelect;
  createdAt: string;
}
