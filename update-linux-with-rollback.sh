#!/bin/bash
# ========================================
# AgPay+ Linux/macOS 更新脚本（带回滚机制）
# ========================================
# 功能：更新指定服务，失败时自动回滚
# 使用：./update-linux-with-rollback.sh
# ========================================

set -e

# 命令行参数
SERVICES=""
FORCE=false
NO_BUILD=false
NO_BACKUP=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --services)
            SERVICES="$2"
            shift 2
            ;;
        --force)
            FORCE=true
            shift
            ;;
        --no-build)
            NO_BUILD=true
            shift
            ;;
        --no-backup)
            NO_BACKUP=true
            shift
            ;;
        *)
            echo "未知参数: $1"
            echo "用法: $0 [--services service1,service2] [--force] [--no-build] [--no-backup]"
            exit 1
            ;;
    esac
done

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
NC='\033[0m'

# 脚本目录
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# 更新状态跟踪
BACKUP_CREATED=false
UPDATED_SERVICES=()
FAILED_SERVICES=()
BACKUP_PATH=""
declare -A SERVICE_IMAGES

# 检测 Docker Compose 命令
get_docker_compose_command() {
    local docker_compose=""
    
    if command -v docker &> /dev/null && docker compose version &> /dev/null 2>&1; then
        docker_compose="docker compose"
    elif command -v docker-compose &> /dev/null; then
        docker_compose="docker-compose"
    fi
    
    echo "$docker_compose"
}

# 备份服务状态
backup_service_state() {
    local docker_compose="$1"
    shift
    local services_to_update=("$@")
    
    echo -e "\n${CYAN}[备份] 保存服务状态...${NC}"
    
    local timestamp=$(date +%Y%m%d_%H%M%S)
    local backup_dir="$SCRIPT_DIR/.backup/update_$timestamp"
    
    if ! mkdir -p "$backup_dir" 2>/dev/null; then
        echo -e "${YELLOW}  ⚠️ 备份失败: 无法创建备份目录${NC}"
        return 1
    fi
    
    # 备份 docker-compose.yml
    if [ -f "$SCRIPT_DIR/docker-compose.yml" ]; then
        cp "$SCRIPT_DIR/docker-compose.yml" "$backup_dir/docker-compose.yml"
    fi
    
    # 备份 .env
    if [ -f "$SCRIPT_DIR/.env" ]; then
        cp "$SCRIPT_DIR/.env" "$backup_dir/.env"
    fi
    
    # 保存每个服务的镜像信息
    cd "$SCRIPT_DIR"
    for service in "${services_to_update[@]}"; do
        local image_info=$(docker inspect --format='{{.Config.Image}}' "$service" 2>&1)
        if [ $? -eq 0 ]; then
            SERVICE_IMAGES["$service"]="$image_info"
            
            # 标记当前镜像为 backup
            local backup_tag="${service}:backup_$timestamp"
            docker tag "$image_info" "$backup_tag" 2>&1 | grep -v "^$" || true
            
            echo -e "${GREEN}  ✅ 已备份 $service 镜像: $backup_tag${NC}"
        fi
    done
    cd - > /dev/null
    
    # 保存服务列表
    printf '%s\n' "${services_to_update[@]}" > "$backup_dir/services.txt"
    
    BACKUP_PATH="$backup_dir"
    BACKUP_CREATED=true
    
    echo -e "${GREEN}  ✅ 备份已保存到: $backup_dir${NC}"
    return 0
}

