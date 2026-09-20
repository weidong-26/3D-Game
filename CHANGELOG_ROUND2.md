# 第二轮修改清单

基线检查日期：2026-09-19。项目有 Git 仓库，当前 `HEAD` 为 `3eebefc`。下表列出第二轮功能涉及的主要文件；其中部分功能和资源在此提交中已存在，本次继续开发的实际工作区差异应以 `git status` / `git diff` 为准。

| 阶段 | 涉及文件 | 内容 |
| --- | --- | --- |
| 1 工程检查 | 未改源文件 | 确认 `Assets/`、`Packages/`、`ProjectSettings/` 齐全，Unity `6000.0.40f1`；`Main.unity` 是运行时组装入口。 |
| 2 控制与镜头 | `Assets/Scripts/Core/MovementRules.cs`、`Assets/Scripts/Runtime/ThirdPersonController.cs`、`ThirdPersonCamera.cs`、`GameBootstrap.cs`、`Tests/MovementRulesTests.cs` | 集中移动参数、加速度、二段跳、镜头上下限与障碍物避让、碰撞设置、坠落重生、`R` 返回出生点。 |
| 3 住宅场景 | `Assets/Scripts/Runtime/TestWorldFactory.cs` | 建立住宅内外、门窗、楼梯、家具、庭院、街道、树、围栏与基础灯光；大多数可接触物有碰撞体。 |
| 4 物品 | `Assets/Scripts/Runtime/PickupItem.cs`、`PhysicsGrabber.cs`、`ProceduralAvatarFactory.cs`、`GameBootstrap.cs` | 杯子、箱子、手电筒；准星或近距离检测、提示、`E` 拾放、手部挂点、持有时关闭碰撞与重力、`F` 手电筒开关。 |
| 5 捏人 | `Assets/Scripts/Core/AppearanceData.cs`、`Assets/Scripts/Runtime/AppearanceSaveService.cs`、`CharacterAppearance.cs`、`CharacterCreatorUI.cs`、`CharacterPreviewCamera.cs`、`ProceduralHeadMesh.cs`、`GameBootstrap.cs`、`Tests/CoreTests.cs` | Face/Body/Style 三页参数、颧骨/眉毛/体型/比例/发色/服装配色、预览旋转与缩放、旧存档升级、`C` 重开编辑。 |
| 6 角色表现 | `Assets/Scripts/Runtime/ProceduralTorsoMesh.cs`、`ProceduralAvatarFactory.cs`、`CharacterMotionAnimator.cs`、`Assets/Resources/PrototypeColor.shader`、`Assets/Editor/PrototypeAnimationGenerator.cs`、`AnimationAssetChecks.cs`、`Assets/Resources/PrototypeAnimator.controller`、`PrototypeAnimations/*.anim` | 形体细化、基础受光材质、七个 Animator 状态与交叉淡入。仍需正式角色网格、骨骼绑定和高质量动作。 |
| 7 验证与构建 | `Assets/Scripts/Runtime/PrototypeSmoke.cs`、`Tests/UnityStubs.cs`、`Tools/verify-phase1.ps1`、`Packages/manifest.json`、`Packages/packages-lock.json`、`README.md`、本清单及测试/许可证报告、`Builds/Windows/` | 扩展自动测试、编译、Windows Build 和独立运行的冒烟检查。 |

Unity 在导入新增源码和资源时自动生成对应 `.meta` 文件。`Library/`、`Logs/` 是本机缓存与测试证据；继续编辑工程主要需要 `Assets/`、`Packages/`、`ProjectSettings/`。

相对于 `3eebefc`，本次继续开发实际修改了 `PrototypeColor.shader`、`AppearanceSaveService.cs`、`CharacterCreatorUI.cs`、`GameBootstrap.cs`、`ProceduralAvatarFactory.cs`、`PrototypeSmoke.cs`、`TestWorldFactory.cs`、`ThirdPersonCamera.cs`、`ThirdPersonController.cs`、`Packages/manifest.json`、`Packages/packages-lock.json`、`README.md`、`Tests/UnityStubs.cs`；新增了 `ProceduralTorsoMesh.cs` 及其 `.meta`、本清单、测试报告和第三方许可清单。Windows Build 和测试日志为忽略的生成文件，没有提交到 Git。
