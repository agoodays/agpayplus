<template>
  <a-card title="AgStateSwitch 状态切换组件示例" :bordered="false">
    <a-space direction="vertical" :size="24" style="width: 100%">
      
      <!-- 基础示例 -->
      <a-card title="1. 基础用法" size="small">
        <a-space :size="16">
          <div>
            <div style="margin-bottom: 8px">Badge 模式 - 启用：</div>
            <ag-state-switch :state="1" />
          </div>
          <div>
            <div style="margin-bottom: 8px">Badge 模式 - 停用：</div>
            <ag-state-switch :state="0" />
          </div>
          <div>
            <div style="margin-bottom: 8px">Badge 模式 - 未知：</div>
            <ag-state-switch :state="-1" />
          </div>
        </a-space>
      </a-card>

      <!-- Switch 模式 -->
      <a-card title="2. Switch 开关模式" size="small">
        <a-space :size="16" align="start">
          <div>
            <div style="margin-bottom: 8px">普通 Switch：</div>
            <ag-state-switch 
              :state="switchState1" 
              show-switch 
              :on-change="handleSwitch1Change"
            />
          </div>
          <div>
            <div style="margin-bottom: 8px">带文本的 Switch：</div>
            <ag-state-switch 
              :state="switchState2" 
              show-switch
              checked-text="开"
              unchecked-text="关"
              :on-change="handleSwitch2Change"
            />
          </div>
          <div>
            <div style="margin-bottom: 8px">禁用的 Switch：</div>
            <ag-state-switch 
              :state="1" 
              show-switch 
              disabled
            />
          </div>
        </a-space>
      </a-card>

      <!-- 自定义文本 -->
      <a-card title="3. 自定义状态文本" size="small">
        <a-space :size="16">
          <div>
            <div style="margin-bottom: 8px">在线/离线：</div>
            <ag-state-switch 
              :state="1" 
              active-text="在线"
              inactive-text="离线"
              unknown-text="维护中"
            />
          </div>
          <div>
            <div style="margin-bottom: 8px">正常/异常：</div>
            <ag-state-switch 
              :state="0" 
              active-text="正常"
              inactive-text="异常"
            />
          </div>
          <div>
            <div style="margin-bottom: 8px">通过/拒绝：</div>
            <ag-state-switch 
              :state="1" 
              active-text="通过"
              inactive-text="拒绝"
            />
          </div>
        </a-space>
      </a-card>

      <!-- 表格中使用 -->
      <a-card title="4. 表格中使用" size="small">
        <ag-table
          :table-columns="tableColumns"
          :req-table-data-func="loadTableData"
          :page-size="5"
        >
          <!-- Badge 模式列 -->
          <template #badgeState="{ record }">
            <ag-state-switch :state="record.state" />
          </template>
          
          <!-- Switch 模式列 -->
          <template #switchState="{ record }">
            <ag-state-switch
              :state="record.state"
              show-switch
              :disabled="record.id === 1"
              :on-change="(newState) => handleTableStateChange(record, newState)"
            />
          </template>
        </ag-table>
      </a-card>

      <!-- 异步更新 -->
      <a-card title="5. 异步状态更新（模拟 API 调用）" size="small">
        <a-space :size="16" align="start">
          <div>
            <div style="margin-bottom: 8px">成功更新：</div>
            <ag-state-switch 
              :state="asyncState1" 
              show-switch
              :on-change="handleAsyncSuccess"
            />
            <div style="margin-top: 4px; font-size: 12px; color: #999">
              切换后延迟 1 秒更新
            </div>
          </div>
          <div>
            <div style="margin-bottom: 8px">更新失败（自动回滚）：</div>
            <ag-state-switch 
              :state="asyncState2" 
              show-switch
              :on-change="handleAsyncFail"
            />
            <div style="margin-top: 4px; font-size: 12px; color: #999">
              始终失败，会自动回滚
            </div>
          </div>
        </a-space>
      </a-card>

    </a-space>
  </a-card>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { AgStateSwitch, AgTable } from '@/components'

// Switch 状态
const switchState1 = ref(1)
const switchState2 = ref(0)

// 异步状态
const asyncState1 = ref(0)
const asyncState2 = ref(1)

// 表格列定义
const tableColumns = ref([
  { title: 'ID', key: 'id', dataIndex: 'id', width: 80 },
  { title: '用户名', key: 'username', dataIndex: 'username', width: 150 },
  { title: '邮箱', key: 'email', dataIndex: 'email', width: 200 },
  { 
    title: '状态（Badge）', 
    key: 'badgeState', 
    customRender: 'badgeState', 
    width: 130 
  },
  { 
    title: '状态（Switch）', 
    key: 'switchState', 
    customRender: 'switchState', 
    width: 130 
  }
])

// Switch 1 切换处理
function handleSwitch1Change(newState) {
  switchState1.value = newState
  message.success(`状态已切换为：${newState === 1 ? '启用' : '停用'}`)
  return Promise.resolve()
}

// Switch 2 切换处理
function handleSwitch2Change(newState) {
  switchState2.value = newState
  message.info(`开关已${newState === 1 ? '打开' : '关闭'}`)
  return Promise.resolve()
}

// 异步成功示例
async function handleAsyncSuccess(newState) {
  message.loading({ content: '正在更新状态...', key: 'async1', duration: 0 })
  
  // 模拟 API 调用
  await new Promise(resolve => setTimeout(resolve, 1000))
  
  asyncState1.value = newState
  message.success({ content: '状态更新成功！', key: 'async1' })
}

// 异步失败示例
async function handleAsyncFail(newState) {
  message.loading({ content: '正在更新状态...', key: 'async2', duration: 0 })
  
  // 模拟 API 调用失败
  await new Promise(resolve => setTimeout(resolve, 1000))
  
  message.error({ content: '状态更新失败！组件已自动回滚', key: 'async2' })
  throw new Error('更新失败')
}

// 表格数据加载
function loadTableData(params) {
  const mockData = [
    { id: 1, username: 'admin', email: 'admin@example.com', state: 1 },
    { id: 2, username: 'user1', email: 'user1@example.com', state: 1 },
    { id: 3, username: 'user2', email: 'user2@example.com', state: 0 },
    { id: 4, username: 'user3', email: 'user3@example.com', state: 1 },
    { id: 5, username: 'user4', email: 'user4@example.com', state: 0 }
  ]
  
  const { pageNumber = 1, pageSize = 10 } = params
  const start = (pageNumber - 1) * pageSize
  const records = mockData.slice(start, start + pageSize)
  
  return Promise.resolve({
    total: mockData.length,
    records
  })
}

// 表格状态切换
async function handleTableStateChange(record, newState) {
  try {
    // 模拟 API 调用
    await new Promise(resolve => setTimeout(resolve, 500))
    
    // 更新成功
    record.state = newState
    message.success(`用户 ${record.username} 已${newState === 1 ? '启用' : '停用'}`)
  } catch (error) {
    message.error('状态更新失败')
    throw error
  }
}
</script>

<style scoped>
:deep(.ant-card-head-title) {
  font-size: 16px;
  font-weight: 600;
}

:deep(.ant-card-body) {
  padding: 16px;
}
</style>
