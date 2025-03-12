import {Pipe, PipeTransform} from "@angular/core";

@Pipe({
  name: 'enumLabel',
  standalone: true,
})
export class EnumLabelPipe implements PipeTransform {
  private labels: { [key: string]: { [key: number]: string } } = {
    ReportType: {
      0: 'Состояние склада',
      1: 'Внутренние перемещения',
      2: 'Списания'
    },
    DateSelect: {
      0: 'День',
      1: 'Неделя',
      2: 'Месяц',
      3: 'Год',
    },
    RequestStatus: {
      0: 'Не указано',
      1: 'Запрошено',
      2: 'Создано',
      3: 'Отклонено',
    },
  };

  transform(value: number, enumName: string): string {
    const label = this.labels[enumName]?.[value];
    return label || 'Неизвестное значение';
  }
}
