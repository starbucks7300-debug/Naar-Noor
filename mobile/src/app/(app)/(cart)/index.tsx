import { View, Text } from 'react-native';
import { useTranslation } from 'react-i18next';

export default function CartScreen() {
  const { t } = useTranslation();

  return (
    <View className="flex-1 items-center justify-center bg-white">
      <Text className="text-2xl font-bold">{t('cart.empty')}</Text>
      <Text className="text-base text-gray-600 mt-2">Coming soon...</Text>
    </View>
  );
}
