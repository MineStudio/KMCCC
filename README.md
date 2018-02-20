KMCCC
=====

[![Join the chat at https://gitter.im/MineStudio/KMCCC](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/MineStudio/KMCCC?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/ldvo1wsyd66boxsf?svg=true)](https://ci.appveyor.com/project/zhouyiran2/kmccc)

如果你需要中文的介绍信息，请打开README_CN.md

An OpenSource Minecraft Launcher for .Net Developers

## KMCCC.Shared

Shared Code Between KMCCC.Basic & KMCCC.Pro

### Included:

- Basic Launch Model (LauncherCore, LaunchOptions, ...)
- Authentication Model (Yggdrasil, Offline, ...)
- Java & System Information Finder
- Zip Module using .Net Internal API ( MS.Internal.IO.Zip.* )
- Basic VersionLocator (JVersion)
- A set of useful extensions
- A launch reporter that can be disabled
- Custom authentication server

## KMCCC.Basic

Basic Version of KMCCC

### Included:

- Everything in kMCCC.Shared
- No more

## KMCCC.Pro

Professional Version of KMCCC

### Included:

- Everything in KMCCC.Shared
- Mojang API
- [WIP] Game File Download & Fix & Check (Including Version, Library, Native&Assets)
- [WIP] More useful extensions for LaunchHandle
- [WIP] More useful libraries that a launch might need.

## Plan

We plan to delay some parameter settings, (such as versionLocator) and make more features available in KMCCC.Pro

# Sample

## How to initialize a LauncherCore

__C#__
```csharp

LauncherCore core = LauncherCore.Create(
	new LauncherCoreCreationOption(
		javaPath: Config.Instance.JavaPath, // by default it will be the first version finded
		gameRootPath: null, // by defualt it will be ./.minecraft/
		versionLocator: the Version Locator // by default it will be new JVersionLocator()
	));

```

__VB__
```vb
Dim core = LauncherCore.Create(
    New LauncherCoreCreationOption(
        javaPath:=Config.Instance.JavaPath, ' by default it will be the first version finded
        gameRootPath:=Nothing, ' by defualt it will be ./.minecraft/
        versionLocator:=theVersionLocator ' by default it will be New JVersionLocator()
    ))
```

## How to find Versions

__C#__
```csharp

var versions = core.GetVersions();

var version = core.GetVersion("1.8");

```

__VB__
```vb

Dim versions = core.GetVersions

Dim version = core.GetVersion("1.8")

```

*unlaunchable Version will be ignored*

## How to launch Minecraft

__C#__
```csharp
var result = core.Launch(new LaunchOptions
{
	Version = App.LauncherCore.GetVersion(server.VersionId),
	Authenticator = new OfflineAuthenticator("Steve"), // offline
	//Authenticator = new YggdrasilLogin("*@*.*", "***", true), // online
	MaxMemory = Config.Instance.MaxMemory, // optional
	MinMemory = Config.Instance.MaxMemory, // optional
	Mode = LaunchMode.MCLauncher, // optional
	Server = new ServerInfo {Address = "mc.hypixel.net"}, //optional
	Size = new WindowSize {Height = 768, Width = 1280} //optional
}, (Action<MinecraftLaunchArguments>) (x => { })); // optional ( modify arguments before launching
```

__VB__
```vb
Dim result = core.Launch(New LaunchOptions With
{
    .Version = App.LauncherCore.GetVersion(server.VersionId),
    .Authenticator = New OfflineAuthenticator("Steve"), ' offline. New YggdrasilLogin("*@*.*", "***", True), for online scenario.
    .MaxMemory = Config.Instance.MaxMemory, ' optional
    .MinMemory = Config.Instance.MaxMemory, ' optional
    .Mode = LaunchMode.MCLauncher, ' optional
    .Server = New ServerInfo With {.Address = "mc.hypixel.net"}, 'optional
    .Size = New WindowSize With {.Height = 768, .Width = 1280} 'optional
}, Sub(x As MinecraftLaunchArguments)
   End Sub) ' optional ( modify arguments before launching
```

## Using anonymous report ##

__C#__
```csharp
Reporter.SetClientName("Your launcher's name"); // set name
Reporter.SetReportLevel(ReportLevel.Full); // full report
//Reporter.SetReportLevel(ReportLevel.Basic); // basic report
//Reporter.SetReportLevel(ReportLevel.Min); // simplified report
//Reporter.SetReportLevel(ReportLevel.None); // turn off
```

__VB__
```vb
Reporter.SetClientName("Your launcher's name") ' set name
Reporter.SetReportLevel(ReportLevel.Full) ' full report
'Reporter.SetReportLevel(ReportLevel.Basic) ' basic report
'Reporter.SetReportLevel(ReportLevel.Min) ' simplified report
'Reporter.SetReportLevel(ReportLevel.None) ' turn off
```

# Enjoy!
