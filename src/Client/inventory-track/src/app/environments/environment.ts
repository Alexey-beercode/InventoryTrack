export const environment = {
  production: false,
  baseAuthUrl: 'http://localhost:5110',
  baseInventoryUrl: 'http://localhost:5111',
  baseMovementUrl: 'http://localhost:5112',
  baseWriteOffUrl: 'http://localhost:5113',
  baseReportUrl: 'http://localhost:5114',
  apiUrls: {
    auth: {
      login: '/api/auth/login',
      register: '/api/auth/register',
      addUserToWarehouse: '/api/auth/user-to-warehouse',
      token_status:'/api/auth/token-status'
    },
    company: {
      getById: '/api/companies/{id}',
      create: '/api/companies',
      update: '/api/companies',
      getAll: '/api/companies',
      delete: '/api/companies/{id}',
      getByUserId: '/api/companies/by-user-id/{userId}',
    },
    document: {
      uploadDocument: '/api/documents',
      getDocumentInfo: '/api/documents/{id}/info',
      downloadDocument: '/api/documents/{id}/download',
      deleteDocument: '/api/documents/{id}',
      generate: '/api/documents/generate'
    },
    user: {
      getAll: '/api/users',
      getById: '/api/users/{id}',
      getByLogin: '/api/users/by-login/{login}',
      getByName: '/api/users/by-name',
      getByCompanyId: '/api/users/by-company/{companyId}',
      registerUserToCompany: '/api/users',
      update: '/api/users',
      delete: '/api/users/{id}',
      addUserToCompany: '/api/users/user-to-company',
      addUserToWarehouse: '/api/users/user-to-warehouse',
    },
    inventoryItem: {
      create: '/api/inventory-items',
      getAll: '/api/inventory-items',
      getFiltered: '/api/inventory-items/filtered',
      getByWarehouse: '/api/inventory-items/warehouse/{warehouseId}',
      getById: '/api/inventory-items/{id}',
      getByName: '/api/inventory-items/by-name/{name}',
      getByStatus: '/api/inventory-items/by-status/{status}',
      update: '/api/inventory-items/{id}',
      updateStatus: '/api/inventory-items/status',
      delete: '/api/inventory-items/{id}',
      addDocument: '/api/inventory-items/add-document/{name}'
    },
    supplier: {
      getAll: '/api/suppliers',
      getById: '/api/suppliers/{id}',
      getByName: '/api/suppliers/by-name/{name}',
      getByAccountNumber: '/api/suppliers/by-account/{accountNumber}',
      create: '/api/suppliers',
      delete: '/api/suppliers/{id}',
      getByCompanyId: '/api/suppliers/by-company/{companyId}',
    },
    warehouse: {
      getAll: '/api/warehouses',
      getById: '/api/warehouses/{id}',
      getByType: '/api/warehouses/by-type/{warehouseType}',
      getByCompany: '/api/warehouses/by-company/{companyId}',
      getByResponsiblePerson: '/api/warehouses/by-responsible-person/{responsiblePersonId}',
      getByName: '/api/warehouses/by-name/{name}',
      getByLocation: '/api/warehouses/by-location/{location}',
      delete: '/api/warehouses/{id}',
      create: '/api/warehouses',
      getAllStates: '/api/warehouses/states',
      getStateById: '/api/warehouses/states/{id}',
      getStatesByCompany: '/api/warehouses/states/by-company/{companyId}',
      getStateByResponsiblePerson:'/api/warehouses/states/by-person/{responsiblePersonId}',
      update: '/api/warehouses/update',
    },
    movementRequest: {
      create: '/api/movement-requests',
      approve: '/api/movement-requests/{id}/approve',
      reject: '/api/movement-requests/{id}/reject',
      delete: '/api/movement-requests/{id}',
      getById: '/api/movement-requests/{id}',
      getByStatus: '/api/movement-requests/status/{status}',
      getByWarehouse: '/api/movement-requests/warehouse/{warehouseId}',
      finalApprove: '/api/movement-requests/{id}/final-approve',
      addDocumentToRequest: '/api/movement-requests/add-document/{documentId}/{requestId}',
      generateDocument: '/api/movement-requests/generate-document/{id}'
    },
    writeOffReason: {
      getAll: '/api/write-off-reasons',
      getById: '/api/write-off-reasons/{id}',
      getByName: '/api/write-off-reasons/by-name/{name}',
      create: '/api/write-off-reasons',
    },
    writeOffRequest: {
      getByWarehouseId: '/api/write-off-requests/warehouse/{warehouseId}',
      getFiltered: '/api/write-off-requests',
      getByStatus: '/api/write-off-requests/status/{status}',
      getById: '/api/write-off-requests/{id}',
      create: '/api/write-off-requests',
      update: '/api/write-off-requests',
      delete: '/api/write-off-requests/{id}',
      reject: '/api/write-off-requests/reject/{id}',  // 👈 добавлен `/`
      approve: "/api/write-off-requests/{id}/approve",
      uploadDocuments: "/api/write-off-requests/{id}/upload-documents",
      generateDocument: '/api/write-off-requests/generate-document/{id}',
      createBatch: '/api/write-off-requests/batch'
    },

    report: {
      getById: '/api/report/id/{id}',
      getByDateRange: '/api/report/by-date-range/{startDate}/{endDate}',
      getByDateSelect: '/api/report/by-date-select/{dateSelect}',
      getByType: '/api/report/by-type/{type}',
      getPaginated: '/api/report/paginated',
      getAll: '/api/report',
      delete: '/api/report/{id}',
      create: '/api/report/create',
      getByCompanyId: '/api/report/by-company-id/{companyId}'
    },
    role: {
      getAll: '/api/roles', // Получить все роли
      getById: '/api/roles/{id}', // Получить роль по ID
      getRolesByUserId: '/api/roles/by-user/{userId}', // Получить роли пользователя
      setRoleToUser: '/api/roles/assign', // Назначить роль пользователю
      removeRoleFromUser: '/api/roles/remove', // Удалить роль у пользователя
    },
  },
};
