# 开发环境状态（ENVIRONMENT_STATUS.md）

生成时间：2026-09-18 21:15（GMT+8）
范围：**仅环境搭建与验证**。未修改游戏逻辑、场景内容或运行时脚本。

---

## 一、已安装软件

| 软件 | 版本 | 安装位置 |
| --- | --- | --- |
| Unity Hub | 3.21.3 | `E:\Unity\Unity Hub\Unity Hub.exe` |
| Unity CLI (Beta) | 1.0.0-beta.8 | `C:\Users\牛伟东1\AppData\Local\Unity\bin\unity.exe` |
| Unity Editor | **6000.0.40f1** (x86_64) | `E:\Unity\Hub\Editor\6000.0.40f1\Editor\Unity.exe` |

- 编辑器安装根目录已设为 `E:\Unity\Hub\Editor`。
- 编辑器磁盘占用约 **8.8 GB**。
- 版本与 `ProjectSettings/ProjectVersion.txt` 声明的 `6000.0.40f1` 完全一致（未升级、未降级）。

## 二、模块

| 模块 ID | 名称 | 状态 | 磁盘占用 |
| --- | --- | --- | --- |
| windows-il2cpp | Windows Build Support (IL2CPP) | **已安装** | 1.39 GB |

`Editor/Data/PlaybackEngines/windowsstandalonesupport` 内含完整变体：
`mono`、`il2cpp`、win32/win64/arm64 × development/nondevelopment（mono + il2cpp）。
即 `Tools\Build-Windows.ps1` 所检查的模块路径存在且完整。

## 三、许可证

- 产品：**Unity Personal**（类型 Assigned），账号 牛伟 `niuw76160@gmail.com`。
- 构建与批处理编译均已在有效许可下实际跑通（见下）。

## 四、验证结果

| 验证项 | 结果 | 证据 |
| --- | --- | --- |
| Unity Hub / CLI 能识别 Unity | 通过 | `unity editors` 列出 `6000.0.40f1 → E:\Unity\Hub\Editor\6000.0.40f1\Editor\Unity.exe` |
| 项目能正常打开（导入 + 编译） | 通过 | 批处理导入生成 19 个 `.meta`，`AssetDatabase` 刷新完成，脚本编译 6.5s，**0 个 `error CS`**，退出码 0 |
| 无缺失模块 / 无关键报错 | 通过 | 补齐 `Packages/manifest.json` 后编译 0 错误 |
| 项目自带验证脚本 | 通过 | `Tools\verify-phase1.ps1`：核心测试 3/3、语法/类型检查、构建脚本单测 2/2、Unity 批处理编译全部 PASS，退出码 0 |
| Windows 构建环境可用 | 通过 | `Builds\Windows\3DGamePrototype.exe` 生成成功，报告 `totalSize = 85173362 bytes`，退出码 0 |

构建产物（`E:\游戏\Builds\Windows`，合计约 82 MB）：

```
3DGamePrototype.exe          672 KB
3DGamePrototype_Data/        110 个文件
UnityPlayer.dll              33.6 MB
MonoBleedingEdge/
D3D12/
UnityCrashHandler64.exe
```

## 五、仍存在的问题 / 必须注意

1. **无头批处理必须加 `-DisableDirectoryMonitor`（最关键）**
   不加该参数时 Unity 会永久卡在 `Application.AssetDatabase Initial Refresh Start`（进程空转、不生成 `.meta`、日志不再增长）。本机文件监视器在此环境下不可用。已内建到 `Tools\Build-Windows.ps1` 与 `Tools\verify-phase1.ps1`；若手写命令，务必带上。

2. **PowerShell 执行策略默认禁止运行脚本**
   需 `-ExecutionPolicy Bypass`，或在会话内先执行 `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force`。

3. **Unity Hub 自带 CLI 不可用**
   `"Unity Hub.exe" -- --headless ...` 在本机因 GPU 进程崩溃（`GPU process isn't usable. Goodbye.`）无法工作，且要求 `ELECTRON_RUN_AS_NODE` 为空。改用 Unity CLI (Beta) 完成安装。不影响 Hub 图形界面正常使用。

4. **Licensing Client 签名校验警告（无害）**
   日志中会出现 `Code 10 while verifying Licensing Client signature` / `LicensingClient has failed validation; ignoring`，随后仍打印 `Successfully resolved entitlement details`。编译与构建均正常，属噪音级警告。

5. **代理依赖**
   环境变量 `HTTP_PROXY` / `HTTPS_PROXY` = `http://127.0.0.1:7897/`。包解析与下载依赖该代理；代理未启动时 `com.unity.*` 包可能长时间等待。

6. **CLI 侧登录态 `auth.sessionState: stale`**
   仅影响 Unity CLI 的云端功能，不影响编辑器/构建。如需恢复，执行 `unity auth login`。

7. **无 GPU 加速**，批处理建议固定使用 `-nographics`。

8. `check.windows-long-paths: warn`（未开启长路径支持），当前项目路径长度未受影响。

9. 未做游戏运行（Play）验证——本任务只覆盖环境与构建链路。

10. **验证期间检测到另一进程在并发修改本项目**
    时间线上出现 `Logs\player-smoke-*.log`（20:53–21:00，运行构建产物做冒烟测试），以及 20:54 新增 `Assets/Resources/PrototypeColor.shader`、修改 `Assets/Scripts/Runtime/RuntimeFactory.cs`。同一时刻曾触发 `Fatal Error! It looks like another Unity instance is running with this project open.` 崩溃。
    → 结论：**多个 Agent / 多个 Unity 实例不得同时操作本项目**，构建前请确认无残留 `Unity.exe` / `UnityPackageManager.exe` / `bee_backend.exe`。
    → 本文件记录的验证结论取自对方改动**之后**的状态，仍然有效。

## 六、为适配本环境所做的改动（仅工具与包配置，未改游戏逻辑）

| 文件 | 改动 | 原因 |
| --- | --- | --- |
| `Packages/manifest.json` | 新增 `com.unity.modules.audio`、`com.unity.modules.jsonserialize`、`com.unity.modules.physics` | 原文件仅有 `com.unity.ugui`，编译报 `CS1069`（PhysicsModule / AudioModule）与 `CS0103`（JsonUtility）；这三个内置模块被源码实际使用 |
| `Tools/Build-Windows.ps1` | 批处理参数新增 `-nographics -DisableDirectoryMonitor` | 否则构建卡死在 AssetDatabase 刷新 |
| `Tools/verify-phase1.ps1` | 同上新增两个参数；默认编辑器查找由硬编码 `C:\Program Files\Unity\Hub\Editor` 改为遍历各盘 `Program Files\Unity\Hub\Editor` 与 `Unity\Hub\Editor` | 编辑器装在 E 盘，原脚本找不到；查找逻辑与 `Build-Windows.ps1` 对齐 |

`Assets/`、`Tests/`、`ProjectSettings/` 未作任何改动。

## 七、给后续接手的说明（重要）

- 环境**已完整可用，不要重新安装** Unity Hub / 编辑器 / 模块。
- 需要复现构建：在项目根目录用 Bypass 策略运行 `.\Tools\Build-Windows.ps1` 即可（脚本已含必需参数）。
- 若出现「另一个 Unity 实例正在使用该项目」类崩溃，先确认没有残留的 `Unity.exe` / `UnityPackageManager.exe` / `bee_backend.exe` 进程。
- 日志位置：`Logs\windows-build.log`（构建）、`Logs\unity-compile.log`（批处理编译）。
