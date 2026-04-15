#!/bin/bash
# ========================================
# AgPay+ 统一部署脚本 (Linux/macOS)
# ========================================
# 功能：
# - 首次部署：自动环境初始化
# - 更新部署：自动备份、支持回滚
# - 多环境支持：dev/staging/production
# - 指定服务更新：支持单个或多个服务
# - 一键初始化：自动配置环境、生成证书
# - 数据库初始化：自动执行数据库脚本
# - 配置验证：检查配置文件有效性
# ========================================
# 使用方法：
# ./deploy.sh                              # 默认生产环境，部署所有服务
# ./deploy.sh --env dev                    # 开发环境
# ./deploy.sh --env staging                # 预发布环境
# ./deploy.sh --services agpay-manager-api       # 仅更新指定服务
# ./deploy.sh --services "agpay-manager-api agpay-agent-api"  # 更新多个服务
# ./deploy.sh --build-cashier              # 强制构建 cashier
# ./deploy.sh --skip-backup                # 跳过备份（首次部署）
# ./deploy.sh --init                       # 一键初始化（自动配置环境）
# ./deploy.sh --init-db                    # 执行数据库初始化
# ./deploy.sh --validate-config            # 验证配置
# ./deploy.sh --help                       # 查看帮助
# ========================================

set -e

# ========================================
# 默认配置
# ========================================
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENVIRONMENT="production"
SERVICES=""
BUILD_CASHIER=""
SKIP_BACKUP=false
SKIP_CERT=false
FORCE_DEPLOY=false

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BLUE='\033[0;34m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# ========================================
# 配置验证函数
# ========================================
validate_config() {
    local env_file="${1:-$SCRIPT_DIR/.env}"
    
    echo -e "${CYAN}========================================${NC}"
    echo -e "${CYAN}  配置验证${NC}"
    echo -e "${CYAN}========================================${NC}"
    
    if [ ! -f "$env_file" ]; then
        echo -e "${RED}  ❌ 配置文件不存在: $env_file${NC}"
        return 1
    fi
    
    local configs=("IPORDOMAIN:服务器 IP 或域名:1"
                 "MYSQL_SERVER_NAME:MySQL 服务器地址:1"
                 "MYSQL_PASSWORD:MySQL 密码:1"
                 "DATA_PATH_HOST:数据存储路径:1"
                 "CERT_PATH:SSL 证书路径:1"
                 "COMPOSE_PROJECT_NAME:项目名称:1"
                 "IMAGE_PREFIX:镜像前缀:1")
    
    local valid=1
    
    for config in "${configs[@]}"; do
        IFS=":" read -r name desc required <<< "$config"
        local value=$(get_env_value "$name" "$env_file")
        
        if [ -z "$value" ]; then
            if [ "$required" -eq 1 ]; then
                echo -e "${RED}  ❌ 缺少配置: $name ($desc)${NC}"
                valid=0
            else
                echo -e "${YELLOW}  ⚠️  可选配置未设置: $name ($desc)${NC}"
            fi
        else
            echo -e "${GREEN}  ✅ $name: $value${NC}"
        fi
    done
    
    # 验证数据路径
    local data_path=$(get_env_value "DATA_PATH_HOST" "$env_file")
    if [ -n "$data_path" ]; then
        if [ ! -d "$data_path" ]; then
            echo -e "${YELLOW}  ⚠️  数据路径不存在，将在部署时创建: $data_path${NC}"
        else
            echo -e "${GREEN}  ✅ 数据路径: $data_path${NC}"
        fi
    fi
    
    # 验证证书路径
    local cert_path=$(get_env_value "CERT_PATH" "$env_file")
    if [ -n "$cert_path" ]; then
        if [ ! -d "$cert_path" ]; then
            echo -e "${YELLOW}  ⚠️  证书路径不存在，将在部署时创建: $cert_path${NC}"
        else
            echo -e "${GREEN}  ✅ 证书路径: $cert_path${NC}"
        fi
    fi
    
    if [ "$valid" -eq 1 ]; then
        echo -e "${GREEN}  ✅ 配置验证通过${NC}"
    else
        echo -e "${RED}  ❌ 配置验证失败${NC}"
    fi
    
    return $valid
}

