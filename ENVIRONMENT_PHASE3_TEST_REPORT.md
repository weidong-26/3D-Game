# Environment Phase 3 Test Report

## 1. 测试环境

- Unity：6000.0.40f1 (157d81624ddf)
- 渲染管线：Universal Render Pipeline
- Git 分支：`environment-phase3`
- Phase 2 基线：`d44344ffdb60f5563751e00cc8abbfaf07278573`
- Phase 3 实现提交：`15807ff1135626b4ffb627b039adc70775ad7524`
- Windows 构建：`Builds/Windows/EnvironmentPhase3.exe`

## 2. 验收结果

| 序号 | 验收项 | 结果 | 证据 |
| --- | --- | --- | --- |
| 1 | Unity 版本、分支与提交 | 通过 | 本报告测试环境；分支从 Phase 2 提交直接创建 |
| 2 | Windows Player 可启动并独立运行 | 通过 | `Logs/Phase3WindowsBuildFinal5.log`：`PHASE3_WINDOWS_BUILD_PASS`；Player 退出码 0 |
| 3 | 学校：开门、坐下/起身、拿取、桌面放置、离开 | 通过 | `PHASE3_SCHOOL_PASS`、`PHASE3_PLACEMENT_PASS StudentDesk_1` |
| 4 | 住宅：门、柜子、座椅、床、拿取/放置 | 通过 | `PHASE3_HOME_PASS`；住宅回归日志中的门、柜、座椅/床、箱子标记均通过 |
| 5 | 公园：长椅、拾取/放置、返回 | 通过 | `PHASE3_PARK_PASS`、`PHASE3_PLACEMENT_PASS PicnicTable` |
| 6 | 城市：行走基础、示例门、返回 | 通过 | `PHASE3_URBAN_PASS` |
| 7 | Pickup 连续 10 次尺寸不变 | 通过 | `PHASE3_PICKUP_10_CYCLES_PASS` |
| 8 | 住宅楼梯上下楼 | 通过 | `HOME_STAIR_UP_DOWN_PASS topY=2.84 topZ=9.22 bottomY=0.22` |
| 9 | 掉出地图自动复位 | 通过 | 住宅、公园、城市分别输出 `PHASE3_RESPAWN_PASS` |
| 10 | 场景切换和返回地图选择 | 通过 | `PHASE3_SCENE_SWITCH_PASS`；四张地图均完成加载 |
| 11 | 统一交互契约 | 通过 | `PHASE3_CONTRACT_TEST_PASS` |
| 12 | 场景组件配置 | 通过 | `PHASE3_SCENE_CONTRACT_PASS` |
| 13 | Phase 2 地图回归 | 通过 | `PHASE2_SMOKE_PASS` |
| 14 | 住宅旧功能回归 | 通过 | `HOME_SMOKE_PASS`、`HOME_BASIC_CHECKS_PASS`、`HOME_WALL_CAMERA_PASS` |
| 15 | Console Error / Exception | 通过 | 最终构建、契约测试、场景测试、Phase 2/3 Player 日志未发现阻断性 Error 或 Exception |

## 3. Windows Player 测试链路

最终 Player 使用 D3D11 图形模式实际运行，脚本化执行以下链路：

1. 地图选择进入学校，开门、坐椅、起身、拿取物品并放到课桌。
2. 切换住宅，测试抽屉、床、放置、掉落复位，并单独执行住宅门/柜/座椅/床/箱子 10 次/楼梯回归。
3. 切换公园，测试长椅、物品放置和掉落复位。
4. 切换城市，测试基础交互和掉落复位。
5. 返回地图选择，验证场景切换后交互仍有效。

最终主要日志：

- `Logs/Phase3PlayerSmokeFinal2.log`
- `Logs/Phase2RegressionFinal2.log`
- `Logs/Phase3HomeRegressionSmoke2.log`
- `Logs/Phase3ContractFinal.log`
- `Logs/Phase3SceneContractFinal.log`
- `Logs/Phase3PlayerCaptureFinal.log`

## 4. 性能记录

以下数据来自 Windows Player 在 1280×720 下对场景执行 120 帧 URP 显式离屏渲染。它用于本次构建内部横向检查，不与 Phase 2 的 VSync Game View 统计直接比较。

| 场景 | 显式渲染吞吐 | Draw Calls | Triangles |
| --- | ---: | ---: | ---: |
| School_Environment_V1 | 345.4 FPS | 198 | 328,060 |
| Home_Environment_V2 | 685.0 FPS | 116 | 64,906 |
| Park_Environment_V1 | 343.7 FPS | 207 | 2,595,499 |
| Urban_Environment_V1 | 589.9 FPS | 446 | 903,499 |

公园可见三角面较高，延续 Phase 2 已记录的 LOD 优化项；本阶段没有为此重建公园。

## 5. 已知问题

- 当前人物骨架和动画资源有限，坐姿/躺姿使用现有 Pose 占位，位置与退出流程可用，但视觉动作仍较生硬。
- 公园几何量仍高，后续应按现有资产补 LOD 和遮挡剔除，不属于本阶段范围。
- 城市只提供一个可扩展门交互示例，未制作大规模建筑室内。
- 自动化在实际 Windows Player 中完成了功能链路；鼠标手感、门体速度与座椅视觉舒适度仍建议由验收人员主观试玩确认。

## 6. 未完成范围

NPC、复杂 AI、战斗、联网、商城、剧情、完整背包、完整建造系统、大规模城市室内均未开始。

## 7. 人物与捏脸系统

未修改人物外观、程序化头像、捏脸 UI、外观保存格式或 Blend Shape 逻辑。

