/**
 * Typography Component - Bootstrap-inspired with full TypeScript support
 */

import React from 'react';
import { Text, TextStyle } from 'react-native';
import { 
  TypographyProps, 
  typographyVariants, 
  getTextColor 
} from './typography-types';
import { typography } from './tokens';
import { useLanguage } from '../localization';

export const Typography: React.FC<TypographyProps> = ({
  variant = 'body1',
  color = 'text-primary',
  size,
  weight,
  align,
  transform,
  decoration,
  spacing,
  lineHeight,
  italic = false,
  numberOfLines,
  children,
  style,
  testID,
}) => {
  const { textAlign: rtlTextAlign, direction } = useLanguage();

  // Start with base variant styles
  const baseStyle = typographyVariants[variant];

  // Build the complete style object
  const textStyle: TextStyle = {
    ...baseStyle,
    
    // Color
    color: getTextColor(color),
    
    // Override with specific props
    ...(size && { fontSize: typography.fontSize[size] }),
    ...(weight && { fontWeight: typography.fontWeight[weight] }),
    ...(spacing && { letterSpacing: typography.letterSpacing[spacing] }),
    ...(lineHeight && { lineHeight: typography.lineHeight[lineHeight] * (size ? typography.fontSize[size] : baseStyle.fontSize || typography.fontSize.base) }),
    
    // Text alignment (RTL-aware)
    textAlign: align || (direction === 'rtl' ? rtlTextAlign as any : 'left'),
    
    // Text decoration and transform
    ...(decoration && { textDecorationLine: decoration }),
    ...(transform && { textTransform: transform }),
    
    // Italic
    ...(italic && { fontStyle: 'italic' }),
    
    // Custom style overrides
    ...style,
  };

  return (
    <Text 
      style={textStyle}
      numberOfLines={numberOfLines}
      testID={testID}
    >
      {children}
    </Text>
  );
};

// Convenience components for common use cases
export const Heading1: React.FC<Omit<TypographyProps, 'variant'>> = (props) => (
  <Typography variant="h1" {...props} />
);

export const Heading2: React.FC<Omit<TypographyProps, 'variant'>> = (props) => (
  <Typography variant="h2" {...props} />
);

export const Heading3: React.FC<Omit<TypographyProps, 'variant'>> = (props) => (
  <Typography variant="h3" {...props} />
);

export const Heading4: React.FC<Omit<TypographyProps, 'variant'>> = (props) => (
  <Typography variant="h4" {...props} />
);

export const Heading5: React.FC<Omit<TypographyProps, 'variant'>> = (props) => (
  <Typography variant="h5" {...props} />
);

export const Heading6: React.FC<Omit<TypographyProps, 'variant'>> = (props) => (
  <Typography variant="h6" {...props} />
);

export const Subtitle: React.FC<Omit<TypographyProps, 'variant'>> = (props) => (
  <Typography variant="subtitle1" {...props} />
);

export const Body: React.FC<Omit<TypographyProps, 'variant'>> = (props) => (
  <Typography variant="body1" {...props} />
);

export const Caption: React.FC<Omit<TypographyProps, 'variant'>> = (props) => (
  <Typography variant="caption" {...props} />
);

export const Label: React.FC<Omit<TypographyProps, 'variant'>> = (props) => (
  <Typography variant="label" {...props} />
);