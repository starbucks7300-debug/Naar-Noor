/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
    "./app/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#C65A1E',
          50:  '#fdf4ef',
          100: '#fae4d0',
          200: '#f4c9a1',
          300: '#eda872',
          400: '#e58043',
          500: '#C65A1E',
          600: '#a84918',
          700: '#8a3912',
          800: '#6c2a0c',
          900: '#4e1c06',
        },
        brand: {
          background: '#0a0a0a',
          surface:    '#111111',
          border:     '#1e1e1e',
          text:       '#f5efe8',
          muted:      '#a89880',
        },
      },
      fontFamily: {
        sans:    ['Open Sans', 'System'],
        display: ['Forum', 'serif'],
      },
    },
  },
  plugins: [],
};
