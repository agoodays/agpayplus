#!/bin/bash
# ========================================
# AgPay+ Linux/macOS 更新脚本
# ========================================
# 功能：更新指定服务或全部服务
# 使用：
#   更新全部: ./update-linux.sh
#   更新指定: ./update-linux.sh --services "ui-manager,manager-api"
# ========================================

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SERVICES=""
NO_BUILD=false
FORCE=false

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

if [ -z "$DOCKER_COMPOSE" ]; then
    echo -e "${RED}  ❌ Docker Compose 未安装${NC}"
    echo -e "${GRAY}  请安装 Docker Compose v2 (docker compose) 或 v1 (docker-compose)${NC}"
    exit 1
fi

# 解析参数
while [[ $# -gt 0 ]]; do
    case $1 in
        --services)
            SERVICES="$2"
            shift 2
            ;;
        --no-build)
            NO_BUILD=true
            shift
            ;;
        --force)
            FORCE=true
            shift
            ;;
        *)
            echo -e "${RED}未知参数: $1${NC}"
            echo "使用方法: $0 [--services \"service1,service2\"] [--no-build] [--force]"
            exit 1
            ;;
    esac
done

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  AgPay+ Linux/macOS 更新脚本${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

# 可用的服务列表
ALL_SERVICES=(
    "ui-manager"
    "ui-agent"
    "ui-merchant"
    "manager-api"
    "agent-api"
    "merchant-api"
    "payment-api"
    "seq"
    "redis"
    "rabbitmq"
)

# 解析要更新的服务
SERVICES_TO_UPDATE=()
if [ -z "$SERVICES" ]; then
    echo -e "${YELLOW}[提示] 将更新所有应用服务（不包括 seq 、 redis 和 rabbitmq）${NC}"
    for service in "${ALL_SERVICES[@]}"; do
        if [[ ! "$service" =~ ^(seq|redis|rabbitmq)$ ]]; then
            SERVICES_TO_UPDATE+=("$service")
        fi
    done
else
    IFS=',' read -ra SERVICE_ARRAY <<< "$SERVICES"
    for service in "${SERVICE_ARRAY[@]}"; do
        service=$(echo "$service" | xargs) # trim whitespace
        
        # 验证服务名称
        if [[ ! " ${ALL_SERVICES[@]} " =~ " ${service} " ]]; then
            echo -e "${RED}  ❌ 无效的服务名称: $service${NC}"
            echo -e "\n${CYAN}可用的服务：${NC}"
            printf '%s\n' "${ALL_SERVICES[@]}" | sed 's/^/  - /'
            exit 1
        fi
        SERVICES_TO_UPDATE+=("$service")
    done
fi

echo -e "${CYAN}即将更新以下服务：${NC}"
printf '%s\n' "${SERVICES_TO_UPDATE[@]}" | sed 's/^/  - /'
echo ""

if [ "$FORCE" = false ]; then
    read -p "是否继续? (y/n): " confirm
    if [ "$confirm" != "y" ]; then
        echo -e "${YELLOW}更新已取消${NC}"
        exit 0
    fi
fi

cd "$SCRIPT_DIR"

# 检查 Docker
echo -e "\n${YELLOW}[1/4] 检查 Docker 环境...${NC}"
if ! docker version &> /dev/null; then
    echo -e "${RED}  ❌ Docker 未安装或未运行${NC}"
    exit 1
fi
echo -e "${GREEN}  ✅ Docker 运行正常${NC}"

# 拉取最新代码（如果是 Git 仓库）
echo -e "\n${YELLOW}[2/4] 检查代码更新...${NC}"
if [ -d "$SCRIPT_DIR/.git" ]; then
    read -p "是否拉取最新代码? (y/n): " pull_code
    if [ "$pull_code" = "y" ]; then
        if git pull; then
            echo -e "${GREEN}  ✅ 代码更新完成${NC}"
        else
            echo -e "${YELLOW}  ! 代码拉取失败，继续使用本地代码${NC}"
        fi
    else
        echo -e "${GREEN}  ✅ 跳过代码拉取${NC}"
    fi
else
    echo -e "${GRAY}  ℹ️ 非 Git 仓库，跳过${NC}"
fi

# 重新构建镜像（跳过基础镜像服务：seq、redis、rabbitmq）
BUILD_LIST=()
PULL_LIST=()
for svc in "${SERVICES_TO_UPDATE[@]}"; do
    if [[ "$svc" =~ ^(seq|redis|rabbitmq)$ ]]; then
        PULL_LIST+=("$svc")
    else
        BUILD_LIST+=("$svc")
    fi
done

if [ "$NO_BUILD" = false ]; then
    echo -e "\n${YELLOW}[3/4] 重新构建镜像...${NC}"
    echo -e "${GRAY}  这可能需要几分钟时间...${NC}"
    if [ ${#BUILD_LIST[@]} -gt 0 ]; then
        BUILD_SERVICES=$(IFS=' '; echo "${BUILD_LIST[*]}")
    if $DOCKER_COMPOSE build --no-cache $BUILD_SERVICES; then
            echo -e "${GREEN}  ✅ 镜像构建成功${NC}"
        else
            echo -e "${RED}  ❌ 镜像构建失败${NC}"
            exit 1
        fi
    else
        echo -e "${GRAY}  ℹ️ 没有需要构建的服务${NC}"
    fi
else
    echo -e "\n${YELLOW}[3/4] 跳过镜像构建...${NC}"
    echo -e "${GREEN}  ✅ 将使用现有镜像${NC}"
fi

# 对于基础镜像服务（seq/redis/rabbitmq）执行 pull（如果需要）
if [ ${#PULL_LIST[@]} -gt 0 ]; then
    PULL_SERVICES=$(IFS=' '; echo "${PULL_LIST[*]}")
    echo -e "\n${YELLOW}正在拉取最新镜像: ${PULL_SERVICES}${NC}"
        if $DOCKER_COMPOSE pull $PULL_SERVICES; then
        echo -e "${GREEN}  ✅ 镜像拉取完成${NC}"
    else
        echo -e "${YELLOW}  ! 镜像拉取失败，继续更新（可能使用本地镜像）${NC}"
    fi
fi

# 更新服务
echo -e "\n${YELLOW}[4/4] 更新服务...${NC}"
for service in "${SERVICES_TO_UPDATE[@]}"; do
    echo -e "${GRAY}  正在更新: $service${NC}"
    
    # 停止并删除旧容器
    $DOCKER_COMPOSE stop "$service" &> /dev/null || true
    $DOCKER_COMPOSE rm -f "$service" &> /dev/null || true
    
    # 启动新容器
    if $DOCKER_COMPOSE up -d "$service"; then
        echo -e "${GREEN}  ✅ $service 更新成功${NC}"
    else
        echo -e "${RED}  ❌ $service 更新失败${NC}"
        exit 1
    fi
    
    sleep 2
done

# 显示更新后的状态
echo -e "\n${CYAN}========================================${NC}"
echo -e "${GREEN}  更新完成！${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""
echo -e "${CYAN}服务状态：${NC}"
$DOCKER_COMPOSE ps

echo -e "\n${CYAN}查看服务日志：${NC}"
for service in "${SERVICES_TO_UPDATE[@]}"; do
    echo -e "${GRAY}  $DOCKER_COMPOSE logs -f $service${NC}"
done
echo ""
