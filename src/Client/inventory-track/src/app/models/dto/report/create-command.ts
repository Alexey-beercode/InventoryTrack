import {DateSelect} from "./enums/data-select";
import {ReportType} from "./enums/report-type";

export interface CreateCommand {
  reportType: ReportType;
  dateSelect: DateSelect;
  companyId: string;
}
