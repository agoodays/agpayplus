<template>
  <div class="container-demo">
    <a-space direction="vertical" style="width: 100%" :size="24">
      <!-- AgDrawer 示例 -->
      <AgCard title="AgDrawer - 抽屉组件">
        <a-alert
          message="抽屉组件 - 支持按比例自动设置大小"
          description="支持固定宽度、百分比、预设尺寸（small/medium/large/xlarge）和自定义比例"
          type="info"
          show-icon
          class="mb-4"
        />

        <a-space wrap>
          <a-button type="primary" @click="basicDrawerVisible = true">
            基础抽屉（720px）
          </a-button>
          <a-button @click="detailDrawerVisible = true">
            详情抽屉（800px）
          </a-button>
          <a-button @click="formDrawerVisible = true">
            表单抽屉（720px）
          </a-button>
          
          <a-divider type="vertical" style="height: 2em" />
          <span style="color: #1890ff; font-weight: bold">预设尺寸：</span>
          
          <a-button @click="smallDrawerVisible = true">
            小尺寸（30%）
          </a-button>
          <a-button @click="mediumDrawerVisible = true">
            中等（50%）
          </a-button>
          <a-button @click="largeDrawerVisible = true">
            大尺寸（70%）
          </a-button>
          <a-button @click="xlargeDrawerVisible = true">
            超大（90%）
          </a-button>
          
          <a-divider type="vertical" style="height: 2em" />
          <span style="color: #52c41a; font-weight: bold">自定义比例：</span>
          
          <a-button @click="ratio40DrawerVisible = true">
            40% 宽度
          </a-button>
          <a-button @click="ratio60DrawerVisible = true">
            60% 宽度
          </a-button>
          <a-button @click="ratio80DrawerVisible = true">
            80% 宽度
          </a-button>
        </a-space>

        <!-- 基础抽屉 -->
        <AgDrawer
          v-model:visible="basicDrawerVisible"
          title="基础抽屉（固定宽度 720px）"
        >
          <a-alert
            message="固定宽度"
            description="使用 :width='720' 设置固定像素宽度"
            type="info"
            class="mb-3"
          />
          <p>这是一个基础的抽屉组件</p>
          <p>关闭时会自动销毁内容</p>
          <a-divider />
          <a-descriptions :column="1">
            <a-descriptions-item label="宽度类型">固定宽度</a-descriptions-item>
            <a-descriptions-item label="宽度值">720px</a-descriptions-item>
            <a-descriptions-item label="销毁策略">关闭时销毁</a-descriptions-item>
          </a-descriptions>
        </AgDrawer>

        <!-- 小尺寸抽屉 (30%) -->
        <AgDrawer
          v-model:visible="smallDrawerVisible"
          title="小尺寸抽屉（30% 屏幕宽度）"
          size="small"
        >
          <a-alert
            message="预设尺寸 - Small"
            description="使用 size='small' 设置为 30% 屏幕宽度"
            type="success"
            class="mb-3"
          />
          <p>适合展示简单的信息</p>
          <a-divider />
          <a-descriptions :column="1">
            <a-descriptions-item label="尺寸类型">预设尺寸</a-descriptions-item>
            <a-descriptions-item label="尺寸名称">small</a-descriptions-item>
            <a-descriptions-item label="屏幕占比">30%</a-descriptions-item>
            <a-descriptions-item label="适用场景">简单信息展示</a-descriptions-item>
          </a-descriptions>
        </AgDrawer>

        <!-- 中等尺寸抽屉 (50%) -->
        <AgDrawer
          v-model:visible="mediumDrawerVisible"
          title="中等尺寸抽屉（50% 屏幕宽度）"
          size="medium"
        >
          <a-alert
            message="预设尺寸 - Medium"
            description="使用 size='medium' 设置为 50% 屏幕宽度"
            type="success"
            class="mb-3"
          />
          <p>适合展示一般的详情信息</p>
          <a-divider />
          <a-descriptions :column="1">
            <a-descriptions-item label="尺寸类型">预设尺寸</a-descriptions-item>
            <a-descriptions-item label="尺寸名称">medium</a-descriptions-item>
            <a-descriptions-item label="屏幕占比">50%</a-descriptions-item>
            <a-descriptions-item label="适用场景">一般详情展示</a-descriptions-item>
          </a-descriptions>
        </AgDrawer>

        <!-- 大尺寸抽屉 (70%) -->
        <AgDrawer
          v-model:visible="largeDrawerVisible"
          title="大尺寸抽屉（70% 屏幕宽度）"
          size="large"
        >
          <a-alert
            message="预设尺寸 - Large"
            description="使用 size='large' 设置为 70% 屏幕宽度"
            type="success"
            class="mb-3"
          />
          <p>适合展示复杂的内容和大量信息</p>
          <a-divider />
          <a-descriptions :column="2" bordered>
            <a-descriptions-item label="尺寸类型">预设尺寸</a-descriptions-item>
            <a-descriptions-item label="尺寸名称">large</a-descriptions-item>
            <a-descriptions-item label="屏幕占比">70%</a-descriptions-item>
            <a-descriptions-item label="适用场景">复杂内容展示</a-descriptions-item>
            <a-descriptions-item label="优点" :span="2">
              自动适应不同屏幕尺寸，在大屏和小屏上都有良好的显示效果
            </a-descriptions-item>
          </a-descriptions>
        </AgDrawer>

        <!-- 超大尺寸抽屉 (90%) -->
        <AgDrawer
          v-model:visible="xlargeDrawerVisible"
          title="超大尺寸抽屉（90% 屏幕宽度）"
          size="xlarge"
        >
          <a-alert
            message="预设尺寸 - XLarge"
            description="使用 size='xlarge' 设置为 90% 屏幕宽度"
            type="success"
            class="mb-3"
          />
          <p>适合展示大量信息或宽表格</p>
          <a-divider />
          
          <!-- 示例表格 -->
          <a-table
            :dataSource="sampleData"
            :columns="sampleColumns"
            :pagination="false"
            size="small"
          />
          
          <a-divider />
          <a-descriptions :column="2" bordered>
            <a-descriptions-item label="尺寸类型">预设尺寸</a-descriptions-item>
            <a-descriptions-item label="尺寸名称">xlarge</a-descriptions-item>
            <a-descriptions-item label="屏幕占比">90%</a-descriptions-item>
            <a-descriptions-item label="适用场景">大量信息、表格展示</a-descriptions-item>
          </a-descriptions>
        </AgDrawer>

        <!-- 自定义比例 40% -->
        <AgDrawer
          v-model:visible="ratio40DrawerVisible"
          title="自定义比例 40%"
          :widthRatio="0.4"
        >
          <a-alert
            message="自定义比例"
            description="使用 :widthRatio='0.4' 设置为 40% 屏幕宽度"
            type="warning"
            class="mb-3"
          />
          <p>可以通过 widthRatio 属性设置任意比例（0-1 之间）</p>
          <a-divider />
          <a-descriptions :column="1">
            <a-descriptions-item label="设置方式">widthRatio</a-descriptions-item>
            <a-descriptions-item label="比例值">0.4</a-descriptions-item>
            <a-descriptions-item label="屏幕占比">40%</a-descriptions-item>
          </a-descriptions>
        </AgDrawer>

        <!-- 自定义比例 60% -->
        <AgDrawer
          v-model:visible="ratio60DrawerVisible"
          title="自定义比例 60%"
          :widthRatio="0.6"
        >
          <a-alert
            message="自定义比例"
            description="使用 :widthRatio='0.6' 设置为 60% 屏幕宽度"
            type="warning"
            class="mb-3"
          />
          <p>比预设尺寸更灵活，可以精确控制宽度比例</p>
          <a-divider />
          <a-descriptions :column="1">
            <a-descriptions-item label="设置方式">widthRatio</a-descriptions-item>
            <a-descriptions-item label="比例值">0.6</a-descriptions-item>
            <a-descriptions-item label="屏幕占比">60%</a-descriptions-item>
          </a-descriptions>
        </AgDrawer>

        <!-- 自定义比例 80% -->
        <AgDrawer
          v-model:visible="ratio80DrawerVisible"
          title="自定义比例 80%"
          :widthRatio="0.8"
        >
          <a-alert
            message="自定义比例"
            description="使用 :widthRatio='0.8' 设置为 80% 屏幕宽度"
            type="warning"
            class="mb-3"
          />
          <p>适合在大屏上展示更多内容</p>
          <a-divider />
          <a-descriptions :column="1">
            <a-descriptions-item label="设置方式">widthRatio</a-descriptions-item>
            <a-descriptions-item label="比例值">0.8</a-descriptions-item>
            <a-descriptions-item label="屏幕占比">80%</a-descriptions-item>
          </a-descriptions>
        </AgDrawer>

        <!-- 详情抽屉 -->
        <AgDrawer
          v-model:visible="detailDrawerVisible"
          title="订单详情"
          :width="800"
        >
          <a-spin :spinning="detailLoading">
            <a-descriptions
              title="订单信息"
              :column="2"
              bordered
            >
              <a-descriptions-item label="订单号">
                ORD202401001
              </a-descriptions-item>
              <a-descriptions-item label="订单金额">
                ?999.00
              </a-descriptions-item>
              <a-descriptions-item label="订单状态">
                <a-tag color="success">已支付</a-tag>
              </a-descriptions-item>
              <a-descriptions-item label="支付时间">
                2024-01-01 10:00:00
              </a-descriptions-item>
              <a-descriptions-item label="商户名称">
                示例商户
              </a-descriptions-item>
              <a-descriptions-item label="支付方式">
                微信支付
              </a-descriptions-item>
              <a-descriptions-item label="备注" :span="2">
                这是一笔测试订单
              </a-descriptions-item>
            </a-descriptions>
          </a-spin>
        </AgDrawer>

        <!-- 表单抽屉 -->
        <AgDrawer
          v-model:visible="formDrawerVisible"
          title="添加商户"
          :width="720"
          :showConfirm="true"
          :confirmLoading="drawerSubmitLoading"
          @confirm="handleDrawerSubmit"
        >
          <a-form
            ref="drawerFormRef"
            :model="drawerForm"
            :rules="drawerRules"
            :label-col="{ span: 6 }"
            :wrapper-col="{ span: 16 }"
          >
            <a-form-item label="商户名称" name="name">
              <a-input v-model:value="drawerForm.name" placeholder="请输入商户名称" />
            </a-form-item>

            <a-form-item label="商户编号" name="code">
              <a-input v-model:value="drawerForm.code" placeholder="请输入商户编号" style="text-transform: uppercase" />
            </a-form-item>

            <a-form-item label="联系人" name="contact">
              <a-input v-model:value="drawerForm.contact" placeholder="请输入联系人" />
            </a-form-item>

            <a-form-item label="联系电话" name="phone">
              <a-input v-model:value="drawerForm.phone" placeholder="请输入联系电话" />
            </a-form-item>

            <a-form-item label="商户类型" name="type">
              <a-select v-model:value="drawerForm.type" placeholder="请选择商户类型">
                <a-select-option value="1">企业商户</a-select-option>
                <a-select-option value="2">个体商户</a-select-option>
              </a-select>
            </a-form-item>

            <a-form-item label="地址" name="address">
              <a-textarea v-model:value="drawerForm.address" :rows="3" placeholder="请输入地址" />
            </a-form-item>
          </a-form>
        </AgDrawer>

        <!-- 大尺寸抽屉 -->
        <AgDrawer
          v-model:visible="largeDrawerVisible"
          title="大尺寸抽屉"
          :width="1200"
        >
          <p>这是一个宽度为 1200px 的大尺寸抽屉</p>
          <p>适合展示大量信息或复杂的内容</p>
        </AgDrawer>
      </AgCard>

      <!-- AgModal 示例 -->
      <AgCard title="AgModal - 模态框组件">
        <a-alert
          message="模态框组件"
          description="在页面中央弹出的对话框，适合确认操作、表单提交等场景"
          type="info"
          show-icon
          class="mb-4"
        />

        <a-space>
          <a-button type="primary" @click="basicModalVisible = true">
            基础模态框
          </a-button>
          <a-button @click="formModalVisible = true">
            表单模态框
          </a-button>
          <a-button danger @click="deleteModalVisible = true">
            删除确认
          </a-button>
          <a-button @click="customModalVisible = true">
            自定义按钮
          </a-button>
        </a-space>

        <!-- 基础模态框 -->
        <AgModal
          v-model:visible="basicModalVisible"
          title="基础模态框"
          @ok="handleBasicModalOk"
        >
          <p>这是一个基础的模态框组件</p>
          <p>点击确定或取消按钮可以关闭模态框</p>
          <a-divider />
          <a-alert
            message="提示"
            description="模态框关闭时会自动销毁内容"
            type="info"
          />
        </AgModal>

        <!-- 表单模态框 -->
        <AgModal
          v-model:visible="formModalVisible"
          title="添加用户"
          :width="600"
          :confirmLoading="modalSubmitLoading"
          @ok="handleModalSubmit"
        >
          <a-form
            ref="modalFormRef"
            :model="modalForm"
            :rules="modalRules"
            :label-col="{ span: 6 }"
            :wrapper-col="{ span: 16 }"
          >
            <a-form-item label="用户名" name="username">
              <a-input v-model:value="modalForm.username" placeholder="请输入用户名" />
            </a-form-item>

            <a-form-item label="真实姓名" name="realname">
              <a-input v-model:value="modalForm.realname" placeholder="请输入真实姓名" />
            </a-form-item>

            <a-form-item label="手机号" name="phone">
              <a-input v-model:value="modalForm.phone" placeholder="请输入手机号" />
            </a-form-item>

            <a-form-item label="邮箱" name="email">
              <a-input v-model:value="modalForm.email" placeholder="请输入邮箱" />
            </a-form-item>

            <a-form-item label="角色" name="role">
              <a-select v-model:value="modalForm.role" placeholder="请选择角色">
                <a-select-option value="admin">管理员</a-select-option>
                <a-select-option value="user">普通用户</a-select-option>
              </a-select>
            </a-form-item>
          </a-form>
        </AgModal>

        <!-- 删除确认框 -->
        <AgModal
          v-model:visible="deleteModalVisible"
          title="确认删除"
          :maskClosable="false"
          okText="确认删除"
          :confirmLoading="deleteLoading"
          @ok="handleDelete"
        >
          <a-alert
            message="删除警告"
            description="删除后数据将无法恢复，确定要删除吗？"
            type="warning"
            show-icon
            class="mb-3"
          />

          <a-checkbox v-model:checked="deleteConfirmed">
            我已了解风险，确认删除
          </a-checkbox>
        </AgModal>

        <!-- 自定义按钮模态框 -->
        <AgModal
          v-model:visible="customModalVisible"
          title="审核"
          :width="500"
        >
          <a-form layout="vertical">
            <a-form-item label="审核意见">
              <a-textarea v-model:value="auditOpinion" :rows="4" placeholder="请输入审核意见" />
            </a-form-item>
          </a-form>

          <template #footer>
            <a-space>
              <a-button @click="customModalVisible = false">取消</a-button>
              <a-button type="danger" @click="handleReject">驳回</a-button>
              <a-button type="primary" @click="handleApprove">通过</a-button>
            </a-space>
          </template>
        </AgModal>
      </AgCard>

      <!-- 实际应用示例 -->
      <AgCard title="实际应用示例 - 商户列表">
        <a-space class="mb-3">
          <a-button type="primary" @click="showAddMerchant">
            <PlusOutlined />
            添加商户
          </a-button>
        </a-space>

        <a-table
          :dataSource="merchants"
          :columns="merchantColumns"
          :pagination="false"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'status'">
              <a-tag :color="record.status === 1 ? 'success' : 'default'">
                {{ record.status === 1 ? '正常' : '禁用' }}
              </a-tag>
            </template>
            <template v-if="column.key === 'action'">
              <a-space>
                <a @click="showMerchantDetail(record)">详情</a>
                <a @click="showEditMerchant(record)">编辑</a>
                <a style="color: #ff4d4f" @click="showDeleteMerchant(record)">删除</a>
              </a-space>
            </template>
          </template>
        </a-table>
      </AgCard>
    </a-space>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import { PlusOutlined } from '@ant-design/icons-vue'
