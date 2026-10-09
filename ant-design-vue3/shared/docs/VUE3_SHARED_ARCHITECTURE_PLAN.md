# Vue3 共享架构优化计划

## 1. 文档用途

本文档是 `ant-design-vue3` 共享架构优化工作的唯一执行计划和交接记录。每个 Agent 开始工作前必须先阅读本文档、检查当前工作区和相关代码；结束或交接前必须更新任务状态、实际改动、验证结果、未决问题和下一步。

计划按阶段执行。阶段未达到验收标准前，不得标记完成；遇到设计决策门槛时，先补充证据和决策记录，不得默认扩大改造范围。

## 2. 目标与范围

### 目标

- 减少 Manager、Agent、Merchant 三个 Vite 配置之间的重复，同时保留每个应用独立开发、构建和部署的能力。
- 将共享页面中的项目差异配置从单一大型配置表整理为可理解、可验证、由应用边界明确提供的配置。
- 保持三个应用现有业务行为、路由、权限、环境变量和构建产物兼容。
- 让任何后续 Agent 都能从本文档准确判断当前状态、已完成内容和下一项安全操作。

### 不在本计划范围

- 合并三个应用为单个运行时应用或改动后端 API。
- 改造 Vue2 遗留项目。
- 一次性重排全部 `shared` 目录或迁移成独立发布的 npm 包。
- 借本次工作修复与共享配置无关的业务问题。

## 3. 当前架构判断

- 三个独立应用共享 `ant-design-vue3/shared` 源码；应用入口、路由及运行上下文仍由各自项目负责。该共享方向暂时保留。
- 三份 `agpay-ui-*/vite.config.js` 有大量共同的插件、依赖预构建和构建选项；应用 ID 与端口等差异应留在应用配置中。
- `shared/config/project-config.js` 集中维护默认值和三个应用的页面差异，覆盖列表、字段显隐和行为选项等多类职责。多个共享页面直接调用 `getProjectConfig(__APP_ID__)`。
- `__APP_ID__` 还被共享 Store 等其他逻辑使用；整理页面差异时不得未经调用面审计就移除该 define。
- `shared/config/resolve-shared-alias.js` 实现了应用源码优先、共享目录兜底的自定义解析。它与现有源码导入方式有关，先评估再决定是否改造。
- 初始检查时，工作区已有未提交变更，包含三个 `vite.config.js`。接手者必须先执行 `git status --short` 并查看相关 diff；不得覆盖或回滚他人改动。

## 4. 执行原则

1. 每阶段单独提交代码范围和验收，不跨阶段顺手重构。
2. 修改前记录目标文件的现有状态；只编辑本计划列明且经过确认的文件。
3. 每次代码编辑后，先执行本阶段最窄的验证，再继续其他修改。
4. 先保持应用行为等价；行为变化必须单独说明并获得明确依据。
5. 不删除 `__APP_ID__`、alias、依赖或配置项，除非调用面审计证明没有需要保留的用途。
6. 不以成功构建替代业务验收；三个应用都要分别验证。
7. 每次 Agent 会话结束前更新第 8 节交接记录。未完成的工作也要记录，不得只留下代码状态让下一位猜测。

## 5. 阶段计划

### 阶段 0：基线与差异盘点

**状态：已完成（2026-10-09）**

**范围：** 三个 Vue3 应用、`shared/config/project-config.js`、共享 alias；只读盘点，不改实现。

**步骤：**

- 检查工作区状态和三个 Vite 配置的现存 diff，识别用户已有改动。
- 对比三个 Vite 配置，登记真正差异：应用 ID、端口、插件、环境变量、代理、PWA、构建参数和依赖解析。
- 搜索全部 `getProjectConfig` 与 `__APP_ID__` 源码调用点，按页面/Store/配置职责分类；排除 `dist` 和其他生成物。
- 检查各应用是否已有本地 `src/config/project-config.js`，并确认 `createSharedAlias` 对本地优先解析的实际行为。
- 确认各应用依赖安装与 Node 模块解析边界，避免公共 Vite 配置导入插件时解析到错误的 `node_modules`。
- 在不覆盖用户改动的前提下，记录三个应用当前可执行的构建/检查基线；遇到已有失败，记录原始失败信息。

