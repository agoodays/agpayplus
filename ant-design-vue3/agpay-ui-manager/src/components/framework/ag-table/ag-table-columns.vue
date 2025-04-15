<script>
import { defineComponent, h } from 'vue';

export default defineComponent({
  name: 'AgTableColumns', // 定义组件名称
  render() {
    const slots = [];
    // 收集子节点
    this.$slots.default?.().forEach((item) => {
      if (item.type) {
        slots.push(item);
      }
    });

    if (slots.length <= 2) {
      // 小于等于两个直接渲染
      return h(
        'div',
        { style: 'display:flex; justify-content: space-evenly;' },
        slots
      );
    } else {
      // 超过两个时，显示第一个，其他放入更多菜单
      const firstEL = [slots[0]];
      const menuEL = [];
      for (let i = 1; i < slots.length; i++) {
        menuEL.push(h('a-menu-item', {}, slots[i]));
      }

      return h('div', { style: 'display:flex; justify-content: space-evenly;' }, [
        ...firstEL,
        h(
          'a-dropdown',
          {},
          {
            default: () =>
              h(
                'a-button',
                { class: 'ant-dropdown-link', type: 'link', style: '' },
                [
                  '更多',
                  h('a-icon', { type: 'down' }),
                ]
              ),
            overlay: () => h('a-menu', {}, menuEL),
          }
        ),
      ]);
    }
  },
});
</script>

<style lang="less" scoped>
/* 当前页面的按钮，减少 padding */
button {
  padding: 8px !important;
}
</style>