import { AgCard, AgDrawer, AgModal } from '/@/components'

// Drawer 相关
const basicDrawerVisible = ref(false)
const detailDrawerVisible = ref(false)
const formDrawerVisible = ref(false)
const smallDrawerVisible = ref(false)
const mediumDrawerVisible = ref(false)
const largeDrawerVisible = ref(false)
const xlargeDrawerVisible = ref(false)
const ratio40DrawerVisible = ref(false)
const ratio60DrawerVisible = ref(false)
const ratio80DrawerVisible = ref(false)
const detailLoading = ref(false)
const drawerSubmitLoading = ref(false)
const drawerFormRef = ref()

// 示例表格数据
const sampleData = ref([
  { id: 1, name: '商品A', price: 100, stock: 50, sales: 120 },
  { id: 2, name: '商品B', price: 200, stock: 30, sales: 80 },
  { id: 3, name: '商品C', price: 150, stock: 45, sales: 95 }
])

const sampleColumns = [
  { title: 'ID', dataIndex: 'id', width: 80 },
  { title: '商品名称', dataIndex: 'name' },
  { title: '价格', dataIndex: 'price' },
  { title: '库存', dataIndex: 'stock' },
  { title: '销量', dataIndex: 'sales' }
]

const drawerForm = reactive({
  name: '',
  code: '',
  contact: '',
  phone: '',
  type: undefined,
  address: ''
})

