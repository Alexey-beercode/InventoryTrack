import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { authInterceptor } from './app/services/auth.interceptor';
import { ErrorHandler } from '@angular/core'; // Правильный импорт
import { GlobalErrorHandlerService } from './app/services/error-handler.service';

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(withInterceptors([authInterceptor])),
    provideRouter(routes),
    { provide: ErrorHandler, useClass: GlobalErrorHandlerService } // Регистрация глобального обработчика
  ],
}).catch(err => console.error(err));