**验收：**

- 有一份可复核的差异清单，能区分共同配置、应用配置和待调查项。
- 已知配置消费点、alias 解析规则、依赖解析边界和基线结果。
- 基线失败与本次改造问题明确区分。

### 阶段 0 盘点结果

**Vite 配置差异**

| 项目 | `__APP_ID__` | 开发端口 | 其余配置 |
|------|-------------|----------|----------|
| Manager | `manager` | 8817 | 与 Agent、Merchant 相同 |
| Agent | `agent` | 8816 | 与 Manager、Merchant 相同 |
| Merchant | `merchant` | 8818 | 与 Manager、Agent 相同 |

- 将应用 ID 与端口标准化后，三个 `vite.config.js` 无其他内容差异。共同部分包括 alias/dedupe、开发服务器共享目录访问、预构建依赖、压缩/PWA/可视化插件、Less 和生产构建选项。
- 执行前已有的三个 Vite 文件 diff 仅调整 import 排序，没有行为差异；改造时保留该排序意图，不覆盖其他工作区改动。
- `loadEnv` 加载的对象目前只出现在注释掉的代理配置中；三个项目的 `.env`、`.env.development`、`.env.production` 文件名和变量名集合一致。未读取或记录任何环境变量值。删除无用 `loadEnv` 前仍需确认相关脚本没有其他依赖。

**依赖和共享解析**

- 三个应用均有独立 `node_modules`、`package-lock.json`；Vite、Vue 插件、PWA、压缩和 visualizer 依赖声明版本一致。
- 因共享目录不是应用 `node_modules` 的父级目录，公共配置模块不应直接导入应用插件。阶段 1 采用公共工厂、应用侧创建并注入插件实例的方向。
- 三个应用均没有 `src/config/project-config.js`；应用 `src/config` 当前有 `app-config.js` 和 `dev-menu-config.js`。alias 先查应用 `src`，再查 `shared`，因此项目差异配置当前回退到共享实现。
- `createSharedAlias` 是自定义文件解析器，涵盖本地优先、扩展名和目录 index 回退、glob 排除；阶段 3 仍仅做评估。

**配置消费点**

- `getProjectConfig(` 搜索得到 49 个匹配、分布于 45 个文件（含定义/说明文本）；调用主要位于共享页面。页面中存在重复读取/合并的情况，阶段 2 再按业务域盘点，不在阶段 1 改动。
- 直接 `__APP_ID__ === 'agent'` 分支出现在商户进件、支付订单、退款订单三个共享页面；共享 App Store 另有两处根据应用 ID 选择站点子配置。阶段 2 必须保留并显式处理这些行为差异。
- `project-config.js` 的默认配置中 `sysUserRoleDist` 有重复声明；暂记为阶段 2 的审计项，不在基线阶段修复。

**构建基线**

| 应用 | 结果 | 模块数 | 用时 | 基线警告 |
|------|------|--------|------|----------|
| Manager | 通过 | 4278 | 约 68 秒 | `ag-axios-core.js` / `ag-axios.js` 跨 chunk 循环依赖警告 |
| Agent | 通过，退出码 0 | 4283 | 约 82 秒 | 同一跨 chunk 循环依赖警告 |
| Merchant | 通过，退出码 0 | 4315 | 约 55 秒 | 同一跨 chunk 循环依赖警告 |

三个构建结束后，工作区没有新增受跟踪源码改动；构建产物由项目忽略规则处理。工作区原有后端/Vue2 改动及三个 Vite 文件的 import 排序变更均保留。

**阶段 1 方案决策**

- 在 `shared/config` 新增不导入第三方插件的 Vite 配置工厂；由各应用配置从本地依赖创建插件并传入工厂。
- 项目配置仅保留 `appId` 和 `port` 等差异；公共配置内容尽量与当前值逐项等价。
- `loadEnv` 的变量目前未参与有效配置，阶段 1 删除前已盘点 env 文件名和变量名；如遇到脚本/代理实际依赖则保留。

### 阶段 1：收敛 Vite 公共配置