# ========================================
# 一键初始化函数
# ========================================
initialize_environment() {
    echo -e "${CYAN}========================================${NC}"
    echo -e "${CYAN}  一键初始化环境${NC}"
    echo -e "${CYAN}========================================${NC}"
    
    # 检测当前环境
    echo -e "${GRAY}  操作系统: $(uname -a)${NC}"
    
    # 检查 Docker
    if ! command -v docker &> /dev/null; then
        echo -e "${RED}  ❌ Docker 未安装，请先安装 Docker${NC}"
        return 1
    fi
    
    # 检查 Docker Compose
    if [ -z "$DOCKER_COMPOSE" ]; then
        echo -e "${RED}  ❌ Docker Compose 未安装${NC}"
        return 1
    fi
    
    # 自动配置环境文件
    local env_files=("development" "staging" "production")
    for env in "${env_files[@]}"; do
        local env_file="$SCRIPT_DIR/.env.$env"
        if [ ! -f "$env_file" ]; then
            echo -e "${YELLOW}  ⚠️  环境配置文件不存在: $env_file${NC}"
            echo -e "${GRAY}  正在创建默认配置文件...${NC}"
            
            # 创建默认配置
            cat > "$env_file" << EOF
# ========================================
# AgPay+ $env 环境配置
# ========================================

# Docker Compose 配置
COMPOSE_PROJECT_NAME=agpayplus-$env
IMAGE_PREFIX=agpay-$env
IMAGE_TAG=latest

# 服务器配置
IPORDOMAIN=localhost

# MySQL 数据库配置
MYSQL_SERVER_NAME=172.17.0.1
MYSQL_PORT=3306
MYSQL_USER=root
MYSQL_PASSWORD=123456
MYSQL_DATABASE=agpayplusdb

# SSL 证书配置
CERT_PASSWORD=123456
CERT_PATH=~/.aspnet/https
CERT_PATH_IN_CONTAINER=/https/agpayplusapi.pfx

# 消息队列配置
MQ_VENDER=RabbitMQ
MQ_HOSTNAME=rabbitmq
MQ_USERNAME=admin
MQ_PASSWORD=admin
MQ_PORT=5672

# 日志配置
SEQ_URL=http://seq:80
ENABLE_SEQ=true
SEQ_API_KEY=

# 数据持久化路径
DATA_PATH_HOST=/opt/agpayplus/$env

# 构建选项
BUILD_CASHIER=false

# Redis 配置
REDIS_HOST=redis
REDIS_PORT=6379
REDIS_PASSWORD=
REDIS_DB=0

# 健康检查配置
HEALTH_CHECK_ENABLED=true
HEALTH_CHECK_INTERVAL=30s
HEALTH_CHECK_TIMEOUT=10s
HEALTH_CHECK_RETRIES=3

# 备份配置
BACKUP_ENABLED=true
BACKUP_RETENTION=5
BACKUP_PATH=/opt/agpayplus/$env/backup
EOF
            
            echo -e "${GREEN}  ✅ 创建配置文件: $env_file${NC}"
        fi
    done
    
    # 生成证书
    echo -e "${GRAY}  生成 SSL 证书...${NC}"
    local cert_script="$SCRIPT_DIR/generate-cert-linux.sh"
    if [ -f "$cert_script" ]; then
        bash "$cert_script"
    else
        echo -e "${YELLOW}  ⚠️  找不到证书生成脚本${NC}"
    fi
    
    # 创建数据目录
    local data_path="/opt/agpayplus/$ENVIRONMENT"
    if [ ! -d "$data_path" ]; then
        echo -e "${GRAY}  创建数据目录: $data_path${NC}"
        mkdir -p "$data_path"/{logs,upload,mysql,redis,rabbitmq,seq}
    fi
    
    echo -e "${GREEN}  ✅ 环境初始化完成${NC}"
    return 0
}