const drawerRules = {
  name: [{ required: true, message: '请输入商户名称' }],
  code: [{ required: true, message: '请输入商户编号' }],
  contact: [{ required: true, message: '请输入联系人' }],
  phone: [{ required: true, message: '请输入联系电话' }],
  type: [{ required: true, message: '请选择商户类型' }]
}

// Modal 相关
const basicModalVisible = ref(false)
const formModalVisible = ref(false)
const deleteModalVisible = ref(false)
const customModalVisible = ref(false)
const modalSubmitLoading = ref(false)
const deleteLoading = ref(false)
const deleteConfirmed = ref(false)
const auditOpinion = ref('')
const modalFormRef = ref()

const modalForm = reactive({
  username: '',
  realname: '',
  phone: '',
  email: '',
  role: undefined
})

const modalRules = {
  username: [{ required: true, message: '请输入用户名' }],
  realname: [{ required: true, message: '请输入真实姓名' }],
  phone: [
    { required: true, message: '请输入手机号' },
    { pattern: /^1\d{10}$/, message: '手机号格式不正确' }
  ],
  email: [
    { required: true, message: '请输入邮箱' },
    { type: 'email', message: '邮箱格式不正确' }
  ],
  role: [{ required: true, message: '请选择角色' }]
}

// 商户列表
const merchants = ref([
  {
    id: 1,
    name: '示例商户A',
    code: 'MCH001',
    contact: '张三',
    phone: '138-0000-0001',
    status: 1
  },
  {
    id: 2,
    name: '示例商户B',
    code: 'MCH002',
    contact: '李四',
    phone: '138-0000-0002',
    status: 1
  },
  {
    id: 3,
    name: '示例商户C',
    code: 'MCH003',
    contact: '王五',
    phone: '138-0000-0003',
    status: 0
  }
])

