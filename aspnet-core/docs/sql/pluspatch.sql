#####    增量发布SQL   #####
## 历史初始化需要执行，新初始化无需执行

ALTER TABLE `t_sys_role` 
ADD COLUMN `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间' AFTER `belong_info_id`;

ALTER TABLE `agpayplusdb_dev`.`t_sys_user_team` 
MODIFY COLUMN `team_id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '团队ID' FIRST,
MODIFY COLUMN `created_uid` BIGINT(20) NULL DEFAULT NULL COMMENT '创建者用户ID' AFTER `belong_info_id`;

ALTER TABLE `t_sys_config` 
ADD COLUMN `created_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '创建时间' AFTER `sort_num`;

ALTER TABLE `t_sys_article` 
MODIFY COLUMN `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID' AFTER `publish_time`,
MODIFY COLUMN `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名' AFTER `created_uid`,
MODIFY COLUMN `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间' AFTER `created_at`;

ALTER TABLE `t_sys_advert` 
MODIFY COLUMN `advert_sort` INT(11) NOT NULL DEFAULT '0' COMMENT '排序字段, 规则：正序' AFTER `link_url`,
MODIFY COLUMN `release_state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '发布状态 0-待发布, 1-已发布' AFTER `advert_sort`,
MODIFY COLUMN `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID' AFTER `agent_no`,
MODIFY COLUMN `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名' AFTER `created_uid`,
MODIFY COLUMN `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间' AFTER `created_at`;

ALTER TABLE `t_sys_log` 
ADD COLUMN `updated_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT '更新时间' AFTER `created_at`;

ALTER TABLE `t_mch_info` 
MODIFY COLUMN `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID' AFTER `init_user_id`,
MODIFY COLUMN `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名' AFTER `created_uid`;

ALTER TABLE `t_mch_apply` 
MODIFY COLUMN `state` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '状态: 0-草稿, 1-审核中, 2-进件成功, 3-驳回待修改, 4-待验证, 5-待签约, 7-等待预审, 8-预审拒绝' AFTER `channel_var2`,
MODIFY COLUMN `merchant_type` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '商户类型: 1-个人, 2-个体工商户, 3-企业' AFTER `mch_short_name`,
MODIFY COLUMN `ep_user_id` BIGINT(20) DEFAULT NULL COMMENT '商户拓展员ID' AFTER `address`,
MODIFY COLUMN `last_apply_at` TIMESTAMP(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT '上次进件时间' AFTER `ep_user_id`,
MODIFY COLUMN `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID' AFTER `last_apply_at`;

ALTER TABLE `t_mch_app` 
MODIFY COLUMN `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID' AFTER `remark`,
MODIFY COLUMN `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名' AFTER `created_uid`;

ALTER TABLE `t_mch_store` 
MODIFY COLUMN `store_id` BIGINT(20) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '门店ID' FIRST,
MODIFY COLUMN `default_flag` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否默认: 0-否, 1-是' AFTER `lat`,
MODIFY COLUMN `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID' AFTER `bind_app_id`;

ALTER TABLE `t_mbr_info` 
MODIFY COLUMN `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID' AFTER `state`,
MODIFY COLUMN `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名' AFTER `created_uid`;

ALTER TABLE `t_mbr_recharge_rule` 
MODIFY COLUMN `rule_id` BIGINT(20) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '规则ID' FIRST;

ALTER TABLE `t_agent_info` 
MODIFY COLUMN `agent_type` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '代理商类型: 1-个人, 2-企业' AFTER `agent_short_name`,
MODIFY COLUMN `level` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '等级: 1-一级, 2-二级, 3-三级 ...' AFTER `agent_type`,
MODIFY COLUMN `add_agent_flag` TINYINT(6) NOT NULL DEFAULT '0' COMMENT '是否允许发展下级: 0-否, 1-是' AFTER `contact_email`,
MODIFY COLUMN `state` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '状态: 0-停用, 1-正常' AFTER `sipw`,
MODIFY COLUMN `init_user_id` BIGINT(20) DEFAULT NULL COMMENT '初始用户ID（创建商户时，允许商户登录的用户）' AFTER `remark`,
MODIFY COLUMN `cashout_fee_rule_type` TINYINT(6) NOT NULL DEFAULT '1' COMMENT '提现配置类型: 1-使用系统默认, 2-自定义' AFTER `sett_account_sub_bank`,
MODIFY COLUMN `un_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '不可用金额' AFTER `bank_card_img`, 
MODIFY COLUMN `balance_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '钱包余额' AFTER `un_amount`,
MODIFY COLUMN `audit_profit_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '在途佣金' AFTER `balance_amount`, 
MODIFY COLUMN `freeze_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '冻结金额' AFTER `audit_profit_amount`,
MODIFY COLUMN `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID' AFTER `freeze_amount`;

ALTER TABLE `t_account_bill` 
MODIFY COLUMN `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT '流水号' FIRST,
MODIFY COLUMN `before_balance` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '变动前余额,单位分' AFTER `info_type`,
MODIFY COLUMN `change_amount` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '变动金额,单位分' AFTER `before_balance`,
MODIFY COLUMN `after_balance` BIGINT(20) NOT NULL DEFAULT '0' COMMENT '变动后余额,单位分' AFTER `change_amount`;

ALTER TABLE `t_isv_info` 
MODIFY COLUMN `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID' AFTER `remark`,
MODIFY COLUMN `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名' AFTER `created_uid`;

ALTER TABLE `t_pay_rate_config` 
MODIFY COLUMN `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID' FIRST,
MODIFY COLUMN `applyment_support` TINYINT(6) NOT NULL COMMENT '是否支持进件: 0-不支持, 1-支持' AFTER `fee_rate`,
MODIFY COLUMN `state` TINYINT(6) NOT NULL COMMENT '状态: 0-停用, 1-启用' AFTER `applyment_support`;

