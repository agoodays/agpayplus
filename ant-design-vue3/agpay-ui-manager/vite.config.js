import { defineConfig } from 'vite'
import { resolve } from 'path';
import vue from '@vitejs/plugin-vue';
import customVariables from '/@/theme/custom-variables';

const pathResolve = (dir) => {
  return resolve(__dirname, '.', dir);
};

console.log(customVariables)

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
        modifyVars: customVariables,
        javascriptEnabled: true,
      },
    },
  },
})
