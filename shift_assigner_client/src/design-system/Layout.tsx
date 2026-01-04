/**
 * Layout Components - Bootstrap-inspired containers and utilities
 */

import React from "react";
import { View, ViewStyle, ScrollView, ScrollViewProps } from "react-native";
import { spacing, colors, shadows, borderRadius } from "./tokens";

// Stack Component (Flexbox utility)
interface StackProps {
  direction?: "row" | "column";
  align?: "flex-start" | "center" | "flex-end" | "stretch";
  justify?:
    | "flex-start"
    | "center"
    | "flex-end"
    | "space-between"
    | "space-around"
    | "space-evenly";
  gap?: keyof typeof spacing;
  padding?: keyof typeof spacing;
  margin?: keyof typeof spacing;
  marginTop?: keyof typeof spacing;
  wrap?: boolean;
  flex?: number;
  style?: ViewStyle;
  children: React.ReactNode;
  testID?: string;
}

export const Stack: React.FC<StackProps> = ({
  direction = "column",
  align = "stretch",
  justify = "flex-start",
  gap = 0,
  padding = 0,
  margin = 0,
  marginTop = 0,
  wrap = false,
  flex,
  style,
  children,
  testID,
}) => {
  const stackStyle: ViewStyle = {
    flexDirection: direction,
    alignItems: align,
    justifyContent: justify,
    gap: spacing[gap],
    padding: spacing[padding],
    margin: spacing[margin],
    marginTop: spacing[marginTop],
    flexWrap: wrap ? "wrap" : "nowrap",
    ...(flex !== undefined && { flex }),
    ...style,
  };

  return (
    <View style={stackStyle} testID={testID}>
      {children}
    </View>
  );
};

// HStack (Horizontal Stack)
export const HStack: React.FC<Omit<StackProps, "direction">> = (props) => (
  <Stack direction="row" {...props} />
);

// VStack (Vertical Stack)
export const VStack: React.FC<Omit<StackProps, "direction">> = (props) => (
  <Stack direction="column" {...props} />
);

// Container Component (Bootstrap-inspired)
interface ContainerProps {
  fluid?: boolean;
  padding?: keyof typeof spacing;
  margin?: keyof typeof spacing;
  maxWidth?: number;
  style?: ViewStyle;
  children: React.ReactNode;
  testID?: string;
}

export const Container: React.FC<ContainerProps> = ({
  fluid = false,
  padding = 4,
  margin = 0,
  maxWidth,
  style,
  children,
  testID,
}) => {
  const containerStyle: ViewStyle = {
    width: "100%",
    paddingHorizontal: spacing[padding],
    marginHorizontal: spacing[margin],
    ...(maxWidth && { maxWidth }),
    ...(!fluid && { alignSelf: "center" }),
    ...style,
  };

  return (
    <View style={containerStyle} testID={testID}>
      {children}
    </View>
  );
};

// Card Component (Bootstrap-inspired)
interface CardProps {
  padding?: keyof typeof spacing;
  margin?: keyof typeof spacing;
  radius?: keyof typeof borderRadius;
  shadow?: keyof typeof shadows;
  backgroundColor?: string;
  style?: ViewStyle;
  children: React.ReactNode;
  testID?: string;
}

export const Card: React.FC<CardProps> = ({
  padding = 4,
  margin = 0,
  radius = "base",
  shadow = "sm",
  backgroundColor = colors.background.paper,
  style,
  children,
  testID,
}) => {
  const cardStyle: ViewStyle = {
    backgroundColor,
    padding: spacing[padding],
    margin: spacing[margin],
    borderRadius: borderRadius[radius],
    ...shadows[shadow],
    ...style,
  };

  return (
    <View style={cardStyle} testID={testID}>
      {children}
    </View>
  );
};

// Section Component (Form sections)
interface SectionProps {
  title?: string;
  padding?: keyof typeof spacing;
  margin?: keyof typeof spacing;
  style?: ViewStyle;
  children: React.ReactNode;
  testID?: string;
}

export const Section: React.FC<SectionProps> = ({
  title,
  padding = 4,
  margin = 2,
  style,
  children,
  testID,
}) => {
  return (
    <Card padding={padding} margin={margin} style={style} testID={testID}>
      {title && <VStack gap={3}>{children}</VStack>}
      {!title && children}
    </Card>
  );
};

// SafeArea Container
interface SafeContainerProps extends ScrollViewProps {
  padding?: keyof typeof spacing;
  backgroundColor?: string;
  children: React.ReactNode;
}

export const SafeContainer: React.FC<SafeContainerProps> = ({
  padding = 4,
  backgroundColor = colors.background.default,
  children,
  style,
  ...scrollProps
}) => {
  return (
    <ScrollView
      style={[{ flex: 1, backgroundColor }, style]}
      contentContainerStyle={{
        flexGrow: 1,
        padding: spacing[padding],
      }}
      showsVerticalScrollIndicator={false}
      keyboardShouldPersistTaps="handled"
      {...scrollProps}
    >
      {children}
    </ScrollView>
  );
};