import vue from '@vitejs/plugin-vue'
import { visualizer } from 'rollup-plugin-visualizer'
import { defineConfig } from 'vite'
import viteCompression from 'vite-plugin-compression'
import { VitePWA } from 'vite-plugin-pwa'
import { createSharedViteConfig } from '../shared/config/create-vite-config'

export default defineConfig(createSharedViteConfig({
  appId: 'manager',
  port: 8817,
  projectRoot: __dirname,
  pluginFactories: {
    vue,
    compression: viteCompression,
    pwa: VitePWA,
    visualizer
  }
}))
