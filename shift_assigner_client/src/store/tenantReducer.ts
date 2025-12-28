import { ImmerReducer, createReducerFunction, createActionCreators } from "immer-reducer";

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

export const initialTenantRegistrationState: TenantRegistrationState = {
  tenant: {
    id: "",
    firstName: "",
    lastName: "",
    phoneNumber: "",
    dateOfBirth: "",
    role: RoleState.Boss,
    passwordHash: "",
    isActive: true,
    tenant: "",
    shiftConfig: null
  },
  isSubmitting: false,
  isSuccess: false
};

export class TenantRegistrationReducer extends ImmerReducer<TenantRegistrationState> {
  setField<Key extends keyof BossTenant>(payload: { key: Key; value: BossTenant[Key] }) {
    this.draftState.tenant[payload.key] = payload.value;
  }

  setShiftConfig(config: TenantShiftScheduling) {
    this.draftState.tenant.shiftConfig = config;
  }

  setTenant(tenant: Partial<BossTenant>) {
    Object.assign(this.draftState.tenant, tenant);
  }

  submitStart() {
    this.draftState.isSubmitting = true;
    this.draftState.error = undefined;
    this.draftState.isSuccess = false;
  }

  submitSuccess() {
    this.draftState.isSubmitting = false;
    this.draftState.isSuccess = true;
    this.draftState.error = undefined;
  }

  submitFailure(error: string) {
    this.draftState.isSubmitting = false;
    this.draftState.isSuccess = false;
    this.draftState.error = error;
  }

  resetForm() {
    return initialTenantRegistrationState;
  }

  clearMessages() {
    this.draftState.error = undefined;
    this.draftState.isSuccess = false;
  }
}

export const TenantRegistrationActions = createActionCreators(TenantRegistrationReducer);

export const tenantRegistrationReducer = createReducerFunction(
  TenantRegistrationReducer,
  initialTenantRegistrationState
);