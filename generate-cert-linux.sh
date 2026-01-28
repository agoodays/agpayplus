#!/bin/bash
# ========================================
# AgPay+ Linux/macOS SSL 证书生成脚本
# ========================================
# 功能：生成自签名开发证书
# 使用：./generate-cert-linux.sh
# ========================================

set -e

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  SSL 证书生成工具${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

# 证书配置
CERT_NAME="agpayplusapi"
CERT_PASSWORD="123456"
CERT_PATH="$HOME/.aspnet/https"
CERT_FILE="$CERT_PATH/$CERT_NAME.pfx"

# 创建证书目录
if [ ! -d "$CERT_PATH" ]; then
    mkdir -p "$CERT_PATH"
    echo -e "${GREEN}✅ 创建证书目录: $CERT_PATH${NC}"
fi

# 检查是否已存在证书
if [ -f "$CERT_FILE" ]; then
    echo -e "${YELLOW}! 证书文件已存在: $CERT_FILE${NC}"
    read -p "是否覆盖现有证书? (y/n): " overwrite
    if [ "$overwrite" != "y" ]; then
        echo -e "${YELLOW}已取消${NC}"
        exit 0
    fi
    rm -f "$CERT_FILE"
fi

echo -e "\n${YELLOW}正在生成证书...${NC}"

# 检查 .NET SDK 是否已安装
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET SDK 未安装${NC}"
    echo ""
    echo -e "${CYAN}请先安装 .NET SDK：${NC}"
    echo -e "${GRAY}  Ubuntu/Debian: https://docs.microsoft.com/dotnet/core/install/linux-ubuntu${NC}"
    echo -e "${GRAY}  macOS: brew install --cask dotnet-sdk${NC}"
    echo ""
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo -e "${GREEN}✅ .NET SDK 版本: $DOTNET_VERSION${NC}"

# 清理旧的开发证书
echo -e "\n${GRAY}清理旧证书...${NC}"
dotnet dev-certs https --clean &> /dev/null || true

# 生成新的开发证书
echo -e "${GRAY}生成新证书...${NC}"

# 根据操作系统使用不同的信任命令
OS_TYPE=$(uname -s)
if [ "$OS_TYPE" = "Darwin" ]; then
    # macOS
    if dotnet dev-certs https -ep "$CERT_FILE" -p "$CERT_PASSWORD" --trust; then
        SUCCESS=true
    else
        SUCCESS=false
    fi
else
    # Linux - 不支持 --trust 参数
    if dotnet dev-certs https -ep "$CERT_FILE" -p "$CERT_PASSWORD"; then
        SUCCESS=true
        
        # 在 Linux 上提供手动信任说明
        echo -e "\n${YELLOW}注意：Linux 系统需要手动信任证书${NC}"
        echo -e "${GRAY}执行以下命令将证书添加到系统信任列表：${NC}"
        echo -e "${CYAN}  # Ubuntu/Debian${NC}"
        echo -e "  sudo cp ~/.aspnet/https/agpayplusapi.pfx /usr/local/share/ca-certificates/agpayplusapi.crt"
        echo -e "  sudo update-ca-certificates"
        echo ""
    else
        SUCCESS=false
    fi
fi

if [ "$SUCCESS" = true ]; then
    echo -e "\n${CYAN}========================================${NC}"
    echo -e "${GREEN}✅ 证书生成成功！${NC}"
    echo -e "${CYAN}========================================${NC}"
    echo ""
    echo -e "${CYAN}证书信息：${NC}"
    echo -e "  文件路径: $CERT_FILE"
    echo -e "  证书密码: $CERT_PASSWORD"
    echo -e "  证书名称: $CERT_NAME"
    echo ""
    
    if [ "$OS_TYPE" = "Darwin" ]; then
        echo -e "${GREEN}注意：此证书已自动添加到系统信任列表${NC}"
    fi
    echo ""
else
    echo -e "\n${RED}❌ 证书生成失败${NC}"
    echo ""
    echo -e "${CYAN}解决方案：${NC}"
    echo -e "${GRAY}  1. 确保已安装 .NET SDK${NC}"
    echo -e "${GRAY}  2. 检查证书目录权限${NC}"
    echo -e "${GRAY}  3. macOS 用户需要输入系统密码以信任证书${NC}"
    echo ""
    exit 1
fi

# 验证证书
echo -e "${YELLOW}验证证书...${NC}"
if [ -f "$CERT_FILE" ]; then
    FILE_SIZE=$(ls -lh "$CERT_FILE" | awk '{print $5}')
    echo -e "${GREEN}✅ 证书验证成功${NC}"
    echo -e "${GRAY}  文件大小: $FILE_SIZE${NC}"
else
    echo -e "${RED}❌ 证书文件不存在${NC}"
    exit 1
fi

echo ""
