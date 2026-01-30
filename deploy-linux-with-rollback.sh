#!/bin/bash
# ========================================
# AgPay+ Linux/macOS 部署脚本（带回滚机制）
# ========================================
# 功能：一键部署所有服务，失败时自动回滚
# 使用：./deploy-linux-with-rollback.sh
# ========================================

set -e

# 命令行参数
SKIP_CERT=false
SKIP_ENV=false
NO_BACKUP=false

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
        --no-backup)
            NO_BACKUP=true
            shift
            ;;
        *)
            echo "未知参数: $1"
            echo "用法: $0 [--skip-cert] [--skip-env] [--no-backup]"
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
NC='\033[0m' # No Color

# 脚本目录
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# 部署状态跟踪
BACKUP_CREATED=false
OLD_CONTAINERS_STOPPED=false
IMAGES_BUILT=false
SERVICES_STARTED=false
BACKUP_PATH=""

# .env 文件解析函数
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
    
    # 展开 ~ 为 $HOME
    value="${value/#\~/$HOME}"
    echo "$value"
}

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

# 创建备份
backup_current_deployment() {
    local docker_compose="$1"
    
    echo -e "\n${CYAN}[备份] 保存当前部署状态...${NC}"
    
    local timestamp=$(date +%Y%m%d_%H%M%S)
    local backup_dir="$SCRIPT_DIR/.backup/$timestamp"
    
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
    
    # 保存当前容器状态
    cd "$SCRIPT_DIR"
    if $docker_compose ps --format json > "$backup_dir/containers.json" 2>/dev/null; then
        :
    fi
    cd - > /dev/null
    
    BACKUP_PATH="$backup_dir"
    BACKUP_CREATED=true
    
    echo -e "${GREEN}  ✓ 备份已保存到: $backup_dir${NC}"
    return 0
}

# 回滚部署
rollback_deployment() {
    local docker_compose="$1"
    local reason="$2"
    
    echo -e "\n${RED}========================================${NC}"
    echo -e "${RED}  部署失败${NC}"
    echo -e "${YELLOW}  原因: $reason${NC}"
    echo -e "${RED}========================================${NC}"
    
    # 检查是否有备份（判断是否为首次部署）
    if [ "$BACKUP_CREATED" != true ]; then
        echo -e "\n${YELLOW}[清理] 这是首次部署，清理失败的资源...${NC}"
        
        cd "$SCRIPT_DIR"
        if $docker_compose down --remove-orphans &> /dev/null; then
            echo -e "${GREEN}  ✓ 已清理失败的容器${NC}"
        fi
        cd - > /dev/null
        
        echo -e "\n${YELLOW}  首次部署失败，请检查错误信息后重试${NC}"
        echo -e "${CYAN}  提示：${NC}"
        echo -e "${GRAY}    1. 检查 .env 配置是否正确${NC}"
        echo -e "${GRAY}    2. 确保网络连接正常（参考 DOCKER_MIRROR_GUIDE.md）${NC}"
        echo -e "${GRAY}    3. 查看错误日志定位问题${NC}"
        return
    fi
    
    # 有备份，执行回滚
    echo -e "\n${YELLOW}  开始回滚...${NC}"
    
    # [1/3] 恢复配置文件
    if [ -n "$BACKUP_PATH" ]; then
        echo -e "\n${YELLOW}[回滚 1/3] 恢复配置文件...${NC}"
        
        # 恢复 docker-compose.yml
        if [ -f "$BACKUP_PATH/docker-compose.yml" ]; then
            cp "$BACKUP_PATH/docker-compose.yml" "$SCRIPT_DIR/docker-compose.yml"
            echo -e "${GREEN}  ✓ 已恢复 docker-compose.yml${NC}"
        fi
        
        # 恢复 .env
        if [ -f "$BACKUP_PATH/.env" ]; then
            cp "$BACKUP_PATH/.env" "$SCRIPT_DIR/.env"
            echo -e "${GREEN}  ✓ 已恢复 .env${NC}"
        fi
    fi
    
    # [2/3] 清理失败的容器
    echo -e "\n${YELLOW}[回滚 2/3] 清理失败的容器...${NC}"
    cd "$SCRIPT_DIR"
    if $docker_compose down --remove-orphans &> /dev/null; then
        echo -e "${GREEN}  ✓ 已清理失败的容器${NC}"
    fi
    cd - > /dev/null
    
    # [3/3] 尝试恢复旧服务
    echo -e "\n${YELLOW}[回滚 3/3] 尝试恢复旧服务...${NC}"
    cd "$SCRIPT_DIR"
    if $docker_compose up -d &> /dev/null; then
        echo -e "${GREEN}  ✓ 已恢复旧服务${NC}"
    else
        echo -e "${YELLOW}  ⚠️ 无法自动恢复旧服务，请手动检查${NC}"
    fi
    cd - > /dev/null
    
    echo -e "\n${RED}========================================${NC}"
    echo -e "${YELLOW}  回滚完成${NC}"
    echo -e "${RED}========================================${NC}"
}

