/**
 * Button Component - Bootstrap-inspired with full variants
 */

import React from 'react';
import { TouchableOpacity, TouchableOpacityProps, ViewStyle, ActivityIndicator } from 'react-native';
import { Typography } from './Typography';
import { colors, spacing, borderRadius, shadows } from './tokens';

// Button Variants
export type ButtonVariant = 
  | 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info'
  | 'outline-primary' | 'outline-secondary' | 'outline-success' | 'outline-danger' | 'outline-warning' | 'outline-info'
  | 'ghost' | 'link';

export type ButtonSize = 'sm' | 'md' | 'lg';

interface ButtonProps extends Omit<TouchableOpacityProps, 'style'> {
  variant?: ButtonVariant;
  size?: ButtonSize;
  fullWidth?: boolean;
  loading?: boolean;
  leftIcon?: React.ReactNode;
  rightIcon?: React.ReactNode;
  children: React.ReactNode;
  style?: ViewStyle;
}

// Button Style Generator
const getButtonStyles = (
  variant: ButtonVariant,
  size: ButtonSize,
  fullWidth: boolean,
  disabled?: boolean,
  loading?: boolean
) => {
  // Size-based styles
  const sizeStyles = {
    sm: {
      paddingHorizontal: spacing[3],
      paddingVertical: spacing[2],
      minHeight: 32,
    },
    md: {
      paddingHorizontal: spacing[4],
      paddingVertical: spacing[3],
      minHeight: 40,
    },
    lg: {
      paddingHorizontal: spacing[6],
      paddingVertical: spacing[4],
      minHeight: 48,
    },
  };

  // Base button style
  let buttonStyle: ViewStyle = {
    ...sizeStyles[size],
    borderRadius: borderRadius.base,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    ...(fullWidth && { width: '100%' }),
    ...(disabled && { opacity: 0.6 }),
  };

  // Variant-based styles
  switch (variant) {
    case 'primary':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: colors.primary[500],
        ...(!disabled && shadows.sm),
      };
      break;
    case 'secondary':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: colors.secondary[500],
        ...(!disabled && shadows.sm),
      };
      break;
    case 'success':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: colors.success[500],
        ...(!disabled && shadows.sm),
      };
      break;
    case 'danger':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: colors.danger[500],
        ...(!disabled && shadows.sm),
      };
      break;
    case 'warning':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: colors.warning[500],
        ...(!disabled && shadows.sm),
      };
      break;
    case 'info':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: colors.info[500],
        ...(!disabled && shadows.sm),
      };
      break;

    // Outline variants
    case 'outline-primary':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: 'transparent',
        borderWidth: 2,
        borderColor: colors.primary[500],
      };
      break;
    case 'outline-secondary':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: 'transparent',
        borderWidth: 2,
        borderColor: colors.secondary[500],
      };
      break;
    case 'outline-success':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: 'transparent',
        borderWidth: 2,
        borderColor: colors.success[500],
      };
      break;
    case 'outline-danger':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: 'transparent',
        borderWidth: 2,
        borderColor: colors.danger[500],
      };
      break;
    case 'outline-warning':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: 'transparent',
        borderWidth: 2,
        borderColor: colors.warning[500],
      };
      break;
    case 'outline-info':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: 'transparent',
        borderWidth: 2,
        borderColor: colors.info[500],
      };
      break;

    // Ghost and link variants
    case 'ghost':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: 'transparent',
      };
      break;
    case 'link':
      buttonStyle = {
        ...buttonStyle,
        backgroundColor: 'transparent',
        padding: 0,
        minHeight: 'auto' as any,
      };
      break;
  }

  return buttonStyle;
};

// Get text color based on variant
const getTextColor = (variant: ButtonVariant, disabled?: boolean): string => {
  if (disabled) return colors.text.disabled;

  switch (variant) {
    case 'primary':
    case 'secondary':
    case 'success':
    case 'danger':
    case 'warning':
    case 'info':
      return colors.text.inverse;

    case 'outline-primary':
    case 'ghost':
      return colors.primary[500];
    case 'outline-secondary':
      return colors.secondary[500];
    case 'outline-success':
      return colors.success[500];
    case 'outline-danger':
      return colors.danger[500];
    case 'outline-warning':
      return colors.warning[500];
    case 'outline-info':
      return colors.info[500];

    case 'link':
      return colors.primary[500];
    default:
      return colors.text.primary;
  }
};

export const Button: React.FC<ButtonProps> = ({
  variant = 'primary',
  size = 'md',
  fullWidth = false,
  loading = false,
  leftIcon,
  rightIcon,
  children,
  disabled,
  style,
  ...props
}) => {
  const buttonStyle = getButtonStyles(variant, size, fullWidth, disabled, loading);
  const textColor = getTextColor(variant, disabled || loading);

  const isDisabled = disabled || loading;

  return (
    <TouchableOpacity
      style={[buttonStyle, style]}
      disabled={isDisabled}
      activeOpacity={0.8}
      {...props}
    >
      {loading && (
        <ActivityIndicator 
          size="small" 
          color={textColor} 
          style={{ marginRight: spacing[2] }}
        />
      )}
      
      {leftIcon && !loading && (
        <>{leftIcon}</>
      )}
      
      <Typography
        variant="button"
        color={textColor as any}
        style={{
          marginLeft: leftIcon && !loading ? spacing[2] : 0,
          marginRight: rightIcon ? spacing[2] : 0,
        }}
      >
        {children}
      </Typography>
      
      {rightIcon && <>{rightIcon}</>}
    </TouchableOpacity>
  );
};