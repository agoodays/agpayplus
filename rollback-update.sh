#!/bin/bash
# ========================================
# AgPay+ 更新回滚脚本 (Linux/macOS)
# ========================================
# 功能：回滚服务更新
# 使用：./rollback-update.sh [备份路径]
# ========================================

set -e

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
WHITE='\033[1;37m'
NC='\033[0m'

# 脚本目录
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# 命令行参数
BACKUP_PATH=""
FORCE=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --force)
            FORCE=true
            shift
            ;;
        *)
            if [ -z "$BACKUP_PATH" ]; then
                BACKUP_PATH="$1"
            fi
            shift
            ;;
    esac
done

# 检测 Docker Compose
get_docker_compose_command() {
    if command -v docker &> /dev/null && docker compose version &> /dev/null 2>&1; then
        echo "docker compose"
    elif command -v docker-compose &> /dev/null; then
        echo "docker-compose"
    else
        echo ""
    fi
}

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  AgPay+ 更新回滚脚本${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

# 检查 Docker Compose
DOCKER_COMPOSE=$(get_docker_compose_command)
if [ -z "$DOCKER_COMPOSE" ]; then
    echo -e "${RED}  ❌ Docker Compose 未安装${NC}"
    exit 1
fi

# 如果没有指定备份路径，列出可用的更新备份
if [ -z "$BACKUP_PATH" ]; then
    BACKUP_ROOT="$SCRIPT_DIR/.backup"
    if [ ! -d "$BACKUP_ROOT" ]; then
        echo -e "${RED}  ❌ 没有找到备份目录${NC}"
        exit 1
    fi
    
    # 获取所有更新备份目录
    mapfile -t BACKUPS < <(find "$BACKUP_ROOT" -maxdepth 1 -type d -name "update_*" -printf "%T@ %p\n" | sort -rn | cut -d' ' -f2-)
    
    if [ ${#BACKUPS[@]} -eq 0 ]; then
        echo -e "${RED}  ❌ 没有可用的更新备份${NC}"
        exit 1
    fi
    
    echo -e "${YELLOW}可用的更新备份：${NC}"
    echo ""
    for i in "${!BACKUPS[@]}"; do
        BACKUP="${BACKUPS[$i]}"
        BACKUP_NAME=$(basename "$BACKUP")
        BACKUP_TIME=$(stat -c %y "$BACKUP" 2>/dev/null || stat -f "%Sm" -t "%Y-%m-%d %H:%M:%S" "$BACKUP" 2>/dev/null)
        
        # 读取服务列表
        SERVICES_FILE="$BACKUP/services.txt"
        if [ -f "$SERVICES_FILE" ]; then
            SERVICE_LIST=$(cat "$SERVICES_FILE" | tr '\n' ', ' | sed 's/,$//')
        else
            SERVICE_LIST="未知"
        fi
        
        echo -e "  ${WHITE}[$((i + 1))]${NC} $BACKUP_TIME"
        echo -e "      ${GRAY}服务: $SERVICE_LIST${NC}"
        echo -e "      ${GRAY}路径: $BACKUP_NAME${NC}"
    done
    echo ""
    
    read -p "请选择要回滚的备份编号 (1-${#BACKUPS[@]}): " SELECTION
    
    if ! [[ "$SELECTION" =~ ^[0-9]+$ ]] || [ "$SELECTION" -lt 1 ] || [ "$SELECTION" -gt ${#BACKUPS[@]} ]; then
        echo -e "${RED}  ❌ 无效的选择${NC}"
        exit 1
    fi
    
    BACKUP_PATH="${BACKUPS[$((SELECTION - 1))]}"
fi

# 验证备份路径
if [ ! -d "$BACKUP_PATH" ]; then
    echo -e "${RED}  ❌ 备份路径不存在: $BACKUP_PATH${NC}"
    exit 1
fi

# 读取服务列表
SERVICES_FILE="$BACKUP_PATH/services.txt"
if [ ! -f "$SERVICES_FILE" ]; then
    echo -e "${RED}  ❌ 备份损坏: 缺少服务列表文件${NC}"
    exit 1
fi

mapfile -t SERVICES < "$SERVICES_FILE"

echo -e "${YELLOW}准备回滚以下服务: ${SERVICES[*]}${NC}"
echo -e "${GRAY}备份路径: $BACKUP_PATH${NC}"
echo ""

# 确认
if [ "$FORCE" = false ]; then
    read -p "确认要回滚吗? (y/n): " CONFIRM
    if [ "$CONFIRM" != "y" ]; then
        echo -e "${YELLOW}  回滚已取消${NC}"
        exit 0
    fi
fi

# [1/3] 停止当前服务
echo -e "\n${YELLOW}[1/3] 停止当前服务...${NC}"
cd "$SCRIPT_DIR"
for service in "${SERVICES[@]}"; do
    echo -e "${GRAY}  停止 $service...${NC}"
    $DOCKER_COMPOSE stop "$service" &> /dev/null || true
    $DOCKER_COMPOSE rm -f "$service" &> /dev/null || true
done
echo -e "${GREEN}  ✅ 已停止所有服务${NC}"
cd - > /dev/null

# [2/3] 恢复镜像
echo -e "\n${YELLOW}[2/3] 恢复镜像...${NC}"
TIMESTAMP=$(basename "$BACKUP_PATH" | grep -oP '\d{8}_\d{6}')

for service in "${SERVICES[@]}"; do
    BACKUP_TAG="${service}:backup_$TIMESTAMP"
    
    # 检查备份镜像是否存在
    if docker images --format "{{.Repository}}:{{.Tag}}" | grep -q "$BACKUP_TAG"; then
        # 获取当前镜像名
        cd "$SCRIPT_DIR"
        CURRENT_IMAGE=$(docker inspect --format='{{.Config.Image}}' "$service" 2>&1)
        cd - > /dev/null
        
        if [ $? -eq 0 ] && [ -n "$CURRENT_IMAGE" ]; then
            # 恢复镜像标签
            docker tag "$BACKUP_TAG" "$CURRENT_IMAGE" &> /dev/null || true
            echo -e "${GREEN}  ✅ 已恢复 $service 镜像${NC}"
        else
            echo -e "${YELLOW}  ⚠️ 无法获取 $service 当前镜像信息，跳过${NC}"
        fi
    else
        echo -e "${YELLOW}  ⚠️ 找不到 $service 的备份镜像${NC}"
    fi
done

# [3/3] 启动服务
echo -e "\n${YELLOW}[3/3] 启动服务...${NC}"
cd "$SCRIPT_DIR"
for service in "${SERVICES[@]}"; do
    echo -e "${GRAY}  启动 $service...${NC}"
        if $DOCKER_COMPOSE up -d "$service" &> /dev/null; then
        echo -e "${GREEN}  ✅ $service 启动成功${NC}"
    else
        echo -e "${RED}  ❌ $service 启动失败${NC}"
    fi
done
cd - > /dev/null

# 等待服务就绪
sleep 5

# 检查服务状态
echo -e "\n${CYAN}服务状态：${NC}"
cd "$SCRIPT_DIR"
for service in "${SERVICES[@]}"; do
    STATUS=$($DOCKER_COMPOSE ps "$service" --format "{{.Service}}: {{.Status}}" 2>&1)
    echo -e "  $STATUS"
done
cd - > /dev/null

echo -e "\n${GREEN}========================================${NC}"
echo -e "${GREEN}  回滚成功！${NC}"
echo -e "${GREEN}========================================${NC}"
