<template>
  <a-layout class="ag-layout">
    <a-layout-sider class="ag-layout-side" v-model:collapsed="collapsed" :width="260" :trigger="null" collapsible>
      <div class="ag-side-logo" />
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
          </a-col>
          <a-col class="ag-layout-header-right"></a-col>
        </a-row>
      </a-layout-header>
      <a-layout-content class="ag-layout-content">
        <a-card>
          <p>Card content</p>
          <p>Card content</p>
          <p>Card content</p>
        </a-card>
      </a-layout-content>
    </a-layout>
  </a-layout>
</template>
<script setup>
import { ref } from 'vue';
const selectedKeys = ref(['1']);
const collapsed = ref(false);
</script>
<style lang="less" scoped>
.ag-layout{
  .ag-layout-side{
    height: 100vh;
    .ag-side-logo {
      height: 32px;
      background: rgba(255, 255, 255, 0.3);
      margin: 16px;
    }
    .ag-side-menu{
    }
  }
  .ag-layout-main{
    .ag-layout-header{
      background: #f5f5f5;
      padding: 0;
      .ag-layout-header-main{
        padding: 0 24px;
        .ag-layout-header-left{
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
    .ag-layout-content{
      margin: 0 15px 24px;
      padding: 0 15px;
      min-height: 280px;
      background: #f5f5f5;
    }
  }
}
</style>
