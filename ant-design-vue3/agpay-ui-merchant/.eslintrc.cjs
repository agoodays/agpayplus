module.exports = {
  root: true,
  env: {
    browser: true,
    es2021: true,
    node: true
  },
  extends: [
    'eslint:recommended',
    'plugin:vue/vue3-recommended',
    'plugin:prettier/recommended'
  ],
  parserOptions: {
    ecmaVersion: 'latest',
    sourceType: 'module'
  },
  plugins: ['vue', 'prettier'],
  rules: {
    'vue/multi-word-component-names': 'off',
    'vue/no-v-html': 'off',
    'no-shadow': 'warn',
    'no-restricted-imports': [
      'warn',
      {
        paths: [
          {
            name: 'vue',
            importNames: [
              'defineProps',
              'defineEmits',
              'defineExpose',
              'withDefaults',
              'defineOptions',
              'defineSlots'
            ],
            message: 'script setup 宏不应从 vue 导入，请直接使用编译宏。'
          }
        ]
      }
    ],
    'no-console': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'no-debugger': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'prettier/prettier': [
      'error',
      {
        endOfLine: 'auto'
      }
    ]
  },
  globals: {
    defineProps: 'readonly',
    defineEmits: 'readonly',
    defineExpose: 'readonly',
    withDefaults: 'readonly'
  },
  overrides: [
    {
      files: ['src/views/**/*.vue'],
      rules: {
        'no-restricted-imports': [
          'warn',
          {
            paths: [
              {
                name: 'vue',
                importNames: [
                  'defineProps',
                  'defineEmits',
                  'defineExpose',
                  'withDefaults',
                  'defineOptions',
                  'defineSlots'
                ],
                message: 'script setup 宏不应从 vue 导入，请直接使用编译宏。'
              }
            ],
            patterns: []
          }
        ]
      }
    }
  ]
}
