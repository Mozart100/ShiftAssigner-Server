// Domain types
export enum RoleState {
  Worker = "Worker",
  Boss = "Boss",
  Admin = "Admin"
}

export interface TenantShiftScheduling {
  morning: boolean;
  day: boolean;
  evening: boolean;
}

export interface BossTenant {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  role: RoleState;
  passwordHash: string;
  isActive: boolean;
  tenant: string;
  shiftConfig?: TenantShiftScheduling | null;
}

export interface TenantRegistrationState {
  tenant: BossTenant;
  isSubmitting: boolean;
  isSuccess: boolean;
  error?: string;
}