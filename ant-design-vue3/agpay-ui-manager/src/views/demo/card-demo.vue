<template>
  <div class="card-demo-container">
    <a-row :gutter="[16, 16]">
      <!-- 基础卡片 -->
      <a-col :span="12">
        <a-card title="基础卡片">
          <p>这是一个基础的卡片组件示例。</p>
          <p>卡片可以包含任意内容。</p>
        </a-card>
      </a-col>

      <!-- 带操作的卡片 -->
      <a-col :span="12">
        <a-card title="带操作的卡片">
          <template #extra>
            <a-space>
              <a-button size="small" @click="handleEdit">编辑</a-button>
              <a-button size="small" danger @click="handleDelete">删除</a-button>
            </a-space>
          </template>

          <p>这个卡片右上角有操作按钮。</p>
          <p>可以通过 extra 插槽自定义操作区域。</p>
        </a-card>
      </a-col>

      <!-- 无标题卡片 -->
      <a-col :span="12">
        <a-card>
          <p><strong>无标题卡片</strong></p>
          <p>这是一个没有标题的卡片。</p>
          <p>内容区域可以完全自定义。</p>
        </a-card>
      </a-col>

      <!-- 带边框的卡片 -->
      <a-col :span="12">
        <a-card title="带边框的卡片" :bordered="true">
          <p>这是一个带边框的卡片。</p>
          <p>通过 bordered 属性控制是否显示边框。</p>
        </a-card>
      </a-col>

      <!-- 信息展示卡片 -->
      <a-col :span="24">
        <a-card title="商户信息">
          <template #extra>
            <a-tag color="green">已认证</a-tag>
          </template>

          <a-descriptions :column="2" bordered>
            <a-descriptions-item label="商户名称"> 示例商户有限公司 </a-descriptions-item>
            <a-descriptions-item label="商户编号"> MCH202401001 </a-descriptions-item>
            <a-descriptions-item label="商户类型"> 企业商户 </a-descriptions-item>
            <a-descriptions-item label="注册时间"> 2024-01-01 10:00:00 </a-descriptions-item>
            <a-descriptions-item label="联系电话"> 138-0000-0000 </a-descriptions-item>
            <a-descriptions-item label="邮箱"> merchant@example.com </a-descriptions-item>
            <a-descriptions-item label="地址" :span="2"> 北京市朝阳区示例路123号 </a-descriptions-item>
          </a-descriptions>
        </a-card>
      </a-col>

      <!-- 统计卡片 -->
      <a-col :span="6">
        <a-card>
          <a-statistic title="今日交易" :value="1234" suffix="笔" :value-style="{ color: '#3f8600' }">
            <template #prefix>
              <ArrowUpOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :span="6">
        <a-card>
          <a-statistic title="交易金额" :value="98765.43" :precision="2" prefix="¥" :value-style="{ color: '#cf1322' }">
            <template #prefix>
              <ArrowDownOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>

      <a-col :span="6">
        <a-card>
          <a-statistic title="活跃用户" :value="8765" suffix="人" />
        </a-card>
      </a-col>

      <a-col :span="6">
        <a-card>
          <a-statistic title="成功率" :value="98.5" suffix="%" :value-style="{ color: '#3f8600' }" />
        </a-card>
      </a-col>

      <!-- 嵌套卡片 -->
      <a-col :span="24">
        <a-card title="嵌套卡片示例">
          <a-row :gutter="16">
            <a-col :span="8">
              <a-card title="子卡片 1" size="small">
                <p>这是嵌套在父卡片内的子卡片。</p>
              </a-card>
            </a-col>
            <a-col :span="8">
              <a-card title="子卡片 2" size="small">
                <p>可以在卡片内嵌套多个子卡片。</p>
              </a-card>
            </a-col>
            <a-col :span="8">
              <a-card title="子卡片 3" size="small">
                <p>适合展示分组信息。</p>
              </a-card>
            </a-col>
          </a-row>
        </a-card>
      </a-col>
    </a-row>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { ArrowUpOutlined, ArrowDownOutlined } from '@ant-design/icons-vue'
import { AgCard } from '@/components'

function handleEdit() {
  message.info('点击了编辑按钮')
}

function handleDelete() {
  message.warning('点击了删除按钮')
}
</script>

<style scoped>
.card-demo-container {
  /* padding: 24px */
}

.mt-2 {
  margin-top: 8px;
}

h4 {
  margin-bottom: 8px;
  color: var(--primary-color);
}
</style>
