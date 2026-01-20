import { defineConfig } from 'vite'
import { resolve } from 'path';
import vue from '@vitejs/plugin-vue';
// custom-variables removed: theme will be applied at runtime via CSS variables

const pathResolve = (dir) => {
  return resolve(__dirname, '.', dir);
};

// https://vitejs.dev/config/
export default defineConfig({
  resolve: {
    alias: [
      // 绝对路径重命名：/@/xxxx => src/xxxx
      {
        find: /\/@\//,
        replacement: pathResolve('src') + '/',
      },
    ]
  },
  server: {
    port: 8817
  },
  plugins: [vue()],
  css: {
    preprocessorOptions: {
      less: {
        // 不再在构建时通过 less.modifyVars 覆盖主题；运行时使用 CSS 变量覆盖样式
        modifyVars: {},
        javascriptEnabled: true,
      },
    },
  },
})
