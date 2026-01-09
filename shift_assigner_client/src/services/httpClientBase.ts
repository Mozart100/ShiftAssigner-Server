/**
 * Axios-based HTTP Client for ShiftAssigner API
 * Centralized service for all server communication
 */

import axios, { AxiosInstance, AxiosResponse, AxiosRequestConfig } from 'axios';

// Server Configuration
const API_BASE_URL = 'https://localhost:7083/api/v1';

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
  shiftConfig: Array<{
    shiftName: string;
    minimumAmountOfWorkers: number;
    maximumAmountOfWorkers: number;
  }>;
}

export interface BossTenantRegistrationResponse {
  token: string;
  tenant: string;
  id: string;
}

export interface ShiftLeaderRegistrationRequest {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  password: string;
}

export interface ShiftLeaderRegistrationResponse {
  token: string;
}

/**
 * Base HTTP Client Class for ShiftAssigner API
 * All domain-specific clients inherit from this base class
 */

export abstract class HttpClientBase {
  protected client: AxiosInstance;
  protected authToken: string | null = null;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      timeout: 10000, // 10 second timeout
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    // Request interceptor for auth tokens
    this.client.interceptors.request.use(
      (config) => {
        if (this.authToken) {
          config.headers.Authorization = `Bearer ${this.authToken}`;
        }
        
        // Log requests in development
        if (__DEV__) {
          console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`, {
            data: config.data,
            params: config.params,
          });
        }
        
        return config;
      },
      (error) => {
        console.error('❌ Request interceptor error:', error);
        return Promise.reject(error);
      }
    );

    // Response interceptor for error handling
    this.client.interceptors.response.use(
      (response: AxiosResponse) => {
        // Log successful responses in development
        if (__DEV__) {
          console.log(`✅ API Response: ${response.config.method?.toUpperCase()} ${response.config.url}`, {
            status: response.status,
            data: response.data,
          });
        }
        
        return response;
      },
      (error) => {
        // Enhanced error handling
        if (__DEV__) {
          console.error('❌ API Error:', {
            url: error.config?.url,
            method: error.config?.method?.toUpperCase(),
            status: error.response?.status,
            statusText: error.response?.statusText,
            data: error.response?.data,
          });
        }

        // Handle specific error cases
        if (error.response?.status === 401) {
          // Clear stored auth token on unauthorized
          this.clearAuthToken();
        }

        // Return a consistent error format
        const apiError = {
          message: error.response?.data?.message || error.message || 'Network Error',
          status: error.response?.status,
          errors: error.response?.data?.errors || [],
        };

        return Promise.reject(apiError);
      }
    );
  }

  // Authentication methods
  public setAuthToken(token: string) {
    this.authToken = token;
  }

  public clearAuthToken() {
    this.authToken = null;
  }

  // Protected generic request methods for child classes
  protected async get<T>(endpoint: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.get<T>(endpoint, config);
    return response.data;
  }

  protected async post<TRequest, TResponse>(
    endpoint: string, 
    data: TRequest, 
    config?: AxiosRequestConfig
  ): Promise<TResponse> {
    const response = await this.client.post<TResponse>(endpoint, data, config);
    return response.data;
  }

  protected async put<TRequest, TResponse>(
    endpoint: string, 
    data: TRequest, 
    config?: AxiosRequestConfig
  ): Promise<TResponse> {
    const response = await this.client.put<TResponse>(endpoint, data, config);
    return response.data;
  }

  protected async delete<T>(endpoint: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.delete<T>(endpoint, config);
    return response.data;
  }

  // Health check
  public async healthCheck(): Promise<{ status: string; timestamp: string }> {
    return this.get('/health');
  }
}

// Export the base class as default
export default HttpClientBase;