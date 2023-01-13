-- 商户门店表
DROP TABLE IF EXISTS `t_mch_store`;
CREATE TABLE `t_mch_store` (
  `store_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '门店ID',
  `store_name` VARCHAR(64) NOT NULL DEFAULT '' COMMENT '门店名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `agent_no` VARCHAR(64) NULL COMMENT '代理商号',
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `contact_phone` VARCHAR(32) NOT NULL COMMENT '联系人电话',
  `store_logo` VARCHAR(128) NOT NULL COMMENT '门店LOGO',
  `store_outer_img` VARCHAR(128) NOT NULL COMMENT '门头照',
  `store_inner_img` VARCHAR(128) NOT NULL COMMENT '门店内景照',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `province_code` VARCHAR(32) NOT NULL COMMENT '省代码',
  `city_code` VARCHAR(32) NOT NULL COMMENT '市代码',
  `area_code` VARCHAR(32) NOT NULL COMMENT '区代码',
  `address` VARCHAR(128) NOT NULL COMMENT '详细地址',
  `lng` VARCHAR(32) NOT NULL COMMENT '经度',
  `lat` VARCHAR(32) NOT NULL COMMENT '纬度',
  `default_flag` TINYINT NOT NULL DEFAULT '0' COMMENT '是否默认: 0-否, 1-是',
  `bind_app_id` VARCHAR(64) NULL COMMENT '绑定AppId',
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`store_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci AUTO_INCREMENT=1001 COMMENT='商户门店表';
  
-- 门店管理
INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE', '门店管理', 'profile', '/store', 'MchStoreListPage', 'ML', 0, 1,  'ENT_MCH', '40', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_LIST', '页面：门店列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
    
-- 门店管理
INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE', '门店管理', 'profile', '/store', 'MchStoreListPage', 'ML', 0, 1,  'ENT_MCH', '40', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_LIST', '页面：门店列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
    
-- 门店管理
INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE', '门店管理', 'shop', '/store', 'MchStoreListPage', 'ML', 0, 1,  'ENT_MCH_CENTER', '60', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_LIST', '页面：门店列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_MCH_STORE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
        
-- 商户进件表
DROP TABLE IF EXISTS `t_mch_apply`;
CREATE TABLE `t_mch_apply` (
  `apply_id` VARCHAR(64) NOT NULL COMMENT '进件单号',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `agent_no` VARCHAR(64) NOT NULL COMMENT '代理商号',
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
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
  `province_code` VARCHAR(32) NOT NULL COMMENT '省代码',
  `city_code` VARCHAR(32) NOT NULL COMMENT '市代码',
  `area_code` VARCHAR(32) NOT NULL COMMENT '区代码',
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
DROP TABLE IF EXISTS `t_sys_user_team`;
CREATE TABLE `t_sys_user_team` (
  `team_id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '团队ID',
  `team_name` VARCHAR(32) NOT NULL COMMENT '团队名称', 
  `team_no` VARCHAR(64) NOT NULL COMMENT '团队编号', 
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心', 
  `stat_range_type` VARCHAR(20) NOT NULL COMMENT '统计周期: year-年, quarter-季度, month-月, week-周', 
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '归属信息ID', 
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (team_id)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='团队信息表';

INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM', '团队管理', 'team', '/teams', 'SysUserTeamPage', 'ML', 0, 1, 'ENT_UR', 15, 'MGR', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_LIST', '页面：团队列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_DEL', '按钮： 删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
	
INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM', '团队管理', 'team', '/teams', 'SysUserTeamPage', 'ML', 0, 1, 'ENT_UR', 15, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_LIST', '页面：团队列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR_TEAM_DEL', '按钮： 删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());

-- 公告信息表
CREATE TABLE `t_sys_article`(  
  `article_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '公告ID',
  `title` VARCHAR(64) NOT NULL COMMENT '公告标题',
  `subtitle` VARCHAR(64) NOT NULL COMMENT '公告副标题',
  `article_type` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '公告类型',
  `article_range` JSON NOT NULL COMMENT '公告范围',
  `content` TEXT COMMENT '公告内容',
  `publisher` VARCHAR(32) NOT NULL COMMENT '发布人',
  `publish_time` TIMESTAMP(6) COMMENT '发布时间',
  `created_uid` BIGINT(20) COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`article_id`)
) ENGINE=INNODB CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci
AUTO_INCREMENT=1001 COMMENT='公告信息表';

-- 公告管理
INSERT INTO t_sys_entitlement VALUES('ENT_ARTICLE', '公告管理', 'message', '/notices', 'NoticeListPage', 'ML', 0, 1,  'ENT_SYS_CONFIG', '30', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_ARTICLE_LIST', '页面：公告列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_ARTICLE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_ARTICLE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_ARTICLE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE', '0', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_ARTICLE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE', '0', 'MGR', NOW(), NOW());

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

ALTER TABLE `t_mch_info`   
  ADD COLUMN `mch_level` VARCHAR(8) DEFAULT 'M0' NOT NULL COMMENT '商户级别: M0商户-简单模式（页面简洁，仅基础收款功能）, M1商户-高级模式（支持api调用，支持配置应用及分账、转账功能）' AFTER `type`,
  ADD COLUMN `refund_mode` JSON NULL COMMENT '退款方式[\"plat\", \"api\"],平台退款、接口退款，平台退款方式必须包含接口退款。' AFTER `mch_level`,
  ADD COLUMN `agent_no` VARCHAR(64) NULL COMMENT '代理商号' AFTER `refund_mode`;

ALTER TABLE `t_mch_app`   
  ADD COLUMN `default_flag` TINYINT(6) DEFAULT 0 NOT NULL COMMENT '是否默认: 0-否, 1-是' AFTER `state`,
  ADD COLUMN `app_sign_type` JSON NOT NULL COMMENT '支持的签名方式 [\"MD5\", \"RSA2\"]' AFTER `default_flag`,
  CHANGE `app_secret` `app_secret` VARCHAR(128) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '应用MD5私钥',
  ADD COLUMN `app_rsa2_public_key` VARCHAR(448) NULL COMMENT 'RSA2应用公钥' AFTER `app_secret`;
  
ALTER TABLE `t_sys_user`   
  CHANGE `sex` `sex` TINYINT DEFAULT 0 NOT NULL COMMENT '性别: 0-未知, 1-男, 2-女',
  CHANGE `is_admin` `is_admin` TINYINT DEFAULT 0 NOT NULL COMMENT '是否超管（超管拥有全部权限）: 0-否 1-是',
  CHANGE `sys_type` `sys_type` VARCHAR(8) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心',
  ADD COLUMN `user_type` TINYINT(6) NOT NULL DEFAULT 1 COMMENT '用户类型: 1-超级管理员, 2-普通操作员, 3-商户拓展员, 11-店长, 12-店员' AFTER `sys_type`,
  ADD COLUMN `invite_code` VARCHAR(20) NULL COMMENT '邀请码' AFTER `user_type`,
  ADD COLUMN `team_id` BIGINT NULL COMMENT '团队ID' AFTER `invite_code`,
--   ADD COLUMN `team_name` VARCHAR(32) NULL COMMENT '团队名称' AFTER `team_id`,
  ADD COLUMN `is_team_leader` TINYINT NULL COMMENT '是否队长:  0-否 1-是' AFTER `team_id`;

ALTER TABLE `t_sys_user`   
  ADD  UNIQUE INDEX `invite_code` (`invite_code`);

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
  ADD COLUMN `qrc_id` VARCHAR(64) NULL COMMENT '二维码' AFTER `app_id`,
  ADD COLUMN `app_name` VARCHAR(64) NULL COMMENT '应用名称' AFTER `app_id`,
  ADD COLUMN `store_id` VARCHAR(64) NULL COMMENT '门店ID' AFTER `app_name`,
  ADD COLUMN `store_name` VARCHAR(64) NULL COMMENT '门店名称' AFTER `store_id`,
  ADD COLUMN `mch_short_name` VARCHAR(32) NULL COMMENT '商户简称' AFTER `mch_name`,
  ADD COLUMN `seller_remark` VARCHAR(256) NULL COMMENT '买家备注' AFTER `body`,
  ADD COLUMN `buyer_remark` VARCHAR(256) NULL COMMENT '卖家备注' AFTER `seller_remark`;

ALTER TABLE `t_pay_order`   
  CHANGE `mch_name` `mch_name` VARCHAR(64) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '商户名称'  AFTER `mch_no`,
  CHANGE `mch_short_name` `mch_short_name` VARCHAR(32) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '商户简称'  AFTER `mch_name`,
  ADD COLUMN `agent_name` VARCHAR(64) NULL COMMENT '代理商名称' AFTER `agent_no`,
  ADD COLUMN `agent_short_name` VARCHAR(32) NULL COMMENT '代理商简称' AFTER `agent_name`;
  
-- 代理商管理
INSERT INTO t_sys_entitlement VALUES('ENT_AGENT', '代理商管理', 'shop', '', 'RouteView', 'ML', 0, 1,  'ROOT', '35', 'MGR', NOW(), NOW());
    INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_INFO', '代理商列表', 'profile', '/agent', 'AgentListPage', 'ML', 0, 1,  'ENT_AGENT', '10', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_LIST', '页面：代理商列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO t_sys_entitlement VALUES('ENT_AGENT_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());

