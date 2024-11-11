import { RoleDTO } from '../role/role-dto';

export interface UserResponseDTO {
  id: string;
  login: string;
  firstName: string;
  lastName: string;
  roles: RoleDTO[];
}
