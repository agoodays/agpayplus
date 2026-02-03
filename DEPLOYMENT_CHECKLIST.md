# 🚀 AgPay+ Docker 部署前检查清单

在执行部署前，请按照此清单逐项检查，确保所有前置条件都已满足。

## ✅ 系统环境检查

### 1. Docker 环境

- [ ] Docker Desktop 已安装并运行（Windows/macOS）
  ```bash
  docker --version
  # 预期输出：Docker version 20.10.x 或更高
  ```

- [ ] Docker Engine 已安装并运行（Linux）
  ```bash
  docker --version
  sudo systemctl status docker
  ```

- [ ] Docker Compose 可用
  ```bash
  docker compose version
  # 预期输出：Docker Compose version v2.x.x 或更高
  ```

- [ ] Docker 有足够的资源配置
  - **CPU**：至少 4 核
  - **内存**：至少 4GB
  - **磁盘空间**：至少 20GB

### 2. .NET SDK（用于生成证书）

- [ ] .NET SDK 6.0 或更高版本已安装
  ```bash
  dotnet --version
  # 预期输出：6.0.x 或更高
  ```

- [ ] 如果未安装，请访问：
  - Windows/macOS: https://dotnet.microsoft.com/download
  - Linux: https://docs.microsoft.com/dotnet/core/install/linux

### 3. MySQL 数据库

- [ ] MySQL 8.0+ 已安装在宿主机上
  ```bash
  mysql --version
  # 预期输出：mysql Ver 8.0.x
  ```

- [ ] MySQL 服务正在运行
  ```bash
  # Windows
  Get-Service MySQL* | Select Status

  # Linux
  sudo systemctl status mysql
  
  # macOS
  brew services list | grep mysql
  ```

- [ ] 已创建数据库
  ```sql
  CREATE DATABASE agpayplusdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
  ```

- [ ] 已导入初始化脚本
  ```bash
  mysql -u root -p agpayplusdb < aspnet-core/docs/sql/agpayplusinit.sql
  ```

- [ ] MySQL 允许远程连接（Docker 容器访问）
  ```sql
  -- 检查用户权限
  SELECT host, user FROM mysql.user WHERE user='root';
  
  -- 如果没有 '%' 或 '172.%'，执行：
  CREATE USER 'root'@'%' IDENTIFIED BY 'your_password';
  GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'root'@'%';
  FLUSH PRIVILEGES;
  ```

## ✅ 项目文件检查

### 1. 代码完整性

- [ ] 项目已克隆到本地
  ```bash
  git clone https://github.com/agoodays/agpayplus.git
  cd agpayplus
  ```

- [ ] 所有必需的目录存在
  ```
  agpayplus/
  ├── aspnet-core/
  │   ├── src/
  │   └── docs/
  └── ant-design-vue/
      ├── agpay-ui-manager/
      ├── agpay-ui-agent/
      ├── agpay-ui-merchant/
      └── agpay-ui-cashier/
  ```

- [ ] RabbitMQ 延迟插件文件存在
  ```bash
  ls aspnet-core/docs/rabbitmq_plugin/rabbitmq_delayed_message_exchange-3.13.0.ez
  ```

### 2. 部署脚本

- [ ] 部署脚本存在并有执行权限（Linux/macOS）
  ```bash
  ls -l *.sh
  chmod +x *.sh  # 如果需要
  ```

- [ ] PowerShell 脚本存在（Windows）
  ```powershell
  Get-ChildItem *.ps1
  ```

### 3. 配置文件

- [ ] 环境变量模板文件存在
```bash
ls -l .env.development .env.staging .env.production .env.example
```

## ✅ 环境配置

### 1. 环境变量配置

- [ ] 已复制并编辑 `.env` 文件
```bash
# Windows
Copy-Item .env.production .env
  
# Linux/macOS
cp .env.production .env
```

- [ ] 核心配置已正确设置

  #### Windows 配置
  ```env
  IPORDOMAIN=localhost                    # ✅ 已设置
  MYSQL_SERVER_NAME=host.docker.internal  # ✅ Windows/macOS 使用此值
  MYSQL_PORT=3306                         # ✅ 已设置
  MYSQL_DATABASE=agpayplusdb              # ✅ 已设置
  MYSQL_USER=root                         # ✅ 已设置
  MYSQL_PASSWORD=your_actual_password     # ⚠️ 需要修改为实际密码
  DATA_PATH_HOST=E:/app/agpayplus         # ⚠️ 修改为实际路径
  CERT_PATH=${USERPROFILE}/.aspnet/https  # ✅ Windows 默认值
  ```

  #### Linux 配置
  ```env
  IPORDOMAIN=localhost                    # ✅ 已设置
  MYSQL_SERVER_NAME=172.17.0.1            # ⚠️ 需要确认宿主机IP
  MYSQL_PORT=3306                         # ✅ 已设置
  MYSQL_DATABASE=agpayplusdb              # ✅ 已设置
  MYSQL_USER=root                         # ✅ 已设置
  MYSQL_PASSWORD=your_actual_password     # ⚠️ 需要修改为实际密码
  DATA_PATH_HOST=/opt/agpayplus           # ✅ Linux 默认值
  CERT_PATH=~/.aspnet/https               # ✅ Linux 默认值
  ```

  #### macOS 配置
  ```env
  IPORDOMAIN=localhost                    # ✅ 已设置
  MYSQL_SERVER_NAME=host.docker.internal  # ✅ macOS 使用此值
  MYSQL_PORT=3306                         # ✅ 已设置
  MYSQL_DATABASE=agpayplusdb              # ✅ 已设置
  MYSQL_USER=root                         # ✅ 已设置
  MYSQL_PASSWORD=your_actual_password     # ⚠️ 需要修改为实际密码
  DATA_PATH_HOST=/opt/agpayplus           # ✅ macOS 默认值
  CERT_PATH=~/.aspnet/https               # ✅ macOS 默认值
  ```

