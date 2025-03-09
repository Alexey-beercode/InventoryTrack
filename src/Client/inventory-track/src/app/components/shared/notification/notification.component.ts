import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../../services/notification.service';
import {NgIf} from "@angular/common";

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css'],
  imports: [
    NgIf
  ],
  standalone: true
})
export class NotificationComponent implements OnInit {
  message: string | null = null;
  timeoutId: any;

  constructor(private notificationService: NotificationService) {}

  ngOnInit() {
    this.notificationService.error$.subscribe((msg) => {
      this.message = msg;
      clearTimeout(this.timeoutId);
      this.timeoutId = setTimeout(() => {
        this.message = null;
      }, 5000);
    });
  }
}
