# 环境第二阶段文件清单

## 场景

- `Assets/Scenes/Environment_Select_V2.unity`
- `Assets/Scenes/School_Environment_V1.unity`
- `Assets/Scenes/Park_Environment_V1.unity`
- `Assets/Scenes/Urban_Environment_V1.unity`

## 运行时代码

- `Assets/Scripts/Interaction/EnvironmentSelectionUI.cs`
- `Assets/Scripts/Interaction/EnvironmentNavigation.cs`
- `Assets/Scripts/Interaction/EnvironmentPhase2Smoke.cs`
- `Assets/Scripts/Runtime/GameBootstrap.cs`（只增加新环境识别和场景加载钩子）

## 编辑器与构建代码

- `Assets/Editor/EnvironmentPhase2Builder.cs`
- `Assets/Editor/EnvironmentPhase2AssetAudit.cs`
- `Assets/Editor/HomeAssetBuilder.cs`（增加环境模型和 Alpha/PBR 材质导入）
- `Assets/Editor/WindowsBuild.cs`（增加第二阶段多场景构建入口）
- `Tools/download_phase2_assets.ps1`
- `Tools/download_phase2_materials.ps1`

## 资源、预制体与审计

- `Assets/Art/EnvironmentPhase2/PolyHaven/`：16 个模型资源的 FBX/JPG 和下载清单。
- `Assets/Art/EnvironmentPhase2/ambientCG/`：5 套 1K PBR 材质。
- `Assets/Art/Approved/EnvironmentPhase2/registry.yaml`
- `Assets/Art/Approved/EnvironmentPhase2/import_report.txt`
- `Assets/Art/Materials/Generated/P2_*`：URP 材质及打包贴图。
- `Assets/Art/Buildings/Meshes/P2_*`：场景模块网格。
- `Assets/Prefabs/EnvironmentPhase2/`：16 个源模型预制体和 4 个批准立面模块。
- `Assets/Prefabs/Buildings/SchoolEnvironment.prefab`
- `Assets/Prefabs/Buildings/ParkEnvironment.prefab`
- `Assets/Prefabs/Buildings/UrbanEnvironment.prefab`
- `Assets/Prefabs/Buildings/Modules/P2_*`

## 项目设置

- `ProjectSettings/EditorBuildSettings.asset`：地图选择、住宅、学校、公园、城市和旧主场景。

## 交付文件

- `ENVIRONMENT_PHASE2_CHANGELOG.md`
- `ENVIRONMENT_PHASE2_TEST_REPORT.md`
- `ENVIRONMENT_PHASE2_FILE_LIST.md`
- `ENVIRONMENT_PHASE2_THIRD_PARTY_ASSETS.md`
- `Deliverables/EnvironmentPhase2/Screenshots/*.png`
- `Deliverables/EnvironmentPhase2/performance.txt`
- `Deliverables/EnvironmentPhase2/EnvironmentPhase2-Windows.zip`
- `Deliverables/EnvironmentPhase2/EnvironmentPhase2-UnitySource.zip`
- `Deliverables/EnvironmentPhase2/SHA256SUMS.txt`