# 回滚服务
rollback_services() {
    local docker_compose="$1"
    local reason="$2"
    
    echo -e "\n${RED}========================================${NC}"
    echo -e "${RED}  更新失败，开始回滚...${NC}"
    echo -e "${YELLOW}  原因: $reason${NC}"
    echo -e "${RED}========================================${NC}"
    
    if [ "$BACKUP_CREATED" = true ] && [ -n "$BACKUP_PATH" ]; then
        echo -e "\n${YELLOW}[回滚 1/2] 恢复服务镜像...${NC}"
        
        cd "$SCRIPT_DIR"
        
        # 停止失败的服务
        for service in "${FAILED_SERVICES[@]}"; do
            echo -e "${GRAY}  停止 $service...${NC}"
            $docker_compose stop "$service" &> /dev/null || true
            $docker_compose rm -f "$service" &> /dev/null || true
        done
        
        # 恢复备份的镜像
        local timestamp=$(basename "$BACKUP_PATH" | grep -oP '\d{8}_\d{6}')
        for service in "${!SERVICE_IMAGES[@]}"; do
            local backup_tag="${service}:backup_$timestamp"
            local original_image="${SERVICE_IMAGES[$service]}"
            
            # 检查备份镜像是否存在
            if docker images --format "{{.Repository}}:{{.Tag}}" | grep -q "$backup_tag"; then
                # 恢复镜像标签
                docker tag "$backup_tag" "$original_image" &> /dev/null || true
                echo -e "${GREEN}  ✅ 已恢复 $service 镜像${NC}"
            fi
        done
        
        cd - > /dev/null
    fi
    
    echo -e "\n${YELLOW}[回滚 2/2] 重启服务...${NC}"
    cd "$SCRIPT_DIR"
    
    # 重启所有受影响的服务
    local all_affected=("${UPDATED_SERVICES[@]}" "${FAILED_SERVICES[@]}")
    local unique_services=($(printf '%s\n' "${all_affected[@]}" | sort -u))
    
    for service in "${unique_services[@]}"; do
        if $docker_compose up -d "$service" &> /dev/null; then
            echo -e "${GREEN}  ✅ 已恢复 $service${NC}"
        else
            echo -e "${YELLOW}  ⚠️ 无法恢复 $service${NC}"
        fi
    done
    
    cd - > /dev/null
    
    echo -e "\n${RED}========================================${NC}"
    echo -e "${YELLOW}  回滚完成${NC}"
    echo -e "${RED}========================================${NC}"
}

# 清理旧备份
cleanup_old_backups() {
    if [ -d "$SCRIPT_DIR/.backup" ]; then
        find "$SCRIPT_DIR/.backup" -type d -name "update_*" -mtime +7 -exec rm -rf {} + 2>/dev/null || true
    fi
}

