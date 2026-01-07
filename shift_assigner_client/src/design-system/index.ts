/**
 * Design System - Main Export File
 * Bootstrap-inspired design system with full TypeScript support
 */

// Design Tokens
export * from './tokens';

// Typography
export * from './Typography';
export * from './typography-types';

// Layout Components
export * from './Layout';

// Form Components
export * from './Button';
export * from './Input';

// Loading Components
export * from './Spinner';

// Theme object for easy access
export const theme = {
  colors: require('./tokens').colors,
  typography: require('./tokens').typography,
  spacing: require('./tokens').spacing,
  borderRadius: require('./tokens').borderRadius,
  shadows: require('./tokens').shadows,
};