/**
 * Worker-specific API Client
 * Handles all worker operations and shift management
 */

import HttpClientBase from './httpClientBase';

export interface WorkerRegistrationRequest {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  password: string;
}

export interface WorkerProfile {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  tenantId: string;
  shiftLeaderId?: string;
  isActive: boolean;
  createdAt: string;
}

export interface WorkerLoginRequest {
  id: string;
  password: string;
}

export interface WorkerLoginResponse {
  token: string;
  id: string;
  tenantId: string;
  shiftLeaderId?: string;
}

export interface WorkerShift {
  date: string;
  shiftName: string;
  startTime: string;
  endTime: string;
  shiftLeader: {
    id: string;
    firstName: string;
    lastName: string;
  };
  status: 'scheduled' | 'completed' | 'missed' | 'cancelled';
}

class WorkerClient extends HttpClientBase {
  /**
   * Register a new worker
   */
  async register(data: WorkerRegistrationRequest): Promise<{ token: string }> {
    const response = await this.post<WorkerRegistrationRequest, { token: string }>('/Workers/register', data);
    
    // Auto-store the auth token after successful registration
    if (response.token) {
      this.setAuthToken(response.token);
    }
    
    return response;
  }

  /**
   * Login existing worker
   */
  async login(credentials: WorkerLoginRequest): Promise<WorkerLoginResponse> {
    const response = await this.post<{ id: string; password: string }, { token: string }>('/Workers/login', credentials);
    
    // Auto-store the auth token after successful login
    if (response.token) {
      this.setAuthToken(response.token);
    }
    
    return response;
  }

  /**
   * Get current worker profile
   */
  async getProfile(): Promise<WorkerProfile> {
    return this.get<WorkerProfile>('/Workers/profile');
  }

  /**
   * Update worker profile
   */
  async updateProfile(data: Partial<WorkerProfile>): Promise<WorkerProfile> {
    return this.put<Partial<WorkerProfile>, WorkerProfile>('/Workers/profile', data);
  }

  /**
   * Get worker's assigned shifts for a date range
   */
  async getMyShifts(startDate: string, endDate: string): Promise<WorkerShift[]> {
    return this.get<WorkerShift[]>('/Workers/my-shifts', {
      params: { startDate, endDate }
    });
  }

  /**
   * Get today's shifts for the worker
   */
  async getTodayShifts(): Promise<WorkerShift[]> {
    const today = new Date().toISOString().split('T')[0];
    return this.getMyShifts(today, today);
  }

  /**
   * Check in for a shift
   */
  async checkInShift(shiftId: string): Promise<void> {
    return this.post<{ shiftId: string }, void>('/Workers/check-in', { shiftId });
  }

  /**
   * Check out from a shift
   */
  async checkOutShift(shiftId: string): Promise<void> {
    return this.post<{ shiftId: string }, void>('/Workers/check-out', { shiftId });
  }

  /**
   * Request time off
   */
  async requestTimeOff(data: {
    startDate: string;
    endDate: string;
    reason: string;
  }): Promise<void> {
    return this.post('/Workers/time-off-request', data);
  }

  /**
   * Get time off requests history
   */
  async getTimeOffRequests(): Promise<Array<{
    id: string;
    startDate: string;
    endDate: string;
    reason: string;
    status: 'pending' | 'approved' | 'denied';
    createdAt: string;
  }>> {
    return this.get('/Workers/time-off-requests');
  }

  /**
   * Get worker's shift leader information
   */
  async getShiftLeaderInfo(): Promise<{
    id: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
  } | null> {
    return this.get('/Workers/shift-leader');
  }

  /**
   * Update availability preferences
   */
  async updateAvailability(availability: {
    mondayMorning: boolean;
    mondayDay: boolean;
    mondayEvening: boolean;
    tuesdayMorning: boolean;
    tuesdayDay: boolean;
    tuesdayEvening: boolean;
    wednesdayMorning: boolean;
    wednesdayDay: boolean;
    wednesdayEvening: boolean;
    thursdayMorning: boolean;
    thursdayDay: boolean;
    thursdayEvening: boolean;
    fridayMorning: boolean;
    fridayDay: boolean;
    fridayEvening: boolean;
    saturdayMorning: boolean;
    saturdayDay: boolean;
    saturdayEvening: boolean;
    sundayMorning: boolean;
    sundayDay: boolean;
    sundayEvening: boolean;
  }): Promise<void> {
    return this.put('/Workers/availability', availability);
  }

  /**
   * Logout current worker
   */
  async logout(): Promise<void> {
    try {
      await this.post('/Auth/logout', {});
    } finally {
      // Always clear the token, even if logout fails
      this.clearAuthToken();
    }
  }
}

// Export singleton instance
export const workerClient = new WorkerClient();
export default workerClient;