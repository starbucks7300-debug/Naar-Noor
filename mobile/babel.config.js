module.exports = {
  presets: ['babel-preset-expo'],
  plugins: [
    '@babel/plugin-transform-export-namespace-from',
    'nativewind/babel',
  ],
  env: {
    test: {
      presets: [
        ['@babel/preset-env', { targets: { node: 'current' } }],
        '@babel/preset-react',
        ['@babel/preset-typescript', { onlyRemoveTypeImports: true }],
      ],
      plugins: [
        '@babel/plugin-transform-export-namespace-from',
      ],
    },
  },
};