ALTER TABLE `t_pay_rate_level_config` 
MODIFY COLUMN `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID' FIRST,
MODIFY COLUMN `rate_config_id` BIGINT(20) NOT NULL COMMENT '支付费率配置ID' AFTER `id`,
MODIFY COLUMN `min_amount` INT(11) NOT NULL DEFAULT '0' COMMENT '最小金额: 计算时大于此值' AFTER `bank_card_type`,
MODIFY COLUMN `max_amount` INT(11) NOT NULL DEFAULT '0' COMMENT '最大金额: 计算时小于或等于此值' AFTER `min_amount`,
MODIFY COLUMN `min_fee` INT(11) NOT NULL DEFAULT '0' COMMENT '保底费用' AFTER `max_amount`,
MODIFY COLUMN `max_fee` INT(11) NOT NULL DEFAULT '0' COMMENT '封顶费用' AFTER `min_fee`,
MODIFY COLUMN `state` TINYINT(6) NOT NULL COMMENT '状态: 0-停用, 1-启用' AFTER `fee_rate`;

ALTER TABLE `t_pay_order_profit` 
MODIFY COLUMN `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID' FIRST,
MODIFY COLUMN `fee_amount` BIGINT(20) NOT NULL COMMENT '手续费(实际手续费),单位分' AFTER `fee_rate_desc`,
MODIFY COLUMN `order_fee_amount` BIGINT(20) NOT NULL COMMENT '收单手续费,单位分' AFTER `fee_amount`,
MODIFY COLUMN `profit_amount` BIGINT(20) NOT NULL COMMENT '分润金额(实际分润),单位分' AFTER `profit_rate`,
MODIFY COLUMN `order_profit_amount` BIGINT(20) NOT NULL COMMENT '收单分润金额,单位分' AFTER `profit_amount`;

ALTER TABLE `t_mch_division_receiver_group` 
MODIFY COLUMN `created_uid` BIGINT(20) DEFAULT NULL COMMENT '创建者用户ID' AFTER `auto_division_flag`,
MODIFY COLUMN `created_by` VARCHAR(64) DEFAULT NULL COMMENT '创建者姓名' AFTER `created_uid`;

ALTER TABLE `t_qr_code` 
MODIFY COLUMN `qrc_shell_id` INT(11) COMMENT '模板ID' AFTER `qrc_id`,
MODIFY COLUMN `fixed_flag` TINYINT(6) NOT NULL COMMENT '是否固定金额: 0-任意金额, 1-固定金额' AFTER `batch_id`,
MODIFY COLUMN `fixed_pay_amount` INT(11) NOT NULL DEFAULT '0' COMMENT '固定金额' AFTER `fixed_flag`,
MODIFY COLUMN `bind_state` TINYINT(6) NOT NULL COMMENT '码牌绑定状态: 0-未绑定, 1-已绑定' AFTER `qrc_alias`,
MODIFY COLUMN `store_id` BIGINT(20) NULL COMMENT '门店ID' AFTER `app_id`,
MODIFY COLUMN `state` TINYINT(6) NOT NULL COMMENT '状态: 0-停用, 1-启用' AFTER `url_placeholder`;

ALTER TABLE `t_check_bill_batch` 
MODIFY COLUMN `handle_state` TINYINT(6) NOT NULL COMMENT '处理状态: 0-未处理, 1-已处理' AFTER `parse_err_msg`,
MODIFY COLUMN `total_fee_amount` BIGINT(20) NOT NULL COMMENT '总手续费,单位分' AFTER `total_count`,
MODIFY COLUMN `channel_total_fee_amount` BIGINT(20) NOT NULL COMMENT '渠道总手续费,单位分' AFTER `channel_total_count`;

ALTER TABLE `t_channel_bill` 
MODIFY COLUMN `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID' FIRST,
MODIFY COLUMN `channel_fee_amount` BIGINT(20) NOT NULL COMMENT '渠道手续费,单位分' AFTER `channel_amount`;

ALTER TABLE `t_diff_bill` 
MODIFY COLUMN `id` BIGINT(20) NOT NULL AUTO_INCREMENT COMMENT 'ID' FIRST,
MODIFY COLUMN `fee_amount` BIGINT(20) NOT NULL COMMENT '渠道手续费,单位分' AFTER `amount`,
MODIFY COLUMN `channel_fee_amount` BIGINT(20) NOT NULL COMMENT '渠道手续费,单位分' AFTER `channel_amount`;

ALTER TABLE `t_device_info` 
MODIFY COLUMN `bind_state` TINYINT(6) NOT NULL COMMENT '绑定状态: 0-未绑定, 1-已绑定' AFTER `batch_id`,
MODIFY COLUMN `bind_type` TINYINT(6) NOT NULL COMMENT '绑定类型: 0-门店, 1-码牌' AFTER `bind_state`,
MODIFY COLUMN `store_id` BIGINT(20) NOT NULL COMMENT '门店ID' AFTER `app_id`,
MODIFY COLUMN `bind_qrc_id` BIGINT(20) NOT NULL COMMENT '绑定码牌ID' AFTER `store_id`;