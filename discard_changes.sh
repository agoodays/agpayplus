#!/bin/bash

# 定义需要保留的文件列表
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

# 添加需要保留的文件到暂存区
for file in "${KEEP_FILES[@]}"; do
  git add "$file"
done

# 获取所有已修改的文件
ALL_CHANGED_FILES=$(git status --porcelain | grep '^ M' | awk '{print $2}')

# 遍历所有已修改的文件，并且如果不是要保留的文件，则重置它们
for file in $ALL_CHANGED_FILES; do
  if [[ ! " ${KEEP_FILES[@]} " =~ " ${file} " ]]; then
    echo "Resetting: $file"
    git checkout -- "$file"
  fi
done

# 执行 git pull 操作
echo "Pulling the latest changes from remote repository..."
git pull

echo "Operation completed."