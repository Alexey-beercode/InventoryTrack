// src/app/services/role.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { RoleDTO } from '../models/dto/role/role-dto';
import { UserRoleDTO } from '../models/dto/user/user-role-dto';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private apiUrl = `${environment.apiUrl}/api/roles`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<RoleDTO[]> {
    return this.http.get<RoleDTO[]>(this.apiUrl);
  }

  getById(id: string): Observable<RoleDTO> {
    return this.http.get<RoleDTO>(`${this.apiUrl}/${id}`);
  }

  getRolesByUserId(userId: string | null): Observable<RoleDTO[]> {
    return this.http.get<RoleDTO[]>(`${this.apiUrl}/by-user/${userId}`);
  }

  setRoleToUser(userRoleDto: UserRoleDTO): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/assign`, userRoleDto, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    });
  }

  removeRoleFromUser(userRoleDto: UserRoleDTO): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/remove`, userRoleDto, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    });
  }
}
