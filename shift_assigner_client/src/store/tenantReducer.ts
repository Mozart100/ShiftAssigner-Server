import { ImmerReducer, createReducerFunction, createActionCreators } from "immer-reducer";
import type { SupportedLanguage } from '../localization/i18n';

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
  currentLanguage: SupportedLanguage;
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
  isSuccess: false,
  currentLanguage: 'en'
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

  setLanguage(language: SupportedLanguage) {
    this.draftState.currentLanguage = language;
  }
}

export const TenantRegistrationActions = createActionCreators(TenantRegistrationReducer);
export const tenantRegistrationReducer = createReducerFunction(
  TenantRegistrationReducer,
  initialTenantRegistrationState
);

// Async Actions
export const submitTenantRegistration = () => async (dispatch: any, getState: any) => {
  const state = getState();
  const tenant = state.tenantRegistration.tenant;

  dispatch(TenantRegistrationActions.submitStart());

  try {
    // Simple validation
    if (!tenant.firstName.trim() || !tenant.lastName.trim() || !tenant.phoneNumber.trim() || !tenant.tenant.trim()) {
      dispatch(TenantRegistrationActions.submitFailure("Please fill all required fields"));
      return;
    }

    // Simulate API call
    console.log('Submitting tenant registration:', tenant);
    
    // Replace with actual API endpoint
    const response = await new Promise<{ ok: boolean; json: () => Promise<{ id: string }> }>((resolve) => {
      setTimeout(() => {
        resolve({
          ok: true,
          json: async () => ({ id: `tenant_${Date.now()}` })
        });
      }, 2000);
    });

    if (!response.ok) {
      throw new Error('Registration failed');
    }

    const result = await response.json();
    dispatch(TenantRegistrationActions.submitSuccess());
    dispatch(TenantRegistrationActions.setField({ key: "id", value: result.id }));

    console.log('Registration successful:', result);

  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : "Registration failed";
    dispatch(TenantRegistrationActions.submitFailure(errorMessage));
    console.error('Registration error:', error);
  }
};