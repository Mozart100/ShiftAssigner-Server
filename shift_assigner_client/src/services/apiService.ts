/**
 * Legacy API Service - Consider migrating to httpClient.ts
 * Updated to use correct server URL
 */

import { BaseTenantData } from '../models/commonTypes';

// Updated API Configuration to match server
const API_BASE_URL = 'https://localhost:7083/api/v1'; // ✅ Matches server configuration

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

export interface BossTenantRegistrationRequest extends BaseTenantData {
}

export interface BossTenantRegistrationResponse {
  token: string;
  tenant: string;
  id: string;
}

class ApiService {
  private async makeRequest<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${API_BASE_URL}${endpoint}`;
    
    const defaultHeaders = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };

    const config: RequestInit = {
      ...options,
      headers: {
        ...defaultHeaders,
        ...options.headers,
      },
    };

    try {
      const response = await fetch(url, config);
      
      if (!response.ok) {
        // Try to parse error response
        let errorMessage = `HTTP ${response.status}: ${response.statusText}`;
        try {
          const errorData = await response.json();
          errorMessage = errorData.message || errorData.error || errorMessage;
        } catch {
          // If we can't parse error JSON, use the default message
        }
        throw new Error(errorMessage);
      }

      // Check if response has content
      const contentType = response.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        return await response.json();
      }
      
      // Return empty object if no JSON content
      return {} as T;
    } catch (error) {
      console.error(`API request failed: ${endpoint}`, error);
      throw error;
    }
  }

  // Boss Tenant Registration - Updated endpoint
  async registerBossTenant(data: BossTenantRegistrationRequest): Promise<BossTenantRegistrationResponse> {
    return this.makeRequest<BossTenantRegistrationResponse>('/Auth/register-boss-tenant', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  // Shift Leader Registration - Updated endpoint
  async registerShiftLeader(data: {
    id: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    dateOfBirth: string;
    password: string;
  }): Promise<{ token: string }> {
    return this.makeRequest<{ token: string }>('/ShiftLeaders/register', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }
}

export const apiService = new ApiService();