const merchantColumns = [
  { title: '商户名称', dataIndex: 'name' },
  { title: '商户编号', dataIndex: 'code' },
  { title: '联系人', dataIndex: 'contact' },
  { title: '联系电话', dataIndex: 'phone' },
  { title: '状态', key: 'status' },
  { title: '操作', key: 'action', width: 180 }
]

// Drawer 方法
async function handleDrawerSubmit() {
  try {
    await drawerFormRef.value.validate()

    drawerSubmitLoading.value = true

    // 模拟 API 调用
    await new Promise(resolve => setTimeout(resolve, 1000))

    console.log('提交表单:', drawerForm)
    message.success('添加成功')
    formDrawerVisible.value = false

    // 重置表单
    Object.assign(drawerForm, {
      name: '',
      code: '',
      contact: '',
      phone: '',
      type: undefined,
      address: ''
    })
  } catch (error) {
    if (!error.errorFields) {
      message.error('添加失败')
    }
  } finally {
    drawerSubmitLoading.value = false
  }
}

// Modal 方法
function handleBasicModalOk() {
  message.info('点击了确定按钮')
  basicModalVisible.value = false
}

async function handleModalSubmit() {
  try {
    await modalFormRef.value.validate()

    modalSubmitLoading.value = true

    // 模拟 API 调用
    await new Promise(resolve => setTimeout(resolve, 1000))

    console.log('提交用户:', modalForm)
    message.success('添加成功')
    formModalVisible.value = false

    // 重置表单
    Object.assign(modalForm, {
      username: '',
      realname: '',
      phone: '',
      email: '',
      role: undefined
    })
  } catch (error) {
    if (!error.errorFields) {
      message.error('添加失败')
    }
  } finally {
    modalSubmitLoading.value = false
  }
}

