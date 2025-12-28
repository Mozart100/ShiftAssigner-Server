import { combineReducers, createStore } from "redux";
import { configureStore } from "@reduxjs/toolkit";
import { useDispatch, useSelector, TypedUseSelectorHook } from 'react-redux';
import { tenantRegistrationReducer } from "./tenantReducer";

export const rootReducer = combineReducers({
  tenantRegistration: tenantRegistrationReducer
});

export type AppState = ReturnType<typeof rootReducer>;

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

// Use throughout your app instead of plain `useDispatch` and `useSelector`
export const useAppDispatch = () => useDispatch();
export const useAppSelector: TypedUseSelectorHook<AppState> = useSelector;