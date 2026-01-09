/**
 * Main API Client Export
 * Provides organized access to all domain-specific clients using inheritance
 */

// Import base class and specialized clients
import HttpClientBase, { httpClient } from './httpClient';
import { TenantClient } from './tenantClient';
import { ShiftLeaderClient } from './shiftLeaderClient';
import { WorkerClient } from './workerClient';

// Create singleton instances of each client
export const tenantClient = new TenantClient();
export const shiftLeaderClient = new ShiftLeaderClient();
export const workerClient = new WorkerClient();

// Re-export base client and class for direct usage
export { httpClient, HttpClientBase };

// Export types from all clients
export type { 
  BossTenantRegistrationRequest, 
  BossTenantRegistrationResponse,
  ShiftLeaderRegistrationRequest,
  ShiftLeaderRegistrationResponse 
} from './httpClient';

export type {
  TenantShiftConfig,
  TenantProfile,
  TenantLoginRequest,
  TenantLoginResponse
} from './tenantClient';

export type {
  ShiftLeaderProfile,
  ShiftLeaderLoginRequest,
  ShiftLeaderLoginResponse,
  WorkerAssignment
} from './shiftLeaderClient';

export type {
  WorkerRegistrationRequest,
  WorkerProfile,
  WorkerLoginRequest,
  WorkerLoginResponse,
  WorkerShift
} from './workerClient';

// Main API client object that organizes all services
export const apiClient = {
  // Base HTTP client for direct access
  http: httpClient,
  
  // Domain-specific clients (now using inheritance)
  tenant: tenantClient,
  shiftLeader: shiftLeaderClient,
  worker: workerClient,
  
  // Convenience auth methods that work across all clients
  auth: {
    setToken: (token: string) => {
      // Set token on all client instances
      httpClient.setAuthToken(token);
      tenantClient.setAuthToken(token);
      shiftLeaderClient.setAuthToken(token);
      workerClient.setAuthToken(token);
    },
    clearToken: () => {
      // Clear token from all client instances
      httpClient.clearAuthToken();
      tenantClient.clearAuthToken();
      shiftLeaderClient.clearAuthToken();
      workerClient.clearAuthToken();
    },
    healthCheck: () => httpClient.healthCheck(),
  }
};

// Default export
export default apiClient;