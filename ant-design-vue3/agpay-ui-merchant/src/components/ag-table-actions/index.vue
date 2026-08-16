<template>
  <div class="ag-table-action-columns">
    <template v-if="!hasOverflow">
      <!-- 操作按钮数量不超过阈值：直接全部显示 -->
      <slot></slot>
    </template>
    <template v-else>
      <!-- 操作按钮数量超过阈值：显示前 maxShowNum-1 个，其余放入"更多"菜单 -->
      <template v-for="(action, index) in frontActions" :key="index">
        <component :is="action" />
      </template>
      <a-dropdown>
        <a-button type="link" size="small">
          <EllipsisOutlined :style="{ fontSize: '20px', verticalAlign: 'top' }" />
        </a-button>
        <template #overlay>
          <a-menu>
            <template v-for="(action, index) in moreActions" :key="index">
              <a-menu-item>
                <component :is="action" />
              </a-menu-item>
            </template>
          </a-menu>
        </template>
      </a-dropdown>
    </template>
  </div>
</template>

<script setup>
/**
 * AgTableActions - 表格操作按钮容器组件
 *
 * 控制表格行内操作按钮的显示数量，超出部分折叠到"更多"下拉菜单中，
 * 避免操作列过宽影响表格布局。
 *
 * 实现原理：
 * 1. 通过 useSlots 获取默认插槽中的所有子节点
 * 2. 过滤掉注释节点和纯文本节点，只保留有效的 VNode
 * 3. 当有效节点数量超过 maxShowNum 时：
 *    - 直接显示前 maxShowNum - 1 个节点
 *    - 剩余节点放入 a-dropdown 下拉菜单中
 * 4. 当有效节点数量不超过 maxShowNum 时，直接显示所有节点
 *
 * @example
 * <AgTableActions :max-show-num="3">
 *   <a-button type="link" @click="handleView">查看</a-button>
 *   <a-button type="link" @click="handleEdit">编辑</a-button>
 *   <a-button type="link" @click="handleDelete">删除</a-button>
 *   <a-button type="link" @click="handleExport">导出</a-button>
 * </AgTableActions>
 */
import { computed, useSlots } from 'vue'
import { EllipsisOutlined } from '@ant-design/icons-vue'

const props = defineProps({
  /** 最多直接显示几个操作按钮，超过的放入"更多"菜单 */
  maxShowNum: {
    type: Number,
    default: 2
  }
})

const slots = useSlots()

/** 获取所有有效的操作子节点（过滤注释和纯文本节点） */
const visibleActions = computed(() => {
  const defaultSlot = slots.default?.() || []
  return defaultSlot.filter((vnode) => {
    return vnode.type && typeof vnode.type !== 'symbol'
  })
})

/** 操作按钮数量是否超过阈值 */
const hasOverflow = computed(() => visibleActions.value.length > props.maxShowNum)

/** 直接显示的操作按钮（前 maxShowNum - 1 个） */
const frontActions = computed(() => {
  if (!hasOverflow.value) return []
  return visibleActions.value.slice(0, props.maxShowNum - 1)
})

/** 放入"更多"菜单的操作按钮 */
const moreActions = computed(() => {
  if (!hasOverflow.value) return []
  return visibleActions.value.slice(props.maxShowNum - 1)
})
</script>

<style scoped>
.ag-table-action-columns {
  align-items: center;
  justify-content: flex-start;
  gap: 4px;
  flex-wrap: nowrap;
  overflow: visible;
}

.ag-table-action-columns :deep(.ant-btn-link) {
  padding: 2px 4px;
  border: none !important;
  box-shadow: none !important;
  font-size: 12px;
}
</style>
