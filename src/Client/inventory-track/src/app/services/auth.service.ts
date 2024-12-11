// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../environments/environment';
import { LoginDTO } from '../models/dto/auth/login-dto';
import { RegisterDTO } from '../models/dto/auth/register-dto';
import { AuthResponseDTO } from '../models/dto/auth/auth-response-dto';
import { AddUserToWarehouseDto } from '../models/dto/auth/add-user-to-warehouse-dto';
import { TokenService } from './token.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = environment.baseAuthUrl;

  constructor(
    private http: HttpClient,
    private tokenService: TokenService,
    private router: Router
  ) {}

  login(loginDto: LoginDTO): Observable<AuthResponseDTO> {
    return this.http
      .post<AuthResponseDTO>(`${this.apiUrl}${environment.apiUrls.auth.login}`, loginDto)
      .pipe(
        tap(response => {
          this.tokenService.setTokens(response.accessToken, response.userId.toString());
        })
      );
  }

  register(registerDto: RegisterDTO): Observable<AuthResponseDTO> {
    return this.http
      .post<AuthResponseDTO>(`${this.apiUrl}${environment.apiUrls.auth.register}`, registerDto)
      .pipe(
        tap(response => {
          this.tokenService.setTokens(response.accessToken, response.userId.toString());
        })
      );
  }

  addUserToWarehouse(dto: AddUserToWarehouseDto): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}${environment.apiUrls.auth.addUserToWarehouse}`,
      dto
    );
  }

  logout(): void {
    this.tokenService.clearTokens();
    this.router.navigate(['/login']);
  }
}
