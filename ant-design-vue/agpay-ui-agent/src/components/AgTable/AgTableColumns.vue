<!--
  AgPay 通用列，添加更多菜单项

  @author terrfly
  @site https://www.agpay.vip
  @date 2021/5/8 07:18
-->

<script>
export default {
  name: 'AgTableColumns', // 定义组件名称
  render (createElement, context) {
    const slots = []
    // eslint-disable-next-line no-unused-expressions
    this.$slots.default?.map(item => {
      if (item.tag) {
        slots.push(item)
      }
      return false
    })
    if (slots.length <= 2) { // 小于等于二个直接渲染
      return createElement(
        'div',
        { style: 'display:flex; justify-content: space-evenly;' },
        slots // 子节点数组
      )
    } else {
      const firstEL = [slots[0]]
      const menuEL = []
      for (let i = 1; i < slots.length; i++) {
        menuEL.push(<a-menu-item>{slots[i]}</a-menu-item>)
      }
      return <div style="display:flex; justify-content: space-evenly;"> {firstEL}
                  <a-dropdown>
                      <a-button class="ant-dropdown-link" type="link" style="">更多<a-icon type="down" /></a-button>
                      <a-menu slot="overlay">
                        {menuEL}
                      </a-menu>
                  </a-dropdown>
             </div>
    }
  }
}
</script>
<style lang="less" scoped>

//当前页面的按钮， 减少padding
button { padding:  8px !important;}

</style>
