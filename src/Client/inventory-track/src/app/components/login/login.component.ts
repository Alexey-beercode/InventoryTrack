// components/login/login.component.ts
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import {CommonModule, NgOptimizedImage} from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { TokenService } from '../../services/token.service';
import {LoginDTO} from "../../models/dto/auth/login-dto";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NgOptimizedImage],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  username = '';
  password = '';
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private tokenService: TokenService,
    private router: Router
  ) {}

  ngOnInit() {
    // Добавим логирование для отладки
    console.log('Checking token status...');
    const currentToken = this.tokenService.getAccessToken();
    console.log('Current token:', currentToken);

    this.tokenService.isLoggedIn().subscribe({
      next: (isValid) => {
        console.log('Token status check result:', isValid);
        if (isValid) {
          this.router.navigate(['/home']);
        }
      },
      error: (error) => {
        console.error('Token status check error:', error);
      }
    });
  }

  onSubmit(): void {
    this.errorMessage = '';

    if (!this.username || !this.password) {
      this.errorMessage = 'Please enter username and password';
      return;
    }

    const loginData:LoginDTO = {
      login: this.username,
      password: this.password
    };

    console.log('Attempting login with:', loginData);

    this.authService.login(loginData).subscribe({
      next: (response) => {
        if (response && response.accessToken) {
          console.log('Login successful');
          this.tokenService.setTokens(response.accessToken,response.userId);
          this.router.navigate(['/home']);
        } else {
          this.errorMessage = 'Invalid login response';
        }
      },
      error: (error) => {
        console.error('Login error:', error);
        if (error.status === 401) {
          this.errorMessage = 'Invalid username or password';
        } else {
          this.errorMessage = 'An error occurred during login. Please try again.';
        }
        this.tokenService.clearTokens(); // Очищаем старые токены при ошибке
      }
    });
  }
}