**状态：已完成（2026-10-09）**

**预期涉及：** 三个应用 `vite.config.js`；是否新增公共配置工厂由阶段 0 的依赖解析调查决定。

**步骤：**

- 先从三个配置提取共同项和应用专属项，确保现有用户 diff 被纳入而非覆盖。
- 选择满足当前依赖布局的实现：公共工厂接收各应用传入的插件/参数，或使用经验证的共享配置模块。不得假设共享目录能直接解析各应用的第三方插件依赖。
- 将应用 ID、开发端口及确实不同的选项保留在项目配置中；删除仅确认无效或无用的配置前，先核查现有脚本和环境文件。
- 逐个验证三个应用的开发配置与生产构建。

**验收：**

- 公共配置只有一个维护入口，项目配置只表达本应用差异。
- 三个应用开发服务器使用原端口，构建目录、资源路径、PWA、压缩、chunk 策略及 define 行为保持基线兼容。
- 三个应用 `npm run build` 均通过；差异或基线失败均有记录。

**回退边界：** 仅回退本阶段新引入的配置抽取，不撤销接手前已有 diff。

### 阶段 1 实施结果

- 新增 `shared/config/create-vite-config.js` 作为唯一公共配置工厂。工厂只导入 Node 内置模块和仓库共享 alias，不直接导入各应用的第三方构建插件；插件实例化器由三个应用自己的 `vite.config.js` 注入。
- 三份项目配置现在只声明各自插件依赖、`appId`、开发端口和项目根目录；应用 ID、端口、alias、dedupe、PWA、压缩、chunk 和构建设置均保留。
- 移除未使用的 `loadEnv` 和仅存在于注释中的代理配置；没有修改任何 `.env` 文件或变量。Vite 的 `import.meta.env` 仍由 Vite 自身按模式加载。
- 改造后构建：Manager 通过（4278 模块，约 51 秒，退出码 0）；Agent 通过（4283 模块，约 54 秒，退出码 0）；Merchant 通过（4315 模块，约 32 秒，退出码 0）。
- 三个构建均仍出现基线已有的 `ag-axios-core.js` / `ag-axios.js` 跨 chunk 循环依赖警告；未扩大范围处理。
- `get_errors` 检查共享工厂和三个 `vite.config.js` 均无诊断；脚本核对三个 `appId`/端口均与基线一致。
- 原工作区中三个 Vite 配置的 import 排序改动被保留；未重置其他 Vue2/后端改动。

### 阶段 2：整理项目差异配置

**状态：已完成（2026-10-09）**

**预期涉及：** `shared/config/project-config.js`、共享页面消费点，以及经阶段 0 确认后的应用本地配置入口。

**步骤：**

- 按业务域盘点配置项，标注为字段/列显隐、页面能力、行为差异、已失效或重复项。
- 对照真实页面验证配置 key 与列、搜索字段、表单字段是否存在；清除“配置存在但 UI 无消费”的假配置前，记录证据。
- 优先让应用组合入口显式提供本应用策略，避免共享页面散布应用 ID 分支。可利用现有本地优先 alias，但必须先验证本地配置文件存在性和解析结果。
- 将默认规则与 Agent、Manager、Merchant 的差异按业务域整理；避免继续向一个文件追加全站配置，也避免为每个页面制造无必要的抽象层。
- 页面差异仅为有限展示规则时保留数据配置；若流程或行为实质不同，评估应用专属包装/实现。
- 确认配置只解析/合并一次或采用等效静态方案；不得引入运行时动态配置需求。
- 保留其他 Store 等仍需要 `__APP_ID__` 的用途。

**验收：**

- 配置职责、应用覆盖来源和页面消费方式可从目录结构直接理解。
- 三个应用的关键角色/字段显隐、订单操作权限和导出差异与基线一致。
- 不存在已知死配置、重复 key 或无法追踪来源的应用分支；未能删除的项附原因。
- 三个应用生产构建通过，相关共享页面完成差异回归。

**决策门槛：** 如果需要大批页面包装才能注入配置，先停下记录成本/收益对比，再决定采用应用配置入口还是继续使用共享配置服务；不能直接扩大成全目录迁移。

