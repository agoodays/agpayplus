<template>
  <div>
    <a-divider orientation="left">{{ title }}</a-divider>
    <a-row :gutter="16">
      <a-col v-for="item in items" :key="item.key || item.span + '-' + item.offset" :span="item.span || 12" :offset="item.offset || 0">
        <template v-if="item.type === 'blank'">
          <div class="form-blank" />
        </template>
        <a-form-item v-else :label="item.label" :name="item.name" :rules="item.rules">
          <template v-if="item.type === 'select'">
            <a-select v-model:value="model[item.key]" :placeholder="item.placeholder">
              <a-select-option v-for="opt in item.options" :key="opt.value" :value="opt.value">{{ opt.label }}</a-select-option>
            </a-select>
          </template>
          <template v-else-if="item.type === 'radio'">
            <a-radio-group v-model:value="model[item.key]">
              <a-radio v-for="opt in item.options" :key="opt.value" :value="opt.value">{{ opt.label }}</a-radio>
            </a-radio-group>
          </template>
          <template v-else-if="item.type === 'textarea'">
            <a-textarea v-model:value="model[item.key]" :placeholder="item.placeholder" :rows="item.rows || 3" />
          </template>
          <template v-else-if="item.type === 'upload'">
            <ag-upload
              :action="item.action || uploadUrl.form"
              :bind-name="item.name"
              :urls="model[item.key] ? [model[item.key]] : []"
              :accept="item.accept || ''"
              :list-type="item.listType || 'picture'"
              :num="item.num || 1"
              :multiple="item.multiple || false"
              :size="item.size || 10"
              @upload-success="onUploadSuccess"
            />
          </template>
          <template v-else>
            <a-input v-model:value="model[item.key]" :placeholder="item.placeholder" />
          </template>
        </a-form-item>
      </a-col>
    </a-row>
  </div>
</template>

<script setup>
import { AgUpload } from '@/components'
import { uploadUrl } from '@/lib/ag-axios'

const props = defineProps({
  title: { type: String, required: true },
  model: { type: Object, required: true },
  items: { type: Array, required: true }
})

const emit = defineEmits(['update:modelValue'])

function onUploadSuccess(name, fileList) {
  const first = fileList?.[0]
  props.model[name] = first?.url || ''
  emit('update:modelValue', props.model)
}
</script>
