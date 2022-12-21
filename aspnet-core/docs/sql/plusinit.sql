-- 商户门店表
DROP TABLE IF EXISTS `t_mch_store`;
CREATE TABLE `t_mch_store` (
  `store_id` VARCHAR(64) NOT NULL COMMENT '门店ID',
  `store_name` VARCHAR(64) NOT NULL DEFAULT '' COMMENT '门店名称',
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `contact_phone` VARCHAR(32) NOT NULL COMMENT '联系人电话',
  `store_logo` VARCHAR(64) NOT NULL COMMENT '门店LOGO',
  `store_outer_img` VARCHAR(64) NOT NULL COMMENT '门头照',
  `store_inner_img` VARCHAR(64) NOT NULL COMMENT '门店内景照',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `province_code` VARCHAR(32) NOT NULL COMMENT '省代码'
  `city_code` VARCHAR(32) NOT NULL COMMENT '市代码'
  `area_code` VARCHAR(32) NOT NULL COMMENT '区代码'
  `address` VARCHAR(128) NOT NULL COMMENT '详细地址',
  `lng` VARCHAR(32) NOT NULL COMMENT '经度'
  `lat` VARCHAR(32) NOT NULL COMMENT '纬度'
  `default_flag` TINYINT NOT NULL DEFAULT '0' COMMENT '是否默认: 0-否, 1-是',
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`app_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='商户门店表';

-- 商户进件表
DROP TABLE IF EXISTS `t_mch_apply`;
CREATE TABLE `t_mch_apply` (
  `apply_id` VARCHAR(64) NOT NULL COMMENT '进件单号',
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `if_code` VARCHAR(20) NOT NULL COMMENT '接口代码 全小写  wxpay alipay ',
  `apply_page_type` VARCHAR(20) NOT NULL COMMENT '来源: PLATFORM_WEB-运营平台(WEB) AGENT_WEB-代理商(WEB) MCH_WEB-商户(WEB) AGENT_LITE-代理商(小程序) MCH_LITE-商户(小程序)',
  `apply_detail_info` VARCHAR(255) NOT NULL COMMENT '申请详细信息', 
  `apply_error_info` VARCHAR(255) NOT NULL COMMENT '申请错误信息', 
  `state` TINYINT NOT NULL DEFAULT '0' COMMENT '状态: 0-草稿, 1-审核中, 2-进件成功, 3-驳回待修改, 4-待验证, 5-待签约', 
  `is_temp_data` BOOLEAN NOT NULL COMMENT '是否临时数据', 
  `mch_full_name` VARCHAR(64) NOT NULL COMMENT '商户名称全称', 
  `mch_short_name` VARCHAR(32) NOT NULL COMMENT '进件商户简称', 
  `merchant_type` TINYINT NOT NULL DEFAULT '0' COMMENT '商户类型: 1-个人, 2-个体工商户, 3-企业', 
  `contact_name` VARCHAR(32) NOT NULL COMMENT '商户联系人姓名', 
  `contact_phone` VARCHAR(32) NOT NULL COMMENT '商户联系人电话', 
  `contact_email` VARCHAR(32) NULL COMMENT '商户联系人邮箱', 
  `province_code` VARCHAR(32) NOT NULL COMMENT '省代码'
  `city_code` VARCHAR(32) NOT NULL COMMENT '市代码'
  `area_code` VARCHAR(32) NOT NULL COMMENT '区代码'
  `address` VARCHAR(128) NOT NULL COMMENT '商户详细地址',
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (apply_id)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='商户进件申请表';

-- 代理商信息表
DROP TABLE IF EXISTS `t_agent_info`;
CREATE TABLE `t_agent_info` (
  `agent_no` VARCHAR(64) NOT NULL COMMENT '代理商号',
  `agent_name` VARCHAR(64) NOT NULL COMMENT '代理商名称',
  `agent_short_name` VARCHAR(32) NOT NULL COMMENT '代理商简称',
  `agent_type` TINYINT NOT NULL DEFAULT '1' COMMENT '代理商类型: 1-个人, 2-企业',
  `level` TINYINT NOT NULL DEFAULT '1' COMMENT '等级: 1-一级, 2-二级, 3-三级 ...',
  `pid` VARCHAR(64) COMMENT '上级代理商号', 
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `contact_name` VARCHAR(32) NOT NULL COMMENT '联系人姓名',
  `contact_tel` VARCHAR(32) NOT NULL COMMENT '联系人手机号',
  `contact_email` VARCHAR(32) NULL COMMENT '联系人邮箱',
  `add_agent_flag` TINYINT NOT NULL DEFAULT '0' COMMENT '是否允许发展下级: 0-否, 1-是',
  `state` TINYINT NOT NULL DEFAULT '1' COMMENT '状态: 0-停用, 1-正常',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `init_user_id` BIGINT DEFAULT NULL COMMENT '初始用户ID（创建商户时，允许商户登录的用户）',
--   `login_user_name` VARCHAR(32) COMMENT '登录名', 
  `sett_account_type` VARCHAR(32) COMMENT '账户类型: ALIPAY_CASH-个人支付宝, WX_CASH-个人微信, BANK_PRIVATE-对私账户, BANK_PUBLIC-对公账户, BANK_CARD-银行卡',
  `sett_account_telphone` VARCHAR(32) COMMENT '结算人手机号', 
  `sett_account_name` VARCHAR(32) COMMENT '结算账户名', 
  `sett_account_no` VARCHAR(32) COMMENT '结算账号', 
  `sett_account_bank` VARCHAR(32) COMMENT '开户行名称', 
  `sett_account_sub_bank` VARCHAR(32) COMMENT '开户行支行名称', 
  `cashout_fee_rule_type` TINYINT NOT NULL DEFAULT '1' COMMENT '提现配置类型: 1-使用系统默认, 2-自定义', 
  `cashout_fee_rule` VARCHAR(255) DEFAULT NULL COMMENT '提现手续费规则', 
  `license_img` VARCHAR(128) DEFAULT NULL COMMENT '营业执照照片',
  `permit_img` VARCHAR(128) DEFAULT NULL COMMENT '开户许可证照片',
  `idcard1_img` VARCHAR(128) DEFAULT NULL COMMENT '身份证人像面照片',
  `idcard2_img` VARCHAR(128) DEFAULT NULL COMMENT '身份证国徽面照片',
  `idcard_in_hand_img` VARCHAR(128) DEFAULT NULL COMMENT '手持身份证照片',
  `bank_card_img` VARCHAR(128) DEFAULT NULL COMMENT '银行卡照片',
  `un_amount` INT NOT NULL DEFAULT '0' COMMENT '不可用金额', 
  `balance_amount` INT NOT NULL DEFAULT '0' COMMENT '钱包余额', 
  `audit_profit_amount` INT NOT NULL DEFAULT '0' COMMENT '在途佣金', 
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (agent_no)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='代理商信息表';

-- 团队信息表
DROP TABLE IF EXISTS `t_sys_team`;
CREATE TABLE `t_sys_team` (
  `team_id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '团队ID',
  `team_name` VARCHAR(32) NOT NULL COMMENT '团队名称', 
  `team_no` VARCHAR(64) NOT NULL COMMENT '团队编号', 
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心', 
  `stat_range_type` VARCHAR(20) NOT NULL COMMENT '统计周期', 
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '归属信息ID', 
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (team_id)
);