**阶段 2 验收结果**

- Manager、Agent、Merchant 项目覆盖分别由各自 `src/config/project-config.js` 提供；shared 仅保留默认 schema 和合并工厂，所有应用覆盖 key 均通过默认 schema 校验。
- 已核对列、搜索、详情和表单覆盖的真实 UI 消费；清除了无消费者 key/字段、重复默认声明、无效企业/码牌配置，以及不存在于详情/表单模板的隐藏字段值。
- 订单退款表达式执行已改为显式模式；搜索项与选项加载共用 `hiddenSearchFields`；getter 不再接收 appId；仍控制业务行为的直接 `__APP_ID__` 分支保留。
- 最近一轮配置收敛后，三个应用 `npm run build` 均通过，退出码均为 0；覆盖 schema 检查、受影响文件诊断通过；旧 selector、死配置及旧 getter 调用无全仓残留。

### 阶段 2 已完成切片：订单配置

- `payOrderPage.refundVisibleWhen` 原先存储 JS 表达式字符串，并在支付订单页通过 `new Function` 执行。现改为 `refundButtonVisibility: 'eligible-only' | 'always'`，消除了动态代码执行。审计发现 Manager 的 `refundPermission: ''` 会先通过 `v-if` 移除退款按钮，因此 Manager 无需覆盖该可见性模式，冗余的 `always` 覆盖已删除。
- 支付订单页和退款订单页现统一由 `hiddenSearchFields` 控制支付接口筛选项显示及选项加载；表格列仍由 `hiddenColumns` 控制。删除重复布尔值 `showPayInterfaceFilter` 及 Merchant 对它的覆盖。
- 涉及 `shared/config/project-config.js`、支付订单页和退款订单页。旧 `refundVisibleWhen`、`showPayInterfaceFilter` 和 `new Function('record', ...)` 在共享源码中无残留。
- 这只是一个已验证业务切片，不代表全量配置审计完成；尚需继续映射实体、系统用户/角色、统计、分账、二维码、通知等配置与实际 UI 消费点。

### 阶段 2 已完成切片：无消费者配置清理

- 对默认配置的 52 个页面 key 与全仓 Vue3 源码读取点进行静态比对，发现 10 个唯一 key 没有消费者；`sysUserRoleDist` 在默认对象中重复声明，相关默认项和 Agent/Merchant 覆盖均已删除。
- 已删除无消费者的 `hiddenActions` 默认字段。全仓精确搜索确认这些 key 和字段仅在配置表中出现，不在应用代码中读取。
- 清理后 Manager、Agent、Merchant `npm run build` 均通过，退出码均为 0；`project-config.js` 无 VS Code 诊断；死配置 key 和 `hiddenActions` 全仓检索无残留。
- 该清单仍是静态 key 级审计；有效配置值是否对应真实列、搜索项和表单字段，需按剩余业务域继续核对。
- 系统角色、用户和团队列表实际只读取 `hiddenColumns` 与 `hiddenSearchFields`；已删除这三个列表配置中未被消费的 `hiddenFormFields` 默认项，实际表单配置 key 保留。
- 全仓未发现对 `getProjectConfig` 返回值的修改；已按 appId 缓存深合并结果，使同一应用后续读取复用同一只读配置对象。
- 支付筛选与 Manager 冗余覆盖清理后，Manager、Agent、Merchant `npm run build` 均通过，退出码均为 0；涉及源码无 VS Code 诊断；无消费者 key、`hiddenActions`、旧动态退款条件和重复布尔开关均无全仓残留。
- 企业权限列表覆盖中隐藏的 `agentNo`、`isvNo`、`mchNo`、`mchName` 不存在于该页搜索项或表格列；企业表单只创建 `seFormHid` 而模板从未读取。已删除 Agent/Merchant 对应覆盖、共享默认项及无效页面读取。
- 企业权限清理后 Manager、Agent、Merchant 构建均通过，退出码均为 0；相关页面、配置文件无 VS Code 诊断；`sysEntPage`/`sysEntAddOrEdit` 死配置检索无残留。
- 二维码模板页面实际只有 `shellAlias` 搜索和预览图/别名/操作列；Agent/Merchant 覆盖中的商户/服务商字段不存在于页面。已移除默认 schema、两应用覆盖和页面配置过滤读取。
- Agent/Merchant 代理商统计页没有 `isvNo` 列，已删除对应无效隐藏列值并保留有效搜索隐藏；服务商统计页真实有 `isvNo` 列，原覆盖保留。企业、二维码和统计调整后，三应用构建均通过，退出码均为 0。