# ========================================
# 主更新流程
# ========================================

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  AgPay+ Linux/macOS 更新脚本（带回滚）${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

# 检查 Docker Compose
DOCKER_COMPOSE=$(get_docker_compose_command)
if [ -z "$DOCKER_COMPOSE" ]; then
    echo -e "${RED}  ❌ Docker Compose 未安装${NC}"
    exit 1
fi

# 定义服务列表
APPLICATION_SERVICES=("ui-manager" "ui-agent" "ui-merchant" "manager-api" "agent-api" "merchant-api" "payment-api")
INFRASTRUCTURE_SERVICES=("redis" "rabbitmq" "seq")

# 解析要更新的服务
SERVICES_TO_UPDATE=()
if [ -n "$SERVICES" ]; then
    IFS=',' read -ra SERVICES_TO_UPDATE <<< "$SERVICES"
    # 去除空格
    for i in "${!SERVICES_TO_UPDATE[@]}"; do
        SERVICES_TO_UPDATE[$i]=$(echo "${SERVICES_TO_UPDATE[$i]}" | xargs)
    done
else
    # 默认更新所有应用服务
    SERVICES_TO_UPDATE=("${APPLICATION_SERVICES[@]}")
fi

echo -e "${YELLOW}准备更新以下服务：${NC}"
for service in "${SERVICES_TO_UPDATE[@]}"; do
    echo -e "  - $service"
done
echo ""

# 确认
if [ "$FORCE" = false ]; then
    read -p "确认继续? (y/n): " confirm
    if [ "$confirm" != "y" ]; then
        echo -e "${YELLOW}  更新已取消${NC}"
        exit 0
    fi
fi

# 设置错误陷阱
trap_error() {
    local exit_code=$?
    if [ $exit_code -ne 0 ] && [ ${#FAILED_SERVICES[@]} -gt 0 ]; then
        rollback_services "$DOCKER_COMPOSE" "服务更新失败"
        exit $exit_code
    fi
}

trap 'trap_error' ERR

# [1/4] 检查 Docker
echo -e "${YELLOW}[1/4] 检查 Docker 环境...${NC}"
if ! docker version --format '{{.Server.Version}}' &> /dev/null; then
    echo -e "${RED}  ❌ Docker 未安装或未运行${NC}"
    exit 1
fi

DOCKER_VERSION=$(docker version --format '{{.Server.Version}}' 2>&1)
    echo -e "${GREEN}  ✅ Docker 版本: $DOCKER_VERSION${NC}"

# [2/4] 创建备份
if [ "$NO_BACKUP" = false ]; then
    backup_service_state "$DOCKER_COMPOSE" "${SERVICES_TO_UPDATE[@]}" || {
        echo -e "${YELLOW}  ⚠️ 备份失败，继续更新...${NC}"
    }
else
    echo -e "\n${YELLOW}[2/4] 跳过备份（--no-backup）${NC}"
fi

# [3/4] 更新服务
echo -e "\n${YELLOW}[3/4] 更新服务...${NC}"

cd "$SCRIPT_DIR"

for service in "${SERVICES_TO_UPDATE[@]}"; do
    echo -e "\n${CYAN}  更新 $service...${NC}"
    
    # 停止服务
    echo -e "${GRAY}    [1/4] 停止服务...${NC}"
    if ! $DOCKER_COMPOSE stop "$service" &> /dev/null; then
        FAILED_SERVICES+=("$service")
                echo -e "${RED}  ❌ $service 停止失败${NC}"
        rollback_services "$DOCKER_COMPOSE" "$service 停止失败"
        exit 1
    fi
    $DOCKER_COMPOSE rm -f "$service" &> /dev/null || true
    
    # 构建或拉取镜像
    if [ "$NO_BUILD" = false ]; then
        if [[ " ${APPLICATION_SERVICES[@]} " =~ " ${service} " ]]; then
            echo -e "${GRAY}    [2/4] 构建镜像...${NC}"
            if ! $DOCKER_COMPOSE build "$service"; then
                FAILED_SERVICES+=("$service")
                echo -e "${RED}  ❌ $service 镜像构建失败${NC}"
                rollback_services "$DOCKER_COMPOSE" "$service 镜像构建失败"
                exit 1
            fi
        else
            echo -e "${GRAY}    [2/4] 拉取镜像...${NC}"
            if ! $DOCKER_COMPOSE pull "$service"; then
                FAILED_SERVICES+=("$service")
                echo -e "${RED}  ❌ $service 镜像拉取失败${NC}"
                rollback_services "$DOCKER_COMPOSE" "$service 镜像拉取失败"
                exit 1
            fi
        fi
    else
        echo -e "${GRAY}    [2/4] 跳过构建${NC}"
    fi
    
    # 启动服务
    echo -e "${GRAY}    [3/4] 启动服务...${NC}"
        if ! $DOCKER_COMPOSE up -d "$service"; then
        FAILED_SERVICES+=("$service")
        echo -e "${RED}  ❌ $service 启动失败${NC}"
        rollback_services "$DOCKER_COMPOSE" "$service 启动失败"
        exit 1
    fi
    
    # 等待服务就绪
    echo -e "${GRAY}    [4/4] 检查服务状态...${NC}"
    sleep 5
    
    status=$($DOCKER_COMPOSE ps "$service" --format "{{.Status}}" 2>&1)
        if [[ ! "$status" =~ "Up" ]]; then
        FAILED_SERVICES+=("$service")
        echo -e "${RED}  ❌ $service 未正常运行: $status${NC}"
        rollback_services "$DOCKER_COMPOSE" "$service 未正常运行"
        exit 1
    fi
    
    UPDATED_SERVICES+=("$service")
    echo -e "${GREEN}  ✅ $service 更新成功${NC}"
done

cd - > /dev/null

# [4/4] 验证
echo -e "\n${YELLOW}[4/4] 验证更新...${NC}"
cd "$SCRIPT_DIR"

ALL_HEALTHY=true
for service in "${SERVICES_TO_UPDATE[@]}"; do
    status=$($DOCKER_COMPOSE ps "$service" --format "{{.Status}}" 2>&1)
    if [[ "$status" =~ "Up" ]]; then
        echo -e "${GREEN}  ✅ $service 运行正常${NC}"
    else
        echo -e "${RED}  ❌ $service 状态异常: $status${NC}"
        ALL_HEALTHY=false
    fi
done

cd - > /dev/null

if [ "$ALL_HEALTHY" = false ]; then
    rollback_services "$DOCKER_COMPOSE" "部分服务状态异常"
    exit 1
fi

# 更新成功
echo -e "\n${GREEN}========================================${NC}"
echo -e "${GREEN}  更新成功！${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "${CYAN}更新的服务：${NC}"
for service in "${UPDATED_SERVICES[@]}"; do
    echo -e "  ${GREEN}✅${NC} $service"
done
echo ""
echo -e "${GRAY}查看服务状态：$DOCKER_COMPOSE ps${NC}"
echo -e "${GRAY}查看服务日志：$DOCKER_COMPOSE logs -f [service-name]${NC}"

if [ -n "$BACKUP_PATH" ]; then
    echo ""
    echo -e "${GRAY}备份位置：$BACKUP_PATH${NC}"
    echo -e "${GRAY}如需回滚，请运行：./rollback-update.sh $BACKUP_PATH${NC}"
fi
echo ""

# 清理旧备份
cleanup_old_backups
