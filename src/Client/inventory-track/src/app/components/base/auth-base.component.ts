// components/base/auth-base.component.ts
import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
import { TokenService } from '../../services/token.service';

@Component({
  template: ''
})
export abstract class AuthBaseComponent implements OnDestroy {
  protected destroy$ = new Subject<void>();
  protected loading = false;
  protected error: string | null = null;

  constructor(
    protected authService: AuthService,
    protected tokenService: TokenService,
    protected router: Router
  ) {
    this.checkAuth();
  }

  private checkAuth(): void {
    this.tokenService.checkTokenStatus()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (isValid) => {
          if (!isValid) {
            this.router.navigate(['/login']);
          }
        }
      });
  }

  protected handleError(error: any): void {
    console.error('Error occurred:', error);
    this.error = 'An error occurred. Please try again.';
    this.loading = false;

    if (error.status === 401) {
      this.tokenService.clearTokens();
      this.router.navigate(['/login']);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
