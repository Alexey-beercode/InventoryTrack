import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { WarehouseService } from '../../../services/warehouse.service';
import { RoleService } from '../../../services/role.service';
import { UserResponseModel } from '../../../models/user/register-user-to-company-model';
import { RoleDTO } from '../../../models/dto/role/role-dto';
import { WarehouseResponseDto } from '../../../models/dto/warehouse/warehouse-response-dto';
import { AddUserToWarehouseDto } from '../../../models/dto/auth/add-user-to-warehouse-dto';
import { UpdateWarehouseDto } from '../../../models/dto/warehouse/update-warehouse-dto';
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {LoadingSpinnerComponent} from "../../shared/loading-spinner/loading-spinner.component";
import {BackButtonComponent} from "../../shared/back-button/back-button.component";
import {ErrorMessageComponent} from "../../shared/error/error.component";
import {FooterComponent} from "../../shared/footer/footer.component";
import {HeaderComponent} from "../../shared/header/header.component";
import {UserResponseDTO} from "../../../models/dto/user/user-response-dto";
import {CompanyService} from "../../../services/company.service";
import {Router, RouterModule} from "@angular/router";
import {TokenService} from "../../../services/token.service";
import {AuthService} from "../../../services/auth.service";
import {map, Observable} from "rxjs";
import {RegisterUserToCompanyDTO} from "../../../models/dto/user/register-user-to-company-dto";

@Component({
  selector: 'app-user-management',
  templateUrl: 'user-managment.component.html',
  styleUrls: ['user-managment.component.css'],
  standalone: true,
  imports: [
    // Подключение существующих компонентов
    CommonModule,
    FormsModule,
    LoadingSpinnerComponent,
    BackButtonComponent,
    ErrorMessageComponent,
    FooterComponent,
    HeaderComponent,
  ],
})
export class UserManagementComponent implements OnInit {
  users: UserResponseModel[] = [];
  roles: RoleDTO[] = [];
  warehouses: WarehouseResponseDto[] = [];
  selectedWarehouseId: string | null = null;
  currentUserForRoleChange: UserResponseModel | null = null;
  showWarehouseModal = false;
  showAddUserModal = false;
  isLoading = false;
  errorMessage: string | null = null;
  companyId: string | null = null;
  newUser: RegisterUserToCompanyDTO = {
    roleId: '',
    firstName: '',
    lastName: '',
    login: '',
    password: '',
    companyId: ''
  };
  constructor(
    private tokenService: TokenService,
    private userService: UserService,
    private roleService: RoleService,
    private companyService: CompanyService,
    private warehouseService:WarehouseService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
  }

  onCompanyIdReceived(companyId: string): void {
    this.companyId = companyId;
    this.loadWarehouses();
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
    this.isLoading = true;
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users = users.map((user) => ({
          id: user.id,
          login: user.login,
          firstName: user.firstName,
          lastName: user.lastName,
          role: user.roles.length > 0 ? user.roles[0] : null,
          selectedRoleId: user.roles.length > 0 ? user.roles[0].id : null,
        }));
        this.isLoading = false;
      },
      error: () => (this.errorMessage = 'Ошибка загрузки пользователей'),
    });
  }

  loadRoles(): void {
    this.roleService.getAll().subscribe({
      next: (roles) => {
        this.roles = roles.filter(
          (role) => role.name === 'Начальник подразделения' || role.name === 'Начальник склада'
        );
      },
      error: (error) => {
        this.errorMessage = 'Ошибка загрузки ролей.';
        console.error(error);
      },
    });
  }

  onRoleChange(user: UserResponseModel): void {
    const selectedRole = this.roles.find((role) => role.id === user.selectedRoleId);
    if (!selectedRole) return;

    if (selectedRole.name === 'Начальник подразделения' || selectedRole.name === 'Начальник склада') {
      this.currentUserForRoleChange = user;
      this.loadWarehouses();
      this.showWarehouseModal = true;
    } else {
      this.updateUserRole(user);
    }
  }

  loadWarehouses(): void {
    if (!this.companyId) return; // Проверка на наличие companyId
    this.isLoading = true;
    this.warehouseService.getWarehousesByCompany(this.companyId).subscribe({
      next: (warehouses) => {
        this.warehouses = warehouses;
        this.isLoading = false;
      },
      error: () => (this.errorMessage = 'Ошибка загрузки складов'),
    });
  }

  assignWarehouse(): void {
    if (!this.selectedWarehouseId || !this.currentUserForRoleChange) return;

    const selectedRole = this.roles.find(
      (role) => role.id === this.currentUserForRoleChange?.selectedRoleId
    );

    if (selectedRole?.name === 'Начальник подразделения') {
      const dto: AddUserToWarehouseDto = {
        userId: this.currentUserForRoleChange.id,
        warehouseId: this.selectedWarehouseId,
      };

      this.userService.addUserToWarehouse(dto).subscribe({
        next: () => {
          this.loadUsers();
          this.resetModal();
        },
        error: (error) => {
          this.errorMessage = 'Ошибка назначения склада пользователю.';
          console.error(error);
        },
      });
    } else if (selectedRole?.name === 'Начальник склада') {
      const warehouse = this.warehouses.find((w) => w.id === this.selectedWarehouseId);
      if (!warehouse) return;

      const dto: UpdateWarehouseDto = {
        ...warehouse,
        responsiblePersonId: this.currentUserForRoleChange?.id || '',
      };

      this.warehouseService.updateWarehouse(dto).subscribe({
        next: () => {
          this.loadUsers();
          this.resetModal();
        },
        error: (error) => {
          this.errorMessage = 'Ошибка обновления склада.';
          console.error(error);
        },
      });
    }
  }

  resetModal(): void {
    this.selectedWarehouseId = null;
    this.currentUserForRoleChange = null;
    this.showWarehouseModal = false;
  }

  updateUserRole(user: UserResponseModel): void {
    if (!user.selectedRoleId) {
      this.errorMessage = 'Роль не выбрана.';
      return;
    }

    this.roleService.setRoleToUser({ userId: user.id, roleId: user.selectedRoleId }).subscribe({
      next: () => this.loadUsers(),
      error: (error) => {
        this.errorMessage = 'Ошибка обновления роли пользователя.';
        console.error(error);
      },
    });
  }
  deleteUser(userId:string | null){
    this.userService.delete(userId!).subscribe(()=>{
      this.loadUsers();
    })
  }
  private getCompanyId(userId: string | null): Observable<string> {
    return this.companyService.getByUserId(userId).pipe(
      map(company => company.id)
    );
  }

  private getRolesByUser(userId: string | null): Observable<RoleDTO[]> {
    return this.roleService.getRolesByUserId(userId!);
  }
  openAddUserModal(): void {
    this.loadWarehouses();
    this.showAddUserModal = true;
  }

  addUserToCompany(): void {
    if (!this.selectedWarehouseId || !this.companyId) return;

    this.newUser.companyId = this.companyId;

    this.userService.registerUserToCompany(this.newUser).subscribe({
      next: () => {
        if (this.newUser.roleId) {
          const dto: AddUserToWarehouseDto = {
            userId: '', // ID пользователя будет возвращен от сервера при создании
            warehouseId: this.selectedWarehouseId!,
          };
          this.userService.addUserToWarehouse(dto).subscribe();
        }
        this.loadUsers();
        this.resetModal();
      },
      error: (error) => {
        this.errorMessage = 'Ошибка создания пользователя.';
        console.error(error);
      },
    });
  }

  protected readonly close = close;
}
