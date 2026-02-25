<template>
  <div class="ag-upload">
    <a-upload
      :name="name"
      :action="action"
      :headers="headers"
      :show-upload-list="showUploadList"
      :before-upload="beforeUpload"
      @change="handleChange"
    >
      <slot>
        <a-button>
          <upload-outlined /> {{ t('components.upload') }}
        </a-button>
      </slot>
    </a-upload>
  </div>
</template>

<script setup>
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps({
  name: { type: String, default: 'file' },
  action: { type: String, default: '' },
  headers: { type: Object, default: () => ({}) },
  showUploadList: { type: Boolean, default: false },
  beforeUpload: { type: Function, default: null }
})

const emit = defineEmits(['change', 'success', 'error'])

function handleChange(info) {
  emit('change', info)
  const { status } = info.file
  if (status === 'done') emit('success', info.file.response)
  if (status === 'error') emit('error', info.file.error)
}
</script>

<style scoped>
</style>