<template>
  <div class="float-input-demo">
    <a-space direction="vertical" style="width: 100%" :size="24">
      <!-- 组件概览 -->
      <AgCard title="浮动标签组件系列 ⭐">
        <a-alert
          message="统一的浮动标签设计"
          description="所有表单组件都支持浮动标签效果，提供一致的视觉体验和交互反馈"
          type="success"
          show-icon
          class="mb-4"
        />

        <a-descriptions :column="2" bordered>
          <a-descriptions-item label="AgInput">
            文本输入框 - 基础输入组件
          </a-descriptions-item>
          <a-descriptions-item label="AgInputNumber">
            数字输入框 - 支持步进、范围限制
          </a-descriptions-item>
          <a-descriptions-item label="AgTextarea">
            文本域 - 多行文本输入
          </a-descriptions-item>
          <a-descriptions-item label="AgSelect">
            下拉选择 - 单选/多选
          </a-descriptions-item>
          <a-descriptions-item label="AgDateRangePicker" :span="2">
            日期范围选择 - 开始日期到结束日期
          </a-descriptions-item>
        </a-descriptions>
      </AgCard>

      <!-- 完整表单示例 -->
      <AgCard title="完整表单示例 - 综合展示">
        <div class="form-demo">
          <a-form
            ref="formRef"
            :model="completeForm"
            :rules="rules"
            layout="vertical"
          >
            <a-row :gutter="16">
              <a-col :span="8">
                <a-form-item name="username">
                  <AgInput
                    v-model="completeForm.username"
                    label="用户名"
                    :required="true"
                    placeholder="请输入用户名"
                  >
                    <template #prefix>
                      <UserOutlined />
                    </template>
                  </AgInput>
                </a-form-item>
              </a-col>

              <a-col :span="8">
                <a-form-item name="age">
                  <AgInputNumber
                    v-model="completeForm.age"
                    label="年龄"
                    :required="true"
                    :min="0"
                    :max="150"
                    placeholder="请输入年龄"
                  />
                </a-form-item>
              </a-col>

              <a-col :span="8">
                <a-form-item name="gender">
                  <AgSelect
                    v-model="completeForm.gender"
                    label="性别"
                    :required="true"
                    :options="genderOptions"
                    placeholder="请选择性别"
                  />
                </a-form-item>
              </a-col>
            </a-row>

            <a-row :gutter="16">
              <a-col :span="12">
                <a-form-item name="dateRange">
                  <AgDateRangePicker
                    v-model:value="completeForm.dateRange"
                    label="在职时间"
                  />
                </a-form-item>
              </a-col>

              <a-col :span="12">
                <a-form-item name="email">
                  <AgInput
                    v-model="completeForm.email"
                    label="邮箱"
                    :required="true"
                    placeholder="请输入邮箱"
                  >
                    <template #prefix>
                      <MailOutlined />
                    </template>
                  </AgInput>
                </a-form-item>
              </a-col>
            </a-row>

            <a-row>
              <a-col :span="24">
                <a-form-item name="introduction">
                  <AgTextarea
                    v-model="completeForm.introduction"
                    label="个人简介"
                    :rows="4"
                    :maxlength="500"
                    :show-count="true"
                    placeholder="请输入个人简介"
                  />
                </a-form-item>
              </a-col>
            </a-row>

            <a-form-item>
              <a-space>
                <a-button type="primary" @click="handleCompleteSubmit">
                  提交
                </a-button>
                <a-button @click="handleCompleteReset">
                  重置
                </a-button>
                <a-button @click="handleFillDemo">
                  填充示例数据
                </a-button>
              </a-space>
            </a-form-item>
          </a-form>
        </div>
      </AgCard>

      <!-- AgInputNumber 示例 -->
      <AgCard title="AgInputNumber - 数字输入框">
        <a-row :gutter="24">
          <a-col :span="8">
            <AgInputNumber
              v-model="number1"
              label="数量"
              :min="0"
              :max="999"
              placeholder="请输入数量"
            />
            <div class="demo-desc">基础数字输入，限制范围 0-999</div>
            <div class="value-display">
              <a-tag color="blue">当前值: {{ number1 ?? '未设置' }}</a-tag>
            </div>
          </a-col>

          <a-col :span="8">
            <AgInputNumber
              v-model="number2"
              label="价格"
              :min="0"
              :step="0.01"
              :precision="2"
              placeholder="请输入价格"
            />
            <div class="demo-desc">保留2位小数，步进0.01</div>
            <div class="value-display">
              <a-tag color="green">当前值: {{ number2 ?? '未设置' }}</a-tag>
            </div>
          </a-col>

          <a-col :span="8">
            <AgInputNumber
              v-model="number3"
              label="百分比"
              :min="0"
              :max="100"
              :step="5"
              placeholder="0-100"
            />
            <div class="demo-desc">步进为5</div>
            <div class="value-display">
              <a-tag color="orange">当前值: {{ number3 ?? '未设置' }}%</a-tag>
            </div>
          </a-col>
        </a-row>
      </AgCard>

      <!-- AgInputNumberRange 示例 -->
      <AgCard title="AgInputNumberRange - 数字范围输入框 ⭐">
        <a-alert
          message="数字范围输入组件"
          description="用于输入最小值-最大值范围，自动验证最小值不大于最大值"
          type="info"
          show-icon
          class="mb-4"
        />

        <a-row :gutter="24">
          <a-col :span="8">
            <AgInputNumberRange
              v-model="range1"
              label="价格区间"
              :min="0"
              :max="10000"
              :placeholder="['最低价', '最高价']"
            />
            <div class="demo-desc">基础价格区间输入</div>
          </a-col>

          <a-col :span="8">
            <AgInputNumberRange
              v-model="range2"
              label="年龄段"
              :min="0"
              :max="150"
              :required="true"
              :placeholder="['最小年龄', '最大年龄']"
            />
            <div class="demo-desc">必填的年龄段输入</div>
          </a-col>

          <a-col :span="8">
            <AgInputNumberRange
              v-model="range3"
              label="温度范围"
              :min="-50"
              :max="50"
              :step="0.1"
              :precision="1"
              :placeholder="['最低温', '最高温']"
            />
            <div class="demo-desc">支持小数，精度为1位</div>
          </a-col>
        </a-row>

        <a-row :gutter="24" style="margin-top: 16px">
          <a-col :span="8">
            <AgInputNumberRange
              v-model="range4"
              label="金额范围"
              :min="0"
              :precision="2"
              :placeholder="['最小金额', '最大金额']"
            />
            <div class="demo-desc">金额输入，保留2位小数</div>
          </a-col>

          <a-col :span="8">
            <AgInputNumberRange
              v-model="range5"
              label="评分区间"
              :min="0"
              :max="100"
              :step="5"
            />
            <div class="demo-desc">步进为5的评分区间</div>
          </a-col>

          <a-col :span="8">
            <AgInputNumberRange
              v-model="range6"
              label="禁用状态"
              :disabled="true"
            />
            <div class="demo-desc">禁用的输入框</div>
          </a-col>
        </a-row>

        <a-divider />

        <div class="result-display">
          <h4>当前值：</h4>
          <pre>{{ {
            range1,
            range2,
            range3,
            range4,
            range5,
            range6
          } }}</pre>
        </div>
      </AgCard>

      <!-- Required 使用说明 -->
      <AgCard title="Required 属性使用说明 ⭐">
        <a-alert
          message="Required 属性说明"
          description="required 属性仅用于显示红色星号，不参与表单验证。验证逻辑需要在 form-item 的 rules 中定义。"
          type="info"
          show-icon
          class="mb-4"
        />

        <a-form :model="requiredForm" :rules="requiredRules" layout="vertical">
          <a-row :gutter="16">
            <a-col :span="8">
              <a-form-item name="requiredAge">
                <AgInputNumber
                  v-model="requiredForm.requiredAge"
                  label="年龄"
                  :required="true"
                  placeholder="必填项（显示星号）"
                />
              </a-form-item>
              <div class="demo-desc">✅ 设置 :required="true" 显示星号</div>
            </a-col>

            <a-col :span="8">
              <a-form-item name="optionalSalary">
                <AgInputNumber
                  v-model="requiredForm.optionalSalary"
                  label="薪资"
                  placeholder="选填项（无星号）"
                />
              </a-form-item>
              <div class="demo-desc">❌ 不设置 required，无星号</div>
            </a-col>

            <a-col :span="8">
              <a-form-item name="displayOnlyBonus">
                <AgInputNumber
                  v-model="requiredForm.displayOnlyBonus"
                  label="奖金"
                  :required="true"
                  placeholder="仅显示星号，不验证"
                />
              </a-form-item>
              <div class="demo-desc">⚠️ 有星号但 rules 无 required</div>
            </a-col>
          </a-row>

          <a-form-item>
            <a-space>
              <a-button type="primary" @click="handleRequiredSubmit">
                验证表单
              </a-button>
              <a-button @click="handleRequiredReset">
                重置
              </a-button>
            </a-space>
          </a-form-item>
        </a-form>

        <a-divider />

        <div class="code-example">
          <h4>推荐用法：</h4>
          <pre><code>{{ requiredExampleCode }}</code></pre>
        </div>
      </AgCard>

      <!-- AgSelect 示例 -->
      <AgCard title="AgSelect - 下拉选择">
        <a-row :gutter="24">
          <a-col :span="8">
            <AgSelect
              v-model="select1"
              label="城市"
              :options="cityOptions"
              placeholder="请选择城市"
            />
            <div class="demo-desc">单选下拉</div>
            <div class="value-display">
              <a-tag color="blue">{{ select1 ? `已选择: ${getCityLabel(select1)}` : '未选择' }}</a-tag>
            </div>
          </a-col>

          <a-col :span="8">
            <AgSelect
              v-model="select2"
              label="爱好"
              mode="multiple"
              :options="hobbyOptions"
              placeholder="请选择爱好"
            />
            <div class="demo-desc">多选下拉</div>
            <div class="value-display">
              <a-tag v-if="select2.length === 0" color="default">未选择</a-tag>
              <a-tag v-else color="green">已选择 {{ select2.length }} 项</a-tag>
              <a-tag v-for="item in select2" :key="item" color="blue" style="margin-left: 4px">
                {{ getHobbyLabel(item) }}
              </a-tag>
            </div>
          </a-col>

          <a-col :span="8">
            <AgSelect
              v-model="select3"
              label="状态"
              :options="statusOptions"
              :required="true"
              placeholder="请选择状态"
            />
            <div class="demo-desc">必选项</div>
            <div class="value-display">
              <a-tag :color="select3 === 1 ? 'success' : select3 === 0 ? 'error' : 'default'">
                {{ select3 !== undefined ? getStatusLabel(select3) : '未选择' }}
              </a-tag>
            </div>
          </a-col>
        </a-row>
      </AgCard>

      <!-- AgDateRangePicker 示例 -->
      <AgCard title="AgDateRangePicker - 高级日期范围选择器 ⭐⭐⭐">
        <a-alert
          message="新版组件 AgDateRangePicker"
          description="功能更强大：支持可选的快捷选择、字符串/数组返回值、智能格式化等。推荐在新项目中使用。"
          type="success"
          show-icon
          class="mb-4"
        />

        <a-tabs>
          <!-- Tab 1: 快捷选择模式 -->
          <a-tab-pane key="1" tab="📋 快捷选择">
            <a-row :gutter="24">
              <a-col :span="12">
                <AgDateRangePicker
                  v-model:value="agDate1"
                  label="搜索时间（字符串）"
                />
                <div class="demo-desc">✅ 自动识别：返回字符串（根据初始值）</div>
                <div class="value-display">
                  <a-tag v-if="agDate1" color="blue">{{ agDate1 }}</a-tag>
                  <a-tag v-else color="default">未选择</a-tag>
                </div>
              </a-col>

              <a-col :span="12">
                <AgDateRangePicker
                  v-model:value="agDate2"
                  label="统计时间（数组）"
                />
                <div class="demo-desc">✅ 自动识别：返回数组（根据初始值）</div>
                <div class="value-display">
                  <a-tag v-if="agDate2 && agDate2.length === 2" color="green">
                    {{ agDate2[0] }} ~ {{ agDate2[1] }}
                  </a-tag>
                  <a-tag v-else color="default">未选择</a-tag>
                </div>
              </a-col>
            </a-row>
            
            <a-divider>手动指定 valueType（可选）</a-divider>
            
            <a-row :gutter="24">
              <a-col :span="12">
                <AgDateRangePicker
                  v-model:value="agDate7"
                  label="强制字符串"
                  value-type="string"
                />
                <div class="demo-desc">⚙️ 手动指定 value-type="string"</div>
                <div class="value-display">
                  <a-tag v-if="agDate7" color="purple">{{ agDate7 }}</a-tag>
                  <a-tag v-else color="default">未选择</a-tag>
                </div>
              </a-col>

              <a-col :span="12">
                <AgDateRangePicker
                  v-model:value="agDate8"
                  label="强制数组"
                  value-type="array"
                />
                <div class="demo-desc">⚙️ 手动指定 value-type="array"</div>
                <div class="value-display">
                  <a-tag v-if="agDate8 && agDate8.length === 2" color="orange">
                    {{ agDate8[0] }} ~ {{ agDate8[1] }}
                  </a-tag>
                  <a-tag v-else color="default">未选择</a-tag>
                </div>
              </a-col>
            </a-row>
          </a-tab-pane>

          <!-- Tab 2: 直接选择模式 -->
          <a-tab-pane key="2" tab="📅 直接选择">
            <a-row :gutter="24">
              <a-col :span="12">
                <AgDateRangePicker
                  v-model:value="agDate3"
                  label="活动时间"
                  :show-quick-select="false"
                  :required="true"
                />
                <div class="demo-desc">⭐ 自动识别：数组返回值</div>
                <div class="value-display">
                  <a-tag v-if="agDate3 && agDate3.length === 2" color="success">
                    {{ agDate3[0] }} ~ {{ agDate3[1] }}
                  </a-tag>
                  <a-tag v-else color="default">未选择</a-tag>
                </div>
              </a-col>

              <a-col :span="12">
                <AgDateRangePicker
                  v-model:value="agDate4"
                  label="日期范围"
                  :show-quick-select="false"
                />
                <div class="demo-desc">自动识别：字符串返回值</div>
                <div class="value-display">
                  <a-tag v-if="agDate4" color="blue">{{ agDate4 }}</a-tag>
                  <a-tag v-else color="default">未选择</a-tag>
                </div>
              </a-col>
            </a-row>
          </a-tab-pane>

          <!-- Tab 3: 带时间 -->
          <a-tab-pane key="3" tab="⏰ 带时间">
            <a-row :gutter="24">
              <a-col :span="12">
                <AgDateRangePicker
                  v-model:value="agDate5"
                  label="精确时间（秒）"
                  :show-time="true"
                  :show-quick-select="false"
                />
                <div class="demo-desc">HH:mm:ss - 自动识别数组</div>
                <div class="value-display">
                  <a-tag v-if="agDate5 && agDate5.length === 2" color="purple">
                    {{ agDate5[0] }}<br />{{ agDate5[1] }}
                  </a-tag>
                  <a-tag v-else color="default">未选择</a-tag>
                </div>
              </a-col>

              <a-col :span="12">
                <AgDateRangePicker
                  v-model:value="agDate6"
                  label="精确时间（分）"
                  :show-time="true"
                  format="YYYY-MM-DD HH:mm"
                  :show-quick-select="false"
                />
                <div class="demo-desc">HH:mm - 自动识别数组</div>
                <div class="value-display">
                  <a-tag v-if="agDate6 && agDate6.length === 2" color="cyan">
                    {{ agDate6[0] }}<br />{{ agDate6[1] }}
                  </a-tag>
                  <a-tag v-else color="default">未选择</a-tag>
                </div>
              </a-col>
            </a-row>
          </a-tab-pane>
        </a-tabs>
      </AgCard>

      <!-- 动态报表选择 Demo -->
      <AgCard title="动态报表选择 - 实战案例 ⭐⭐⭐">
        <a-alert
          message="动态控制日期选择器"
          description="根据报表类型（日报、周报、月报）动态切换日期选择面板，提供更符合业务场景的选择体验"
          type="info"
          show-icon
          class="mb-4"
        />

        <div class="report-demo">
          <a-form layout="inline" style="margin-bottom: 24px">
            <a-form-item label="">
              <AgSelect
                v-model:value="reportType"
                style="width: 150px"
                :options="reportTypeOptions"
                label="报表类型"
                @change="handleReportTypeChange"
              />
            </a-form-item>

            <a-form-item label="">
              <AgDateRangePicker
                :key="`date-picker-${reportType}`"
                v-model:value="reportDate"
                :picker="reportPickerType"
                :format="reportFormat"
                style="width: 300px"
                label="选择时间"
              />
            </a-form-item>

            <a-form-item>
              <a-button type="primary" @click="handleGenerateReport">
                生成报表
              </a-button>
            </a-form-item>
          </a-form>

          <a-divider>选择结果</a-divider>

          <a-descriptions bordered :column="3">
            <a-descriptions-item label="报表类型">
              <a-tag :color="reportTypeColor">{{ reportTypeLabel }}</a-tag>
            </a-descriptions-item>
            <a-descriptions-item label="Picker类型">
              <a-tag color="blue">{{ reportPickerType }}</a-tag>
            </a-descriptions-item>
            <a-descriptions-item label="显示格式">
              <a-tag color="cyan">{{ reportFormat }}</a-tag>
            </a-descriptions-item>
            <a-descriptions-item label="选择的时间" :span="3">
              <a-tag v-if="reportDate && reportDate.length === 2" color="green">
                {{ reportDate[0] }} ~ {{ reportDate[1] }}
              </a-tag>
              <a-tag v-else color="default">未选择</a-tag>
            </a-descriptions-item>
            <a-descriptions-item label="格式化显示" :span="3">
              <div v-if="reportDate && reportDate.length === 2" class="formatted-date">
                <strong>{{ formatReportDate(reportDate) }}</strong>
              </div>
              <span v-else style="color: #999">未选择时间</span>
            </a-descriptions-item>
          </a-descriptions>

          <a-divider>使用说明</a-divider>

          <a-row :gutter="16">
            <a-col :span="8">
              <div class="tip-card">
                <div class="tip-title">📅 日报</div>
                <div class="tip-content">
                  <p>选择器类型：<code>date</code></p>
                  <p>显示格式：<code>YYYY-MM-DD</code></p>
                  <p>选择精度：按日选择</p>
                  <p>适用场景：每日数据统计</p>
                </div>
              </div>
            </a-col>

            <a-col :span="8">
              <div class="tip-card">
                <div class="tip-title">📊 周报</div>
                <div class="tip-content">
                  <p>选择器类型：<code>week</code></p>
                  <p>显示格式：<code>YYYY-wo</code></p>
                  <p>选择精度：按周选择</p>
                  <p>适用场景：周统计分析</p>
                </div>
              </div>
            </a-col>

            <a-col :span="8">
              <div class="tip-card">
                <div class="tip-title">📈 月报</div>
                <div class="tip-content">
                  <p>选择器类型：<code>month</code></p>
                  <p>显示格式：<code>YYYY-MM</code></p>
                  <p>选择精度：按月选择</p>
                  <p>适用场景：月度汇总</p>
                </div>
              </div>
            </a-col>
          </a-row>

          <a-row :gutter="16" style="margin-top: 16px">
            <a-col :span="12">
              <div class="tip-card">
                <div class="tip-title">📉 季报</div>
                <div class="tip-content">
                  <p>选择器类型：<code>quarter</code></p>
                  <p>显示格式：<code>YYYY-[Q]Q</code></p>
                  <p>选择精度：按季度选择</p>
                  <p>适用场景：季度报表</p>
                </div>
              </div>
            </a-col>

            <a-col :span="12">
              <div class="tip-card">
                <div class="tip-title">📊 年报</div>
                <div class="tip-content">
                  <p>选择器类型：<code>year</code></p>
                  <p>显示格式：<code>YYYY</code></p>
                  <p>选择精度：按年选择</p>
                  <p>适用场景：年度总结</p>
                </div>
              </div>
            </a-col>
          </a-row>

          <a-divider />

          <div class="code-example">
            <h4>💡 实现代码：</h4>
            <pre><code>{{ reportExampleCode }}</code></pre>
          </div>
        </div>
      </AgCard>

      <!-- AgTextarea 示例 -->
      <AgCard title="AgTextarea - 文本域">
        <a-row :gutter="24">
          <a-col :span="12">
            <AgTextarea
              v-model="textarea1"
              label="备注"
              :rows="4"
              placeholder="请输入备注信息"
            />
            <div class="demo-desc">基础文本域，4行</div>
            <div class="value-display">
              <a-tag color="blue">
                {{ textarea1 ? `已输入 ${textarea1.length} 字` : '未输入' }}
              </a-tag>
            </div>
          </a-col>

          <a-col :span="12">
            <AgTextarea
              v-model="textarea2"
              label="详细描述"
              :rows="4"
              :maxlength="200"
              :show-count="true"
              placeholder="最多200字"
            />
            <div class="demo-desc">限制长度，显示字数</div>
            <div class="value-display">
              <a-tag :color="textarea2.length > 150 ? 'warning' : 'green'">
                {{ textarea2.length }}/200 字
              </a-tag>
            </div>
          </a-col>
        </a-row>
      </AgCard>

      <!-- AgInput 基础用法 -->
      <AgCard title="AgInput - 基础用法">
        <a-row :gutter="24">
          <a-col :span="8">
            <AgInput
              v-model="basic1"
              label="用户名"
            />
            <div class="demo-desc">无 placeholder，默认状态</div>
            <div class="value-display">
              <a-tag color="blue">{{ basic1 || '未输入' }}</a-tag>
            </div>
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="basic2"
              label="邮箱地址"
              placeholder="请输入邮箱"
            />
            <div class="demo-desc">有 placeholder，标签始终上浮</div>
            <div class="value-display">
              <a-tag color="green">{{ basic2 || '未输入' }}</a-tag>
            </div>
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="basic3"
              label="手机号码"
              :maxlength="11"
            />
            <div class="demo-desc">限制最大长度</div>
            <div class="value-display">
              <a-tag :color="basic3.length === 11 ? 'success' : 'default'">
                {{ basic3 ? `${basic3.length}/11` : '未输入' }}
              </a-tag>
            </div>
          </a-col>
        </a-row>
      </AgCard>

      <!-- 必填标识 -->
      <AgCard title="必填标识">
        <a-row :gutter="24">
          <a-col :span="8">
            <AgInput
              v-model="required1"
              label="姓名"
              :required="true"
              placeholder="请输入姓名"
            />
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="required2"
              label="身份证号"
              :required="true"
              :maxlength="18"
            />
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="required3"
              label="联系电话"
              :required="true"
              placeholder="11位手机号"
              :maxlength="11"
            />
          </a-col>
        </a-row>
      </AgCard>

      <!-- 不同类型 -->
      <AgCard title="不同输入类型">
        <a-row :gutter="24">
          <a-col :span="8">
            <AgInput
              v-model="type1"
              label="文本输入"
              type="text"
              placeholder="text 类型"
            />
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="type2"
              label="密码输入"
              type="password"
              placeholder="password 类型"
            />
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="type3"
              label="邮箱输入"
              type="email"
              placeholder="email 类型"
            />
          </a-col>
        </a-row>
      </AgCard>

      <!-- 带图标 -->
      <AgCard title="带图标">
        <a-row :gutter="24">
          <a-col :span="8">
            <AgInput
              v-model="icon1"
              label="用户名"
              placeholder="请输入用户名"
            >
              <template #prefix>
                <UserOutlined style="color: rgba(0,0,0,.25)" />
              </template>
            </AgInput>
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="icon2"
              label="搜索"
              placeholder="输入关键字"
            >
              <template #suffix>
                <SearchOutlined style="color: rgba(0,0,0,.25)" />
              </template>
            </AgInput>
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="icon3"
              label="邮箱"
              placeholder="请输入邮箱"
            >
              <template #prefix>
                <MailOutlined style="color: rgba(0,0,0,.25)" />
              </template>
            </AgInput>
          </a-col>
        </a-row>
      </AgCard>

      <!-- 清除按钮 -->
      <AgCard title="清除按钮">
        <a-row :gutter="24">
          <a-col :span="8">
            <AgInput
              v-model="clear1"
              label="可清除输入"
              :allow-clear="true"
              placeholder="输入内容后显示清除按钮"
            />
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="clear2"
              label="搜索关键字"
              :allow-clear="true"
              placeholder="搜索"
            >
              <template #suffix>
                <SearchOutlined />
              </template>
            </AgInput>
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="clear3"
              label="订单号"
              :allow-clear="true"
            />
          </a-col>
        </a-row>
      </AgCard>

      <!-- 不同尺寸 -->
      <AgCard title="不同尺寸">
        <a-row :gutter="24">
          <a-col :span="8">
            <AgInput
              v-model="size1"
              label="小尺寸"
              size="small"
              placeholder="small"
            />
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="size2"
              label="中等尺寸"
              size="middle"
              placeholder="middle (默认)"
            />
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="size3"
              label="大尺寸"
              size="large"
              placeholder="large"
            />
          </a-col>
        </a-row>
      </AgCard>

      <!-- 禁用状态 -->
      <AgCard title="禁用状态">
        <a-row :gutter="24">
          <a-col :span="8">
            <AgInput
              v-model="disabled1"
              label="禁用输入"
              :disabled="true"
            />
          </a-col>

          <a-col :span="8">
            <AgInput
              v-model="disabled2"
              label="禁用(有值)"
              :disabled="true"
            />
          </a-col>

          <a-col :span="8">
            <AgInput
              label="禁用(无值)"
              :disabled="true"
            />
          </a-col>
        </a-row>
      </AgCard>
    </a-space>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { message } from 'ant-design-vue'
