import { defineConfig, loadEnv } from 'vite'
import { resolve } from 'path'
import vue from '@vitejs/plugin-vue'
import viteCompression from 'vite-plugin-compression'
import { visualizer } from 'rollup-plugin-visualizer'
import { VitePWA } from 'vite-plugin-pwa'

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
      open: true
      // proxy: {
      //   '/api': {
      //     target: env.VITE_APP_API_BASE_URL,
      //     changeOrigin: true,
      //     ws: true,
      //     rewrite: (path) => path.replace(/^\/api/, '')
      //   }
      // }
    },
    // 依赖预构建配置
    optimizeDeps: {
      include: ['vue', 'vue-router', 'pinia', 'ant-design-vue', '@ant-design/icons-vue', 'echarts', '@wangeditor/editor', '@wangeditor/editor-for-vue', 'lodash', 'dayjs', 'axios'],
      exclude: [],
      // 强制预构建所有依赖
      force: true
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
      // PWA 支持
      VitePWA({
        registerType: 'autoUpdate',
        includeAssets: ['favicon.ico', 'robots.txt', 'imgs/*'],
        manifest: {
          name: 'AgPay Plus',
          short_name: 'AgPay',
          description: 'AgPay Plus 管理系统',
          theme_color: '#1890ff',
          icons: [
            {
              src: 'imgs/logo.png',
              sizes: '192x192',
              type: 'image/png'
            },
            {
              src: 'imgs/logo.png',
              sizes: '512x512',
              type: 'image/png'
            }
          ]
        },
        workbox: {
          globPatterns: ['**/*.{js,css,html,ico,png,svg}'],
          runtimeCaching: [
            {
              urlPattern: /^https:\/\/api\./i,
              handler: 'NetworkFirst',
              options: {
                cacheName: 'api-cache',
                expiration: {
                  maxEntries: 100,
                  maxAgeSeconds: 60 * 60 * 24 // 1 day
                },
                cacheableResponse: {
                  statuses: [0, 200]
                }
              }
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
      minify: 'esbuild',
      esbuildOptions: {
        compress: true,
        drop: ['console', 'debugger'],
        // 禁用分号删除，可能导致语法错误
        semicolons: true
      },
      rollupOptions: {
        output: {
          // 分包策略
          manualChunks: {
            'vue-vendor': ['vue', 'vue-router', 'pinia', 'pinia-plugin-persistedstate'],
            'antd-vendor': ['ant-design-vue', '@ant-design/icons-vue'],
            'chart-vendor': ['echarts'],
            'editor-vendor': ['@wangeditor/editor', '@wangeditor/editor-for-vue'],
            'util-vendor': ['lodash', 'dayjs', 'axios']
          },
          // 优化输出
          compact: false,
          chunkFileNames: 'assets/[name]-[hash].js',
          entryFileNames: 'assets/[name]-[hash].js',
          assetFileNames: 'assets/[name]-[hash].[ext]'
        }
      },
      chunkSizeWarningLimit: 1000,
      cssCodeSplit: true,
      cacheDir: 'node_modules/.vite',
      cssMinify: 'esbuild',
      parallel: true,
      sourcemap: false
    }
  }
})
