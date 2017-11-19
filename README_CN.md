KMCCC
=====

[![Join the chat at https://gitter.im/MineStudio/KMCCC](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/MineStudio/KMCCC?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/ldvo1wsyd66boxsf?svg=true)](https://ci.appveyor.com/project/zhouyiran2/kmccc)

一个使用.Net开发的开源Minecraft启动核心。

## KMCCC.Shared

KMCCC.Basic与KMCCC.Pro共享的代码

### 包括:

- 基本的启动模型（Launch Model）
- 验证模型（Yggdrasil、离线……）
- Java与系统信息的查询
- Zip模块，使用.Net Internal API ( MS.Internal.IO.Zip.* )
- 基本的版本定位器（JVersion）
- 一些有用的扩展
- 启动报告，可以选择关闭
- 自定义验证服务器

## KMCCC.Basic

基本功能的KMCCC

### 包含:

- 包括kMCCC.Shared的所有功能
- 没了（逃）

## KMCCC.Pro

专业功能的KMCCC

### 包含:

- 包括KMCCC.Shared的所有功能
- Mojang API
- [开发中] 游戏文件的下载、检查、修复 (包括 Version, Library, Native&Assets)
- [开发中] 更多LaunchHandle有用的扩展
- [开发中] 可能需要更多有用的库

## 计划

我们计划延迟一些参数设定, (比如versionLocator)并给KMCCC.Pro提供更多功能

# 样例

## 如何初始化 LauncherCore

```csharp

LauncherCore core = LauncherCore.Create(
	new LauncherCoreCreationOption(
		javaPath: Config.Instance.JavaPath, // 默认为找到的第一个版本
		gameRootPath: null, // 默认为 ./.minecraft/
		versionLocator: the Version Locator // 默认情况下将会 new JVersionLocator()
	));

```

## 如何找到 Versions（指定游戏版本）

```csharp

var versions = core.GetVersions();

var version = core.GetVersion("1.8");

```

*无效的版本将会被忽略*

## 如何启动 Minecraft


```csharp
var result = core.Launch(new LaunchOptions
{
	Version = App.LauncherCore.GetVersion(server.VersionId)
	Authenticator = new OfflineAuthenticator("Steve"), // 离线模式启动
	//Authenticator = new YggdrasilLogin("*@*.*", "***", true), // 在线模式
	MaxMemory = Config.Instance.MaxMemory, // 可选
	MinMemory = Config.Instance.MaxMemory, // 可选
	Mode = LaunchMode.MCLauncher, // 可选
	Server = new ServerInfo {Address = "mc.hypixel.net"}, //可选
	Size = new WindowSize {Height = 768, Width = 1280} //可选
}, (Action<MinecraftLaunchArguments>) (x => { })); // 可选 ( 启动前修改参数
```

## 使用匿名报告 ##

```csharp
Reporter.SetClientName("你的启动器名字"); // 设置启动器名字
Reporter.SetReportLevel(ReportLevel.Full); // 报告所有信息
//Reporter.SetReportLevel(ReportLevel.Basic); // 报告基本信息
//Reporter.SetReportLevel(ReportLevel.Min); // 报告非常少的信息
//Reporter.SetReportLevel(ReportLevel.None); // 关掉
```

# Enjoy!