### 阶段 2 已完成切片：应用配置边界

- 将 Manager、Agent、Merchant 覆盖分别迁入各应用 `src/config/project-config.js`；共享层 `shared/config/project-config-core.js` 只保留默认 schema 与 `createProjectConfig`，不再按 `__APP_ID__` 选择覆盖。
- 三个共享页面通过既有 `@/config/project-config` 导入，由本地优先 alias 命中应用配置；应用配置在模块加载时合并一次并返回单例对象。
- Manager、Agent、Merchant 覆盖分别包含 5、34、35 个 view key，静态校验确认所有 key 均存在于共享默认 schema；迁移后三个应用生产构建均通过，退出码均为 0。
- shared 中旧 `project-config.js` selector/全局覆盖表已删除；共享页面的 getter 改为无参数读取，44 个文件共清理 47 处旧 appId 实参。业务行为仍需要的直接 `__APP_ID__` 分支保留。

### 阶段 3：评估共享 alias 与包边界

**状态：已完成（保留自定义 alias，不迁移 workspace）**

**目标：** 判断自定义 `createSharedAlias` 是否已成为长期维护负担，以及是否有必要迁移到显式的 `@shared` / `@app` 路径或 workspace 包。

**评估依据：**

- alias 对本地优先覆盖、glob 导入、扩展名和目录入口的支持是否仍被实际依赖。
- 采用显式路径后需要改动的导入数量、风险和渐进迁移方式。
- 各应用依赖是否能统一管理，workspace 是否会解决真实问题，而非仅增加工具链复杂度。

**验收：**

- 输出继续保留或分阶段替换的决策与理由。
- 除非有独立收益和回滚方案，本阶段只记录评估结论，不执行全仓导入重写或 workspace 迁移。

**阶段 3 评估结果**

- 共享源码有 545 个 `@/` 导入，分布于 184 个文件；三个应用与 shared 的同路径源码碰撞分别为 Manager 1 个、Agent 1 个、Merchant 2 个。
- Merchant 的 `src/views/current/user-info-page.vue` 是真实本地实现，并由 `src/config/app-config.js` 动态路由使用；各应用 `main.js` 与共享入口也同名。删除本地优先语义会改变解析行为。
- 全量改写成显式 `@shared` 需要迁移数百个导入，风险和 diff 规模明显高于现阶段收益。三个应用依赖版本一致，但没有独立发布 shared 包的需求。
- 决策：保留 `createSharedAlias` 的本地优先/共享回退行为，不迁移 workspace；阶段 1 已把重复 Vite 设置集中，项目差异也已放回 app 配置，后续仅在出现明确维护收益时再评估 alias 简化。

### 阶段 4：整体验收与收尾

**状态：已完成（2026-10-09）**

**验收清单：**

- 三个应用分别执行 `npm run build`。
- 对涉及源码的应用执行 `npm run lint`；若 lint 命令不覆盖 `shared`，记录这一覆盖缺口，不伪称已检查。
- 启动并抽查 Manager、Agent、Merchant 的入口、路由加载、登录/权限相关状态及受差异配置影响的页面。
- 检查生产产物中无错误应用 ID、资源路径、PWA 或 chunk 命名变化。
- 检查最终 diff，仅包含本计划范围；检查文档索引和本计划状态同步。
- 记录剩余风险、明确不处理的问题和后续建议。

**阶段 4 当前结果**

