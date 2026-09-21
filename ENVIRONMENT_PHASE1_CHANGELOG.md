# 住宅环境升级第一阶段变更记录

日期：2026-09-21  
分支：`environment-upgrade`  
Unity：6000.0.40f1  
渲染管线：URP 17.0.4

## 已完成

- 保留原场景 `Assets/Scenes/Main.unity`，新增 `Assets/Scenes/Home_Environment_V2.unity`。
- 建立可进入的现代住宅：完整一层、二层、客厅、卧室、厨房区、走廊、楼板、墙体、天花板、门窗、楼梯、护栏和室外安全地面。
- 将项目切换到 URP，增加住宅专用材质、全局渲染设置、室内灯光和 Poly Haven 2K HDRI。
- 导入 7 个 Poly Haven CC0 家具模型及 3 套 ambientCG CC0 PBR 材质。模型、贴图、Prefab、碰撞体和来源记录分目录保存。
- 建立统一 `Interactable` 流程，住宅内实现门、抽屉、椅子、床和拾取物。鼠标按钮可执行交互，E 键保留为快捷方式。
- 修复拾取物在父节点切换时累计缩放的问题。拾取时保存世界缩放、原父节点、刚体和碰撞状态，放下时恢复。
- 摄像机改为按住鼠标右键拖动，默认显示鼠标；保留原有第一/第三人称相关代码，不重写角色控制。
- 增加住宅场景跌落复位点、简单碰撞体、连续楼梯碰撞面和 Windows 构建入口。
- 增加自动运行版验收脚本、资产导入审计、批准资产注册表、截图和性能记录。

## 主要代码与配置

- `Assets/Scripts/Interaction/HomeInteractable.cs`
- `Assets/Scripts/Interaction/HomeEnvironmentSmoke.cs`
- `Assets/Editor/HomeEnvironmentBuilder.cs`
- `Assets/Editor/HomeAssetBuilder.cs`
- `Assets/Editor/HomeAssetAudit.cs`
- `Assets/Editor/EnvironmentSetup.cs`
- `Assets/Editor/WindowsBuild.cs`
- `Assets/Scripts/Runtime/GameBootstrap.cs`
- `Assets/Scripts/Runtime/RuntimeFactory.cs`
- `Packages/manifest.json`
- `ProjectSettings/GraphicsSettings.asset`
- `ProjectSettings/QualitySettings.asset`
- `ProjectSettings/InputManager.asset`
- `ProjectSettings/EditorBuildSettings.asset`

## 资产管线

- 场景引用 `Assets/Prefabs/` 下的批准 Prefab，不直接引用下载目录中的原始 FBX。
- `Assets/Art/Approved/registry.yaml` 记录运行时 Prefab、来源、许可证和四项 QA 状态。
- `Assets/Art/Approved/import_report.txt` 记录模型尺寸、三角面、材质、UV、法线和 URP 检查结果。
- 下载 URL 与 MD5 位于 `Assets/Art/Furniture/PolyHaven/download_manifest.csv`。
- 完整来源和许可证见 `THIRD_PARTY_ASSETS.md`。

## 本阶段范围

本阶段按任务书要求停在住宅环境。学校、公园和城市没有开始制作；捏脸、正式人物模型和角色控制架构没有改动。

