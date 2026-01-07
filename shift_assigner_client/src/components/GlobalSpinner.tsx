/**
 * GlobalSpinner - Redux-connected loading overlay
 */

import React from 'react';
import { View } from 'react-native';
import { useSelector } from 'react-redux';
import { RootState } from '../store';
import { Spinner, Typography } from '../design-system';

export const GlobalSpinner: React.FC = () => {
  const { isLoading, currentMessage } = useSelector((state: RootState) => state.loading);

  if (!isLoading) {
    return null;
  }

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
      }}
      testID="global-spinner"
    >
      <Spinner size="large" />
      {currentMessage && (
        <Typography 
          variant="body1" 
          align="center"
          style={{ 
            marginTop: 16,
            color: '#fff',
            fontSize: 16,
            fontWeight: '500',
          }}
        >
          {currentMessage}
        </Typography>
      )}
    </View>
  );
};

// Hook for easy loading control
export const useGlobalLoading = () => {
  const loadingState = useSelector((state: RootState) => state.loading);
  
  return {
    isLoading: loadingState.isLoading,
    currentMessage: loadingState.currentMessage,
    operations: loadingState.operations,
  };
};