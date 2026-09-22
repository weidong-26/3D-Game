# Environment Phase 3 Changelog

## 基线

- Unity：6000.0.40f1，URP
- 分支：`environment-phase3`
- Phase 2 基线：`d44344ffdb60f5563751e00cc8abbfaf07278573`
- Phase 3 实现提交：`15807ff1135626b4ffb627b039adc70775ad7524`

## 已完成

### 统一环境交互

- 新增 `IEnvironmentInteractable`、`InteractionDetector` 和单例式 `InteractionPromptUI`。
- 统一由玩家侧 `InteractionController` 检测目标和处理 E/F 输入。
- 没有目标或超出范围时隐藏提示，目标变化时同步高亮和操作文案。
- 使用共享 `MaterialPropertyBlock` 高亮，不为每个物体实例化材质。

### 门、柜子、座椅与床

- 新增可复用的 `InteractableDoor` 和 `InteractableCabinet`，以协程插值完成开关，并带触发冷却。
- 新增 `SeatInteractable` 和 `LieInteractable`，每个 Prefab 使用自身 Seat/Lie/Exit 点，不依赖世界坐标。
- 已接入学校教室门与椅子、住宅门/抽屉/座椅/床、公园长椅、城市示例门。

### 拾取与放置

- 新增 `PickupInteractable` 和 `PlacementSurface`。
- 拾取时保存父节点、世界缩放、刚体和碰撞状态；放置时恢复世界尺寸和物理状态。
- 放置使用前方射线、允许放置表面和占用空间检测。
- 学校课桌、住宅地板、公园地面/广场/棋桌已配置放置面。

### 玩家安全与场景接入

- 新增 `DefaultSpawnPoint` 与 `PlayerRespawnSystem`；掉到 Kill Height 以下自动回到场景出生点。
- 学校、住宅、公园、城市均配置默认出生点。
- 保留 Phase 2 四张地图、返回地图选择和原住宅楼梯结构。
- 新增 Phase 3 Windows 构建入口、契约测试、场景配置测试、Windows Player 冒烟与截图工具。

## 未触及范围

- 未修改人物外观、捏脸数据、Blend Shape 或角色生成流程。
- 未增加 NPC、战斗、联网、商城、任务、完整背包、完整建造系统或大规模城市室内。
- 本阶段没有新增第三方美术或代码资产。

