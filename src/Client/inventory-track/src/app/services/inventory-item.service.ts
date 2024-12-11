import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { InventoryItemResponseDto } from '../models/dto/inventory-item/inventory-item-response-dto';
import { CreateInventoryItemDto } from '../models/dto/inventory-item/create-inventory-item-dto';
import { UpdateInventoryItemDto } from '../models/dto/inventory-item/update-inventory-item-dto';
import { FilterInventoryItemDto } from '../models/dto/inventory-item/filter-inventory-item-dto';
import { ChangeInventoryItemStatusDto } from '../models/dto/inventory-item/change-inventory-item-status-dto';

@Injectable({
  providedIn: 'root',
})
export class InventoryItemService {
  private readonly baseUrl = environment.baseInventoryUrl;
  private readonly apiUrls = environment.apiUrls.inventoryItem;

  constructor(private http: HttpClient) {}

  createInventoryItem(dto: CreateInventoryItemDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}${this.apiUrls.create}`, dto);
  }

  getAllInventoryItems(): Observable<InventoryItemResponseDto[]> {
    return this.http.get<InventoryItemResponseDto[]>(`${this.baseUrl}${this.apiUrls.getAll}`);
  }

  getFilteredItems(filter: FilterInventoryItemDto): Observable<InventoryItemResponseDto[]> {
    // Преобразуем объект фильтра в параметры строки
    let params = new HttpParams();
    Object.entries(filter).forEach(([key, value]) => {
      if (value instanceof Date) {
        params = params.append(key, value.toISOString()); // Преобразуем Date в ISO строку
      } else if (value !== undefined && value !== null) {
        params = params.append(key, value.toString());
      }
    });

    return this.http.get<InventoryItemResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getFiltered}`,
      { params }
    );
  }

  getInventoryItemsByWarehouse(warehouseId: string): Observable<InventoryItemResponseDto[]> {
    return this.http.get<InventoryItemResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByWarehouse.replace('{warehouseId}', warehouseId)}`
    );
  }

  getInventoryItemById(id: string): Observable<InventoryItemResponseDto> {
    return this.http.get<InventoryItemResponseDto>(
      `${this.baseUrl}${this.apiUrls.getById.replace('{id}', id)}`
    );
  }

  getByName(name: string): Observable<InventoryItemResponseDto> {
    return this.http.get<InventoryItemResponseDto>(
      `${this.baseUrl}${this.apiUrls.getByName.replace('{name}', name)}`
    );
  }

  getByStatus(status: string): Observable<InventoryItemResponseDto[]> {
    return this.http.get<InventoryItemResponseDto[]>(
      `${this.baseUrl}${this.apiUrls.getByStatus.replace('{status}', status)}`
    );
  }

  updateInventoryItem(id: string, dto: UpdateInventoryItemDto): Observable<void> {
    return this.http.put<void>(
      `${this.baseUrl}${this.apiUrls.update.replace('{id}', id)}`,
      dto
    );
  }

  updateStatus(dto: ChangeInventoryItemStatusDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}${this.apiUrls.updateStatus}`, dto);
  }

  deleteInventoryItem(id: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}${this.apiUrls.delete.replace('{id}', id)}`
    );
  }
}
