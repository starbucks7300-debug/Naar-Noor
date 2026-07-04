import React, { useState } from 'react';
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
import { useRouter, useLocalSearchParams } from 'expo-router';
import { useRegistration } from '@hooks/useRegistration';
import { getPasswordStrength, getPasswordStrengthLabel, getPasswordStrengthColor } from '@utils/passwordValidation';

export default function RegisterScreen() {
  const router = useRouter();
  const [form, setForm] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    phone: '',
  });

  const { register, isLoading, isSuccess, formErrors } = useRegistration();
  const passwordStrength = getPasswordStrength(form.password);
  const passwordStrengthLabel = getPasswordStrengthLabel(form.password);
  const passwordStrengthColor = getPasswordStrengthColor(form.password);

  const isFormValid =
    form.email &&
    form.password &&
    form.confirmPassword &&
    form.firstName &&
    form.lastName &&
    form.phone &&
    form.password === form.confirmPassword &&
    passwordStrength >= 4;

  const handleRegister = () => {
    register(form);
  };

  React.useEffect(() => {
    if (isSuccess) {
      router.replace('/(app)');
    }
  }, [isSuccess]);

  return (
    <KeyboardAvoidingView behavior={Platform.OS === 'ios' ? 'padding' : 'height'} style={{ flex: 1 }}>
      <ScrollView className="flex-1 bg-white">
        <View className="p-6 pt-12">
          <Text className="text-3xl font-bold mb-2">Create Account</Text>
          <Text className="text-gray-600 mb-8">Register to get started</Text>

          {formErrors.general && (
            <View className="bg-red-100 border border-red-400 rounded p-3 mb-4">
              <Text className="text-red-700">{formErrors.general}</Text>
            </View>
          )}

          {/* First Name */}
          <View className="mb-4">
            <Text className="text-sm font-medium text-gray-700 mb-2">First Name</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="John"
              value={form.firstName}
              onChangeText={(text) => setForm({ ...form, firstName: text })}
              editable={!isLoading}
            />
            {formErrors.firstName && <Text className="text-red-500 text-xs mt-1">{formErrors.firstName}</Text>}
          </View>

          {/* Last Name */}
          <View className="mb-4">
            <Text className="text-sm font-medium text-gray-700 mb-2">Last Name</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="Doe"
              value={form.lastName}
              onChangeText={(text) => setForm({ ...form, lastName: text })}
              editable={!isLoading}
            />
            {formErrors.lastName && <Text className="text-red-500 text-xs mt-1">{formErrors.lastName}</Text>}
          </View>

          {/* Email */}
          <View className="mb-4">
            <Text className="text-sm font-medium text-gray-700 mb-2">Email</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="john@example.com"
              keyboardType="email-address"
              value={form.email}
              onChangeText={(text) => setForm({ ...form, email: text })}
              editable={!isLoading}
              autoCapitalize="none"
            />
            {formErrors.email && <Text className="text-red-500 text-xs mt-1">{formErrors.email}</Text>}
          </View>

          {/* Phone */}
          <View className="mb-4">
            <Text className="text-sm font-medium text-gray-700 mb-2">Phone Number</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="+1 (202) 555-1234"
              keyboardType="phone-pad"
              value={form.phone}
              onChangeText={(text) => setForm({ ...form, phone: text })}
              editable={!isLoading}
            />
            {formErrors.phone && <Text className="text-red-500 text-xs mt-1">{formErrors.phone}</Text>}
          </View>

          {/* Password */}
          <View className="mb-4">
            <Text className="text-sm font-medium text-gray-700 mb-2">Password</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="Enter password"
              secureTextEntry
              value={form.password}
              onChangeText={(text) => setForm({ ...form, password: text })}
              editable={!isLoading}
            />

            {form.password && (
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

            {formErrors.password && <Text className="text-red-500 text-xs mt-1">{formErrors.password}</Text>}
          </View>

          {/* Confirm Password */}
          <View className="mb-6">
            <Text className="text-sm font-medium text-gray-700 mb-2">Confirm Password</Text>
            <TextInput
              className="border border-gray-300 rounded px-4 py-3"
              placeholder="Confirm password"
              secureTextEntry
              value={form.confirmPassword}
              onChangeText={(text) => setForm({ ...form, confirmPassword: text })}
              editable={!isLoading}
            />
            {form.confirmPassword && form.password !== form.confirmPassword && (
              <Text className="text-red-500 text-xs mt-1">Passwords do not match</Text>
            )}
            {formErrors.confirmPassword && <Text className="text-red-500 text-xs mt-1">{formErrors.confirmPassword}</Text>}
          </View>

          {/* Register Button */}
          <TouchableOpacity
            className="bg-blue-600 rounded py-3 mb-4"
            onPress={handleRegister}
            disabled={!isFormValid || isLoading}
            style={{ opacity: isFormValid && !isLoading ? 1 : 0.5 }}
          >
            {isLoading ? (
              <ActivityIndicator color="white" />
            ) : (
              <Text className="text-white font-bold text-center">Create Account</Text>
            )}
          </TouchableOpacity>

          {/* Login Link */}
          <View className="flex-row justify-center">
            <Text className="text-gray-600">Already have an account? </Text>
            <TouchableOpacity onPress={() => router.replace('/(auth)/login')}>
              <Text className="text-blue-600 font-bold">Login</Text>
            </TouchableOpacity>
          </View>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}
