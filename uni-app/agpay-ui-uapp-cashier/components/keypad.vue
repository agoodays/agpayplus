<template>
	<view class="keypad">
		<view 
			v-for="(key, index) in keys"
			:key="index"
			class="key"
			:class="{ 'gray-bg': ['C', '⌫'].includes(key) }"
			@touchstart="handleKeyPress(key)"
		>
			{{ key }}
		</view>
	</view>
</template>

<script>
export default {
	props: ['value'],
	data() {
		return {
			keys: ['1','2','3','4','5','6','7','8','9','C','0','⌫']
		}
	},
	methods: {
		handleKeyPress(key) {
			let newValue = this.value
			
			if (key === '⌫') {
				newValue = newValue.slice(0, -1)
			} else if (key === 'C') {
				newValue = ''
			} else {
				if (newValue.includes('.') && key === '.') return
				if (newValue === '0' && key !== '.') newValue = ''
				newValue += key
			}

			this.$emit('input', newValue)
		}
	}
}
</script>

<style scoped>
.keypad {
	display: grid;
	grid-template-columns: repeat(3, 1fr);
	gap: 10px;
	padding: 15px;
}

.key {
	height: 60px;
	display: flex;
	justify-content: center;
	align-items: center;
	background: #f0f0f0;
	border-radius: 8px;
	font-size: 24px;
	user-select: none;
}

.gray-bg {
	background: #e0e0e0;
}
</style>