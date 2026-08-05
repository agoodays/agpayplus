# 文档索引 📚

## 📋 文档分类

### 🎯 核心文档（必读）

| 文档 | 说明 | 优先级 |
|-----|------|--------|
| [README.md](./README.md) | **项目说明** - 项目整体介绍 | ⭐⭐⭐⭐⭐ |
| [QUICK_START.md](./QUICK_START.md) | **快速开始** - 快速上手指南与组件分类总览 | ⭐⭐⭐⭐⭐ |
| [DOCUMENTATION_INDEX.md](./DOCUMENTATION_INDEX.md) | **文档索引** - 完整文档列表与统计 | ⭐⭐⭐⭐⭐ |
| [CUSTOM_COMPONENTS_USAGE_GUIDE.md](./CUSTOM_COMPONENTS_USAGE_GUIDE.md) | **自定义组件使用指南** - 统一使用规范与场景示例 | ⭐⭐⭐⭐⭐ |
| [FRONTEND_NAMING_CONVENTIONS.md](./FRONTEND_NAMING_CONVENTIONS.md) | **前端开发与命名规范** - 文件、符号、分层与提交检查清单 | ⭐⭐⭐⭐⭐ |

### 🎨 组件文档

所有组件文档位于: `src/components/[组件名]/README.md`，共 20 个通用组件。

#### 浮动标签表单组件（7个）✅

| 组件 | 文档路径 | 状态 | 说明 |
|-----|---------|------|------|
| AgInput | [src/components/ag-input/README.md](./src/components/ag-input/README.md) | ✅ | 浮动标签输入框 |
| AgInputNumber | [src/components/ag-input-number/README.md](./src/components/ag-input-number/README.md) | ✅ | 数字输入框 |
| AgInputNumberRange | [src/components/ag-input-number-range/README.md](./src/components/ag-input-number-range/README.md) | ✅ | 数字范围输入框 |
| AgTextarea | [src/components/ag-textarea/README.md](./src/components/ag-textarea/README.md) | ✅ | 多行文本输入 |
| AgSelect | [src/components/ag-select/README.md](./src/components/ag-select/README.md) | ✅ | 下拉选择器 |
| AgSelectInfinite | [src/components/ag-select-infinite/README.md](./src/components/ag-select-infinite/README.md) | ✅ | 无限滚动下拉选择 |
| **AgDateRangePicker** | [src/components/ag-date-range-picker/README.md](./src/components/ag-date-range-picker/README.md) | ✅ **v2.0** | **高级日期范围选择器** ⭐⭐⭐ |

> **💡 日期选择器选择指南：**
> - **AgDateRangePicker（推荐）**：功能全面，支持周报/月报/季报/年报，自动调整日期范围 ⭐⭐⭐

#### 数据展示组件（4个）✅

| 组件 | 文档路径 | 状态 |
|-----|---------|------|
| AgTable | [src/components/ag-table/README.md](./src/components/ag-table/README.md) | ✅ |
| AgTableAction | [src/components/ag-table-action/README.md](./src/components/ag-table-action/README.md) | ✅ |
| AgTableActions | [src/components/ag-table-actions/README.md](./src/components/ag-table-actions/README.md) | ✅ |
| AgCard | [src/components/ag-card/README.md](./src/components/ag-card/README.md) | ✅ |

#### 搜索组件（1个）✅

| 组件 | 文档路径 | 状态 |
|-----|---------|------|
| AgSearch | [src/components/ag-search/README.md](./src/components/ag-search/README.md) | ✅ |

#### 容器组件（2个）✅

| 组件 | 文档路径 | 状态 |
|-----|---------|------|
| AgDrawer | [src/components/ag-drawer/README.md](./src/components/ag-drawer/README.md) | ✅ |
| AgModal | [src/components/ag-modal/README.md](./src/components/ag-modal/README.md) | ✅ |

#### 功能组件（4个）✅

| 组件 | 文档路径 | 状态 |
|-----|---------|------|
| AgUpload | [src/components/ag-upload/README.md](./src/components/ag-upload/README.md) | ✅ |
| AgEditor | [src/components/ag-editor/README.md](./src/components/ag-editor/README.md) | ✅ |
| AgStateSwitch | [src/components/ag-state-switch/README.md](./src/components/ag-state-switch/README.md) | ✅ |
| AgLoading | [src/components/ag-loading/README.md](./src/components/ag-loading/README.md) | ✅ |

#### 工具组件（2个）✅

| 组件 | 文档路径 | 状态 | 说明 |
|-----|---------|------|------|
| AgErrorBoundary | [src/components/ag-error-boundary/README.md](./src/components/ag-error-boundary/README.md) | ✅ | 全局错误边界 |
| GlobalLoad | [src/components/global-load/README.md](./src/components/global-load/README.md) | ✅ | 全局加载控制 |

---

## 🎯 使用建议

### 新项目开发者

1. ⭐ 查看 [QUICK_START.md](./QUICK_START.md) - 快速上手与组件分类总览
2. 📖 参考组件文档: `src/components/[组件名]/README.md`
3. 🎨 查看 Demo: `src/views/demo/`

### 维护者

1. 📖 查看专题文档了解设计决策
2. 🔧 参考优化记录了解改进历程
3. 📦 归档过时文档保持简洁

### 新组件开发

1. 📋 参考现有组件模式
2. 📝 编写完整的 README.md
3. 🎨 创建 Demo 示例

---

## 📞 获取帮助

- 📖 [快速开始](./QUICK_START.md) - 组件分类总览与使用指南
- 🎯 [Demo 示例](./src/views/demo/) - 实际代码示例
- 💬 [问题反馈](https://github.com/agoodays/agpayplus/issues)

---

**最后更新**: 2026-08-05  
**维护者**: AGPay Team  

### 📊 文档统计

```
🎊 文档库状态
━━━━━━━━━━━━━━━━━━━━━━
📁 根目录文档:   5 个
📚 组件文档:     20 个
📦 归档文档:     0 个
✨ 文档覆盖率:   100%
🎯 Demo 数量:    10 个
━━━━━━━━━━━━━━━━━━━━━━
```

| 指标 | 数量 | 说明 |
|-----|------|------|
| 根目录文档 | 5 个 | 核心活跃文档 |
| 组件数量 | 20 个 | 通用组件 |
| 组件文档 | 20 个 | 覆盖率 100% ✨ |
| Demo 数量 | 10 个 | 完整示例 |
| 归档文档 | 0 个 | 已清理完毕 |

🎉 保持文档简洁，专注核心内容！

