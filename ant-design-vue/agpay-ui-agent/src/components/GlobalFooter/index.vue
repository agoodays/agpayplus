<template>
  <global-footer class="footer custom-render">
    <template v-slot:links>
    </template>
    <template v-slot:copyright>
      <a href="http://www.agooday.com" target="_blank">@吉日科技</a>
    </template>
  </global-footer>
</template>

<script>
import { GlobalFooter } from '@ant-design-vue/pro-layout'

export default {
  name: 'ProGlobalFooter',
  components: {
    GlobalFooter
  }
}
</script>
