# 住宅环境升级第一阶段测试报告

日期：2026-09-21  
测试对象：`Builds/Windows/HomeEnvironmentV2.exe`  
设备：Intel(R) UHD Graphics，Direct3D 11，1280×720

## 验收结果

| 项目 | 结果 | 证据 |
| --- | --- | --- |
| Windows 构建 | 通过 | `Logs/HomeWindowsBuildFinal.log`：`Build Successful`，135,883,467 bytes |
| 住宅可进入 | 通过 | 入口、门、地面与出生点均在独立运行版加载 |
| 一层、二层存在 | 通过 | 结构 Collider 自动检查及截图 01、03、04 |
| 楼梯正常上下 | 通过 | `HOME_STAIR_UP_DOWN_PASS topY=2.84 topZ=9.22 bottomY=0.22` |
| 墙体、楼板、天花板完整 | 通过 | 关键结构 Collider 自动检查；外墙阻挡测试通过 |
| 门可开关 | 通过 | 鼠标 UI Button 连续执行开门、关门：`HOME_DOOR_INTERACTION_PASS` |
| 抽屉可开关 | 通过 | 鼠标 UI Button 连续执行开、关：`HOME_DRAWER_INTERACTION_PASS` |
| 椅子可坐下、起身 | 通过 | 姿态与控制器状态检查：`HOME_SEAT_BED_INTERACTION_PASS` |
| 床可躺下、起身 | 通过 | 姿态与控制器状态检查：`HOME_SEAT_BED_INTERACTION_PASS` |
| 拾取缩放不变 | 通过 | 同一箱子连续拾取、放下 10 次，逐次检查世界缩放、父节点、Rigidbody、重力和 Collider：`HOME_BOX_10_CYCLES_PASS` |
| 摄像机可上下观察 | 通过 | 俯仰边界反射测试及实际运行观察：`HOME_WALL_CAMERA_PASS` |
| 不穿外墙 | 通过 | CharacterController 向外墙移动后位置未越界 |
| Console/Player 红色运行错误 | 通过 | Direct3D 11 日志 `Logs/HomePlayerSmokeGPUFinal.log` 与 `Logs/HomeCaptureGPUFinal2.log` 无 `ERROR`、Exception 或 `HOME_*_FAIL` |
| 资产导入审计 | 通过 | `HOME_ASSET_AUDIT_PASS 7 CC0 models, 6 interactables, 1 house prefab` |

独立运行版总结果：`HOME_SMOKE_PASS`，退出码 0。

## 性能快照

数据来自可见 Windows Player 的 120 个 Game 帧；测试时 UI 启用，角色 Renderer 已恢复启用。

- 平均 FPS：60.1（受 VSync 限制）
- Draw Calls：108
- Triangles：85,391
- 分辨率：1280×720
- 采样视角：卧室抽屉交互视角

原始记录：`Deliverables/EnvironmentPhase1/performance.txt`。

## 截图

- `01_Exterior.png`：住宅整体
- `02_LivingRoom.png`：客厅与厨房区
- `03_Stairs.png`：楼梯与护栏
- `04_SecondFloor.png`：二层走廊与房间结构
- `05_BedAndFurniture.png`：床与抽屉柜
- `06_DoorInteraction.png`：门交互按钮
- `07_DrawerInteraction.png`：打开后的抽屉与关闭按钮

目录：`Deliverables/EnvironmentPhase1/Screenshots/`。

## 已知限制

- 本轮没有修改捏脸系统、正式人物模型或角色控制架构；这些属于住宅环境阶段之外的工作。
- 住宅结构使用项目制作的模块化网格，近景家具使用 CC0 模型；美术质量属于可玩原型升级，尚未达到最终商业场景精修级别。
- 性能数据是单台设备、单一视角的短时快照，不代表所有硬件和所有视角。
- Blender MCP 已调用，但本机 Blender 插件未启动，服务返回无法连接；因此没有把 Blender MCP 结果写成资产验收证据。当前会话没有暴露 CoplayDev Unity MCP 接口。Unity 验证由 Unity 6 编辑器批处理和独立 Windows Player 完成。
- `unity-asset-pipeline` 已用于 Prefab、URP 材质、碰撞体、导入报告和批准注册表。当前注册表是项目使用的 v1 实用格式，未宣称完成完整 `asset-contract.yaml v2` 的全部 BeautyCell/变形测试流程。

## 下一阶段建议（未执行）

先在住宅场景增加烘焙光照、反射探针、窗帘与小型生活道具，再以相同 Prefab 与交互规范制作 `School_Environment_V1`。本轮未自动开始学校、公园或城市。

