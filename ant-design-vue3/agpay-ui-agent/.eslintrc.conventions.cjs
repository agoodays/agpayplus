module.exports = {
  root: true,
  env: {
    browser: true,
    es2021: true,
    node: true
  },
  parser: 'vue-eslint-parser',
  parserOptions: {
    parser: 'espree',
    ecmaVersion: 'latest',
    sourceType: 'module'
  },
  plugins: ['vue'],
  rules: {
    'no-shadow': 'warn',
    'no-unused-vars': 'off',
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
            patterns: [
              {
                group: ['@/api/manage', '@/api/manage*'],
                message: '页面层禁止直连 manage，请改用 src/api/business 领域 API。'
              }
            ]
          }
        ]
      }
    }
  ]
}
