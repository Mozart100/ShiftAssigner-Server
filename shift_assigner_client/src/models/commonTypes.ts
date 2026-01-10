/**
 * Common interfaces shared between UI state and API contracts
 */

export enum RoleState {
  Worker = "Worker",
  TenantBoss = "TenantBoss",
  TeamLeader = "TeamLeader"
}

export interface TenantShiftScheduling {
  shiftName: string; // Morning, Night
  minimumAmountOfWorkers: number;
  maximumAmountOfWorkers: number;
}

// Common interface for tenant data
export interface BaseTenantData {
  id:string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  tenant: string;
  password: string;
  role: RoleState;
  shiftConfig: TenantShiftScheduling[];
}