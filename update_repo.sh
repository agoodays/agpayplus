#!/bin/bash

KEEP_FILES=(
  "aspnet-core/src/AGooday.AgPay.Agent.Api/Dockerfile"
  "aspnet-core/src/AGooday.AgPay.Agent.Api/appsettings.json"
  "aspnet-core/src/AGooday.AgPay.Manager.Api/Dockerfile"
  "aspnet-core/src/AGooday.AgPay.Manager.Api/appsettings.json"
  "aspnet-core/src/AGooday.AgPay.Merchant.Api/Dockerfile"
  "aspnet-core/src/AGooday.AgPay.Merchant.Api/appsettings.json"
  "aspnet-core/src/AGooday.AgPay.Payment.Api/Dockerfile"
  "aspnet-core/src/AGooday.AgPay.Payment.Api/appsettings.json"
)

for file in "${KEEP_FILES[@]}"; do
  git add "$file"
done

# 放弃所有其他未暂存的更改
git restore .

# 或者使用下面的命令如果 Git 版本较低
# git checkout .

# 拉取最新的远程代码
git pull origin main