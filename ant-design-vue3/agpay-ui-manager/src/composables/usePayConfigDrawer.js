import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'

function getModelValue(model) {
	return model?.value !== undefined ? model.value : model
}

function setModelValue(model, value) {
	if (model?.value !== undefined) {
		model.value = value
		return
	}
	Object.assign(model, value)
}

export function usePayConfigDrawer({
	props,
	emit,
	infoForm,
	paramForm,
	saveObject,
	ifParams,
	isAdd,
	initialSaveObject,
	initialIfParams,
	loadConfig,
	buildSubmitPayload,
	saveConfig,
	clearEmptyKeys = [],
	requireParams = true,
	successMessage = '保存成功',
	shouldInit = (propsData) => Boolean(propsData?.record?.ifCode)
}) {
	const localOpen = ref(false)
	const loading = ref(false)

	watch(
		() => props.open,
		async (val) => {
			localOpen.value = val
			if (val && shouldInit(props)) {
				await initForm()
			}
		}
	)

	watch(localOpen, (val) => {
		emit('update:open', val)
	})

	async function initForm() {
		infoForm.value?.resetFields?.()
		paramForm.value?.resetFields?.()

		if (typeof initialSaveObject === 'function') {
			setModelValue(saveObject, initialSaveObject(props))
		}

		if (typeof initialIfParams === 'function') {
			setModelValue(ifParams, initialIfParams(props, getModelValue(saveObject)))
		}

		if (typeof loadConfig === 'function') {
			await loadConfig()
		}
	}

	async function validateForm(formRef) {
		if (!formRef.value?.validate) {
			return true
		}
		try {
			await formRef.value.validate()
			return true
		} catch {
			return false
		}
	}

	function clearEmptyKey(key) {
		const paramsValue = getModelValue(ifParams)
		if (paramsValue?.[key] === undefined || paramsValue?.[key] === null || paramsValue?.[key] === '') {
			paramsValue[key] = undefined
		}
		if (Object.prototype.hasOwnProperty.call(paramsValue, `${key}_ph`)) {
			paramsValue[`${key}_ph`] = undefined
		}
	}

	async function submit() {
		const valid1 = await validateForm(infoForm)
		const valid2 = await validateForm(paramForm)
		if (!valid1 || !valid2) {
			return false
		}

		loading.value = true
		try {
			const paramsValue = getModelValue(ifParams)
			if (requireParams && (!paramsValue || Object.keys(paramsValue).length === 0)) {
				message.error('参数不能为空！')
				return false
			}

			clearEmptyKeys.forEach((key) => clearEmptyKey(key))

			const reqParams =
				typeof buildSubmitPayload === 'function'
					? buildSubmitPayload({ saveObject: getModelValue(saveObject), ifParams: getModelValue(ifParams), isAdd: isAdd?.value !== undefined ? isAdd.value : isAdd })
					: null

			if (!reqParams) {
				return false
			}

			await saveConfig(reqParams)
			message.success(successMessage)
			localOpen.value = false
			emit('success')
			return true
		} finally {
			loading.value = false
		}
	}

	function uploadSuccess(name, fileList) {
		const [firstItem] = fileList
		const paramsValue = getModelValue(ifParams)
		paramsValue[name] = firstItem?.url
	}

	function handleClose() {
		localOpen.value = false
	}

	return {
		localOpen,
		loading,
		initForm,
		submit,
		validateForm,
		uploadSuccess,
		handleClose
	}
}
