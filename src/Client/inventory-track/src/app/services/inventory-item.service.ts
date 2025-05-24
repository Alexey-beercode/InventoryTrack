import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { InventoryItemResponseDto } from '../models/dto/inventory-item/inventory-item-response-dto';
import { CreateInventoryItemDto } from '../models/dto/inventory-item/create-inventory-item-dto';
import { UpdateInventoryItemDto } from '../models/dto/inventory-item/update-inventory-item-dto';
import { FilterInventoryItemDto } from '../models/dto/inventory-item/filter-inventory-item-dto';
import { ChangeInventoryItemStatusDto } from '../models/dto/inventory-item/change-inventory-item-status-dto';
import {BatchInfoDto} from "../models/dto/inventory-item/batch-info-dto";

@Injectable({
  providedIn: 'root',
})
export class InventoryItemService {
  private readonly baseUrl = environment.baseInventoryUrl;
  private readonly apiUrls = environment.apiUrls.inventoryItem;

  constructor(private http: HttpClient) {}

  createInventoryItem(dto: CreateInventoryItemDto): Observable<InventoryItemResponseDto> {
    return this.http.post<InventoryItemResponseDto>(
      `${this.baseUrl}${this.apiUrls.create}`,
      dto
    );
  }

  uploadDocument(file: File): Observable<string> {
    return new Observable((observer) => {
      const reader = new FileReader();

      reader.onload = () => {
        const base64String = (reader.result as string).split(',')[1]; // Получаем base64-строку
        const fileName = file.name; // Название файла
        const fileType = file.type; // MIME-тип файла

        // Отправляем данные на сервер
        this.http.post<string>(`${this.baseUrl}/api/documents/upload`, {
          fileBase64: base64String,
          fileName: fileName,
          fileType: fileType
        }).subscribe({
          next: (response) => {
            observer.next(response);
            observer.complete();
          },
          error: (err) => {
            observer.error(err);
          }
        });
      };

      reader.onerror = (error) => {
        observer.error(error);
      };

      reader.readAsDataURL(file); // Читаем файл как Data URL
    });
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

  // Добавить эти методы в InventoryItemService

  /**
   * 🆕 Get all batches for specific item name
   * @param itemName Item name
   * @returns Observable with batches info
   */
  getBatchesByItemName(itemName: string): Observable<BatchInfoDto[]> {
    return this.http.get<BatchInfoDto[]>(
      `${this.baseUrl}/api/inventory-items/batches/by-name/${encodeURIComponent(itemName)}`
    );
  }

  /**
   * 🆕 Get all items with all batches for specific item name
   * @param itemName Item name
   * @returns Observable with full items info
   */
  getAllBatchesByItemName(itemName: string): Observable<InventoryItemResponseDto[]> {
    return this.http.get<InventoryItemResponseDto[]>(
      `${this.baseUrl}/api/inventory-items/all-batches/by-name/${encodeURIComponent(itemName)}`
    );
  }
}