-- 费率信息表
DROP TABLE IF EXISTS `t_pay_rate_config`;
CREATE TABLE `t_pay_rate_config` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `config_mode` VARCHAR(20) NOT NULL COMMENT '配置模式: mgrIsv-服务商, mgrAgent-代理商, agentSubagent-子代理商 ,mgrMch-商户 ,agentMch-商户',
  `config_type` VARCHAR(20) NOT NULL COMMENT '配置类型: ISVCOST-服务商低价, AGENTRATE-代理商费率, AGENTDEF-代理商默认费率 ,MCHAPPLYDEF-商户默认费率',
  `info_type` TINYINT NOT NULL COMMENT '账号类型:1-服务商 2-商户 3-商户应用',
  `info_id` VARCHAR(64) NOT NULL COMMENT '服务商号/商户号/应用ID',
  `if_code` VARCHAR(20) NOT NULL COMMENT '支付接口',
  `way_code` VARCHAR(20) NOT NULL COMMENT '支付方式',
  `fee_type` TINYINT NOT NULL COMMENT '费率类型:SINGLE-单笔费率, LEVEL-阶梯费率',
  `min_fee` INT NOT NULL DEFAULT '0' COMMENT '保底费用', 
  `max_fee` INT NOT NULL DEFAULT '0' COMMENT '封顶费用', 
  `fee_rate` DECIMAL(20,6) NOT NULL COMMENT '支付方式费率',
  `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_AppId_WayCode` (`info_type`,`info_id`,`if_code`,`way_code`)
) ENGINE=INNODB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='支付费率配置表'

-- 阶梯费率信息表
DROP TABLE IF EXISTS `t_level_rate_config`;
CREATE TABLE `t_level_rate_config` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'ID',
  -- 模式：普通模式 银联模式
  -- 支付方式：所有 借记卡（储蓄卡） 贷记卡（信用卡）
  `rate_config_id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '支付费率配置ID',
  `min_amount` INT NOT NULL DEFAULT '0' COMMENT '最小金额: 计算时大于此值', 
  `max_amount` INT NOT NULL DEFAULT '0' COMMENT '最大金额: 计算时小于或等于此值', 
  -- 保底费用
  -- 封顶费用
  `fee_rate` DECIMAL(20,6) NOT NULL COMMENT '支付方式费率',
  `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
) ENGINE=INNODB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='阶梯费率表'

