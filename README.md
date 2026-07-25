# MobBuffController 怪物范围Buff控制器

- 作者: 淦
- 在指定类型怪物周围给玩家添加或移除Buff，支持自定义持续时间和影响范围。

## 指令

| 语法 | 权限 | 说明 |
|------|------|------|
| `/mobbuff add <怪物ID/名称> <buffID> <范围> <持续时间(秒)>` | `mobbuff.use` | 在指定怪物周围给玩家添加Buff |
| `/mobbuff del <怪物ID/名称> <buffID> <范围>` | `mobbuff.use` | 在指定怪物周围给玩家移除Buff |

### 参数说明
- **怪物ID/名称**：目标怪物的数字ID或名称（支持模糊匹配，如“僵尸”会匹配所有含“僵尸”的怪物）

- **buffID**：要添加/移除的Buff数字ID（可用 `/bufflist` 查询）

- **范围**：以怪物为中心的影响半径（单位：游戏格子）

- **持续时间**：（仅add命令）Buff持续时间，单位为秒

### 使用示例

/mobbuff add 僵尸 1 20 30        # 给20格内僵尸周围的玩家添加30秒再生Buff

/mobbuff del 史莱姆 24 15         # 移除15格内史莱姆周围玩家的燃烧Debuff

/mobbuff add 50 87 25 60         # 给噬魂怪（ID=50）周围25格内的玩家添加60秒发光Buff

/mobbuff add 骷髅 23 30 120       # 给30格内骷髅周围的玩家添加120秒夜视Buff
## 权限

- `mobbuff.use`：允许使用 `/mobbuff` 命令。默认未授予，需手动添加。

/group addperm default mobbuff.use   # 给所有玩家添加权限
/user addperm <玩家名> mobbuff.use   # 给特定玩家添加权限
## 配置

> 本插件无需配置文件，所有功能通过命令实时控制。
## 更新日志

### v2025.6.28.4
- 修复API兼容性问题（移除对 HasBuff 的依赖，优化 RemoveBuff 逻辑）

### v2025.4.18.3
- 增加自定义持续时间（秒为单位）
- 移除 `/moblist` 命令，简化使用

### v2025.4.18.2
- 支持指定怪物类型（ID/名称）
- 优化距离计算性能

### v2025.4.18.1
- 初始版本发布，实现怪物范围Buff控制
## 反馈

- 优先提交 Issue 至插件仓库：[[GitHub 链接](https://github.com/ICU-Club/TargetedMobBuff.git)](https://github.com/Gangan1145/MobuffController.git)
