/**
 * Input Component - Bootstrap-inspired form inputs
 */

import React, { useState } from 'react';
import { TextInput, TextInputProps, View, ViewStyle } from 'react-native';
import { Typography } from './Typography';
import { colors, spacing, borderRadius, typography } from './tokens';
import { useLanguage } from '../localization';

interface InputProps extends Omit<TextInputProps, 'style'> {
  label?: string;
  error?: string;
  helperText?: string;
  leftIcon?: React.ReactNode;
  rightIcon?: React.ReactNode;
  size?: 'sm' | 'md' | 'lg';
  variant?: 'outlined' | 'filled';
  fullWidth?: boolean;
  containerStyle?: ViewStyle;
  inputStyle?: ViewStyle;
  required?: boolean;
  disabled?: boolean;
}

export const Input: React.FC<InputProps> = ({
  label,
  error,
  helperText,
  leftIcon,
  rightIcon,
  size = 'md',
  variant = 'outlined',
  fullWidth = true,
  containerStyle,
  inputStyle,
  required = false,
  disabled,
  ...props
}) => {
  const [isFocused, setIsFocused] = useState(false);
  const { textAlign } = useLanguage();

  // Size-based styles
  const sizeStyles = {
    sm: {
      paddingHorizontal: spacing[2],
      paddingVertical: spacing[1],
      minHeight: 28,
    },
    md: {
      paddingHorizontal: spacing[4],
      paddingVertical: spacing[3],
      minHeight: 44,
    },
    lg: {
      paddingHorizontal: spacing[6],
      paddingVertical: spacing[5],
      minHeight: 56,
    },
  };

  const fontSizes = {
    sm: typography.fontSize.xs,
    md: typography.fontSize.base,
    lg: typography.fontSize.xl,
  };

  // Get input container style
  const getInputContainerStyle = (): ViewStyle => {
    const baseStyle: ViewStyle = {
      flexDirection: 'row',
      alignItems: 'center',
      borderRadius: borderRadius.base,
      ...(fullWidth && { width: '100%' }),
      ...sizeStyles[size],
    };

    if (variant === 'outlined') {
      return {
        ...baseStyle,
        backgroundColor: size === 'lg' ? '#e8f5e8' : colors.background.paper, // Temporary green for lg
        borderWidth: 2,
        borderColor: error
          ? colors.danger[500]
          : isFocused
          ? colors.primary[500]
          : colors.border.default,
      };
    }

    // Filled variant
    return {
      ...baseStyle,
      backgroundColor: disabled ? colors.gray[100] : colors.gray[50],
      borderBottomWidth: 2,
      borderBottomColor: error
        ? colors.danger[500]
        : isFocused
        ? colors.primary[500]
        : colors.border.default,
    };
  };

  // Get text input style
  const getTextInputStyle = () => {
    return {
      flex: 1,
      fontSize: fontSizes[size],
      color: disabled ? colors.text.disabled : colors.text.primary,
      textAlign: textAlign as 'left' | 'right' | 'center',
      fontFamily: typography.fontFamily.sans,
      ...(leftIcon && { marginLeft: spacing[2] }),
      ...(rightIcon && { marginRight: spacing[2] }),
    };
  };

  return (
    <View style={[{ marginBottom: spacing[1] }, containerStyle]}>
      {/* Label */}
      {label && (
        <Typography
          variant="label"
          color={error ? 'danger' : 'text-primary'}
          style={{ marginBottom: spacing[1] }}
        >
          {label}
          {required && (
            <Typography color="danger"> *</Typography>
          )}
        </Typography>
      )}

      {/* Input Container */}
      <View style={[getInputContainerStyle(), inputStyle]}>
        {leftIcon && <View>{leftIcon}</View>}
        
        <TextInput
          style={getTextInputStyle()}
          placeholderTextColor={colors.text.hint}
          editable={!disabled}
          onFocus={() => setIsFocused(true)}
          onBlur={() => setIsFocused(false)}
          {...props}
        />
        
        {rightIcon && <View>{rightIcon}</View>}
      </View>

      {/* Helper Text / Error */}
      {(error || helperText) && (
        <Typography
          variant="caption"
          color={error ? 'danger' : 'text-secondary'}
          style={{ marginTop: spacing[1] }}
        >
          {error || helperText}
        </Typography>
      )}
    </View>
  );
};