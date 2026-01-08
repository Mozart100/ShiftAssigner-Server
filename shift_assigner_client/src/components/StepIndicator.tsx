/**
 * Step Indicator Component
 */

import React from 'react';
import { View } from 'react-native';
import { Typography, HStack } from '../design-system';

interface StepIndicatorProps {
  currentStep: number;
  totalSteps: number;
  stepLabels: string[];
}

export const StepIndicator: React.FC<StepIndicatorProps> = ({
  currentStep,
  totalSteps,
  stepLabels,
}) => {
  return (
    <View style={{ marginBottom: 24 }}>
      <HStack justify="space-between" align="center" style={{ marginBottom: 8 }}>
        {Array.from({ length: totalSteps }, (_, index) => {
          const stepNumber = index + 1;
          const isActive = stepNumber === currentStep;
          const isCompleted = stepNumber < currentStep;
          
          return (
            <View key={stepNumber} style={{ alignItems: 'center', flex: 1 }}>
              {/* Step Circle */}
              <View
                style={{
                  width: 32,
                  height: 32,
                  borderRadius: 16,
                  backgroundColor: isCompleted ? '#10B981' : isActive ? '#3B82F6' : '#E5E7EB',
                  alignItems: 'center',
                  justifyContent: 'center',
                  marginBottom: 8,
                }}
              >
                <Typography
                  variant="body2"
                  style={{
                    color: isCompleted || isActive ? 'white' : '#9CA3AF',
                    fontWeight: '600',
                  }}
                >
                  {isCompleted ? '✓' : stepNumber}
                </Typography>
              </View>
              
              {/* Step Label */}
              <Typography
                variant="caption"
                style={{
                  color: isActive ? '#3B82F6' : '#6B7280',
                  fontWeight: isActive ? '600' : '400',
                  textAlign: 'center',
                }}
              >
                {stepLabels[index]}
              </Typography>
            </View>
          );
        })}
      </HStack>
      
      {/* Progress Bar */}
      <View
        style={{
          height: 4,
          backgroundColor: '#E5E7EB',
          borderRadius: 2,
          marginHorizontal: 16,
        }}
      >
        <View
          style={{
            height: 4,
            backgroundColor: '#3B82F6',
            borderRadius: 2,
            width: `${((currentStep - 1) / (totalSteps - 1)) * 100}%`,
          }}
        />
      </View>
    </View>
  );
};