import { TenantRegistrationActions } from './tenantReducer';
import { selectTenant } from './selectors';
import { RootState } from './index';

export const submitTenantRegistration = () => async (dispatch: any, getState: () => RootState) => {
  const state = getState();
  const tenant = selectTenant(state);

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