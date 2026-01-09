import { ImmerReducer, createReducerFunction, createActionCreators } from "immer-reducer";
import type { SupportedLanguage } from '../localization/i18n';
import { startLoading, stopLoading, spinnerOperations } from './loadingReducer';
import { showSuccess, showError } from './toastReducer';
import type { RootState } from './index';
import type { ThunkAction } from 'redux-thunk';
import type { AnyAction } from 'redux';
import { apiClient } from '../services';
import { BossTenantRegistrationRequest } from "../services/httpClientBase";

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



export interface BossTenant {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  role: RoleState;
  password: string; // For registration form
  isActive: boolean;
  tenant: string;
  shiftConfig: TenantShiftScheduling[] | null;
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
    id: "tenant_12345",
    firstName: "John",
    lastName: "Doe",
    phoneNumber: "+1-555-123-4567",
    dateOfBirth: "1985-03-15",
    role: RoleState.TenantBoss,
    password: "SecurePass123!",
    isActive: true,
    tenant: "Acme Corporation",
    shiftConfig: []
  },
  isSubmitting: false,
  isSuccess: false,
  currentLanguage: 'en'
};

export class TenantRegistrationReducer extends ImmerReducer<TenantRegistrationState> {
  setField<Key extends keyof BossTenant>(payload: { key: Key; value: BossTenant[Key] }) {
    this.draftState.tenant[payload.key] = payload.value;
  }

  setShiftConfig(config: TenantShiftScheduling[]) {
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

  static validateTenant(tenant: BossTenant): string | null {
    if (!tenant.firstName.trim()) return "First name is required";
    if (!tenant.lastName.trim()) return "Last name is required";
    if (!tenant.phoneNumber.trim()) return "Phone number is required";
    if (!tenant.tenant.trim()) return "Tenant name is required";
    if (!tenant.password.trim()) return "Password is required";
    if (tenant.password.length < 6) return "Password must be at least 6 characters";
    
    return null; // No validation errors
  }
}

export const TenantRegistrationActions = createActionCreators(TenantRegistrationReducer);
export const tenantRegistrationReducer = createReducerFunction(
  TenantRegistrationReducer,
  initialTenantRegistrationState
);

// Async Actions with proper TypeScript typing
export const submitTenantRegistration = (): ThunkAction<Promise<void>, RootState, unknown, AnyAction> => 
  async (dispatch, getState) => {

    const {tenantClient}  = apiClient
    const state: RootState = getState();
    const tenant: BossTenant = state.tenantRegistration.tenant;

    // Start global loading with predefined operation ID and message
    dispatch(startLoading('registerTenant')); // Will use "register Tenant" message
    dispatch(TenantRegistrationActions.submitStart());

    try {
      // Validate using the static method
      const validationError = TenantRegistrationReducer.validateTenant(tenant);
      
      if (validationError) {
        dispatch(TenantRegistrationActions.submitFailure(validationError));
        dispatch(stopLoading('registerTenant'));
        return;
      }

      const registrationData = {
        ...tenant, 
        role: 'Boss' 
      };

      console.log('🚀 Submitting tenant registration to server:', {
        ...registrationData,
        password: '[REDACTED]' 
      });
      
      // const response = await tenantClient.register(registrationData);
      
      // // Handle successful registration
      // dispatch(TenantRegistrationActions.submitSuccess());
      // dispatch(TenantRegistrationActions.setField({ key: "id", value: response.id }));

      // console.log('✅ Registration successful:', {
      //   id: response.id,
      //   tenant: response.tenant,
      //   token: response.token ? '[RECEIVED]' : '[NOT_RECEIVED]'
      // });

      // Note: tenantClient.register() automatically stores the auth token

    } catch (error: any) {
      // let errorMessage = "Registration failed";
      
      // // Handle different error types
      // if (error?.message) {
      //   errorMessage = error.message;
      // } else if (error?.errors && Array.isArray(error.errors)) {
      //   errorMessage = error.errors.join(', ');
      // } else if (typeof error === 'string') {
      //   errorMessage = error;
      // }

      // console.error('❌ Registration error:', error);
      // dispatch(TenantRegistrationActions.submitFailure(errorMessage));
    } finally {
      // Stop global loading
      dispatch(stopLoading('registerTenant'));
    }
  };