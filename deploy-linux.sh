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

# 检测 Docker Compose 命令
DOCKER_COMPOSE=""
if command -v docker &> /dev/null && docker compose version &> /dev/null 2>&1; then
    DOCKER_COMPOSE="docker compose"
elif command -v docker-compose &> /dev/null; then
    DOCKER_COMPOSE="docker-compose"
fi

# .env 文件解析函数（处理注释、空格、引号）
get_env_value() {
    local key="$1"
    local env_file="${2:-$SCRIPT_DIR/.env}"
    
    if [ ! -f "$env_file" ]; then
        echo ""
        return 1
    fi
    
    # 读取并处理 .env 文件
    local value=$(grep -E "^\s*${key}\s*=" "$env_file" | \
        head -n 1 | \
        sed -e 's/^[[:space:]]*//' \
            -e 's/[[:space:]]*$//' \
            -e "s/^${key}=//" \
            -e 's/^["'\'']//' \
            -e 's/["'\'']*$//' \
            -e 's/#.*//')
    
    # 展开 ~ 为 $HOME
    value="${value/#\~/$HOME}"
    
    echo "$value"
}

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
if [ -z "$DOCKER_COMPOSE" ]; then
    echo -e "${RED}  ❌ Docker Compose 未安装${NC}"
    echo -e "${GRAY}  请安装 Docker Compose v2 (docker compose) 或 v1 (docker-compose)${NC}"
    exit 1
fi

COMPOSE_VERSION=$($DOCKER_COMPOSE version --short 2>/dev/null || $DOCKER_COMPOSE --version | grep -oE '[0-9]+\.[0-9]+\.[0-9]+')
echo -e "${GREEN}  ✅ Docker Compose: $DOCKER_COMPOSE ($COMPOSE_VERSION)${NC}"

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
DATA_PATH=$(get_env_value "DATA_PATH_HOST")

if [ -z "$DATA_PATH" ]; then
    echo -e "${RED}  ❌ .env 文件中未找到 DATA_PATH_HOST 配置${NC}"
    exit 1
fi

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
        # 尝试不用 sudo 创建，失败则提示使用 sudo
        if mkdir -p "$dir" 2>/dev/null; then
            echo -e "${GREEN}  ✅ 创建目录: $dir${NC}"
        else
            echo -e "${YELLOW}  需要 sudo 权限创建目录: $dir${NC}"
            sudo mkdir -p "$dir"
            sudo chown -R $(id -u):$(id -g) "$dir"
            echo -e "${GREEN}  ✅ 创建目录: $dir${NC}"
        fi
    else
        # 检查目录是否可写
        if [ -w "$dir" ]; then
            echo -e "${GRAY}  ℹ️ 目录已存在: $dir${NC}"
        else
            echo -e "${YELLOW}  ⚠️ 目录存在但无写权限: $dir${NC}"
            echo -e "${GRAY}  尝试修正权限...${NC}"
            sudo chown -R $(id -u):$(id -g) "$dir" 2>/dev/null || true
        fi
    fi
done

# 停止并删除旧容器
echo -e "\n${YELLOW}[5/7] 清理旧容器...${NC}"
cd "$SCRIPT_DIR"
if $DOCKER_COMPOSE ps -q &> /dev/null; then
    $DOCKER_COMPOSE down --remove-orphans &> /dev/null || true
    echo -e "${GREEN}  ✅ 已清理旧容器${NC}"
else
    echo -e "${GRAY}  ℹ️ 没有需要清理的容器${NC}"
fi

# 构建镜像
echo -e "\n${YELLOW}[6/7] 构建 Docker 镜像...${NC}"
echo -e "${GRAY}  这可能需要几分钟时间，请耐心等待...${NC}"
cd "$SCRIPT_DIR"
if $DOCKER_COMPOSE build --no-cache; then
    echo -e "${GREEN}  ✅ 镜像构建成功${NC}"
else
    echo -e "${RED}  ❌ 镜像构建失败${NC}"
    exit 1
fi

# 启动服务
echo -e "\n${YELLOW}[7/7] 启动服务...${NC}"
cd "$SCRIPT_DIR"
if $DOCKER_COMPOSE up -d; then
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
echo -e "${GRAY}查看服务状态：$DOCKER_COMPOSE ps${NC}"
echo -e "${GRAY}查看服务日志：$DOCKER_COMPOSE logs -f [service-name]${NC}"
echo -e "${GRAY}停止所有服务：$DOCKER_COMPOSE down${NC}"
echo ""
