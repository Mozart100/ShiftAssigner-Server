// API Configuration
const API_BASE_URL = 'http://localhost:5000/api/v1'; // Adjust to your server URL

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

export interface BossTenantRegistrationRequest {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  tenant: string;
  password: string;
  role: string;
  shiftConfig?: {
    morning: boolean;
    day: boolean;
    evening: boolean;
  } | null;
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

  // Boss Tenant Registration
  async registerBossTenant(    data: BossTenantRegistrationRequest
  ): Promise<BossTenantRegistrationResponse> {
    return this.makeRequest<BossTenantRegistrationResponse>('/tenants/register', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  // Shift Leader Registration
  async registerShiftLeader(data: {
    id: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    dateOfBirth: string;
    password: string;
  }): Promise<{ token: string }> {
    return this.makeRequest<{ token: string }>('/shiftleaders/register', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }
}

export const apiService = new ApiService();