import { resolve } from 'node:path'
import { createSharedAlias } from './resolve-shared-alias'

export function createSharedViteConfig({ appId, port, projectRoot, pluginFactories }) {
  const projectPath = (...parts) => resolve(projectRoot, ...parts)
  const { vue, compression, pwa, visualizer } = pluginFactories

  return {
    resolve: {
      symlinks: false,
      alias: [
        createSharedAlias({ projectRoot }),
        { find: '@', replacement: projectPath('src') }
      ],
      modules: [projectPath('node_modules'), 'node_modules'],
      dedupe: [
        'vue', 'vue-router', 'pinia', 'pinia-plugin-persistedstate',
        'ant-design-vue', '@ant-design/icons-vue', '@wangeditor/editor', '@wangeditor/editor-for-vue', '@amap/amap-jsapi-loader',
        'axios', 'crypto-js', 'dayjs', 'js-base64', 'lodash', 'nprogress', 'vue-i18n', 'colorpickers',
        'reconnectingwebsocket', 'viewerjs'
      ]
    },
    define: {
      __APP_ID__: JSON.stringify(appId)
    },
    server: {
      fs: { allow: [projectRoot, projectPath('..', 'shared')] },
      port,
      host: '0.0.0.0',
      open: true
    },
    optimizeDeps: {
      include: ['vue', 'vue-router', 'pinia', 'ant-design-vue', '@ant-design/icons-vue', 'echarts', '@wangeditor/editor', '@wangeditor/editor-for-vue', 'lodash', 'dayjs', 'axios'],
      exclude: [],
      force: true
    },
    plugins: [
      vue(),
      compression({
        verbose: true,
        disable: false,
        threshold: 10240,
        algorithm: 'gzip',
        ext: '.gz'
      }),
      compression({
        verbose: true,
        disable: false,
        threshold: 10240,
        algorithm: 'brotliCompress',
        ext: '.br'
      }),
      pwa({
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
          maximumFileSizeToCacheInBytes: 3 * 1024 * 1024,
          runtimeCaching: [
            {
              urlPattern: /^https:\/\/api\./i,
              handler: 'NetworkFirst',
              options: {
                cacheName: 'api-cache',
                expiration: {
                  maxEntries: 100,
                  maxAgeSeconds: 60 * 60 * 24
                },
                cacheableResponse: {
                  statuses: [0, 200]
                }
              }
            }
          ]
        }
      }),
      visualizer({
        open: false,
        gzipSize: true,
        brotliSize: true
      })
    ],
    css: {
      preprocessorOptions: {
        less: {
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
        semicolons: true
      },
      rollupOptions: {
        output: {
          manualChunks: {
            'vue-vendor': ['vue', 'vue-router', 'pinia', 'pinia-plugin-persistedstate'],
            'antd-vendor': ['ant-design-vue', '@ant-design/icons-vue'],
            'chart-vendor': ['echarts'],
            'editor-vendor': ['@wangeditor/editor', '@wangeditor/editor-for-vue'],
            'util-vendor': ['lodash', 'dayjs', 'axios']
          },
          compact: false,
          chunkFileNames: 'assets/[name]-[hash].js',
          entryFileNames: 'assets/[name]-[hash].js',
          assetFileNames: 'assets/[name]-[hash].[ext]'
        }
      },
      chunkSizeWarningLimit: 2600,
      cssCodeSplit: true,
      cacheDir: 'node_modules/.vite',
      cssMinify: 'esbuild',
      parallel: true,
      sourcemap: false
    }
  }
}