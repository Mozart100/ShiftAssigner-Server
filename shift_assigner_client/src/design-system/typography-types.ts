/**
 * Typography Types - Full TypeScript support
 */

import { TextStyle } from 'react-native';
import { colors, typography } from './tokens';

// Typography Variants
export type TypographyVariant = 
  | 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6'
  | 'subtitle1' | 'subtitle2'
  | 'body1' | 'body2'
  | 'button'
  | 'caption'
  | 'overline'
  | 'label'
  | 'input';

// Color Variants
export type ColorVariant = 
  | 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info'
  | 'text-primary' | 'text-secondary' | 'text-disabled' | 'text-hint' | 'text-inverse'
  | 'muted' | 'white' | 'black';

// Typography Component Props
export interface TypographyProps {
  variant?: TypographyVariant;
  color?: ColorVariant;
  size?: keyof typeof typography.fontSize;
  weight?: keyof typeof typography.fontWeight;
  align?: 'left' | 'center' | 'right' | 'justify';
  transform?: 'none' | 'uppercase' | 'lowercase' | 'capitalize';
  decoration?: 'none' | 'underline' | 'line-through';
  spacing?: keyof typeof typography.letterSpacing;
  lineHeight?: keyof typeof typography.lineHeight;
  italic?: boolean;
  numberOfLines?: number;
  children: React.ReactNode;
  style?: TextStyle;
  testID?: string;
}

// Predefined Typography Styles (Bootstrap-inspired)
export const typographyVariants: Record<TypographyVariant, TextStyle> = {
  // Headings
  h1: {
    fontSize: typography.fontSize['5xl'],
    fontWeight: typography.fontWeight.bold,
    lineHeight: typography.lineHeight.tight * typography.fontSize['5xl'],
    letterSpacing: typography.letterSpacing.tight,
  },
  h2: {
    fontSize: typography.fontSize['4xl'],
    fontWeight: typography.fontWeight.bold,
    lineHeight: typography.lineHeight.tight * typography.fontSize['4xl'],
    letterSpacing: typography.letterSpacing.tight,
  },
  h3: {
    fontSize: typography.fontSize['3xl'],
    fontWeight: typography.fontWeight.bold,
    lineHeight: typography.lineHeight.tight * typography.fontSize['3xl'],
    letterSpacing: typography.letterSpacing.tight,
  },
  h4: {
    fontSize: typography.fontSize['2xl'],
    fontWeight: typography.fontWeight.bold,
    lineHeight: typography.lineHeight.snug * typography.fontSize['2xl'],
    letterSpacing: typography.letterSpacing.normal,
  },
  h5: {
    fontSize: typography.fontSize.xl,
    fontWeight: typography.fontWeight.bold,
    lineHeight: typography.lineHeight.snug * typography.fontSize.xl,
    letterSpacing: typography.letterSpacing.normal,
  },
  h6: {
    fontSize: typography.fontSize.lg,
    fontWeight: typography.fontWeight.bold,
    lineHeight: typography.lineHeight.normal * typography.fontSize.lg,
    letterSpacing: typography.letterSpacing.normal,
  },

  // Subtitles
  subtitle1: {
    fontSize: typography.fontSize.base,
    fontWeight: typography.fontWeight.medium,
    lineHeight: typography.lineHeight.normal * typography.fontSize.base,
    letterSpacing: typography.letterSpacing.wide,
  },
  subtitle2: {
    fontSize: typography.fontSize.sm,
    fontWeight: typography.fontWeight.medium,
    lineHeight: typography.lineHeight.normal * typography.fontSize.sm,
    letterSpacing: typography.letterSpacing.wide,
  },

  // Body Text
  body1: {
    fontSize: typography.fontSize.base,
    fontWeight: typography.fontWeight.normal,
    lineHeight: typography.lineHeight.relaxed * typography.fontSize.base,
    letterSpacing: typography.letterSpacing.normal,
  },
  body2: {
    fontSize: typography.fontSize.sm,
    fontWeight: typography.fontWeight.normal,
    lineHeight: typography.lineHeight.normal * typography.fontSize.sm,
    letterSpacing: typography.letterSpacing.normal,
  },

  // Button
  button: {
    fontSize: typography.fontSize.sm,
    fontWeight: typography.fontWeight.semibold,
    lineHeight: typography.lineHeight.normal * typography.fontSize.sm,
    letterSpacing: typography.letterSpacing.wide,
    textTransform: 'uppercase',
  },

  // Caption
  caption: {
    fontSize: typography.fontSize.xs,
    fontWeight: typography.fontWeight.normal,
    lineHeight: typography.lineHeight.normal * typography.fontSize.xs,
    letterSpacing: typography.letterSpacing.wide,
  },

  // Overline
  overline: {
    fontSize: typography.fontSize.xs,
    fontWeight: typography.fontWeight.medium,
    lineHeight: typography.lineHeight.normal * typography.fontSize.xs,
    letterSpacing: typography.letterSpacing.widest,
    textTransform: 'uppercase',
  },

  // Label
  label: {
    fontSize: typography.fontSize.sm,
    fontWeight: typography.fontWeight.medium,
    lineHeight: typography.lineHeight.normal * typography.fontSize.sm,
    letterSpacing: typography.letterSpacing.normal,
  },

  // Input
  input: {
    fontSize: typography.fontSize.base,
    fontWeight: typography.fontWeight.normal,
    lineHeight: typography.lineHeight.normal * typography.fontSize.base,
    letterSpacing: typography.letterSpacing.normal,
  },
};

// Color Mapping
export const getTextColor = (variant?: ColorVariant): string => {
  switch (variant) {
    case 'primary':
      return colors.primary[500];
    case 'secondary':
      return colors.secondary[500];
    case 'success':
      return colors.success[500];
    case 'danger':
      return colors.danger[500];
    case 'warning':
      return colors.warning[500];
    case 'info':
      return colors.info[500];
    case 'text-primary':
      return colors.text.primary;
    case 'text-secondary':
      return colors.text.secondary;
    case 'text-disabled':
      return colors.text.disabled;
    case 'text-hint':
      return colors.text.hint;
    case 'text-inverse':
      return colors.text.inverse;
    case 'muted':
      return colors.gray[500];
    case 'white':
      return colors.background.paper;
    case 'black':
      return colors.text.primary;
    default:
      return colors.text.primary;
  }
};