#####  ↓↓↓↓↓↓↓↓↓↓  代理商管理初始化DML  ↓↓↓↓↓↓↓↓↓↓  #####

-- 权限表数据 （ 不包含根目录 ）
INSERT INTO t_sys_entitlement VALUES ('ENT_COMMONS', '系统通用菜单', 'no-icon', '', 'RouteView', 'MO', 0, 1, 'ROOT', -1, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_C_USERINFO', '个人中心', 'no-icon', '/current/userinfo', 'CurrentUserInfo', 'MO', 0, 1, 'ENT_COMMONS', -1, 'AGENT', NOW(), NOW());

INSERT INTO t_sys_entitlement VALUES ('ENT_C_MAIN', '主页', 'home', '/main', 'MainPage', 'ML', 0, 1, 'ROOT', 1, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_C_MAIN_PAY_TYPE_COUNT', '主页交易方式统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_C_MAIN_PAY_COUNT', '主页交易统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_C_MAIN_PAY_AMOUNT_WEEK', '主页周支付统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_C_MAIN_NUMBER_COUNT', '主页数量总统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());

-- 商户管理
INSERT INTO t_sys_entitlement VALUES ('ENT_MCH', '商户管理', 'shop', '', 'RouteView', 'ML', 0, 1, 'ROOT', 30, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_INFO', '商户列表', 'profile', '/mch', 'MchListPage', 'ML', 0, 1, 'ENT_MCH', 10, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_LIST', '页面：商户列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_CONFIG', '应用配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());

    -- 应用管理
	INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP', '应用列表', 'appstore', '/apps', 'MchAppPage', 'ML', 0, 1, 'ENT_MCH', 20, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_LIST', '页面：应用列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_APP_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_PASSAGE_LIST', '应用支付通道配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_PASSAGE_CONFIG', '应用支付通道配置入口', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_PASSAGE_LIST', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_PASSAGE_ADD', '应用支付通道配置保存', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_PASSAGE_LIST', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_CONFIG_VIEW', '应用支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_CONFIG_LIST', '应用支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_PAY_CONFIG_ADD', '应用支付参数配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());

-- 代理商管理		
INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT', '代理商管理', 'shop', '', 'RouteView', 'ML', 0, 1, 'ROOT', 35, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO', '代理商列表', 'profile', '/agent', 'AgentListPage', 'ML', 0, 1, 'ENT_AGENT', 10, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_LIST', '页面：代理商列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_AGENT_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
		
-- -- 服务商管理
-- INSERT INTO t_sys_entitlement VALUES ('ENT_ISV', '服务商管理', 'block', '', 'RouteView', 'ML', 0, 1, 'ROOT', 40, 'AGENT', NOW(), NOW());
-- 	INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_INFO', '服务商列表', 'profile', '/isv', 'IsvListPage', 'ML', 0, 1, 'ENT_ISV', 10, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_PAY_CONFIG_VIEW', '服务商支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_PAY_CONFIG_LIST', '服务商支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_PAY_CONFIG_ADD', '服务商支付参数配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_LIST', '页面：服务商列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_ISV_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ISV_INFO', 0, 'AGENT', NOW(), NOW());

-- 订单管理
INSERT INTO t_sys_entitlement VALUES ('ENT_ORDER', '订单管理', 'transaction', '', 'RouteView', 'ML', 0, 1, 'ROOT', 50, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_PAY_ORDER', '支付订单', 'account-book', '/pay', 'PayOrderListPage', 'ML', 0, 1, 'ENT_ORDER', 10, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_ORDER_LIST', '页面：订单列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PAY_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PAY_ORDER_REFUND', '按钮：订单退款', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PAY_ORDER_SEARCH_PAY_WAY', '筛选项：支付方式', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_REFUND_ORDER', '退款订单', 'exception', '/refund', 'RefundOrderListPage', 'ML', 0, 1, 'ENT_ORDER', 20, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_REFUND_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_REFUND_ORDER', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_REFUND_LIST', '页面：退款订单列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_REFUND_ORDER', 0, 'AGENT', NOW(), NOW());
-- 	INSERT INTO t_sys_entitlement VALUES ('ENT_TRANSFER_ORDER', '转账订单', 'property-safety', '/transfer', 'TransferOrderListPage', 'ML', 0, 1, 'ENT_ORDER', 25, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_TRANSFER_ORDER_LIST', '页面：转账订单列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_TRANSFER_ORDER', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_TRANSFER_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_TRANSFER_ORDER', 0, 'AGENT', NOW(), NOW());
-- 	INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_NOTIFY', '商户通知', 'notification', '/notify', 'MchNotifyListPage', 'ML', 0, 1, 'ENT_ORDER', 30, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_NOTIFY_LIST', '页面：商户通知列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_NOTIFY', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_NOTIFY_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_NOTIFY', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_MCH_NOTIFY_RESEND', '按钮：重发通知', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_NOTIFY', 0, 'AGENT', NOW(), NOW());

-- 支付配置菜单
INSERT INTO t_sys_entitlement VALUES ('ENT_PC', '支付配置', 'file-done', '', 'RouteView', 'ML', 0, 1, 'ROOT', 60, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE', '支付接口', 'interaction', '/ifdefines', 'IfDefinePage', 'ML', 0, 1, 'ENT_PC', 10, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_LIST', '页面：支付接口定义列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_IF_DEFINE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_IF_DEFINE', 0, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY', '支付方式', 'appstore', '/payways', 'PayWayPage', 'ML', 0, 1, 'ENT_PC', 20, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_LIST', '页面：支付方式列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_PC_WAY_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PC_WAY', 0, 'AGENT', NOW(), NOW());

-- 系统管理
INSERT INTO t_sys_entitlement VALUES ('ENT_SYS_CONFIG', '系统管理', 'setting', '', 'RouteView', 'ML', 0, 1, 'ROOT', 200, 'AGENT', NOW(), NOW());
	INSERT INTO t_sys_entitlement VALUES ('ENT_UR', '用户角色管理', 'team', '', 'RouteView', 'ML', 0, 1, 'ENT_SYS_CONFIG', 10, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER', '操作员管理', 'contacts', '/users', 'SysUserPage', 'ML', 0, 1, 'ENT_UR', 10, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_VIEW', '按钮： 详情', '', 'no-icon', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_UPD_ROLE', '按钮： 角色分配', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_SEARCH', '按钮：搜索', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_LIST', '页面：操作员列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_EDIT', '按钮： 修改基本信息', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_DELETE', '按钮： 删除操作员', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_USER_ADD', '按钮：添加操作员', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());

		INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE', '角色管理', 'user', '/roles', 'RolePage', 'ML', 0, 1, 'ENT_UR', 20, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_EDIT', '按钮： 修改基本信息', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_DIST', '按钮： 分配权限', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_DEL', '按钮： 删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_ADD', '按钮：添加角色', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());		
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_LIST', '页面：角色列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());