import {
  UserOutlined,
  MailOutlined,
  SearchOutlined
} from '@ant-design/icons-vue'
import {
  AgCard,
  AgInput,
  AgInputNumber,
  AgInputNumberRange,
  AgDateRangePicker,
  AgTextarea,
  AgSelect
} from '@/components'

// AgDateRangePicker 示例数据（新版）
const agDate1 = ref('')
const agDate2 = ref([])
const agDate3 = ref([])
const agDate4 = ref('')
const agDate5 = ref([])
const agDate6 = ref([])
const agDate7 = ref('')
const agDate8 = ref('')
const agDate9 = ref('')

// 动态报表选择
const reportType = ref('daily')
const reportDate = ref([])
const reportPickerType = ref('date')

const reportTypeOptions = [
  { label: '日报', value: 'daily' },
  { label: '周报', value: 'weekly' },
  { label: '月报', value: 'monthly' },
  { label: '季报', value: 'quarterly' },
  { label: '年报', value: 'yearly' }
]

const reportTypeColor = computed(() => {
  const colors = {
    daily: 'blue',
    weekly: 'green',
    monthly: 'orange',
    quarterly: 'purple',
    yearly: 'red'
  }
  return colors[reportType.value]
})

const reportTypeLabel = computed(() => {
  const labels = {
    daily: '日报',
    weekly: '周报',
    monthly: '月报',
    quarterly: '季报',
    yearly: '年报'
  }
  return labels[reportType.value]
})

