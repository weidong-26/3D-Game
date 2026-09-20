# 3DGamePrototype 第二轮可玩原型

这是可继续编辑的 Unity 源工程。编辑器版本固定为 **Unity 6000.0.40f1**；启动场景是 `Assets/Scenes/Main.unity`。住宅、角色和界面在运行时由脚本建立，因此场景文件本身很小。当前版本完成了控制、住宅场景、物品交互和 Unity 原生捏人 MVP；角色美术仍是程序化原型，详见 `TEST_REPORT_ROUND2.md`。

## 打开和运行

1. 用 Unity Hub 添加本目录 `E:\游戏`，选择 Unity `6000.0.40f1`。
2. 打开 `Assets/Scenes/Main.unity` 并按 Play。首屏是捏人界面；可以在 Face、Body、Style 页面调节，右键拖动旋转角色，滚轮缩放，点 **Save & Play** 进入场景。
3. Windows 运行包位于 `Builds/Windows/`。双击 `3DGamePrototype.exe`。运行时须保留同目录的 `3DGamePrototype_Data/`、`UnityPlayer.dll`、`MonoBleedingEdge/`、`D3D12/` 和 `UnityCrashHandler64.exe`。

可转移的交付压缩包位于 `Builds/Deliverables/`：`3DGamePrototype-Source-Round2.zip` 包含完整可编辑源码和测试材料；`3DGamePrototype-Windows-Round2.zip` 包含全部 Windows 运行文件。两包分别解压，源码包用 Unity Hub 打开，运行包直接启动 exe。

## 按键

| 按键 | 功能 |
| --- | --- |
| `WASD` | 行走 |
| `Left Shift` | 奔跑 |
| `Space` | 跳跃；空中再按一次二段跳 |
| 鼠标 | 第三人称镜头水平环绕、向上和向下观察 |
| 滚轮 | 调整镜头距离；捏人界面缩放预览 |
| `E` | 拾取或放下杯子、手电筒、箱子 |
| `F` | 手持手电筒时开关灯 |
| `R` | 返回出生点 |
| `C` | 在游玩时重新打开捏人界面 |
| `Esc` | 释放鼠标；单击游戏画面重新锁定 |

移动参数集中在 `Assets/Scripts/Runtime/ThirdPersonController.cs` 的 Inspector 序列化字段，包括行走/奔跑速度、加速度、转向、跳跃高度、空中跳跃次数、重力和重生高度。低于重生高度会自动返回安全点。镜头角度、距离和遮挡球半径在 `ThirdPersonCamera.cs` 配置。

## 角色创建

本项目使用本地 Unity 运行时捏人界面，不依赖 Tafi。头部为固定拓扑网格及 BlendShape，另有体型、身高、肩宽、腿长、肤色、发型、发色和三种衣服配色。角色参数保存在 `Application.persistentDataPath/appearance.json`；旧版参数文件会补齐本轮新增字段。进入场景后按 `C` 可以再次修改。该角色仍是原型美术，不是高精度扫描人像或正式骨骼资产。

Tafi 曾发布支持游戏内创建的 Astra SDK，但本工程没有获得其当前 SDK、接入文档或授权，因此没有集成 Tafi。调查与授权待确认事项见 `THIRD_PARTY_LICENSES.md`。

## 重新验证和构建

在 PowerShell 中于项目根目录运行：

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Tools\verify-phase1.ps1 -UnityEditor 'E:\Unity\Hub\Editor\6000.0.40f1\Editor\Unity.exe'
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Tools\Build-Windows.ps1 -UnityEditor 'E:\Unity\Hub\Editor\6000.0.40f1\Editor\Unity.exe'
```

构建脚本会检查 Windows Build Support、Unity 版本和完整运行文件。可在构建程序后加 `-prototype-smoke` 运行内部冒烟检查并自动退出；实际结果见 `TEST_REPORT_ROUND2.md`。批处理需保留 `-DisableDirectoryMonitor`，本机否则会卡在资源刷新。

## 文件与状态

- `Assets/Scripts/Core/`：外貌数据、存档兼容和运动规则。
- `Assets/Scripts/Runtime/`：角色、镜头、控制、住宅、交互、捏人 UI 与运行冒烟检查。
- `Assets/Resources/PrototypeAnimator.controller` 和 `PrototypeAnimations/`：七个原型动作状态及片段。
- `Assets/Editor/`：动画资源生成/检查和 Windows 构建入口。
- `CHANGELOG_ROUND2.md`：各阶段修改文件清单。
- `TEST_REPORT_ROUND2.md`：Bug、测试证据、未完成项及下一步。
- `THIRD_PARTY_LICENSES.md`：第三方资源和 Tafi 授权调查。