-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_ENT', '权限管理', 'apartment', '/ents', 'EntPage', 'ML', 0, 1, 'ENT_UR', 30, 'AGENT', NOW(), NOW());
-- 			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_ENT_LIST', '页面： 权限列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE_ENT', 0, 'AGENT', NOW(), NOW());
-- 			INSERT INTO t_sys_entitlement VALUES ('ENT_UR_ROLE_ENT_EDIT', '按钮： 权限变更', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE_ENT', 0, 'AGENT', NOW(), NOW());

	INSERT INTO t_sys_entitlement VALUES ('ENT_SYS_CONFIG_INFO', '系统配置', 'setting', '/config', 'SysConfigPage', 'ML', 0, 1, 'ENT_SYS_CONFIG', 15, 'AGENT', NOW(), NOW());
		INSERT INTO t_sys_entitlement VALUES ('ENT_SYS_CONFIG_EDIT', '按钮： 修改', 'no-icon', '', '', 'PB', 0, 1, 'ENT_SYS_CONFIG_INFO', 0, 'AGENT', NOW(), NOW());

-- 	INSERT INTO t_sys_entitlement VALUES ('ENT_SYS_LOG', '系统日志', 'file-text', '/log', 'SysLogPage', 'ML', 0, 1, 'ENT_SYS_CONFIG', 20, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_LOG_LIST', '页面：系统日志列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_SYS_LOG', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_SYS_LOG_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_SYS_LOG', 0, 'AGENT', NOW(), NOW());
-- 		INSERT INTO t_sys_entitlement VALUES ('ENT_SYS_LOG_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_SYS_LOG', 0, 'AGENT', NOW(), NOW());