# ========================================
# 数据库初始化函数
# ========================================
initialize_database() {
    local env_file="${1:-$SCRIPT_DIR/.env}"
    
    echo -e "${CYAN}========================================${NC}"
    echo -e "${CYAN}  数据库初始化${NC}"
    echo -e "${CYAN}========================================${NC}"
    
    local mysql_server=$(get_env_value "MYSQL_SERVER_NAME" "$env_file")
    local mysql_port=$(get_env_value "MYSQL_PORT" "$env_file")
    local mysql_user=$(get_env_value "MYSQL_USER" "$env_file")
    local mysql_password=$(get_env_value "MYSQL_PASSWORD" "$env_file")
    local mysql_database=$(get_env_value "MYSQL_DATABASE" "$env_file")
    
    echo -e "${GRAY}  数据库连接: $mysql_server:$mysql_port${NC}"
    echo -e "${GRAY}  数据库: $mysql_database${NC}"
    
    # 检查 MySQL 是否可访问
    if ! command -v mysql &> /dev/null; then
        echo -e "${YELLOW}  ⚠️  MySQL 客户端未安装，跳过数据库初始化${NC}"
        return 1
    fi
    
    # 检查数据库是否存在
    local test_cmd="mysql -h $mysql_server -P $mysql_port -u $mysql_user -p$mysql_password -e 'SHOW DATABASES;'"
    local result=$($test_cmd 2>&1)
    
    if echo "$result" | grep -q "ERROR"; then
        echo -e "${RED}  ❌ MySQL 连接失败: $result${NC}"
        return 1
    fi
    
    # 执行初始化 SQL 脚本
    local sql_dir="$SCRIPT_DIR/aspnet-core/docs/sql"
    if [ -d "$sql_dir" ]; then
        local sql_files=("agpayplusinit.sql" "plusinit.sql")
        for file in "${sql_files[@]}"; do
            local sql_script="$sql_dir/$file"
            if [ -f "$sql_script" ]; then
                echo -e "${GRAY}  执行 SQL 脚本: $file${NC}"
                local init_cmd="mysql -h $mysql_server -P $mysql_port -u $mysql_user -p$mysql_password $mysql_database < '$sql_script'"
                local init_result=$($init_cmd 2>&1)
                
                if echo "$init_result" | grep -q "ERROR"; then
                    echo -e "${YELLOW}  ⚠️  SQL 脚本执行可能存在错误: $init_result${NC}"
                else
                    echo -e "${GREEN}  ✅ SQL 脚本执行成功: $file${NC}"
                fi
            fi
        done
    else
        echo -e "${YELLOW}  ⚠️  SQL 脚本目录不存在: $sql_dir${NC}"
    fi
    
    return 0
}

# ========================================
# 帮助信息
# ========================================
show_help() {
    cat << EOF
${CYAN}========================================
  AgPay+ 统一部署脚本 (Linux/macOS)
========================================${NC}

${GREEN}功能：${NC}
  • 首次部署：自动环境初始化
  • 更新部署：自动备份、支持回滚
  • 多环境支持：development/staging/production
  • 指定服务更新
  • 一键初始化：自动配置环境、生成证书
  • 数据库初始化：自动执行数据库脚本
  • 配置验证：检查配置文件有效性

${GREEN}使用方法：${NC}
  $0 [选项]

${GREEN}选项：${NC}
  ${YELLOW}-e, --env <环境>${NC}           指定环境 (development/staging/production)
                               默认: production
  ${YELLOW}-s, --services <服务>${NC}     指定要部署的服务（空格分隔）
                               可选值：agpay-ui-manager, agpay-ui-agent, agpay-ui-merchant,
                                     agpay-manager-api, agpay-agent-api, agpay-merchant-api, agpay-payment-api
                               示例: "agpay-manager-api agpay-agent-api"
  ${YELLOW}-b, --build-cashier${NC}       强制构建 cashier（收银台）
  ${YELLOW}--skip-backup${NC}             跳过备份（首次部署时使用）
  ${YELLOW}--skip-cert${NC}               跳过 SSL 证书生成
  ${YELLOW}-f, --force${NC}               强制部署，跳过确认提示
  ${YELLOW}--init${NC}                    一键初始化（自动配置环境）
  ${YELLOW}--init-db${NC}                 执行数据库初始化
  ${YELLOW}--validate-config${NC}         验证配置文件
  ${YELLOW}--verbose${NC}                 详细输出
  ${YELLOW}-h, --help${NC}                显示此帮助信息

${GREEN}示例：${NC}
  ${GRAY}# 首次生产环境部署（跳过备份）${NC}
  $0 --env production --skip-backup

  ${GRAY}# 开发环境部署并构建 cashier${NC}
  $0 --env development --build-cashier

  ${GRAY}# 仅部署指定服务${NC}
  $0 --services agpay-manager-api

  ${GRAY}# 部署多个服务${NC}
  $0 --services "agpay-ui-manager agpay-manager-api"

  ${GRAY}# 强制部署（无确认提示）${NC}
  $0 --force

${CYAN}环境说明：${NC}
  ${GRAY}• development  - 开发环境（配置文件: .env.development）${NC}
  ${GRAY}• staging      - 预发布环境（配置文件: .env.staging）${NC}
  ${GRAY}• production   - 生产环境（配置文件: .env.production）${NC}

${CYAN}注意事项：${NC}
  ${GRAY}• 首次部署请使用 --skip-backup 参数${NC}
  ${GRAY}• 确保 Docker 和 Docker Compose 已安装${NC}
  ${GRAY}• 部署前请检查 .env 配置文件${NC}

EOF
}

