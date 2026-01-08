/**
 * ToastNotification - Animated toast notifications that slide up from bottom
 */

import React, { useEffect, useRef } from 'react';
import { View, Animated, Dimensions, TouchableOpacity } from 'react-native';
import { useSelector, useDispatch } from 'react-redux';
import { RootState } from '../store';
import { hideToast, Toast, ToastType } from '../store/toastReducer';
import { Typography } from '../design-system';

const { width: screenWidth, height: screenHeight } = Dimensions.get('window');

interface ToastItemProps {
  toast: Toast;
  index: number;
  onHide: (id: string) => void;
}

const ToastItem: React.FC<ToastItemProps> = ({ toast, index, onHide }) => {
  const slideAnim = useRef(new Animated.Value(100)).current;
  const opacityAnim = useRef(new Animated.Value(0)).current;

  useEffect(() => {
    // Slide in animation
    Animated.parallel([
      Animated.timing(slideAnim, {
        toValue: 0,
        duration: 300,
        useNativeDriver: true,
      }),
      Animated.timing(opacityAnim, {
        toValue: 1,
        duration: 300,
        useNativeDriver: true,
      }),
    ]).start();

    // Auto hide
    if (toast.autoHide) {
      const timer = setTimeout(() => {
        hideWithAnimation();
      }, toast.duration);

      return () => clearTimeout(timer);
    }
  }, []);

  const hideWithAnimation = () => {
    Animated.parallel([
      Animated.timing(slideAnim, {
        toValue: 100,
        duration: 250,
        useNativeDriver: true,
      }),
      Animated.timing(opacityAnim, {
        toValue: 0,
        duration: 250,
        useNativeDriver: true,
      }),
    ]).start(() => {
      onHide(toast.id);
    });
  };

  const getToastColors = (type: ToastType) => {
    switch (type) {
      case 'success':
        return { backgroundColor: '#10B981', borderColor: '#059669' }; // Green
      case 'warning':
        return { backgroundColor: '#F59E0B', borderColor: '#D97706' }; // Yellow/Orange
      case 'error':
        return { backgroundColor: '#EF4444', borderColor: '#DC2626' }; // Red
      case 'info':
        return { backgroundColor: '#3B82F6', borderColor: '#2563EB' }; // Blue
      default:
        return { backgroundColor: '#6B7280', borderColor: '#4B5563' }; // Gray
    }
  };

  const colors = getToastColors(toast.type);

  return (
    <Animated.View
      style={{
        transform: [{ translateY: slideAnim }],
        opacity: opacityAnim,
        position: 'absolute',
        bottom: 20 + (index * 80), // Stack toasts
        left: 16,
        right: 16,
        backgroundColor: colors.backgroundColor,
        borderLeftWidth: 4,
        borderLeftColor: colors.borderColor,
        borderRadius: 8,
        padding: 16,
        marginBottom: 8,
        shadowColor: '#000',
        shadowOffset: {
          width: 0,
          height: 2,
        },
        shadowOpacity: 0.25,
        shadowRadius: 3.84,
        elevation: 5,
        zIndex: 1000 + index,
      }}
    >
      <TouchableOpacity
        onPress={hideWithAnimation}
        activeOpacity={0.8}
        style={{
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'space-between',
        }}
      >
        <View style={{ flex: 1, marginRight: 8 }}>
          <Typography
            variant="body1"
            style={{
              color: 'white',
              fontSize: 14,
              fontWeight: '500',
              lineHeight: 20,
            }}
          >
            {toast.message}
          </Typography>
        </View>
        
        {/* Close button */}
        <TouchableOpacity
          onPress={hideWithAnimation}
          style={{
            width: 24,
            height: 24,
            borderRadius: 12,
            backgroundColor: 'rgba(255, 255, 255, 0.2)',
            alignItems: 'center',
            justifyContent: 'center',
          }}
        >
          <Typography
            style={{
              color: 'white',
              fontSize: 16,
              fontWeight: 'bold',
              lineHeight: 16,
            }}
          >
            ×
          </Typography>
        </TouchableOpacity>
      </TouchableOpacity>
    </Animated.View>
  );
};

export const ToastContainer: React.FC = () => {
  const toasts = useSelector((state: RootState) => state.toast.toasts);
  const dispatch = useDispatch();

  const handleHideToast = (id: string) => {
    dispatch(hideToast(id));
  };

  if (toasts.length === 0) {
    return null;
  }

  return (
    <View
      style={{
        position: 'absolute',
        bottom: 0,
        left: 0,
        right: 0,
        pointerEvents: 'box-none', // Allow touches to pass through empty areas
      }}
    >
      {toasts.map((toast, index) => (
        <ToastItem
          key={toast.id}
          toast={toast}
          index={index}
          onHide={handleHideToast}
        />
      ))}
    </View>
  );
};