import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { SupplierResponseDto } from '../models/dto/supplier/supplier-response-dto';
import { CreateSupplierDto } from '../models/dto/supplier/create-supplier-dto';

@Injectable({
  providedIn: 'root',
})
export class SupplierService {
  private readonly baseUrl = environment.baseInventoryUrl;
  private readonly apiUrls = environment.apiUrls.supplier;

  constructor(private http: HttpClient) {}

  getAllSuppliers(): Observable<SupplierResponseDto[]> {
    return this.http.get<SupplierResponseDto[]>(`${this.baseUrl}${this.apiUrls.getAll}`);
  }

  getSupplierById(id: string): Observable<SupplierResponseDto> {
    return this.http.get<SupplierResponseDto>(
      `${this.baseUrl}${this.apiUrls.getById.replace('{id}', id)}`
    );
  }

  getSupplierByName(name: string): Observable<SupplierResponseDto> {
    return this.http.get<SupplierResponseDto>(
      `${this.baseUrl}${this.apiUrls.getByName.replace('{name}', name)}`
    );
  }

  getSupplierByAccountNumber(accountNumber: string): Observable<SupplierResponseDto> {
    return this.http.get<SupplierResponseDto>(
      `${this.baseUrl}${this.apiUrls.getByAccountNumber.replace('{accountNumber}', accountNumber)}`
    );
  }

  createSupplier(dto: CreateSupplierDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}${this.apiUrls.create}`, dto);
  }

  deleteSupplier(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}${this.apiUrls.delete.replace('{id}', id)}`
    );
  }

  getByCompanyId(companyId:string):Observable<SupplierResponseDto[]> {
    return this.http.get<SupplierResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByCompanyId.replace('{companyId}', companyId)}`
    );
  }
}
