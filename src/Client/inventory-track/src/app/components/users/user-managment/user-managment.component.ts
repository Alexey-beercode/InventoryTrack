import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { RoleService } from '../../../services/role.service';
import { WarehouseService } from '../../../services/warehouse.service';
import { UserResponseModel } from '../../../models/user/register-user-to-company-model';
import { RoleDTO } from '../../../models/dto/role/role-dto';
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { LoadingSpinnerComponent } from "../../shared/loading-spinner/loading-spinner.component";
import { BackButtonComponent } from "../../shared/back-button/back-button.component";
import { ErrorMessageComponent } from "../../shared/error/error.component";
import { FooterComponent } from "../../shared/footer/footer.component";
import { HeaderComponent } from "../../shared/header/header.component";
import { CompanyService } from "../../../services/company.service";
import { TokenService } from "../../../services/token.service";
import { RegisterUserToCompanyDTO } from "../../../models/dto/user/register-user-to-company-dto";
import { UpdateWarehouseDto } from '../../../models/dto/warehouse/update-warehouse-dto';
import {WarehouseType} from "../../../models/dto/warehouse/enums/warehouse-type.enum";

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
  isLoading = false;
  errorMessage: string | null = null;
  companyId: string | null = null;
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
    private tokenService: TokenService,
    private userService: UserService,
    private roleService: RoleService,
    private warehouseService: WarehouseService,
    private companyService: CompanyService
  ) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  onCompanyIdReceived(companyId: string): void {
    if (!companyId) return;

    this.companyId = companyId;
    this.isLoading = true;

    this.loadUsers();
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
    this.updateUserRole(user);
  }

  async updateUserRole(user: UserResponseModel) {
    if (!user.selectedRoleId) {
      this.errorMessage = 'Роль не выбрана.';
      return;
    }

    try {
      await this.updateWarehousesForUserChange(user.id); // Обновляем склады перед сменой роли

      // Удаляем старую роль, если есть
      if (user.role?.id) {
        await this.roleService.removeRoleFromUser({ userId: user.id, roleId: user.role.id }).toPromise();
      }

      // Назначаем новую роль
      await this.roleService.setRoleToUser({ userId: user.id, roleId: user.selectedRoleId! }).toPromise();

      console.log(`Роль обновлена у пользователя ${user.firstName} ${user.lastName}`);
      this.loadUsers();
    } catch (error) {
      console.error('Ошибка при обновлении роли пользователя:', error);
      this.errorMessage = 'Ошибка при обновлении роли.';
    }
  }

  async deleteUser(userId: string | null) {
    if (!userId) return;
    try {
      await this.updateWarehousesForUserChange(userId); // Обновляем склады перед удалением

      // Удаляем пользователя после обновления складов
      await this.userService.delete(userId).toPromise();
      console.log(`Пользователь ${userId} удален.`);
      this.loadUsers();
    } catch (error) {
      console.error('Ошибка при удалении пользователя:', error);
      this.errorMessage = 'Ошибка при удалении пользователя.';
    }
  }

  /**
   * Метод обновления складов при изменении пользователя (удаление/смена роли)
   */
  private async updateWarehousesForUserChange(userId: string) {
    try {
      const warehouses = await this.warehouseService.getWarehousesByResponsiblePerson(userId).toPromise();

      if (warehouses && warehouses.length > 0) {
        for (const warehouse of warehouses) {
          let warehouseType: number;

          try {
            // Преобразуем объект в JSON, а потом обратно, чтобы получить "чистый" объект
            const warehouseTypeObj = JSON.parse(JSON.stringify(warehouse.type));

            if ('value' in warehouseTypeObj) {
              warehouseType = warehouseTypeObj.value;
            } else if ('name' in warehouseTypeObj) {
              // Если value нет, но есть name, маппим вручную
              warehouseType = warehouseTypeObj.name === 'Производственный' ? 0 : 1;
            } else {
              warehouseType = 0; // Значение по умолчанию
            }
          } catch {
            warehouseType = 0; // Фолбэк в случае ошибки
          }

          const updatedWarehouse: UpdateWarehouseDto = {
            id: warehouse.id,
            name: warehouse.name,
            type: warehouseType, // Всегда число (0 или 1)
            location: warehouse.location,
            responsiblePersonId: this.tokenService.getUserId()!,
            companyId: warehouse.companyId,
            accountantId: this.tokenService.getUserId()!,
          };

          await this.warehouseService.updateWarehouse(updatedWarehouse).toPromise();
          console.log(`Обновлен склад: ${warehouse.name}, новый ответственный: ${updatedWarehouse.responsiblePersonId}`);
        }
      }
    } catch (error) {
      console.error('Ошибка при обновлении складов пользователя:', error);
    }
  }


  openAddUserModal(): void {
    this.showAddUserModal = true;
  }

  addUserToCompany(): void {
    if (!this.companyId) return;

    this.newUser.companyId = this.companyId;

    this.userService.registerUserToCompany(this.newUser).subscribe({
      next: () => {
        this.loadUsers();
        this.resetNewUser(); // Обнуляем форму
        this.showAddUserModal = false;
      },
      error: (error) => {
        this.errorMessage = 'Ошибка создания пользователя.';
        console.error(error);
      },
    });
  }

  /**
   * Метод для сброса формы создания пользователя
   */
  private resetNewUser(): void {
    this.newUser = {
      roleId: '',
      firstName: '',
      lastName: '',
      login: '',
      password: '',
      companyId: this.companyId ?? ''
    };
  }

}
