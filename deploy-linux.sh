#!/bin/bash
# ========================================
# AgPay+ Linux/macOS 部署脚本
# ========================================
# 功能：一键部署所有服务
# 使用：./deploy-linux.sh
# ========================================

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SKIP_CERT=false
SKIP_ENV=false

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# 解析参数
while [[ $# -gt 0 ]]; do
    case $1 in
        --skip-cert)
            SKIP_CERT=true
            shift
            ;;
        --skip-env)
            SKIP_ENV=true
            shift
            ;;
        *)
            echo -e "${RED}未知参数: $1${NC}"
            exit 1
            ;;
    esac
done

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  AgPay+ Linux/macOS 部署脚本${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

# 检查 Docker 是否运行
echo -e "${YELLOW}[1/7] 检查 Docker 环境...${NC}"
if ! command -v docker &> /dev/null; then
    echo -e "${RED}  ❌ Docker 未安装${NC}"
    exit 1
fi

if ! docker version &> /dev/null; then
    echo -e "${RED}  ❌ Docker 未运行，请先启动 Docker${NC}"
    exit 1
fi

DOCKER_VERSION=$(docker version --format '{{.Server.Version}}')
echo -e "${GREEN}  ✅ Docker 版本: $DOCKER_VERSION${NC}"

# 检查 Docker Compose
if ! docker compose version &> /dev/null; then
    echo -e "${RED}  ❌ Docker Compose 未安装${NC}"
    exit 1
fi

COMPOSE_VERSION=$(docker compose version --short)
echo -e "${GREEN}  ✅ Docker Compose 版本: $COMPOSE_VERSION${NC}"

# 配置环境变量文件
echo -e "\n${YELLOW}[2/7] 配置环境变量...${NC}"
if [ "$SKIP_ENV" = false ]; then
    if [ -f "$SCRIPT_DIR/.env" ]; then
        echo -e "${YELLOW}  ! 检测到已存在的 .env 文件${NC}"
        read -p "  是否使用 .env.linux 覆盖? (y/n): " response
        if [ "$response" = "y" ]; then
            cp "$SCRIPT_DIR/.env.linux" "$SCRIPT_DIR/.env"
            echo -e "${GREEN}  ✅ 已复制 .env.linux 到 .env${NC}"
        else
            echo -e "${GREEN}  ✅ 使用现有 .env 配置${NC}"
        fi
    else
        cp "$SCRIPT_DIR/.env.linux" "$SCRIPT_DIR/.env"
        echo -e "${GREEN}  ✅ 已创建 .env 文件${NC}"
    fi
    
    # 提示用户修改配置
    echo -e "\n${CYAN}  请检查并修改 .env 文件中的配置：${NC}"
    echo -e "${GRAY}    - IPORDOMAIN: 服务器IP或域名${NC}"
    echo -e "${GRAY}    - MYSQL_SERVER_NAME: MySQL服务器地址（Linux需要宿主机IP）${NC}"
    echo -e "${GRAY}    - MYSQL_*: MySQL数据库配置${NC}"
    echo -e "${GRAY}    - DATA_PATH_HOST: 数据存储路径${NC}"
    echo ""
    read -p "  配置完成后按 Enter 继续，或输入 'n' 退出: " continue
    if [ "$continue" = "n" ]; then
        echo -e "${YELLOW}  部署已取消${NC}"
        exit 0
    fi
else
    echo -e "${GREEN}  ✅ 跳过环境变量配置${NC}"
fi

# 生成 SSL 证书
echo -e "\n${YELLOW}[3/7] 配置 SSL 证书...${NC}"
if [ "$SKIP_CERT" = false ]; then
    CERT_PATH="$HOME/.aspnet/https"
    CERT_FILE="$CERT_PATH/agpayplusapi.pfx"
    
    if [ -f "$CERT_FILE" ]; then
        echo -e "${YELLOW}  ! 检测到已存在的证书文件${NC}"
        read -p "  是否重新生成证书? (y/n): " response
        if [ "$response" = "y" ]; then
            echo -e "${GRAY}  正在生成新证书...${NC}"
            bash "$SCRIPT_DIR/generate-cert-linux.sh"
        else
            echo -e "${GREEN}  ✅ 使用现有证书${NC}"
        fi
    else
        echo -e "${GRAY}  正在生成证书...${NC}"
        bash "$SCRIPT_DIR/generate-cert-linux.sh"
    fi
else
    echo -e "${GREEN}  ✅ 跳过证书生成${NC}"
fi

# 创建数据目录
echo -e "\n${YELLOW}[4/7] 创建数据目录...${NC}"
DATA_PATH=$(grep "DATA_PATH_HOST=" "$SCRIPT_DIR/.env" | cut -d'=' -f2)

directories=(
    "$DATA_PATH"
    "$DATA_PATH/logs"
    "$DATA_PATH/upload"
    "$DATA_PATH/seq"
    "$DATA_PATH/mysql"
    "$DATA_PATH/redis"
    "$DATA_PATH/rabbitmq"
)

for dir in "${directories[@]}"; do
    if [ ! -d "$dir" ]; then
        sudo mkdir -p "$dir"
        sudo chown -R $(whoami):$(whoami) "$dir"
        echo -e "${GREEN}  ✅ 创建目录: $dir${NC}"
    else
        echo -e "${GRAY}  ℹ️ 目录已存在: $dir${NC}"
    fi
done

# 停止并删除旧容器
echo -e "\n${YELLOW}[5/7] 清理旧容器...${NC}"
cd "$SCRIPT_DIR"
if docker compose ps -q &> /dev/null; then
    docker compose down --remove-orphans &> /dev/null || true
    echo -e "${GREEN}  ✅ 已清理旧容器${NC}"
else
    echo -e "${GRAY}  ! 没有需要清理的容器${NC}"
fi

# 构建镜像
echo -e "\n${YELLOW}[6/7] 构建 Docker 镜像...${NC}"
echo -e "${GRAY}  这可能需要几分钟时间，请耐心等待...${NC}"
cd "$SCRIPT_DIR"
if docker compose build --no-cache; then
    echo -e "${GREEN}  ✅ 镜像构建成功${NC}"
else
    echo -e "${RED}  ❌ 镜像构建失败${NC}"
    exit 1
fi

# 启动服务
echo -e "\n${YELLOW}[7/7] 启动服务...${NC}"
cd "$SCRIPT_DIR"
if docker compose up -d; then
    echo -e "${GREEN}  ✅ 服务启动成功${NC}"
else
    echo -e "${RED}  ❌ 服务启动失败${NC}"
    exit 1
fi

# 显示服务状态
echo -e "\n${CYAN}========================================${NC}"
echo -e "${GREEN}  部署完成！${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""
echo -e "${CYAN}服务访问地址：${NC}"
echo -e "  运营平台:    https://localhost:8817"
echo -e "  代理商系统:  https://localhost:8816"
echo -e "  商户系统:    https://localhost:8818"
echo -e "  支付网关:    https://localhost:9819"
echo -e "  收银台:      https://localhost:9819/cashier"
echo -e "  RabbitMQ:    http://localhost:15672 (admin/admin)"
echo -e "  Seq:         http://localhost:5341"
echo ""
echo -e "${GRAY}查看服务状态：docker compose ps${NC}"
echo -e "${GRAY}查看服务日志：docker compose logs -f [service-name]${NC}"
echo -e "${GRAY}停止所有服务：docker compose down${NC}"
echo ""
