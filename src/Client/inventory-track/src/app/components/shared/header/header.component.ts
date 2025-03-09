import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TokenService } from '../../../services/token.service';
import { UserService } from '../../../services/user.service';
import { CompanyService } from '../../../services/company.service';
import { UserResponseDTO } from '../../../models/dto/user/user-response-dto';
import { CompanyResponseDTO } from '../../../models/dto/company/company-response-dto';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { BackButtonComponent } from '../back-button/back-button.component';
import { ErrorMessageComponent } from '../error/error.component';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    ErrorMessageComponent,
  ],
  standalone: true,
})
export class HeaderComponent implements OnInit {
  @Input() title: string = 'Inventory Track';
  @Input() isAuthorize: boolean = false;

  @Output() userEmitter = new EventEmitter<UserResponseDTO | null>(); // Эмиттер для пользователя
  @Output() companyEmitter = new EventEmitter<CompanyResponseDTO | null>(); // Эмиттер для компании
  @Output() companyIdEmitter = new EventEmitter<string>(); // Эмиттер для companyId

  navLinks: { path: string; label: string }[] = [];
  user: UserResponseDTO | null = null;
  company: CompanyResponseDTO | null = null;
  showCreateModal: boolean = false;
  newCompany = { name: '', legalAddress: '', unp: 0, postalAddress: '' };
  submitted: boolean = false;
  errorMessage: string | null = null;
  isLoading: boolean = false;
  canAddCompany: boolean = false;

  constructor(
    private tokenService: TokenService,
    private router: Router,
    private userService: UserService,
    private companyService: CompanyService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.setupNavLinks();
    this.loadUser();
  }

  private setupNavLinks(): void {
    const roles = this.tokenService.getUserRoles();
    if (roles.includes('Accountant')) {
      this.canAddCompany = true;
      this.navLinks.push(
        { path: '/warehouses', label: 'Склады' },
        { path: '/write-offs', label: 'Списания' },
        { path: '/reports', label: 'Отчёты' },
        { path: '/suppliers', label: 'Контрагенты' },
        { path: '/workers', label: 'Работники' }
      );
    }

    if (roles.includes('Warehouse Manager')) {
      this.navLinks.push(
        { path: '/warehouse-view', label: 'Мой склад' },
        { path: '/movements', label: 'Заявки на перемещение' },
        {path: '/create-item',label: 'Оформить материальную ценность'},
        {path: '/create-item',label: 'Сформировать отчет'},
        { path: '/create-write-off', label: 'Оформить списание' },

      );
    }

    if (roles.includes('Department Head')) {
      this.navLinks.push(
        { path: '/warehouse-view', label: 'Склады' },
        { path: '/create-movement', label: 'Оформить перемещение' },
        { path: '/create-write-off', label: 'Оформить списание' },
        {path: '/create-item',label: 'Сформировать отчет'},
      );
    }
  }

  private loadUser(): void {
    const userId = this.tokenService.getUserId();
    if (userId) {
      this.isLoading = true;
      this.userService.getById(userId).subscribe({
        next: (user) => {
          this.user = user;
          this.userEmitter.emit(user); // Эмитим пользователя
          this.loadCompany(user.id);
        },
        error: (error) => {
          this.errorMessage = 'Ошибка при загрузке пользователя';
          console.error(error);

          this.userEmitter.emit(null); // Эмитим null, если ошибка
          this.isLoading = false;
        },
      });
    } else {
      // Проверяем, если текущий маршрут не является "login", перенаправляем
      if (!this.isAuthorize) {
        this.router.navigate(['login']);
      }
    }
  }

  private loadCompany(userId: string): void {
    this.companyService.getByUserId(userId).subscribe({
      next: (company) => {
        this.company = company;
        this.companyEmitter.emit(company); // Эмитим компанию
        if (this.company?.id) {
          this.companyIdEmitter.emit(this.company.id); // Эмитим companyId
        }
        this.isLoading = false;
      },
      error: () => {
        this.company = null;
        this.companyEmitter.emit(null); // Эмитим null, если ошибка
        this.isLoading = false;
      },
    });
  }

  createCompany(form: NgForm): void {
    this.submitted = true;
    if (!form.valid) return;

    const userId = this.tokenService.getUserId();
    if (userId) {
      this.isLoading = true;
      const newCompany = { ...this.newCompany, responsibleUserId: userId };
      this.companyService.create(newCompany).subscribe({
        next: () => {
          this.showCreateModal = false;
          this.loadCompany(userId);
        },
        error: (error) => {
          this.errorMessage = 'Ошибка при создании компании';
          console.error(error);
          this.isLoading = false;
        },
      });
    }
  }

  logout(): void {
    this.authService.logout();
  }

  navigateToUserDetails(): void {
    if (this.user) {
      this.router.navigate(['/user-details']);
    }
  }

}