const reportFormat = computed(() => {
  const formatMap = {
    daily: 'YYYY-MM-DD',
    weekly: 'YYYY-wo',
    monthly: 'YYYY-MM',
    quarterly: 'YYYY-[Q]Q',
    yearly: 'YYYY'
  }
  return formatMap[reportType.value]
})

function handleReportTypeChange(value) {
  // 确保 reportType 立即更新（虽然 v-model 应该自动更新，但为了保险起见）
  reportType.value = value
  
  // 根据报表类型设置对应的选择器类型
  const pickerMap = {
    daily: 'date',
    weekly: 'week',
    monthly: 'month',
    quarterly: 'quarter',
    yearly: 'year'
  }
  reportPickerType.value = pickerMap[value]
  
  // 清空已选择的日期
  reportDate.value = []
  
  message.info(`已切换到${reportTypeLabel.value}模式`)
}

function formatReportDate(dates) {
  if (!dates || dates.length !== 2) return ''
  
  const formatMap = {
    daily: '日期范围',
    weekly: '周范围',
    monthly: '月份范围',
    quarterly: '季度范围',
    yearly: '年份范围'
  }
  
  return `${formatMap[reportType.value]}: ${dates[0]} 至 ${dates[1]}`
}

function handleGenerateReport() {
  if (!reportDate.value || reportDate.value.length === 0) {
    message.warning('请先选择时间范围')
    return
  }
  
  console.log('生成报表参数:', {
    type: reportType.value,
    dateRange: reportDate.value,
    picker: reportPickerType.value
  })
  
  message.success(`正在生成${reportTypeLabel.value}...`)
}

