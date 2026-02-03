#!/bin/bash
# ========================================
# AgPay+ 服务更新脚本 (Linux/macOS)
# ========================================
# 功能：
# - 支持指定服务更新
# - 自动备份、支持回滚
# - 多环境支持：development/staging/production
# - 健康检查和自动回滚
# ========================================
# 使用方法：
# ./update.sh                              # 更新所有服务（生产环境）
# ./update.sh --env development            # 更新所有服务（开发环境）
# ./update.sh --services agpay-manager-api       # 仅更新 agpay-manager-api
# ./update.sh --services "agpay-manager-api agpay-agent-api"  # 更新多个服务
# ./update.sh --build-cashier              # 强制构建 cashier
# ========================================

set -e

# ========================================
# 默认配置
# ========================================
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENVIRONMENT="production"
SERVICES=""
BUILD_CASHIER="false"
FORCE_UPDATE=false

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BLUE='\033[0;34m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# 备份目录
BACKUP_DIR="${SCRIPT_DIR}/.backup"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_PATH="${BACKUP_DIR}/${ENVIRONMENT}_update_${TIMESTAMP}"

# 检测 Docker Compose 命令
DOCKER_COMPOSE=""
if command -v docker &> /dev/null && docker compose version &> /dev/null 2>&1; then
    DOCKER_COMPOSE="docker compose"
elif command -v docker-compose &> /dev/null; then
    DOCKER_COMPOSE="docker-compose"
fi

# ========================================
# 帮助信息
# ========================================
show_help() {
    cat << EOF
${CYAN}========================================
  AgPay+ 服务更新脚本
========================================${NC}

${GREEN}功能：${NC}
  • 指定服务更新
  • 自动备份、支持回滚
  • 多环境支持
  • 健康检查和自动回滚

${GREEN}使用方法：${NC}
  $0 [选项]

${GREEN}选项：${NC}
  ${YELLOW}--env <环境>${NC}              指定环境：development, staging, production (默认: production)
  ${YELLOW}--services <服务列表>${NC}     指定要更新的服务（用引号包含多个服务，空格分隔）
                              可选值：agpay-ui-manager, agpay-ui-agent, agpay-ui-merchant,
                                     agpay-manager-api, agpay-agent-api, agpay-merchant-api, agpay-payment-api
  ${YELLOW}--build-cashier${NC}          强制构建 cashier（仅影响 agpay-payment-api）
  ${YELLOW}--force${NC}                  强制更新（跳过确认）
  ${YELLOW}--help${NC}                   显示此帮助信息

${GREEN}示例：${NC}
  ${GRAY}# 更新所有服务${NC}
  $0

  ${GRAY}# 更新 agpay-manager-api${NC}
  $0 --services agpay-manager-api

  ${GRAY}# 更新多个服务${NC}
  $0 --services "agpay-manager-api agpay-agent-api"

  ${GRAY}# 更新 agpay-payment-api 并重新构建 cashier${NC}
  $0 --services agpay-payment-api --build-cashier

  ${GRAY}# 开发环境更新${NC}
  $0 --env development --services "agpay-ui-manager agpay-ui-agent"

${GREEN}回滚：${NC}
  如果更新失败，脚本会自动回滚
  手动回滚：./rollback.sh --env ${ENVIRONMENT}

EOF
}

# ========================================
# .env 文件解析函数
# ========================================
get_env_value() {
    local key="$1"
    local env_file="${2:-$SCRIPT_DIR/.env}"
    
    if [ ! -f "$env_file" ]; then
        echo ""
        return 1
    fi
    
    local value=$(grep -E "^\s*${key}\s*=" "$env_file" | \
        head -n 1 | \
        sed -e 's/^[[:space:]]*//' \
            -e 's/[[:space:]]*$//' \
            -e "s/^${key}=//" \
            -e 's/^["'\'']//' \
            -e 's/["'\'']*$//' \
            -e 's/#.*//')
    
    value="${value/#\~/$HOME}"
    echo "$value"
}

