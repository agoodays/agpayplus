<template>
  <a-drawer
    :open="isShow"
    title="分配角色"
    :mask-closable="true"
    @close="isShow = false"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="30%">
    <div>
      <div :style="{ paddingBottom: '20px', borderBottom: '1px solid #E9E9E9' }">
        <a-checkbox
          :indeterminate="checkedVal.length != 0 && allRoleList.length != checkedVal.length"
          :checked="checkedVal.length != 0 && allRoleList.length === checkedVal.length"
          @change="onCheckAllChange">
          全选
        </a-checkbox>
      </div>
      <br />
      <a-checkbox-group v-model:value="checkedVal" :options="allRoleList"/>
    </div>

    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" icon="close" @click="isShow = false">取消</a-button>
      <a-button type="primary" @click="handleOkFunc" icon="check" :loading="confirmLoading">保存</a-button>
    </div>
  </a-drawer>

</template>

<script setup>
import { ref, reactive, defineProps } from 'vue'
import { uSysUserRoleRela, req, reqLoad, API_URL_ROLE_LIST, API_URL_USER_ROLE_RELA_LIST } from '@/api/manage'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const isShow = ref(false)
const confirmLoading = ref(false)
const recordId = ref(null)
const sysType = ref(null)
const belongInfoId = ref(null)
const allRoleList = ref([])
const checkedVal = ref([])

const show = (recordIdParam, sysTypeParam, belongInfoIdParam) => {
  // 重置数据
  allRoleList.value = []
  checkedVal.value = []
  confirmLoading.value = false // 关闭loading
  recordId.value = recordIdParam
  sysType.value = sysTypeParam
  belongInfoId.value = belongInfoIdParam

  // 查询所有角色列表
  reqLoad.list(API_URL_ROLE_LIST, { pageSize: -1, sysType: sysType.value, belongInfoId: belongInfoId.value }).then(res => {
    if (res.total <= 0) {
      import('ant-design-vue').then(({ message }) => {
        message.error(`当前暂无角色，请先行添加`)
      })
      return
    }

    allRoleList.value = []
    res.records.map(role => {
      allRoleList.value.push({ label: role.roleName, value: role.roleId })
      isShow.value = true
    })

    // 查询已分配的列表
    req.list(API_URL_USER_ROLE_RELA_LIST, { pageSize: -1, userId: recordIdParam }).then(relaRes => {
      checkedVal.value = []
      relaRes.records.map(rela => {
          checkedVal.value.push(rela.roleId)
      })
    })
  })
}

const handleOkFunc = () => {
  confirmLoading.value = true // 显示loading
  uSysUserRoleRela(recordId.value, checkedVal.value).then(res => {
    import('ant-design-vue').then(({ message }) => {
      message.success('更新成功！')
      isShow.value = false

      if (props.callbackFunc !== undefined) {
        props.callbackFunc() // 刷新列表
      }
    })
  }).catch(res => { 
    confirmLoading.value = false 
  }) // 恢复loading
}

const onCheckAllChange = (e) => {
  checkedVal.value = []
  if (e.target.checked) { // 全选
    allRoleList.value.map(role => { 
      checkedVal.value.push(role.value) 
    })
  }
}
</script>