const reportExampleCode = `<template>
  <a-form layout="inline">
    <a-form-item label="报表类型">
      <AgSelect v-model:value="reportType" @change="handleReportTypeChange">
        <a-select-option value="daily">日报</a-select-option>
        <a-select-option value="weekly">周报</a-select-option>
        <a-select-option value="monthly">月报</a-select-option>
        <a-select-option value="quarterly">季报</a-select-option>
        <a-select-option value="yearly">年报</a-select-option>
      </AgSelect>
    </a-form-item>

    <a-form-item label="选择时间">
      <AgDateRangePicker
        v-model:value="reportDate"
        :picker="reportPickerType"
        :format="reportFormat"
        :show-quick-select="false"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
import { ref, computed } from 'vue'

const reportType = ref('daily')
const reportDate = ref([])
const reportPickerType = ref('date')

// 根据报表类型返回对应的日期格式
const reportFormat = computed(() => {
  const formatMap = {
    daily: 'YYYY-MM-DD',
    weekly: 'YYYY-wo',      // 周
    monthly: 'YYYY-MM',     // 月
    quarterly: 'YYYY-[Q]Q', // 季度
    yearly: 'YYYY'          // 年
  }
  return formatMap[reportType.value]
})

function handleReportTypeChange(value) {
  const pickerMap = {
    daily: 'date',
    weekly: 'week',
    monthly: 'month',
    quarterly: 'quarter',
    yearly: 'year'
  }
  reportPickerType.value = pickerMap[value]
  reportDate.value = []  // 清空日期
}
<\/script>`

