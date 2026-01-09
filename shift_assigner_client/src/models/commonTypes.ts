/**
 * Common interfaces shared between UI state and API contracts
 */

export interface TenantShiftScheduling {
  shiftName: string; // Morning, Night
  minimumAmountOfWorkers: number;
  maximumAmountOfWorkers: number;
}

// Common interface for tenant data
export interface BaseTenantData {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  tenant: string;
  password: string;
  shiftConfig: TenantShiftScheduling[];
}