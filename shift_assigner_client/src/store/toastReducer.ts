import { ImmerReducer, createReducerFunction, createActionCreators } from "immer-reducer";

export type ToastType = 'success' | 'warning' | 'error' | 'info';

export interface Toast {
  id: string;
  type: ToastType;
  message: string;
  duration?: number; // in milliseconds, default 4000
  autoHide?: boolean; // default true
}

export interface ToastState {
  toasts: Toast[];
}

export const initialToastState: ToastState = {
  toasts: [],
};

export class ToastReducer extends ImmerReducer<ToastState> {
  showToast(payload: { 
    type: ToastType; 
    message: string; 
    duration?: number;
    autoHide?: boolean;
  }) {
    const toast: Toast = {
      id: `toast_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
      type: payload.type,
      message: payload.message,
      duration: payload.duration || 4000,
      autoHide: payload.autoHide !== false, // default true
    };
    
    this.draftState.toasts.push(toast);
    
    // Limit to maximum 3 toasts
    if (this.draftState.toasts.length > 3) {
      this.draftState.toasts.shift();
    }
  }

  hideToast(payload: { id: string }) {
    this.draftState.toasts = this.draftState.toasts.filter(
      toast => toast.id !== payload.id
    );
  }

  clearAllToasts() {
    this.draftState.toasts = [];
  }
}

export const ToastActions = createActionCreators(ToastReducer);
export const toastReducer = createReducerFunction(ToastReducer, initialToastState);

// Helper action creators
export const showToast = (type: ToastType, message: string, options?: { duration?: number; autoHide?: boolean }) =>
  ToastActions.showToast({ 
    type, 
    message, 
    duration: options?.duration,
    autoHide: options?.autoHide 
  });

export const showSuccess = (message: string, options?: { duration?: number; autoHide?: boolean }) =>
  showToast('success', message, options);

export const showWarning = (message: string, options?: { duration?: number; autoHide?: boolean }) =>
  showToast('warning', message, options);

export const showError = (message: string, options?: { duration?: number; autoHide?: boolean }) =>
  showToast('error', message, options);

export const showInfo = (message: string, options?: { duration?: number; autoHide?: boolean }) =>
  showToast('info', message, options);

export const hideToast = (id: string) => ToastActions.hideToast({ id });
export const clearAllToasts = () => ToastActions.clearAllToasts();