const formRef = ref()
const completeForm = reactive({
  username: '',
  age: undefined,
  gender: undefined,
  dateRange: '',
  email: '',
  introduction: ''
})

const rules = {
  username: [
    { required: true, message: '请输入用户名' },
    { min: 3, max: 20, message: '用户名长度3-20位' }
  ],
  age: [{ required: true, message: '请输入年龄' }],
  gender: [{ required: true, message: '请选择性别' }],
  email: [
    { required: true, message: '请输入邮箱' },
    { type: 'email', message: '邮箱格式不正确' }
  ]
}

const genderOptions = [
  { label: '男', value: 'male' },
  { label: '女', value: 'female' },
  { label: '其他', value: 'other' }
]

async function handleCompleteSubmit() {
  try {
    await formRef.value.validate()
    console.log('表单数据:', completeForm)
    message.success('提交成功')
  } catch (error) {
    message.error('请检查表单')
  }
}

function handleCompleteReset() {
  formRef.value.resetFields()
  message.info('已重置')
}

function handleFillDemo() {
  completeForm.username = 'zhangsan'
  completeForm.age = 28
  completeForm.gender = 'male'
  completeForm.dateRange = 'near30'
  completeForm.email = 'zhangsan@example.com'
  completeForm.introduction = '这是一段个人简介示例文本，展示文本域的使用效果。'
  message.success('已填充示例数据')
}

