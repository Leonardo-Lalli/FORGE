<div align="center">

> ⚠️ **业余教育项目** ⚠️  
> 本应用由学生作为学校项目开发。  
> **不是商业产品。** 服务器托管在家庭硬件上。  
> 可能存在错误、限制和意外停机。  
> 仅用于测试和演示，请勿用于重要数据。

[🇬🇧 English](README_en.md) · [🇮🇹 Italiano](README.md) · [🇪🇸 Español](README_es.md) · [🇨🇳 中文](README_zh.md)

<img src="assets/screenshots/dashboard.jpeg" width="120" style="border-radius:24px" />

# FORGE

### 社交健身日记

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Android--first-512BD4?logo=dotnet)](https://learn.microsoft.com/dotnet/maui/)
[![PocketBase](https://img.shields.io/badge/PocketBase-Backend-000000?logo=pocketbase)](https://pocketbase.io/)
[![ExerciseDB](https://img.shields.io/badge/ExerciseDB-1500%2B%20动作-22C55E)](https://oss.exercisedb.dev)
[![MVVM](https://img.shields.io/badge/架构-MVVM%20%7C%20CommunityToolkit-7C3AED)](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
[![Tests](https://img.shields.io/badge/测试-27%20通过-brightgreen)](tests/)
[![License](https://img.shields.io/badge/许可-MIT-blue)](LICENSE)

**把每一次训练变成进步。挑战你的朋友。突破你的极限。**

</div>

---

## 什么是 FORGE

FORGE 是一款记录健身训练的 Android 应用，具有社交功能。记录动作、组数和次数。关注朋友。解锁成就。用详细的数据查看你的进步。

---

## 功能

| 类别 | 功能 |
|------|------|
| 🏋️ **训练** | 动作搜索（1500+ 带GIF）, 组数 kg×次数, 完成标记, 休息计时器, 进度照片, 最小化/草稿 |
| 📊 **统计** | 训练量图表, 最佳成绩, 月度日历, 周/月/季/年/全部筛选 |
| 👥 **社交** | 好友动态, 点赞 ♥, 关注/取消关注, 用户搜索, 好友请求 |
| 🏆 **成就** | 48个徽章可解锁, 自动追踪, 个人资料展示 |
| 👤 **个人** | 头像, 简介, 统计, 训练历史, 已解锁徽章 |
| 🎨 **界面** | 明暗双主题, Inter/Lexend/Space Grotesk 字体 |
| 📱 **离线** | 本地 SQLite 数据库, 联网后自动同步 |
| 🔒 **安全** | SecureStorage 加密密码, HTTPS, API 行级权限, 管理面板已阻止 |

---

## 技术架构

| 层级 | 技术 |
|------|------|
| 框架 | .NET MAUI 10 (Android 优先) |
| UI | MVVM + CommunityToolkit.Mvvm 8.4 |
| 导航 | Shell (3 标签 + 8 路由) |
| 后端 | PocketBase 自托管 |
| API | ExerciseDB v1 (1500+ 动作, 免费) |
| 持久化 | SQLite (sqlite-net-pcl) |
| 测试 | xUnit (27 测试) |
| 字体 | Inter, Lexend, Space Grotesk |

---

## 开发

```bash
git clone https://github.com/USERNAME/FORGE.git
cd FORGE
cp .env.example .env  # 编辑你的 PocketBase URL
dotnet build src/Forge/Forge.csproj -f net10.0-android
dotnet test tests/Forge.Tests/
```

---

## 隐私 · 安全 · 免责声明

**隐私**: 数据存储在手机 (SQLite) 和 FORGE 服务器 (PocketBase)。照片作为训练的一部分保存。

**安全**: 密码加密 (SecureStorage), HTTPS, API 行级权限, 管理面板已阻止, 速率限制。

**免责**: FORGE 是一个教育项目，不是商业产品。服务器是家用的，可能不会一直可用。

**许可**: MIT — 查看 [LICENSE](LICENSE)。

---

<div align="center">

**FORGE** — 塑造你的体魄。挑战你的朋友。铸就你的传奇。

</div>
