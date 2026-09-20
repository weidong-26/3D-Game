# 第二轮 Bug 修复与测试报告

日期：2026-09-19。工程路径：`E:\游戏`。编辑器：Unity `6000.0.40f1`。本报告区分源码检查、自动化验证和仍需人工验收的内容。

## 动手前检查

| 检查项 | 结果 |
| --- | --- |
| Unity 源工程 | `Assets/`、`Packages/`、`ProjectSettings/` 均存在，故可继续编辑；不是仅有 exe 的 Build。 |
| Unity 版本 | `ProjectSettings/ProjectVersion.txt` 为 `6000.0.40f1`，本机同版本编辑器可编译和构建。 |
| 结构 | `Assets/Scenes/Main.unity` 是很小的启动场景；世界、角色、镜头和 UI 主要由 `Assets/Scripts/Runtime/` 在运行时创建。`Assets/Scripts/Core/` 存配置及存档数据；`Assets/Editor/` 是构建代码。项目有 Git 仓库，本次继续开发前的提交为 `3eebefc`。 |
| 待处理问题 | 用户反馈了镜头、跳跃、地图和人物表现等问题。检查当前提交后发现，其中若干功能已有初步实现；本轮继续修复细节、扩展验证。人物质量、复杂碰撞和异机运行仍未达到完整验收。 |
| 可直接修复 | 基于现有源码修复输入/碰撞/重生，扩充程序化场景与交互，提供 Unity 原生捏人 MVP，并重新构建。 |
| 需要补充 | 若要求正式 Tafi 接入，仍需当前 SDK、文档及商业授权；若要求高品质真人角色，仍需有明确商用许可的模型、骨骼、材质和动作资源。Tafi 调查见 `THIRD_PARTY_LICENSES.md`。 |

`ENVIRONMENT_STATUS.md` 是先前环境状态记录，不是本轮操作指令。本轮以实际工程文件、Git 差异和 Unity 运行结果为准。

## 分阶段修改与验证

涉及文件及与当前 `HEAD` 的实际差异见 `CHANGELOG_ROUND2.md`；下表给出各阶段的检查结果，包含本轮开始时已经在提交中的基础功能。

| 阶段 | 完成的检查 | 结果与范围 |
| --- | --- | --- |
| 1 工程 | 检查源目录、版本、场景及既有脚本；运行原有基线验证 | 源工程齐全；基线 3/3 核心测试及 Unity 编译通过。 |
| 2 控制 | 先写运动规则测试，再实现；运行 6/6 规则测试与 Unity 编译 | 相机水平/俯仰角计算、常规跳与一次空中跳、重生阈值、`R` 返回出生点在代码及运行冒烟中通过。玩家碰撞器配置了坡度、台阶和皮肤宽度。实际复杂斜坡、贴墙等手感未充分人工测试。 |
| 3 生活场景 | Unity 编译；运行时查找房屋地面、家具、楼梯、道路和碰撞器 | 住宅内外、庭院、街道、围栏、树及基础照明实例化成功。大多为基础网格，装饰和生活感仍有限。 |
| 4 交互 | Unity 编译；独立 Player 中依次测试杯子、箱子、手电筒 | 拾取、手持、放下及手电筒开关通过；杯子持有时刚体设为运动学、碰撞关闭，放下后恢复。长时间物理稳定性仍需游玩测试。开门/坐下/睡觉只是后续扩展点，未作为已完成功能。 |
| 5 捏人 | 6/6 核心测试；独立 Player 检查 22 个滑块、保存、进入场景和重开界面；再以两个进程验证存档 | 首进程写入隔离的外貌文件，第二进程成功读取并验证创建界面初值，之后清理测试文件。普通冒烟会还原原有玩家存档。脸部形变和身体/发色/衣着参数为可扩展 MVP；视觉精细度不足。 |
| 6 人物动画 | 生成动画控制器前的资源检查失败，生成后 `ANIMATION_STATES_PASS 7/7`；Unity 编译及 Build 成功 | Idle、Walk、Run、Jump、Fall、Land、Hold 七个状态和交叉切换资源存在。当前是变换层级的程序化人物及动作，没有正式蒙皮骨骼和高品质动作，**未达到真人角色质量目标**。 |
| 7 综合 | `Tools/verify-phase1.ps1`、Windows Build、独立 Player 冒烟和跨进程存档测试 | 下述命令及日志显示通过。本机 Player 使用 Direct3D 11 启动；未在另一台无 Unity 编辑器的 Windows 电脑上实测。 |

