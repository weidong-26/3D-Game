# 住宅环境升级第一阶段文件清单

## 新增场景与运行时代码

- `Assets/Scenes/Home_Environment_V2.unity`
- `Assets/Scripts/Interaction/HomeInteractable.cs`
- `Assets/Scripts/Interaction/HomeEnvironmentSmoke.cs`

## 新增编辑器工具

- `Assets/Editor/EnvironmentSetup.cs`
- `Assets/Editor/HomeAssetBuilder.cs`
- `Assets/Editor/HomeEnvironmentBuilder.cs`
- `Assets/Editor/HomeAssetAudit.cs`

## 新增美术与 Prefab

- `Assets/Art/Buildings/`
- `Assets/Art/Environment/`
- `Assets/Art/Furniture/PolyHaven/`
- `Assets/Art/Materials/`
- `Assets/Art/Approved/`
- `Assets/Prefabs/Buildings/`
- `Assets/Prefabs/Furniture/`
- `Assets/Prefabs/Interactables/`

## 修改的现有文件

- `Assets/Editor/WindowsBuild.cs`
- `Assets/Scripts/Runtime/GameBootstrap.cs`
- `Assets/Scripts/Runtime/RuntimeFactory.cs`
- `Packages/manifest.json`
- `Packages/packages-lock.json`
- `ProjectSettings/EditorBuildSettings.asset`
- `ProjectSettings/GraphicsSettings.asset`
- `ProjectSettings/InputManager.asset`
- `ProjectSettings/QualitySettings.asset`

## 新增配置、记录与工具

- `Assets/DefaultVolumeProfile.asset`
- `Assets/UniversalRenderPipelineGlobalSettings.asset`
- `ProjectSettings/SceneTemplateSettings.json`
- `ProjectSettings/ShaderGraphSettings.asset`
- `Tools/download_home_assets.ps1`
- `Tools/download_home_materials.ps1`
- `THIRD_PARTY_ASSETS.md`
- `ENVIRONMENT_PHASE1_CHANGELOG.md`
- `ENVIRONMENT_PHASE1_TEST_REPORT.md`
- `ENVIRONMENT_PHASE1_FILE_LIST.md`
- `Deliverables/EnvironmentPhase1/`

所有 Unity 资源同时包含对应 `.meta` 文件。原场景与原有资源未删除。

