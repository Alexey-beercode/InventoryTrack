import { Component, OnInit } from "@angular/core";
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { UserResponseDTO } from '../../models/dto/user/user-response-dto';
import { TokenService } from "../../services/token.service";
import { Router } from "@angular/router";
import { AuthBaseComponent } from "../base/auth-base.component";
import { CompanyResponseDTO } from "../../models/dto/company/company-response-dto";
import { CompanyService } from "../../services/company.service";
import {Location, NgIf} from "@angular/common";
import {FormsModule, NgForm} from "@angular/forms";
import { CreateCompanyDTO } from "../../models/dto/company/create-company-dto";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  imports: [
    NgIf,
    FormsModule
  ],
  standalone: true
})
export class HomeComponent extends AuthBaseComponent implements OnInit {
  user: UserResponseDTO | null = null;
  companyByUser: CompanyResponseDTO | null = null;
  showCreateModal: boolean = false;
  submitted: boolean = false;
  errorMessage: string | null = null;
  newCompany: CreateCompanyDTO = {
    name: '',
    legalAddress: '',
    unp: 0,
    postalAddress: '',
    responsibleUserId: '' // Это поле не будет заполняться в форме, но может быть установлено программно
  };

  constructor(
    protected override authService: AuthService,
    protected override tokenService: TokenService,
    protected override router: Router,
    private userService: UserService,
    private companyService: CompanyService,
    private location:Location
  ) {
    super(authService, tokenService, router);
  }

  ngOnInit(): void {
    this.tokenService.isLoggedIn().subscribe({
      next: (isValid) => {
        if (isValid) {
          const userId = this.tokenService.getUserId();
          this.loadUser(userId);
        } else {
          this.router.navigate(['/login']);
        }
      },
      error: () => this.router.navigate(['/login'])
    });
  }

  loadUser(id: string | null): void {
    this.userService.getById(id).subscribe({
      next: (user: UserResponseDTO) => {
        this.user = user;
        this.loadCompany(user.id); // Загрузка компании после загрузки пользователя
      },
      error: (error) => {
        this.handleError(error);
      }
    });
  }

  goBack(){
    this.location.back();
  }

  loadCompany(userId: string | null): void {
    this.companyService.getByUserId(userId).subscribe({
      next: (company: CompanyResponseDTO) => {
        this.companyByUser = company;
      }
    });
  }

  createCompany(form: NgForm) {
    this.submitted = true; // Установите флаг в true при попытке отправки формы
      this.newCompany.responsibleUserId = this.tokenService.getUserId() as string; // Установка ID ответственного лица
      this.companyService.create(this.newCompany).subscribe({
        next: (response) => {
          console.log('Компания успешно создана', response);
          this.showCreateModal = false; // Закрываем модальное окно
          this.loadCompany(this.tokenService.getUserId()); // Обновляем информацию о компании
        },
        error: (error) => {
          this.errorMessage=error.errorMessage
          console.error('Ошибка при создании компании', error);
        }
      });
  }

  showCompanyModal() {
    this.showCreateModal = true;
  }
  logout(): void {
    this.loading = true;
    this.authService.logout();
  }

  getWorkers() {
    this.router.navigate(['workers'])
  }
}
