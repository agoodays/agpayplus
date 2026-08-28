<template>
  <div>
    <a-card :bordered="false">
      <ag-card
        ref="cardRef"
        :load-data="loadData"
        :span="agpayCard.span"
        :height="agpayCard.height"
        :name="agpayCard.name"
        :add-authority="agpayCard.addAuthority"
        @add="handleAdd"
      >
        <template #cardContentSlot="{ record }">
          <div :style="{'height': agpayCard.height + 'px'}" class="ag-card-content">
            <!-- 卡片自定义样式 -->
            <div class="ag-card-content-header" :style="{backgroundColor: record.bgColor, height: agpayCard.height/2 + 'px'}">
              <img v-if="record.icon" :src="record.icon" :style="{height: agpayCard.height/5 + 'px'}">
            </div>
            <div class="ag-card-content-body" :style="{height: (agpayCard.height/2 - 50) + 'px'}">
              <div class="title">
                {{ record.ifName }}
              </div>
            </div>
            <!-- 卡片底部操作栏 -->
            <div class="ag-card-ops">
              <a-tooltip placement="top" title="编辑">
                <a-button type="text" v-if="hasPermission('ENT_PC_IF_DEFINE_EDIT')"  @click="handleEdit(record.ifCode)">
                  <edit-outlined />
                </a-button>
              </a-tooltip>
              <a-tooltip placement="top" title="删除">
                <a-button type="text" danger @click="del(record.ifCode)">
                  <delete-outlined />
                </a-button>
              </a-tooltip>
            </div>
          </div>
        </template>
      </ag-card>
    </a-card>
    <!-- 新增页面组件  -->
    <add-or-edit v-model:open="addOrEditOpen" :if-code="currentIfCode" @success="handleSuccess" />
  </div>
</template>

<script setup>
/**
 * 支付接口定义列表页面组件
 * 功能：展示支付接口定义卡片列表，支持新增、编辑、删除操作
 */
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { AgCard } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { reactive, ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'

// 权限检查
const { hasPermission } = usePermission()

/**
 * 卡片组件引用
 */
const cardRef = ref(null)

/**
 * 新增编辑组件引用
 */
const addOrEditOpen = ref(false)
const currentIfCode = ref('')

/**
 * 卡片配置
 */
const agpayCard = reactive({
  name: '支付接口',
  height: 360,
  span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 },
  addAuthority: hasPermission('ENT_PC_IF_DEFINE_ADD')
})

/**
 * 请求支付接口定义数据
 * @returns {Promise<Object>} 支付接口定义列表
 */
const loadData = async () => {
  return await payConfigApi.queryIfDefineList()
}

/**
 * 刷新卡片列表
 */
const reloadCardList = () => {
  cardRef.value?.reload()
}

/**
 * 新增支付接口定义
 */
function handleAdd() {
  currentIfCode.value = null
  addOrEditOpen.value = true
}


/**
 * 编辑支付接口定义
 * @param {string|number} ifCode 接口编码
 */
function handleEdit(ifCode) {
  currentIfCode.value = ifCode
  addOrEditOpen.value = true
}

/**
 * 操作成功回调
 */
const handleSuccess = () => {
  reloadCardList()
}

/**
 * 删除支付接口定义
 * @param {string} ifCode - 接口编码
 */
const del = async (ifCode) => {
  const { infoBox } = await import('@/utils/info-box')
  infoBox.confirmDanger('确认删除？', '', async () => {
    try {
      await payConfigApi.delIfDefineById(ifCode)
      message.success('删除成功！')
      reloadCardList()
    } catch (error) {
      console.error('删除支付接口定义失败:', error)
    }
  })
}
</script>

<style lang="less" scoped>
.ag-card-content {
  width: 100%;
  position: relative;
  background-color: var(--base-bg-color);
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  transition: box-shadow 0.3s;

  &:hover {
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
  }
}

.ag-card-ops {
  width: 100%;
  height: 50px;
  background-color: var(--surface-subtle);
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  gap: 24px;
  border-top: 1px solid var(--border-color);
  position: absolute;
  bottom: 0;

  :deep(.ant-btn) {
    padding: 4px 8px;
    font-size: 16px;

    &:hover {
      color: var(--primary-color);
    }
  }
}

.ag-card-content-header {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
}

.ag-card-content-body {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 8px 0;
}

.title {
  font-size: 16px;
  font-weight: 700;
  color: var(--text-color);
  letter-spacing: 1px;
}
</style>