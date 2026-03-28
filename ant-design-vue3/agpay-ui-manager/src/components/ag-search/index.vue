<template>
  <div class="ag-search">
    <a-form :model="state.model" @submit.prevent>
      <a-row :gutter="[16, 16]" class="search-row">
        <!-- 
        搜索项响应式配置：
        推荐使用: <a-col v-bind="$attrs.colSpan || { xs: 24, sm: 12, md: 8, lg: 6 }">
        或直接: <a-col :xs="24" :sm="12" :md="8" :lg="6">
      -->

        <!-- 基础搜索条件（始终显示） -->
        <slot name="base" :col-span="colSpan" />

        <!-- 高级搜索条件（可展开/收起） -->
        <template v-if="!collapsed || !collapsible">
          <slot name="advanced" :col-span="colSpan" />
        </template>

        <!-- 默认插槽（向后兼容） -->
        <template v-if="!$slots.base && !$slots.advanced">
          <slot :col-span="colSpan" />
        </template>

        <!-- 操作按钮 - 响应式布局 -->
        <a-col
          :xs="24"
          :sm="24"
          :md="collapsible ? 24 : 8"
          :lg="collapsible ? 8 : 6"
          :xl="collapsible ? 6 : 6"
          class="search-buttons"
        >
          <a-form-item>
            <a-space :size="8">
              <a-button type="primary" @click="onSearch">
                <search-outlined />
                查询
              </a-button>
              <a-button @click="onReset">
                <redo-outlined />
                重置
              </a-button>
              <a v-if="collapsible" class="collapse-link" @click="toggleCollapsed">
                {{ collapsed ? '展开' : '收起' }}
                <down-outlined v-if="collapsed" />
                <up-outlined v-else />
              </a>
            </a-space>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </div>
</template>

<script setup>
import { reactive, ref, watch } from 'vue'
import { SearchOutlined, RedoOutlined, DownOutlined, UpOutlined } from '@ant-design/icons-vue'
const props = defineProps({
  searchData: { type: Object, default: null },
  modelValue: { type: Object, default: () => ({}) },
  // 是否支持展开/收起
  collapsible: { type: Boolean, default: false },
  // 默认是否收起
  defaultCollapsed: { type: Boolean, default: true },
  // 响应式列配置
  colSpan: {
    type: Object,
    default: () => ({
      xs: 24, // 手机：1 列
      sm: 12, // 平板：2 列
      md: 8, // 小桌面：3 列
      lg: 6, // 桌面：4 列
      xl: 6 // 大屏：4 列
    })
  }
})

const emit = defineEmits(['update:modelValue', 'search', 'reset'])

const state = reactive({
  model: props.modelValue || {}
})

const collapsed = ref(props.defaultCollapsed)

// 暴露响应式配置给插槽使用
defineExpose({
  colSpan: props.colSpan
})

// 监听 modelValue 变化
watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      state.model = val
    }
  },
  { deep: true }
)

function onSearch() {
  emit('search', state.model)
  emit('update:modelValue', state.model)
}

function onReset() {
  // 重置为空对象，但保留所有 key
  const keys = Object.keys(state.model)
  keys.forEach((key) => {
    state.model[key] = undefined
  })
  emit('reset')
  emit('update:modelValue', state.model)
}

function toggleCollapsed() {
  collapsed.value = !collapsed.value
}
</script>

<style scoped></style>