// AgInputNumber
const number1 = ref(undefined)
const number2 = ref(undefined)
const number3 = ref(undefined)

// AgInputNumberRange
const range1 = ref([undefined, undefined])
const range2 = ref([undefined, undefined])
const range3 = ref([undefined, undefined])
const range4 = ref([undefined, undefined])
const range5 = ref([undefined, undefined])
const range6 = ref([10, 20])

// Required 使用说明
const requiredForm = reactive({
  requiredAge: undefined,
  optionalSalary: undefined,
  displayOnlyBonus: undefined
})

const requiredRules = {
  requiredAge: [
    { required: true, message: '请输入年龄', type: 'number' },
    { type: 'number', min: 0, max: 150, message: '年龄范围0-150' }
  ],
  optionalSalary: [
    { type: 'number', min: 0, message: '薪资不能为负数' }
    // 注意：没有 required，但组件也不显示星号
  ],
  // displayOnlyBonus 没有 rules，但组件设置了 :required="true"
  // 这会显示星号，但不会验证
}

const requiredExampleCode = `<a-form :model="form" :rules="rules">
  <a-form-item name="age">
    <AgInputNumber
      v-model="form.age"
      label="年龄"
      :required="true"
      :min="0"
      :max="150"
    />
  </a-form-item>
</a-form>

const rules = {
  age: [
    { required: true, message: '请输入年龄' }
  ]
}`

