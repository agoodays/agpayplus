<template>
  <div class="demo-index-container">
    <a-card class="welcome-card mb-4">
      <template #title>
        <div class="welcome-title">
          <ExperimentOutlined class="title-icon" />
          <span>AgPay 组件库演示</span>
        </div>
      </template>
      
      <a-alert
        message="欢迎使用 AgPay 组件库"
        description="这里展示了所有自定义组件的使用示例，点击左侧菜单查看具体组件演示"
        type="success"
        show-icon
        class="mb-4"
      />

      <a-descriptions title="项目信息" :column="2" bordered>
        <a-descriptions-item label="Vue 版本">
          Vue 3.3+
        </a-descriptions-item>
        <a-descriptions-item label="UI 框架">
          Ant Design Vue 4.x
        </a-descriptions-item>
        <a-descriptions-item label="构建工具">
          Vite 4.x
        </a-descriptions-item>
        <a-descriptions-item label="状态管理">
          Pinia
        </a-descriptions-item>
        <a-descriptions-item label="组件数量">
          20 个
        </a-descriptions-item>
        <a-descriptions-item label="Demo 数量">
          10 个
        </a-descriptions-item>
      </a-descriptions>
    </a-card>

    <a-row :gutter="[16, 16]">
      <!-- 浮动标签表单组件 -->
      <a-col :span="12">
        <AgCard title="浮动标签表单组件" hoverable>
          <template #extra>
            <a-tag color="blue">8 个</a-tag>
          </template>
          
          <a-list size="small" :data-source="floatLabelComponents">
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #avatar>
                    <a-avatar :style="{ backgroundColor: '#1890ff' }">
                      <component :is="item.icon" />
                    </a-avatar>
                  </template>
                  <template #title>
                    <a @click="goToDemo(item.path)">{{ item.name }}</a>
                  </template>
                  <template #description>
                    {{ item.description }}
                  </template>
                </a-list-item-meta>
              </a-list-item>
            </template>
          </a-list>
        </AgCard>
      </a-col>

      <!-- 表格相关组件 -->
      <a-col :span="12">
        <AgCard title="表格相关组件" hoverable>
          <template #extra>
            <a-tag color="green">6 个</a-tag>
          </template>
          
          <a-list size="small" :data-source="tableComponents">
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #avatar>
                    <a-avatar :style="{ backgroundColor: '#52c41a' }">
                      <component :is="item.icon" />
                    </a-avatar>
                  </template>
                  <template #title>
                    <a @click="goToDemo(item.path)">{{ item.name }}</a>
                  </template>
                  <template #description>
                    {{ item.description }}
                  </template>
                </a-list-item-meta>
              </a-list-item>
            </template>
          </a-list>
        </AgCard>
      </a-col>

      <!-- 表单相关组件 -->
      <a-col :span="12">
        <AgCard title="表单相关组件" hoverable>
          <template #extra>
            <a-tag color="orange">3 个</a-tag>
          </template>
          
          <a-list size="small" :data-source="formComponents">
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #avatar>
                    <a-avatar :style="{ backgroundColor: '#fa8c16' }">
                      <component :is="item.icon" />
                    </a-avatar>
                  </template>
                  <template #title>
                    <a @click="goToDemo(item.path)">{{ item.name }}</a>
                  </template>
                  <template #description>
                    {{ item.description }}
                  </template>
                </a-list-item-meta>
              </a-list-item>
            </template>
          </a-list>
        </AgCard>
      </a-col>

      <!-- 容器组件 -->
      <a-col :span="12">
        <AgCard title="容器组件" hoverable>
          <template #extra>
            <a-tag color="purple">3 个</a-tag>
          </template>
          
          <a-list size="small" :data-source="containerComponents">
            <template #renderItem="{ item }">
              <a-list-item>
                <a-list-item-meta>
                  <template #avatar>
                    <a-avatar :style="{ backgroundColor: '#722ed1' }">
                      <component :is="item.icon" />
                    </a-avatar>
                  </template>
                  <template #title>
                    <a @click="goToDemo(item.path)">{{ item.name }}</a>
                  </template>
                  <template #description>
                    {{ item.description }}
                  </template>
                </a-list-item-meta>
              </a-list-item>
            </template>
          </a-list>
        </AgCard>
      </a-col>

      <!-- 快速开始 -->
      <a-col :span="24">
        <AgCard title="快速开始" hoverable>
          <template #extra>
            <a-tag color="cyan">教程</a-tag>
          </template>
          
          <a-row :gutter="[32, 16]">
            <a-col :span="12">
              <a-steps direction="vertical" size="small" :current="0">
                <a-step title="查看示例" description="点击上方组件名称查看各组件示例" />
                <a-step title="阅读文档" description="查看 src/components/[组件名]/README.md" />
                <a-step title="复制代码" description="复制示例代码到你的项目中" />
                <a-step title="开始开发" description="根据需求调整组件参数" />
              </a-steps>
            </a-col>
            <a-col :span="12">
              <a-alert
                message="💡 推荐阅读"
                type="info"
                show-icon
              >
                <template #description>
                  <ul style="margin: 8px 0; padding-left: 20px;">
                    <li><a href="#" @click.prevent="openDoc('COMPONENTS_OVERVIEW.md')">组件库总览</a></li>
                    <li><a href="#" @click.prevent="openDoc('QUICK_START.md')">快速开始指南</a></li>
                    <li><a href="#" @click.prevent="openDoc('FLOAT_COMPONENTS_README.md')">浮动标签完整指南</a></li>
                    <li><a href="#" @click.prevent="openDoc('AG_SEARCH_SLOT_PROPS_SOLUTION.md')">AgSearch 简化方案</a></li>
                  </ul>
                </template>
              </a-alert>
            </a-col>
          </a-row>
        </AgCard>
      </a-col>
    </a-row>

    <!-- 统计信息 -->
    <a-row :gutter="16" class="mt-4">
      <a-col :span="6">
        <a-card>
          <a-statistic
            title="组件数量"
            :value="20"
            suffix="个"
            :value-style="{ color: '#3f8600' }"
          >
            <template #prefix>
              <AppstoreOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      
      <a-col :span="6">
        <a-card>
          <a-statistic
            title="Demo 数量"
            :value="10"
            suffix="个"
            :value-style="{ color: '#1890ff' }"
          >
            <template #prefix>
              <FileTextOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      
      <a-col :span="6">
        <a-card>
          <a-statistic
            title="文档数量"
            :value="11"
            suffix="份"
            :value-style="{ color: '#cf1322' }"
          >
            <template #prefix>
              <CodeOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
      
      <a-col :span="6">
        <a-card>
          <a-statistic
            title="完成度"
            :value="100"
            suffix="%"
            :value-style="{ color: '#52c41a' }"
          >
            <template #prefix>
              <CheckCircleOutlined />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import {
  ExperimentOutlined,
  TableOutlined,
  SearchOutlined,
  FormOutlined,
  UploadOutlined,
  CalendarOutlined,
  EditOutlined,
  ControlOutlined,
  CreditCardOutlined,
  FontSizeOutlined,
  LoadingOutlined,
  AppstoreOutlined,
  FileTextOutlined,
  CodeOutlined,
  CheckCircleOutlined,
  FieldNumberOutlined,
  FontColorsOutlined,
  SelectOutlined,
  FileMarkdownOutlined,
  BarsOutlined,
  LayoutOutlined,
  BorderOutlined
} from '@ant-design/icons-vue'
import { AgCard } from '/@/components'

