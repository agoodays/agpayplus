#!/bin/bash
# ========================================
# AgPay+ 回滚脚本 (Linux/macOS)
# ========================================
# 功能：
# - 回滚到上一个备份版本
# - 支持指定服务回滚
# - 支持指定备份版本
# - 多环境支持
# - 自动模式（不需要确认）
# ========================================
# 使用方法：
# ./rollback.sh                            # 回滚所有服务（生产环境）
# ./rollback.sh --env development          # 回滚开发环境
# ./rollback.sh --services agpay-manager-api    # 仅回滚指定服务
# ./rollback.sh --backup 20240101_120000   # 回滚到指定备份
# ./rollback.sh --list                     # 列出所有备份
# ./rollback.sh --help                     # 查看帮助
# ========================================

set -e

# ========================================
# 默认配置
# ========================================
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENVIRONMENT="production"
SERVICES=""
BACKUP=""
LIST=false
AUTO=false

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BLUE='\033[0;34m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# ========================================
# 帮助信息
# ========================================
show_help() {
    cat << EOF
${CYAN}========================================
  AgPay+ 回滚脚本 (Linux/macOS)
========================================${NC}

${GREEN}功能：${NC}
  • 回滚到上一个备份版本
  • 支持指定服务回滚
  • 支持指定备份版本
  • 多环境支持

${GREEN}使用方法：${NC}
  $0 [选项]

${GREEN}选项：${NC}
  ${YELLOW}--env <环境>${NC}           指定环境 (development/staging/production)
                               默认: production
  ${YELLOW}--services <服务>${NC}     指定要回滚的服务（空格分隔）
                               示例: "agpay-manager-api agpay-agent-api"
  ${YELLOW}--backup <版本>${NC}        指定备份版本（时间戳格式）
  ${YELLOW}--list${NC}                列出所有可用备份
  ${YELLOW}--auto${NC}                自动模式（不需要确认）
  ${YELLOW}--help, -h${NC}            显示此帮助信息

${GREEN}示例：${NC}
  ${GRAY}# 列出所有备份${NC}
  $0 --list

  ${GRAY}# 回滚所有服务到最新备份${NC}
  $0

  ${GRAY}# 回滚指定服务${NC}
  $0 --services agpay-manager-api

  ${GRAY}# 回滚到指定备份版本${NC}
  $0 --backup 20240101_120000

  ${GRAY}# 自动回滚（部署失败时使用）${NC}
  $0 --auto --services agpay-manager-api

${CYAN}环境说明：${NC}
  ${GRAY}• development  - 开发环境（配置文件: .env.development）${NC}
  ${GRAY}• staging      - 预发布环境（配置文件: .env.staging）${NC}
  ${GRAY}• production   - 生产环境（配置文件: .env.production）${NC}

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
            -e 's/^["'"'"']//' \
            -e 's/["'"'"']*$//' \
            -e 's/#.*//')
    
    value="${value/#\~/$HOME}"
    echo "$value"
}

# ========================================
# 检测 Docker Compose 命令
# ========================================
DOCKER_COMPOSE=""
if command -v docker &> /dev/null && docker compose version &> /dev/null 2>&1; then
    DOCKER_COMPOSE="docker compose"
elif command -v docker-compose &> /dev/null; then
    DOCKER_COMPOSE="docker-compose"
fi

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
        --backup)
            BACKUP="$2"
            shift 2
            ;;
        --list)
            LIST=true
            shift
            ;;
        --auto)
            AUTO=true
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

# 备份目录
BACKUP_DIR=$(get_env_value "BACKUP_PATH")
if [ -z "$BACKUP_DIR" ]; then
    BACKUP_DIR="$SCRIPT_DIR/.backup"
fi