### 2. Linux 特殊配置

- [ ] 获取 Docker 网桥 IP（Linux）
  ```bash
  ip addr show docker0 | grep inet
  # 或使用默认 IP
  # MYSQL_SERVER_NAME=172.17.0.1
  ```

- [ ] 确认宿主机 MySQL 可从 Docker 网络访问
  ```bash
  # 测试连接
  docker run --rm mysql:8.0 mysql -h 172.17.0.1 -u root -p
  ```

### 3. 数据目录

- [ ] 数据存储目录已创建（或部署脚本会自动创建）
  ```bash
  # Windows
  New-Item -ItemType Directory -Path E:\app\agpayplus -Force
  
  # Linux/macOS
  sudo mkdir -p /opt/agpayplus
  sudo chown -R $(whoami):$(whoami) /opt/agpayplus
  ```

## ✅ SSL 证书

### 选项 1：自动生成（推荐）

- [ ] 部署脚本会自动生成开发证书
  - 证书名称：agpayplusapi
  - 证书密码：123456
  - 证书路径：`~/.aspnet/https/agpayplusapi.pfx`

### 选项 2：手动生成

- [ ] 执行证书生成脚本
  ```bash
  # Windows
  .\generate-cert-windows.ps1
  
  # Linux/macOS
  ./generate-cert-linux.sh
  ```

- [ ] 验证证书文件存在
  ```bash
  # Windows
  Test-Path $env:USERPROFILE\.aspnet\https\agpayplusapi.pfx
  
  # Linux/macOS
  ls -l ~/.aspnet/https/agpayplusapi.pfx
  ```

### 选项 3：使用生产证书

- [ ] 准备正式 SSL 证书（.pfx 格式）
- [ ] 复制到证书目录
- [ ] 更新 docker-compose.yml 中的证书密码

## ✅ 网络和端口

### 1. 端口占用检查

确保以下端口未被占用：

- [ ] **8817**（运营平台前端）
  ```bash
  # Windows
  netstat -ano | findstr :8817
  
  # Linux/macOS
  lsof -i :8817
  ```

- [ ] **8816**（代理商前端）
- [ ] **8818**（商户前端）
- [ ] **9817/5817**（运营平台 API）
- [ ] **9816/5816**（代理商 API）
- [ ] **9818/5818**（商户 API）
- [ ] **9819/5819**（支付网关 API + 收银台）
- [ ] **6379**（Redis）
- [ ] **5672/15672**（RabbitMQ）

### 2. 防火墙规则

- [ ] Windows 防火墙允许 Docker 通信
- [ ] Linux iptables/firewalld 允许相关端口
- [ ] 云服务器安全组规则已配置

## ✅ 部署前最终检查

### 1. 验证 Docker Compose 配置

- [ ] 检查配置语法
  ```bash
  docker compose config
  ```

- [ ] 查看将使用的配置
  ```bash
  docker compose config --services
  # 预期输出：
  # agpay-ui-manager
  # agpay-ui-agent
  # agpay-ui-merchant
  # agpay-manager-api
  # agpay-agent-api
  # agpay-merchant-api
  # agpay-payment-api
  # redis
  # rabbitmq
  ```

### 2. 磁盘空间

- [ ] 检查可用磁盘空间
  ```bash
  # Windows
  Get-PSDrive C
  
  # Linux/macOS
  df -h
  ```

- [ ] 至少有 20GB 可用空间（用于 Docker 镜像和容器）

### 3. 内存和 CPU

- [ ] 系统有足够的可用内存（至少 4GB）
  ```bash
  # Windows
  Get-ComputerInfo | Select-Object CsTotalPhysicalMemory, CsPhyicallyInstalledMemory
  
  # Linux
  free -h
  
  # macOS
  top -l 1 | grep PhysMem
  ```

## ✅ 准备开始部署

所有检查项都完成后，您可以开始部署：

### Windows 部署

```powershell
# 完整部署
.\deploy.ps1

# 如果已有证书，跳过证书生成
.\deploy.ps1 -SkipCert

# 如果已配置 .env，跳过环境配置
.\deploy.ps1 -SkipEnv
```

### Linux/macOS 部署

```bash
# 完整部署
./deploy.sh

# 跳过证书生成
./deploy.sh --skip-cert

# 跳过环境配置
./deploy.sh --skip-env
```

## 📊 预期部署时间

- **首次部署**：15-25 分钟（包括下载镜像、构建）
- **后续更新**：5-10 分钟

最慢的部分：
1. Payment API 构建（包含收银台前端）：5-7 分钟
2. 前端构建：每个 3-5 分钟
3. 后端 API 构建：每个 2-3 分钟

## 🆘 如遇问题

如果遇到问题，请查看：

1. **部署日志**：`docker compose logs -f`
2. **错误排查文档**：[TROUBLESHOOTING.md](TROUBLESHOOTING.md) - 常见问题与解决办法
3. **收银台部署说明**：[CASHIER_DEPLOYMENT.md](CASHIER_DEPLOYMENT.md)

常见问题：
- MySQL 连接失败 → 检查 `MYSQL_SERVER_NAME` 和防火墙
- 端口被占用 → 修改 docker-compose.yml 中的端口映射
- 证书错误 → 重新生成证书
- 构建失败 → 清理 Docker 缓存：`docker system prune -a`

---

**准备就绪？开始部署吧！** 🚀
