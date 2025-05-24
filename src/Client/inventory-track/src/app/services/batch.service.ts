// src/app/services/batch.service.ts (НОВЫЙ СЕРВИС)
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { BatchInfoDto } from '../models/dto/inventory-item/batch-info-dto';
import { InventoryItemResponseDto } from '../models/dto/inventory-item/inventory-item-response-dto';

@Injectable({
  providedIn: 'root',
})
export class BatchService {
  private readonly baseInventoryUrl = environment.baseInventoryUrl;
  constructor(private http: HttpClient) {}

  /**
   * Generate batch number for new items (Belarus standards)
   * @param manufactureDate Manufacture date
   * @param sequenceNumber Sequence number for the day
   * @param organizationPrefix Optional organization prefix
   * @returns Generated batch number
   */
  generateBatchNumber(
    manufactureDate: Date,
    sequenceNumber: number = 1,
    organizationPrefix?: string
  ): string {
    const dateStr = manufactureDate.toISOString().split('T')[0]; // YYYY-MM-DD
    const sequence = sequenceNumber.toString().padStart(4, '0'); // 0001, 0002, etc.

    return organizationPrefix
      ? `${organizationPrefix}-${dateStr}-${sequence}`
      : `${dateStr}-${sequence}`;
  }

  /**
   * Validate batch number format
   * @param batchNumber Batch number to validate
   * @returns true if valid
   */
  isValidBatchNumber(batchNumber: string): boolean {
    // Basic format: YYYY-MM-DD-XXXX
    const basicPattern = /^\d{4}-\d{2}-\d{2}-\d{4}$/;

    // With prefix: PREFIX-YYYY-MM-DD-XXXX
    const prefixPattern = /^[A-Z]{2,10}-\d{4}-\d{2}-\d{2}-\d{4}$/;

    return basicPattern.test(batchNumber) || prefixPattern.test(batchNumber);
  }

  /**
   * Extract manufacture date from batch number
   * @param batchNumber Batch number
   * @returns Date or null if invalid
   */
  extractDateFromBatch(batchNumber: string): Date | null {
    if (!this.isValidBatchNumber(batchNumber)) {
      return null;
    }

    try {
      const parts = batchNumber.split('-');
      const yearIndex = parts.length === 4 ? 0 : 1; // Account for optional prefix

      const year = parseInt(parts[yearIndex]);
      const month = parseInt(parts[yearIndex + 1]);
      const day = parseInt(parts[yearIndex + 2]);

      return new Date(year, month - 1, day); // month is 0-indexed
    } catch {
      return null;
    }
  }
}
