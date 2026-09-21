# 环境第二阶段测试报告

日期：2026-09-21  
测试对象：`Builds/Windows/EnvironmentPhase2.exe`  
分辨率：1280 × 720  
图形设备：Intel UHD Graphics，Direct3D 11

## 自动验收

| 项目 | 结果 | 证据 |
| --- | --- | --- |
| Unity 脚本编译与 Windows StrictMode 构建 | 通过 | `Logs/Phase2WindowsBuildFinal.log`：`Build Successful`、`PHASE2_WINDOWS_BUILD_PASS` |
| 学校场景加载、玩家、摄像机、返回按钮 | 通过 | `PHASE2_SCENE_PASS School_Environment_V1` |
| 学校教室、走廊、门、桌椅、交互与关键地面 | 通过 | 对象、Collider、Interaction 及 3 个地面采样点通过 |
| 公园场景、步道、广场、池塘、家具与关键地面 | 通过 | `PHASE2_SCENE_PASS Park_Environment_V1` |
| 城市场景、道路、人行道、立面与关键地面 | 通过 | `PHASE2_SCENE_PASS Urban_Environment_V1` |
| 四张地图卡片 | 通过 | `PHASE2_SELECTOR_PASS`，按钮数不少于 4 |
| URP 激活 | 通过 | 三个场景均检查 `GraphicsSettings.currentRenderPipeline` |
| 批准资源导入审计 | 通过 | 16 个 CC0 源模型、4 个派生立面模块、3 个环境预制体、4 个场景 |
| 独立播放器 Console | 通过 | 最终 Smoke 与 Capture 日志无运行时 Exception、NullReference 或 Error |

最终自动验收结论：`PHASE2_SMOKE_PASS`，进程退出码 0。

## 冻结住宅回归

使用重新构建的 `HomeEnvironmentV2.exe` 运行原住宅自动验收：

- 门开关：通过。
- 抽屉开关：通过。
- 椅子坐下/起身、床躺下/起身：通过。
- 箱子连续拾取/放下 10 次，世界缩放和刚体/碰撞状态恢复：通过。
- 相机墙体遮挡：通过。
- 楼梯自动上楼和返回：通过，最高 `Y=2.84`，返回 `Y=0.22`。
- 结论：`HOME_SMOKE_PASS`，进程退出码 0。

## Game 视图性能

数据由独立 Windows 玩家每场景采样 120 帧，VSync 目标 60 FPS：

| 场景 | FPS | Draw Calls | Triangles |
| --- | ---: | ---: | ---: |
| School_Environment_V1 | 60.0 | 416 | 609,303 |
| Park_Environment_V1 | 60.0 | 74 | 1,401,501 |
| Urban_Environment_V1 | 60.0 | 315 | 641,193 |

原始记录：`Deliverables/EnvironmentPhase2/performance.txt`。

## 截图检查

已从独立玩家采集 10 张 1280 × 720 PNG：教室、桌椅、走廊、公园全景、广场、池塘、城市路口、街道、公寓立面、地图选择。

人工检查结果：模型朝向正确；树木和路灯已按人物尺度调整；城市不再显示整套模块资源板；地面没有可见缺口。学校局部顶灯仍可见柔和光斑，属于当前实时点光源方案。

## 已知限制

- 公园树木源模型细节较高，当前公园约 140 万可见三角面；本机仍保持 60 FPS，低端独显或核显需按目标平台再次验证。
- 城市建筑只完成可行走街区和外立面，没有室内、车辆和 NPC。
- 学校完成一个教室和一段走廊，没有扩展为完整校园。
- Blender MCP 无法连接到正在运行的 Blender 插件端；CoplayDev Unity MCP 未出现在本次可调用工具中。最终验证使用 Unity 6000.0.40f1 编辑器批处理和实际 Windows 玩家完成。
- 当前资源注册表记录了来源、许可证、预制体和实际 Unity 导入检查，但不等同于 `unity-asset-pipeline` 的完整 v2 资产合同与 BeautyCell 基准审批；工程缺少其所需的 `art-spec.yaml`、`composition.yaml` 和批准参考帧。
