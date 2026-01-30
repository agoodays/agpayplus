# DockerWslCleanup 使用文档

> **文件名**：`DockerWslCleanup.ps1`  
> **适用系统**：Windows 10 / Windows 11 + Docker Desktop（WSL2 后端）  
> **功能**：一键清理 Docker 无用资源，并压缩 WSL2 虚拟硬盘（支持 `docker_data.vhdx` 和 `ext4.vhdx`），真实释放磁盘空间  
> **版本**：v2.0（兼容 PowerShell 5.1+，自动提权，安全可靠）

---

## 📥 一、脚本下载与安装

### 1. 获取脚本
将下方完整脚本代码复制保存为 `DockerWslCleanup.ps1`：

> ✅ **重要提示**：必须使用 **记事本（Notepad）** 保存，并选择 **UTF-8 编码**！  
> ❌ 不要使用 VS Code 默认设置、Word 或浏览器直接另存为。

#### 保存步骤：
1. 打开 **记事本（Notepad）**
2. 粘贴完整脚本（见文末）
3. 点击 **文件 → 另存为**
   - 文件名：`DockerWslCleanup.ps1`
   - 保存类型：**所有文件 (\*.\*)**
   - 编码：**UTF-8**
4. 保存到任意目录（如 `C:\Scripts\` 或你的项目文件夹）

---

## ▶️ 二、运行方式

### 基础运行（推荐）
```powershell
# 在 PowerShell 中执行（无需手动提权）
.\DockerWslCleanup.ps1
```
- 首次运行会自动弹出 **UAC 管理员权限提示**
- 自动完成清理 + 压缩

---

### 常用参数

| 参数 | 作用 | 示例 |
|------|------|------|
| `-SkipVolumes` | **保留所有 Docker 卷**（防止误删数据库、配置等数据） | `.\DockerWslCleanup.ps1 -SkipVolumes` |
| `-DryRun` | **仅预览**将清理的内容，**不删除任何东西** | `.\DockerWslCleanup.ps1 -DryRun` |

> 💡 **如果你使用 MySQL、PostgreSQL、Redis 等容器并挂载了命名卷，请务必加 `-SkipVolumes`！**

---

## 🔒 三、权限说明

- 脚本启动时会自动检测管理员权限
- 若无权限，将自动重启并请求提升（弹出 UAC 窗口）
- 所有磁盘操作均为 **只读模式**（`attach vdisk readonly`），**不会损坏数据**
- 清理前可用 `-DryRun` 安全预览

---

## 📊 四、典型输出示例

```text
=============================================
   Docker + WSL2 智能清理与磁盘压缩工具
=============================================
[ℹ️] 获取清理前磁盘使用情况...
TYPE            TOTAL     ACTIVE    SIZE
Images          15        3         32.1GB
Containers      3         3         2.4GB
Local Volumes   6         2         10.2GB
Build Cache     200       0         25.8GB

[ℹ️] 开始清理 Docker 资源...
[ℹ️] 准备压缩 WSL2 虚拟硬盘...
[ℹ️] 检测到数据盘: C:\Users\haha\AppData\Local\Docker\wsl\disk\docker_data.vhdx
[ℹ️] 当前大小: 72.50 GB
[ℹ️] 正在压缩虚拟硬盘（请稍候）...
[✅] 压缩完成！
  压缩前: 72.50 GB
  压缩后: 18.30 GB
  释放空间: 54.20 GB
[✅] 🎉 清理与压缩全部完成！
```

---

## ⚠️ 五、注意事项

### 1. 环境要求
- ✅ 已启用 **WSL2**（在 Docker Desktop 设置中勾选 *Use the WSL 2 based engine*）
- ✅ **Docker Desktop 正在运行**（脚本依赖 `docker` CLI）

### 2. 关于数据卷（Volumes）
- 默认行为：**删除所有未被容器使用的卷**
- 安全建议：
  - 使用 `-SkipVolumes` 保留所有卷
  - 或提前备份重要卷：
    ```bash
    docker run --rm -v your_volume:/data -v ${PWD}:/backup alpine tar czf /backup/your_volume.tar.gz -C /data .
    ```

### 3. 首次运行被阻止？
如果看到安全警告：
```text
“此脚本来自 Internet，可能不安全…”
```
运行一次解除限制：
```powershell
Unblock-File -Path ".\DockerWslCleanup.ps1"
```

### 4. 压缩耗时
- 虚拟硬盘越大，压缩时间越长（通常 30 秒 ~ 5 分钟）
- 期间请勿操作 Docker 或 WSL2

---

## 🛠 六、故障排查

| 问题 | 解决方案 |
|------|--------|
| `param : 无法识别` | 确保 `param()` 是脚本第一个可执行语句（前面只能有注释） |
| 找不到 `.vhdx` 文件 | 检查路径 `%LOCALAPPDATA%\Docker\wsl\`，确认 Docker 使用 WSL2 后端 |
| 压缩后 `.vhdx` 大小没变 | 确保先清理 Docker 资源再压缩；WSL2 必须完全关闭（`wsl --shutdown`） |
| 权限错误 | 脚本已内置提权，若仍失败，请手动以管理员身份运行 PowerShell |

---

> ✨ **现在就运行 `.\DockerWslCleanup.ps1`，轻松释放几十 GB 磁盘空间！**  