# 检测 Docker Compose 命令
DOCKER_COMPOSE=""
if command -v docker &> /dev/null && docker compose version &> /dev/null 2>&1; then
    DOCKER_COMPOSE="docker compose"
elif command -v docker-compose &> /dev/null; then
    DOCKER_COMPOSE="docker-compose"
fi

# ========================================
# 执行 Docker Compose 命令
# ========================================
exec_docker_compose() {
    local args=($@)
    echo -e "${GRAY}  执行: $DOCKER_COMPOSE ${args[*]}${NC}"
    $DOCKER_COMPOSE "${args[@]}"
    local exit_code=$?
    if [ $exit_code -ne 0 ]; then
        echo -e "${RED}  ❌ Docker Compose 执行失败，退出码: $exit_code${NC}"
        return $exit_code
    fi
    return 0
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
        --env|-e)
            ENVIRONMENT="$2"
            shift 2
            ;;
        --services|-s)
            SERVICES="$2"
            shift 2
            ;;
        --build-cashier|-b)
            BUILD_CASHIER="true"
            shift
            ;;
        --skip-backup)
            SKIP_BACKUP=true
            shift
            ;;
        --skip-cert)
            SKIP_CERT=true
            shift
            ;;
        --force|-f)
            FORCE_DEPLOY=true
            shift
            ;;
        --init)
            INIT=true
            shift
            ;;
        --init-db)
            INIT_DB=true
            shift
            ;;
        --validate-config)
            VALIDATE_CONFIG=true
            shift
            ;;
        --verbose)
            VERBOSE=true
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

# 一键初始化
if [ "$INIT" = true ]; then
    initialize_environment
    if [ $? -eq 0 ]; then
        # 如果请求，执行数据库初始化
        if [ "$INIT_DB" = true ]; then
            initialize_database
            if [ $? -eq 0 ]; then
                echo -e "${GREEN}  ✅ 数据库初始化成功${NC}"
            else
                echo -e "${RED}  ❌ 数据库初始化失败${NC}"
            fi
        fi
        echo -e "${GREEN}  ✅ 一键初始化成功完成${NC}"
    else
        echo -e "${RED}  ❌ 环境初始化失败${NC}"
    fi
    exit 0
fi

# 配置验证
if [ "$VALIDATE_CONFIG" = true ]; then
    validate_config
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}  ✅ 配置验证通过${NC}"
    else
        echo -e "${RED}  ❌ 配置验证失败${NC}"
    fi
    exit 0
fi

# 数据库初始化
if [ "$INIT_DB" = true ]; then
    initialize_database
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}  ✅ 数据库初始化完成${NC}"
    else
        echo -e "${RED}  ❌ 数据库初始化失败${NC}"
    fi
    exit 0
fi

