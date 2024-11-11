import { Injectable } from '@angular/core';
import {catchError, map, Observable, of} from 'rxjs';
import { HttpClient } from '@angular/common/http';
import {environment} from "../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly ACCESS_TOKEN_KEY = 'accessToken';
  private readonly USER_ID_KEY = 'userId';
  private readonly apiUrl = `${environment.apiUrl}/api/auth`;
  constructor(private http: HttpClient,) {
  }

  setTokens(accessToken: string, userId: string): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, accessToken);
    localStorage.setItem(this.USER_ID_KEY, userId);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  getUserId(): string | null {
    return localStorage.getItem(this.USER_ID_KEY);
  }

  clearTokens(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.USER_ID_KEY);
  }

  isLoggedIn(): Observable<boolean> {
    return of(!!this.getAccessToken()); // Возвращаем Observable
  }

  checkTokenStatus(): Observable<boolean> {
    const accessToken = this.getAccessToken();
    console.log('Checking token status with token:', accessToken);

    if (!accessToken) {
      console.log('No token found');
      this.clearTokens();
      return of(false);
    }

    // Добавим проверку формата токена
    if (!this.isValidTokenFormat(accessToken)) {
      console.log('Invalid token format');
      this.clearTokens();
      return of(false);
    }

    return this.http.get(`${this.apiUrl}/token-status`, {
      headers: { 'Authorization': `Bearer ${accessToken}` },
      observe: 'response',
      responseType: 'text'
    }).pipe(
      map(response => {
        console.log('Token status response:', response);
        return response.status === 200;
      }))
  }
  private isValidTokenFormat(token: string): boolean {
    // Простая проверка формата JWT
    const parts = token.split('.');
    return parts.length === 3;
  }
}
