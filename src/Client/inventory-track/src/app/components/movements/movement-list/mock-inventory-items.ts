import { InventoryItemResponseDto } from '../../../models/dto/inventory-item/inventory-item-response-dto';

export const mockInventoryItems: InventoryItemResponseDto[] = [
  {
    id: 'item-1',
    name: 'Ноутбук Dell Inspiron',
    uniqueCode: 'ITM-0001',
    quantity: 15,
    estimatedValue: 3000,
    expirationDate: new Date('2025-12-31'),
    supplier: {
      id: 'supplier-1',
      name: 'Dell Corporation',
      phoneNumber: '+1-800-555-1234',
      postalAddress: '1 Dell Way, Round Rock, TX, USA',
      accountNumber: 'ACC-12345',
      companyId: 'company-1',
    },
    deliveryDate: new Date('2024-01-15'),
    documentId: 'doc-1',
    status: {
      value: 'Available',
      name: 'Доступен',
    },
    warehouseDetails: [
      { warehouseId: 'wh-5', warehouseName: 'Основной склад', quantity: 10 },
      { warehouseId: 'wh-2', warehouseName: 'Склад филиала', quantity: 5 },
    ],
    documentInfo: {
      id: 'doc-1',
      fileName: 'dell-invoice.pdf',
      fileType: 'application/pdf',
    },
  },
  {
    id: 'item-2',
    name: 'Смартфон Samsung Galaxy S21',
    uniqueCode: 'ITM-0002',
    quantity: 25,
    estimatedValue: 1200,
    expirationDate: new Date('2026-06-30'),
    supplier: {
      id: 'supplier-2',
      name: 'Samsung Electronics',
      phoneNumber: '+82-2-555-6789',
      postalAddress: 'Samsung Town, Seoul, South Korea',
      accountNumber: 'ACC-54321',
      companyId: 'company-1',
    },
    deliveryDate: new Date('2023-12-01'),
    documentId: 'doc-2',
    status: {
      value: 'Available',
      name: 'Доступен',
    },
    warehouseDetails: [
      { warehouseId: 'wh-5', warehouseName: 'Основной склад', quantity: 15 },
      { warehouseId: 'wh-3', warehouseName: 'Склад региональный', quantity: 10 },
    ],
    documentInfo: {
      id: 'doc-2',
      fileName: 'samsung-invoice.pdf',
      fileType: 'application/pdf',
    },
  },
  {
    id: 'item-3',
    name: 'Кофемашина Philips LatteGo',
    uniqueCode: 'ITM-0003',
    quantity: 8,
    estimatedValue: 650,
    expirationDate: new Date('2027-01-01'),
    supplier: {
      id: 'supplier-3',
      name: 'Philips Electronics',
      phoneNumber: '+31-20-123-4567',
      postalAddress: 'High Tech Campus, Eindhoven, Netherlands',
      accountNumber: 'ACC-78901',
      companyId: 'company-2',
    },
    deliveryDate: new Date('2024-02-10'),
    documentId: 'doc-3',
    status: {
      value: 'Reserved',
      name: 'Зарезервирован',
    },
    warehouseDetails: [
      { warehouseId: 'wh-5', warehouseName: 'Склад Philips', quantity: 8 },
    ],
    documentInfo: {
      id: 'doc-3',
      fileName: 'philips-invoice.pdf',
      fileType: 'application/pdf',
    },
  },
];