async function handleRequiredSubmit() {
  try {
    await formRef.value.validate()
    message.success('验证通过！')
    console.log('表单数据:', requiredForm)
  } catch (error) {
    message.error('请填写必填项')
    console.log('验证失败:', error)
  }
}

function handleRequiredReset() {
  formRef.value.resetFields()
  message.info('已重置')
}

// AgSelect
const select1 = ref(undefined)
const select2 = ref([])
const select3 = ref(undefined)

const cityOptions = [
  { label: '北京', value: 'beijing' },
  { label: '上海', value: 'shanghai' },
  { label: '广州', value: 'guangzhou' },
  { label: '深圳', value: 'shenzhen' }
]

const hobbyOptions = [
  { label: '阅读', value: 'reading' },
  { label: '运动', value: 'sports' },
  { label: '音乐', value: 'music' },
  { label: '旅游', value: 'travel' }
]

const statusOptions = [
  { label: '启用', value: 1 },
  { label: '禁用', value: 0 }
]

// AgTextarea
const textarea1 = ref('')
const textarea2 = ref('')

// AgInput 基础用法
const basic1 = ref('')
const basic2 = ref('')
const basic3 = ref('')

// 必填标识
const required1 = ref('')
const required2 = ref('')
const required3 = ref('')

// 不同类型
const type1 = ref('')
const type2 = ref('')
const type3 = ref('')

