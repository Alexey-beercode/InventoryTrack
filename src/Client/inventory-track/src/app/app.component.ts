// src/app/app.component.ts
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import {FooterComponent} from "./components/shared/footer/footer.component";
import {HeaderComponent} from "./components/shared/header/header.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, FooterComponent, HeaderComponent],
  templateUrl: 'app.component.html',
  styleUrls:['app.component.css']
})
export class AppComponent {}
