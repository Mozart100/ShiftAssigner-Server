/**
 * Tenant-specific API Client
 * Handles all tenant/boss organization management operations
 */

import HttpClientBase from './httpClientBase';
import type { BossTenantRegistrationRequest, BossTenantRegistrationResponse } from './httpClientBase';

export interface TenantShiftConfig {
  shiftName: string;
  minimumAmountOfWorkers: number;
  maximumAmountOfWorkers: number;
}

export interface TenantProfile {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  tenant: string;
  role: string;
  createdAt: string;
}

export interface TenantLoginRequest {
  tenant: string;
  password: string;
}

export interface TenantLoginResponse {
  token: string;
  tenant: string;
  id: string;
  role: string;
}

class TenantClient extends HttpClientBase {
  /**
   * Register a new boss tenant (organization owner)
   */
  async register(data: BossTenantRegistrationRequest): Promise<BossTenantRegistrationResponse> {
    const response = await this.post<BossTenantRegistrationRequest, BossTenantRegistrationResponse>(
      '/Auth/register-boss-tenant', 
      data
    );
    
    // Auto-store the auth token after successful registration
    if (response.token) {
      this.setAuthToken(response.token);
    }
    
    return response;
  }

  /**
   * Login existing tenant
   */
  async login(credentials: TenantLoginRequest): Promise<TenantLoginResponse> {
    const response = await this.post<TenantLoginRequest, TenantLoginResponse>(
      '/Auth/login-tenant', 
      credentials
    );
    
    // Auto-store the auth token after successful login
    if (response.token) {
      this.setAuthToken(response.token);
    }
    
    return response;
  }

  /**
   * Get current tenant profile
   */
  async getProfile(): Promise<TenantProfile> {
    return this.get<TenantProfile>('/Tenant/profile');
  }

  /**
   * Update tenant profile
   */
  async updateProfile(data: Partial<TenantProfile>): Promise<TenantProfile> {
    return this.put<Partial<TenantProfile>, TenantProfile>('/Tenant/profile', data);
  }

  /**
   * Get tenant's shift configuration
   */
  async getShiftConfig(): Promise<TenantShiftConfig[]> {
    return this.get<TenantShiftConfig[]>('/Tenant/shifts');
  }

  /**
   * Update tenant's shift configuration
   */
  async updateShiftConfig(shifts: TenantShiftConfig[]): Promise<TenantShiftConfig[]> {
    return this.put<TenantShiftConfig[], TenantShiftConfig[]>('/Tenant/shifts', shifts);
  }

  /**
   * Get all shift leaders under this tenant
   */
  async getShiftLeaders(): Promise<Array<{
    id: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    isActive: boolean;
  }>> {
    return this.get('/Tenant/shift-leaders');
  }

  /**
   * Get all workers under this tenant
   */
  async getWorkers(): Promise<Array<{
    id: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    shiftLeaderId?: string;
    isActive: boolean;
  }>> {
    return this.get('/Tenant/workers');
  }

  /**
   * Get tenant dashboard statistics
   */
  async getDashboardStats(): Promise<{
    totalShiftLeaders: number;
    totalWorkers: number;
    activeShifts: number;
    totalShifts: number;
  }> {
    return this.get('/Tenant/dashboard/stats');
  }

  /**
   * Logout current tenant
   */
  async logout(): Promise<void> {
    try {
      await this.post('/Auth/logout', {});
    } finally {
      // Always clear the token, even if logout fails
      this.clearAuthToken();
    }
  }

  /**
   * Delete tenant account (careful!)
   */
  async deleteAccount(): Promise<void> {
    await this.delete('/Tenant/account');
    this.clearAuthToken();
  }
}

// Export singleton instance
export const tenantClient = new TenantClient();
export default tenantClient;