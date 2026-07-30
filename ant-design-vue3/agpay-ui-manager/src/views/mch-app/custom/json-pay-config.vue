<template>
	<ag-drawer
		title="填写参数"
		width="40%"
		:closable="true"
		:mask-closable="false"
		v-model:open="localOpen"
		:drawer-style="{ overflow: 'hidden' }"
		:body-style="{ paddingBottom: '80px', overflow: 'auto' }"
		@close="handleClose"
	>
		<a-form ref="infoForm" :model="saveObject" layout="vertical" :rules="rules">
			<a-row :gutter="16">
				<a-col :span="12">
					<a-form-item label="状态" name="state">
						<a-radio-group v-model:value="saveObject.state" :options="stateOptions" />
					</a-form-item>
				</a-col>
				<a-col :span="24">
					<a-form-item label="备注" name="remark">
						<a-input v-model:value="saveObject.remark" placeholder="请输入" type="textarea" />
					</a-form-item>
				</a-col>
			</a-row>
		</a-form>

		<a-divider orientation="left">
			<a-tag color="#FF4B33">{{ saveObject.ifCode }} 商户参数配置</a-tag>
		</a-divider>

		<a-form ref="mchParamForm" :model="ifParams" layout="vertical" :rules="ifParamsRules">
			<a-row :gutter="16">
				<a-col v-for="(item, key) in mchParams" :key="key" :span="item.type === 'text' ? 12 : 24">
					<a-form-item :label="item.desc" :name="item.name" v-if="item.type === 'text' || item.type === 'textarea'">
						<a-input v-model:value="ifParams[item.name]" :placeholder="item.star === '1' ? (ifParams[item.name + '_ph'] || '请输入') : '请输入'" :type="item.type" />
					</a-form-item>
					<a-form-item :label="item.desc" :name="item.name" v-else-if="item.type === 'radio'">
						<a-radio-group v-model:value="ifParams[item.name]">
							<a-radio v-for="(radioItem, radioKey) in item.values" :key="radioKey" :value="radioItem.value">
								{{ radioItem.title }}
							</a-radio>
						</a-radio-group>
					</a-form-item>
					<a-form-item :label="item.desc" :name="item.name" v-else-if="item.type === 'file'">
						<ag-upload
							:action="action"
							:bind-name="item.name"
							:urls="[ifParams[item.name]]"
							list-type="picture"
							@upload-success="uploadSuccess"
						>
							<template #uploadSlot="{ loading: uploadLoading }">
								<a-button class="ag-upload-btn">
									<component :is="uploadLoading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传
								</a-button>
							</template>
						</ag-upload>
					</a-form-item>
				</a-col>
			</a-row>
		</a-form>

		<div class="drawer-btn-center">
			<a-button :style="{ marginRight: '8px' }" @click="handleClose">
				<template #icon><CloseOutlined /></template>
				取消
			</a-button>
			<a-button type="primary" @click="handleSubmit" :loading="loading">
				<template #icon><CheckOutlined /></template>
				保存
			</a-button>
		</div>
	</ag-drawer>
</template>

