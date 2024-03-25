#####  表结构及初始化数据SQL  #####

--   RBAC设计思路：  [用户] 1<->N [角色] 1<->N [权限]

-- 权限表
DROP TABLE IF EXISTS `t_sys_entitlement`;
CREATE TABLE `t_sys_entitlement` (
  `ent_id` VARCHAR(64) NOT NULL COMMENT '权限ID[ENT_功能模块_子模块_操作], eg: ENT_ROLE_LIST_ADD',
  `ent_name` VARCHAR(32) NOT NULL COMMENT '权限名称',
  `menu_icon` VARCHAR(32) COMMENT '菜单图标',
  `menu_uri` VARCHAR(128) COMMENT '菜单uri/路由地址',
  `component_name` VARCHAR(32) COMMENT '组件Name（前后端分离使用）',
  `ent_type` CHAR(2) NOT NULL COMMENT '权限类型 ML-左侧显示菜单, MO-其他菜单, PB-页面/按钮',
  `quick_jump` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '快速开始菜单 0-否, 1-是',
  `match_rule` JSON NULL COMMENT '权限匹配规则',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '状态 0-停用, 1-启用',
  `pid` VARCHAR(32) NOT NULL COMMENT '父ID',
  `ent_sort` INT(11) NOT NULL DEFAULT '0' COMMENT '排序字段, 规则：正序',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`ent_id`, `sys_type`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='系统权限表';

-- 角色表
DROP TABLE IF EXISTS `t_sys_role`;
CREATE TABLE `t_sys_role` (
  `role_id` VARCHAR(32) NOT NULL COMMENT '角色ID, ROLE_开头',
  `role_name` VARCHAR(32) NOT NULL COMMENT '角色名称',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心',
  `belong_info_id` VARCHAR(64) NOT NULL DEFAULT '0' COMMENT '所属商户ID / 代理商ID / 0(平台)',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`role_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='系统角色表';

-- 角色<->权限 关联表
DROP TABLE IF EXISTS `t_sys_role_ent_rela`;
CREATE TABLE `t_sys_role_ent_rela` (
  `role_id` VARCHAR(32) NOT NULL COMMENT '角色ID',
  `ent_id` VARCHAR(64) NOT NULL COMMENT '权限ID' ,
  PRIMARY KEY (`role_id`, `ent_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='系统角色权限关联表';

-- 系统用户表
DROP TABLE IF EXISTS `t_sys_user`;
CREATE TABLE `t_sys_user` (
  `sys_user_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '系统用户ID',
  `login_username` VARCHAR(32) NOT NULL COMMENT '登录用户名',
  `realname` VARCHAR(32) NOT NULL COMMENT '真实姓名',
  `telphone` VARCHAR(32) NOT NULL COMMENT '手机号',
  `sex` TINYINT(4) NOT NULL DEFAULT '0' COMMENT '性别: 0-未知, 1-男, 2-女',
  `avatar_url` VARCHAR(128) DEFAULT NULL COMMENT '头像地址',
  `user_no` VARCHAR(32) DEFAULT NULL COMMENT '员工编号',
  `safe_word` VARCHAR(32) DEFAULT NULL COMMENT '预留信息',
  `init_user` TINYINT(1) NOT NULL DEFAULT '0' COMMENT '初始用户',
  `user_type` TINYINT(6) NOT NULL COMMENT '用户类型: 1-超级管理员, 2-普通操作员, 3-商户拓展员, 11-店长, 12-店员',
  `ent_rules` JSON DEFAULT NULL COMMENT '权限配置规则 ["USER_TYPE_11_INIT", "STORE"]',
  `bind_store_ids` JSON DEFAULT NULL COMMENT '绑定门店ID [1001, 1002]',
  `invite_code` VARCHAR(20) DEFAULT NULL COMMENT '邀请码',
  `team_id` BIGINT(20) DEFAULT NULL COMMENT '团队ID',
  `is_team_leader` TINYINT(4) DEFAULT NULL COMMENT '是否队长:  0-否 1-是',
  `state` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '状态 0-停用 1-启用',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心',
  `belong_info_id` VARCHAR(64) NOT NULL DEFAULT '0' COMMENT '所属商户ID / 代理商ID / 0(平台)',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`sys_user_id`),
  UNIQUE KEY `Uni_SysType_LoginUsername` (`sys_type`,`login_username`),
  UNIQUE KEY `Uni_SysType_Telphone` (`sys_type`,`telphone`),
  UNIQUE KEY `Uni_SysType_UserNo` (`sys_type`, `user_no`),
  UNIQUE INDEX `Uni_InviteCode` (`invite_code`)
) ENGINE=INNODB AUTO_INCREMENT=100001 DEFAULT CHARSET=utf8mb4 COMMENT='系统用户表';

-- 团队信息表
DROP TABLE IF EXISTS `t_sys_user_team`;
CREATE TABLE `t_sys_user_team` (
  `team_id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '团队ID',
  `team_name` VARCHAR(32) NOT NULL COMMENT '团队名称', 
  `team_no` VARCHAR(64) NOT NULL COMMENT '团队编号', 
  `stat_range_type` VARCHAR(20) NOT NULL COMMENT '统计周期: year-年, quarter-季度, month-月, week-周', 
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心', 
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '所属商户ID / 代理商ID / 0(平台)', 
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (team_id)
) ENGINE=INNODB AUTO_INCREMENT=1001 DEFAULT CHARSET=utf8mb4 COMMENT='团队信息表';

-- 系统用户认证表
DROP TABLE IF EXISTS `t_sys_user_auth`;
CREATE TABLE `t_sys_user_auth` (
  `auth_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `user_id` BIGINT(20) NOT NULL COMMENT 'user_id',
  `identity_type` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '登录类型  1-登录账号 2-手机号 3-邮箱  10-微信  11-QQ 12-支付宝 13-微博',
  `identifier` VARCHAR(128) NOT NULL COMMENT '认证标识 ( 用户名 | open_id )',
  `credential` VARCHAR(128) NOT NULL COMMENT '密码凭证',
  `salt` VARCHAR(128) NOT NULL COMMENT 'salt',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统： MGR-运营平台, MCH-商户中心',
  PRIMARY KEY (`auth_id`)
) ENGINE=INNODB AUTO_INCREMENT=1001 DEFAULT CHARSET=utf8mb4 COMMENT='系统用户认证表';

-- 系统用户登录尝试记录表
DROP TABLE IF EXISTS `t_sys_user_login_attempt`;
CREATE TABLE `t_sys_user_login_attempt` (
  `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '系统用户ID',
  `user_id` BIGINT(20) NOT NULL COMMENT '系统用户ID',
  `identity_type` TINYINT(6) NOT NULL COMMENT '登录类型: 1-登录账号 2-手机号 3-邮箱  10-微信  11-QQ 12-支付宝 13-微博',
  `identifier` VARCHAR(128) NOT NULL COMMENT '认证标识 ( 用户名 | open_id )',
  `ip_address` VARCHAR(128) NOT NULL COMMENT 'IP地址',
  `success` TINYINT(1) NOT NULL DEFAULT '0' COMMENT '登录成功',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心',
  `attempt_time` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '尝试时间',
  PRIMARY KEY (`id`),
  INDEX `Idx_UserId_SysType` (`user_id`, `sys_type`)
) ENGINE=INNODB AUTO_INCREMENT=1001 DEFAULT CHARSET=utf8mb4 COMMENT='系统用户登录尝试记录表';

-- 操作员<->角色 关联表
DROP TABLE IF EXISTS `t_sys_user_role_rela`;
CREATE TABLE `t_sys_user_role_rela` (
  `user_id` BIGINT(20) NOT NULL COMMENT '用户ID',
  `role_id`VARCHAR(32) NOT NULL COMMENT '角色ID',
  PRIMARY KEY (`user_id`, `role_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='操作员<->角色 关联表';

-- 系统配置表
DROP TABLE IF EXISTS `t_sys_config`;
CREATE TABLE `t_sys_config` (
  `config_key` VARCHAR(50) NOT NULL COMMENT '配置KEY',
  `config_name` VARCHAR(50) NOT NULL COMMENT '配置名称',
  `config_desc` VARCHAR(200) NOT NULL COMMENT '描述信息',
  `group_key` VARCHAR(50) NOT NULL COMMENT '分组key',
  `group_name` VARCHAR(50) NOT NULL COMMENT '分组名称',
  `config_val` TEXT NOT NULL COMMENT '配置内容项',
  `type` VARCHAR(20) NOT NULL DEFAULT 'text' COMMENT '类型: text-输入框, textarea-多行文本, uploadImg-上传图片, switch-开关',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心',
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '所属商户ID / 代理商ID / 0(平台)',
  `sort_num` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '显示顺序',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`config_key`, `sys_type`, `belong_info_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='系统配置表';

-- 文章信息表
DROP TABLE IF EXISTS `t_sys_article`;
CREATE TABLE `t_sys_article`(  
  `article_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '文章ID',
  `title` VARCHAR(64) NOT NULL COMMENT '文章标题',
  `subtitle` VARCHAR(64) NOT NULL COMMENT '文章副标题',
  `article_type` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '文章类型: 1-公告',
  `article_range` JSON NOT NULL COMMENT '文章范围',
  `content` TEXT COMMENT '文章内容',
  `publisher` VARCHAR(32) NOT NULL COMMENT '发布人',
  `publish_time` TIMESTAMP(6) COMMENT '发布时间',
  `created_uid` BIGINT(20) COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`article_id`)
) ENGINE=INNODB AUTO_INCREMENT=1001 CHARSET=utf8mb4 COMMENT='文章信息表';

-- 系统操作日志表
DROP TABLE IF EXISTS `t_sys_log`;
CREATE TABLE `t_sys_log` (
  `sys_log_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `user_id` BIGINT(20) DEFAULT NULL COMMENT '系统用户ID',
  `user_name` VARCHAR(32) DEFAULT NULL COMMENT '用户姓名',
  `browser` VARCHAR(60) NOT NULL DEFAULT '' COMMENT '浏览器',
  `os` VARCHAR(60) NOT NULL DEFAULT '' COMMENT '操作系统',
  `device` VARCHAR(60) NOT NULL DEFAULT '' COMMENT '设备',
  `browser_info` VARCHAR(200) NOT NULL DEFAULT '' COMMENT '浏览器信息',
  `user_ip` VARCHAR(128) NOT NULL DEFAULT '' COMMENT '用户IP',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心',
  `method_name` VARCHAR(128) NOT NULL DEFAULT '' COMMENT '方法名',
  `method_remark` VARCHAR(128) NOT NULL DEFAULT '' COMMENT '方法描述',
  `req_url` VARCHAR(256) NOT NULL DEFAULT '' COMMENT '请求地址',
  `req_method` VARCHAR(10) NOT NULL DEFAULT '' COMMENT '请求方法',
  `opt_req_param` VARCHAR(2048) NOT NULL DEFAULT '' COMMENT '操作请求参数',
  `opt_res_info` VARCHAR(2048) NOT NULL DEFAULT '' COMMENT '操作响应结果',
  `elapsed_ms` BIGINT(20) DEFAULT NULL COMMENT '耗时（毫秒）',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  PRIMARY KEY (`sys_log_id`)
) ENGINE = INNODB DEFAULT CHARSET = utf8mb4 COMMENT = '系统操作日志表';

-- 商户信息表
DROP TABLE IF EXISTS `t_mch_info`;
CREATE TABLE `t_mch_info` (
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mch_name` VARCHAR(64) NOT NULL COMMENT '商户名称',
  `mch_short_name` VARCHAR(32) NOT NULL COMMENT '商户简称',
  `type` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '类型: 1-普通商户, 2-特约商户(服务商模式)',
  `isv_no` VARCHAR(64) COMMENT '服务商号',
  `mch_level` VARCHAR(8) DEFAULT 'M0' NOT NULL COMMENT '商户级别: M0商户-简单模式（页面简洁，仅基础收款功能）, M1商户-高级模式（支持api调用，支持配置应用及分账、转账功能）',
  `refund_mode` JSON NULL COMMENT '退款方式[\"plat\", \"api\"],平台退款、接口退款，平台退款方式必须包含接口退款。',
  `sipw` VARCHAR(128) NOT NULL COMMENT '支付密码',
  `top_agent_no` VARCHAR(64) NULL COMMENT '顶级代理商号',
  `agent_no` VARCHAR(64) NULL COMMENT '代理商号',
  `contact_name` VARCHAR(32) COMMENT '联系人姓名',
  `contact_tel` VARCHAR(32) COMMENT '联系人手机号',
  `contact_email` VARCHAR(32) COMMENT '联系人邮箱',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '商户状态: 0-停用, 1-正常',
  `remark` VARCHAR(128) COMMENT '商户备注',
  `init_user_id` BIGINT(20) DEFAULT NULL COMMENT '初始用户ID（创建商户时，允许商户登录的用户）',
  `created_uid` BIGINT(20) COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`mch_no`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='商户信息表';

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
  `apply_error_info` VARCHAR(255) NOT NULL COMMENT '申请错误信息(响应提示信息)', 
  `channel_apply_no` VARCHAR(64) NOT NULL COMMENT '渠道申请单号', 
  `succ_res_parameter` VARCHAR(255) NOT NULL COMMENT '成功响应参数(渠道响应参数)', 
  `channel_var1` VARCHAR(255) NOT NULL COMMENT '渠道拓展参数1', 
  `channel_var2` VARCHAR(255) NOT NULL COMMENT '渠道拓展参数2', 
  `state` TINYINT NOT NULL DEFAULT '0' COMMENT '状态: 0-草稿, 1-审核中, 2-进件成功, 3-驳回待修改, 4-待验证, 5-待签约, 7-等待预审, 8-预审拒绝', 
  `is_temp_data` BOOLEAN NOT NULL COMMENT '是否临时数据', 
  `mch_full_name` VARCHAR(64) NOT NULL COMMENT '商户名称全称', 
  `mch_short_name` VARCHAR(32) NOT NULL COMMENT '进件商户简称', 
  `merchant_type` TINYINT NOT NULL DEFAULT '0' COMMENT '商户类型: 1-个人, 2-个体工商户, 3-企业', 
  `contact_name` VARCHAR(32) NOT NULL COMMENT '商户联系人姓名', 
  `contact_phone` VARCHAR(32) NOT NULL COMMENT '商户联系人电话', 
  `contact_email` VARCHAR(32) NULL COMMENT '商户联系人邮箱', 
  `province_code` VARCHAR(32) NOT NULL COMMENT '省代码',
  `city_code` VARCHAR(32) NOT NULL COMMENT '市代码',
  `district_code` VARCHAR(32) NOT NULL COMMENT '区代码',
  `address` VARCHAR(128) NOT NULL COMMENT '商户详细地址',
  `ep_user_id` BIGINT DEFAULT NULL COMMENT '商户拓展员ID',
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `last_apply_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '上次进件时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (apply_id)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='商户进件申请表';

-- 商户应用表
DROP TABLE IF EXISTS `t_mch_app`;
CREATE TABLE `t_mch_app` (
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `app_name` VARCHAR(64) NOT NULL DEFAULT '' COMMENT '应用名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '应用状态: 0-停用, 1-正常',
  `default_flag` TINYINT(6) DEFAULT '0' NOT NULL COMMENT '是否默认: 0-否, 1-是',
  `app_sign_type` JSON NOT NULL COMMENT '支持的签名方式 [\"MD5\", \"RSA2\"]',
  `app_secret` VARCHAR(128) NOT NULL COMMENT '应用MD5私钥',
  `app_rsa2_public_key` VARCHAR(448) NULL COMMENT 'RSA2应用公钥',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `created_uid` BIGINT(20) COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`app_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='商户应用表';

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
  `district_code` VARCHAR(32) NOT NULL COMMENT '区代码',
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
) ENGINE=INNODB AUTO_INCREMENT=1001 DEFAULT CHARSET=utf8mb4 COMMENT='商户门店表';

-- 商户会员信息表
DROP TABLE IF EXISTS `t_mbr_info`;
CREATE TABLE `t_mbr_info` (
  `mbr_no` VARCHAR(64) NOT NULL COMMENT '会员号',
  `mbr_name` VARCHAR(64) NOT NULL COMMENT '会员名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mbr_tel` VARCHAR(32) COMMENT '手机号',
  `balance` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '账户余额,单位分',
  `ali_user_id` VARCHAR(64) COMMENT '支付宝用户ID',
  `wx_mp_open_id` VARCHAR(64) COMMENT '微信公众平台OpenId',
  `avatar_url` VARCHAR(128) DEFAULT NULL COMMENT '头像地址',
  `remark` VARCHAR(128) COMMENT '会员备注',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '会员状态: 0-停用, 1-正常',
  `created_uid` BIGINT(20) COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`mbr_no`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='商户会员信息表';

-- 商户会员帐单表
DROP TABLE IF EXISTS `t_mbr_bill`;
CREATE TABLE `t_mbr_bill` (
  `mbr_bill_id` VARCHAR(30) NOT NULL COMMENT '会员帐单单号',
  `mbr_no` VARCHAR(64) NOT NULL COMMENT '会员号',
  `mbr_name` VARCHAR(64) NOT NULL COMMENT '会员名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mbr_tel` VARCHAR(32) COMMENT '手机号',
  `before_balance` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '变动前余额,单位分',
  `change_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '变动金额,单位分',
  `after_balance` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '变动后余额,单位分',
  `biz_type` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '业务类型: 0-, 1-支付充值, 2-现金充值, 3-会员消费, 4-消费退款, 5-人工调账',
  `remark` VARCHAR(128) COMMENT '帐单备注',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`mbr_bill_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='商户会员帐单表';

-- 商户会员充值订单表
DROP TABLE IF EXISTS `t_mbr_recharge_order`;
CREATE TABLE `t_mbr_recharge_order` (
  `recharge_order_id` VARCHAR(30) NOT NULL COMMENT '充值单号',
  `mbr_no` VARCHAR(64) NOT NULL COMMENT '会员号',
  `mbr_name` VARCHAR(64) NOT NULL COMMENT '会员名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mbr_tel` VARCHAR(32) COMMENT '手机号',
  `pay_order_id` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '关联支付订单号',
  `way_code` VARCHAR(20) NOT NULL COMMENT '支付方式代码',
  `way_name` VARCHAR(20) NOT NULL COMMENT '支付方式名称',
  `way_type` VARCHAR(20) NOT NULL COMMENT '支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, OTHER-其他',
  `pay_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '支付金额,单位分',
  `entry_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '入账金额,单位分',
  `give_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '赠送金额,单位分',
  `after_balance` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '账户余额,单位分',
  `avatar_url` VARCHAR(128) DEFAULT NULL COMMENT '头像地址',
  `remark` VARCHAR(128) COMMENT '充值备注',
  `state` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '充值状态: 0-初始化, 1-充值中, 2-充值成功, 3-充值失败',
  `notify_url` VARCHAR(128) NOT NULL DEFAULT '' COMMENT '异步通知地址',
  `return_url` VARCHAR(128) DEFAULT '' COMMENT '页面跳转地址',
  `expired_time` DATETIME DEFAULT NULL COMMENT '订单失效时间',
  `success_time` DATETIME DEFAULT NULL COMMENT '订单支付成功时间',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`recharge_order_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='商户会员充值记录表';

-- 商户充值规则表
DROP TABLE IF EXISTS `t_mbr_recharge_rule`;
CREATE TABLE `t_mbr_recharge_rule` (
  `rule_id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '规则ID',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `recharge_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '充值金额,单位分',
  `give_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '赠送金额,单位分',
  `sort` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '排序',
  `remark` VARCHAR(128) COMMENT '备注',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '状态: 0-停用, 1-正常',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`rule_id`)
) ENGINE=INNODB AUTO_INCREMENT=1001 DEFAULT CHARSET=utf8mb4 COMMENT='商户充值规则表';

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
  `sipw` VARCHAR(128) NOT NULL COMMENT '支付密码',
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
  `un_amount` BIGINT NOT NULL DEFAULT '0' COMMENT '不可用金额', 
  `balance_amount` BIGINT NOT NULL DEFAULT '0' COMMENT '钱包余额', 
  `audit_profit_amount` BIGINT NOT NULL DEFAULT '0' COMMENT '在途佣金', 
  `freeze_amount` BIGINT NOT NULL DEFAULT '0' COMMENT '冻结金额', 
  `created_uid` BIGINT DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (agent_no)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='代理商信息表';

-- 账户帐单表
DROP TABLE IF EXISTS `t_account_bill`;
CREATE TABLE `t_account_bill` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '流水号',
  `bill_id` VARCHAR(30) NOT NULL COMMENT '帐单单号',
  `info_id` VARCHAR(64) NOT NULL COMMENT 'PLATFORM_PROFIT-运营平台利润账户, PLATFORM_INACCOUNT-运营平台入账账户, 代理商号',
  `info_name` VARCHAR(64) NOT NULL COMMENT '运营平台, 代理商名称',
  `info_type` VARCHAR(20) NOT NULL COMMENT 'PLATFORM-运营平台, AGENT-代理商',
  `before_balance` BIGINT NOT NULL DEFAULT '0' COMMENT '变动前余额,单位分',
  `change_amount` BIGINT NOT NULL DEFAULT '0' COMMENT '变动金额,单位分',
  `after_balance` BIGINT NOT NULL DEFAULT '0' COMMENT '变动后余额,单位分',
  `biz_type` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '业务类型: 1-订单佣金计算, 2-退款轧差, 3-佣金提现, 4-人工调账',
  `account_type` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '账户类型: 1-钱包账户, 2-在途账户',
  `rela_biz_order_type` TINYINT(6) NOT NULL COMMENT '关联订单类型: 1-支付订单, 2-退款订单, 3-提现申请订单',
  `rela_biz_order_id` VARCHAR(30) DEFAULT NULL COMMENT '关联订单号',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '帐单备注',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_BillId` (`bill_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='账户帐单表';

-- 服务商信息表
DROP TABLE IF EXISTS `t_isv_info`;
CREATE TABLE `t_isv_info` (
  `isv_no` VARCHAR(64) NOT NULL COMMENT '服务商号',
  `isv_name` VARCHAR(64) NOT NULL COMMENT '服务商名称',
  `isv_short_name` VARCHAR(32) NOT NULL COMMENT '服务商简称',
  `contact_name` VARCHAR(32) COMMENT '联系人姓名',
  `contact_tel` VARCHAR(32) COMMENT '联系人手机号',
  `contact_email` VARCHAR(32) COMMENT '联系人邮箱',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '状态: 0-停用, 1-正常',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `created_uid` BIGINT(20) COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`isv_no`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='服务商信息表';

-- 支付方式表  pay_way
DROP TABLE IF EXISTS `t_pay_way`;
CREATE TABLE `t_pay_way` (
  `way_code` VARCHAR(20) NOT NULL COMMENT '支付方式代码  例如： wxpay_jsapi',
  `way_name` VARCHAR(20) NOT NULL COMMENT '支付方式名称',
  `way_type` VARCHAR(20) NOT NULL COMMENT '支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, DCEPPAY-数字人民币, OTHER-其他',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`way_code`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='支付方式表';

-- 支付接口定义表
DROP TABLE IF EXISTS `t_pay_interface_define`;
CREATE TABLE `t_pay_interface_define` (
  `if_code` VARCHAR(20) NOT NULL COMMENT '接口代码 全小写  wxpay alipay ',
  `if_name` VARCHAR(20) NOT NULL COMMENT '接口名称',
  `is_mch_mode` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '是否支持普通商户模式: 0-不支持, 1-支持',
  `is_isv_mode` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '是否支持服务商子商户模式: 0-不支持, 1-支持',
  `config_page_type` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '支付参数配置页面类型:1-JSON渲染,2-自定义',
  `is_support_applyment` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否支持进件: 0-不支持, 1-支持',
  `is_open_applyment` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否开启进件: 0-关闭, 1-开启',
  `is_support_check_bill` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否支持对账: 0-不支持, 1-支持',
  `is_open_check_bill` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否开启对账: 0-关闭, 1-开启',
  `is_support_cashout` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否支持提现: 0-不支持, 1-支持',
  `is_open_cashout` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否开启提现: 0-关闭, 1-开启',
  `isv_params` VARCHAR(4096) DEFAULT NULL COMMENT 'ISV接口配置定义描述,json字符串',
  `isvsub_mch_params` VARCHAR(4096) DEFAULT NULL COMMENT '特约商户接口配置定义描述,json字符串',
  `normal_mch_params` VARCHAR(4096) DEFAULT NULL COMMENT '普通商户接口配置定义描述,json字符串',
  `way_codes` JSON NOT NULL COMMENT '支持的支付方式 ["wxpay_jsapi", "wxpay_bar"]',
  `icon` VARCHAR(256) DEFAULT NULL COMMENT '页面展示：卡片-图标',
  `bg_color` VARCHAR(20) DEFAULT NULL COMMENT '页面展示：卡片-背景色',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '状态: 0-停用, 1-启用',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`if_code`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='支付接口定义表';

-- 支付接口配置参数表
DROP TABLE IF EXISTS `t_pay_interface_config`;
CREATE TABLE `t_pay_interface_config` (
  `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `info_type` VARCHAR(20) NOT NULL COMMENT '账号类型:ISV-服务商, ISV_OAUTH2-服务商oauth2, AGENT-代理商, MCH_APP-商户应用, MCH_APP_OAUTH2-商户应用oauth2',
  `info_id` VARCHAR(64) NOT NULL COMMENT '服务商号/商户号/应用ID',
  `if_code` VARCHAR(20) NOT NULL COMMENT '支付接口代码',
  `if_params` VARCHAR(4096) NOT NULL COMMENT '接口配置参数,json字符串',
  `sett_hold_day` TINYINT(4) NOT NULL DEFAULT '0' COMMENT '结算周期（自然日）',
  `if_rate` DECIMAL(20,6) DEFAULT NULL COMMENT '支付接口费率',
  `is_open_applyment` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否开启进件: 0-关闭, 1-开启',
  `is_open_cashout` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否开启提现: 0-关闭, 1-开启',
  `is_open_check_bill` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否开启对账: 0-关闭, 1-开启',
  `ignore_check_bill_mch_nos` VARCHAR(4096) DEFAULT NULL COMMENT '对账过滤子商户',
  `oauth2_info_id` VARCHAR(20) DEFAULT NULL COMMENT 'oauth2配置Id',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '状态: 0-停用, 1-启用',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_uid` BIGINT(20) DEFAULT NULL COMMENT '更新者用户ID',
  `updated_by` VARCHAR(64) DEFAULT NULL COMMENT '更新者姓名',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_InfoType_InfoId_IfCode` (`info_type`, `info_id`, `if_code`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='支付接口配置参数表';

-- 商户支付通道表 (允许商户  支付方式 对应多个支付接口的配置)
DROP TABLE IF EXISTS `t_mch_pay_passage`;
CREATE TABLE `t_mch_pay_passage` (
  `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `if_code` VARCHAR(20) NOT NULL COMMENT '支付接口',
  `way_code` VARCHAR(20) NOT NULL COMMENT '支付方式',
  `rate` DECIMAL(20,6) NOT NULL COMMENT '支付方式费率',
  `risk_config` JSON DEFAULT NULL COMMENT '风控数据',
  `state` TINYINT(6) NOT NULL COMMENT '状态: 0-停用, 1-启用',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_AppId_WayCode` (`app_id`,`if_code`, `way_code`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='商户支付通道表';


-- 轮询表
-- mch_no, way_code, 轮询策略。


-- 支付费率配置表
DROP TABLE IF EXISTS `t_pay_rate_config`;
CREATE TABLE `t_pay_rate_config` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'ID',
--   `config_mode` VARCHAR(20) NOT NULL COMMENT '配置模式: mgrIsv-服务商, mgrAgent-代理商, agentSubagent-子代理商, mgrMch-商户, agentMch-代理商商户',
  `config_type` VARCHAR(20) NOT NULL COMMENT '配置类型: ISVCOST-服务商低价, AGENTRATE-代理商费率, AGENTDEF-代理商默认费率, MCHAPPLYDEF-商户进件默认费率, MCHRATE-商户费率',
  `info_type` VARCHAR(20) NOT NULL COMMENT '账号类型:ISV-服务商, AGENT-代理商, MCH_APP-商户应用, MCH_APPLY-商户进件',
  `info_id` VARCHAR(64) NOT NULL COMMENT '服务商号/商户号/应用ID',
  `if_code` VARCHAR(20) NOT NULL COMMENT '支付接口',
  `way_code` VARCHAR(20) NOT NULL COMMENT '支付方式',
  `fee_type` VARCHAR(20) NOT NULL COMMENT '费率类型:SINGLE-单笔费率, LEVEL-阶梯费率',
  `level_mode` VARCHAR(20) NULL COMMENT '阶梯模式: 模式: NORMAL-普通模式, UNIONPAY-银联模式',
  `fee_rate` DECIMAL(20,6) NULL COMMENT '支付方式费率',
  `applyment_support` TINYINT NOT NULL COMMENT '是否支持进件: 0-不支持, 1-支持',
  `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_InfoId_WayCode` (`config_type`,`info_type`,`info_id`,`if_code`,`way_code`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='支付费率配置表';

-- 支付费率阶梯配置表
DROP TABLE IF EXISTS `t_pay_rate_level_config`;
CREATE TABLE `t_pay_rate_level_config` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `rate_config_id` BIGINT NOT NULL COMMENT '支付费率配置ID',
  `bank_card_type` VARCHAR(20) NULL COMMENT '银行卡类型: DEBIT-借记卡（储蓄卡）, CREDIT-贷记卡（信用卡）',
  `min_amount` INT NOT NULL DEFAULT '0' COMMENT '最小金额: 计算时大于此值', 
  `max_amount` INT NOT NULL DEFAULT '0' COMMENT '最大金额: 计算时小于或等于此值', 
  `min_fee` INT NOT NULL DEFAULT '0' COMMENT '保底费用', 
  `max_fee` INT NOT NULL DEFAULT '0' COMMENT '封顶费用', 
  `fee_rate` DECIMAL(20,6) NOT NULL COMMENT '支付方式费率',
  `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='支付费率阶梯配置表';

-- 支付订单表
DROP TABLE IF EXISTS `t_pay_order`;
CREATE TABLE `t_pay_order` (
  `pay_order_id` VARCHAR(30) NOT NULL COMMENT '支付订单号',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mch_name` VARCHAR(64) NOT NULL COMMENT '商户名称',
  `mch_short_name` VARCHAR(32) DEFAULT NULL COMMENT '商户简称',
  `agent_no` VARCHAR(64) DEFAULT NULL COMMENT '代理商号',
  `agent_name` VARCHAR(64) DEFAULT NULL COMMENT '代理商名称',
  `agent_short_name` VARCHAR(32) DEFAULT NULL COMMENT '代理商简称',
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `isv_name` VARCHAR(64) DEFAULT NULL COMMENT '服务商名称',
  `isv_short_name` VARCHAR(32) DEFAULT NULL COMMENT '服务商简称',
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `app_name` VARCHAR(64) DEFAULT NULL COMMENT '应用名称',
  `store_id` BIGINT(20) DEFAULT NULL COMMENT '门店ID',
  `store_name` VARCHAR(64) DEFAULT NULL COMMENT '门店名称',
  `qrc_id` VARCHAR(64) DEFAULT NULL COMMENT '二维码',
  `mch_type` TINYINT(6) NOT NULL COMMENT '类型: 1-普通商户, 2-特约商户(服务商模式)',
  `mch_order_no` VARCHAR(64) NOT NULL COMMENT '商户订单号',
  `if_code` VARCHAR(20) DEFAULT NULL COMMENT '支付接口代码',
  `way_code` VARCHAR(20) NOT NULL COMMENT '支付方式代码',
  `way_type` VARCHAR(20) NOT NULL COMMENT '支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, DCEPPAY-数字人民币, OTHER-其他',
  `amount` BIGINT(20) NOT NULL COMMENT '支付金额,单位分',
  `mch_fee_rate` DECIMAL(20,6) NOT NULL COMMENT '商户手续费费率快照',
  `mch_fee_rate_desc` VARCHAR(128) DEFAULT NULL COMMENT '商户手续费费率快照描述',
  `mch_fee_amount` BIGINT(20) NOT NULL COMMENT '商户手续费(实际手续费),单位分',
  `mch_order_fee_amount` BIGINT(20) NOT NULL COMMENT '收单手续费,单位分',
  `currency` VARCHAR(3) NOT NULL DEFAULT 'CNY' COMMENT '三位货币代码,人民币: CNY',
  `state` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '支付状态: 0-订单生成, 1-支付中, 2-支付成功, 3-支付失败, 4-已撤销, 5-已退款, 6-订单关闭',
  `notify_state` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '向下游回调状态, 0-未发送,  1-已发送',
  `client_ip` VARCHAR(32) DEFAULT NULL COMMENT '客户端IP',
  `subject` VARCHAR(64) NOT NULL COMMENT '商品标题',
  `body` VARCHAR(256) NOT NULL COMMENT '商品描述信息',
  `seller_remark` VARCHAR(256) DEFAULT NULL COMMENT '买家备注',
  `buyer_remark` VARCHAR(256) DEFAULT NULL COMMENT '卖家备注',
  `channel_mch_no` VARCHAR(64) DEFAULT NULL COMMENT '渠道商户号',
  `channel_isv_no` VARCHAR(64) DEFAULT NULL COMMENT '渠道服务商机构号',
  `channel_extra` VARCHAR(512) DEFAULT NULL COMMENT '特定渠道发起额外参数',
  `channel_user` VARCHAR(64) DEFAULT NULL COMMENT '渠道用户标识,如微信openId,支付宝账号',
  `channel_order_no` VARCHAR(64) DEFAULT NULL COMMENT '渠道订单号',
  `platform_order_no` VARCHAR(64) DEFAULT NULL COMMENT '用户支付凭证交易单号 微信/支付宝流水号',
  `platform_mch_order_no` VARCHAR(64) DEFAULT NULL COMMENT '用户支付凭证商户单号',
  `refund_state` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '退款状态: 0-未发生实际退款, 1-部分退款, 2-全额退款',
  `refund_times` INT(11) NOT NULL DEFAULT '0' COMMENT '退款次数',
  `refund_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '退款总金额,单位分',
  `division_mode` TINYINT(6) DEFAULT '0' COMMENT '订单分账模式：0-该笔订单不允许分账, 1-支付成功按配置自动完成分账, 2-商户手动分账(解冻商户金额)',
  `division_state` TINYINT(6) DEFAULT '0' COMMENT '订单分账状态：0-未发生分账, 1-等待分账任务处理, 2-分账处理中, 3-分账任务已结束(不体现状态)',
  `division_last_time` DATETIME DEFAULT NULL COMMENT '最新分账时间',
  `err_code` VARCHAR(128) DEFAULT NULL COMMENT '渠道支付错误码',
  `err_msg` VARCHAR(256) DEFAULT NULL COMMENT '渠道支付错误描述',
  `ext_param` VARCHAR(128) DEFAULT NULL COMMENT '商户扩展参数',
  `notify_url` VARCHAR(128) NOT NULL DEFAULT '' COMMENT '异步通知地址',
  `return_url` VARCHAR(128) DEFAULT '' COMMENT '页面跳转地址',
  `expired_time` DATETIME DEFAULT NULL COMMENT '订单失效时间',
  `success_time` DATETIME DEFAULT NULL COMMENT '订单支付成功时间',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`pay_order_id`),
  UNIQUE KEY `Uni_MchNo_MchOrderNo` (`mch_no`, `mch_order_no`),
  INDEX `Idx_CreatedAt` (`created_at`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='支付订单表';

-- 支付订单分润表
DROP TABLE IF EXISTS `t_pay_order_profit`;
CREATE TABLE `t_pay_order_profit` (
  `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `info_id` VARCHAR(64) NOT NULL COMMENT 'PLATFORM_PROFIT-运营平台利润账户, PLATFORM_INACCOUNT-运营平台入账账户, 代理商号',
  `info_name` VARCHAR(64) NOT NULL COMMENT '运营平台, 代理商名称',
  `info_type` VARCHAR(20) NOT NULL COMMENT 'PLATFORM-运营平台, AGENT-代理商',
  `pay_order_id` VARCHAR(30) NOT NULL COMMENT '支付订单号（与t_pay_order对应）',
  `fee_rate` DECIMAL(20,6) NOT NULL COMMENT '费率快照',
  `fee_rate_desc` VARCHAR(128) DEFAULT NULL COMMENT '费率快照描述',
  `profit_rate` DECIMAL(20,6) NOT NULL COMMENT '分润点数（利润率）',
  `profit_amount` BIGINT NOT NULL COMMENT '分润金额(实际分润),单位分',
  `order_profit_amount` BIGINT NOT NULL COMMENT '收单分润金额,单位分',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_InfoId_InfoType_PayOrderId` (`info_id`,`info_type`,`pay_order_id`),
  KEY `Idx_CreatedAt` (`created_at`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='支付订单分润表';

-- 商户通知记录表
DROP TABLE IF EXISTS `t_mch_notify_record`;
CREATE TABLE `t_mch_notify_record` (
  `notify_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '商户通知记录ID',
  `order_id` VARCHAR(64) NOT NULL COMMENT '订单ID',
  `order_type` TINYINT(6) NOT NULL COMMENT '订单类型: 1-支付,2-退款',
  `mch_order_no` VARCHAR(64) NOT NULL COMMENT '商户订单号',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `agent_no` VARCHAR(64) DEFAULT NULL COMMENT '代理商号',
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `notify_url` TEXT NOT NULL COMMENT '通知地址',
  `req_method` VARCHAR(10) NOT NULL COMMENT '通知请求方法',
  `req_media_type` VARCHAR(64) NOT NULL COMMENT '通知请求媒体类型',
  `req_body` TEXT COMMENT '通知请求正文',
  `res_result` TEXT COMMENT '通知响应结果',
  `notify_count` INT(11) NOT NULL DEFAULT '0' COMMENT '通知次数',
  `notify_count_limit` INT(11) NOT NULL DEFAULT '6' COMMENT '最大通知次数, 默认6次',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '通知状态,1-通知中,2-通知成功,3-通知失败',
  `last_notify_time` DATETIME DEFAULT NULL COMMENT '最后一次通知时间',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`notify_id`),
  UNIQUE KEY `Uni_OrderId_Type` (`order_id`, `order_type`)
) ENGINE=INNODB AUTO_INCREMENT=1001 DEFAULT CHARSET=utf8mb4 COMMENT='商户通知记录表';

-- 订单接口数据快照（加密存储）
DROP TABLE IF EXISTS `t_order_snapshot`;
CREATE TABLE `t_order_snapshot` (
  `order_id` VARCHAR(64) NOT NULL COMMENT '订单ID',
  `order_type` TINYINT(6) NOT NULL COMMENT '订单类型: 1-支付, 2-退款',
  `mch_req_data` TEXT DEFAULT NULL COMMENT '下游请求数据',
  `mch_req_time` DATETIME DEFAULT NULL COMMENT '下游请求时间',
  `mch_resp_data` TEXT DEFAULT NULL COMMENT '向下游响应数据',
  `mch_resp_time` DATETIME DEFAULT NULL COMMENT '向下游响应时间',
  `channel_req_data` TEXT DEFAULT NULL COMMENT '向上游请求数据',
  `channel_req_time` DATETIME DEFAULT NULL COMMENT '向上游请求时间',
  `channel_resp_data` TEXT DEFAULT NULL COMMENT '上游响应数据',
  `channel_resp_time` DATETIME DEFAULT NULL COMMENT '上游响应时间',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`order_id`, `order_type`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='订单接口数据快照';

-- 退款订单表
DROP TABLE IF EXISTS `t_refund_order`;
CREATE TABLE `t_refund_order` (
  `refund_order_id` VARCHAR(30) NOT NULL COMMENT '退款订单号（支付系统生成订单号）',
  `pay_order_id` VARCHAR(30) NOT NULL COMMENT '支付订单号（与t_pay_order对应）',
  `channel_pay_order_no` VARCHAR(64) DEFAULT NULL COMMENT '渠道支付单号（与t_pay_order channel_order_no对应）',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mch_name` VARCHAR(64) NOT NULL COMMENT '商户名称',
  `mch_short_name` VARCHAR(32) DEFAULT NULL COMMENT '商户简称',
  `agent_no` VARCHAR(64) DEFAULT NULL COMMENT '代理商号',
  `agent_name` VARCHAR(64) DEFAULT NULL COMMENT '代理商名称',
  `agent_short_name` VARCHAR(32) DEFAULT NULL COMMENT '代理商简称',
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `isv_name` VARCHAR(64) DEFAULT NULL COMMENT '服务商名称',
  `isv_short_name` VARCHAR(32) DEFAULT NULL COMMENT '服务商简称',
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `app_name` VARCHAR(64) DEFAULT NULL COMMENT '应用名称',
  `store_id` BIGINT(20) DEFAULT NULL COMMENT '门店ID',
  `store_name` VARCHAR(64) DEFAULT NULL COMMENT '门店名称',
  `mch_type` TINYINT(6) NOT NULL COMMENT '类型: 1-普通商户, 2-特约商户(服务商模式)',
  `mch_refund_no` VARCHAR(64) NOT NULL COMMENT '商户退款单号（商户系统的订单号）',
  `way_code` VARCHAR(20) NOT NULL COMMENT '支付方式代码',
  `way_type` VARCHAR(20) NOT NULL COMMENT '支付类型: WECHAT-微信, ALIPAY-支付宝, YSFPAY-云闪付, UNIONPAY-银联, DCEPPAY-数字人民币, OTHER-其他',
  `if_code` VARCHAR(20) NOT NULL COMMENT '支付接口代码',
  `pay_amount` BIGINT(20) NOT NULL COMMENT '支付金额,单位分',
  `refund_amount` BIGINT(20) NOT NULL COMMENT '退款金额,单位分',
  `refund_fee_amount` BIGINT(20) NOT NULL COMMENT '手续费退还金额,单位分',
  `currency` VARCHAR(3) NOT NULL DEFAULT 'CNY' COMMENT '三位货币代码,人民币: CNY',
  `state` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '退款状态:0-订单生成,1-退款中,2-退款成功,3-退款失败,4-退款任务关闭',
  `client_ip` VARCHAR(32) DEFAULT NULL COMMENT '客户端IP',
  `refund_reason` VARCHAR(256) NOT NULL COMMENT '退款原因',
  `channel_order_no` VARCHAR(32) DEFAULT NULL COMMENT '渠道订单号',
  `err_code` VARCHAR(128) DEFAULT NULL COMMENT '渠道错误码',
  `err_msg` VARCHAR(2048) DEFAULT NULL COMMENT '渠道错误描述',
  `channel_extra` VARCHAR(512) DEFAULT NULL COMMENT '特定渠道发起时额外参数',
  `notify_url` VARCHAR(128) DEFAULT NULL COMMENT '通知地址',
  `ext_param` VARCHAR(64) DEFAULT NULL COMMENT '扩展参数',
  `success_time` DATETIME DEFAULT NULL COMMENT '订单退款成功时间',
  `expired_time` DATETIME DEFAULT NULL COMMENT '退款失效时间（失效后系统更改为退款任务关闭状态）',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`refund_order_id`),
  UNIQUE KEY `Uni_MchNo_MchRefundNo` (`mch_no`, `mch_refund_no`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='退款订单表';

-- 转账订单表
DROP TABLE IF EXISTS `t_transfer_order`;
CREATE TABLE `t_transfer_order` (
  `transfer_id` VARCHAR(32) NOT NULL COMMENT '转账订单号',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `mch_name` VARCHAR(64) NOT NULL COMMENT '商户名称',
  `mch_short_name` VARCHAR(32) DEFAULT NULL COMMENT '商户简称',
  `agent_no` VARCHAR(64) DEFAULT NULL COMMENT '代理商号',
  `isv_no` VARCHAR(64) DEFAULT NULL COMMENT '服务商号',
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `mch_type` TINYINT(6) NOT NULL COMMENT '类型: 1-普通商户, 2-特约商户(服务商模式)',
  `mch_order_no` VARCHAR(64) NOT NULL COMMENT '商户订单号',
  `if_code` VARCHAR(20) NOT NULL COMMENT '支付接口代码',
  `entry_type` VARCHAR(20) NOT NULL COMMENT '入账方式： WX_CASH-微信零钱; ALIPAY_CASH-支付宝转账; BANK_CARD-银行卡',
  `amount` BIGINT(20) NOT NULL COMMENT '转账金额,单位分',
  `currency` VARCHAR(3) NOT NULL DEFAULT 'CNY' COMMENT '三位货币代码,人民币: CNY',
  `account_no` VARCHAR(64) NOT NULL COMMENT '收款账号',
  `account_name` VARCHAR(64) DEFAULT NULL COMMENT '收款人姓名',
  `bank_name` VARCHAR(32) DEFAULT NULL COMMENT '收款人开户行名称',
  `transfer_desc` VARCHAR(128) NOT NULL DEFAULT '' COMMENT '转账备注信息',
  `client_ip` VARCHAR(32) DEFAULT NULL COMMENT '客户端IP',
  `state` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '支付状态: 0-订单生成, 1-转账中, 2-转账成功, 3-转账失败, 4-订单关闭',
  `channel_extra` VARCHAR(512) DEFAULT NULL COMMENT '特定渠道发起额外参数',
  `channel_order_no` VARCHAR(64) DEFAULT NULL COMMENT '渠道订单号',
  `err_code` VARCHAR(128) DEFAULT NULL COMMENT '渠道支付错误码',
  `err_msg` VARCHAR(256) DEFAULT NULL COMMENT '渠道支付错误描述',
  `ext_param` VARCHAR(128) DEFAULT NULL COMMENT '商户扩展参数',
  `notify_url` VARCHAR(128) NOT NULL DEFAULT '' COMMENT '异步通知地址',
  `success_time` DATETIME DEFAULT NULL COMMENT '转账成功时间',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`transfer_id`),
  UNIQUE KEY `Uni_MchNo_MchOrderNo` (`mch_no`, `mch_order_no`),
  INDEX `Idx_CreatedAt` (`created_at`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='转账订单表';

-- 商户分账接收者账号组
DROP TABLE IF EXISTS `t_mch_division_receiver_group`;
CREATE TABLE `t_mch_division_receiver_group` (
  `receiver_group_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '组ID',
  `receiver_group_name` VARCHAR(64) NOT NULL COMMENT '组名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `auto_division_flag` TINYINT(6) NOT NULL DEFAULT 0 COMMENT '自动分账组（当订单分账模式为自动分账，改组将完成分账逻辑） 0-否 1-是',
  `created_uid` BIGINT(20) NOT NULL COMMENT '创建者用户ID',
  `created_by` VARCHAR(64) NOT NULL COMMENT '创建者姓名',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`receiver_group_id`)
) ENGINE=INNODB AUTO_INCREMENT=100001 DEFAULT CHARSET=utf8mb4 COMMENT='分账账号组';

-- 商户分账接收者账号绑定关系表
DROP TABLE IF EXISTS `t_mch_division_receiver`;
CREATE TABLE `t_mch_division_receiver` (
  `receiver_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '分账接收者ID',
  `receiver_alias` VARCHAR(64) NOT NULL COMMENT '接收者账号别名',
  `receiver_group_id` BIGINT(20) COMMENT '组ID（便于商户接口使用）',
  `receiver_group_name` VARCHAR(64) COMMENT '组名称',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `isv_no` VARCHAR(64) COMMENT '服务商号',
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `if_code` VARCHAR(20) NOT NULL COMMENT '支付接口代码',
  `acc_type` TINYINT(6) NOT NULL COMMENT '分账接收账号类型: 0-个人(对私) 1-商户(对公)',
  `acc_no` VARCHAR(50) NOT NULL COMMENT '分账接收账号',
  `acc_name` VARCHAR(30) NOT NULL DEFAULT '' COMMENT '分账接收账号名称',
  `relation_type` VARCHAR(30) NOT NULL COMMENT '分账关系类型（参考微信）， 如： SERVICE_PROVIDER 服务商等',
  `relation_type_name` VARCHAR(30) NOT NULL COMMENT '当选择自定义时，需要录入该字段。 否则为对应的名称',
  `division_profit` DECIMAL(20,6) COMMENT '分账比例',
  `state` TINYINT(6) NOT NULL COMMENT '分账状态（本系统状态，并不调用上游关联关系）: 1-正常分账, 0-暂停分账',
  `channel_acc_no` TEXT COMMENT '渠道账号信息',
  `channel_bind_result` TEXT COMMENT '上游绑定返回信息，一般用作查询账号异常时的记录',
  `channel_ext_info` TEXT COMMENT '渠道特殊信息',
  `bind_success_time` DATETIME DEFAULT NULL COMMENT '绑定成功时间',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`receiver_id`)
) ENGINE=INNODB AUTO_INCREMENT=800001 DEFAULT CHARSET=utf8mb4 COMMENT='商户分账接收者账号绑定关系表';

-- 分账记录表
DROP TABLE IF EXISTS `t_pay_order_division_record`;
CREATE TABLE `t_pay_order_division_record` (
  `record_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '分账记录ID',
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `isv_no` VARCHAR(64) COMMENT '服务商号',
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `mch_name` VARCHAR(30) NOT NULL COMMENT '商户名称',
  `mch_type` TINYINT(6) NOT NULL COMMENT '类型: 1-普通商户, 2-特约商户(服务商模式)',
  `if_code` VARCHAR(20)  NOT NULL COMMENT '支付接口代码',
  `pay_order_id` VARCHAR(30) NOT NULL COMMENT '系统支付订单号',
  `pay_order_channel_order_no` VARCHAR(64) COMMENT '支付订单渠道支付订单号',
  `pay_order_amount` BIGINT(20) NOT NULL COMMENT '订单金额,单位分',
  `pay_order_division_amount` BIGINT(20) NOT NULL COMMENT '订单实际分账金额, 单位：分（订单金额 - 商户手续费 - 已退款金额）',
  `batch_order_id` VARCHAR(30) NOT NULL COMMENT '系统分账批次号',
  `channel_batch_order_id` VARCHAR(64) COMMENT '上游分账批次号',
  `state` TINYINT(6) NOT NULL COMMENT '状态: 0-待分账 1-分账成功（明确成功）, 2-分账失败（明确失败）, 3-分账已受理（上游受理）',
  `channel_resp_result` TEXT COMMENT '上游返回数据包',
  `receiver_id` BIGINT(20) NOT NULL COMMENT '账号快照》 分账接收者ID',
  `receiver_group_id` BIGINT(20) COMMENT '账号快照》 组ID（便于商户接口使用）',
  `receiver_alias` VARCHAR(64) COMMENT '接收者账号别名',
  `acc_type` TINYINT(6) NOT NULL COMMENT '账号快照》 分账接收账号类型: 0-个人 1-商户',
  `acc_no` VARCHAR(50) NOT NULL COMMENT '账号快照》 分账接收账号',
  `acc_name` VARCHAR(30) NOT NULL DEFAULT '' COMMENT '账号快照》 分账接收账号名称',
  `relation_type` VARCHAR(30) NOT NULL COMMENT '账号快照》 分账关系类型（参考微信）， 如： SERVICE_PROVIDER 服务商等',
  `relation_type_name` VARCHAR(30) NOT NULL COMMENT '账号快照》 当选择自定义时，需要录入该字段。 否则为对应的名称',
  `division_profit` DECIMAL(20,6) NOT NULL COMMENT '账号快照》 配置的实际分账比例',
  `cal_division_amount` BIGINT(20) NOT NULL COMMENT '计算该接收方的分账金额,单位分',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`record_id`)
) ENGINE=INNODB AUTO_INCREMENT=1001 DEFAULT CHARSET=utf8mb4 COMMENT='分账记录表';

-- 码牌模板信息表
DROP TABLE IF EXISTS `t_qr_code_shell`;
CREATE TABLE `t_qr_code_shell` (
  `id` INT(11) NOT NULL AUTO_INCREMENT COMMENT '码牌模板ID',
  `style_code` VARCHAR(20) NOT NULL COMMENT '样式代码: shellA, shellB', 
  `shell_alias` VARCHAR(20) NOT NULL COMMENT '模板别名',
  `config_info` VARCHAR(4096) NOT NULL COMMENT '模板配置信息,json字符串',
  `shell_img_view_url` VARCHAR(255) COMMENT '模板预览图Url',
--   `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心', 
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '所属商户ID / 代理商ID / 0(平台)', 
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (id)
) ENGINE=INNODB AUTO_INCREMENT=1001 DEFAULT CHARSET=utf8mb4 COMMENT='码牌模板信息表';

-- 码牌信息表
DROP TABLE IF EXISTS `t_qr_code`;
CREATE TABLE `t_qr_code` (
  -- `id` INT(11) NOT NULL AUTO_INCREMENT,
  `qrc_id` VARCHAR(64) NOT NULL COMMENT '码牌ID',
  `qrc_shell_id` INT COMMENT '模板ID', 
  `batch_id` VARCHAR(64) NULL COMMENT '批次号',
  `fixed_flag` TINYINT NOT NULL COMMENT '是否固定金额: 0-任意金额, 1-固定金额', 
  `fixed_pay_amount` INT NOT NULL DEFAULT '0' COMMENT '固定金额',
  `entry_page` VARCHAR(20) NOT NULL COMMENT '选择页面类型: default-默认(未指定，取决于二维码是否绑定到微信侧), h5-固定H5页面, lite-固定小程序页面', 
  `alipay_way_code` VARCHAR(20) NOT NULL COMMENT '支付宝支付方式(仅H5呈现时生效): ALI_JSAPI ALI_WAP', 
  `qrc_alias` VARCHAR(20) COMMENT '码牌别名',
  `bind_state` TINYINT NOT NULL COMMENT '码牌绑定状态: 0-未绑定, 1-已绑定',  
  `agent_no` VARCHAR(64) NULL COMMENT '代理商号',
  `mch_no` VARCHAR(64) NULL COMMENT '商户号',
  -- `mch_name` VARCHAR(30) NOT NULL COMMENT '商户名称',
  `app_id` VARCHAR(64) NULL COMMENT '应用ID',
  `store_id` BIGINT NULL COMMENT '门店ID',
  `qr_url` VARCHAR(255) NOT NULL COMMENT '二维码Url', 
  -- `qrc_state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用', 
  `state` TINYINT NOT NULL COMMENT '状态: 0-停用, 1-启用',
  -- `qrc_belong_type` INT NOT NULL DEFAULT '1' COMMENT '获取方式: 1-自制, 2-下发',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心', 
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '所属商户ID / 代理商ID / 0(平台)', 
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (qrc_id)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='码牌信息表';

-- 设备供应商定义表
DROP TABLE IF EXISTS `t_device_provider_define`;
CREATE TABLE `t_device_provider_define` (
  `provider_code` VARCHAR(20) NOT NULL COMMENT '供应商代码 全小写 zgwl',
  `provider_name` VARCHAR(20) NOT NULL COMMENT '供应商名称',
--   `provider_type` TINYINT(6) NOT NULL COMMENT '供应商类型:1-云音响 2-云打印',
  `config_page_type` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '支付参数配置页面类型:1-JSON渲染,2-自定义',
  `provider_params` VARCHAR(4096) DEFAULT NULL COMMENT '供应商配置定义描述,json字符串',
  `device_types` JSON NOT NULL COMMENT '支持设备类型 [1, 2]',
  `icon` VARCHAR(256) DEFAULT NULL COMMENT '页面展示：卡片-图标',
  `bg_color` VARCHAR(20) DEFAULT NULL COMMENT '页面展示：卡片-背景色',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '状态: 0-停用, 1-启用',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`provider_code`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='设备供应商定义表';

-- 设备厂商配置参数表
DROP TABLE IF EXISTS `t_device_provider_config`;
CREATE TABLE `t_device_provider_config` (
  `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `device_type` TINYINT(6) NOT NULL COMMENT '设备类型:1-云音响 2-云打印',
  `provider_code` VARCHAR(20) NOT NULL COMMENT '供应商代码',
  `provider_params` VARCHAR(4096) NOT NULL COMMENT '设备供应商配置参数,json字符串',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '状态: 0-停用, 1-启用',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_DeviceType_Provider` (`device_type`, `provider_code`)
) ENGINE=INNODB DEFAULT CHARSET=utf8mb4 COMMENT='设备厂商配置参数表';

-- 设备信息表
DROP TABLE IF EXISTS `t_device_info`;
CREATE TABLE `t_device_info` (
  `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `device_no` VARCHAR(64) NOT NULL COMMENT '设备号',
  `device_name` VARCHAR(64) NOT NULL COMMENT '设备名称',
  `config_id` BIGINT(20) NOT NULL COMMENT '关联设备厂商配置参数ID',
  `device_type` TINYINT(6) NOT NULL COMMENT '设备类型:1-云音响 2-云打印',
  `provider_code` VARCHAR(20) NOT NULL COMMENT '供应商代码',
  `batch_id` VARCHAR(64) NULL COMMENT '批次号',
  `bind_state` TINYINT NOT NULL COMMENT '绑定状态: 0-未绑定, 1-已绑定',
  `bind_type` TINYINT NOT NULL COMMENT '绑定类型: 0-门店, 1-码牌', 
  `mch_no` VARCHAR(64) NOT NULL COMMENT '商户号',
  `app_id` VARCHAR(64) NOT NULL COMMENT '应用ID',
  `store_id` BIGINT NOT NULL COMMENT '门店ID',
  `bind_qrc_id` BIGINT NOT NULL COMMENT '绑定码牌ID',
  `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '状态: 0-停用, 1-启用',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  `sys_type` VARCHAR(8) NOT NULL COMMENT '所属系统: MGR-运营平台, AGENT-代理商平台, MCH-商户中心', 
  `belong_info_id` VARCHAR(64) NOT NULL COMMENT '所属商户ID / 代理商ID / 0(平台)', 
  `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间',
  `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Uni_ProviderCode_DeviceType_DeviceNo` (`provider_code`, `device_type`, `device_no`)
) ENGINE=INNODB AUTO_INCREMENT=1001 DEFAULT CHARSET=utf8mb4 COMMENT='设备信息表';

#####  ↑↑↑↑↑↑↑↑↑↑  表结构DDL  ↑↑↑↑↑↑↑↑↑↑  #####

#####  ↓↓↓↓↓↓↓↓↓↓  初始化DML  ↓↓↓↓↓↓↓↓↓↓  #####

#####  ↓↓↓↓↓↓↓↓↓↓  运营平台初始化DML  ↓↓↓↓↓↓↓↓↓↓  #####

-- 权限表数据 （ 不包含根目录 ）
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_COMMONS', '系统通用菜单', 'no-icon', '', 'RouteView', 'MO', 0, 1,  'ROOT', '-1', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_USERINFO', '个人中心', 'no-icon', '/current/userinfo', 'CurrentUserInfo', 'MO', 0, 1,  'ENT_COMMONS', '-1', 'MGR', NOW(), NOW());

INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN', '主页', 'home', '/main', 'MainPage', 'ML', 0, 1,  'ROOT', '1', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_AMOUNT_WEEK', '主页周支付统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_NUMBER_COUNT', '主页数量总统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_COUNT', '主页交易统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_TYPE_COUNT', '主页交易方式统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_ISV_MCH_COUNT', '服务商/商户统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_DAY_COUNT', '今日/昨日交易统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_TREND_COUNT', '趋势图统计	', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'MGR', NOW(), NOW());

-- 商户管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH', '商户管理', 'shop', '', 'RouteView', 'ML', 0, 1,  'ROOT', '30', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO', '商户列表', 'profile', '/mch', 'MchListPage', 'ML', 0, 1,  'ENT_MCH', '10', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_LIST', '页面：商户列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_CONFIG', '应用配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_CONFIG_PAGE', '按钮：商户配置信息', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_OAUTH2_CONFIG_ADD', '按钮：oauth2配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_OAUTH2_CONFIG_VIEW', '按钮：oauth2配置详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_INFO', '0', 'MGR', NOW(), NOW());

    -- 应用管理
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP', '应用列表', 'appstore', '/apps', 'MchAppPage', 'ML', 0, 1,  'ENT_MCH', '20', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_LIST', '页面：应用列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_CONFIG_LIST', '应用支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_CONFIG_ADD', '应用支付参数配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_PAY_CONFIG_LIST', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_CONFIG_VIEW', '应用支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_PAY_CONFIG_LIST', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_PASSAGE_LIST', '应用支付通道配置列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_PASSAGE_CONFIG', '应用支付通道配置入口', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_PAY_PASSAGE_LIST', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_PASSAGE_ADD', '应用支付通道配置保存', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_PAY_PASSAGE_LIST', '0', 'MGR', NOW(), NOW());

    -- 门店管理
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE', '门店管理', 'profile', '/store', 'MchStorePage', 'ML', 0, 1,  'ENT_MCH', '40', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_LIST', '页面：门店列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_APP_DIS', '按钮：应用分配', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MGR', NOW(), NOW());

-- 代理商管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT', '代理商管理', 'shop', '', 'RouteView', 'ML', 0, 1,  'ROOT', '35', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_INFO', '代理商列表', 'profile', '/agent', 'AgentListPage', 'ML', 0, 1, 'ENT_AGENT', '10', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_LIST', '页面：代理商列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_AGENT_INFO', '0', 'MGR', NOW(), NOW());
 	INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_PAY_CONFIG_LIST', '代理商支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_PAY_CONFIG_VIEW', '代理商支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_PAY_CONFIG_LIST', 0, 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_PAY_CONFIG_ADD', '代理商支付参数配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_PAY_CONFIG_LIST', 0, 'MGR', NOW(), NOW());

-- 服务商管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV', '服务商管理', 'block', '', 'RouteView', 'ML', 0, 1,  'ROOT', '40', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_INFO', '服务商列表', 'profile', '/isv', 'IsvListPage', 'ML', 0, 1,  'ENT_ISV', '10', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_LIST', '页面：服务商列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_OAUTH2_CONFIG_VIEW', '按钮：oauth2配置详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_OAUTH2_CONFIG_ADD', '按钮：oauth2配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_INFO', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_PAY_CONFIG_LIST', '服务商支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_INFO', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_PAY_CONFIG_ADD', '服务商支付参数配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_PAY_CONFIG_LIST', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ISV_PAY_CONFIG_VIEW', '服务商支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ISV_PAY_CONFIG_LIST', '0', 'MGR', NOW(), NOW());

-- 佣金管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PROFIT', '佣金管理', 'wallet', '', 'RouteView', 'ML', 0, 1,  'ROOT', '45', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PROFIT_PLATFORM', '运营佣金统计', 'account-book', '/platformProfits', 'PlatformProfitPage', 'ML', 0, 1, 'ENT_PROFIT', '10', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PROFIT_PLATFORM_LIST', '页面：运营佣金统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PROFIT_PLATFORM', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ACCOUNT_BILL', '钱包流水', 'fund-view', '/accountBill', 'AccountBillPage', 'ML', 0, 1, 'ENT_PROFIT', '30', 'MGR', NOW(), NOW());
 	INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ACCOUNT_BILL_LIST', '页面：钱包流水列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ACCOUNT_BILL', 0, 'MGR', NOW(), NOW());
 	INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ACCOUNT_BILL_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_ACCOUNT_BILL', 0, 'MGR', NOW(), NOW());

-- 订单管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ORDER', '订单管理', 'transaction', '', 'RouteView', 'ML', 0, 1,  'ROOT', '50', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER', '支付订单', 'account-book', '/pay', 'PayOrderListPage', 'ML', 0, 1,  'ENT_ORDER', '10', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ORDER_LIST', '页面：订单列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PAY_ORDER', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PAY_ORDER', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER_REFUND', '按钮：订单退款', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PAY_ORDER', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER_SEARCH_PAY_WAY', '筛选项：支付方式', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PAY_ORDER', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_REFUND_ORDER', '退款订单', 'exception', '/refund', 'RefundOrderListPage', 'ML', 0, 1,  'ENT_ORDER', '20', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_REFUND_LIST', '页面：退款订单列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_REFUND_ORDER', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_REFUND_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_REFUND_ORDER', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_TRANSFER_ORDER', '转账订单', 'property-safety', '/transfer', 'TransferOrderListPage', 'ML', 0, 1,  'ENT_ORDER', '25', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_TRANSFER_ORDER_LIST', '页面：转账订单列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_TRANSFER_ORDER', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_TRANSFER_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_TRANSFER_ORDER', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_NOTIFY', '商户通知', 'notification', '/notify', 'MchNotifyListPage', 'ML', 0, 1,  'ENT_ORDER', '30', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_NOTIFY_LIST', '页面：商户通知列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_NOTIFY', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_NOTIFY_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_NOTIFY', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_NOTIFY_RESEND', '按钮：重发通知', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_NOTIFY', '0', 'MGR', NOW(), NOW());

-- 数据统计
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ORDER_STATISTIC', '数据统计', 'bar-chart', '', 'RouteView', 'ML', 0, 1,  'ROOT', '53', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_STATISTIC_TRANSACTION', '交易报表', 'account-book', '/statistic/transaction', 'TransactionPage', 'ML', 0, 1,  'ENT_ORDER_STATISTIC', '10', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_STATISTIC_MCH', '商户统计', 'line-chart', '/statistic/mch', 'MchCountPage', 'ML', 0, 1,  'ENT_ORDER_STATISTIC', '20', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_STATISTIC_MCH_STORE', '门店统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_STATISTIC_MCH', '10', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_STATISTIC_MCH_WAY_CODE', '支付方式统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_STATISTIC_MCH', '20', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_STATISTIC_MCH_WAY_TYPE', '支付类型统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_STATISTIC_MCH', '30', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_STATISTIC_AGENT', '代理商统计', 'area-chart', '/statistic/agent', 'AgentCountPage', 'ML', 0, 1,  'ENT_ORDER_STATISTIC', '30', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_STATISTIC_ISV', '服务商统计', 'fund', '/statistic/isv', 'IsvCountPage', 'ML', 0, 1,  'ENT_ORDER_STATISTIC', '40', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_STATISTIC_CHANNEL', '通道统计', 'project', '/statistic/channel', 'ChannelCountPage', 'ML', 0, 1,  'ENT_ORDER_STATISTIC', '50', 'MGR', NOW(), NOW());

-- 分账管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION', '分账管理', 'apartment', '', 'RouteView', 'ML', 0, 1,  'ROOT', '55', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP', '账号组管理', 'team', '/divisionReceiverGroup', 'DivisionReceiverGroupPage', 'ML', 0, 1,  'ENT_DIVISION', '10', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP_LIST', '页面：数据列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER_GROUP', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER_GROUP', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER_GROUP', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER_GROUP', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP_DELETE', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER_GROUP', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER', '收款账号管理', 'trademark', '/divisionReceiver', 'DivisionReceiverPage', 'ML', 0, 1,  'ENT_DIVISION', '20', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_LIST', '页面：数据列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_ADD', '按钮：新增收款账号', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_DELETE', '按钮：删除收款账号', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_EDIT', '按钮：修改账号信息', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECORD', '分账记录', 'unordered-list', '/divisionRecord', 'DivisionRecordPage', 'ML', 0, 1,  'ENT_DIVISION', '30', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECORD_LIST', '页面：数据列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECORD', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECORD_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECORD', '0', 'MGR', NOW(), NOW());

-- 支付配置菜单
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC', '支付配置', 'file-done', '', 'RouteView', 'ML', 0, 1,  'ROOT', '60', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_IF_DEFINE', '支付接口', 'interaction', '/ifdefines', 'IfDefinePage', 'ML', 0, 1,  'ENT_PC', '10', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_IF_DEFINE_LIST', '页面：支付接口定义列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_IF_DEFINE', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_IF_DEFINE_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_IF_DEFINE', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_IF_DEFINE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_IF_DEFINE', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_IF_DEFINE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_IF_DEFINE', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_IF_DEFINE_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_IF_DEFINE', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_IF_DEFINE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_IF_DEFINE', '0', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_WAY', '支付方式', 'appstore', '/payways', 'PayWayPage', 'ML', 0, 1,  'ENT_PC', '20', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_WAY_LIST', '页面：支付方式列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_WAY', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_WAY_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_WAY', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_WAY_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_WAY', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_WAY_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_WAY', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_WAY_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_WAY', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PC_WAY_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PC_WAY', '0', 'MGR', NOW(), NOW());

-- 设备配置
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE', '设备配置', 'appstore', '', 'RouteView', 'ML', 0, 1,  'ROOT', '70', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_QRC', '码牌', 'shop', '', 'RouteView', 'ML', 0, 1,  'ENT_DEVICE', '10', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_SHELL', '模板管理', 'file', '/shell', 'QrCodeShellPage', 'ML', 0, 1,  'ENT_QRC', '10', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_SHELL_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DEVICE_QRC_SHELL', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_SHELL_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC_SHELL', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_SHELL_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC_SHELL', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_SHELL_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC_SHELL', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_SHELL_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC_SHELL', '0', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC', '码牌管理', 'qrcode', '/qrc', 'QrCodePage', 'ML', 0, 1,  'ENT_QRC', '20', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());
 	    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DEVICE_QRC_EXPORT', '按钮：导出', 'no-icon', '', '', 'PB', 0, 1, 'ENT_DEVICE_QRC', '0', 'MGR', NOW(), NOW());

-- 系统管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_SYS_CONFIG', '系统管理', 'setting', '', 'RouteView', 'ML', 0, 1,  'ROOT', '200', 'MGR', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR', '用户角色管理', 'team', '', 'RouteView', 'ML', 0, 1,  'ENT_SYS_CONFIG', '10', 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER', '操作员管理', 'contacts', '/users', 'SysUserPage', 'ML', 0, 1,  'ENT_UR', '10', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_LIST', '页面：操作员列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_SEARCH', '按钮：搜索', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_ADD', '按钮：添加操作员', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_VIEW', '按钮： 详情', '', 'no-icon', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_EDIT', '按钮： 修改基本信息', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_DELETE', '按钮： 删除操作员', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_UPD_ROLE', '按钮： 角色分配', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_LOGIN_LIMIT_DELETE', '按钮：解除登录限制', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MGR', NOW(), NOW());

        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE', '角色管理', 'user', '/roles', 'RolePage', 'ML', 0, 1,  'ENT_UR', '20', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_LIST', '页面：角色列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_ADD', '按钮：添加角色', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_DIST', '按钮： 分配权限', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_EDIT', '按钮： 修改基本信息', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_DEL', '按钮： 删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MGR', NOW(), NOW());

        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_ENT', '权限管理', 'apartment', '/ents', 'EntPage', 'ML', 0, 1,  'ENT_UR', '30', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_ENT_LIST', '页面： 权限列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE_ENT', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_ENT_EDIT', '按钮： 权限变更', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE_ENT', '0', 'MGR', NOW(), NOW());

    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_SYS_CONFIG_INFO', '系统配置', 'setting', '/config', 'SysConfigPage', 'ML', 0, 1,  'ENT_SYS_CONFIG', '15', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_SYS_CONFIG_EDIT', '按钮： 修改', 'no-icon', '', '', 'PB', 0, 1,  'ENT_SYS_CONFIG_INFO', '0', 'MGR', NOW(), NOW());
    
    -- 公告管理
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ARTICLE_NOTICEINFO', '公告管理', 'message', '/notices', 'NoticeInfoPage', 'ML', 0, 1,  'ENT_SYS_CONFIG', '30', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_NOTICE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE_NOTICEINFO', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_NOTICE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE_NOTICEINFO', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_NOTICE_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE_NOTICEINFO', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_NOTICE_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE_NOTICEINFO', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_NOTICE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_ARTICLE_NOTICEINFO', '0', 'MGR', NOW(), NOW());
    
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_SYS_LOG', '系统日志', 'file-text', '/log', 'SysLogPage', 'ML', 0, 1,  'ENT_SYS_CONFIG', '40', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_LOG_LIST', '页面：系统日志列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_SYS_LOG', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_SYS_LOG_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_SYS_LOG', '0', 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_SYS_LOG_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_SYS_LOG', '0', 'MGR', NOW(), NOW());
    
#####  ↓↓↓↓↓↓↓↓↓↓  代理商系统初始化DML  ↓↓↓↓↓↓↓↓↓↓  #####

-- 权限表数据 （ 不包含根目录 ）
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_COMMONS', '系统通用菜单', 'no-icon', '', 'RouteView', 'MO', 0, 1, 'ROOT', '-1', 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_USERINFO', '个人中心', 'no-icon', '/current/userinfo', 'CurrentUserInfo', 'MO', 0, 1, 'ENT_COMMONS', -1, 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ARTICLE_NOTICEINFO', '公告管理', 'message', '/notices', 'NoticeInfoPage', 'MO', 0, 1,  'ENT_COMMONS', '-1', 'AGENT', NOW(), NOW());

INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN', '主页', 'home', '/main', 'MainPage', 'ML', 0, 1, 'ROOT', '1', 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_TYPE_COUNT', '主页交易方式统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_COUNT', '主页交易统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_AMOUNT_WEEK', '主页周支付统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_NUMBER_COUNT', '主页数量总统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_C_MAIN', 0, 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_ISV_MCH_COUNT', '服务商/商户统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_DAY_COUNT', '今日/昨日交易统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_TREND_COUNT', '趋势图统计	', 'no-icon', '', '', 'PB', 0, 1,  'ENT_C_MAIN', '0', 'AGENT', NOW(), NOW());

-- 账户中心
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_ACCOUNT_CENTER', '账户中心', 'wallet', '', 'RouteView', 'ML', 0, 1, 'ROOT', 5, 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_STATISTIC', '数据统计', 'fund-view', '/statistic', 'StatisticsPage', 'ML', 0, 1, 'ENT_AGENT_ACCOUNT_CENTER', 20, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_CURRENT_INFO', '代理商信息', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_STATISTIC', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_ORDER_STATISTIC', '订单/商户统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_STATISTIC', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_STATISTIC_COUNT', '代理商统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_STATISTIC', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_HARDWARE_STATISTIC', '硬件统计', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_STATISTIC', 0, 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_SELF_PAY_CONFIG', '费率配置', 'file-done', '/passageConfig', 'PayConfigPage', 'ML', 0, 1, 'ENT_AGENT_ACCOUNT_CENTER', 30, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_SELF_PAY_CONFIG_LIST', '费率配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_SELF_PAY_CONFIG', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_SELF_PAY_CONFIG_ADD', '费率配置保存', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_SELF_PAY_CONFIG', 0, 'AGENT', NOW(), NOW());

-- 商户管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH', '商户管理', 'shop', '', 'RouteView', 'ML', 0, 1, 'ROOT', '30', 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO', '商户列表', 'profile', '/mch', 'MchListPage', 'ML', 0, 1, 'ENT_MCH', 10, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_LIST', '页面：商户列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_CONFIG', '应用配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_INFO', 0, 'AGENT', NOW(), NOW());

    -- 应用管理
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP', '应用列表', 'appstore', '/apps', 'MchAppPage', 'ML', 0, 1, 'ENT_MCH', '20', 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_LIST', '页面：应用列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_PASSAGE_LIST', '应用支付通道配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_PASSAGE_CONFIG', '应用支付通道配置入口', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_PASSAGE_LIST', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_PASSAGE_ADD', '应用支付通道配置保存', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_PASSAGE_LIST', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_CONFIG_LIST', '应用支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_APP', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_CONFIG_ADD', '应用支付参数配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_CONFIG_VIEW', '应用支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
        
     -- 门店管理
     INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE', '门店管理', 'profile', '/store', 'MchStorePage', 'ML', 0, 1,  'ENT_MCH', '40', 'AGENT', NOW(), NOW());
         INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_LIST', '页面：门店列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
         INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
         INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
         INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_APP_DIS', '按钮：应用分配', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
         INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());
         INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'AGENT', NOW(), NOW());

-- 代理商管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT', '代理商管理', 'shop', '', 'RouteView', 'ML', 0, 1, 'ROOT', '35', 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_INFO', '代理商列表', 'profile', '/agent', 'AgentListPage', 'ML', 0, 1, 'ENT_AGENT', 10, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_LIST', '页面：代理商列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_INFO_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_PAY_CONFIG_LIST', '代理商支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_INFO', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_PAY_CONFIG_VIEW', '代理商支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_AGENT_PAY_CONFIG_ADD', '代理商支付参数配置', 'no-icon', '', '', 'PB', 0, 1, 'ENT_AGENT_PAY_CONFIG_LIST', 0, 'AGENT', NOW(), NOW());

-- 订单管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ORDER', '订单管理', 'transaction', '', 'RouteView', 'ML', 0, 1, 'ROOT', '50', 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER', '支付订单', 'account-book', '/pay', 'PayOrderListPage', 'ML', 0, 1, 'ENT_ORDER', 10, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ORDER_LIST', '页面：订单列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER_REFUND', '按钮：订单退款', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER_SEARCH_PAY_WAY', '筛选项：支付方式', 'no-icon', '', '', 'PB', 0, 1, 'ENT_PAY_ORDER', 0, 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_REFUND_ORDER', '退款订单', 'exception', '/refund', 'RefundOrderListPage', 'ML', 0, 1, 'ENT_ORDER', 20, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_REFUND_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_REFUND_ORDER', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_REFUND_LIST', '页面：退款订单列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_REFUND_ORDER', 0, 'AGENT', NOW(), NOW());

-- 系统管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_SYS_CONFIG', '系统管理', 'setting', '', 'RouteView', 'ML', 0, 1, 'ROOT', '200', 'AGENT', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR', '用户角色管理', 'team', '', 'RouteView', 'ML', 0, 1, 'ENT_SYS_CONFIG', 10, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER', '操作员管理', 'contacts', '/users', 'SysUserPage', 'ML', 0, 1, 'ENT_UR', 10, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_VIEW', '按钮：详情', '', 'no-icon', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_UPD_ROLE', '按钮：角色分配', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_SEARCH', '按钮：搜索', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_LIST', '页面：操作员列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_EDIT', '按钮：修改基本信息', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_DELETE', '按钮：删除操作员', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_ADD', '按钮：添加操作员', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_USER', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_LOGIN_LIMIT_DELETE', '按钮：解除登录限制', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM', '团队管理', 'team', '/teams', 'SysUserTeamPage', 'ML', 0, 1, 'ENT_UR', 15, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM_LIST', '页面：团队列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM_DEL', '按钮： 删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'AGENT', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE', '角色管理', 'user', '/roles', 'RolePage', 'ML', 0, 1, 'ENT_UR', 20, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_EDIT', '按钮：修改基本信息', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_DIST', '按钮：分配权限', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_ADD', '按钮：添加角色', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());		
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_LIST', '页面：角色列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_ROLE', 0, 'AGENT', NOW(), NOW());

#####  ↓↓↓↓↓↓↓↓↓↓  商户系统初始化DML  ↓↓↓↓↓↓↓↓↓↓  #####

-- 【商户系统】 主页
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_COMMONS', '系统通用菜单', 'no-icon', '', 'RouteView', 'MO', 0, 1,  'ROOT', '-1', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_USERINFO', '个人中心', 'no-icon', '/current/userinfo', 'CurrentUserInfo', 'MO', 0, 1,  'ENT_COMMONS', '-1', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ARTICLE_NOTICEINFO', '公告管理', 'message', '/notices', 'NoticeInfoPage', 'MO', 0, 1,  'ENT_COMMONS', '-1', 'MCH', NOW(), NOW());

INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MAIN', '主页', 'home', '/main', 'MainPage', 'ML', 0, 1,  'ROOT', '1', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MAIN_PAY_AMOUNT_WEEK', '主页周支付统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_MAIN', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MAIN_NUMBER_COUNT', '主页数量总统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_MAIN', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MAIN_PAY_COUNT', '主页交易统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_MAIN', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MAIN_PAY_TYPE_COUNT', '主页交易方式统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_MAIN', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MAIN_USER_INFO', '主页用户信息', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_MAIN', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_COUNT', '主页交易统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_MAIN', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_TYPE_COUNT', '主页交易方式统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_MAIN', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_DAY_COUNT', '今日/昨日交易统计', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_MAIN', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_C_MAIN_PAY_TREND_COUNT', '趋势图统计	', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_MAIN', '0', 'MCH', NOW(), NOW());

-- 【商户系统】 商户中心
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_CENTER', '商户中心', 'team', '', 'RouteView', 'ML', 0, 1, 'ROOT', '10', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_INFO', '商户信息', 'user', '/info', 'MchInfoPage', 'ML', 0, 1,  'ENT_MCH_CENTER', '0', 'MCH', NOW(), NOW());
    -- 应用管理
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP', '应用管理', 'appstore', '/apps', 'MchAppPage', 'ML', 0, 1,  'ENT_MCH_CENTER', '10', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_LIST', '页面：应用列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_APP_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_OAUTH2_CONFIG_VIEW', '按钮：oauth2配置详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_OAUTH2_CONFIG_ADD', '按钮：oauth2配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_CONFIG_LIST', '应用支付参数配置列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_CONFIG_ADD', '应用支付参数配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_PAY_CONFIG_LIST', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_CONFIG_VIEW', '应用支付参数配置详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_PAY_CONFIG_LIST', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_PASSAGE_LIST', '应用支付通道配置列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_APP', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_PASSAGE_CONFIG', '应用支付通道配置入口', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_PAY_PASSAGE_LIST', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_PASSAGE_ADD', '应用支付通道配置保存', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_PAY_PASSAGE_LIST', '0', 'MCH', NOW(), NOW());
    -- 门店管理
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE', '门店管理', 'shop', '/store', 'MchStorePage', 'ML', 0, 1,  'ENT_MCH_CENTER', '60', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_LIST', '页面：门店列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_APP_DIS', '按钮：应用分配', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_STORE_DEL', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_STORE', '0', 'MCH', NOW(), NOW());

    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_TEST', '支付测试', 'transaction', '/paytest', 'PayTestPage', 'ML', 0, 1,  'ENT_MCH_CENTER', '20', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_TEST_PAYWAY_LIST', '页面：获取全部支付方式', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_PAY_TEST', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_PAY_TEST_DO', '按钮：支付测试', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_PAY_TEST', '0', 'MCH', NOW(), NOW());

    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_TRANSFER', '转账', 'property-safety', '/doTransfer', 'MchTransferPage', 'ML', 0, 1,  'ENT_MCH_CENTER', '30', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_TRANSFER_IF_CODE_LIST', '页面：获取全部代付通道', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_TRANSFER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_TRANSFER_CHANNEL_USER', '按钮：获取渠道用户', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_TRANSFER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_TRANSFER_DO', '按钮：发起转账', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_TRANSFER', '0', 'MCH', NOW(), NOW());

-- 会员权限
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR', '会员中心', 'team', '', 'RouteView', 'ML', 0, 1, 'ROOT', '15', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_CONFIG', '会员配置', 'setting', '/mbr/mbrConfig', 'MbrConfigPage', 'ML', 0, 1, 'ENT_MCH_MBR', 5, 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_INFO', '会员管理', 'user', '/mbr/mbrInfo', 'MbrPage', 'ML', 0, 1, 'ENT_MCH_MBR', 10, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_INFO', 0, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_INFO_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_INFO', 0, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_INFO_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_INFO', 0, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_INFO_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_INFO', 0, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_INFO_MANUAL', '按钮：调账', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_INFO', 0, 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_RECHARGE_RULE', '充值规则', 'profile', '/mbr/rechargeRule', 'RechargeRulePage', 'ML', 0, 1, 'ENT_MCH_MBR', 20, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_RECHARGE_RULE_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_RECHARGE_RULE', 0, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_RECHARGE_RULE_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_RECHARGE_RULE', 0, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_RECHARGE_RULE_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_RECHARGE_RULE', 0, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_RECHARGE_RULE_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_RECHARGE_RULE', 0, 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_BILL', '会员账单', 'exception', '/mbr/account', 'MbrBillPage', 'ML', 0, 1, 'ENT_MCH_MBR', 30, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_BILL_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_BILL', 0, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_BILL_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_BILL', 0, 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_RECHARGE_ORDER', '充值记录', 'transaction', '/mbr/recharge', 'MbrRechargePage', 'ML', 0, 1, 'ENT_MCH_MBR', 40, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_RECHARGE_ORDER_LIST', '页面：列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_RECHARGE_ORDER', 0, 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_MBR_RECHARGE_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_MCH_MBR_RECHARGE_ORDER', 0, 'MCH', NOW(), NOW());

-- 【商户系统】 订单管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ORDER', '订单中心', 'transaction', '', 'RouteView', 'ML', 0, 1,  'ROOT', '20', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER', '订单管理', 'account-book', '/pay', 'PayOrderListPage', 'ML', 0, 1,  'ENT_ORDER', '10', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_ORDER_LIST', '页面：订单列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PAY_ORDER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PAY_ORDER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER_SEARCH_PAY_WAY', '筛选项：支付方式', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PAY_ORDER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_PAY_ORDER_REFUND', '按钮：订单退款', 'no-icon', '', '', 'PB', 0, 1,  'ENT_PAY_ORDER', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_REFUND_ORDER', '退款记录', 'exception', '/refund', 'RefundOrderListPage', 'ML', 0, 1,  'ENT_ORDER', '20', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_REFUND_LIST', '页面：退款订单列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_REFUND_ORDER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_REFUND_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_REFUND_ORDER', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_TRANSFER_ORDER', '转账订单', 'property-safety', '/transfer', 'TransferOrderListPage', 'ML', 0, 1,  'ENT_ORDER', '30', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_TRANSFER_ORDER_LIST', '页面：转账订单列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_TRANSFER_ORDER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_TRANSFER_ORDER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_TRANSFER_ORDER', '0', 'MCH', NOW(), NOW());

-- 【商户系统】 分账管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION', '分账管理', 'apartment', '', 'RouteView', 'ML', 0, 1,  'ROOT', '30', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP', '账号组管理', 'team', '/divisionReceiverGroup', 'DivisionReceiverGroupPage', 'ML', 0, 1,  'ENT_DIVISION', '10', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP_LIST', '页面：数据列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER_GROUP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER_GROUP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER_GROUP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP_EDIT', '按钮：修改', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER_GROUP', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_GROUP_DELETE', '按钮：删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER_GROUP', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER', '收款账号管理', 'trademark', '/divisionReceiver', 'DivisionReceiverPage', 'ML', 0, 1,  'ENT_DIVISION', '20', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_LIST', '页面：数据列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_ADD', '按钮：新增收款账号', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_DELETE', '按钮：删除收款账号', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECEIVER_EDIT', '按钮：修改账号信息', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECEIVER', '0', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECORD', '分账记录', 'unordered-list', '/divisionRecord', 'DivisionRecordPage', 'ML', 0, 1,  'ENT_DIVISION', '30', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECORD_LIST', '页面：数据列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECORD', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECORD_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECORD', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_DIVISION_RECORD_RESEND', '按钮：重试', 'no-icon', '', '', 'PB', 0, 1,  'ENT_DIVISION_RECORD', '0', 'MCH', NOW(), NOW());

-- 【商户系统】 系统管理
INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_SYS_CONFIG', '系统管理', 'setting', '', 'RouteView', 'ML', 0, 1,  'ROOT', '200', 'MCH', NOW(), NOW());
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR', '用户角色管理', 'team', '', 'RouteView', 'ML', 0, 1,  'ENT_SYS_CONFIG', '10', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER', '操作员管理', 'contacts', '/users', 'SysUserPage', 'ML', 0, 1,  'ENT_UR', '10', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_LIST', '页面：操作员列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_SEARCH', '按钮：搜索', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_ADD', '按钮：添加操作员', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_VIEW', '按钮： 详情', '', 'no-icon', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_EDIT', '按钮： 修改基本信息', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_DELETE', '按钮： 删除操作员', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_UPD_ROLE', '按钮： 角色分配', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_USER_LOGIN_LIMIT_DELETE', '按钮：解除登录限制', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_USER', '0', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM', '团队管理', 'team', '/teams', 'SysUserTeamPage', 'ML', 0, 1, 'ENT_UR', 15, 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM_LIST', '页面：团队列表', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM_ADD', '按钮：新增', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM_EDIT', '按钮：编辑', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM_VIEW', '按钮：详情', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_TEAM_DEL', '按钮： 删除', 'no-icon', '', '', 'PB', 0, 1, 'ENT_UR_TEAM', 0, 'MGR', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE', '角色管理', 'user', '/roles', 'RolePage', 'ML', 0, 1,  'ENT_UR', '20', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_LIST', '页面：角色列表', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_SEARCH', '页面：搜索', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_ADD', '按钮：添加角色', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_DIST', '按钮： 分配权限', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_EDIT', '按钮： 修改名称', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MCH', NOW(), NOW());
            INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_UR_ROLE_DEL', '按钮： 删除', 'no-icon', '', '', 'PB', 0, 1,  'ENT_UR_ROLE', '0', 'MCH', NOW(), NOW());
    
    INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_CONFIG', '系统配置', 'setting', '/config', 'MchConfigPage', 'ML', 0, 1,  'ENT_SYS_CONFIG', '30', 'MCH', NOW(), NOW());
        INSERT INTO `t_sys_entitlement` (`ent_id`, `ent_name`, `menu_icon`, `menu_uri`, `component_name`, `ent_type`, `quick_jump`, `state`, `pid`, `ent_sort`, `sys_type`, `created_at`, `updated_at`) VALUES ('ENT_MCH_CONFIG_EDIT', '按钮：修改系统配置', 'no-icon', '', '', 'PB', 0, 1,  'ENT_MCH_CONFIG', '0', 'MCH', NOW(), NOW());

-- 默认角色
INSERT INTO `t_sys_role` (`role_id`, `role_name`, `sys_type`, `belong_info_id`, `updated_at`) VALUES ('ROLE_ADMIN', '系统管理员', 'MGR', '0', NOW());
INSERT INTO `t_sys_role` (`role_id`, `role_name`, `sys_type`, `belong_info_id`, `updated_at`) VALUES ('ROLE_OP', '普通操作员', 'MGR', '0', NOW());
-- 角色权限关联， [超管]用户 拥有所有权限
-- insert into t_sys_role_ent_rela select '801', ent_id from t_sys_entitlement;

-- 超管用户： agpayadmin / agpay123
INSERT INTO `t_sys_user` (`sys_user_id`, `login_username`, `realname`, `telphone`, `sex`, `avatar_url`, `user_no`, `user_type`, `state`, `sys_type`, `belong_info_id`, `created_at`,`updated_at`) VALUES (801, 'agpayadmin', '超管', '13000000001', '1', 'https://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/defava_m.png', 'D0001', 1, 1, 'MGR', '0', NOW(), NOW());
INSERT INTO `t_sys_user_auth` (`auth_id`, `user_id`, `identity_type`, `identifier`, `credential`, `salt`, `sys_type`) VALUES (801, '801', '1', 'agpayadmin', '$2a$11$Kx.LaVw2ArNAlbBaZkh9UueiP24R1uXQAzPnhG.6zwY1wxOpmBD3e', 'testkey', 'MGR');

-- insert into t_sys_user_role_rela values (801, 801);

-- 初始化运营平台系统参数
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mgrSiteUrl', '运营平台网址(不包含结尾/)', '运营平台网址(不包含结尾/)', 'applicationConfig', '系统应用配置', 'http://127.0.0.1:9217', 'text', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('agentSiteUrl', '代理商平台网址(不包含结尾/)', '代理商平台网址(不包含结尾/)', 'applicationConfig', '系统应用配置', 'https://127.0.0.1:9816', 'text', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mchSiteUrl', '商户平台网址(不包含结尾/)', '商户平台网址(不包含结尾/)', 'applicationConfig', '系统应用配置', 'http://127.0.0.1:9218', 'text', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('paySiteUrl', '支付网关地址(不包含结尾/)', '支付网关地址(不包含结尾/)', 'applicationConfig', '系统应用配置', 'http://127.0.0.1:9216', 'text', 'MGR', '0', 0, NOW());

INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('ossUseType', '文件上传服务类型', '文件上传服务类型', 'ossConfig', '文件上传服务', 'localFile', 'radio', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('ossPublicSiteUrl', '公共oss访问地址(不包含结尾/)', '公共oss访问地址(不包含结尾/)', 'ossConfig', '系统应用配置', 'http://127.0.0.1:9217/api/anon/localOssFiles', 'text', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('aliyunOssConfig', '阿里云oss配置', '阿里云oss配置', 'ossConfig', '阿里云oss配置', '{"accessKeyId":"LTAI4GEqjdMVqr6y7xTjsTo1","endpoint":"oss-cn-beijing.aliyuncs.com","expireTime":30000,"publicBucketName":"jeepaypublic","privateBucketName":"jeepayprivate","accessKeySecret":"lsMY95aWVv8Ghuoq91sDeNAU76xIYo"}', 'text', 'MGR', '0', 0, NOW());

INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('apiMapWebKey', '[高德地图商户端web配置]Key', '高德地图Key', 'apiMapConfig', '高德地图商户端web配置', '6cebea39ba50a4c9bc565baaf57d1c8b', 'text', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('apiMapWebSecret', '[高德地图商户端web配置]秘钥', '高德地图Key', 'apiMapConfig', '高德地图商户端web配置', 'dccbb5a56d2a1850eda2b6e67f8f2f13', 'text', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('apiMapWebServiceKey', '[高德地图商户端web服务]Key', '商户端web服务key', 'apiMapConfig', '高德地图商户端web配置', '1e558c3dc1ce7ab2a0b332d78fcd4c16', 'text', 'MGR', '0', 0, NOW());

INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('smsProviderKey', '短信使用厂商', '短信使用厂商', 'smsConfig', '短信配置', 'agpaydx', 'radio', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('agpaydxSmsConfig', '[吉日短信]短信配置', '[吉日短信]短信配置', 'smsConfig', '短信配置', '{"signName":"吉日付","userName":"agooday","accountPwd":"agooday"}', 'text', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('aliyundySmsConfig', '阿里云短信服务', '阿里云短信服务', 'smsConfig', '短信配置', '{"accessKeyId":"LTAI5tChn8DqogEzgm5FSyhZ","loginMchTemplateId":"SMS_178515044","registerMchTemplateId":"SMS_215795545","accessKeySecret":"u17oHlUkGe9l7q9aoApV7boNe3GlGe","signName":"吉日科技","mbrTelBindTemplateId":"SMS_215790589","forgetPwdTemplateId":"SMS_215795546","accountOpenTemplateId":"SMS_234420379"}', 'text', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mocktestSmsConfig', '[模拟测试]短信配置', '[模拟测试]短信配置', 'smsConfig', '短信配置', '{"mockCode": "888666"}', 'text', 'MGR', '0', 0, NOW());

INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('ocrType', 'OCR识别使用类型', 'OCR识别使用类型 1-腾讯OCR 2-阿里OCR', 'ocrConfig', 'OCR识别参数配置', '1', 'radio', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('ocrState', 'OCR识别使用状态', 'OCR识别使用状态 0-关闭 1-开启', 'ocrConfig', 'OCR识别参数配置', '1', 'radio', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('tencentOcrConfig', '腾讯OCR识别参数配置', '腾讯OCR识别参数配置', 'ocrConfig', 'OCR识别参数配置', '{"secretId":"AKIDHK7ewxhBOKzNTJr88svhCUVFiuqVsyoN","secretKey":"JL7cqnTs1tUord9QQ9blfIejY6NM5Xje"}', 'text', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('aliOcrConfig', '阿里OCR识别参数配置', '阿里OCR识别参数配置', 'ocrConfig', 'OCR识别参数配置', '{"accessKeyId": "LTAI4GEqjdMVqr6y7xTjsTo1","accessKeySecret": "lsMY95aWVv8Ghuoq91sDeNAU76xIYo"}', 'text', 'MGR', '0', 0, NOW());

INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('loginErrorMaxLimit', '限制登录次数(xx分钟最多尝试xx次)(0表示不限制)', '限制登录次数', 'securityConfig', '安全配置', '{"limitMinute":15,"maxLoginAttempts":3}', 'text', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('passwordRegexp', '密码规则', '密码规则(正则表达式)', 'securityConfig', '安全配置', '{"regexpRules":"^.{6,}$","errTips":"密码不符合规则，最少6位"}', 'text', 'MGR', '0', 0, NOW());

INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mchPrivacyPolicy', '隐私政策', '隐私政策', 'mchTreatyConfig', '隐私政策', '<h2 style="text-indent: 0px; text-align: start;">隐私政策</h2><p style="text-indent: 0px; text-align: start;">吉日商户通APP（以下简称“我们”）尊重并保护所有吉日商户通用户的个人信息及隐私安全。为了给您提供更准确、更有个性化的服务，我们依据《中华人民共和国网络安全法》、《信息安全技术&nbsp;个人信息安全规范》以及其他相关法律法规和技术规范明确了我们收集/使用/披露您的个人信息的原则。本隐私政策进一步阐述了关于您个人信息的相关权利。</p><p style="text-indent: 0px; text-align: start;">本政策与您所使用的我们的产品与/或服务息息相关，请在使用/继续使用我们的各项产品与服务前，仔细阅读并充分理解本政策。如果您不同意本政策的内容，将可能导致我们的产品/服务无法正常运行，您应立即停止访问/使用我们的产品与/或服务。若您同意本《隐私政策》，即视为您已经仔细阅读/充分理解，并同意本《隐私政策》的全部内容。若您使用/继续使用我们提供的产品与/或服务的行为，均视为您已仔细阅读/充分理解，并同意本隐私政策的全部内容。</p><p style="text-indent: 0px; text-align: start;">本隐私政策属于吉日商户通APP产品与/或服务使用协议不可分割的一部分。</p><h3 style="text-indent: 0px; text-align: start;">一、我们如何收集和使用您的个人信息</h3><p style="text-indent: 0px; text-align: start;">我们会遵循正当、合法、必要的原则，处于对本政策所述的以下目的，收集和使用您在使用我们的产品与/或服务时所主动提供，或因使用我们的产品与/或服务时被动产生的个人信息。除本政策另有规定外，在未征得您事先许可的情况下，我们不会将这些信息对外披露或提供给第三方。若我们需要将您的个人信息用于本政策未载明的其他用途，或基于特定目的将已经收集的信息用于其他目的，我们将以合理的方式告知您，并在使用前征得您的同意。</p><h4 style="text-indent: 0px; text-align: start;">1.账号注册及登录</h4><p style="text-indent: 0px; text-align: start;">1.1当您注册吉日商户通APP账号时，您需要根据吉日商户通APP的要求提供您的个人注册信息，我们会收集您所填写的商户名称、手机号码以及您所选择的商户类型。</p><p style="text-indent: 0px; text-align: start;">1.2为了给您提供更合理的服务，当您登录吉日商户通APP时，我们会使用您的用户ID/手机号，以确认您账号所属的商户信息。</p><h4 style="text-indent: 0px; text-align: start;">2.向您提供产品与/或服务时</h4><p style="text-indent: 0px; text-align: start;">2.1信息浏览、管理、修改、新增等功能。</p><p style="text-indent: 0px; text-align: start;">当您使用吉日商户通APP中的信息浏览、管理、修改和新增等功能时，我们会请求您授权照片、相机、和存储功能的权限。如果您拒绝授权提供，将无法使用相应的功能，但并不影响您使用吉日商户通APP的其他功能。</p><p style="text-indent: 0px; text-align: start;">2.1.1当您使用用户头像修改/上传等功能时，我们会请求您授权存储功能的权限，如果您拒绝授权提供，将无法使用相应功能。但并不影响您使用吉日商户通APP的其他功能。</p><p style="text-indent: 0px; text-align: start;">2.1.2当您使用吉日商户通APP中的编辑个人信息、门店管理、码牌管理、云喇叭管理、云打印机管理等功能时，您所提供的图片、文字、状态等信息将会上传并存储至云端服务器中，由于存储是实现以上功能及其多端同步的必要条件。我们会以加密的方式存储这些信息，您也可以随时修改这些信息。</p><p style="text-indent: 0px; text-align: start;">2.2安全运行。为了保障软件与服务的安全运行，我们会收集您的设备型号、设备名称、设备唯一标识符（包括：IMEI、IMSI、Android&nbsp;ID、IDFA）、浏览器类型和设置、使用的语言、操作系统和应用程序版本、网络设备硬件地址、访问日期和时间、登录IP地址、接入网络的方式等。</p><p style="text-indent: 0px; text-align: start;">2.3搜索功能。当您使用吉日商户通APP提供的搜索服务时，我们会收集您所输入的关键词信息、访问时间信息。这些信息是搜索功能的必要条件。</p><p style="text-indent: 0px; text-align: start;">2.4扫码。当您使用吉日商户通APP提供的扫一扫支付、绑定新码、扫码获取云喇叭/打印机设备号等功能和/或服务时，我们会请求您授权相机的权限。如果您拒绝授权提供，将无法使用上述功能。</p><p style="text-indent: 0px; text-align: start;">2.5收款。当您使用吉日商户通APP提供的收款功能时、我们会收集该笔收款订单的订单号、收款金额、收款时间、支付时间、支付方式、订单状态、门店信息、用户信息，这些信息用于生成详细的订单记录。</p><p style="text-indent: 0px; text-align: start;">2.6退款。当您使用吉日商户通APP提供的订单退款功能时、我们会收集该笔订单的订单号、订单金额、支付金额、退款金额、支付时间、退款时间、门店信息、用户信息，这些信息用于生成详细的退款记录，并对比收退款金额，以限制退款金额不能大于支付金额。</p><p style="text-indent: 0px; text-align: start;">2.7查询。当您使用吉日商户通APP提供的订单记录、门店列表、码牌列表、云喇叭列表、云打印机列表、收款通知接收人列表等功能时，我们会收集您的账户信息和商户ID，用于展示在您查询权限内的信息。</p><p style="text-indent: 0px; text-align: start;">2.8拨号。当您使用吉日商户通APP，关于我们-联系电话中的快捷拨号功能时，我们会请求您设备的拨号权限。如果您拒绝授权提供，将无法使用快捷拨号功能，但不会影响您使用吉日商户通APP的其他服务。</p><p style="text-indent: 0px; text-align: start;">2.9设备权限调用汇总</p><p style="text-indent: 0px; text-align: start;">我们对吉日商户通APP为您提供服务时，所需要您授权的设备权限汇总如下。注意：您可以拒绝其中部分权限，但将无法使用需要该权限的功能和服务。您可以随时取消已授权的设备权限，不同设备权限显示方式和关闭方式可能有所不同，具体请参考设备及操作系统开发方的说明和操作指引：</p><table style="text-align: start;"><tbody><tr><td colspan="1" rowspan="1"><strong>设备权限</strong></td><td colspan="1" rowspan="1"><strong>对应业务功能</strong></td><td colspan="1" rowspan="1"><strong>功能场景说明</strong></td><td colspan="1" rowspan="1"><strong>取消授权</strong></td></tr><tr><td colspan="1" rowspan="1">相机</td><td colspan="1" rowspan="1">扫一扫收款与喇叭/云打印机新增、修改绑定新码</td><td colspan="1" rowspan="1">1.扫描消费者付款码，使用相机识别二维码。&nbsp;2.扫描未绑定的码牌时，使用相机识别二维码。3.获取云打印机/云喇叭设备号时，使用相机识别设备条码</td><td colspan="1" rowspan="1">​该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">访问照片</td><td colspan="1" rowspan="1">头像上传通道申请上传照片</td><td colspan="1" rowspan="1">1.用户头像上传时我们需要访问您的相册，以便您选取要上传的照片2.通道申请时上传企业营业执照、法人身份证照片、商户门头照等信息时，我们需要访问您的相册，以便您选取要上传的照片</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">储存</td><td colspan="1" rowspan="1">APP稳定运行下载码牌图片</td><td colspan="1" rowspan="1">1.日志信息记录、信息缓存2.下载码牌图片至手机相册中</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">获取电话状态（设备信息）</td><td colspan="1" rowspan="1">APP安全运行</td><td colspan="1" rowspan="1">1.本政策第2.2条描述</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr></tbody></table><h4 style="text-indent: 0px; text-align: start;">3.征得授权同意的例外</h4><p style="text-indent: 0px; text-align: start;">根据相关法律法规的规定，在以下情形中，我们可以在不征得您的授权同意的情况下收集、使用一些必要的个人信息：</p><p style="text-indent: 0px; text-align: start;">（1）与我们旅行法律法规规定的义务相关的；</p><p style="text-indent: 0px; text-align: start;">（2）与国家安全、国防安全直接相关的；</p><p style="text-indent: 0px; text-align: start;">（3）与公共安全、公共卫生、重大公共利益直接相关的；</p><p style="text-indent: 0px; text-align: start;">（4）与犯罪侦查、起诉、审判和判决执行及相关事项直接相关的；</p><p style="text-indent: 0px; text-align: start;">（5）出于维护您或其他个人的生命、财产及相关重大合法权益但有很难得到本人同意的；</p><p style="text-indent: 0px; text-align: start;">（6）所收集的个人信息是您自行向社会公众公开的；</p><p style="text-indent: 0px; text-align: start;">（7）从合法公开披露的信息中收集到您的客人信息：从合法的新闻报道、政府信息公开等相关渠道；</p><p style="text-indent: 0px; text-align: start;">（8）根据您与平台签署的在线协议或合同所必需的；</p><p style="text-indent: 0px; text-align: start;">（9）用于维护我们产品和/或服务的安全稳定运行所必需的：发现、处置产品或服务的故障及相关问题处理；</p><p style="text-indent: 0px; text-align: start;">（10）法律法规规定的其他情形。</p><h3 style="text-indent: 0px; text-align: start;">二、我们如何共享、转让、公开披露您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.共享</h4><p style="text-indent: 0px; text-align: start;">对于您的个人信息，我们不会与任何公司、组织和个人进行共享，除非存在以下一种或多种情形：</p><p style="text-indent: 0px; text-align: start;">（1）事先已获得您的授权；</p><p style="text-indent: 0px; text-align: start;">（2）您自行提出的；</p><p style="text-indent: 0px; text-align: start;">（3）与商业合作伙伴的必要共享：</p><p style="text-indent: 0px; text-align: start;">您理解并知悉，为了向您提供更完善、优质的产品和服务；或由于您在使用吉日商户通中由第三方服务提供企业/机构所提供的服务时的情况下，我们将授权第三方服务提供企业/机构为您提供部分服务。此种情况下，我们可能会与合作伙伴共享您的某些个人信息，其中包括您已授权或自行提出的（包括但不限于商户名称、手机号、法人信息、商户营业执照等）必要信息，以及您在使用本APP时自动产生的某些信息（包括订单、订单金额、交易时间、收款方式、收款金额、门店信息、退款信息）。请您注意、我们仅处于合法、正当、必要、特定、明确的目的共享这些信息。我们将对信息数据的输出形式、流转、使用进行安全评估与处理，以保护数据安全。同时，我们会对合作伙伴、服务商机构进行严格的监督与管理，一但发现其存在违规处理个人信息的行为，将立即停止合作并追究其法律责任。</p><p style="text-indent: 0px; text-align: start;">目前，我们的合作伙伴包括以下类型：</p><p style="text-indent: 0px; text-align: start;">A.第三方支付机构：当您使用吉日商户通提供的支付业务时，将会使用并通过第三方支付机构的支付通道，其中包括但不限于微信支付、支付宝支付、银联云闪付支付等第三方支付平台。我们会与第三方支付机构共享来自于您的部分交易信息。为保障您在使用我们所提供的收款功能/服务时的合理、合规及合法性，在您正式使用前述功能/服务前，您需要向对应的第三方支付机构发起支付通道申请，在此情况下，我们会收集您所主动提供的商户名称、企业名称、法人信息、营业执照、账户信息等必要信息，并将上述信息与第三方支付机构共享。</p><p style="text-indent: 0px; text-align: start;">（4）您可以基于吉日商户通APP与第三人（包括不特定对象）共享您的个人信息或其他信息，但因您的共享行为而导致的信息泄露、被使用及其他相关请何况，与吉日商户通APP无关，吉日商户通不因此承担法律责任。</p><h4 style="text-indent: 0px; text-align: start;">2.转让</h4><p style="text-indent: 0px; text-align: start;">转让是指将取得您个人信息的控制权转让给其他公司、组织或个人。除非获取您的明确同意，否则我们不会将您的个人信息转让给任何公司、组织或个人。但下述情形除外：</p><p style="text-indent: 0px; text-align: start;">（1）已事先征得您的同意；</p><p style="text-indent: 0px; text-align: start;">（2）您自行提出的；</p><p style="text-indent: 0px; text-align: start;">（3）如果公司发生合并、收购或破产清算，将可能涉及到个人信息转让，此种情况下我们会告知您有关情况并要求新的持有您个人信息的公司、组织继续受本政策的约束。否则我们将要求其重新获取您的明示同意。</p><p style="text-indent: 0px; text-align: start;">（4）其他法律法规规定的情形。</p><h4 style="text-indent: 0px; text-align: start;">3.公开披露</h4><p style="text-indent: 0px; text-align: start;">公开披露是指向社会或不特定人群发布信息的行为。原则上，我们不会对您的个人信息进行公开披露。但下述情况除外：</p><p style="text-indent: 0px; text-align: start;">（1）取得您的明示同意后。</p><h4 style="text-indent: 0px; text-align: start;">4.共享、转让、公开披露个人信息授权同意的例外情形</h4><p style="text-indent: 0px; text-align: start;">根据相关法律法规的固定，在以下情形中，我们可能在未事先征得您的授权同意的情况下共享、转让、公开披露您的个人信息：</p><p style="text-indent: 0px; text-align: start;">（1）与我们履行法律法规规定的义务相关的，含依照法律规定、司法机关或行政机关强制要求向有权机关披露您的个人信息；在该种情况下，我们会要求披露请求方出局与其请求相应的有效法律文件，并对被披露的信息采取符合法律和业界标准的安全防护措施；</p><p style="text-indent: 0px; text-align: start;">（2）与国家安全、国防安全直接相关的；</p><p style="text-indent: 0px; text-align: start;">（3）与公共安全、公共卫生、重大公共利益直接相关的；</p><p style="text-indent: 0px; text-align: start;">（4）与犯罪侦擦、&nbsp;起诉、审判和判决执行及相关事项直接相关的；</p><p style="text-indent: 0px; text-align: start;">（5）出于维护您或其他个人的生命、财产及相关重大合法权益但又很难得到本人同意的；</p><p style="text-indent: 0px; text-align: start;">（6）您自行向社会公众公开的个人信息；</p><p style="text-indent: 0px; text-align: start;">（7）从合法公开披露的信息中收集到的个人信息：合法的新闻报道、政府信息公开及其他相关渠道；</p><p style="text-indent: 0px; text-align: start;">（8）法律法规规定的其他情形。</p><p style="text-indent: 0px; text-align: start;">请您了解，根据现行法律规定及监管要求，共享、转让经去标识化处理的个人信息，且确保数据接收方无法复原并重新识别个人信息主体的，无需另行向您通知并征得您的同意。</p><h3 style="text-indent: 0px; text-align: start;">三、我们如何存储和保护您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.存储</h4><p style="text-indent: 0px; text-align: start;">存储地点：我们将从中华人民共和国境内获得的个人信息存放于中华人民共和国境内。如果发生个人信息的跨境传输，我们会单独向您以邮件或通知的方式告知您个人信息处境的目的与接受方，并征得您的同意。我们会严格遵守中华人民共和国法律法规，确保数据接收方有充足的数据能力来保护您的个人信息安全。</p><p style="text-indent: 0px; text-align: start;">存储时间：我们承诺始终按照法律的规定在合理必要期限内存储您的个人信息。超出上述期限后，我们将删除您的个人信息或对您的个人信息进行脱敏、去标识化、匿名化处理。</p><p style="text-indent: 0px; text-align: start;">如果我们停止运营，我们将立即停止收集您的个人信息，并将停止运营这一决定和/或事实以右键或通知的方式告知您，并对所有已获取的您的个人信息进行删除或匿名化处理。</p><h4 style="text-indent: 0px; text-align: start;">2.保护</h4><p style="text-indent: 0px; text-align: start;">为了您的个人信息安全，我们将采用各种符合行业标准的安全措施来保护您的个人信息安全，以最大程度降低您的个人信息被损毁、泄露、盗用、非授权方式访问、使用、披露和更改的风险。我们将积极建立数据分类分级制度、数据安全管理规范、数据安全开发规范来管理规范个人信息的采集、存储和使用，确保未收集与我们提供的产品和服务无关的信息。</p><p style="text-indent: 0px; text-align: start;">在数据存储安全上，我们与第三方机构合作，包括但不限于阿里云、腾讯云等。</p><p style="text-indent: 0px; text-align: start;">为保障您的账户和个人信息的安全，请妥善保管您的账户及密码信息。我们将通过多端服务器备份的方式保障您的信息不丢失、损毁或因不可抗力因素而导致的无法使用。通过第三方存储服务机构提供的堡垒机、安全防火墙等服务，保障您的信息不被滥用、变造和泄露。</p><p style="text-indent: 0px; text-align: start;">尽管有上述安全措施，但也请您注意：在信息网络上并不存在绝对“完善的安全措施”。为防止安全事故的发生，我们已按照法律法规规定，制定了妥善的预警机制和应急预案。如确发生安全事件，我们将及时将相关情况以邮件、电话、信函、短信等方式告知您、难以逐一告知单一个体时，我们会采取合理、有效的方式发布公告。同时，我们还将按照监管部门要求，主动上报个人信息安全事件的处置情况、紧密配合政府机关的工作。</p><p style="text-indent: 0px; text-align: start;">当我们的产品或服务发生停止运营的情况下，我们将立即停止收集您的个人信息，并将停止运营这一决定和/或事实以邮件或通知的方式告知您，并对所有已获取的您的个人信息进行删除或匿名化处理。</p><h4 style="text-indent: 0px; text-align: start;">3.匿名化处理</h4><p style="text-indent: 0px; text-align: start;">为保障我们已收集到的您的个人信息的安全，当我们不再提供服务、停止运营或因其他不可抗力因素而不得不销毁您的个人信息的情况下。我们将会采取删除或匿名化的方式处理您的个人信息。</p><p style="text-indent: 0px; text-align: start;">匿名化是指通过对个人信息的技术处理，使个人信息的主体无法被识别，且处理后无法被复原的过程。严格意义上，匿名化后的信息不属于个人信息。</p><h3 style="text-indent: 0px; text-align: start;">四、您如何管理您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.自主决定授权信息</h4><p style="text-indent: 0px; text-align: start;">您可以自主决定是否授权我们向您申请的某些设备权限，具体请参考第一条，2.9所述。</p><p style="text-indent: 0px; text-align: start;">注意：根据不同的操作系统及硬件设备，您管理这些权限的方式可能会有所不同，具体操作方式，请参照您的设备或操作系统开发方的说明和操作指引。</p><h4 style="text-indent: 0px; text-align: start;">2.访问、获取、更改和删除相关信息</h4><p style="text-indent: 0px; text-align: start;">您可以通过交互本APP的交互界面对相关信息进行访问、获取、更改和删除。</p><p style="text-indent: 0px; text-align: start;">（1）您登录账户的名称和头像：</p><p style="text-indent: 0px; text-align: start;">您可以通过在“我的”页面，通过点击头像一栏的按钮进入修改个人信息页面，对个人名称进行查看和修改。通过点击个人头像，查看您的账户头像，授权我们访问您的相册后，您可以更改您的账户头像。</p><p style="text-indent: 0px; text-align: start;">（2）门店信息：</p><p style="text-indent: 0px; text-align: start;">您可以在“首页-门店管理”页面，通过点击门店列表中的单个门店进入所选门店的编辑页面，对门店名称和备注信息进行查看和修改。通过点击状态条目中的开关按钮，您可以启用或禁用所选门店的状态。</p><p style="text-indent: 0px; text-align: start;">（3）云喇叭/云打印机设备信息：</p><p style="text-indent: 0px; text-align: start;">您可以在“我的-云喇叭管理/云打印管理”页面，通过点击设备列表中的单个云喇叭/云打印机进入所选设备的编辑页面，对设备名称、设备编号、所属门店等信息进行查看和修改。通过点击状态条目中的开关按钮，您可以启用或禁用所选设备的状态。</p><h3 style="text-indent: 0px; text-align: start;">五、您如何注销您的账号</h3><p style="text-indent: 0px; text-align: start;">您可以通过第九条中指明的联系方式联系我们，并像我们阐明您注销账号的原因。或在本APP的”我的-设置-其他设置-注销账号“页面输入注销原因并点击提交按钮向我们提交您的注销申请。在满足账号注销的条件下，我们将尽快注销您的账号。注意：由于您账号在使用期间内产生的交易信息将不会被立刻处理，而是需要经过确认、复查后，确保该笔交易已完成所有流程后，进行脱敏处理。此外，除法律明确规定必须由我们保留的个人信息外，您在使用本APP期间内所产生或由您提交的其他个人信息将会被删除或匿名化处理，且该处理不可逆，您将无法找回这些个人信息。</p><h3 style="text-indent: 0px; text-align: start;">六、有关第三方提供产品和/或服务的特别说明</h3><p style="text-indent: 0px; text-align: start;">您在使用吉日商户通APP时，可能会使用到由第三方提供的产品和/或服务，在这种情况下，您需要接受该第三方的服务条款及隐私政策（而非本隐私政策）的约束，您需要仔细阅读其条款并自行决定是否接受。请您妥善保管您的个人信息，仅在必要的情况下向他人提供。本政策仅适用于我们所收集、保存、使用、共享、披露信息，并不适用于任何第三方提供服务时（包含您向该第三方提供的任何个人信息）或第三方信息的使用规则，第三方使用您的个人信息时的行为，由其自行负责。</p><h3 style="text-indent: 0px; text-align: start;">七、我们如何使用Cookie和其他同类技术</h3><p style="text-indent: 0px; text-align: start;">在您未拒绝接受cookies的情况下，我们会在您的计算机以及相关移动设备上设定或取用cookies，以便您能登录或使用依赖于cookies的吉日商户通的产品与/或服务。您有权选择接受或拒绝接受cookies。您可以通过修改浏览器设置的方式或在移动设备设置中设置拒绝我们使用cookies。若您拒绝使用cookies，则您可能无法登录或使用依赖于cookies的吉日商户通App网络服务或功能。</p><h3 style="text-indent: 0px; text-align: start;">八、更新隐私政策</h3><p style="text-indent: 0px; text-align: start;">我们保留更新或修订本隐私政策的权力。这些修订或更新构成本政策的一部分，并具有等同于本政策的效。未经您的同意，我们不会削减您依据当前生效的本政策所应享受的权利。</p><p style="text-indent: 0px; text-align: start;">我们会不时更新本政策，如遇本政策更新，我们会通过APP通知等相关合理方式通知您，如遇重大更新，您需要重新仔细阅读、充分理解并同意修订更新后的政策，才可继续使用我们所提供的产品和/或服务。</p><h3 style="text-indent: 0px; text-align: start;">九、联系我们</h3><p style="text-indent: 0px; text-align: start;">如果您对本政策有任何疑问，您可以通过以下方式联系我们，我们将尽快审核所涉问题，并在验证您的用户身份后予以答复。</p><p style="text-indent: 0px; text-align: start;">网站：<a href="https://www.agooday.com" target="_blank">www.agooday.com&nbsp;&nbsp;&nbsp;</a></p><h3 style="text-indent: 0px; text-align: start;">十、其他</h3><p style="text-indent: 0px; text-align: start;">如果您认为我们的个人信息处理行为损害了您的合法权益，您可以向有关政府部门进行反应。或因本政策以及我们处理您个人信息事宜引起的任何争议，您可以诉至沧州市人民法院。</p>', 'editor', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mchServiceAgreement', '用户服务协议', '用户服务协议', 'mchTreatyConfig', '用户服务协议', '<h2 style="text-indent: 0px; text-align: start;">用户服务协议</h2><p style="text-indent: 0px; text-align: start;">感谢您使用吉日支付，在使用“吉日商户通”软件及相关服务前，请您认真阅读本协议，并确认承诺同意遵守本协议的全部约定。本协议由您与吉日科技（武汉）有限公司（包括其关联机构，以下合成“本公司”）于您点击同意本协议之时，在湖北省沧州市签署并生效。</p><h3 style="text-indent: 0px; text-align: start;">一、协议条款的确认及接受</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>吉日支付（包括网址为<a href="https://www.agooday.com" target="_blank">www.agpay.com&nbsp;</a>的网站，以及可在IOS系统及Android&nbsp;系统中运行的名为“吉日商户通APP”、“吉日展业宝APP”、及其他不同版本的应用程序，以及名为“吉日商户通”、“吉日展业宝”的微信小程序，以下简称"本网站"或“吉日支付”）由吉日科技（武汉）有限公司（包括其关联机构，以下合称“本公司”）运营并享有完全的所有权及知识产权等权益，吉日支付提供的服务将完全按照其发布的条款和操作规则严格执行。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>您确认同意本协议（协议文本包括《吉日支付用户服务协议》、《吉日支付用户隐私政策》及吉日支付已公示或将来公示的各项规则及提示，所有前述协议、规则及提示乃不可分割的整体，具有同等法律效力，共同构成用户使用吉日支付及相关服务的整体协议，以下合称“本协议”）所有条款并完成注册程序时，本协议在您于本公司间成立并发生法律效力，同时您成为吉日支付正式用户。</p><h3 style="text-indent: 0px; text-align: start;">二、账号注册及使用规则</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>当您使用“吉日商户通”APP时，您可以在APP/微信小程序中的注册页，或在地址为<a href="https://mch.s.agpay.vip/register" target="_blank">https://mch.s.agpay.vip/register</a>的网页进行注册，注册成功后，吉日支付将给与您一个商户账号及相应的密码，该商户账号和密码有您负责保管，您应当对以其商户账号进行的所有活动和事件负法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>您须对在吉日支付注册信息的真实性、合法性、有效性承担全部责任，您不得冒充他人（包括但不限于冒用他人姓名、名称、字号、头像等足以让人引起混淆的方式，以及冒用他人手机号码）开设账号；不得利用他人的名义发布任何信息；不得利用他人的名义发起任何交易；不得恶意使用注册账户导致他人误认；否则吉日支付有权立即停止提供服务，收回账号，并由您独自承担由此而产生的一切法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>您理解且确认，您在吉日支付注册的账号的所有权及有关权益均归本公司所有，您完成注册手续后仅享有该账号的使用权（包括但不限于该账号绑定的由吉日支付提供的产品和/或服务）。您的账号仅限于您本人使用，未经本公司书面同意，禁止以任何形式赠与、借用、出租、转让、售卖或以其他任何形式许可他人使用该账号。如果本公司发现或有合理理由认为使用者并非账号初始注册人，公司有权在未通知您的请款修改，暂停或终止向该账号提供服务，并有权注销该账号，而无需向注册该账号的用户承担法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>4.</strong>您理解并确认，除用于登录、使用“吉日商户通”APP及相关服务外，您还可以使用您的注册账号登录使用吉日支付提供的其他商户产品和/或服务，以及其他本公司的合作伙伴、第三方服务提供商所提供的软件及服务。若您以吉日支付账号登录和/或使用前述服务时，同样应受到其他软件、服务实际提供方的用户协议及其他协议条款的约束。</p><p style="text-indent: 0px; text-align: start;"><strong>5.</strong>您理解并确认，部分由其他第三方平台（包括但不限于银联云闪付、微信支付、支付宝支付、随行付等）提供的产品及服务，在您使用吉日支付提供的产品和/或服务时，仅作为基础服务为您提供。您的吉日支付账号与您在上述第三方平台注册的第三方平台账号仅在技术层面上构成单方面绑定。如果您不使用/不继续使用吉日支付提供的产品和/或服务，您的第三方平台账号均不会受到影响，您可以继续第三方平台提供的产品及服务。</p><p style="text-indent: 0px; text-align: start;"><strong>6.</strong>您承诺不得以任何方式利用吉日支付直接或间接从事违反中国法律、社会公德的行为，吉日支付有权对违反上述承诺的内容予以屏蔽、留证，并将由您独自承担由此而产生的一切法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>7.</strong>您不得利用本网站制作、上载、复制、发布、传播或转载如下内容：</p><p style="text-indent: 0px; text-align: start;">（1）反对宪法所确定的基本原则的；</p><p style="text-indent: 0px; text-align: start;">（2）危害国家安全，泄露国家秘密，颠覆国家政权，破坏国家统一的；</p><p style="text-indent: 0px; text-align: start;">（3）损害国家荣誉和利益的；</p><p style="text-indent: 0px; text-align: start;">（4）煽动民族仇恨、民族歧视，破坏民族团结的；</p><p style="text-indent: 0px; text-align: start;">（5）破坏国家宗教政策，宣扬邪教和封建迷信的；</p><p style="text-indent: 0px; text-align: start;">（6）散布谣言，扰乱社会秩序，破坏社会稳定的；</p><p style="text-indent: 0px; text-align: start;">（7）散布淫秽、色情、赌博、暴力、凶杀、恐怖或者教唆犯罪的；</p><p style="text-indent: 0px; text-align: start;">（8）侮辱或者诽谤他人，侵害他人合法权益的；</p><p style="text-indent: 0px; text-align: start;">（9）侵害未成年人合法权益或者损害未成年人身心健康的；</p><p style="text-indent: 0px; text-align: start;">（10）含有法律、行政法规禁止的其他内容的信息。</p><p style="text-indent: 0px; text-align: start;"><strong>8.</strong>吉日支付有权对您使用我们的产品和/或服务时的情况进行审查和监督，如您在使用吉日支付时违反任何上述规定，本公司有权暂停或终止对您提供服务，以减轻您的不当行为所造成的影响。</p><h3 style="text-indent: 0px; text-align: start;">三、服务内容</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>本公司可能为不同的终端设备及使用需求开发不同的应用程序软件版本，您应当更具实际设备需求状况获取、下载、安装合适的版本。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>本网站的服务具体内容根据实际情况提供，吉日支付保留变更、终端或终止部分或全部服务的权力。吉日支付不承担因业务调整给您造成的损失。除非本协议另有其他明示规定，增加或强化目前本网站的任何新功能，包括所推出的新产品，均受到本协议之规范。您了解并同意，本网站服务仅依其当前所呈现的状况提供，对于任何用户通讯或个人化设定之时效、删除、传递错误、未予储存或其他任何问题，吉日支付均不承担任何责任。如您不接受服务调整，请停止使用本服务；否则，您的继续使用行为将被视为其对调整服务的完全同意且承诺遵守。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>吉日支付在提供服务时，&nbsp;可能会对部分服务的用户收取一定的费用或交易佣金。在此情况下，吉日支付会在相关页面上做明确的提示。如您拒绝支付该等费用，则不能使用相关的服务。</p><p style="text-indent: 0px; text-align: start;"><strong>4.</strong>您理解，吉日支付仅提供相应的服务，除此外与相关服务有关的设备（如电脑、移动设备、调制解调器及其他与接入互联网有关的装置）及所需的费用（如电话费、上网费等）均应由您自行负担。</p><p style="text-indent: 0px; text-align: start;"><strong>5.</strong>吉日支付提供的服务可能包括：文字、软件、声音、图片、视频、数据统计、图表、支付通道等。所有这些内容均受著作权、商标和其他财产所有权法律保护。您只有在获得吉日支付或其他相关权利人的授权之后才能使用这些内容，不能擅自复制、再造这些内容、或创造与内容有关的派生产品。</p><h3 style="text-indent: 0px; text-align: start;">四、知识产权</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>本公司在吉日支付软件及相关服务中提供的内容（包括但不限于软件、技术、程序、网页、文字、图片、图像、商标、标识、音频、视频、图表、版面设计、电子文档等，未申明版权或网络上公开的无版权内容除外）的知识产权属于本公司所有。同时本公司提供服务所依托的软件著作权、专利权、商标及其他知识产权均归本公司所有。未经本公司许可，任何人不得擅自使用。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>请您在任何情况下都不要私自使用本公司的包括但不限于“吉日”、“吉日支付”、“agpay”、“agpay.cn”、“agpay.com”、“agooday”和“agooday.com”等在内的任何商标、服务标记、商号、域名、网站名称或其他显著品牌特征等（以下统称为“标识”）。未经本公司事先书面同意，您不得将本条款前述标识以单独或结合任何方式展示、使用或申请注册商标、进行域名注册等，也不得实时向他人明示或暗示有权展示、使用或其他有权处理这些标识的行为。由于您违反本协议使用公司上述商标、标识等给本公司或他人造成损失的，由您承担全部法律责任。</p><h3 style="text-indent: 0px; text-align: start;">五、用户授权及隐私保护</h3><p style="text-indent: 0px; text-align: start;">吉日支付尊重并保护所有吉日支付用户的个人信息及隐私安全。为了给您提供更准确、更有个性化的服务，吉日支付依据《中华人民共和国网络安全法》、《信息安全技术&nbsp;个人信息安全规范》以及其他相关法律法规和技术规范明确了本公司收集/使用/披露您的个人信息的原则。详情请参照<a href="https://uutool.cn/ueditor/#%E9%9A%90%E7%A7%81%E6%94%BF%E7%AD%96" target="">《隐私协议》</a>。</p><h3 style="text-indent: 0px; text-align: start;">六、免责声明</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>吉日支付不担保本网站服务一定能满足您的要求，也不担保本网站服务不会中断，对本网站服务的及时性、安全性、准确性、真实性、完整性也都不作担保。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>对于因不可抗力或吉日支付不能控制的原因造成的本网站服务终端或其他缺陷，本网站不承担任何责任，但本公司将尽力减少因此而给您造成的损失和影响。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>对于您利用吉日支付或本公司发布的其他产品和/或服务，进行违法犯罪，或进行任何违反中国法律、社会公德的行为，本公司有权立即停止对您提供服务，并将由您独自承担由此产生的一切法律责任。</p><h3 style="text-indent: 0px; text-align: start;">七、违约责任</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>针对您违反本协议或其他服务条款的行为，本公司有权独立判断并视情况采取预先警示、限制帐号部分或者全部功能直至永久关闭帐号等措施。本公司有权公告处理结果，且有权根据实际情况决定是否恢复使用。对涉嫌违反法律法规、涉嫌违法犯罪的行为将保存有关记录，并依法向有关主管部门报告、配合有关主管部门调查。</p><h3 style="text-indent: 0px; text-align: start;">八、协议修改</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>吉日支付有权根据法律法规政策、国家有权机构或公司经营要求修改本协议的有关条款，吉日支付会通过适当的方式在网站上予以公示。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>若您不同意吉日支付对本协议相关条款所做的修改，您有权停止使用本网站服务。如果您继续使用本网站服务，则视为您接受吉日支付对本协议相关条款所做的修改。</p>', 'editor', 'MGR', '0', 0, NOW());

INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('agentPrivacyPolicy', '隐私政策', '隐私政策', 'agentTreatyConfig', '隐私政策', '<h2 style="text-indent: 0px; text-align: start;">隐私政策</h2><p style="text-indent: 0px; text-align: start;">吉日展业宝APP（以下简称“我们”）尊重并保护所有吉日商户通用户的个人信息及隐私安全。为了给您提供更准确、更有个性化的服务，我们依据《中华人民共和国网络安全法》、《信息安全技术&nbsp;个人信息安全规范》以及其他相关法律法规和技术规范明确了我们收集/使用/披露您的个人信息的原则。本隐私政策进一步阐述了关于您个人信息的相关权利。</p><p style="text-indent: 0px; text-align: start;">本政策与您所使用的我们的产品与/或服务息息相关，请在使用/继续使用我们的各项产品与服务前，仔细阅读并充分理解本政策。如果您不同意本政策的内容，将可能导致我们的产品/服务无法正常运行，您应立即停止访问/使用我们的产品与/或服务。若您同意本《隐私政策》，即视为您已经仔细阅读/充分理解，并同意本《隐私政策》的全部内容。若您使用/继续使用我们提供的产品与/或服务的行为，均视为您已仔细阅读/充分理解，并同意本隐私政策的全部内容。</p><p style="text-indent: 0px; text-align: start;">本隐私政策属于吉日展业宝APP产品与/或服务使用协议不可分割的一部分。</p><h3 style="text-indent: 0px; text-align: start;">一、我们如何收集和使用您的个人信息</h3><p style="text-indent: 0px; text-align: start;">我们会遵循正当、合法、必要的原则，处于对本政策所述的以下目的，收集和使用您在使用我们的产品与/或服务时所主动提供，或因使用我们的产品与/或服务时被动产生的个人信息。除本政策另有规定外，在未征得您事先许可的情况下，我们不会将这些信息对外披露或提供给第三方。若我们需要将您的个人信息用于本政策未载明的其他用途，或基于特定目的将已经收集的信息用于其他目的，我们将以合理的方式告知您，并在使用前征得您的同意。</p><h4 style="text-indent: 0px; text-align: start;">1.账号注册及登录</h4><p style="text-indent: 0px; text-align: start;">1.1当您注册吉日展业宝APP账号时，您需要根据吉日展业宝APP的要求提供您的个人注册信息，我们会收集您所填写的商户名称、手机号码以及您所选择的商户类型。</p><p style="text-indent: 0px; text-align: start;">1.2为了给您提供更合理的服务，当您登录吉日展业宝APP时，我们会使用您的用户ID/手机号，以确认您账号所属的商户信息。</p><h4 style="text-indent: 0px; text-align: start;">2.向您提供产品与/或服务时</h4><p style="text-indent: 0px; text-align: start;">2.1信息浏览、管理、修改、新增等功能。</p><p style="text-indent: 0px; text-align: start;">当您使用吉日展业宝APP中的信息浏览、管理、修改和新增等功能时，我们会请求您授权照片、相机、和存储功能的权限。如果您拒绝授权提供，将无法使用相应的功能，但并不影响您使用吉日商户通APP的其他功能。</p><p style="text-indent: 0px; text-align: start;">2.1.1当您使用用户头像修改/上传等功能时，我们会请求您授权存储功能的权限，如果您拒绝授权提供，将无法使用相应功能。但并不影响您使用吉日展业宝APP的其他功能。</p><p style="text-indent: 0px; text-align: start;">2.1.2当您使用吉日展业宝APP中的编辑个人信息、门店管理、码牌管理、云喇叭管理、云打印机管理等功能时，您所提供的图片、文字、状态等信息将会上传并存储至云端服务器中，由于存储是实现以上功能及其多端同步的必要条件。我们会以加密的方式存储这些信息，您也可以随时修改这些信息。</p><p style="text-indent: 0px; text-align: start;">2.2安全运行。为了保障软件与服务的安全运行，我们会收集您的设备型号、设备名称、设备唯一标识符（包括：IMEI、IMSI、Android&nbsp;ID、IDFA）、浏览器类型和设置、使用的语言、操作系统和应用程序版本、网络设备硬件地址、访问日期和时间、登录IP地址、接入网络的方式等。</p><p style="text-indent: 0px; text-align: start;">2.3搜索功能。当您使用吉日展业宝APP提供的搜索服务时，我们会收集您所输入的关键词信息、访问时间信息。这些信息是搜索功能的必要条件。</p><p style="text-indent: 0px; text-align: start;">2.4扫码。当您使用吉日展业宝APP提供的扫一扫支付、绑定新码、扫码获取云喇叭/打印机设备号等功能和/或服务时，我们会请求您授权相机的权限。如果您拒绝授权提供，将无法使用上述功能。</p><p style="text-indent: 0px; text-align: start;">2.5收款。当您使用吉日展业宝APP提供的收款功能时、我们会收集该笔收款订单的订单号、收款金额、收款时间、支付时间、支付方式、订单状态、门店信息、用户信息，这些信息用于生成详细的订单记录。</p><p style="text-indent: 0px; text-align: start;">2.6退款。当您使用吉日展业宝APP提供的订单退款功能时、我们会收集该笔订单的订单号、订单金额、支付金额、退款金额、支付时间、退款时间、门店信息、用户信息，这些信息用于生成详细的退款记录，并对比收退款金额，以限制退款金额不能大于支付金额。</p><p style="text-indent: 0px; text-align: start;">2.7查询。当您使用吉日展业宝APP提供的订单记录、门店列表、码牌列表、云喇叭列表、云打印机列表、收款通知接收人列表等功能时，我们会收集您的账户信息和商户ID，用于展示在您查询权限内的信息。</p><p style="text-indent: 0px; text-align: start;">2.8拨号。当您使用吉日展业宝APP，关于我们-联系电话中的快捷拨号功能时，我们会请求您设备的拨号权限。如果您拒绝授权提供，将无法使用快捷拨号功能，但不会影响您使用吉日展业宝APP的其他服务。</p><p style="text-indent: 0px; text-align: start;">2.9设备权限调用汇总</p><p style="text-indent: 0px; text-align: start;">我们对吉日展业宝APP为您提供服务时，所需要您授权的设备权限汇总如下。注意：您可以拒绝其中部分权限，但将无法使用需要该权限的功能和服务。您可以随时取消已授权的设备权限，不同设备权限显示方式和关闭方式可能有所不同，具体请参考设备及操作系统开发方的说明和操作指引：</p><table style="text-align: start;"><tbody><tr><td colspan="1" rowspan="1"><strong>设备权限</strong></td><td colspan="1" rowspan="1"><strong>对应业务功能</strong></td><td colspan="1" rowspan="1"><strong>功能场景说明</strong></td><td colspan="1" rowspan="1"><strong>取消授权</strong></td></tr><tr><td colspan="1" rowspan="1">相机</td><td colspan="1" rowspan="1">扫一扫收款与喇叭/云打印机新增、修改绑定新码</td><td colspan="1" rowspan="1">1.扫描消费者付款码，使用相机识别二维码。&nbsp;2.扫描未绑定的码牌时，使用相机识别二维码。3.获取云打印机/云喇叭设备号时，使用相机识别设备条码</td><td colspan="1" rowspan="1">​该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">访问照片</td><td colspan="1" rowspan="1">头像上传通道申请上传照片</td><td colspan="1" rowspan="1">1.用户头像上传时我们需要访问您的相册，以便您选取要上传的照片2.通道申请时上传企业营业执照、法人身份证照片、商户门头照等信息时，我们需要访问您的相册，以便您选取要上传的照片</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">储存</td><td colspan="1" rowspan="1">APP稳定运行下载码牌图片</td><td colspan="1" rowspan="1">1.日志信息记录、信息缓存2.下载码牌图片至手机相册中</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr><tr><td colspan="1" rowspan="1">获取电话状态（设备信息）</td><td colspan="1" rowspan="1">APP安全运行</td><td colspan="1" rowspan="1">1.本政策第2.2条描述</td><td colspan="1" rowspan="1">该权限允许拒绝或取消授权，不影响APP其他功能</td></tr></tbody></table><h4 style="text-indent: 0px; text-align: start;">3.征得授权同意的例外</h4><p style="text-indent: 0px; text-align: start;">根据相关法律法规的规定，在以下情形中，我们可以在不征得您的授权同意的情况下收集、使用一些必要的个人信息：</p><p style="text-indent: 0px; text-align: start;">（1）与我们旅行法律法规规定的义务相关的；</p><p style="text-indent: 0px; text-align: start;">（2）与国家安全、国防安全直接相关的；</p><p style="text-indent: 0px; text-align: start;">（3）与公共安全、公共卫生、重大公共利益直接相关的；</p><p style="text-indent: 0px; text-align: start;">（4）与犯罪侦查、起诉、审判和判决执行及相关事项直接相关的；</p><p style="text-indent: 0px; text-align: start;">（5）出于维护您或其他个人的生命、财产及相关重大合法权益但有很难得到本人同意的；</p><p style="text-indent: 0px; text-align: start;">（6）所收集的个人信息是您自行向社会公众公开的；</p><p style="text-indent: 0px; text-align: start;">（7）从合法公开披露的信息中收集到您的客人信息：从合法的新闻报道、政府信息公开等相关渠道；</p><p style="text-indent: 0px; text-align: start;">（8）根据您与平台签署的在线协议或合同所必需的；</p><p style="text-indent: 0px; text-align: start;">（9）用于维护我们产品和/或服务的安全稳定运行所必需的：发现、处置产品或服务的故障及相关问题处理；</p><p style="text-indent: 0px; text-align: start;">（10）法律法规规定的其他情形。</p><h3 style="text-indent: 0px; text-align: start;">二、我们如何共享、转让、公开披露您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.共享</h4><p style="text-indent: 0px; text-align: start;">对于您的个人信息，我们不会与任何公司、组织和个人进行共享，除非存在以下一种或多种情形：</p><p style="text-indent: 0px; text-align: start;">（1）事先已获得您的授权；</p><p style="text-indent: 0px; text-align: start;">（2）您自行提出的；</p><p style="text-indent: 0px; text-align: start;">（3）与商业合作伙伴的必要共享：</p><p style="text-indent: 0px; text-align: start;">您理解并知悉，为了向您提供更完善、优质的产品和服务；或由于您在使用吉日展业宝中由第三方服务提供企业/机构所提供的服务时的情况下，我们将授权第三方服务提供企业/机构为您提供部分服务。此种情况下，我们可能会与合作伙伴共享您的某些个人信息，其中包括您已授权或自行提出的（包括但不限于商户名称、手机号、法人信息、商户营业执照等）必要信息，以及您在使用本APP时自动产生的某些信息（包括订单、订单金额、交易时间、收款方式、收款金额、门店信息、退款信息）。请您注意、我们仅处于合法、正当、必要、特定、明确的目的共享这些信息。我们将对信息数据的输出形式、流转、使用进行安全评估与处理，以保护数据安全。同时，我们会对合作伙伴、服务商机构进行严格的监督与管理，一但发现其存在违规处理个人信息的行为，将立即停止合作并追究其法律责任。</p><p style="text-indent: 0px; text-align: start;">目前，我们的合作伙伴包括以下类型：</p><p style="text-indent: 0px; text-align: start;">A.第三方支付机构：当您使用吉日展业宝提供的支付业务时，将会使用并通过第三方支付机构的支付通道，其中包括但不限于微信支付、支付宝支付、银联云闪付支付等第三方支付平台。我们会与第三方支付机构共享来自于您的部分交易信息。为保障您在使用我们所提供的收款功能/服务时的合理、合规及合法性，在您正式使用前述功能/服务前，您需要向对应的第三方支付机构发起支付通道申请，在此情况下，我们会收集您所主动提供的商户名称、企业名称、法人信息、营业执照、账户信息等必要信息，并将上述信息与第三方支付机构共享。</p><p style="text-indent: 0px; text-align: start;">（4）您可以基于吉日展业宝APP与第三人（包括不特定对象）共享您的个人信息或其他信息，但因您的共享行为而导致的信息泄露、被使用及其他相关请何况，与吉日展业宝APP无关，吉日展业宝不因此承担法律责任。</p><h4 style="text-indent: 0px; text-align: start;">2.转让</h4><p style="text-indent: 0px; text-align: start;">转让是指将取得您个人信息的控制权转让给其他公司、组织或个人。除非获取您的明确同意，否则我们不会将您的个人信息转让给任何公司、组织或个人。但下述情形除外：</p><p style="text-indent: 0px; text-align: start;">（1）已事先征得您的同意；</p><p style="text-indent: 0px; text-align: start;">（2）您自行提出的；</p><p style="text-indent: 0px; text-align: start;">（3）如果公司发生合并、收购或破产清算，将可能涉及到个人信息转让，此种情况下我们会告知您有关情况并要求新的持有您个人信息的公司、组织继续受本政策的约束。否则我们将要求其重新获取您的明示同意。</p><p style="text-indent: 0px; text-align: start;">（4）其他法律法规规定的情形。</p><h4 style="text-indent: 0px; text-align: start;">3.公开披露</h4><p style="text-indent: 0px; text-align: start;">公开披露是指向社会或不特定人群发布信息的行为。原则上，我们不会对您的个人信息进行公开披露。但下述情况除外：</p><p style="text-indent: 0px; text-align: start;">（1）取得您的明示同意后。</p><h4 style="text-indent: 0px; text-align: start;">4.共享、转让、公开披露个人信息授权同意的例外情形</h4><p style="text-indent: 0px; text-align: start;">根据相关法律法规的固定，在以下情形中，我们可能在未事先征得您的授权同意的情况下共享、转让、公开披露您的个人信息：</p><p style="text-indent: 0px; text-align: start;">（1）与我们履行法律法规规定的义务相关的，含依照法律规定、司法机关或行政机关强制要求向有权机关披露您的个人信息；在该种情况下，我们会要求披露请求方出局与其请求相应的有效法律文件，并对被披露的信息采取符合法律和业界标准的安全防护措施；</p><p style="text-indent: 0px; text-align: start;">（2）与国家安全、国防安全直接相关的；</p><p style="text-indent: 0px; text-align: start;">（3）与公共安全、公共卫生、重大公共利益直接相关的；</p><p style="text-indent: 0px; text-align: start;">（4）与犯罪侦擦、&nbsp;起诉、审判和判决执行及相关事项直接相关的；</p><p style="text-indent: 0px; text-align: start;">（5）出于维护您或其他个人的生命、财产及相关重大合法权益但又很难得到本人同意的；</p><p style="text-indent: 0px; text-align: start;">（6）您自行向社会公众公开的个人信息；</p><p style="text-indent: 0px; text-align: start;">（7）从合法公开披露的信息中收集到的个人信息：合法的新闻报道、政府信息公开及其他相关渠道；</p><p style="text-indent: 0px; text-align: start;">（8）法律法规规定的其他情形。</p><p style="text-indent: 0px; text-align: start;">请您了解，根据现行法律规定及监管要求，共享、转让经去标识化处理的个人信息，且确保数据接收方无法复原并重新识别个人信息主体的，无需另行向您通知并征得您的同意。</p><h3 style="text-indent: 0px; text-align: start;">三、我们如何存储和保护您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.存储</h4><p style="text-indent: 0px; text-align: start;">存储地点：我们将从中华人民共和国境内获得的个人信息存放于中华人民共和国境内。如果发生个人信息的跨境传输，我们会单独向您以邮件或通知的方式告知您个人信息处境的目的与接受方，并征得您的同意。我们会严格遵守中华人民共和国法律法规，确保数据接收方有充足的数据能力来保护您的个人信息安全。</p><p style="text-indent: 0px; text-align: start;">存储时间：我们承诺始终按照法律的规定在合理必要期限内存储您的个人信息。超出上述期限后，我们将删除您的个人信息或对您的个人信息进行脱敏、去标识化、匿名化处理。</p><p style="text-indent: 0px; text-align: start;">如果我们停止运营，我们将立即停止收集您的个人信息，并将停止运营这一决定和/或事实以右键或通知的方式告知您，并对所有已获取的您的个人信息进行删除或匿名化处理。</p><h4 style="text-indent: 0px; text-align: start;">2.保护</h4><p style="text-indent: 0px; text-align: start;">为了您的个人信息安全，我们将采用各种符合行业标准的安全措施来保护您的个人信息安全，以最大程度降低您的个人信息被损毁、泄露、盗用、非授权方式访问、使用、披露和更改的风险。我们将积极建立数据分类分级制度、数据安全管理规范、数据安全开发规范来管理规范个人信息的采集、存储和使用，确保未收集与我们提供的产品和服务无关的信息。</p><p style="text-indent: 0px; text-align: start;">在数据存储安全上，我们与第三方机构合作，包括但不限于阿里云、腾讯云等。</p><p style="text-indent: 0px; text-align: start;">为保障您的账户和个人信息的安全，请妥善保管您的账户及密码信息。我们将通过多端服务器备份的方式保障您的信息不丢失、损毁或因不可抗力因素而导致的无法使用。通过第三方存储服务机构提供的堡垒机、安全防火墙等服务，保障您的信息不被滥用、变造和泄露。</p><p style="text-indent: 0px; text-align: start;">尽管有上述安全措施，但也请您注意：在信息网络上并不存在绝对“完善的安全措施”。为防止安全事故的发生，我们已按照法律法规规定，制定了妥善的预警机制和应急预案。如确发生安全事件，我们将及时将相关情况以邮件、电话、信函、短信等方式告知您、难以逐一告知单一个体时，我们会采取合理、有效的方式发布公告。同时，我们还将按照监管部门要求，主动上报个人信息安全事件的处置情况、紧密配合政府机关的工作。</p><p style="text-indent: 0px; text-align: start;">当我们的产品或服务发生停止运营的情况下，我们将立即停止收集您的个人信息，并将停止运营这一决定和/或事实以邮件或通知的方式告知您，并对所有已获取的您的个人信息进行删除或匿名化处理。</p><h4 style="text-indent: 0px; text-align: start;">3.匿名化处理</h4><p style="text-indent: 0px; text-align: start;">为保障我们已收集到的您的个人信息的安全，当我们不再提供服务、停止运营或因其他不可抗力因素而不得不销毁您的个人信息的情况下。我们将会采取删除或匿名化的方式处理您的个人信息。</p><p style="text-indent: 0px; text-align: start;">匿名化是指通过对个人信息的技术处理，使个人信息的主体无法被识别，且处理后无法被复原的过程。严格意义上，匿名化后的信息不属于个人信息。</p><h3 style="text-indent: 0px; text-align: start;">四、您如何管理您的个人信息</h3><h4 style="text-indent: 0px; text-align: start;">1.自主决定授权信息</h4><p style="text-indent: 0px; text-align: start;">您可以自主决定是否授权我们向您申请的某些设备权限，具体请参考第一条，2.9所述。</p><p style="text-indent: 0px; text-align: start;">注意：根据不同的操作系统及硬件设备，您管理这些权限的方式可能会有所不同，具体操作方式，请参照您的设备或操作系统开发方的说明和操作指引。</p><h4 style="text-indent: 0px; text-align: start;">2.访问、获取、更改和删除相关信息</h4><p style="text-indent: 0px; text-align: start;">您可以通过交互本APP的交互界面对相关信息进行访问、获取、更改和删除。</p><p style="text-indent: 0px; text-align: start;">（1）您登录账户的名称和头像：</p><p style="text-indent: 0px; text-align: start;">您可以通过在“我的”页面，通过点击头像一栏的按钮进入修改个人信息页面，对个人名称进行查看和修改。通过点击个人头像，查看您的账户头像，授权我们访问您的相册后，您可以更改您的账户头像。</p><p style="text-indent: 0px; text-align: start;">（2）门店信息：</p><p style="text-indent: 0px; text-align: start;">您可以在“首页-门店管理”页面，通过点击门店列表中的单个门店进入所选门店的编辑页面，对门店名称和备注信息进行查看和修改。通过点击状态条目中的开关按钮，您可以启用或禁用所选门店的状态。</p><p style="text-indent: 0px; text-align: start;">（3）云喇叭/云打印机设备信息：</p><p style="text-indent: 0px; text-align: start;">您可以在“我的-云喇叭管理/云打印管理”页面，通过点击设备列表中的单个云喇叭/云打印机进入所选设备的编辑页面，对设备名称、设备编号、所属门店等信息进行查看和修改。通过点击状态条目中的开关按钮，您可以启用或禁用所选设备的状态。</p><h3 style="text-indent: 0px; text-align: start;">五、您如何注销您的账号</h3><p style="text-indent: 0px; text-align: start;">您可以通过第九条中指明的联系方式联系我们，并像我们阐明您注销账号的原因。或在本APP的”我的-设置-其他设置-注销账号“页面输入注销原因并点击提交按钮向我们提交您的注销申请。在满足账号注销的条件下，我们将尽快注销您的账号。注意：由于您账号在使用期间内产生的交易信息将不会被立刻处理，而是需要经过确认、复查后，确保该笔交易已完成所有流程后，进行脱敏处理。此外，除法律明确规定必须由我们保留的个人信息外，您在使用本APP期间内所产生或由您提交的其他个人信息将会被删除或匿名化处理，且该处理不可逆，您将无法找回这些个人信息。</p><h3 style="text-indent: 0px; text-align: start;">六、有关第三方提供产品和/或服务的特别说明</h3><p style="text-indent: 0px; text-align: start;">您在使用吉日展业宝APP时，可能会使用到由第三方提供的产品和/或服务，在这种情况下，您需要接受该第三方的服务条款及隐私政策（而非本隐私政策）的约束，您需要仔细阅读其条款并自行决定是否接受。请您妥善保管您的个人信息，仅在必要的情况下向他人提供。本政策仅适用于我们所收集、保存、使用、共享、披露信息，并不适用于任何第三方提供服务时（包含您向该第三方提供的任何个人信息）或第三方信息的使用规则，第三方使用您的个人信息时的行为，由其自行负责。</p><h3 style="text-indent: 0px; text-align: start;">七、我们如何使用Cookie和其他同类技术</h3><p style="text-indent: 0px; text-align: start;">在您未拒绝接受cookies的情况下，我们会在您的计算机以及相关移动设备上设定或取用cookies，以便您能登录或使用依赖于cookies的吉日展业宝的产品与/或服务。您有权选择接受或拒绝接受cookies。您可以通过修改浏览器设置的方式或在移动设备设置中设置拒绝我们使用cookies。若您拒绝使用cookies，则您可能无法登录或使用依赖于cookies的吉日展业宝App网络服务或功能。</p><h3 style="text-indent: 0px; text-align: start;">八、更新隐私政策</h3><p style="text-indent: 0px; text-align: start;">我们保留更新或修订本隐私政策的权力。这些修订或更新构成本政策的一部分，并具有等同于本政策的效。未经您的同意，我们不会削减您依据当前生效的本政策所应享受的权利。</p><p style="text-indent: 0px; text-align: start;">我们会不时更新本政策，如遇本政策更新，我们会通过APP通知等相关合理方式通知您，如遇重大更新，您需要重新仔细阅读、充分理解并同意修订更新后的政策，才可继续使用我们所提供的产品和/或服务。</p><h3 style="text-indent: 0px; text-align: start;">九、联系我们</h3><p style="text-indent: 0px; text-align: start;">如果您对本政策有任何疑问，您可以通过以下方式联系我们，我们将尽快审核所涉问题，并在验证您的用户身份后予以答复。</p><p style="text-indent: 0px; text-align: start;">网站：<a href="https://www.agooday.com" target="_blank">www.agooday.com</a></p><h3 style="text-indent: 0px; text-align: start;">十、其他</h3><p style="text-indent: 0px; text-align: start;">如果您认为我们的个人信息处理行为损害了您的合法权益，您可以向有关政府部门进行反应。或因本政策以及我们处理您个人信息事宜引起的任何争议，您可以诉至沧州市人民法院。</p>', 'editor', 'MGR', '0', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('agentServiceAgreement', '用户服务协议', '用户服务协议', 'agentTreatyConfig', '用户服务协议', '<h2 style="text-indent: 0px; text-align: start;">用户服务协议</h2><p style="text-indent: 0px; text-align: start;">感谢您使用吉日支付，在使用“吉日展业宝”软件及相关服务前，请您认真阅读本协议，并确认承诺同意遵守本协议的全部约定。本协议由您与吉日科技（武汉）有限公司（包括其关联机构，以下合成“本公司”）于您点击同意本协议之时，在湖北省沧州市签署并生效。</p><h3 style="text-indent: 0px; text-align: start;">一、协议条款的确认及接受</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>吉日支付（包括网址为<a href="https://www.agooday.com" target="_blank">www.agpay.com&nbsp;</a>的网站，以及可在IOS系统及Android&nbsp;系统中运行的名为“吉日商户通APP”、“吉日展业宝APP”、及其他不同版本的应用程序，以及名为“吉日商户通”、“吉日展业宝”的微信小程序，以下简称"本网站"或“吉日支付”）由吉日科技（武汉）有限公司（包括其关联机构，以下合称“本公司”）运营并享有完全的所有权及知识产权等权益，吉日支付提供的服务将完全按照其发布的条款和操作规则严格执行。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>您确认同意本协议（协议文本包括《吉日支付用户服务协议》、《吉日支付用户隐私政策》及吉日支付已公示或将来公示的各项规则及提示，所有前述协议、规则及提示乃不可分割的整体，具有同等法律效力，共同构成用户使用吉日支付及相关服务的整体协议，以下合称“本协议”）所有条款并完成注册程序时，本协议在您于本公司间成立并发生法律效力，同时您成为吉日支付正式用户。</p><h3 style="text-indent: 0px; text-align: start;">二、账号注册及使用规则</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>当您使用“吉日展业宝”APP时，您可以在APP/微信小程序中的注册页，或在地址为<a href="https://mch.s.agpay.vip/register" target="_blank">https://mch.s.agpay.vip/register</a>的网页进行注册，注册成功后，吉日支付将给与您一个商户账号及相应的密码，该商户账号和密码有您负责保管，您应当对以其商户账号进行的所有活动和事件负法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>您须对在吉日支付注册信息的真实性、合法性、有效性承担全部责任，您不得冒充他人（包括但不限于冒用他人姓名、名称、字号、头像等足以让人引起混淆的方式，以及冒用他人手机号码）开设账号；不得利用他人的名义发布任何信息；不得利用他人的名义发起任何交易；不得恶意使用注册账户导致他人误认；否则吉日支付有权立即停止提供服务，收回账号，并由您独自承担由此而产生的一切法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>您理解且确认，您在吉日支付注册的账号的所有权及有关权益均归本公司所有，您完成注册手续后仅享有该账号的使用权（包括但不限于该账号绑定的由吉日支付提供的产品和/或服务）。您的账号仅限于您本人使用，未经本公司书面同意，禁止以任何形式赠与、借用、出租、转让、售卖或以其他任何形式许可他人使用该账号。如果本公司发现或有合理理由认为使用者并非账号初始注册人，公司有权在未通知您的请款修改，暂停或终止向该账号提供服务，并有权注销该账号，而无需向注册该账号的用户承担法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>4.</strong>您理解并确认，除用于登录、使用“吉日展业宝”APP及相关服务外，您还可以使用您的注册账号登录使用吉日支付提供的其他商户产品和/或服务，以及其他本公司的合作伙伴、第三方服务提供商所提供的软件及服务。若您以吉日支付账号登录和/或使用前述服务时，同样应受到其他软件、服务实际提供方的用户协议及其他协议条款的约束。</p><p style="text-indent: 0px; text-align: start;"><strong>5.</strong>您理解并确认，部分由其他第三方平台（包括但不限于银联云闪付、微信支付、支付宝支付、随行付等）提供的产品及服务，在您使用吉日支付提供的产品和/或服务时，仅作为基础服务为您提供。您的吉日支付账号与您在上述第三方平台注册的第三方平台账号仅在技术层面上构成单方面绑定。如果您不使用/不继续使用吉日支付提供的产品和/或服务，您的第三方平台账号均不会受到影响，您可以继续第三方平台提供的产品及服务。</p><p style="text-indent: 0px; text-align: start;"><strong>6.</strong>您承诺不得以任何方式利用吉日支付直接或间接从事违反中国法律、社会公德的行为，吉日支付有权对违反上述承诺的内容予以屏蔽、留证，并将由您独自承担由此而产生的一切法律责任。</p><p style="text-indent: 0px; text-align: start;"><strong>7.</strong>您不得利用本网站制作、上载、复制、发布、传播或转载如下内容：</p><p style="text-indent: 0px; text-align: start;">（1）反对宪法所确定的基本原则的；</p><p style="text-indent: 0px; text-align: start;">（2）危害国家安全，泄露国家秘密，颠覆国家政权，破坏国家统一的；</p><p style="text-indent: 0px; text-align: start;">（3）损害国家荣誉和利益的；</p><p style="text-indent: 0px; text-align: start;">（4）煽动民族仇恨、民族歧视，破坏民族团结的；</p><p style="text-indent: 0px; text-align: start;">（5）破坏国家宗教政策，宣扬邪教和封建迷信的；</p><p style="text-indent: 0px; text-align: start;">（6）散布谣言，扰乱社会秩序，破坏社会稳定的；</p><p style="text-indent: 0px; text-align: start;">（7）散布淫秽、色情、赌博、暴力、凶杀、恐怖或者教唆犯罪的；</p><p style="text-indent: 0px; text-align: start;">（8）侮辱或者诽谤他人，侵害他人合法权益的；</p><p style="text-indent: 0px; text-align: start;">（9）侵害未成年人合法权益或者损害未成年人身心健康的；</p><p style="text-indent: 0px; text-align: start;">（10）含有法律、行政法规禁止的其他内容的信息。</p><p style="text-indent: 0px; text-align: start;"><strong>8.</strong>吉日支付有权对您使用我们的产品和/或服务时的情况进行审查和监督，如您在使用吉日支付时违反任何上述规定，本公司有权暂停或终止对您提供服务，以减轻您的不当行为所造成的影响。</p><h3 style="text-indent: 0px; text-align: start;">三、服务内容</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>本公司可能为不同的终端设备及使用需求开发不同的应用程序软件版本，您应当更具实际设备需求状况获取、下载、安装合适的版本。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>本网站的服务具体内容根据实际情况提供，吉日支付保留变更、终端或终止部分或全部服务的权力。吉日支付不承担因业务调整给您造成的损失。除非本协议另有其他明示规定，增加或强化目前本网站的任何新功能，包括所推出的新产品，均受到本协议之规范。您了解并同意，本网站服务仅依其当前所呈现的状况提供，对于任何用户通讯或个人化设定之时效、删除、传递错误、未予储存或其他任何问题，吉日支付均不承担任何责任。如您不接受服务调整，请停止使用本服务；否则，您的继续使用行为将被视为其对调整服务的完全同意且承诺遵守。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>吉日支付在提供服务时，&nbsp;可能会对部分服务的用户收取一定的费用或交易佣金。在此情况下，吉日支付会在相关页面上做明确的提示。如您拒绝支付该等费用，则不能使用相关的服务。</p><p style="text-indent: 0px; text-align: start;"><strong>4.</strong>您理解，吉日支付仅提供相应的服务，除此外与相关服务有关的设备（如电脑、移动设备、调制解调器及其他与接入互联网有关的装置）及所需的费用（如电话费、上网费等）均应由您自行负担。</p><p style="text-indent: 0px; text-align: start;"><strong>5.</strong>吉日支付提供的服务可能包括：文字、软件、声音、图片、视频、数据统计、图表、支付通道等。所有这些内容均受著作权、商标和其他财产所有权法律保护。您只有在获得吉日支付或其他相关权利人的授权之后才能使用这些内容，不能擅自复制、再造这些内容、或创造与内容有关的派生产品。</p><h3 style="text-indent: 0px; text-align: start;">四、知识产权</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>本公司在吉日支付软件及相关服务中提供的内容（包括但不限于软件、技术、程序、网页、文字、图片、图像、商标、标识、音频、视频、图表、版面设计、电子文档等，未申明版权或网络上公开的无版权内容除外）的知识产权属于本公司所有。同时本公司提供服务所依托的软件著作权、专利权、商标及其他知识产权均归本公司所有。未经本公司许可，任何人不得擅自使用。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>请您在任何情况下都不要私自使用本公司的包括但不限于“吉日”、“吉日支付”、“agpay”、“agpay.cn”、“agpay.com”、“agooday”和“agooday.com”等在内的任何商标、服务标记、商号、域名、网站名称或其他显著品牌特征等（以下统称为“标识”）。未经本公司事先书面同意，您不得将本条款前述标识以单独或结合任何方式展示、使用或申请注册商标、进行域名注册等，也不得实时向他人明示或暗示有权展示、使用或其他有权处理这些标识的行为。由于您违反本协议使用公司上述商标、标识等给本公司或他人造成损失的，由您承担全部法律责任。</p><h3 style="text-indent: 0px; text-align: start;">五、用户授权及隐私保护</h3><p style="text-indent: 0px; text-align: start;">吉日支付尊重并保护所有吉日支付用户的个人信息及隐私安全。为了给您提供更准确、更有个性化的服务，吉日支付依据《中华人民共和国网络安全法》、《信息安全技术&nbsp;个人信息安全规范》以及其他相关法律法规和技术规范明确了本公司收集/使用/披露您的个人信息的原则。详情请参照<a href="https://uutool.cn/ueditor/#%E9%9A%90%E7%A7%81%E6%94%BF%E7%AD%96" target="">《隐私协议》</a>。</p><h3 style="text-indent: 0px; text-align: start;">六、免责声明</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>吉日支付不担保本网站服务一定能满足您的要求，也不担保本网站服务不会中断，对本网站服务的及时性、安全性、准确性、真实性、完整性也都不作担保。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>对于因不可抗力或吉日支付不能控制的原因造成的本网站服务终端或其他缺陷，本网站不承担任何责任，但本公司将尽力减少因此而给您造成的损失和影响。</p><p style="text-indent: 0px; text-align: start;"><strong>3.</strong>对于您利用吉日支付或本公司发布的其他产品和/或服务，进行违法犯罪，或进行任何违反中国法律、社会公德的行为，本公司有权立即停止对您提供服务，并将由您独自承担由此产生的一切法律责任。</p><h3 style="text-indent: 0px; text-align: start;">七、违约责任</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>针对您违反本协议或其他服务条款的行为，本公司有权独立判断并视情况采取预先警示、限制帐号部分或者全部功能直至永久关闭帐号等措施。本公司有权公告处理结果，且有权根据实际情况决定是否恢复使用。对涉嫌违反法律法规、涉嫌违法犯罪的行为将保存有关记录，并依法向有关主管部门报告、配合有关主管部门调查。</p><h3 style="text-indent: 0px; text-align: start;">八、协议修改</h3><p style="text-indent: 0px; text-align: start;"><strong>1.</strong>吉日支付有权根据法律法规政策、国家有权机构或公司经营要求修改本协议的有关条款，吉日支付会通过适当的方式在网站上予以公示。</p><p style="text-indent: 0px; text-align: start;"><strong>2.</strong>若您不同意吉日支付对本协议相关条款所做的修改，您有权停止使用本网站服务。如果您继续使用本网站服务，则视为您接受吉日支付对本协议相关条款所做的修改。</p>', 'editor', 'MGR', '0', 0, NOW());

-- 初始化商户系统系统参数
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('appVoice', '是否启用app订单语音播报', '是否启用app订单语音播报', 'orderConfig', '系统配置', '1', 'radio', 'MCH', 'M0000000000', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('qrcEscaping', '是否启用码牌防逃单功能', '是否启用码牌防逃单功能', 'orderConfig', '系统配置', '1', 'radio', 'MCH', 'M0000000000', 0, NOW());

INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mchPayNotifyUrl', 'POS支付回调地址', 'POS支付回调地址', 'payOrderNotifyConfig', '回调和查单参数', '', 'text', 'MCH', 'M0000000000', 10, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mchRefundNotifyUrl', 'POS退款回调地址', 'POS退款回调地址', 'payOrderNotifyConfig', '回调和查单参数', '', 'text', 'MCH', 'M0000000000', 20, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mchNotifyPostType', '商户接收通知方式', '商户接收通知方式', 'payOrderNotifyConfig', '回调和查单参数', 'POST_JSON', 'radio', 'MCH', 'M0000000000', 30, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('payOrderNotifyExtParams', '支付订单回调和查单参数', '支付订单回调和查单参数', 'payOrderNotifyConfig', '回调和查单参数', '[]', 'text', 'MCH', 'M0000000000', 40, NOW());

INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('divisionConfig', '分账管理', '分账管理', 'divisionManage', '分账管理', '{"overrideAutoFlag":0,"autoDivisionRules":{"amountLimit":0,"delayTime":120},"mchDivisionEntFlag":1}', 'text', 'MCH', 'M0000000000', 0, NOW());

INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mchApiEntList', '商户接口权限集合', '商户接口权限集合', 'mchApiEnt', '商户接口权限集合', '[]', 'text', 'MCH', 'M0000000000', 0, NOW());

-- 会员配置
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mbrMaxBalance', '会员最大储值余额(元), 0表示依据系统配置', '会员最大储值余额(元), 0表示依据系统配置', 'mbrConfig', '会员配置', '1', 'text', 'MCH', 'M0000000000', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mbrCustomAmountState', '充值自定义金额', '充值自定义金额', 'mbrConfig', '会员配置', '1', 'radio', 'MCH', 'M0000000000', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mbrModelState', '会员模块状态开关', '会员模块状态开关', 'mbrConfig', '会员配置', '1', 'radio', 'MCH', 'M0000000000', 0, NOW());
INSERT INTO `t_sys_config` (`config_key`, `config_name`, `config_desc`, `group_key`, `group_name`, `config_val`, `type`, `sys_type`, `belong_info_id`, `sort_num`, `updated_at`) VALUES ('mbrPayState', '会员支付开关', '会员支付开关', 'mbrConfig', '会员配置', '1', 'radio', 'MCH', 'M0000000000', 0, NOW());

-- 初始化支付方式
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('ALI_BAR', '支付宝条码', 'ALIPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('ALI_JSAPI', '支付宝生活号', 'ALIPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('ALI_APP', '支付宝APP', 'ALIPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('ALI_WAP', '支付宝WAP', 'ALIPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('ALI_PC', '支付宝PC网站', 'ALIPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('ALI_QR', '支付宝二维码', 'ALIPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('ALI_LITE', '支付宝小程序', 'ALIPAY');

INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('WX_BAR', '微信条码', 'WECHAT');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('WX_JSAPI', '微信公众号', 'WECHAT');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('WX_APP', '微信APP', 'WECHAT');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('WX_H5', '微信H5', 'WECHAT');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('WX_NATIVE', '微信扫码', 'WECHAT');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('WX_LITE', '微信小程序', 'WECHAT');

INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('UP_APP', '银联App支付', 'UNIONPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('UP_B2B', '银联企业网银支付', 'UNIONPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('UP_BAR', '银联二维码(被扫)', 'UNIONPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('UP_JSAPI', '银联Js支付', 'UNIONPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('UP_PC', '银联网关支付', 'UNIONPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('UP_QR', '银联二维码(主扫)', 'UNIONPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('UP_WAP', '银联手机网站支付', 'UNIONPAY');

INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('YSF_BAR', '云闪付条码', 'YSFPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('YSF_JSAPI', '云闪付jsapi', 'YSFPAY');

INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('DCEP_BAR', '数字人民币条码', 'DCEPPAY');
INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('DCEP_QR', '数字人民币二维码', 'DCEPPAY');

INSERT INTO `t_pay_way` (`way_code`, `way_name`, `way_type`) VALUES ('PP_PC', 'PayPal支付', 'OTHER');

-- 初始化支付接口定义
INSERT INTO `t_pay_interface_define` (`if_code`, `if_name`, `is_mch_mode`, `is_isv_mode`, `config_page_type`, `isv_params`, `isvsub_mch_params`, `normal_mch_params`, `way_codes`, `icon`, `bg_color`, `state`, `remark`)
VALUES ('alipay', '支付宝官方', 1, 1, 2,
        '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"","values":"1,0","titles":"沙箱环境,生产环境","verify":"required"},{"name":"pid","desc":"合作伙伴身份（PID）","type":"text","verify":"required"},{"name":"appId","desc":"应用App ID","type":"text","verify":"required"},{"name":"privateKey", "desc":"应用私钥", "type": "textarea","verify":"required","star":"1"},{"name":"alipayPublicKey", "desc":"支付宝公钥(不使用证书时必填)", "type": "textarea","star":"1"},{"name":"signType","desc":"接口签名方式(推荐使用RSA2)","type":"radio","verify":"","values":"RSA,RSA2","titles":"RSA,RSA2","verify":"required"},{"name":"useCert","desc":"公钥证书","type":"radio","verify":"","values":"1,0","titles":"使用证书（请使用RSA2私钥）,不使用证书"},{"name":"appPublicCert","desc":"应用公钥证书（.crt格式）","type":"file","verify":""},{"name":"alipayPublicCert","desc":"支付宝公钥证书（.crt格式）","type":"file","verify":""},{"name":"alipayRootCert","desc":"支付宝根证书（.crt格式）","type":"file","verify":""}]',
        '[{"name":"appAuthToken", "desc":"子商户app_auth_token", "type": "text","readonly":"readonly"},{"name":"refreshToken", "desc":"子商户刷新token", "type": "hidden","readonly":"readonly"},{"name":"expireTimestamp", "desc":"authToken有效期（13位时间戳）", "type": "hidden","readonly":"readonly"}]',
        '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"","values":"1,0","titles":"沙箱环境,生产环境","verify":"required"},{"name":"appId","desc":"应用App ID","type":"text","verify":"required"},{"name":"privateKey", "desc":"应用私钥", "type": "textarea","verify":"required","star":"1"},{"name":"alipayPublicKey", "desc":"支付宝公钥(不使用证书时必填)", "type": "textarea","star":"1"},{"name":"signType","desc":"接口签名方式(推荐使用RSA2)","type":"radio","verify":"","values":"RSA,RSA2","titles":"RSA,RSA2","verify":"required"},{"name":"useCert","desc":"公钥证书","type":"radio","verify":"","values":"1,0","titles":"使用证书（请使用RSA2私钥）,不使用证书"},{"name":"appPublicCert","desc":"应用公钥证书（.crt格式）","type":"file","verify":""},{"name":"alipayPublicCert","desc":"支付宝公钥证书（.crt格式）","type":"file","verify":""},{"name":"alipayRootCert","desc":"支付宝根证书（.crt格式）","type":"file","verify":""}]',
        '[{"wayCode": "ALI_JSAPI"}, {"wayCode": "ALI_WAP"}, {"wayCode": "ALI_BAR"}, {"wayCode": "ALI_APP"}, {"wayCode": "ALI_PC"}, {"wayCode": "ALI_QR"}]',
        'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/alipay.png', '#1779FF', 1, '支付宝官方通道');

INSERT INTO `t_pay_interface_define` (`if_code`, `if_name`, `is_mch_mode`, `is_isv_mode`, `config_page_type`, `isv_params`, `isvsub_mch_params`, `normal_mch_params`, `way_codes`, `icon`, `bg_color`, `state`, `remark`)
VALUES ('wxpay', '微信支付官方', 1, 1, 2,
        '[{"name":"mchId", "desc":"微信支付商户号", "type": "text","verify":"required"},{"name":"appId","desc":"应用App ID","type":"text","verify":"required"},{"name":"appSecret","desc":"应用AppSecret","type":"text","verify":"required","star":"1"},{"name":"oauth2Url", "desc":"oauth2地址（置空将使用官方）", "type": "text"},{"name":"key", "desc":"API密钥", "type": "textarea","verify":"required","star":"1"},{"name":"apiVersion", "desc":"微信支付API版本", "type": "radio","values":"V2,V3","titles":"V2,V3","verify":"required"},{"name":"apiV3Key", "desc":"API V3秘钥（V3接口必填）", "type": "textarea","verify":"","star":"1"},{"name":"serialNo", "desc":"序列号（V3接口必填）", "type": "textarea","verify":"","star":"1"},{"name":"cert", "desc":"API证书(.p12格式)", "type": "file","verify":""},{"name":"apiClientKey", "desc":"私钥文件(.pem格式)", "type": "file","verify":""}]',
        '[{"name":"subMchId","desc":"子商户ID","type":"text","verify":"required"},{"name":"subMchAppId","desc":"子账户appID(线上支付必填)","type":"text","verify":""}]',
        '[{"name":"mchId", "desc":"微信支付商户号", "type": "text","verify":"required"},{"name":"appId","desc":"应用App ID","type":"text","verify":"required"},{"name":"appSecret","desc":"应用AppSecret","type":"text","verify":"required","star":"1"},{"name":"oauth2Url", "desc":"oauth2地址（置空将使用官方）", "type": "text"},{"name":"key", "desc":"API密钥", "type": "textarea","verify":"required","star":"1"},{"name":"apiVersion", "desc":"微信支付API版本", "type": "radio","values":"V2,V3","titles":"V2,V3","verify":"required"},{"name":"apiV3Key", "desc":"API V3秘钥（V3接口必填）", "type": "textarea","verify":"","star":"1"},{"name":"serialNo", "desc":"序列号（V3接口必填）", "type": "textarea","verify":"","star":"1" },{"name":"cert", "desc":"API证书(.p12格式)", "type": "file","verify":""},{"name":"apiClientKey", "desc":"私钥文件(.pem格式)", "type": "file","verify":""}]',
        '[{"wayCode": "WX_APP"}, {"wayCode": "WX_H5"}, {"wayCode": "WX_NATIVE"}, {"wayCode": "WX_JSAPI"}, {"wayCode": "WX_BAR"}, {"wayCode": "WX_LITE"}]',
        'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/wxpay.png', '#04BE02', 1, '微信官方通道');

INSERT INTO `t_pay_interface_define` (`if_code`, `if_name`, `is_mch_mode`, `is_isv_mode`, `config_page_type`, `isv_params`, `isvsub_mch_params`, `normal_mch_params`, `way_codes`, `icon`, `bg_color`, `state`, `remark`)
VALUES ('ysfpay', '云闪付官方', 0, 1, 1,
        '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"","values":"1,0","titles":"沙箱环境,生产环境","verify":"required"},{"name":"serProvId","desc":"服务商开发ID[serProvId]","type":"text","verify":"required"},{"name":"isvPrivateCertFile","desc":"服务商私钥文件（.pfx格式）","type":"file","verify":"required"},{"name":"isvPrivateCertPwd","desc":"服务商私钥文件密码","type":"text","verify":"required","star":"1"},{"name":"ysfpayPublicKey","desc":"云闪付开发公钥（证书管理页面可查询）","type":"textarea","verify":"required","star":"1"},{"name":"acqOrgCode","desc":"可用支付机构编号","type":"text","verify":"required"}]',
        '[{"name":"merId","desc":"商户编号","type":"text","verify":"required"}]',
        NULL,
        '[{"wayCode": "YSF_BAR"}, {"wayCode": "ALI_JSAPI"}, {"wayCode": "WX_JSAPI"}, {"wayCode": "ALI_BAR"}, {"wayCode": "WX_BAR"}]',
        'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/ysfpay.png', 'red', 1, '云闪付官方通道');

INSERT INTO `t_pay_interface_define` (`if_code`, `if_name`, `is_mch_mode`, `is_isv_mode`, `config_page_type`, `isv_params`, `isvsub_mch_params`, `normal_mch_params`, `way_codes`, `icon`, `bg_color`, `state`, `remark`)
VALUES ('pppay', 'PayPal支付', 1, 0, 1,
        NULL,
        NULL,
        '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境, 生产环境"},{"name":"clientId","desc":"Client ID（客户端ID）","type":"text","verify":"required"},{"name":"secret","desc":"Secret（密钥）","type":"text","verify":"required","star":"1"},{"name":"refundWebhook","desc":"退款 Webhook id","type":"text","verify":"required"},{"name":"notifyWebhook","desc":"支付 Webhook id","type":"text","verify":"required"}]',
        '[{"wayCode": "PP_PC"}]',
        'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/paypal.png', '#005ea6', 1, 'PayPal官方通道');

INSERT INTO `t_pay_interface_define` (`if_code`, `if_name`, `is_mch_mode`, `is_isv_mode`, `config_page_type`, `isv_params`, `isvsub_mch_params`, `normal_mch_params`, `way_codes`, `icon`, `bg_color`, `state`, `remark`)
VALUES ('sxfpay', '随行付支付', 0, 1, 1,
        '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"orgId","desc":"机构编号","type":"text","verify":"required"},{"name":"publicKey","desc":"天阙平台公钥","type":"textarea","verify":"required","star":"1"},{"name":"privateKey","desc":"天阙平台私钥","type":"textarea","verify":"required","star":"1"},{"name":"channelNoWx","desc":"微信渠道号[服务商通过海科在(微信)申请的渠道编号]","type":"text","verify":"required"},{"name":"channelNoAli","desc":"支付宝渠道号[服务商自行申请的支付宝渠道号(PID)]","type":"text","verify":"required"}]',
        '[{"name":"mno","desc":"商户编号","type":"text","verify":"required"},{"name":"subMchLiteAppId","desc":"子商户小程序AppId(置空表示使用服务商)","type":"text"}]',
        NULL,
        '[{"wayCode": "ALI_BAR"},{"wayCode": "ALI_JSAPI"},{"wayCode": "ALI_QR"},{"wayCode": "WX_BAR"},{"wayCode": "WX_JSAPI"},{"wayCode": "WX_NATIVE"},{"wayCode": "YSF_BAR"},{"wayCode": "YSF_JSAPI"}]',
        'https://paas.tianquetech.com/favicon.png', '#0084c0', 1, '天阙开放平台');

INSERT INTO `t_pay_interface_define` (`if_code`, `if_name`, `is_mch_mode`, `is_isv_mode`, `config_page_type`, `isv_params`, `isvsub_mch_params`, `normal_mch_params`, `way_codes`, `icon`, `bg_color`, `state`, `remark`)
VALUES ('lespay', '乐刷支付', 0, 1, 1,
        '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"agentId","desc":"服务商编号","type":"text","verify":"required"},{"name":"tradeKey","desc":"交易密钥","type":"text","verify":"required","star":"1"},{"name":"noticeKey","desc":"异步通知回调密钥","type":"text","verify":"required","star":"1"},{"name":"channelNoWx","desc":"微信渠道号[服务商通过海科在(微信)申请的渠道编号]","type":"text","verify":"required"},{"name":"channelNoAli","desc":"支付宝渠道号[服务商自行申请的支付宝渠道号(PID)]","type":"text","verify":"required"}]',
        '[{"name":"merchantId","desc":"商户编号","type":"text","verify":"required"},{"name":"limitPay","desc":"是否禁止信用卡（默认为不禁用）","type":"radio","verify":"","values":"0,1","titles":"否,是"},{"name":"t0","desc":"t0交易标志（默认为d1）","type":"radio","verify":"required","values":"0,1","titles":"d1交易,d0交易"}]',
        NULL,
        '[{"wayCode": "ALI_BAR"},{"wayCode": "ALI_JSAPI"},{"wayCode": "ALI_QR"},{"wayCode": "WX_BAR"},{"wayCode": "WX_JSAPI"},{"wayCode": "YSF_BAR"},{"wayCode": "YSF_JSAPI"}]',
        'https://h5-static.yeahka.com/static/leshuatech/images/favicon2.ico', '#ffe353', 1, '乐刷聚合支付');

INSERT INTO `t_pay_interface_define` (`if_code`, `if_name`, `is_mch_mode`, `is_isv_mode`, `config_page_type`, `isv_params`, `isvsub_mch_params`, `normal_mch_params`, `way_codes`, `icon`, `bg_color`, `state`, `remark`)
VALUES ('hkrtpay', '海科融通支付', 0, 1, 1,
        '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"agentNo","desc":"服务商编号","type":"text","verify":"required"},{"name":"accessId","desc":"接入机构标识accessid","type":"text","verify":"required"},{"name":"accessKey","desc":"服务商的接入秘钥","type":"text","verify":"required","star":"1"},{"name":"transferKey","desc":"服务商的传输密钥","type":"text","verify":"required","star":"1"},{"name":"channelNoWx","desc":"微信渠道号[服务商通过海科在(微信)申请的渠道编号]","type":"text","verify":"required"},{"name":"channelNoAli","desc":"支付宝渠道号[服务商自行申请的支付宝渠道号(PID)]","type":"text","verify":"required"}]',
        '[{"name":"merchNo","desc":"商户编号","type":"text","verify":"required"}]',
        NULL,
        '[{"wayCode": "ALI_BAR"},{"wayCode": "ALI_JSAPI"},{"wayCode": "ALI_QR"},{"wayCode": "WX_BAR"},{"wayCode": "WX_JSAPI"},{"wayCode": "YSF_BAR"},{"wayCode": "YSF_JSAPI"}]',
        'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/hkpay.svg', '#2d2f92', 1, '海科融通支付');

INSERT INTO `t_pay_interface_define` (`if_code`, `if_name`, `is_mch_mode`, `is_isv_mode`, `config_page_type`, `isv_params`, `isvsub_mch_params`, `normal_mch_params`, `way_codes`, `icon`, `bg_color`, `state`, `remark`)
VALUES ('umspay', '银联商务支付', 0, 1, 1,
        '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"appId","desc":"产品ID","type":"text","verify":"required","star":"1"},{"name":"appKey","desc":"产品密钥","type":"text","verify":"required","star":"1"}]',
        '[{"name":"mid","desc":"商户号","type":"text","verify":"required"},{"name":"tid","desc":"终端号","type":"text","verify":"required"}]',
        NULL,
        '[{"wayCode": "ALI_BAR"},{"wayCode": "ALI_JSAPI"},{"wayCode": "ALI_QR"},{"wayCode": "WX_BAR"},{"wayCode": "WX_JSAPI"},{"wayCode": "WX_NATIVE"},{"wayCode": "YSF_BAR"},{"wayCode": "YSF_JSAPI"}]',
        'https://saas.chinaums.com/saas-web/img/logo.689cab92.png', '#1e0ae8', 1, '银联商务天满服务平台');

INSERT INTO `t_pay_interface_define` (`if_code`, `if_name`, `is_mch_mode`, `is_isv_mode`, `config_page_type`, `isv_params`, `isvsub_mch_params`, `normal_mch_params`, `way_codes`, `icon`, `bg_color`, `state`, `remark`)
VALUES ('lcswpay', '利楚扫呗支付', 1, 0, 1,
        NULL,
        NULL,
        '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境, 生产环境"},{"name":"merchantNo","desc":"商户号","type":"text","verify":"required"},{"name":"terminalId","desc":"终端号","type":"text","verify":"required"},{"name":"accessToken","desc":"令牌标识","type":"text","verify":"required","star":"1"},{"name":"subMchAppId","desc":"子商户公众号AppId","type":"text"},{"name":"subMchLiteAppId","desc":"子商户小程序AppId","type":"text"}]',
        '[{"wayCode": "ALI_BAR"},{"wayCode": "ALI_JSAPI"},{"wayCode": "ALI_QR"},{"wayCode": "WX_BAR"},{"wayCode": "WX_JSAPI"},{"wayCode": "WX_NATIVE"},{"wayCode": "YSF_BAR"},{"wayCode": "YSF_JSAPI"}]',
        'https://www.lcsw.cn/20170616033613576_easyicon_net_48.ico', '#00AFFE', 1, '利楚扫呗支付');

-- 初始化设备供应商接口定义
INSERT INTO `t_device_provider_define` (`provider_code`, `provider_name`, `config_page_type`, `provider_params`, `device_types`, `icon`, `bg_color`, `state`, `remark`)
VALUES ('zgwl', '智谷联', 1,
        '[{"name":"accessKeyId","desc":"AccessKeyId","type":"text","verify":"required","star":"1"},{"name":"accessKeySecret","desc":"AccessKeySecret","type":"text","verify":"required","star":"1"},{"name":"instanceId","desc":"实例ID","type":"text","verify":"required"},{"name":"endPoint","desc":"公网接入点","type":"text","verify":"required"},{"name":"topic","desc":"Topic","type":"text","verify":"required"},{"name":"groupId","desc":"GroupId","type":"text","verify":"required"}]',
        '[1, 2]', 
        'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/alipay.png', '#1779FF', 1, '智谷联');

#####  ↓↓↓↓↓↓↓↓↓↓  修改表结构  ↓↓↓↓↓↓↓↓↓↓  #####


