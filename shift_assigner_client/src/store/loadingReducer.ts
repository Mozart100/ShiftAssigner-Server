import { ImmerReducer, createReducerFunction, createActionCreators } from "immer-reducer";

// Predefined operation IDs for type safety and consistency
export const spinnerOperations = {
  registerShiftLeader: "register ShiftLeader",
  registerTenant: "register Tenant",
} as const;

export type KnownSpinnerOperation = keyof typeof spinnerOperations;

interface LoadingOperation {
  id: string;
  message?: string;
}

export interface LoadingState {
  operations: LoadingOperation[];
  isLoading: boolean;
  currentMessage?: string;
}

export const initialLoadingState: LoadingState = {
  operations: [],
  isLoading: false,
  currentMessage: undefined,
};

export class LoadingReducer extends ImmerReducer<LoadingState> {
  startLoading(payload: { id: string; message?: string }) {
    const { id, message } = payload;
    
    // Add operation if not already present
    if (!this.draftState.operations.find(op => op.id === id)) {
      this.draftState.operations.push({ id, message });
    }
    
    // Update loading state
    this.draftState.isLoading = this.draftState.operations.length > 0;
    this.draftState.currentMessage = message || this.draftState.operations[0]?.message;
  }

  stopLoading(payload: { id: string }) {
    const { id } = payload;
    
    // Remove operation
    this.draftState.operations = this.draftState.operations.filter(op => op.id !== id);
    
    // Update loading state
    this.draftState.isLoading = this.draftState.operations.length > 0;
    this.draftState.currentMessage = this.draftState.operations[0]?.message;
  }

  clearAllLoading() {
    this.draftState.operations = [];
    this.draftState.isLoading = false;
    this.draftState.currentMessage = undefined;
  }
}

export const LoadingActions = createActionCreators(LoadingReducer);
export const loadingReducer = createReducerFunction(LoadingReducer, initialLoadingState);

// Helper action creators with type safety
export const startLoading = (id: KnownSpinnerOperation | string, message?: string) => {
  let operationId: string;
  let operationMessage: string | undefined;
  
  if (typeof id === 'string') {
    operationId = id;
    operationMessage = message;
  } else {
    operationId = spinnerOperations[id];
    operationMessage = message || spinnerOperations[id]; // Use predefined message as default
  }
  
  return LoadingActions.startLoading({ id: operationId, message: operationMessage });
};

export const stopLoading = (id: KnownSpinnerOperation | string) => {
  const operationId = typeof id === 'string' ? id : spinnerOperations[id];
  return LoadingActions.stopLoading({ id: operationId });
};