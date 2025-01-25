import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { WarehouseResponseDto } from '../models/dto/warehouse/warehouse-response-dto';
import { CreateWarehouseDto } from '../models/dto/warehouse/create-warehouse-dto';
import { WarehouseStateResponseDto } from '../models/dto/warehouse/warehouse-state-response-dto';
import {UpdateWarehouseDto} from "../models/dto/warehouse/update-warehouse-dto";

@Injectable({
  providedIn: 'root',
})
export class WarehouseService {
  private readonly baseUrl = environment.baseInventoryUrl;
  private readonly apiUrls = environment.apiUrls.warehouse;

  constructor(private http: HttpClient) {}

  getAllWarehouses(): Observable<WarehouseResponseDto[]> {
    return this.http.get<WarehouseResponseDto[]>(`${this.baseUrl}${this.apiUrls.getAll}`);
  }

  getWarehouseById(id: string): Observable<WarehouseResponseDto> {
    return this.http.get<WarehouseResponseDto>(
      `${this.baseUrl}${this.apiUrls.getById.replace('{id}', id)}`
    );
  }

  getWarehousesByType(type: string): Observable<WarehouseResponseDto[]> {
    return this.http.get<WarehouseResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByType.replace('{warehouseType}', type)}`
    );
  }

  getWarehousesByCompany(companyId: string): Observable<WarehouseResponseDto[]> {
    return this.http.get<WarehouseResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByCompany.replace('{companyId}', companyId)}`
    );
  }

  getWarehousesByResponsiblePerson(responsiblePersonId: string): Observable<WarehouseResponseDto[]> {
    return this.http.get<WarehouseResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByResponsiblePerson.replace('{responsiblePersonId}', responsiblePersonId)}`
    );
  }

  getWarehousesByName(name: string): Observable<WarehouseResponseDto[]> {
    return this.http.get<WarehouseResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByName.replace('{name}', name)}`
    );
  }

  getWarehousesByLocation(location: string): Observable<WarehouseResponseDto[]> {
    return this.http.get<WarehouseResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByLocation.replace('{location}', location)}`
    );
  }

  deleteWarehouse(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}${this.apiUrls.delete.replace('{id}', id)}`
    );
  }

  createWarehouse(dto: CreateWarehouseDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}${this.apiUrls.create}`, dto);
  }

  getAllWarehousesStates(): Observable<WarehouseStateResponseDto[]> {
    return this.http.get<WarehouseStateResponseDto[]>(`${this.baseUrl}${this.apiUrls.getAllStates}`);
  }

  getWarehouseStateById(id: string): Observable<WarehouseStateResponseDto> {
    return this.http.get<WarehouseStateResponseDto>(
      `${this.baseUrl}${this.apiUrls.getStateById.replace('{id}', id)}`
    );
  }

  getWarehousesStatesByCompany(companyId: string): Observable<WarehouseStateResponseDto[]> {
    return this.http.get<WarehouseStateResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getStatesByCompany.replace('{companyId}', companyId)}`
    );
  }
  updateWarehouse(dto: UpdateWarehouseDto): Observable<void> {
    const url = `${environment.baseInventoryUrl}${environment.apiUrls.warehouse.update}`;
    return this.http.put<void>(url, dto);
  }

}