# ========================================
# 解析命令行参数
# ========================================
while [[ $# -gt 0 ]]; do
    case $1 in
        --env)
            ENVIRONMENT="$2"
            shift 2
            ;;
        --services)
            SERVICES="$2"
            shift 2
            ;;
        --build-cashier)
            BUILD_CASHIER="true"
            shift
            ;;
        --force)
            FORCE_UPDATE=true
            shift
            ;;
        --help|-h)
            show_help
            exit 0
            ;;
        *)
            echo -e "${RED}未知参数: $1${NC}"
            echo "使用 --help 查看帮助"
            exit 1
            ;;
    esac
done

# ========================================
# 环境验证
# ========================================
ENV_FILE="$SCRIPT_DIR/.env.$ENVIRONMENT"
if [ ! -f "$ENV_FILE" ]; then
    echo -e "${RED}❌ 环境配置文件不存在: $ENV_FILE${NC}"
    echo -e "${YELLOW}可用环境: development, staging, production${NC}"
    exit 1
fi

# 复制环境配置到 .env
cp "$ENV_FILE" "$SCRIPT_DIR/.env"

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  AgPay+ 服务更新脚本${NC}"
echo -e "${CYAN}========================================${NC}"
echo -e "${BLUE}环境:${NC} $ENVIRONMENT"
echo -e "${BLUE}配置文件:${NC} $ENV_FILE"
if [ -n "$SERVICES" ]; then
    echo -e "${BLUE}服务:${NC} $SERVICES"
else
    echo -e "${BLUE}服务:${NC} 所有服务"
fi
echo -e "${CYAN}========================================${NC}"
echo ""

# ========================================
# 检查 Docker 环境
# ========================================
echo -e "${YELLOW}[1/7] 检查 Docker 环境...${NC}"
if ! command -v docker &> /dev/null; then
    echo -e "${RED}  ❌ Docker 未安装${NC}"
    exit 1
fi

if ! docker version &> /dev/null; then
    echo -e "${RED}  ❌ Docker 未运行${NC}"
    exit 1
fi

if [ -z "$DOCKER_COMPOSE" ]; then
    echo -e "${RED}  ❌ Docker Compose 未安装${NC}"
    exit 1
fi

echo -e "${GREEN}  ✅ Docker 环境正常${NC}"

# ========================================
# 检查现有部署
# ========================================
echo ""
echo -e "${YELLOW}[2/7] 检查现有部署...${NC}"

PROJECT_NAME=$(get_env_value "COMPOSE_PROJECT_NAME")
EXISTING_CONTAINERS=$($DOCKER_COMPOSE ps -q 2>/dev/null | wc -l)

if [ "$EXISTING_CONTAINERS" -eq 0 ]; then
    echo -e "${RED}  ❌ 未检测到运行中的服务${NC}"
    echo -e "${YELLOW}  请先执行部署: ./deploy.sh --env $ENVIRONMENT${NC}"
    exit 1
fi

echo -e "${GREEN}  ✅ 检测到运行中的服务${NC}"
echo -e "${GRAY}  运行中的容器数: $EXISTING_CONTAINERS${NC}"

# ========================================
# 备份当前部署
# ========================================
echo ""
echo -e "${YELLOW}[3/7] 备份当前部署...${NC}"

mkdir -p "$BACKUP_PATH"

# 保存当前运行的镜像信息
echo -e "${GRAY}  保存容器和镜像信息...${NC}"
$DOCKER_COMPOSE ps --format json > "$BACKUP_PATH/containers.json" 2>/dev/null || true
$DOCKER_COMPOSE images --format json > "$BACKUP_PATH/images.json" 2>/dev/null || true

# 保存镜像
echo -e "${GRAY}  导出镜像...${NC}"
IMAGE_PREFIX=$(get_env_value "IMAGE_PREFIX")
IMAGE_TAG=$(get_env_value "IMAGE_TAG")

if [ -n "$SERVICES" ]; then
    # 仅备份指定服务
    for service in $SERVICES; do
        image="${IMAGE_PREFIX}-${service}:${IMAGE_TAG}"
        if docker images -q "$image" &> /dev/null; then
            echo -e "${GRAY}    备份: $service${NC}"
            docker save "$image" | gzip > "$BACKUP_PATH/${service}.tar.gz"
        fi
    done
