module.exports = {
  root: true,
  env: {
    browser: true,
    es2021: true,
    node: true
  },
  extends: [
    'plugin:vue/vue3-recommended',
    'eslint:recommended'
  ],
  parserOptions: {
    ecmaVersion: 'latest',
    sourceType: 'module',
    parser: '@babel/eslint-parser',
    requireConfigFile: false
  },
  rules: {
    // ==================== 分号规则 ====================
    // 禁止使用分号
    'semi': ['error', 'never'],
    
    // 在特殊情况下需要分号时给出警告
    'semi-style': ['error', 'first'],
    
    // 禁止不必要的分号
    'no-extra-semi': 'error',
    
    // ==================== 引号规则 ====================
    // 使用单引号
    'quotes': ['error', 'single', { avoidEscape: true }],
    
    // ==================== 逗号规则 ====================
    // 不使用尾随逗号
    'comma-dangle': ['error', 'none'],
    
    // 逗号前后的空格
    'comma-spacing': ['error', { before: false, after: true }],
    
    // ==================== 箭头函数规则 ====================
    // 箭头函数参数只有一个时省略括号
    'arrow-parens': ['error', 'as-needed'],
    
    // 箭头函数体的大括号
    'arrow-body-style': ['off'],
    
    // ==================== 空格规则 ====================
    // 对象字面量的大括号内侧需要空格
    'object-curly-spacing': ['error', 'always'],
    
    // 数组括号内侧不需要空格
    'array-bracket-spacing': ['error', 'never'],
    
    // ==================== Vue 特定规则 ====================
    // 组件名称必须是多个单词
    'vue/multi-word-component-names': 'off',
    
    // HTML 缩进
    'vue/html-indent': ['error', 2],
    
    // 单行元素的内容前后需要换行
    'vue/singleline-html-element-content-newline': 'off',
    
    // 组件标签的自闭合
    'vue/html-self-closing': ['error', {
      html: {
        void: 'always',
        normal: 'never',
        component: 'always'
      }
    }],
    
    // ==================== 其他规则 ====================
    // 允许 console
    'no-console': 'off',
    
    // 允许 debugger（开发环境）
    'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'warn',
    
    // 未使用的变量
    'no-unused-vars': ['warn', { 
      argsIgnorePattern: '^_',
      varsIgnorePattern: '^_'
    }]
  }
}
