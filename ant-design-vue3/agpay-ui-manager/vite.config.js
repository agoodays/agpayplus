import { defineConfig, loadEnv } from 'vite'
import { resolve } from 'path'
import vue from '@vitejs/plugin-vue'
import viteCompression from 'vite-plugin-compression'
import { VitePWA } from 'vite-plugin-pwa'
import { visualizer } from 'rollup-plugin-visualizer'

const pathResolve = (dir) => {
  return resolve(__dirname, '.', dir)
}

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd())
  
  return {
    resolve: {
      alias: [
        // 绝对路径重命名：@/xxxx => src/xxxx
        {
          find: '@',
          replacement: pathResolve('src')
        }
      ]
    },
    server: {
      port: 8817,
      host: '0.0.0.0',
      open: true,
      // proxy: {
      //   '/api': {
      //     target: env.VITE_APP_API_BASE_URL,
      //     changeOrigin: true,
      //     ws: true,
      //     rewrite: (path) => path.replace(/^\/api/, '')
      //   }
      // }
    },
    plugins: [
      vue(),
      // Gzip 压缩
      viteCompression({
        verbose: true,
        disable: false,
        threshold: 10240,
        algorithm: 'gzip',
        ext: '.gz'
      }),
      // Brotli 压缩
      viteCompression({
        verbose: true,
        disable: false,
        threshold: 10240,
        algorithm: 'brotliCompress',
        ext: '.br'
      }),
      // PWA 应用
      VitePWA({
        registerType: 'autoUpdate',
        manifest: {
          name: 'AgPay Manager',
          short_name: 'AgPay',
          theme_color: '#1890ff',
          icons: [
            {
              src: '/logo.png',
              sizes: '192x192',
              type: 'image/png'
            }
          ]
        }
      }),
      // 包分析
      visualizer({
        open: false,
        gzipSize: true,
        brotliSize: true
      })
    ],
    css: {
      preprocessorOptions: {
        less: {
          // 运行时使用 CSS 变量覆盖样式
          modifyVars: {},
          javascriptEnabled: true
        }
      }
    },
    build: {
      target: 'es2015',
      outDir: 'dist',
      assetsDir: 'assets',
      minify: 'terser',
      terserOptions: {
        compress: {
          drop_console: true,
          drop_debugger: true
        }
      },
      rollupOptions: {
        output: {
          // 分包策略
          manualChunks: {
            'vue-vendor': ['vue', 'vue-router', 'pinia'],
            'antd-vendor': ['ant-design-vue', '@ant-design/icons-vue'],
            'chart-vendor': ['echarts'],
            'editor-vendor': ['@wangeditor/editor', '@wangeditor/editor-for-vue']
          }
        }
      },
      chunkSizeWarningLimit: 2000
    }
  }
})
