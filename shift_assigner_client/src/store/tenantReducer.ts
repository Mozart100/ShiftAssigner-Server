import { ImmerReducer, createReducerFunction, createActionCreators } from "immer-reducer";
import { TenantRegistrationState, BossTenant, RoleState, TenantShiftScheduling } from "./types";

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