## 最终可复现证据

在项目根目录执行：

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Tools\verify-phase1.ps1 -UnityEditor 'E:\Unity\Hub\Editor\6000.0.40f1\Editor\Unity.exe'
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Tools\Build-Windows.ps1 -UnityEditor 'E:\Unity\Hub\Editor\6000.0.40f1\Editor\Unity.exe'
```

- `verify-phase1.ps1` 最终退出码为 0：核心测试 6/6、运动规则测试 6/6、Editor 构建脚本测试 2/2、Unity 批处理编译通过。
- `Build-Windows.ps1` 最终退出码为 0，输出 `BUILD_SUCCESS`。`Builds/Windows/` 包含 exe、对应 Data、`UnityPlayer.dll`、`MonoBleedingEdge/`、`D3D12/` 和 CrashHandler。构建文件共 134 个，约 85.2 MB；`3DGamePrototype.exe` SHA-256 为 `399F876D64A9C14FE63DFDE277461F37A0F234A42A5359ACBFCBD040BF7BD09A`。
- 最新独立 Player 在有图形设备的本机以 `-batchmode -prototype-smoke` 运行，退出码 0，日志 `Logs/player-round2-graphics-smoke.log` 记录 `WORLD_INTERACTION_SAVE_RESPAWN_PASS` 和 `PROTOTYPE_SMOKE_PASS`。该测试调用镜头输入函数检查向上、向下与水平环绕；调用跳跃/拾放/重生流程检查状态，不等于人工操作鼠标、键盘的完整体验测试。
- 独立 Player 先后以 `-prototype-persist-write` 和 `-prototype-persist-read` 两次启动，退出码均为 0，日志分别记录 `PERSIST_WRITE_PASS` 和 `PERSIST_RESTART_PASS`；隔离测试存档已删除。日志位于 `Logs/player-round2-prototype-persist-write.log`、`Logs/player-round2-prototype-persist-read.log`。
- `Logs/creator-smoke.png`、`Logs/gameplay-smoke.png` 是本机此前抓取的纯场景/人物画面，不能证明 UI 排版，也清楚显示目前模型和建筑是简陋的原型美术。
- `Builds/Deliverables/3DGamePrototype-Source-Round2.zip` 包含 `Assets/`、`Packages/`、`ProjectSettings/`、`Tests/`、`Tools/`、文档和日志；`3DGamePrototype-Windows-Round2.zip` 包含运行文件。已逐个解压读取 ZIP 内文件并核对必需目录/文件；这只是压缩包完整性检查，不等于异机运行验证。

## 未通过完整验收的要求

1. **角色质量**：程序化形体虽有人体比例、脸部滑块和七个动作状态，但视觉仍偏简陋，未达到“自然、干净的真人风格”及正式骨骼/材质/动画质量。需要授权明确的人物及动作资源，或者专门的建模绑定工作。
2. **物理和操作手感**：自动冒烟覆盖核心状态转换，尚未完成在各种帧率、窗口分辨率、台阶斜坡、墙角、室内镜头遮挡下的人工长时间游玩。不能据此声称卡墙、穿地和所有镜头穿模场景已彻底消除。
3. **异机兼容**：本机独立 Windows Player 确实启动并跑完测试，但本机同时装有 Unity Editor；尚未在不安装 Unity 的另一台 Windows 电脑上实测。Build 文件齐全仅证明可分发结构，不能替代该测试。
4. **Tafi**：历史上有运行时 Astra SDK，但现行获取方式、Unity 版本、费用、联网与导出/商用条款未确认，因此没有接入。当前游戏内捏人采用 Unity 原生实现。
5. **家具行为**：床、椅子、门等目前仅是场景道具，坐下、睡觉、开门尚无玩家交互；昼夜循环也未实现，当前只有基础日间灯光。

下一步应先完成授权明确的高品质角色/动作资源与骨骼绑定，再进行实际键鼠游玩调参与异机 Windows 验证；如需 Tafi，先取得现行 SDK 与许可条款，再做最小接入实验。
