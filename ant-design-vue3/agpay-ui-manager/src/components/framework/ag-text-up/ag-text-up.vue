<template>
  <div class="ag-text-up table-head-layout">
    <a-input
      required="required"
      :value="msg"
      @input="updateValue"
      @focus="handleFocus"
      @blur="handleBlur"
    />
    <label :class="{ active: isActive || msg }">{{ placeholder }}</label>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';

// Props
defineProps({
  msg: { type: String, default: '' },
  placeholder: { type: String, default: '' },
});

// Emits
defineEmits(['input']);

// Local state
const isActive = ref(false);

// Methods
const updateValue = (event) => {
  emit('input', event.target.value);
};

const handleFocus = () => {
  isActive.value = true;
};

const handleBlur = () => {
  isActive.value = false;
};

// Watch msg to handle pre-filled values
watch(
  () => msg,
  (newValue) => {
    if (newValue) {
      isActive.value = true;
    }
  },
  { immediate: true }
);
</script>

<style scoped lang="less">
/* 文字上移效果 */
.ag-text-up {
  position: relative;

  input {
    outline: 0;
    text-indent: 60px;
    transition: all 0.3s ease-in-out;
  }

  input::-webkit-input-placeholder {
    color: var(--text-color-weak);
    text-indent: 0;
  }

  input + label {
    pointer-events: none;
    position: absolute;
    left: 0;
    bottom: 6px;
    padding: 2px 11px;
    color: var(--text-color-weak);
    font-size: 13px;
    text-transform: uppercase;
    transition: all 0.3s ease-in-out;
    border-radius: var(--border-radius-small);
    background: transparent;
    height: 20px;
    line-height: 20px;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  input + label:after {
    position: absolute;
    content: '';
    width: 0;
    height: 0;
    top: 100%;
    left: 50%;
    margin-left: -3px;
    border-left: 3px solid transparent;
    border-right: 3px solid transparent;
    transition: all 0.3s ease-in-out;
  }

  input:focus,
  input:active,
  input:valid + label {
    text-indent: 0;
    background: var(--base-bg-color);
  }

  input:focus + label,
  input:active + label,
  input:valid + label {
    color: var(--text-on-primary);
    background: var(--primary-color);
    transform: translateY(-33px);
  }

  input:focus + label:after,
  input:active + label:after {
    border-top: 4px solid var(--ant-primary-color);
  }

  input:valid {
    text-indent: 0; // 文字不下移
  }

  input:valid + label {
    background: var(--hover-bg-color); // 更换背景色
  }

  input:valid + label:after {
    border-top: 4px solid var(--hover-bg-color); // 更换背景色
  }

  label.active {
    color: var(--text-on-primary);
    background: var(--ant-primary-color);
    transform: translateY(-33px);
  }
}
</style>