else
    # 备份所有服务
    $DOCKER_COMPOSE images --format "{{.Repository}}:{{.Tag}}" | while read image; do
        service_name=$(echo "$image" | sed "s/${IMAGE_PREFIX}-//;s/:${IMAGE_TAG}//")
        echo -e "${GRAY}    备份: $service_name${NC}"
        docker save "$image" | gzip > "$BACKUP_PATH/${service_name}.tar.gz"
    done
fi

# 保存环境配置
cp "$SCRIPT_DIR/.env" "$BACKUP_PATH/.env.backup"
cp "$SCRIPT_DIR/docker-compose.yml" "$BACKUP_PATH/docker-compose.yml.backup"

echo "$TIMESTAMP" > "$BACKUP_DIR/latest_${ENVIRONMENT}"
echo -e "${GREEN}  ✅ 备份完成: $BACKUP_PATH${NC}"

# 清理旧备份（保留最近 5 个）
BACKUP_COUNT=$(ls -1 "$BACKUP_DIR" | grep -E "^${ENVIRONMENT}_update_" | wc -l)
if [ "$BACKUP_COUNT" -gt 5 ]; then
    echo -e "${GRAY}  清理旧备份...${NC}"
    ls -1t "$BACKUP_DIR" | grep -E "^${ENVIRONMENT}_update_" | tail -n +6 | xargs -I {} rm -rf "$BACKUP_DIR/{}"
fi

# ========================================
# 构建参数准备
# ========================================
echo ""
echo -e "${YELLOW}[4/7] 准备构建参数...${NC}"

BUILD_ARGS=""
if [ "$BUILD_CASHIER" = "true" ]; then
    BUILD_ARGS="--build-arg BUILD_CASHIER=true"
    echo -e "${CYAN}  ℹ️  将构建 cashier${NC}"
else
    BUILD_CASHIER_ENV=$(get_env_value "BUILD_CASHIER")
    if [ "$BUILD_CASHIER_ENV" = "true" ]; then
        BUILD_ARGS="--build-arg BUILD_CASHIER=true"
        echo -e "${CYAN}  ℹ️  根据环境配置构建 cashier${NC}"
    else
        echo -e "${GRAY}  ℹ️  使用现有 cashier${NC}"
    fi
fi

# ========================================
# 构建新镜像
# ========================================
echo ""
echo -e "${YELLOW}[5/7] 构建新镜像...${NC}"

if [ -n "$SERVICES" ]; then
    echo -e "${CYAN}  构建服务: $SERVICES${NC}"
    $DOCKER_COMPOSE build $BUILD_ARGS $SERVICES
else
    echo -e "${CYAN}  构建所有服务${NC}"
    $DOCKER_COMPOSE build $BUILD_ARGS
fi

if [ $? -ne 0 ]; then
    echo -e "${RED}  ❌ 构建失败${NC}"
    exit 1
fi

echo -e "${GREEN}  ✅ 构建完成${NC}"

# ========================================
# 更新确认
# ========================================
if [ "$FORCE_UPDATE" = false ]; then
    echo ""
    echo -e "${YELLOW}========================================${NC}"
    echo -e "${YELLOW}  准备更新${NC}"
    echo -e "${YELLOW}========================================${NC}"
    echo -e "环境: ${CYAN}$ENVIRONMENT${NC}"
    echo -e "项目: ${CYAN}$PROJECT_NAME${NC}"
    if [ -n "$SERVICES" ]; then
        echo -e "服务: ${CYAN}$SERVICES${NC}"
    else
        echo -e "服务: ${CYAN}所有服务${NC}"
    fi
    echo -e "${YELLOW}========================================${NC}"
    echo ""
    read -p "确认更新？[y/N] " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo -e "${RED}更新已取消${NC}"
        exit 0
    fi
fi

# ========================================
# 更新服务
# ========================================
echo ""
echo -e "${YELLOW}[6/7] 更新服务...${NC}"

