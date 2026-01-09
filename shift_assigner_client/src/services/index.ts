/**
 * Main API Client Export
 * Provides organized access to all domain-specific clients using inheritance
 */

// Import base class and specialized clients
import HttpClientBase from './httpClientBase';
import tenantClient  from './tenantClient';
import shiftLeaderClient from './shiftLeaderClient';


// Export types from all clients
// export type { 
//   BossTenantRegistrationRequest, 
//   BossTenantRegistrationResponse,
//   ShiftLeaderRegistrationRequest,
//   ShiftLeaderRegistrationResponse 
// } from './httpClient';

// export type {
//   TenantShiftConfig,
//   TenantProfile,
//   TenantLoginRequest,
//   TenantLoginResponse
// } from './tenantClient';

// export type {
//   ShiftLeaderProfile,
//   ShiftLeaderLoginRequest,
//   ShiftLeaderLoginResponse,
//   WorkerAssignment
// } from './shiftLeaderClient';

// export type {
//   WorkerRegistrationRequest,
//   WorkerProfile,
//   WorkerLoginRequest,
//   WorkerLoginResponse,
//   WorkerShift
// } from './workerClient';

// Main API client object that organizes all services
export const apiClient = {
  
  // Domain-specific clients (now using inheritance)
  tenantClient: tenantClient,
  shiftLeaderClient: shiftLeaderClient,
  
  // Convenience auth methods that work across all clients
  auth: {
    setToken: (token: string) => {
      // Set token on all client instances
      // httpClient.setAuthToken(token);
      tenantClient.setAuthToken(token);
      shiftLeaderClient.setAuthToken(token);
    },
    clearToken: () => {
      // Clear token from all client instances
      // httpClient.clearAuthToken();
      tenantClient.clearAuthToken();
      shiftLeaderClient.clearAuthToken();
    },
    // healthCheck: () => httpClient.healthCheck(),
  }
};

// Default export
export default apiClient;