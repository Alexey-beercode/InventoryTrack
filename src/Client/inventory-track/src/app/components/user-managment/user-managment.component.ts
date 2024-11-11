import {Component, OnInit} from '@angular/core';
import {UserService} from '../../services/user.service';
import {RoleService} from '../../services/role.service';
import {UserResponseDTO} from '../../models/dto/user/user-response-dto';
import {RoleDTO} from '../../models/dto/role/role-dto';
import {RegisterUserToCompanyDTO} from '../../models/dto/user/register-user-to-company-dto';
import {AuthBaseComponent} from "../base/auth-base.component";
import {Router} from "@angular/router";
import {TokenService} from "../../services/token.service";
import {CompanyService} from "../../services/company.service";
import {AuthService} from "../../services/auth.service";
import {map, Observable} from "rxjs";
import {CommonModule, Location, NgIf} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {UserResponseModel} from "../../models/user/register-user-to-company-model";

@Component({
  selector: 'app-user-management',
  templateUrl: 'user-managment.component.html',
  styleUrls: ['user-managment.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule,NgIf]
})
export class UserManagementComponent extends AuthBaseComponent implements OnInit {
  users: UserResponseModel[] = [];
  roles: RoleDTO[] = [];
  showAddUserModal: boolean = false;
  newUser: RegisterUserToCompanyDTO = {
    roleId: '',
    firstName: '',
    lastName: '',
    login: '',
    password: '',
    companyId: ''
  };

  constructor(
    protected override authService: AuthService,
    protected override tokenService: TokenService,
    protected override router: Router,
    private userService: UserService,
    private roleService: RoleService,
    private companyService: CompanyService,
    private location: Location
  ) {
    super(authService, tokenService, router);
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
  }

  mapUserToModel(user: UserResponseDTO): UserResponseModel {
    return {
      id: user.id,
      login: user.login,
      firstName: user.firstName,
      lastName: user.lastName,
      role: (user.roles && user.roles.length > 0) ? user.roles[0] : null, // Проверяем, что user.roles не null и имеет элементы
      selectedRoleId: (user.roles && user.roles.length > 0) ? user.roles[0].id : null // То же самое для selectedRoleId
    };
  }

  loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users = users.map(user => {
          const model = this.mapUserToModel(user);
          model.selectedRoleId = model.role ? model.role.id : null; // Устанавливаем выбранный id роли
          return model;
        });
      },
      error: (error) => this.handleError(error),
    });
  }

  loadRoles(): void {
    this.roleService.getAll().subscribe({
      next: (roles) => {
        console.log('Полученные роли:', roles); // Лог для отладки
        this.roles = roles;
      },
      error: (error) => this.handleError(error)
    });
  }

  private getRolesByUser(userId: string | null): Observable<RoleDTO[]> {
    return this.roleService.getRolesByUserId(userId);
  }

  private getCompanyId(userId: string | null): Observable<string> {
    return this.companyService.getByUserId(userId).pipe(
      map(company => company.id)
    );
  }

  addUserToCompany(): void {
    const userId = this.tokenService.getUserId();
    this.getCompanyId(userId).subscribe(companyId => {
      this.newUser.companyId = companyId;
      this.getRolesByUser(userId).subscribe(roles => {
        if (roles.length > 0) {
          this.newUser.roleId = roles[0].id;
          this.userService.registerUserToCompany(this.newUser).subscribe(() => {
            console.log('Пользователь добавлен в компанию');
            this.loadUsers();
            this.showAddUserModal = false; // Закрываем модальное окно
          });
        } else {
          console.error('Нет доступных ролей для пользователя');
        }
      });
    });
  }

  goBack(): void {
    this.location.back();
  }

  updateUserRole(user: UserResponseDTO): void {
    // Реализуйте логику для обновления роли пользователя
  }
}