ENV_FILE="$SCRIPT_DIR/.env.$ENVIRONMENT"
if [ ! -f "$ENV_FILE" ]; then
    echo -e "${RED}❌ 环境配置文件不存在: $ENV_FILE${NC}"
    echo -e "${YELLOW}可用环境: development, staging, production${NC}"
    exit 1
fi

# 复制环境配置到 .env
cp "$ENV_FILE" "$SCRIPT_DIR/.env"

# 验证配置
echo -e "${GRAY}  验证配置文件...${NC}"
validate_config

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  AgPay+ 统一部署脚本${NC}"
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
echo -e "${YELLOW}[1/9] 检查 Docker 环境...${NC}"
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

if [ -z "$DOCKER_COMPOSE" ]; then
    echo -e "${RED}  ❌ Docker Compose 未安装${NC}"
    exit 1
fi

COMPOSE_VERSION=$($DOCKER_COMPOSE version --short 2>/dev/null || echo "unknown")
echo -e "${GREEN}  ✅ Docker Compose 版本: $COMPOSE_VERSION${NC}"

# ========================================
# 检查现有部署（判断是首次部署还是更新）
# ========================================
echo ""
echo -e "${YELLOW}[2/9] 检查现有部署...${NC}"

PROJECT_NAME=$(get_env_value "COMPOSE_PROJECT_NAME")
EXISTING_CONTAINERS=$($DOCKER_COMPOSE ps -q 2>/dev/null | wc -l)

IS_FIRST_DEPLOY=false
if [ "$EXISTING_CONTAINERS" -eq 0 ]; then
    IS_FIRST_DEPLOY=true
    echo -e "${CYAN}  ℹ️  首次部署${NC}"
    SKIP_BACKUP=true
else
    echo -e "${CYAN}  ℹ️  检测到现有部署，将执行更新${NC}"
    echo -e "${GRAY}  运行中的容器数: $EXISTING_CONTAINERS${NC}"
fi

# ========================================
# SSL 证书检查/生成
# ========================================
if [ "$SKIP_CERT" = false ]; then
    echo ""
    echo -e "${YELLOW}[3/9] 检查 SSL 证书...${NC}"
    
    CERT_PATH=$(get_env_value "CERT_PATH")
    CERT_FILE="$CERT_PATH/agpayplusapi.pfx"
    
    if [ ! -f "$CERT_FILE" ]; then
        echo -e "${YELLOW}  ⚠️  证书不存在，开始生成...${NC}"
        
        if [ -f "$SCRIPT_DIR/generate-cert-linux.sh" ]; then
            bash "$SCRIPT_DIR/generate-cert-linux.sh"
        else
            echo -e "${RED}  ❌ 找不到证书生成脚本${NC}"
            exit 1
        fi
    else
        echo -e "${GREEN}  ✅ 证书已存在: $CERT_FILE${NC}"
    fi
else
    echo -e "${GRAY}  ⏭️  跳过证书检查${NC}"
fi

# ========================================
# 数据目录初始化
# ========================================
echo ""
echo -e "${YELLOW}[4/9] 初始化数据目录...${NC}"

DATA_PATH=$(get_env_value "DATA_PATH_HOST")
if [ -z "$DATA_PATH" ]; then
    echo -e "${RED}  ❌ DATA_PATH_HOST 未配置${NC}"
    exit 1
fi

# 创建必要的目录
mkdir -p "$DATA_PATH"/{logs,upload,mysql,redis,rabbitmq,seq}
echo -e "${GREEN}  ✅ 数据目录: $DATA_PATH${NC}"

