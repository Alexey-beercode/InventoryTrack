import { Injectable, ErrorHandler, Injector } from '@angular/core';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root',
})
export class GlobalErrorHandlerService implements ErrorHandler {
  constructor(private injector: Injector) {}

  handleError(error: any): void {
    const notificationService = this.injector.get(NotificationService);

    let errorMessage = 'Произошла ошибка!';

    if (error instanceof Error) {
      errorMessage = error.message;
    } else if (typeof error === 'string') {
      errorMessage = error;
    } else if (error?.error?.message) {
      errorMessage = error.error.message;
    } else if (error?.message) {
      errorMessage = error.message;
    }

    console.error('Глобальная ошибка:', error);
    notificationService.showError(errorMessage);
  }
}