# ========================================
# 列出备份
# ========================================
show_backups() {
    echo -e "${CYAN}========================================${NC}"
    echo -e "${CYAN}  可用备份列表${NC}"
    echo -e "${CYAN}========================================${NC}"
    echo ""
    
    if [ ! -d "$BACKUP_DIR" ]; then
        echo -e "${YELLOW}  ⚠️  暂无备份${NC}"
        return
    fi
    
    # 按环境分组显示
    for env in "production" "staging" "development"; do
        backups=($(ls -d "$BACKUP_DIR/${env}_"* 2>/dev/null | sort -r))
        
        if [ ${#backups[@]} -gt 0 ]; then
            echo -e "${YELLOW}$env 环境:${NC}"
            for backup in "${backups[@]}"; do
                backup_time=$(basename "$backup" | sed "s/^${env}_update_//;s/^${env}_//")
                backup_size=$(du -sh "$backup" | cut -f1)
                
                # 检查是否是最新备份
                latest_marker=""
                latest_file="$BACKUP_DIR/latest_$env"
                if [ -f "$latest_file" ]; then
                    latest=$(cat "$latest_file")
                    if [[ "$backup" == *"$latest"* ]]; then
                        latest_marker=" (最新)"
                    fi
                fi
                
                echo -e "  ${GRAY}$backup_time${NC}  ${BLUE}[$backup_size]${NC}$latest_marker"
                
                # 显示包含的服务
                services=($(ls "$backup"/*.tar* 2>/dev/null | xargs basename 2>/dev/null | sed 's/\.tar.*//'))
                if [ ${#services[@]} -gt 0 ]; then
                    echo -e "    服务: ${GRAY}${services[*]}${NC}"
                else
                    echo -e "    服务: ${GRAY}无镜像${NC}"
                fi
            done
            echo ""
        fi
    done
    
    echo -e "${CYAN}========================================${NC}"
}

# 列出备份模式
if [ "$LIST" = true ]; then
    show_backups
    exit 0
fi

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
echo -e "${CYAN}  AgPay+ 回滚脚本${NC}"
echo -e "${CYAN}========================================${NC}"
echo -e "环境: ${BLUE}$ENVIRONMENT${NC}"
if [ -n "$SERVICES" ]; then
    echo -e "服务: ${BLUE}$SERVICES${NC}"
else
    echo -e "服务: ${BLUE}所有服务${NC}"
fi
echo -e "${CYAN}========================================${NC}"
echo ""

# ========================================
# 检查备份目录
# ========================================
echo -e "${YELLOW}[1/5] 检查备份...${NC}"

if [ ! -d "$BACKUP_DIR" ]; then
    echo -e "${RED}  ❌ 备份目录不存在${NC}"
    exit 1
fi

# 确定要使用的备份版本
BACKUP_PATH=""
if [ -n "$BACKUP" ]; then
    # 使用指定的备份版本
    BACKUP_PATH="$BACKUP_DIR/${ENVIRONMENT}_${BACKUP}"
    if [ ! -d "$BACKUP_PATH" ]; then
        # 尝试 update 备份
        BACKUP_PATH="$BACKUP_DIR/${ENVIRONMENT}_update_${BACKUP}"
    fi
    
    if [ ! -d "$BACKUP_PATH" ]; then
        echo -e "${RED}  ❌ 指定的备份不存在: $BACKUP${NC}"
        echo -e "${YELLOW}  使用 --list 查看所有可用备份${NC}"
        exit 1
    fi
else
    # 使用最新的备份
    latest_file="$BACKUP_DIR/latest_$ENVIRONMENT"
    if [ -f "$latest_file" ]; then
        latest_timestamp=$(cat "$latest_file")
        BACKUP_PATH="$BACKUP_DIR/${ENVIRONMENT}_update_${latest_timestamp}"
        
        if [ ! -d "$BACKUP_PATH" ]; then
            BACKUP_PATH="$BACKUP_DIR/${ENVIRONMENT}_${latest_timestamp}"
        fi
    else
        # 查找最新的备份目录
        latest_backup=$(ls -d "$BACKUP_DIR/${ENVIRONMENT}_"* 2>/dev/null | sort -r | head -n 1)
        if [ -d "$latest_backup" ]; then
            BACKUP_PATH="$latest_backup"
        fi
    fi
    
    if [ -z "$BACKUP_PATH" ] || [ ! -d "$BACKUP_PATH" ]; then
        echo -e "${RED}  ❌ 找不到可用的备份${NC}"
        echo -e "${YELLOW}  使用 --list 查看所有可用备份${NC}"
        exit 1
    fi
fi

echo -e "${GREEN}  ✅ 找到备份: $(basename "$BACKUP_PATH")${NC}"

# 列出备份中的服务
echo -e "  备份中的服务:" "${GRAY}"
services=($(ls "$BACKUP_PATH"/*.tar* 2>/dev/null | xargs basename 2>/dev/null | sed 's/\.tar.*//'))
if [ ${#services[@]} -gt 0 ]; then
    for svc in "${services[@]}"; do
        echo -e "    - $svc" "${GRAY}"
    done
else
    echo -e "${YELLOW}  ⚠️  备份中没有镜像文件${NC}"
fi

# ========================================
# 回滚确认
# ========================================
if [ "$AUTO" = false ]; then
    echo ""
    echo -e "${CYAN}========================================${NC}"
    echo -e "${CYAN}  准备回滚${NC}"
    echo -e "${CYAN}========================================${NC}"
    echo -e "环境: ${BLUE}$ENVIRONMENT${NC}"
    echo -e "备份: ${BLUE}$(basename "$BACKUP_PATH")${NC}"
    if [ -n "$SERVICES" ]; then
        echo -e "服务: ${BLUE}$SERVICES${NC}"
    else
        echo -e "服务: ${BLUE}所有服务${NC}"
    fi
    echo -e "${RED}警告: 这将覆盖当前运行的服务${NC}"
    echo -e "${CYAN}========================================${NC}"
    echo ""
    
    read -p "确认回滚？[y/N] " -n 1 -r
    echo
    
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo -e "${RED}回滚已取消${NC}"
        exit 0
    fi
fi

# ========================================
# 恢复环境配置
# ========================================
echo ""
echo -e "${YELLOW}[2/5] 恢复环境配置...${NC}"

env_backup="$BACKUP_PATH/.env.backup"
if [ -f "$env_backup" ]; then
    cp "$env_backup" "$SCRIPT_DIR/.env"
    echo -e "${GREEN}  ✅ 环境配置已恢复${NC}"
else
    echo -e "${YELLOW}  ⚠️  备份中没有环境配置文件，使用当前配置${NC}"
fi

# ========================================
# 加载镜像
# ========================================
echo ""
echo -e "${YELLOW}[3/5] 加载备份镜像...${NC}"

if [ -n "$SERVICES" ]; then
    # 仅加载指定服务的镜像
    for service in $SERVICES; do
        tar_file="$BACKUP_PATH/${service}.tar"
        tgz_file="$BACKUP_PATH/${service}.tar.gz"
        if [ -f "$tar_file" ]; then
            echo -e "  加载: $service" "${GRAY}"
            docker load -i "$tar_file"
        elif [ -f "$tgz_file" ]; then
            echo -e "  加载: $service (gz)" "${GRAY}"
            gunzip -c "$tgz_file" | docker load
        else
            echo -e "${YELLOW}  ⚠️  服务 $service 的备份不存在${NC}"
        fi
    done
else
    # 加载所有备份的镜像 (.tar 优先)
    image_files=($(ls "$BACKUP_PATH"/*.tar 2>/dev/null) $(ls "$BACKUP_PATH"/*.tar.gz 2>/dev/null))
    for image_file in "${image_files[@]}"; do
        if [ -f "$image_file" ]; then
            service=$(basename "$image_file" | sed 's/\.tar.*//')
            echo -e "  加载: $service" "${GRAY}"
            if [[ "$image_file" == *.tar ]]; then
                docker load -i "$image_file"
            elif [[ "$image_file" == *.tar.gz ]]; then
                gunzip -c "$image_file" | docker load
            fi
        fi
    done
fi

echo -e "${GREEN}  ✅ 镜像加载完成${NC}"

# ========================================
# 重启服务
# ========================================
echo ""
echo -e "${YELLOW}[4/5] 重启服务...${NC}"

if [ -n "$SERVICES" ]; then
    for service in $SERVICES; do
        echo -e "${CYAN}  重启 $service...${NC}"
        $DOCKER_COMPOSE stop "$service"
        $DOCKER_COMPOSE rm -f "$service"
        $DOCKER_COMPOSE up -d "$service"
    done
else
    echo -e "${CYAN}  重启所有服务...${NC}"
    $DOCKER_COMPOSE down 2>/dev/null || true
    $DOCKER_COMPOSE up -d
fi

# ========================================
# 健康检查
# ========================================
echo ""
echo -e "${YELLOW}[5/5] 健康检查...${NC}"

echo -e "  等待服务启动..." "${GRAY}"
sleep 10

if [ -n "$SERVICES" ]; then
    check_services="$SERVICES"
else
    check_services=$($DOCKER_COMPOSE ps --services 2>/dev/null)
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
        echo -e "    最近日志:${GRAY}"
        $DOCKER_COMPOSE logs --tail=20 "$service" 2>&1 | sed 's/^/      /'
    fi
done

# ========================================
# 回滚结果
# ========================================
echo ""
if [ -n "$failed_services" ]; then
    echo -e "${RED}========================================${NC}"
    echo -e "${RED}  ⚠️  回滚完成，但部分服务未正常运行${NC}"
    echo -e "${RED}========================================${NC}"
    echo -e "${RED}失败的服务:$failed_services${NC}"
    echo ""
    echo -e "${YELLOW}  请检查日志:${NC}"
    echo -e "  ${GRAY}$DOCKER_COMPOSE logs -f${NC}"
    echo ""
    exit 1
else
    echo -e "${GREEN}========================================${NC}"
    echo -e "${GREEN}  🎉 回滚成功！${NC}"
    echo -e "${GREEN}========================================${NC}"
    echo ""
    echo -e "${CYAN}环境信息：${NC}"
    echo -e "  环境: ${YELLOW}$ENVIRONMENT${NC}"
    echo -e "  备份: ${YELLOW}$(basename "$BACKUP_PATH")${NC}"
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
    echo -e "  查看备份: ${GRAY}$0 --list${NC}"
    echo ""
    echo -e "${GREEN}========================================${NC}"
fi
