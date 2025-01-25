import { Component, OnInit } from "@angular/core";
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { UserResponseDTO } from '../../models/dto/user/user-response-dto';
import { TokenService } from "../../services/token.service";
import { Router } from "@angular/router";
import { AuthBaseComponent } from "../base/auth-base.component";
import { CompanyResponseDTO } from "../../models/dto/company/company-response-dto";
import { CompanyService } from "../../services/company.service";
import {Location, NgIf} from "@angular/common";
import {FormsModule, NgForm} from "@angular/forms";
import { CreateCompanyDTO } from "../../models/dto/company/create-company-dto";
import {HeaderComponent} from "../shared/header/header.component";
import {FooterComponent} from "../shared/footer/footer.component";
import {WarehouseStatesListComponent} from "../warehouses/warehouses-list/warehouses-list.component";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  imports: [
    NgIf,
    FormsModule,
    HeaderComponent,
    FooterComponent,
    WarehouseStatesListComponent
  ],
  standalone: true
})
export class HomeComponent  {

}