async function handleDelete() {
  if (!deleteConfirmed.value) {
    message.warning('请确认删除操作')
    return
  }

  deleteLoading.value = true

  try {
    // 模拟 API 调用
    await new Promise(resolve => setTimeout(resolve, 1000))

    message.success('删除成功')
    deleteModalVisible.value = false
    deleteConfirmed.value = false
  } catch (error) {
    message.error('删除失败')
  } finally {
    deleteLoading.value = false
  }
}

async function handleApprove() {
  if (!auditOpinion.value) {
    message.warning('请输入审核意见')
    return
  }

  try {
    // 模拟 API 调用
    await new Promise(resolve => setTimeout(resolve, 500))

    message.success('审核通过')
    customModalVisible.value = false
    auditOpinion.value = ''
  } catch (error) {
    message.error('操作失败')
  }
}

async function handleReject() {
  if (!auditOpinion.value) {
    message.warning('请输入驳回原因')
    return
  }

  try {
    // 模拟 API 调用
    await new Promise(resolve => setTimeout(resolve, 500))

    message.success('已驳回')
    customModalVisible.value = false
    auditOpinion.value = ''
  } catch (error) {
    message.error('操作失败')
  }
}

// 商户操作
function showAddMerchant() {
  Object.assign(drawerForm, {
    name: '',
    code: '',
    contact: '',
    phone: '',
    type: undefined,
    address: ''
  })
  formDrawerVisible.value = true
}

function showMerchantDetail(record) {
  detailDrawerVisible.value = true
  console.log('查看商户详情:', record)
}

function showEditMerchant(record) {
  Object.assign(drawerForm, record)
  formDrawerVisible.value = true
}

function showDeleteMerchant(record) {
  deleteConfirmed.value = false
  deleteModalVisible.value = true
  console.log('删除商户:', record)
}
</script>

<style scoped>
.container-demo {
  /* padding: 24px */
}

.mb-3 {
  margin-bottom: 12px
}

.mb-4 {
  margin-bottom: 16px
}
</style>
