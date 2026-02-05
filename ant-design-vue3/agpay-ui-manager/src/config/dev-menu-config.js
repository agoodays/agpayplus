/**
 * 开发模式菜单配置
 * 用于开发环境下的菜单和路由配置
 */

/**
 * 开发模式菜单树配置
 * 
 * 配置说明：
 * - entId: 菜单唯一标识（必填）
 * - entName: 菜单显示名称（必填）
 * - menuUri: 路由路径（菜单项必填，目录可选）
 * - componentName: 组件路径，相对于 views 目录（菜单项必填）
 * - entType: 类型，'MO' = 目录，'ML' = 菜单链接（必填）
 * - menuIcon: 菜单图标（可选）
 * - children: 子菜单（可选）
 */
export const devMenuTree = [
  {
    entId: 'ENT_DEMO',
    entName: '组件示例',
    menuUri: '',
    entType: 'MO',
    menuIcon: 'ExperimentOutlined',
    children: [
      {
        entId: 'ENT_DEMO_INDEX',
        entName: '组件总览',
        menuUri: '/demo/index',
        componentName: 'demo/index',
        entType: 'ML',
        menuIcon: 'HomeOutlined'
      },
      {
        entId: 'ENT_DEMO_SEARCH_TABLE',
        entName: '搜索表格',
        menuUri: '/demo/search-table-demo',
        componentName: 'demo/search-table-demo',
        entType: 'ML',
        menuIcon: 'TableOutlined'
      },
      {
        entId: 'ENT_DEMO_STATE_SWITCH',
        entName: '状态切换',
        menuUri: '/demo/state-switch-demo',
        componentName: 'demo/state-switch-demo',
        entType: 'ML',
        menuIcon: 'ControlOutlined'
      },
      {
        entId: 'ENT_DEMO_FORM',
        entName: '表单组件',
        menuUri: '/demo/form-demo',
        componentName: 'demo/form-demo',
        entType: 'ML',
        menuIcon: 'FormOutlined'
      },
      {
        entId: 'ENT_DEMO_FLOAT_INPUT',
        entName: '浮动标签组件',
        menuUri: '/demo/float-label-demo',
        componentName: 'demo/float-label-demo',
        entType: 'ML',
        menuIcon: 'FontSizeOutlined'
      },
      {
        entId: 'ENT_DEMO_SELECT_INFINITE',
        entName: '分页下拉选择',
        menuUri: '/demo/select-infinite-demo',
        componentName: 'demo/select-infinite-demo',
        entType: 'ML',
        menuIcon: 'SelectOutlined'
      },
      {
        entId: 'ENT_DEMO_CARD',
        entName: '卡片组件',
        menuUri: '/demo/card-demo',
        componentName: 'demo/card-demo',
        entType: 'ML',
        menuIcon: 'CreditCardOutlined'
      },
      {
        entId: 'ENT_DEMO_UPLOAD',
        entName: '文件上传',
        menuUri: '/demo/upload-demo',
        componentName: 'demo/upload-demo',
        entType: 'ML',
        menuIcon: 'UploadOutlined'
      },
      {
        entId: 'ENT_DEMO_EDITOR',
        entName: '富文本编辑',
        menuUri: '/demo/editor-demo',
        componentName: 'demo/editor-demo',
        entType: 'ML',
        menuIcon: 'EditOutlined'
      },
      {
        entId: 'ENT_DEMO_CONTAINER',
        entName: '容器组件',
        menuUri: '/demo/container-demo',
        componentName: 'demo/container-demo',
        entType: 'ML',
        menuIcon: 'ContainerOutlined'
      }
    ]
  }
]

/**
 * 开发模式用户信息
 */
export const devUserInfo = {
  sysUserId: 'dev-user-001',
  realname: '开发者',
  loginUsername: 'developer',
  avatarUrl: '',
  telphone: '13800138000',
  sex: 1,
  state: 1,
  isAdmin: true,
  sysType: 'MGR',
  belongInfoId: 'dev-info-001',
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString()
}

/**
 * 添加新的示例页面
 * 
 * @example
 * // 1. 在 src/views/demo/ 目录创建组件
 * // 2. 在下面的数组中添加菜单配置
 * {
 *   entId: 'ENT_DEMO_MY_NEW',
 *   entName: '我的新示例',
 *   menuUri: '/main/demo-my-new',
 *   componentName: 'demo/my-new-demo',
 *   entType: 'ML',
 *   menuIcon: 'StarOutlined'
 * }
 */
