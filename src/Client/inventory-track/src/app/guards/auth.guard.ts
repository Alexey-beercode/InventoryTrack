// src/app/guards/auth.guard.ts

import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { UserService } from '../services/user.service';
import { Observable, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private tokenService: TokenService,
    private router: Router,
    private userService: UserService
  ) {}

  canActivate(): Observable<boolean> {
    if (!this.tokenService.isLoggedIn()) {
      console.log('Токен не в порядке');
      this.router.navigate(['/login']);
      return of(false); // Возвращаем Observable с `false`
    }

    return this.loadUser().pipe(
      map((isUserLoaded) => {
        if (isUserLoaded) {
          console.log('Токен в порядке, пользователь загружен');
          return true;
        } else {
          console.log('Ошибка при загрузке пользователя');
          this.router.navigate(['/login']);
          return false;
        }
      }),
      catchError((error) => {
        console.error('Ошибка в AuthGuard:', error);
        this.router.navigate(['/login']);
        return of(false); // Возвращаем Observable с `false` при ошибке
      })
    );
  }

  private loadUser(): Observable<boolean> {
    const userId = this.tokenService.getUserId();
    if (!userId) {
      return of(false); // Если `userId` отсутствует, сразу возвращаем `false`
    }

    return this.userService.getById(userId).pipe(
      map((user) => {
        return true; // Если пользователь успешно загружен
      }),
      catchError((error) => {
        console.error('Ошибка при загрузке пользователя:', error);
        return of(false); // Если произошла ошибка, возвращаем `false`
      })
    );
  }
}
