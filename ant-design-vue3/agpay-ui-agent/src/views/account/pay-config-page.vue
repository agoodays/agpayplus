<template>
  <div>
    <a-card :bordered="false">
      <AgPayConfigPanel
        :perm-code="'ENT_AGENT_SELF_PAY_CONFIG_ADD'"
        config-mode="agentSelf"
        :info-id="infoId"
      />
    </a-card>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { AgPayConfigPanel } from '@/components'
import { useUserStore } from '@/store/modules/system/user'

const userStore = useUserStore()
const infoId = computed(() => userStore.belongInfoId)
</script>
