import React, { useState } from 'react';
import { View, ScrollView, TextInput, TouchableOpacity, Text, ActivityIndicator, KeyboardAvoidingView, Platform, Alert } from 'react-native';
import { useRouter } from 'expo-router';
import { useMutation } from '@tanstack/react-query';
import { requestPasswordReset } from '@services/api/authApi';
import { validateEmail } from '@utils/emailValidation';
import { logger } from '@services/logger';

export default function ForgotPasswordScreen() {
  const router = useRouter();
  const [email, setEmail] = useState('');
  const [errors, setErrors] = useState<{ email?: string; general?: string }>({});
  const [submitted, setSubmitted] = useState(false);

  const mutation = useMutation({
    mutationFn: async (email: string) => {
      return await requestPasswordReset({ email });
    },
    onSuccess: () => {
      logger.info('Password reset email sent', { email });
      setSubmitted(true);
      setErrors({});
    },
    onError: (error: any) => {
      const errorMessage = error.response?.data?.message || 'Failed to send reset email. Please try again.';
      logger.error('Password reset request failed', error as Error);
      setErrors({ general: errorMessage });
    },
  });

  const validateForm = (): boolean => {
    const formErrors: { email?: string } = {};
    const emailValidation = validateEmail(email);

    if (!emailValidation.valid) {
      formErrors.email = emailValidation.errors[0];
    }

    setErrors(formErrors);
    return Object.keys(formErrors).length === 0;
  };

  const handleRequestReset = () => {
    if (!validateForm()) return;
    mutation.mutate(email);
  };

  if (submitted) {
    return (
      <View className="flex-1 bg-white justify-center items-center p-6">
        <View className="bg-green-100 border border-green-400 rounded-lg p-6 mb-4">
          <Text className="text-green-800 text-lg font-bold mb-2">Check Your Email</Text>
          <Text className="text-green-700 mb-4">We've sent a password reset link to {email}</Text>
          <Text className="text-green-700 text-sm mb-4">The link will expire in 1 hour.</Text>
        </View>

        <TouchableOpacity
          className="bg-blue-600 rounded py-3 px-6 mb-3 w-full"
          onPress={() => router.replace('/(auth)/login')}
        >
          <Text className="text-white font-bold text-center">Back to Login</Text>
        </TouchableOpacity>

        <TouchableOpacity
          className="border border-blue-600 rounded py-3 px-6 w-full"
          onPress={() => {
            setSubmitted(false);
            setEmail('');
          }}
        >
          <Text className="text-blue-600 font-bold text-center">Try Another Email</Text>
        </TouchableOpacity>
      </View>
    );
  }

  return (
    <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : 'height'} style={{ flex: 1 }}>
      <ScrollView className="flex-1 bg-white">
        <View className="p-6 pt-12">
          <Text className="text-3xl font-bold mb-2">Reset Password</Text>
          <Text className="text-gray-600 mb-8">Enter your email to receive a password reset link</Text>

          {errors.general && (
            <View className="bg-red-100 border border-red-400 rounded p-3 mb-4">
              <Text className="text-red-700 text-sm">{errors.general}</Text>
            </View>
          )}

          <View className="mb-6">
            <Text className="text-sm font-medium text-gray-700 mb-2">Email Address</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="john@example.com"
              keyboardType="email-address"
              value={email}
              onChangeText={setEmail}
              editable={!mutation.isPending}
              autoCapitalize="none"
            />
            {errors.email && <Text className="text-red-500 text-xs mt-1">{errors.email}</Text>}
          </View>

          <TouchableOpacity
            className="bg-blue-600 rounded py-3 mb-4"
            onPress={handleRequestReset}
            disabled={!email || mutation.isPending}
            style={{ opacity: email && !mutation.isPending ? 1 : 0.5 }}
          >
            {mutation.isPending ? (
              <ActivityIndicator color="white" />
            ) : (
              <Text className="text-white font-bold text-center">Send Reset Link</Text>
            )}
          </TouchableOpacity>

          <View className="flex-row justify-center">
            <Text className="text-gray-600">Remember your password? </Text>
            <TouchableOpacity onPress={() => router.replace('/(auth)/login')}>
              <Text className="text-blue-600 font-bold">Login</Text>
            </TouchableOpacity>
          </View>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}
