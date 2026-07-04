import React, { useState, useEffect } from 'react';
import {
  View,
  ScrollView,
  TextInput,
  TouchableOpacity,
  Text,
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';
import { useRouter } from 'expo-router';
import { useLogin } from '@hooks/useLogin';

export default function LoginScreen() {
  const router = useRouter();
  const [form, setForm] = useState({ email: '', password: '' });
  const { login, isLoading, isSuccess, formErrors, isLockedOut, remainingAttempts, lockoutRemainingTime } = useLogin();
  const [lockoutCountdown, setLockoutCountdown] = useState(0);

  const isFormValid = form.email && form.password && !isLockedOut;

  const handleLogin = () => {
    login(form);
  };

  useEffect(() => {
    if (isSuccess) {
      router.replace('/(app)');
    }
  }, [isSuccess]);

  useEffect(() => {
    if (isLockedOut && lockoutRemainingTime > 0) {
      setLockoutCountdown(lockoutRemainingTime);
      const timer = setInterval(() => {
        setLockoutCountdown((prev) => (prev > 0 ? prev - 1 : 0));
      }, 1000);
      return () => clearInterval(timer);
    }
  }, [isLockedOut, lockoutRemainingTime]);

  return (
    <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : 'height'} style={{ flex: 1 }}>
      <ScrollView className="flex-1 bg-white">
        <View className="p-6 pt-20">
          <Text className="text-3xl font-bold mb-2">Welcome Back</Text>
          <Text className="text-gray-600 mb-8">Login to your account</Text>

          {formErrors.general && (
            <View className="bg-red-100 border border-red-400 rounded p-3 mb-4">
              <Text className="text-red-700 text-sm">{formErrors.general}</Text>
              {!isLockedOut && remainingAttempts > 0 && (
                <Text className="text-red-600 text-xs mt-1">{remainingAttempts} attempts remaining</Text>
              )}
            </View>
          )}

          {isLockedOut && (
            <View className="bg-yellow-100 border border-yellow-400 rounded p-3 mb-4">
              <Text className="text-yellow-800 font-bold">Account Temporarily Locked</Text>
              <Text className="text-yellow-800 text-sm mt-1">Try again in {lockoutCountdown} seconds</Text>
              <TouchableOpacity onPress={() => router.push('/(auth)/forgot-password')} className="mt-2">
                <Text className="text-blue-600 text-sm font-bold">Reset Password</Text>
              </TouchableOpacity>
            </View>
          )}

          {/* Email */}
          <View className="mb-4">
            <Text className="text-sm font-medium text-gray-700 mb-2">Email</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="john@example.com"
              keyboardType="email-address"
              value={form.email}
              onChangeText={(text) => setForm({ ...form, email: text })}
              editable={!isLoading && !isLockedOut}
              autoCapitalize="none"
            />
            {formErrors.email && <Text className="text-red-500 text-xs mt-1">{formErrors.email}</Text>}
          </View>

          {/* Password */}
          <View className="mb-2">
            <Text className="text-sm font-medium text-gray-700 mb-2">Password</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="Enter password"
              secureTextEntry
              value={form.password}
              onChangeText={(text) => setForm({ ...form, password: text })}
              editable={!isLoading && !isLockedOut}
            />
            {formErrors.password && <Text className="text-red-500 text-xs mt-1">{formErrors.password}</Text>}
          </View>

          {/* Forgot Password Link */}
          <TouchableOpacity
            className="mb-6"
            onPress={() => router.push('/(auth)/forgot-password')}
            disabled={isLoading || isLockedOut}
          >
            <Text className="text-blue-600 text-sm font-bold text-right">Forgot Password?</Text>
          </TouchableOpacity>

          {/* Login Button */}
          <TouchableOpacity
            className="bg-blue-600 rounded py-3 mb-4"
            onPress={handleLogin}
            disabled={!isFormValid || isLoading}
            style={{ opacity: isFormValid && !isLoading ? 1 : 0.5 }}
          >
            {isLoading ? (
              <ActivityIndicator color="white" />
            ) : (
              <Text className="text-white font-bold text-center">Login</Text>
            )}
          </TouchableOpacity>

          {/* Register Link */}
          <View className="flex-row justify-center">
            <Text className="text-gray-600">Don't have an account? </Text>
            <TouchableOpacity onPress={() => router.replace('/(auth)/register')} disabled={isLoading || isLockedOut}>
              <Text className="text-blue-600 font-bold">Register</Text>
            </TouchableOpacity>
          </View>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}
