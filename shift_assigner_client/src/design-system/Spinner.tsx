/**
 * Spinner Component - Reusable loading animation
 */

import React from 'react';
import { View, ViewStyle, ActivityIndicator } from 'react-native';
import { colors } from './tokens';

interface SpinnerProps {
  size?: 'small' | 'large' | number;
  color?: string;
  style?: ViewStyle;
  overlay?: boolean; // Shows spinner over content with backdrop
  testID?: string;
}

export const Spinner: React.FC<SpinnerProps> = ({
  size = 'large',
  color = colors.primary.main,
  style,
  overlay = false,
  testID,
}) => {
  if (overlay) {
    return (
      <View
        style={{
          position: 'absolute',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          backgroundColor: 'rgba(0, 0, 0, 0.3)',
          justifyContent: 'center',
          alignItems: 'center',
          zIndex: 1000,
          ...style,
        }}
        testID={testID}
      >
        <View
          style={{
            backgroundColor: 'white',
            borderRadius: 12,
            padding: 24,
            alignItems: 'center',
            shadowColor: '#000',
            shadowOffset: {
              width: 0,
              height: 2,
            },
            shadowOpacity: 0.25,
            shadowRadius: 3.84,
            elevation: 5,
          }}
        >
          <ActivityIndicator size={size} color={color} />
        </View>
      </View>
    );
  }

  return (
    <View
      style={{
        justifyContent: 'center',
        alignItems: 'center',
        ...style,
      }}
      testID={testID}
    >
      <ActivityIndicator size={size} color={color} />
    </View>
  );
};

// Loading overlay hook for easy integration
export const useLoadingOverlay = () => {
  const [isLoading, setIsLoading] = React.useState(false);

  const showLoading = () => setIsLoading(true);
  const hideLoading = () => setIsLoading(false);

  const LoadingOverlay = () => 
    isLoading ? <Spinner overlay size="large" /> : null;

  return {
    isLoading,
    showLoading,
    hideLoading,
    LoadingOverlay,
  };
};