const router = useRouter()

// 浮动标签表单组件
const floatLabelComponents = [
  {
    name: 'AgInput',
    description: '文本输入框，支持浮动标签',
    icon: 'FormOutlined',
    path: '/demo/float-label-demo'
  },
  {
    name: 'AgInputNumber',
    description: '数字输入框，支持范围和精度',
    icon: 'FieldNumberOutlined',
    path: '/demo/float-label-demo'
  },
  {
    name: 'AgInputNumberRange',
    description: '数字范围输入，最小-最大值',
    icon: 'BarsOutlined',
    path: '/demo/float-label-demo'
  },
  {
    name: 'AgTextarea',
    description: '多行文本域，支持字数统计',
    icon: 'FileMarkdownOutlined',
    path: '/demo/float-label-demo'
  },
  {
    name: 'AgSelect',
    description: '下拉选择框，支持异步加载',
    icon: 'SelectOutlined',
    path: '/demo/float-label-demo'
  },
  {
    name: 'AgSelectInfinite',
    description: '分页下拉选择，支持触底加载',
    icon: 'SelectOutlined',
    path: '/demo/select-infinite-demo'
  },
  {
    name: 'AgDateRangePicker',
    description: '日期范围选择器',
    icon: 'CalendarOutlined',
    path: '/demo/float-label-demo'
  }
]

// 表格相关组件
const tableComponents = [
  {
    name: 'AgTable',
    description: '高级数据表格，支持分页、排序、筛选',
    icon: 'TableOutlined',
    path: '/demo/search-table-demo'
  },
  {
    name: 'AgSearch',
    description: '搜索表单，支持响应式布局',
    icon: 'SearchOutlined',
    path: '/demo/search-table-demo'
  },
  {
    name: 'AgTableAction',
    description: '行操作按钮，支持下拉菜单',
    icon: 'TableOutlined',
    path: '/demo/search-table-demo'
  },
  {
    name: 'AgTableActions',
    description: '列操作组件，统一的操作列',
    icon: 'TableOutlined',
    path: '/demo/search-table-demo'
  }
]

// 表单相关组件
const formComponents = [
  {
    name: 'AgUpload',
    description: '文件上传组件，支持拖拽上传',
    icon: 'UploadOutlined',
    path: '/demo/upload-demo'
  },
  {
    name: 'AgEditor',
    description: '富文本编辑器，基于 wangEditor',
    icon: 'EditOutlined',
    path: '/demo/editor-demo'
  },
  {
    name: 'AgStateSwitch',
    description: '状态开关，支持 API 集成',
    icon: 'ControlOutlined',
    path: '/demo/state-switch-demo'
  }
]

// 容器组件
const containerComponents = [
  {
    name: 'AgCard',
    description: '卡片容器，支持标题、操作区',
    icon: 'CreditCardOutlined',
    path: '/demo/card-demo'
  },
  {
    name: 'AgDrawer',
    description: '抽屉容器，支持多种配置',
    icon: 'LayoutOutlined',
    path: '/demo/container-demo'
  },
  {
    name: 'AgModal',
    description: '模态框，支持自定义内容',
    icon: 'BorderOutlined',
    path: '/demo/container-demo'
  }
]

function goToDemo(path) {
  router.push(path)
}

function openDoc(docName) {
  message.info(`请查看项目根目录下的 ${docName} 文件`)
}
</script>

<style scoped>
.demo-index-container {
  /* padding: 24px */
}

.welcome-card {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1)
}

.welcome-title {
  display: flex;
  align-items: center;
  font-size: 18px
}

.title-icon {
  margin-right: 8px;
  font-size: 24px;
  color: #1890ff
}

.mb-4 {
  margin-bottom: 16px
}

.mt-4 {
  margin-top: 16px
}
</style>