if [ -n "$SERVICES" ]; then
    for service in $SERVICES; do
        echo -e "${CYAN}  更新 $service...${NC}"
        
        # 停止旧服务
        $DOCKER_COMPOSE stop "$service"
        
        # 删除旧容器
        $DOCKER_COMPOSE rm -f "$service"
        
        # 启动新服务
        $DOCKER_COMPOSE up -d "$service"
        
        if [ $? -eq 0 ]; then
            echo -e "${GREEN}    ✅ $service 更新成功${NC}"
        else
            echo -e "${RED}    ❌ $service 更新失败${NC}"
            # 继续更新其他服务，稍后统一检查
        fi
    done
else
    echo -e "${CYAN}  更新所有服务...${NC}"
    $DOCKER_COMPOSE up -d
fi

# ========================================
# 健康检查
# ========================================
echo ""
echo -e "${YELLOW}[7/7] 健康检查...${NC}"

echo -e "${GRAY}  等待服务启动...${NC}"
sleep 10

if [ -n "$SERVICES" ]; then
    check_services=$SERVICES
else
    check_services=$($DOCKER_COMPOSE ps --services)
fi

failed_services=""
for service in $check_services; do
    status=$($DOCKER_COMPOSE ps "$service" --format "{{.State}}" 2>/dev/null || echo "unknown")
    
    if [ "$status" = "running" ]; then
        echo -e "${GREEN}  ✅ $service: $status${NC}"
    else
        echo -e "${RED}  ❌ $service: $status${NC}"
        failed_services="$failed_services $service"
        
        # 显示失败的服务日志
        echo -e "${GRAY}    最近日志:${NC}"
        $DOCKER_COMPOSE logs --tail=20 "$service" 2>&1 | sed 's/^/      /'
    fi
done

# ========================================
# 回滚逻辑
# ========================================
if [ -n "$failed_services" ]; then
    echo ""
    echo -e "${RED}========================================${NC}"
    echo -e "${RED}  更新失败${NC}"
    echo -e "${RED}========================================${NC}"
    echo -e "${RED}失败的服务:$failed_services${NC}"
    echo ""
    
    echo -e "${YELLOW}开始自动回滚...${NC}"
    
    if [ -f "$SCRIPT_DIR/rollback.sh" ]; then
        bash "$SCRIPT_DIR/rollback.sh" --env "$ENVIRONMENT" --auto --services "$failed_services"
    else
        echo -e "${RED}  ❌ 找不到回滚脚本${NC}"
        echo -e "${YELLOW}  请手动回滚: ./rollback.sh --env $ENVIRONMENT${NC}"
    fi
    
    exit 1
fi

# ========================================
# 更新成功
# ========================================
echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}  🎉 更新成功！${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "${CYAN}环境信息：${NC}"
echo -e "  环境: ${YELLOW}$ENVIRONMENT${NC}"
echo -e "  项目: ${YELLOW}$PROJECT_NAME${NC}"
if [ -n "$SERVICES" ]; then
    echo -e "  更新的服务: ${YELLOW}$SERVICES${NC}"
fi
echo ""

# 读取访问地址
IPORDOMAIN=$(get_env_value "IPORDOMAIN")
echo -e "${CYAN}访问地址：${NC}"
echo -e "  运营平台: ${BLUE}https://${IPORDOMAIN}:8817${NC}"
echo -e "  代理商系统: ${BLUE}https://${IPORDOMAIN}:8816${NC}"
echo -e "  商户系统: ${BLUE}https://${IPORDOMAIN}:8818${NC}"
echo -e "  支付网关: ${BLUE}https://${IPORDOMAIN}:9819${NC}"
echo ""

echo -e "${CYAN}常用命令：${NC}"
echo -e "  查看状态: ${GRAY}$DOCKER_COMPOSE ps${NC}"
echo -e "  查看日志: ${GRAY}$DOCKER_COMPOSE logs -f [服务名]${NC}"
echo -e "  回滚版本: ${GRAY}./rollback.sh --env $ENVIRONMENT${NC}"
echo ""
echo -e "${GREEN}========================================${NC}"