-- 码牌信息表
DROP TABLE IF EXISTS `t_mch_qrcodes`;
CREATE TABLE `t_mch_qrcodes` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `qrc_id` VARCHAR(64) NOT NULL COMMENT '码牌ID',
  `agent_no` VARCHAR(64) NOT NULL COMMENT '代理商号',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mch_name` VARCHAR(30) NOT NULL COMMENT '商户名称',
  `entry_page` VARCHAR(20) NOT NULL COMMENT '选择页面类型: default-默认(未指定，取决于二维码是否绑定到微信侧), h5-固定H5页面, lite-固定小程序页面', 
  `batch_id` VARCHAR(64) NOT NULL COMMENT '批次号',
  `store_id` VARCHAR(64) NOT NULL COMMENT '门店ID',
  `qrc_alias` VARCHAR(255) COMMENT '码牌别名', 
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `qrc_belong_type` INT, 
  `alipay_way_code` VARCHAR(20) NOT NULL COMMENT '支付宝支付方式(仅H5呈现时生效)', 
  `fixed_flag` TINYINT NOT NULL COMMENT '是否固定金额: 0-任意金额, 1-固定金额', 
  `fixed_pay_amount` INT NOT NULL DEFAULT '0' COMMENT '固定金额',
  `qrc_shell_id` INT, 
  `qrc_belong_type` INT NOT NULL DEFAULT '1' COMMENT '1-自制, 2-下发',
  `bind_state` TINYINT NOT NULL COMMENT '码牌绑定状态: 0-未绑定, 1-已绑定', 
  `qrc_state` INT, 
  -- `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (id)
);

ALTER TABLE `t_sys_user`   
  CHANGE `sex` `sex` TINYINT DEFAULT 0 NOT NULL COMMENT '性别: 0-未知, 1-男, 2-女',
  CHANGE `is_admin` `is_admin` TINYINT DEFAULT 0 NOT NULL COMMENT '是否超管（超管拥有全部权限）: 0-否 1-是',
  CHANGE `sys_type` `sys_type` VARCHAR(8) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心',
  ADD COLUMN `user_type` TINYINT NULL COMMENT '1-超级管理员, 2-普通操作员, 3-商户拓展员' AFTER `sys_type`,
  ADD COLUMN `invite_code` VARCHAR(20) NULL COMMENT '邀请码' AFTER `user_type`,
  ADD COLUMN `team_id` BIGINT NULL COMMENT '团队ID' AFTER `invite_code`,
  ADD COLUMN `team_name` VARCHAR(32) NULL COMMENT '团队名称' AFTER `team_id`,
  ADD COLUMN `is_team_leader` TINYINT NULL COMMENT '是否队长:  0-否 1-是' AFTER `team_name`;
  
ALTER TABLE `t_pay_interface_define`
  ADD COLUMN `is_support_applyment` TINYINT DEFAULT 1 NOT NULL COMMENT '是否支持进件: 0-不支持, 1-支持' AFTER `config_page_type`;

ALTER TABLE `t_pay_interface_config`   
  ADD COLUMN `sett_hold_day` TINYINT DEFAULT 0 NOT NULL COMMENT '结算周期（自然日）' AFTER `if_params`;

ALTER TABLE `t_pay_way`   
  ADD COLUMN `way_type` VARCHAR(20) NOT NULL COMMENT '支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, OTHER-其他' AFTER `way_name`;

ALTER TABLE `t_pay_order`   
  ADD COLUMN `agent_no` VARCHAR(64) NULL COMMENT '代理商号' AFTER `mch_no`,
  ADD COLUMN `isv_name` VARCHAR(64) NULL COMMENT '服务商名称' AFTER `isv_no`,
  ADD COLUMN `isv_short_name` VARCHAR(32) NULL COMMENT '服务商简称' AFTER `isv_name`,
  ADD COLUMN `qrc_id` VARCHAR(64) NULL COMMENT '应用名称' AFTER `app_id`,
  ADD COLUMN `app_name` VARCHAR(64) NULL COMMENT '应用名称' AFTER `app_id`,
  ADD COLUMN `store_id` VARCHAR(64) NULL COMMENT '门店ID' AFTER `app_name`,
  ADD COLUMN `store_name` VARCHAR(64) NULL COMMENT '门店名称' AFTER `store_id`,
  CHANGE `mch_name` `mch_name` VARCHAR(64) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULLLYDEF COMMENT '商户名称',
  ADD COLUMN `mch_short_name` VARCHAR(32) NULL COMMENT '商户简称' AFTER `mch_name`;
  ADD COLUMN `seller_remark` VARCHAR(255) NULL COMMENT '买家备注' AFTER `body`;
  ADD COLUMN `buyer_remark` VARCHAR(255) NULL COMMENT '卖家备注' AFTER `seller_remark`;

-- 代理商管理
INSERT INTO t_sys_entitlement VALUES('ENT_AGENT', '代理商管理', 'shop', '', 'RouteView', 'ML', 0, 1,  'ROOT', '30', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_INFO', '代理商列表', 'profile', '/agent', 'AgentListPage', 'ML', 0, 1,  'ENT_AGENT', '10', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_LIST', '页面：代理商列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());








