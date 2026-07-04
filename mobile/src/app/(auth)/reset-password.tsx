import React, { useState } from 'react';
import { View, ScrollView, TextInput, TouchableOpacity, Text, ActivityIndicator, KeyboardAvoidingView, Platform } from 'react-native';
import { useRouter, useLocalSearchParams } from 'expo-router';
import { useMutation } from '@tanstack/react-query';
import { confirmPasswordReset } from '@services/api/authApi';
import { validatePassword, getPasswordStrength, getPasswordStrengthLabel, getPasswordStrengthColor } from '@utils/passwordValidation';
import { logger } from '@services/logger';

export default function ResetPasswordScreen() {
  const router = useRouter();
  const { token } = useLocalSearchParams<{ token: string }>();
  
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errors, setErrors] = useState<{ password?: string; confirmPassword?: string; general?: string }>({});
  const [success, setSuccess] = useState(false);

  const passwordStrength = getPasswordStrength(password);
  const passwordStrengthLabel = getPasswordStrengthLabel(password);
  const passwordStrengthColor = getPasswordStrengthColor(password);

  const mutation = useMutation({
    mutationFn: async (data: { token: string; newPassword: string }) => {
      return await confirmPasswordReset(data);
    },
    onSuccess: () => {
      logger.info('Password reset successful');
      setSuccess(true);
      setErrors({});
    },
    onError: (error: any) => {
      const errorMessage = error.response?.data?.message || 'Failed to reset password. Please try again.';
      logger.error('Password reset confirmation failed', error as Error);
      setErrors({ general: errorMessage });
    },
  });

  const validateForm = (): boolean => {
    const formErrors: typeof errors = {};

    const passwordValidation = validatePassword(password);
    if (!passwordValidation.valid) {
      formErrors.password = passwordValidation.errors[0];
    }

    if (password !== confirmPassword) {
      formErrors.confirmPassword = 'Passwords do not match';
    }

    setErrors(formErrors);
    return Object.keys(formErrors).length === 0;
  };

  const handleResetPassword = () => {
    if (!token) {
      setErrors({ general: 'Invalid reset link' });
      return;
    }

    if (!validateForm()) return;

    mutation.mutate({ token, newPassword: password });
  };

  if (success) {
    return (
      <View className="flex-1 bg-white justify-center items-center p-6">
        <View className="bg-green-100 border border-green-400 rounded-lg p-6 mb-4">
          <Text className="text-green-800 text-lg font-bold mb-2">Password Reset Successful</Text>
          <Text className="text-green-700">Your password has been reset. Please log in with your new password.</Text>
        </View>

        <TouchableOpacity
          className="bg-blue-600 rounded py-3 px-6 w-full"
          onPress={() => router.replace('/(auth)/login')}
        >
          <Text className="text-white font-bold text-center">Back to Login</Text>
        </TouchableOpacity>
      </View>
    );
  }

  return (
    <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : 'height'} style={{ flex: 1 }}>
      <ScrollView className="flex-1 bg-white">
        <View className="p-6 pt-12">
          <Text className="text-3xl font-bold mb-2">Reset Password</Text>
          <Text className="text-gray-600 mb-8">Enter your new password</Text>

          {errors.general && (
            <View className="bg-red-100 border border-red-400 rounded p-3 mb-4">
              <Text className="text-red-700 text-sm">{errors.general}</Text>
            </View>
          )}

          <View className="mb-4">
            <Text className="text-sm font-medium text-gray-700 mb-2">New Password</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="Enter new password"
              secureTextEntry
              value={password}
              onChangeText={setPassword}
              editable={!mutation.isPending}
            />

            {password && (
              <View className="mt-2">
                <View className="flex-row items-center mb-2">
                  <View
                    className="h-2 flex-1 rounded-full mr-2"
                    style={{ backgroundColor: passwordStrengthColor }}
                  />
                  <Text className="text-xs font-medium" style={{ color: passwordStrengthColor }}>
                    {passwordStrengthLabel}
                  </Text>
                </View>
              </View>
            )}

            {errors.password && <Text className="text-red-500 text-xs mt-1">{errors.password}</Text>}
          </View>

          <View className="mb-6">
            <Text className="text-sm font-medium text-gray-700 mb-2">Confirm Password</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="Confirm new password"
              secureTextEntry
              value={confirmPassword}
              onChangeText={setConfirmPassword}
              editable={!mutation.isPending}
            />
            {confirmPassword && password !== confirmPassword && (
              <Text className="text-red-500 text-xs mt-1">Passwords do not match</Text>
            )}
            {errors.confirmPassword && <Text className="text-red-500 text-xs mt-1">{errors.confirmPassword}</Text>}
          </View>

          <TouchableOpacity
            className="bg-blue-600 rounded py-3 mb-4"
            onPress={handleResetPassword}
            disabled={!password || !confirmPassword || mutation.isPending}
            style={{ opacity: password && confirmPassword && !mutation.isPending ? 1 : 0.5 }}
          >
            {mutation.isPending ? (
              <ActivityIndicator color="white" />
            ) : (
              <Text className="text-white font-bold text-center">Reset Password</Text>
            )}
          </TouchableOpacity>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}