<script setup>
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { AgDrawer, AgUpload } from '@/components'
import { getStateOptions } from '@/constants/common-const'
import { upload } from '@/lib/ag-axios'
import { CheckOutlined, CloseOutlined, LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const stateOptions = computed(() => getStateOptions(t))
const icons = { LoadingOutlined, UploadOutlined }

const props = defineProps({
	open: {
		type: Boolean,
		default: false
	},
	appId: {
		type: String,
		default: ''
	},
	record: {
		type: Object,
		default: () => ({})
	}
})

const emit = defineEmits(['update:open', 'success'])

const infoForm = ref(null)
const mchParamForm = ref(null)
const loading = ref(false)
const localOpen = ref(false)
const mchType = ref(null)
const action = ref(upload.cert)
const mchParams = ref([])

const saveObject = reactive({
	infoId: null,
	ifCode: null,
	state: 1,
	remark: ''
})

const ifParams = reactive({})

const rules = {
	infoId: [{ required: true, trigger: 'blur' }],
	ifCode: [{ required: true, trigger: 'blur' }]
}

const ifParamsRules = ref({})

watch(
	() => props.open,
	(val) => {
		localOpen.value = val
		if (val && props.appId && props.record.ifCode) {
			resetForm()
			getMchPayConfig(props.record)
		}
	}
)

watch(localOpen, (val) => {
	emit('update:open', val)
})

const resetForm = () => {
	mchType.value = props.record.mchType
	Object.keys(saveObject).forEach((key) => {
		saveObject[key] = null
	})

	Object.keys(ifParams).forEach((key) => {
		delete ifParams[key]
	})

	mchParams.value = []
	saveObject.infoId = props.appId
	saveObject.ifCode = props.record.ifCode
	saveObject.state = props.record.ifConfigState === 0 ? 0 : 1

	if (mchParamForm.value) {
		mchParamForm.value.resetFields()
	}
}

const getMchPayConfig = async (record) => {
	try {
		const res = await mchAppApi.getMchPayConfigUnique(saveObject.infoId, saveObject.ifCode)
		if (res && res.ifParams) {
			Object.assign(saveObject, res)
			const parsedParams = JSON.parse(res.ifParams)
			Object.assign(ifParams, parsedParams)
		}

		const newItems = []
		const mchParamsStr = mchType.value === 1 ? record.normalMchParams : record.isvsubMchParams
		JSON.parse(mchParamsStr).forEach((item) => {
			let radioItems = []
			if (item.type === 'radio') {
				const valueItems = item.values.split(',')
				const titleItems = item.titles.split(',')
				for (let i = 0; i < valueItems.length; i++) {
					let radioVal = valueItems[i]
					if (!Number.isNaN(Number(radioVal))) {
						radioVal = Number(radioVal)
					}
					radioItems.push({ value: radioVal, title: titleItems[i] })
				}
			}

			if (item.star === '1') {
				ifParams[item.name + '_ph'] = ifParams[item.name] ? ifParams[item.name] : '请输入'
				if (ifParams[item.name]) {
					ifParams[item.name] = ''
				}
			}

			newItems.push({
				name: item.name,
				desc: item.desc,
				type: item.type,
				verify: item.verify,
				values: radioItems,
				star: item.star
			})
		})

		mchParams.value = newItems
		generateRules()
	} catch (error) {
		console.error('获取商户支付配置失败:', error)
	}
}

const handleSubmit = async () => {
	try {
		await infoForm.value.validate()
		await mchParamForm.value.validate()

		loading.value = true
		const reqParams = {
			infoId: saveObject.infoId,
			ifCode: saveObject.ifCode,
			state: saveObject.state,
			remark: saveObject.remark
		}

		if (Object.keys(ifParams).length === 0) {
			message.error('参数不能为空！')
			return
		}

		Object.keys(mchParams.value).forEach((key) => {
			const item = mchParams.value[key]
			if (item.star === '1' && ifParams[item.name] === '') {
				ifParams[item.name] = undefined
			}
			if (ifParams[item.name + '_ph'] !== undefined) {
				delete ifParams[item.name + '_ph']
			}
		})

		reqParams.ifParams = JSON.stringify(ifParams)
		await mchAppApi.addMchPayConfig(reqParams)

		message.success('保存成功')
		localOpen.value = false
		emit('success')
	} catch (error) {
		console.error('提交失败:', error)
	} finally {
		loading.value = false
	}
}

const uploadSuccess = (name, fileList) => {
	const [firstItem] = fileList
	ifParams[name] = firstItem?.url
}

const generateRules = () => {
	const generatedRules = {}
	Object.keys(mchParams.value).forEach((key) => {
		const item = mchParams.value[key]
		const ruleItems = []
		if (item.verify === 'required' && item.star !== '1') {
			ruleItems.push({
				required: true,
				message: '请输入' + item.desc,
				trigger: 'blur'
			})
			generatedRules[item.name] = ruleItems
		}
	})
	ifParamsRules.value = generatedRules
}

const handleClose = () => {
	localOpen.value = false
}
</script>

<style lang="less" scoped></style>
