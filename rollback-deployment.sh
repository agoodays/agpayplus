#!/bin/bash
# ========================================
# AgPay+ Linux/macOS 回滚脚本
# ========================================
# 功能：回滚到指定备份版本
# 使用：./rollback-deployment.sh [备份路径]
# ========================================

set -e

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
WHITE='\033[1;37m'
NC='\033[0m' # No Color

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

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  AgPay+ 回滚脚本${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

# 检查 Docker Compose
DOCKER_COMPOSE=$(get_docker_compose_command)
if [ -z "$DOCKER_COMPOSE" ]; then
    echo -e "${RED}  ❌ Docker Compose 未安装${NC}"
    exit 1
fi

# 如果没有指定备份路径，列出可用的备份
if [ -z "$BACKUP_PATH" ]; then
    BACKUP_ROOT="$SCRIPT_DIR/.backup"
    if [ ! -d "$BACKUP_ROOT" ]; then
        echo -e "${RED}  ❌ 没有找到备份目录${NC}"
        exit 1
    fi
    
    # 获取所有备份目录，按时间倒序
    mapfile -t BACKUPS < <(find "$BACKUP_ROOT" -maxdepth 1 -type d ! -path "$BACKUP_ROOT" -printf "%T@ %p\n" | sort -rn | cut -d' ' -f2-)
    
    if [ ${#BACKUPS[@]} -eq 0 ]; then
        echo -e "${RED}  ❌ 没有可用的备份${NC}"
        exit 1
    fi
    
    echo -e "${YELLOW}可用的备份：${NC}"
    echo ""
    for i in "${!BACKUPS[@]}"; do
        BACKUP="${BACKUPS[$i]}"
        BACKUP_NAME=$(basename "$BACKUP")
        BACKUP_TIME=$(stat -c %y "$BACKUP" 2>/dev/null || stat -f "%Sm" -t "%Y-%m-%d %H:%M:%S" "$BACKUP" 2>/dev/null)
        echo -e "  ${WHITE}[$((i + 1))]${NC} $BACKUP_TIME - $BACKUP_NAME"
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

echo -e "${YELLOW}准备回滚到: $BACKUP_PATH${NC}"
echo ""

# 确认
if [ "$FORCE" = false ]; then
    read -p "确认要回滚吗? 这将停止当前服务并恢复备份 (y/n): " CONFIRM
    if [ "$CONFIRM" != "y" ]; then
        echo -e "${YELLOW}  回滚已取消${NC}"
        exit 0
    fi
fi

# [1/4] 停止当前服务
echo -e "\n${YELLOW}[1/4] 停止当前服务...${NC}"
cd "$SCRIPT_DIR"
    if $DOCKER_COMPOSE down --remove-orphans &> /dev/null; then
    echo -e "${GREEN}  ✅ 已停止当前服务${NC}"
else
    echo -e "${GRAY}  ! 没有运行中的服务${NC}"
fi
cd - > /dev/null

# [2/4] 恢复配置文件
echo -e "\n${YELLOW}[2/4] 恢复配置文件...${NC}"

    if [ -f "$BACKUP_PATH/docker-compose.yml" ]; then
    cp "$BACKUP_PATH/docker-compose.yml" "$SCRIPT_DIR/docker-compose.yml"
    echo -e "${GREEN}  ✅ 已恢复 docker-compose.yml${NC}"
fi

    if [ -f "$BACKUP_PATH/.env" ]; then
    cp "$BACKUP_PATH/.env" "$SCRIPT_DIR/.env"
    echo -e "${GREEN}  ✅ 已恢复 .env${NC}"
fi

# [3/4] 检查镜像
echo -e "\n${YELLOW}[3/4] 检查镜像...${NC}"
cd "$SCRIPT_DIR"

BUILD_NEEDED=false

# 检查是否所有镜像都存在
SERVICES=$($DOCKER_COMPOSE config --services 2>&1)
for SERVICE in $SERVICES; do
    # 检查镜像是否存在
    if ! docker images --format "{{.Repository}}:{{.Tag}}" | grep -q "$SERVICE"; then
        BUILD_NEEDED=true
        break
    fi
done

    if [ "$BUILD_NEEDED" = true ]; then
    echo -e "${YELLOW}  ! 需要重新构建镜像${NC}"
    if ! $DOCKER_COMPOSE build; then
        echo -e "${RED}  ❌ 镜像构建失败${NC}"
        cd - > /dev/null
        exit 1
    fi
    echo -e "${GREEN}  ✅ 镜像构建成功${NC}"
else
    echo -e "${GREEN}  ✅ 镜像已存在${NC}"
fi

cd - > /dev/null

# [4/4] 启动服务
echo -e "\n${YELLOW}[4/4] 启动服务...${NC}"
cd "$SCRIPT_DIR"
    if ! $DOCKER_COMPOSE up -d; then
    echo -e "${RED}  ❌ 服务启动失败${NC}"
    cd - > /dev/null
    exit 1
fi

# 等待并检查服务状态
sleep 5
STATUS=$($DOCKER_COMPOSE ps --format "{{.Service}}: {{.Status}}")

echo -e "${GREEN}  ✅ 服务启动成功${NC}"
echo ""
echo -e "${CYAN}服务状态：${NC}"
echo "$STATUS" | while IFS= read -r line; do
    echo -e "  $line"
done

cd - > /dev/null

echo -e "\n${GREEN}========================================${NC}"
echo -e "${GREEN}  回滚成功！${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "${CYAN}服务访问地址：${NC}"
echo -e "  运营平台:    https://localhost:8817"
echo -e "  代理商系统:  https://localhost:8816"
echo -e "  商户系统:    https://localhost:8818"
echo -e "  支付网关:    https://localhost:9819"
echo ""
echo -e "${GRAY}查看服务状态：$DOCKER_COMPOSE ps${NC}"
echo -e "${GRAY}查看服务日志：$DOCKER_COMPOSE logs -f [service-name]${NC}"
echo ""
