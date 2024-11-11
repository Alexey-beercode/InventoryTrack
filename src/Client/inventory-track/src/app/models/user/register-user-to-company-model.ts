import {RoleDTO} from "../dto/role/role-dto";

export interface UserResponseModel {
  id: string;
  login: string;
  firstName: string;
  lastName: string;
  role: RoleDTO | null;
  selectedRoleId?: string|null; // Добавьте это свойство
}
