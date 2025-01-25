import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from "../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly ACCESS_TOKEN_KEY = 'accessToken';
  private readonly USER_ID_KEY = 'userId';
  private readonly baseUrl = environment.baseAuthUrl;
  private readonly apiUrls = environment.apiUrls.auth;

  constructor(private http: HttpClient) {}

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
    return this.checkTokenStatus();
  }

  checkTokenStatus(): Observable<boolean> {
    const accessToken = this.getAccessToken();
    console.log('Checking token status with token:', accessToken);

    if (!accessToken) {
      console.log('No token found');
      this.clearTokens();
      return of(false);
    }

    if (!this.isValidTokenFormat(accessToken)) {
      console.log('Invalid token format');
      this.clearTokens();
      return of(false);
    }

    return this.http.get(`${this.baseUrl}${this.apiUrls.token_status}`, {
      headers: { 'Authorization': `Bearer ${accessToken}` },
      observe: 'response',
      responseType: 'text'
    }).pipe(
      map(response => {
        console.log('Token status response:', response);
        return response.status === 200;
      })
    );
  }

  getUserRoles(): string[] {
    const token = this.getAccessToken();
    if (!token || !this.isValidTokenFormat(token)) {
      return [];
    }

    const decodedToken = this.decodeToken(token);
    if (decodedToken) {
      // Проверяем, есть ли в токене ClaimTypes.Role
      const rolesClaim = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']; // Обычный namespace для Role
      if (rolesClaim) {
        // Если роли представлены как массив
        return Array.isArray(rolesClaim) ? rolesClaim : [rolesClaim];
      }

      // Если поле ролей называется просто "role"
      const alternativeRoleClaim = decodedToken['role'];
      if (alternativeRoleClaim) {
        return Array.isArray(alternativeRoleClaim) ? alternativeRoleClaim : [alternativeRoleClaim];
      }
    }

    console.error('Roles not found in the token');
    return [];
  }


  private isValidTokenFormat(token: string): boolean {
    const parts = token.split('.');
    return parts.length === 3;
  }

  private decodeToken(token: string): any {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch (error) {
      console.error('Failed to decode token:', error);
      return null;
    }
  }

}
