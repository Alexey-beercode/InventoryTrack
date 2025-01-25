import { Component, Input } from '@angular/core';
import {CommonModule} from "@angular/common";

@Component({
  selector: 'app-error-message',
  templateUrl: 'error.component.html',
  styleUrls: ['./error.component.css'],
  imports:[CommonModule],
  standalone: true,
})
export class ErrorMessageComponent {
  @Input() message: string | null = null;
}
