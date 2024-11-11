import { UserResponseDTO } from '../user/user-response-dto';

export interface CompanyResponseDTO {
  id: string;
  name: string;
  unp: string;
  legalAddress: string;
  postalAddress: string;
  responsiblePerson: UserResponseDTO;
}
