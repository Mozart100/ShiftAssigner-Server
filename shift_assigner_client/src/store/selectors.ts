import { TenantRegistrationState, BossTenant } from "./types";

export const selectTenant = (state: { tenantRegistration: TenantRegistrationState }): BossTenant =>
  state.tenantRegistration.tenant;

export const selectIsSubmitting = (state: { tenantRegistration: TenantRegistrationState }): boolean =>
  state.tenantRegistration.isSubmitting;

export const selectIsSuccess = (state: { tenantRegistration: TenantRegistrationState }): boolean =>
  state.tenantRegistration.isSuccess;

export const selectError = (state: { tenantRegistration: TenantRegistrationState }): string | undefined =>
  state.tenantRegistration.error;

export const selectIsFormValid = (state: { tenantRegistration: TenantRegistrationState }): boolean => {
  const { tenant } = state.tenantRegistration;
  return (
    tenant.firstName.trim() !== "" &&
    tenant.lastName.trim() !== "" &&
    tenant.phoneNumber.trim() !== "" &&
    tenant.tenant.trim() !== ""
  );
};

export const selectFullName = (state: { tenantRegistration: TenantRegistrationState }): string => {
  const tenant = selectTenant(state);
  return `${tenant.firstName} ${tenant.lastName}`.trim();
};