# ========================================
# 备份当前部署
# ========================================
if [ "$SKIP_BACKUP" = false ] && [ "$IS_FIRST_DEPLOY" = false ]; then
    # 检查备份是否启用
    BACKUP_ENABLED=$(get_env_value "BACKUP_ENABLED")
    if [ "$BACKUP_ENABLED" = "true" ]; then
        echo ""
        echo -e "${YELLOW}[5/9] 备份当前部署...${NC}"
        
        # 获取备份路径
        BACKUP_PATH_FROM_ENV=$(get_env_value "BACKUP_PATH")
        BACKUP_DIR=${BACKUP_PATH_FROM_ENV:-"$SCRIPT_DIR/.backup"}
        TIMESTAMP=$(date +%Y%m%d_%H%M%S)
        BACKUP_PATH="$BACKUP_DIR/$TIMESTAMP"
        
        mkdir -p "$BACKUP_PATH"
        
        # 保存当前运行的镜像信息
        echo -e "${GRAY}  保存镜像信息...${NC}"
        exec_docker_compose ps --format json > "$BACKUP_PATH/containers.json" 2>/dev/null || true
        exec_docker_compose images --format json > "$BACKUP_PATH/images.json" 2>/dev/null || true
        
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
            exec_docker_compose images --format "{{.Repository}}:{{.Tag}}" | while read image; do
                service_name=$(echo "$image" | sed "s/${IMAGE_PREFIX}-//;s/:${IMAGE_TAG}//")
                echo -e "${GRAY}    备份: $service_name${NC}"
                docker save "$image" | gzip > "$BACKUP_PATH/${service_name}.tar.gz"
            done
        fi
        
        # 保存环境配置
        cp "$SCRIPT_DIR/.env" "$BACKUP_PATH/.env.backup"
        cp "$SCRIPT_DIR/docker-compose.yml" "$BACKUP_PATH/docker-compose.yml.backup"
        
        echo "$TIMESTAMP" > "$BACKUP_DIR/latest"
        echo -e "${GREEN}  ✅ 备份完成: $BACKUP_PATH${NC}"
        
        # 清理旧备份
        BACKUP_RETENTION=$(get_env_value "BACKUP_RETENTION")
        RETENTION_COUNT=${BACKUP_RETENTION:-5}
        BACKUP_COUNT=$(ls -1 "$BACKUP_DIR" | grep -E "^[0-9]" | wc -l)
        if [ "$BACKUP_COUNT" -gt "$RETENTION_COUNT" ]; then
            echo -e "${GRAY}  清理旧备份...${NC}"
            ls -1t "$BACKUP_DIR" | grep -E "^[0-9]" | tail -n +$((RETENTION_COUNT + 1)) | xargs -I {} rm -rf "$BACKUP_DIR/{}"
        fi
    else
        echo -e "${GRAY}[5/9] ⏭️  备份已禁用${NC}"
    fi
else
    echo -e "${GRAY}[5/9] ⏭️  跳过备份${NC}"
fi

# ========================================
# 构建参数准备
# ========================================
echo ""
echo -e "${YELLOW}[6/9] 准备构建参数...${NC}"

BUILD_ARGS=""
if [ -n "$BUILD_CASHIER" ]; then
    BUILD_ARGS="--build-arg BUILD_CASHIER=$BUILD_CASHIER"
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
# 构建镜像
# ========================================
echo ""
echo -e "${YELLOW}[7/9] 构建镜像...${NC}"

if [ -n "$SERVICES" ]; then
    echo -e "${CYAN}  构建服务: $SERVICES${NC}"
    exec_docker_compose build $BUILD_ARGS $SERVICES
else
    echo -e "${CYAN}  构建所有服务${NC}"
    exec_docker_compose build $BUILD_ARGS
fi

if [ $? -ne 0 ]; then
    echo -e "${RED}  ❌ 构建失败${NC}"
    exit 1
fi

echo -e "${GREEN}  ✅ 构建完成${NC}"

# ========================================
# 部署确认
# ========================================
if [ "$FORCE_DEPLOY" = false ]; then
    echo ""
    echo -e "${YELLOW}========================================${NC}"
    echo -e "${YELLOW}  准备部署${NC}"
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
    read -p "确认部署？[y/N] " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo -e "${RED}部署已取消${NC}"
        exit 0
    fi
fi

# ========================================
# 部署服务
# ========================================
echo ""
echo -e "${YELLOW}[8/9] 部署服务...${NC}"

deploy_success=false

if [ -n "$SERVICES" ]; then
    echo -e "${CYAN}  停止指定服务...${NC}"
    exec_docker_compose stop $SERVICES
    
    echo -e "${CYAN}  启动指定服务...${NC}"
    exec_docker_compose up -d $SERVICES
