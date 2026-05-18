-- 优化 t_pay_order 表索引

-- 按商户号和状态查询索引
CREATE INDEX Idx_MchNo_State ON t_pay_order(mch_no, state);

-- 按应用ID查询索引
CREATE INDEX Idx_AppId ON t_pay_order(app_id);

-- 按支付方式查询索引
CREATE INDEX Idx_WayCode ON t_pay_order(way_code);

-- 按成功时间查询索引
CREATE INDEX Idx_SuccessTime ON t_pay_order(success_time);

-- 按代理商号查询索引
CREATE INDEX Idx_AgentNo ON t_pay_order(agent_no);

-- 按服务商号查询索引
CREATE INDEX Idx_IsvNo ON t_pay_order(isv_no);

-- 按渠道订单号查询索引
CREATE INDEX Idx_ChannelOrderNo ON t_pay_order(channel_order_no);

-- 按创建时间和状态查询索引
CREATE INDEX Idx_CreatedAt_State ON t_pay_order(created_at, state);
