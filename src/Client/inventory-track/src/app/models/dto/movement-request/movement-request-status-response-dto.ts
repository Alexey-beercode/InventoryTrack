import {MovementRequestStatus} from "./enums/movement-request-status.enum";

export class MovementRequestStatusResponseDto {
  value!:MovementRequestStatus; // Enum value as a string
  name!: string;  // Display name
}