- Manager、Agent、Merchant `npm run build` 和 `npm run lint` 均通过；三个 dev server 和对应 API 均运行于预期端口。
- 已按仓库 watch 任务启动 Manager、Agent、Merchant API，端口 `5817`、`5816`、`5818` 均有对应 API 进程监听；重载后登录页站点配置/验证码请求可用，验证码图片正常显示。
- 三端全量 lint 均已通过，`npm run lint` 退出码为 0。应用 `jsconfig.json` 的具名 `@/` 路径现统一为应用 `src` 优先、`shared` 兜底，并补齐 `@/assets/*` 映射。
- 开发态 bypass smoke 进入 Agent/Merchant 支付订单页时发现 `pay/detail-drawer.vue` 漏导 `getProjectConfig`，现已修复并完成三端 build 回归；Agent 显示支付接口筛选、Merchant 隐藏该筛选。
- 全仓 getter 导入扫描发现 10 个共享视图漏导入 `getProjectConfig`，已补齐；正常认证后 Manager 商户列表曾复现同一运行时错误，修复后页面数据显示正常。
- Merchant 商户应用列表的 `maSearchHidden/maColHidden` 遗留引用已删除，门店页缺失的 `msSearchHidden` 已补齐。Manager 商户列表、Agent/Merchant 支付订单、Merchant 商户信息/应用/门店页面均完成真实登录 smoke，无配置 getter ReferenceError；最后补丁后的三端 build/lint 均通过。
- 支付订单差异符合配置：Manager 退款操作被权限隐藏，Agent 显示支付接口筛选，Merchant 隐藏该筛选和列；门店/商户详情字段按应用配置呈现。
- 唯一残余异常为 Merchant 商户应用页选择器请求 `GET /api/mchInfo?pageNumber=1&pageSize=10&mchName=` 返回 404；应用列表本身仍显示记录，不阻断本计划验收。

## 6. 建议验证命令

在仓库根目录使用 PowerShell：

```powershell
Push-Location ant-design-vue3/agpay-ui-manager; npm run build; Pop-Location
Push-Location ant-design-vue3/agpay-ui-agent; npm run build; Pop-Location
Push-Location ant-design-vue3/agpay-ui-merchant; npm run build; Pop-Location
```

源码 lint 按相同方式对三个项目分别执行 `npm run lint`。命令失败时保留完整输出，并区分环境问题、基线问题和本次变更回归。

## 7. Agent 接续协议

任何接手 Agent 按以下顺序工作：

1. 阅读本文件及仓库 `AGENTS.md`；若修改 Vue3 前端代码，再阅读适用的 Vue3 instructions。
2. 执行 `git status --short`，检查当前阶段相关文件的 diff；未确认来源的变更视为用户改动并保留。
3. 查看第 8 节最近一条交接记录，确认当前阶段、已做事项、失败检查和下一步。
4. 开始操作前更新当前阶段状态；每个实质编辑后先做对应的窄验证。
5. 结束前更新交接记录和阶段状态；若未完成，明确写出恢复工作的第一步、所需上下文和阻塞原因。

不依赖对话历史、临时终端状态或 Agent 私有记忆来解释未完成工作。所有必要信息写回本文件或代码注释/测试中；不要把密钥、令牌或个人环境路径写入计划书。

## 8. 状态与交接记录

### 当前总状态

- 计划状态：阶段 0 至阶段 4 全部完成。
- 当前阶段：已完成；保留一个非阻断的 Merchant `/api/mchInfo` 404 跟进项。
- 最近验证：三端 `npm run build` 和 `npm run lint` 均通过；真实登录后 Manager/Agent/Merchant 配置页面 smoke 完成；共享 getter 运行时错误已修复。
- 下一个动作：如后续处理 Merchant 404，单独核对 `mch-app-page.vue` 调用与后端 `/api/mchInfo` 路由契约，不影响本计划完成。

- 修改文件：三个项目 jsconfig、ESLint 修复涉及的 `src` 文件、共享配置消费者导入、支付详情抽屉和商户应用列表、本计划书。
- 验证命令与结果：三应用最终 `npm run lint` 和 `npm run build` 均通过；全仓 getter 导入扫描完整；浏览器从真实菜单路由验证配置差异页面。
- 未解决 / 风险：Merchant 应用页选择器请求 `/api/mchInfo` 返回 404，但主列表显示数据，不阻断配置架构验收。
- 下一步第一动作：无本计划内必需动作；若处理 API 404，单独确认路由契约后再改。
- 工作区注意事项：不要覆盖既有后端/Vue2/Vite 改动；三个前端 dev server 当前运行在 8817、8816、8818。

