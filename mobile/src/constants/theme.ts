import '@/global.css';
import { Platform } from 'react-native';

export const BrandColors = {
  primary:          '#C65A1E',
  primaryLight:     '#D97240',
  primaryDark:      '#A04A18',
  primaryGlow:      'rgba(198, 90, 30, 0.25)',
  danger:           '#ef4444',
  dangerMuted:      'rgba(239, 68, 68, 0.12)',
  success:          '#10b981',
  successMuted:     'rgba(16, 185, 129, 0.12)',
} as const;

export const Colors = {
  light: {
    background:          '#f5efe8',
    backgroundSecondary: '#ede6dc',
    backgroundElement:   '#e3dbd0',
    backgroundSelected:  '#d8cfc4',
    surface:             '#ede6dc',
    text:                '#1a1208',
    textSecondary:       '#8c7d68',
    textMuted:           '#a0937d',
    border:              'rgba(0, 0, 0, 0.11)',
    borderStrong:        'rgba(0, 0, 0, 0.22)',
    inputBackground:     'rgba(0, 0, 0, 0.05)',
    ...BrandColors,
  },
  dark: {
    background:          '#0a0a0a',
    backgroundSecondary: '#0d0d0d',
    backgroundElement:   '#111111',
    backgroundSelected:  '#1a1a1a',
    surface:             '#111111',
    text:                '#ffffff',
    textSecondary:       '#737373',
    textMuted:           '#525252',
    border:              'rgba(255, 255, 255, 0.10)',
    borderStrong:        'rgba(255, 255, 255, 0.25)',
    inputBackground:     'rgba(255, 255, 255, 0.05)',
    ...BrandColors,
  },
} as const;

export type ThemeColor = keyof typeof Colors.light & keyof typeof Colors.dark;

export const Fonts = Platform.select({
  ios: {
    display: 'Forum',
    sans:    'Open Sans',
    mono:    'ui-monospace',
  },
  android: {
    display: 'Forum',
    sans:    'OpenSans-Regular',
    mono:    'monospace',
  },
  default: {
    display: 'Forum',
    sans:    'Open Sans',
    mono:    'monospace',
  },
  web: {
    display: 'var(--font-display)',
    sans:    'var(--font-sans)',
    mono:    'var(--font-mono)',
  },
});

export const FontSizes = {
  xs:   10,
  sm:   12,
  base: 14,
  md:   16,
  lg:   18,
  xl:   20,
  '2xl': 24,
  '3xl': 30,
  '4xl': 36,
  '5xl': 48,
} as const;

export const FontWeights = {
  light:    '300',
  normal:   '400',
  medium:   '500',
  semibold: '600',
  bold:     '700',
} as const;

export const Spacing = {
  half:  2,
  one:   4,
  two:   8,
  three: 16,
  four:  24,
  five:  32,
  six:   64,
} as const;

export const Radii = {
  sm:   6,
  md:   12,
  lg:   16,
  xl:   20,
  full: 9999,
} as const;

export const Shadows = {
  card: {
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.12,
    shadowRadius: 16,
    elevation: 4,
  },
  primary: {
    shadowColor: '#C65A1E',
    shadowOffset: { width: 0, height: 0 },
    shadowOpacity: 0.4,
    shadowRadius: 12,
    elevation: 6,
  },
} as const;

export const BottomTabInset = Platform.select({ ios: 50, android: 80 }) ?? 0;
export const MaxContentWidth = 800;
