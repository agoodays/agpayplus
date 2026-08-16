import Viewer from 'viewerjs'
import 'viewerjs/dist/viewer.css'

/**
 * 图片预览工具函数
 * @param {Object} options - 配置选项
 * @param {string[]} options.images - 图片地址列表
 * @param {Object} options.options - Viewer.js 配置选项
 */
export const viewerApi = async (options) => {
  const { images = [], options: viewerOptions = {} } = options

  if (!images || images.length === 0) {
    return
  }

  if (typeof document === 'undefined' || !document.body) {
    console.error('viewerApi: document.body is not available')
    return
  }

  const container = document.createElement('div')
  container.style.position = 'fixed'
  container.style.top = '-9999px'
  container.style.left = '-9999px'
  document.body.appendChild(container)

  const loadedPromises = images.map((imgUrl, index) => {
    return new Promise((resolve, reject) => {
      const img = document.createElement('img')
      img.src = imgUrl
      img.dataset.src = imgUrl
      img.alt = `image-${index}`
      img.onload = () => resolve(img)
      img.onerror = () => reject(new Error(`图片加载失败: ${imgUrl}`))
      container.appendChild(img)
    })
  })

  try {
    await Promise.all(loadedPromises)

    const defaultOptions = {
      inline: false,
      button: true,
      navbar: true,
      title: true,
      toolbar: true,
      tooltip: true,
      movable: true,
      zoomable: true,
      rotatable: true,
      scalable: true,
      transition: true,
      fullscreen: true,
      keyboard: true,
      url: 'data-src',
      initialViewIndex: viewerOptions.initialViewIndex || 0,
      hidden: () => {
        container.remove()
      }
    }

    const mergedOptions = { ...defaultOptions, ...viewerOptions }

    try {
      const viewer = new Viewer(container, mergedOptions)
      viewer.show()
    } catch (error) {
      console.error('viewerApi: 创建 Viewer 失败:', error)
      container.remove()
    }
  } catch (error) {
    console.error('viewerApi: 图片加载失败:', error)
    container.remove()
  }
}

export default viewerApi