else
    echo -e "${CYAN}  部署所有服务...${NC}"
    exec_docker_compose up -d
fi

if [ $? -eq 0 ]; then
    deploy_success=true
    echo -e "${GREEN}  ✅ 服务启动成功${NC}"
else
    echo -e "${RED}  ❌ 服务启动失败${NC}"
    exit 1
fi

# ========================================
# 健康检查
# ========================================
echo ""
echo -e "${YELLOW}[9/9] 健康检查...${NC}"

sleep 5

if [ -n "$SERVICES" ]; then
    check_services=$SERVICES
else
    check_services=$(exec_docker_compose ps --services)
fi

failed_services=""
for service in $check_services; do
    status=$(exec_docker_compose ps "$service" --format "{{.State}}" 2>/dev/null || echo "unknown")
    
    if [ "$status" = "running" ]; then
        echo -e "${GREEN}  ✅ $service: $status${NC}"
    else
        echo -e "${RED}  ❌ $service: $status${NC}"
        failed_services="$failed_services $service"
        
        # 显示失败的服务日志
        echo -e "${GRAY}    最近日志:${NC}"
        exec_docker_compose logs --tail=10 "$service" 2>&1 | sed 's/^/      /'
    fi
 done

# ========================================
# 回滚逻辑
# ========================================
if [ -n "$failed_services" ]; then
    echo ""
    echo -e "${RED}========================================${NC}"
    echo -e "${RED}  部署失败${NC}"
    echo -e "${RED}========================================${NC}"
    echo -e "${RED}失败的服务:$failed_services${NC}"
    echo ""
    
    if [ "$SKIP_BACKUP" = false ] && [ -f "$BACKUP_DIR/latest" ]; then
        echo -e "${YELLOW}开始自动回滚...${NC}"
        
        if [ -f "$SCRIPT_DIR/rollback.sh" ]; then
            bash "$SCRIPT_DIR/rollback.sh" --auto --services "$failed_services"
        else
            echo -e "${RED}  ❌ 找不到回滚脚本${NC}"
            echo -e "${YELLOW}  请手动回滚: ./rollback.sh${NC}"
        fi
    else
        echo -e "${YELLOW}  无可用备份，请检查日志:${NC}"
        echo -e "${GRAY}  $DOCKER_COMPOSE logs -f${NC}"
    fi
    
    exit 1
fi

# ========================================
# 部署成功
# ========================================
echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}  🎉 部署成功！${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "${CYAN}环境信息：${NC}"
echo -e "  环境: ${YELLOW}$ENVIRONMENT${NC}"
echo -e "  项目: ${YELLOW}$PROJECT_NAME${NC}"
echo ""

# 读取访问地址
IPORDOMAIN=$(get_env_value "IPORDOMAIN")
echo -e "${CYAN}访问地址：${NC}"
echo -e "  管理平台: ${BLUE}https://${IPORDOMAIN}:8817${NC}"
echo -e "  代理商系统: ${BLUE}https://${IPORDOMAIN}:8816${NC}"
echo -e "  商户系统: ${BLUE}https://${IPORDOMAIN}:8818${NC}"
echo -e "  支付网关: ${BLUE}https://${IPORDOMAIN}:9819${NC}"
echo -e "  日志查看器: ${BLUE}http://${IPORDOMAIN}:5341${NC} (Seq)"
echo ""
echo -e "${CYAN}常用命令：${NC}"
echo -e "  查看状态: ${GRAY}$DOCKER_COMPOSE ps${NC}"
echo -e "  查看日志: ${GRAY}$DOCKER_COMPOSE logs -f [服务名]${NC}"
echo -e "  停止服务: ${GRAY}$DOCKER_COMPOSE stop [服务名]${NC}"
echo -e "  重启服务: ${GRAY}$DOCKER_COMPOSE restart [服务名]${NC}"
if [ "$SKIP_BACKUP" = false ]; then
    echo -e "  回滚版本: ${GRAY}./rollback.sh${NC}"
fi
echo ""
echo -e "${GREEN}========================================${NC}"
