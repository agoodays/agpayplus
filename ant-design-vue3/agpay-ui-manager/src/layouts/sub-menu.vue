<template>
  <a-sub-menu :key="menuInfo.entId">
    <!-- 图标插槽 -->
    <template #icon>
      <component :is="getIconComponent(menuInfo.menuIcon || menuInfo.icon)" />
    </template>
    
    <!-- 标题插槽 -->
    <template #title>
      <span>{{ menuInfo.entName }}</span>
    </template>
    
    <!-- 子菜单内容 -->
    <template v-for="item in menuInfo.children" :key="item.entId">
      <!-- 没有子菜单 -->
      <a-menu-item v-if="!item.children || item.children.length === 0" :key="item.entId">
        <template #icon>
          <component :is="getIconComponent(item.menuIcon || item.icon)" />
        </template>
        <span>{{ item.entName }}</span>
      </a-menu-item>
      
      <!-- 递归渲染子菜单 -->
      <sub-menu v-else :menu-info="item" :key="item.entId" />
    </template>
  </a-sub-menu>
</template>

<script setup>
import * as antIcons from '@ant-design/icons-vue'

// ==================== Props ====================

defineProps({
  menuInfo: {
    type: Object,
    required: true
  }
})

// ==================== 图标动态加载 ====================

/**
 * 动态获取图标组件
 * @param {string} iconName - 图标名称（如 'HomeOutlined'）
 * @returns {Component} 图标组件
 */
function getIconComponent(iconName) {
  if (!iconName) return null
  return antIcons[iconName]
}
</script>