// 带图标
const icon1 = ref('')
const icon2 = ref('')
const icon3 = ref('')

// 清除按钮
const clear1 = ref('')
const clear2 = ref('')
const clear3 = ref('')

// 不同尺寸
const size1 = ref('')
const size2 = ref('')
const size3 = ref('')

// 禁用状态
const disabled1 = ref('禁用状态')
const disabled2 = ref('有值的禁用状态')

// 辅助函数
function getCityLabel(value) {
  const city = cityOptions.find(item => item.value === value)
  return city ? city.label : value
}

function getHobbyLabel(value) {
  const hobby = hobbyOptions.find(item => item.value === value)
  return hobby ? hobby.label : value
}

function getStatusLabel(value) {
  const status = statusOptions.find(item => item.value === value)
  return status ? status.label : value
}

function formatDateRange(range) {
  if (!range || range.length !== 2) return '未选择'
  return `${range[0]} ~ ${range[1]}`
}

</script>

<style scoped>
.float-input-demo {
  /* padding: 24px; */
}

.mb-4 {
  margin-bottom: 16px;
}

.demo-desc {
  margin-top: 8px;
  font-size: 12px;
  color: rgba(0, 0, 0, 0.45);
}

.value-display {
  margin-top: 8px;
  min-height: 24px;
}

.value-display .ant-tag {
  margin: 2px 0;
}

.usage-hint {
  margin-top: 16px;
  padding: 12px;
  background: #e6f7ff;
  border-left: 3px solid #1890ff;
  border-radius: 4px;
}

.usage-hint strong {
  color: #1890ff;
}

.usage-tips {
  margin-top: 16px;
}

.usage-tips h4 {
  margin: 0 0 16px 0;
  font-size: 16px;
  color: #333;
}

.usage-tips ul {
  margin: 0;
  padding-left: 20px;
}

.usage-tips li {
  margin: 8px 0;
  line-height: 1.8;
}

.usage-tips code {
  padding: 2px 6px;
  background: #f5f5f5;
  border: 1px solid #d9d9d9;
  border-radius: 3px;
  font-family: 'Consolas', 'Monaco', monospace;
  font-size: 12px;
}

.tip-card {
  padding: 16px;
  /* background: #fafafa; */
  border: 1px solid #e8e8e8;
  border-radius: 4px;
  height: 100%;
}

.tip-title {
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 12px;
  color: #1890ff;
}

.tip-content {
  margin-bottom: 8px;
  line-height: 1.8;
}

.tip-content code {
  display: block;
  padding: 2px 6px;
  background: #fff;
  border: 1px solid #d9d9d9;
  border-radius: 3px;
  font-family: 'Consolas', 'Monaco', monospace;
  font-size: 11px;
  margin: 4px 0;
}

.tip-desc {
  font-size: 12px;
  color: #666;
  line-height: 1.6;
}

.form-demo {
  padding: 24px;
  /* background: #fafafa; */
  border-radius: 4px;
}

.code-example {
  margin-top: 16px;
  padding: 16px;
  background: #f5f5f5;
  border-radius: 4px;
}

.code-example h4 {
  margin: 0 0 12px 0;
  font-size: 14px;
  font-weight: 600;
}

.code-example pre {
  margin: 0;
  padding: 12px;
  background: #ffffff;
  border: 1px solid #d9d9d9;
  border-radius: 2px;
  overflow-x: auto;
}

.code-example code {
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
  font-size: 12px;
  line-height: 1.6;
  color: #333;
}

.result-display {
  margin-top: 16px;
  padding: 16px;
  background: #f5f5f5;
  border-radius: 4px;
}

.result-display h4 {
  margin: 0 0 12px 0;
  font-size: 14px;
  font-weight: 600;
}

.result-display pre {
  margin: 0;
  padding: 12px;
  background: #ffffff;
  border: 1px solid #d9d9d9;
  border-radius: 2px;
  overflow-x: auto;
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
  font-size: 12px;
  line-height: 1.6;
}

.report-demo {
  padding: 24px;
  /* background: #fafafa; */
  border-radius: 4px;
}

.formatted-date {
  font-size: 14px;
  color: #52c41a;
  padding: 8px 12px;
  background: #f6ffed;
  border: 1px solid #b7eb8f;
  border-radius: 4px;
  display: inline-block;
}



</style>