#### 2026-10-09 | 阶段 2 配置边界整理
- 阶段：阶段 2；状态：已完成
- 本次完成：完成订单切片、死配置清理、项目配置迁入各应用本地入口、getter appId 实参清理，以及企业/码牌/统计/详情/表单覆盖审计；shared 现在只保留默认 schema 和合并工厂。
- 修改文件：`shared/config/project-config-core.js`、三个应用 `src/config/project-config.js`、相关共享订单/企业/码牌/表单/详情页面、本计划书。
- 验证命令与结果：最后改动后三个应用 `npm run build` 均通过（退出码 0）；应用覆盖 key 与默认 schema 校验通过；getter appId 实参、死配置和旧退款表达式无全仓残留；受影响文件无诊断。
- 未解决 / 风险：三端仍有基线 `ag-axios` 跨 chunk 循环依赖警告；`git diff --check` 仅报告 LF/CRLF 属性提示。
- 下一步第一动作：检查共享 alias 的路径碰撞与导入模式，据此记录保留或替换决策。
- 工作区注意事项：保留已有所有 Vite、Vue2 和后端改动。

### 最近交接记录

#### 2026-10-09 | 阶段 1 Vite 配置收敛
- 阶段：阶段 1；状态：已完成
- 本次完成：增加共享 Vite 配置工厂，三个应用通过本地插件注入；移除未使用的 `loadEnv` 与注释代理。
- 修改文件：`shared/config/create-vite-config.js`、三个应用的 `vite.config.js`、本计划书。
- 验证命令与结果：三个应用 `npm run build` 均通过，退出码均为 0；四个 JS 配置文件无 VS Code 诊断；应用 ID/端口核对通过。
- 未解决 / 风险：三端仍有基线存在的 `ag-axios` 跨 chunk 循环依赖警告；阶段 2 尚未开始。
- 下一步第一动作：审计 `project-config.js` 默认配置与三个应用覆盖项，映射到共享页面实际消费点和 UI 字段。
- 工作区注意事项：改造前已有的 Vite import 排序变化已保留；不要撤销其他既有工作区改动。

### 决策记录

| 日期 | 决策 | 理由 / 证据 | 影响阶段 |
|------|------|-------------|----------|
| 2026-10-09 | 保留三个独立应用和 `shared`；优先收敛重复 Vite 配置，随后整理差异配置 | 三应用共享大量页面实现，但应用入口与业务上下文独立；Vite 配置重复，项目差异表承担多类职责 | 1、2 |
| 2026-10-09 | 将 alias/workspace 调整设为评估项，不作为默认实施范围 | 自定义本地优先 alias 影响共享源码解析，全面替换涉及面尚未盘点 | 3 |
| 2026-10-09 | Vite 配置工厂不直接导入第三方插件，插件由应用侧实例化后注入 | 三个应用各自拥有 `node_modules`，目标插件版本一致；共享目录无法自然解析兄弟应用的依赖 | 1 |
| 2026-10-09 | 保留本地优先 alias，不迁移 workspace | 545 个共享 `@/` 导入、Merchant 本地页面覆盖和同名入口表明该解析语义仍在使用；全量改写成本高且无独立发布需求 | 3 |
| 2026-10-09 | jsconfig 具名 alias 使用应用 `src` 优先、shared 兜底 | 与 Vite `createSharedAlias` 解析顺序一致，避免编辑器把 `@/config/project-config` 指向旧 shared selector | 2、3 |

### 每次工作会话更新模板

复制以下模板并追加到本节顶部，保留历史记录：

```text
#### YYYY-MM-DD | Agent/会话标识
- 阶段：阶段 N；状态：进行中 / 已完成 / 阻塞
- 本次完成：
- 修改文件：
- 验证命令与结果：
- 未解决 / 风险：
- 下一步第一动作：
- 工作区注意事项：
```
