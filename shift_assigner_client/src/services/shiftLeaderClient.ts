/**
 * Shift Leader-specific API Client
 * Handles all shift leader operations and team management
 */

import HttpClientBase from './httpClientBase';
import type { ShiftLeaderRegistrationRequest, ShiftLeaderRegistrationResponse } from './httpClientBase';

export interface ShiftLeaderProfile {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  tenantId: string;
  isActive: boolean;
  createdAt: string;
}

export interface ShiftLeaderLoginRequest {
  id: string;
  password: string;
}

export interface ShiftLeaderLoginResponse {
  token: string;
  id: string;
  tenantId: string;
}

export interface WorkerAssignment {
  workerId: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  assignedDate: string;
  isActive: boolean;
}

class ShiftLeaderClient extends HttpClientBase {
  /**
   * Register a new shift leader
   */
  async register(data: ShiftLeaderRegistrationRequest): Promise<ShiftLeaderRegistrationResponse> {
    const response = await this.post<ShiftLeaderRegistrationRequest, ShiftLeaderRegistrationResponse>(
      '/ShiftLeaders/register', 
      data
    );
    
    // Auto-store the auth token after successful registration
    if (response.token) {
      this.setAuthToken(response.token);
    }
    
    return response;
  }

  /**
   * Login existing shift leader
   */
  async login(credentials: ShiftLeaderLoginRequest): Promise<ShiftLeaderLoginResponse> {
    const response = await this.post<{ id: string; password: string }, { token: string }>('/ShiftLeaders/login', credentials);
    
    // Auto-store the auth token after successful login
    if (response.token) {
      this.setAuthToken(response.token);
    }
    
    return response;
  }

  /**
   * Get current shift leader profile
   */
  async getProfile(): Promise<ShiftLeaderProfile> {
    return this.get<ShiftLeaderProfile>('/ShiftLeaders/profile');
  }

  /**
   * Update shift leader profile
   */
  async updateProfile(data: Partial<ShiftLeaderProfile>): Promise<ShiftLeaderProfile> {
    return this.put<Partial<ShiftLeaderProfile>, ShiftLeaderProfile>('/ShiftLeaders/profile', data);
  }

  /**
   * Get workers assigned to this shift leader
   */
  async getAssignedWorkers(): Promise<WorkerAssignment[]> {
    return this.get<WorkerAssignment[]>('/ShiftLeaders/workers');
  }

  /**
   * Assign a worker to this shift leader
   */
  async assignWorker(workerId: string): Promise<void> {
    return this.post<{ workerId: string }, void>('/ShiftLeaders/assign-worker', { workerId });
  }

  /**
   * Remove a worker assignment
   */
  async unassignWorker(workerId: string): Promise<void> {
    return this.delete(`/ShiftLeaders/workers/${workerId}`);
  }

  /**
   * Get available shifts for this shift leader
   */
  async getAvailableShifts(): Promise<Array<{
    shiftName: string;
    minimumAmountOfWorkers: number;
    maximumAmountOfWorkers: number;
    currentWorkerCount: number;
  }>> {
    return this.get('/ShiftLeaders/available-shifts');
  }

  /**
   * Schedule workers for a specific shift
   */
  async scheduleWorkersForShift(data: {
    shiftName: string;
    date: string;
    workerIds: string[];
  }): Promise<void> {
    return this.post('/ShiftLeaders/schedule-shift', data);
  }

  /**
   * Get shift schedule for a date range
   */
  async getShiftSchedule(startDate: string, endDate: string): Promise<Array<{
    date: string;
    shiftName: string;
    assignedWorkers: Array<{
      id: string;
      firstName: string;
      lastName: string;
    }>;
  }>> {
    return this.get('/ShiftLeaders/schedule', {
      params: { startDate, endDate }
    });
  }

  /**
   * Logout current shift leader
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
export const shiftLeaderClient = new ShiftLeaderClient();
export default shiftLeaderClient;