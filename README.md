KMCCC
=====

[![Join the chat at https://gitter.im/MineStudio/KMCCC](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/MineStudio/KMCCC?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/ldvo1wsyd66boxsf?svg=true)](https://ci.appveyor.com/project/zhouyiran2/kmccc)

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

## KMCCC.Basic

Basic Version of KMCCC

### Included:

- Everything in kMCCC.Shared
- No more

## KMCCC.Pro

Professional Version of KMCCC

### Included:

- Everything in KMCCC.Shared
- [WIP] Game File Download & Fix & Check (Including Version, Library, Native&Assets)
- [WIP] More useful extensions for LaunchHandle
- [WIP] More useful libraries that a launch might need.

## Plan

We plan to delay some parameter settings, (such as versionLocator) and make more features available in KMCCC.Pro

# Sample

## How to initialize a LauncherCore

```csharp

LauncherCore core = LauncherCore.Create(
	new LauncherCoreCreationOption(
		javaPath: Config.Instance.JavaPath, // by default it will be the first version finded
		gameRootPath: null, // by defualt it will be ./.minecraft/
		versionLocator: the Version Locator // by default it will be new JVersionLocator()
	));

```

## How to find Versions

```csharp

var versions = core.GetVersions();

var version = core.GetVersion("1.8");

```

*unlaunchable Version will be ignored*

## How to launch Minecraft


```csharp
var result = core.Launch(new LaunchOptions
{
	Version = App.LauncherCore.GetVersion(server.VersionId)
	Authenticator = new OfflineAuthenticator("Steve"), // offline
	//Authenticator = new YggdrasilLogin("*@*.*", "***", true), // online
	MaxMemory = Config.Instance.MaxMemory, // optional
	MinMemory = Config.Instance.MaxMemory, // optional
	Mode = LaunchMode.MCLauncher, // optional
	Server = new ServerInfo {Address = "mc.hypixel.net"}, //optional
	Size = new WindowSize {Height = 768, Width = 1280} //optional
}, (Action<MinecraftLaunchArguments>) (x => { })); // optional ( modify arguments before launching
```

# Enjoy!
