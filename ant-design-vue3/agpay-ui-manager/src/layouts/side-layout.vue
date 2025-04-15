<template>
  <a-layout class="ag-layout">
    <a-layout-sider class="ag-layout-side" v-model:collapsed="collapsed" :width="260" :trigger="null" collapsible>
      <div class="ag-side-logo">        
        <!-- 当侧边栏卷起来的时候，切换仅有图标 -->
        <img src="/@/assets/logo.svg" alt="agooday" style="width: 32px; height: 32px; margin-left: 8px;">
        <!-- 在这里可以添加title，我们以图片的方式替代文字 -->
        <img v-show="!collapsed" src="/@/assets/agpay.svg" alt="agpay" style="width:90px; height: 32px; margin: 5px 0 0 5px">
      </div>
      <a-menu class="ag-side-menu" v-model:selectedKeys="selectedKeys" theme="dark" mode="inline">
        <a-menu-item key="1">
          <pie-chart-outlined />
          <span>Option 1</span>
        </a-menu-item>
        <a-menu-item key="2">
          <desktop-outlined />
          <span>Option 2</span>
        </a-menu-item>
        <a-sub-menu key="sub1">
          <template #title>
              <span>
                <user-outlined />
                <span>User</span>
              </span>
          </template>
          <a-menu-item key="3">Tom</a-menu-item>
          <a-menu-item key="4">Bill</a-menu-item>
          <a-menu-item key="5">Alex</a-menu-item>
        </a-sub-menu>
        <a-sub-menu key="sub2">
          <template #title>
              <span>
                <team-outlined />
                <span>Team</span>
              </span>
          </template>
          <a-menu-item key="6">Team 1</a-menu-item>
          <a-menu-item key="8">Team 2</a-menu-item>
        </a-sub-menu>
        <a-menu-item key="9">
          <file-outlined />
          <span>File</span>
        </a-menu-item>
      </a-menu>
    </a-layout-sider>
    <a-layout class="ag-layout-main">
      <!-- 顶部  -->
      <a-layout-header class="ag-layout-header">
        <a-row class="ag-layout-header-main" justify="space-between">
          <a-col class="ag-layout-header-left">
            <menu-unfold-outlined
                v-if="collapsed"
                class="trigger"
                @click="() => collapsed = !collapsed"
            />
            <menu-fold-outlined v-else class="trigger" @click="() => collapsed = !collapsed" />
            <reload-outlined class="trigger"/>
            <page-header-wrapper/>
          </a-col>
          <a-col class="ag-layout-header-right">            
            <a-dropdown>
              <div class="ag-layout-header-user">
                <a-avatar shape="square" size="small" class="ag-layout-header-user-avatar" />
                <span class="user-name">超ddd管</span>
              </div>
              <template #overlay>
                <a-menu>
                  <a-menu-item>
                    <a-icon type="setting" /> 账户设置
                  </a-menu-item>
                  <a-menu-divider />
                  <a-menu-item key="logout" @click="handleLogout">
                    <a-icon type="logout" /> 退出登录
                  </a-menu-item>
                </a-menu>
              </template>
            </a-dropdown>
          </a-col>
        </a-row>
      </a-layout-header>
      <a-layout-content class="ag-layout-content">
        <a-card>
          <p>Card content</p>
          <p>Card content</p>
          <p>Card content</p>
        </a-card>
      </a-layout-content>
      <!-- footer 版权公司信息 -->
      <a-layout-footer class="ag-layout-footer">
        <div class="ag-version">
          <a target="_blank" class="ag-copyright" href="https://www.agpay.com"> Copyright &copy;2023-{{ currentYear }} AgPay | 吉日科技 </a>
        </div>
      </a-layout-footer>
      <!--- 回到顶部 -->
      <a-back-top :target="backTopTarget" :visibilityHeight="80" />
    </a-layout>
  </a-layout>
</template>

<script setup>
import { ref, getCurrentInstance } from 'vue';
import dayjs from 'dayjs';
import { useRouter } from 'vue-router';
import { useUserStore } from '/@/store/modules/system/user';

const router = useRouter();
const { proxy } = getCurrentInstance(); // 获取当前组件实例的 proxy 对象

const selectedKeys = ref(['1']);
const collapsed = ref(false);
const currentYear = dayjs().year();

const handleLogout = () => {
  proxy.$infoBox.confirmPrimary('是否退出登录？', `你好${useUserStore().userName}确认退出登录吗？`, () => {
    useUserStore().logout().then(() => {
      // 跳转到登录页
      router.push({ name: 'login' })
    })
  });
}
</script>

<style lang="less" scoped>
.ag-layout{
  .ag-layout-side {
    height: 100vh;
    .ag-side-logo {
      height: 32px;
      // background: rgba(255, 255, 255, 0.3);
      margin: 16px;
    }
    .ag-side-menu {}
  }
  .ag-layout-main {
    .ag-layout-header{
      background: #f5f5f5;
      padding: 0;
      .ag-layout-header-main{
        padding: 0 24px;
        .ag-layout-header-right {
          .ag-layout-header-user {
            cursor: pointer;
            height: 50px;
            .user-name {
              padding: 5px;
              // font-size: 16px;
              color: rgba(0, 0, 0, 0.85);
            }
          }
        }
        .ag-layout-header-left {
          .trigger {
            font-size: 18px;
            line-height: 64px;
            padding-right: 16px;
            cursor: pointer;
            transition: color 0.3s;
          }
        }
      }
    }
    .ag-layout-content {
      margin: 0 15px 24px;
      padding: 0 15px;
      min-height: 280px;
      background: #f5f5f5;
    }
  }
  .ag-layout-footer {
    position: relative;
    padding: 7px 0px;
    display: flex;
    justify-content: center;
    .ag-version {
      font-size: 14px;
      color: rgba(0, 0, 0, 0.45);
      a {
        color: rgba(0, 0, 0, 0.45);
      }
      a:hover {
        color: @primary-color;
      }
    }
  }
}
</style>
