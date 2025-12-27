import { combineReducers, createStore } from "redux";
import { configureStore } from "@reduxjs/toolkit";
import { tenantRegistrationReducer } from "./tenantReducer";

export const rootReducer = combineReducers({
  tenantRegistration: tenantRegistrationReducer
});

export type RootState = ReturnType<typeof rootReducer>;

export const store = configureStore({
  reducer: rootReducer,
  devTools: process.env.NODE_ENV !== 'production',
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      thunk: true,
      serializableCheck: {
        ignoredActions: [], // Add actions to ignore if needed
      },
    }),
});