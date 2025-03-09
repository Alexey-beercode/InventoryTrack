import {Component, Input, OnInit} from '@angular/core';
import { UserService } from '../../../services/user.service';
import { WarehouseService } from '../../../services/warehouse.service';
import { RoleService } from '../../../services/role.service';
import { UserResponseModel } from '../../../models/user/register-user-to-company-model';
import { RoleDTO } from '../../../models/dto/role/role-dto';
import { WarehouseResponseDto } from '../../../models/dto/warehouse/warehouse-response-dto';
import { AddUserToWarehouseDto } from '../../../models/dto/auth/add-user-to-warehouse-dto';
import { UpdateWarehouseDto } from '../../../models/dto/warehouse/update-warehouse-dto';
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { LoadingSpinnerComponent } from "../../shared/loading-spinner/loading-spinner.component";
import { BackButtonComponent } from "../../shared/back-button/back-button.component";
import { ErrorMessageComponent } from "../../shared/error/error.component";
import { FooterComponent } from "../../shared/footer/footer.component";
import { HeaderComponent } from "../../shared/header/header.component";
import { UserResponseDTO } from "../../../models/dto/user/user-response-dto";
import { CompanyService } from "../../../services/company.service";
import { TokenService } from "../../../services/token.service";
import { AuthService } from "../../../services/auth.service";
import { map, Observable } from "rxjs";
import { RegisterUserToCompanyDTO } from "../../../models/dto/user/register-user-to-company-dto";

@Component({
  selector: 'app-user-management',
  templateUrl: 'user-managment.component.html',
  styleUrls: ['user-managment.component.css'],
  standalone: true,
  imports: [
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
    private warehouseService: WarehouseService
  ) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  onCompanyIdReceived(companyId: string): void {
    if (!companyId) return;

    this.companyId = companyId;
    this.isLoading = true;

    this.loadWarehouses(() => {
      this.loadUsers();
    });
  }

  loadWarehouses(callback?: () => void): void {
    if (!this.companyId) return;

    this.warehouseService.getWarehousesByCompany(this.companyId).subscribe({
      next: (warehouses) => {
        this.warehouses = warehouses;
        if (callback) callback(); // Вызываем loadUsers() только после загрузки складов
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Ошибка загрузки складов';
        this.isLoading = false;
      },
    });
  }


  loadUsers(): void {
    this.isLoading = true;
    this.userService.getByCompanyId(this.companyId!).subscribe({
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
        type: warehouse.type.valueOf(),
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

  deleteUser(userId: string | null) {
    this.userService.delete(userId!).subscribe(() => {
      this.loadUsers();
    });
  }

  openAddUserModal(): void {
    this.loadWarehouses();
    this.showAddUserModal = true;
  }

  addUserToCompany(): void {
    console.log("зашли в метод");
    if (!this.selectedWarehouseId || !this.companyId) return;
    console.log("все хорошо");
    this.newUser.companyId = this.companyId;

    this.userService.registerUserToCompany(this.newUser).subscribe({
      next: (userId: string) => {
        if (this.newUser.roleId) {
          const dto: AddUserToWarehouseDto = {
            userId: userId,
            warehouseId: this.selectedWarehouseId!,
          };
          this.userService.addUserToWarehouse(dto).subscribe({
            next: () => {
              console.log("Пользователь успешно добавлен в склад");
              this.loadUsers();
              this.resetModal();
            },
            error: (err) => console.error("Ошибка добавления пользователя в склад:", err)
          });
        } else {
          this.loadUsers();
          this.resetModal();
        }
      },
      error: (error) => {
        this.errorMessage = 'Ошибка создания пользователя.';
        console.error(error);
      },
    });
  }
}
