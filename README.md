# 3D 轻量游戏原型（第一阶段）

这是一个 Unity 6.0 LTS 项目，只实现第一阶段：固定拓扑的程序化角色、基础捏脸、参数持久化、第三人称移动和小型物理测试地图。项目没有加入联机、AI 生成、战斗、商城、剧情、AI NPC 或复杂动画。

## 打开与运行

1. 安装 Unity Hub 和 Unity **6000.0（Unity 6.0 LTS）**，建议使用 `6000.0.40f1` 或同系列更新版本。
2. 在 Unity Hub 中选择“添加/打开磁盘中的项目”，目录选择本项目根目录。
3. 打开 `Assets/Scenes/Main.unity`，点击 Play。
4. 首屏调整外貌，点击 **Save & Play** 保存并进入测试地图。

项目只使用 Unity 自带能力和运行时生成的占位几何体，没有需要单独授权或下载的角色素材。

## 生成可双击运行版本

首次构建需要在 Unity Hub 中安装：

- Unity Editor `6000.0.40f1`
- `Windows Build Support` 模块

安装完成后，在项目根目录打开 PowerShell 并运行：

```powershell
.\Tools\Build-Windows.ps1
```

如果 Unity 安装在自定义目录，可显式指定：

```powershell
.\Tools\Build-Windows.ps1 -UnityEditor "D:\Unity\6000.0.40f1\Editor\Unity.exe"
```

构建脚本会把 `Assets/Scenes/Main.unity` 设为唯一启动场景，生成并检查以下运行文件：

```text
Builds/Windows/3DGamePrototype.exe
Builds/Windows/3DGamePrototype_Data/
Builds/Windows/UnityPlayer.dll
```

构建成功后，直接双击 `Builds/Windows/3DGamePrototype.exe` 即可游玩，不需要再打开 Unity。请保留同目录下的 `3DGamePrototype_Data`、`UnityPlayer.dll` 及 Unity 生成的其他文件；不要只复制 exe。

## 操作

- `WASD`：行走
- `Left Shift`：奔跑
- `Space`：跳跃
- 鼠标：旋转镜头
- 滚轮：调整镜头距离
- `E`：抓取/放下准星指向的刚体
- `Esc`：释放鼠标；单击游戏画面重新锁定

角色碰到橙色刚体会推动它们。地图内提供方块、球体、胶囊体和堆叠方块用于抓取、推动与碰撞测试。

## 捏脸与保存

头部由 `ProceduralHeadMesh` 每次按同一段数和同一顶点顺序生成，拓扑保持一致。网格包含 10 个实际 BlendShape：

- FaceWidth、FaceLength
- ChinWidth、ChinLength
- EyeSize、EyeSpacing
- NoseSize、NoseWidth
- MouthSize、LipThickness

眼睛、鼻子和嘴唇的外部占位几何体会同步变化，以便在轻量原型中更清楚地看到参数效果。肤色和 3 种程序化发型也可实时预览。

保存文件位于 `Application.persistentDataPath/appearance.json`。其中只包含 11 个 0–1 参数与一个发型编号，不保存或复制整个 3D 模型。下次启动时会先读取并恢复这些参数。

## 项目目录

```text
Assets/
  Editor/WindowsBuild.cs           Windows 64 位编辑器构建入口
  Scenes/Main.unity               空启动场景
  Scripts/Core/                   不依赖 Unity 的外貌参数核心
  Scripts/Runtime/                角色、UI、移动、镜头、物理与启动逻辑
Packages/manifest.json            Unity UI 依赖
ProjectSettings/                  Unity 版本、构建场景和旧输入轴配置
Tests/CoreTests.cs                可独立运行的核心回归测试
Tools/verify-phase1.ps1           核心测试、结构检查及可选 Unity 批处理编译
Tools/Build-Windows.ps1            固定输出路径的 Windows 构建与产物检查
```

## 关键脚本

- `GameBootstrap.cs`：进入场景后组装第一阶段全部运行时对象，并切换捏脸/游玩状态。
- `AppearanceData.cs`：唯一的外貌数据结构、默认值、参数归一化和随机外貌。
- `ProceduralHeadMesh.cs`：生成固定拓扑头模与 10 个 BlendShape。
- `ProceduralAvatarFactory.cs`：生成统一基础人物和 3 种发型占位体。
- `CharacterAppearance.cs`：把参数映射到 BlendShape、五官、肤色和发型。
- `CharacterCreatorUI.cs`：实时滑杆、随机、默认和保存按钮。
- `ThirdPersonController.cs` / `ThirdPersonCamera.cs`：行走、奔跑、跳跃和镜头旋转。
- `PhysicsGrabber.cs`：准星射线抓取和释放刚体。
- `AppearanceSaveService.cs`：把参数以 JSON 保存并在启动时恢复。
- `WindowsBuild.cs`：固定启动场景并调用 Unity 的 Windows 64 位构建管线。
- `Build-Windows.ps1`：定位指定 Unity 版本、检查 Windows Build Support、执行构建并核对运行文件。

## 验证

在 PowerShell 中运行：

```powershell
.\Tools\verify-phase1.ps1
```

如果 Unity 不在默认安装目录，可指定编辑器：

```powershell
.\Tools\verify-phase1.ps1 -UnityEditor "D:\Unity\6000.0.40f1\Editor\Unity.exe"
```

脚本会运行参数核心测试、使用本地 API 桩做全部运行时脚本的语法/类型检查并核对必要项目文件；找到 Unity 时还会执行一次真实的批处理导入和编译，并把日志写入 `Logs/unity-compile.log`。

## 第一阶段完成情况

- [x] Unity 3D 项目骨架和启动场景
- [x] 第三人称行走、奔跑、跳跃、镜头旋转
- [x] 固定拓扑统一人物基础模型
- [x] 10 个五官 BlendShape / Morph 参数
- [x] 肤色与 3 种发型
- [x] 实时预览、滑杆、随机、恢复默认、保存 UI
- [x] 保存后进入简单 3D 测试地图
- [x] 可推动、抓取和碰撞的物理物体
- [x] 参数化 JSON 保存与重启恢复逻辑
- [ ] 成品美术模型、骨骼动画与正式动作资源（不属于本阶段）
- [ ] 多人、AI 自动捏脸、战斗、商城、剧情、AI NPC、大地图（明确未开发）

## 下一阶段建议

保持现有 `AppearanceData` 作为稳定数据协议，下一阶段优先替换程序化占位角色为一套拥有相同 BlendShape 名称的合法正式模型，并加入基础 Idle/Walk/Run/Jump 动画状态机。完成美术替换和移动手感验收后，再讨论联机同步或 AI 自动捏脸；两者都只同步/生成参数，不传输模型文件。

`AppearanceData` 应继续作为外貌数据的唯一来源。新增玩法时应在 `Assets/Scripts/` 下建立独立脚本或功能文件夹，通过小型入口接入，避免把状态、输入、UI 和玩法逻辑继续堆入 `GameBootstrap`。