# 清理旧备份
cleanup_old_backups() {
    if [ -d "$SCRIPT_DIR/.backup" ]; then
        # 清理超过7天的备份
        find "$SCRIPT_DIR/.backup" -type d -mtime +7 -exec rm -rf {} + 2>/dev/null || true
    fi
}

# 陷阱函数：捕获错误并回滚
trap_error() {
    local exit_code=$?
    local error_line=$1
    
    if [ $exit_code -ne 0 ]; then
        rollback_deployment "$DOCKER_COMPOSE" "脚本在第 $error_line 行失败 (退出码: $exit_code)"
        exit $exit_code
    fi
}

# ========================================
# 主部署流程
# ========================================

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  AgPay+ Linux/macOS 部署脚本（带回滚）${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

# 检查 Docker Compose
DOCKER_COMPOSE=$(get_docker_compose_command)
if [ -z "$DOCKER_COMPOSE" ]; then
    echo -e "${RED}  ✗ Docker Compose 未安装${NC}"
    echo -e "${GRAY}  请安装 Docker Compose v2 (docker compose) 或 v1 (docker-compose)${NC}"
    exit 1
fi

# 设置错误陷阱
trap 'trap_error $LINENO' ERR

# [1/8] 检查 Docker 环境
echo -e "${YELLOW}[1/8] 检查 Docker 环境...${NC}"
if ! docker version --format '{{.Server.Version}}' &> /dev/null; then
    echo -e "${RED}  ✗ Docker 未安装或未运行${NC}"
    exit 1
fi

DOCKER_VERSION=$(docker version --format '{{.Server.Version}}' 2>&1)
echo -e "${GREEN}  ✓ Docker 版本: $DOCKER_VERSION${NC}"

if [ "$DOCKER_COMPOSE" = "docker compose" ]; then
    COMPOSE_VERSION=$($DOCKER_COMPOSE version --short 2>&1)
else
    COMPOSE_VERSION=$($DOCKER_COMPOSE --version 2>&1 | sed 's/.*version //')
fi
echo -e "${GREEN}  ✓ Docker Compose: $DOCKER_COMPOSE ($COMPOSE_VERSION)${NC}"

# [2/8] 创建备份
if [ "$NO_BACKUP" = false ]; then
    # 检查是否为首次部署
    cd "$SCRIPT_DIR"
    EXISTING_CONTAINERS=$($DOCKER_COMPOSE ps -q 2>&1)
    cd - > /dev/null
    
    if [ -n "$EXISTING_CONTAINERS" ]; then
        backup_current_deployment "$DOCKER_COMPOSE" || {
            echo -e "${YELLOW}  ⚠️ 备份失败，继续部署...${NC}"
        }
    else
        echo -e "\n${YELLOW}[2/8] 跳过备份（首次部署）${NC}"
        BACKUP_CREATED=false
    fi
else
    echo -e "\n${YELLOW}[2/8] 跳过备份（--no-backup）${NC}"
fi

# [3/8] 配置环境变量
echo -e "\n${YELLOW}[3/8] 配置环境变量...${NC}"
if [ "$SKIP_ENV" = false ]; then
    if [ -f "$SCRIPT_DIR/.env" ]; then
        echo -e "${YELLOW}  ! 检测到已存在的 .env 文件${NC}"
        read -p "  是否使用 .env.linux 覆盖? (y/n) " response
        if [ "$response" = "y" ]; then
            cp "$SCRIPT_DIR/.env.linux" "$SCRIPT_DIR/.env"
            echo -e "${GREEN}  ✓ 已复制 .env.linux 到 .env${NC}"
        else
            echo -e "${GREEN}  ✓ 使用现有 .env 配置${NC}"
        fi
    else
        cp "$SCRIPT_DIR/.env.linux" "$SCRIPT_DIR/.env"
        echo -e "${GREEN}  ✓ 已创建 .env 文件${NC}"
    fi
    
    echo ""
    echo -e "${CYAN}  请检查并修改 .env 文件中的配置：${NC}"
    echo -e "${GRAY}    - IPORDOMAIN: 服务器IP或域名${NC}"
    echo -e "${GRAY}    - MYSQL_*: MySQL数据库配置${NC}"
    echo -e "${GRAY}    - DATA_PATH_HOST: 数据存储路径${NC}"
    echo ""
    read -p "  配置完成后按 Enter 继续，或输入 'n' 退出: " continue
    if [ "$continue" = "n" ]; then
        echo -e "${YELLOW}  部署已取消${NC}"
        exit 0
    fi
else
    echo -e "${GREEN}  ✓ 跳过环境变量配置${NC}"
fi

# [4/8] 生成证书
echo -e "\n${YELLOW}[4/8] 配置 SSL 证书...${NC}"
if [ "$SKIP_CERT" = false ]; then
    CERT_PATH="$HOME/.aspnet/https"
    CERT_FILE="$CERT_PATH/agpayplusapi.pfx"
    
    if [ -f "$CERT_FILE" ]; then
        echo -e "${YELLOW}  ! 检测到已存在的证书文件${NC}"
        read -p "  是否重新生成证书? (y/n) " response
        if [ "$response" = "y" ]; then
            echo -e "${GRAY}  正在生成新证书...${NC}"
            bash "$SCRIPT_DIR/generate-cert-linux.sh"
        else
            echo -e "${GREEN}  ✓ 使用现有证书${NC}"
        fi
    else
        echo -e "${GRAY}  正在生成证书...${NC}"
        bash "$SCRIPT_DIR/generate-cert-linux.sh"
    fi
else
    echo -e "${GREEN}  ✓ 跳过证书生成${NC}"
fi

# [5/8] 创建数据目录
echo -e "\n${YELLOW}[5/8] 创建数据目录...${NC}"
DATA_PATH=$(get_env_value "DATA_PATH_HOST")
if [ -z "$DATA_PATH" ]; then
    rollback_deployment "$DOCKER_COMPOSE" ".env 文件中未找到 DATA_PATH_HOST 配置"
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
        # 先尝试不用 sudo
        if mkdir -p "$dir" 2>/dev/null; then
            echo -e "${GREEN}  ✓ 创建目录: $dir${NC}"
        else
            # 失败时使用 sudo
            echo -e "${YELLOW}  需要 sudo 权限创建目录: $dir${NC}"
            sudo mkdir -p "$dir"
            sudo chown -R $(id -u):$(id -g) "$dir"
            echo -e "${GREEN}  ✓ 创建目录: $dir${NC}"
        fi
    else
        if [ -w "$dir" ]; then
            echo -e "${GRAY}  ℹ️ 目录已存在: $dir${NC}"
        else
            echo -e "${YELLOW}  ⚠️ 目录存在但无写权限: $dir${NC}"
            sudo chown -R $(id -u):$(id -g) "$dir" 2>/dev/null || true
        fi
    fi
done

# [6/8] 停止旧容器
echo -e "\n${YELLOW}[6/8] 停止旧容器...${NC}"
cd "$SCRIPT_DIR"
if $DOCKER_COMPOSE down --remove-orphans &> /dev/null; then
    OLD_CONTAINERS_STOPPED=true
    echo -e "${GREEN}  ✓ 已停止旧容器${NC}"
else
    echo -e "${GRAY}  ! 没有需要停止的容器${NC}"
fi
cd - > /dev/null

# [7/8] 构建镜像
echo -e "\n${YELLOW}[7/8] 构建 Docker 镜像...${NC}"
echo -e "${GRAY}  这可能需要几分钟时间，请耐心等待...${NC}"
cd "$SCRIPT_DIR"
if ! $DOCKER_COMPOSE build --no-cache; then
    cd - > /dev/null
    rollback_deployment "$DOCKER_COMPOSE" "镜像构建失败"
    exit 1
fi
IMAGES_BUILT=true
echo -e "${GREEN}  ✓ 镜像构建成功${NC}"
cd - > /dev/null

# [8/8] 启动服务
echo -e "\n${YELLOW}[8/8] 启动服务...${NC}"
cd "$SCRIPT_DIR"
if ! $DOCKER_COMPOSE up -d; then
    cd - > /dev/null
    rollback_deployment "$DOCKER_COMPOSE" "服务启动失败"
    exit 1
fi

# 等待服务就绪
echo -e "${GRAY}  等待服务启动...${NC}"
sleep 10

# 检查服务状态
RUNNING_SERVICES=$($DOCKER_COMPOSE ps --filter "status=running" --format "{{.Service}}" 2>&1 | wc -l)
ALL_SERVICES=$($DOCKER_COMPOSE ps --format "{{.Service}}" 2>&1 | wc -l)

if [ "$RUNNING_SERVICES" -lt "$ALL_SERVICES" ]; then
    cd - > /dev/null
    rollback_deployment "$DOCKER_COMPOSE" "部分服务启动失败"
    exit 1
fi

SERVICES_STARTED=true
echo -e "${GREEN}  ✓ 所有服务启动成功${NC}"
cd - > /dev/null

# 部署成功
echo -e "\n${GREEN}========================================${NC}"
echo -e "${GREEN}  部署成功！${NC}"
echo -e "${GREEN}========================================${NC}"
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

if [ -n "$BACKUP_PATH" ]; then
    echo ""
    echo -e "${GRAY}备份位置：$BACKUP_PATH${NC}"
    echo -e "${GRAY}如需回滚，请运行：./rollback-deployment.sh $BACKUP_PATH${NC}"
fi
echo ""

# 清理旧备份
cleanup_old_backups
