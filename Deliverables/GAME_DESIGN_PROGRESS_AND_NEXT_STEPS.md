# Unity 自由探索切片：人物、环境与玩法进度

更新日期：2026-10-12  
工程：`E:\游戏`  
当前分支：`environment-phase3`，HEAD `88cd609`。工作区已有大量未提交改动；本报告按当前工作区静态检查，未重置、覆盖或整理这些改动。**本轮未运行代码测试。**

## 结论摘要

项目已经从基础第三人称原型发展为四张可进入的环境、第三人称移动、物件操作和家具交互原型。当前体验最接近“玩家挑选一个地点，自由走动并试用环境中的物件或人物”。它还没有明确的任务、成长或剧情循环；按用户确认的自由探索方向，这些暂不作为切片前置条件。

目前的关键整合问题是角色创建器与新校园人物没有接通：新模型有 Humanoid 骨架和测试动画，但没有可用的 BlendShape 通道；导入角色分支中的 `CharacterAppearance.Apply` 会因缺少原程序化头部引用而提前返回。因此外观参数能进入保存流程，却不会改变新模型。角色定制目标可以保留，但需要模型侧提供对应形变或可替换部件/材质后再完成映射。

环境侧已具备空间和可交互物件，四张地图目前是由地图选择界面串联的独立场景。历史截图显示它们仍处于功能原型阶段：场景辨识度和可探索内容密度需要继续建设。下一步应给每张地图明确一个探索焦点、一条短路线和一个代表性交互，并让它们共同支撑“选地点—探索—互动—换地点”的轻量循环。

## 人物设计与接入进度

| 内容 | 当前情况 | 证据与边界 |
|---|---|---|
| 玩家外观创建 | 原程序化角色有 Face、Body、Style 三页，共 22 个参数；外观 JSON 有保存/读取和旧数据补齐逻辑。 | `CharacterCreatorUI.cs`、`AppearanceSaveService.cs`、`TEST_REPORT_ROUND2.md`。这些旧测试不能证明参数已作用于新角色。 |
| 新主角模型 | `character_campus_front_humanoid.fbx` 已作为 Humanoid 接入 Profile。验收报告记录 52 根骨骼、52 个 bind pose、83,529 个加权顶点，模型高约 1.74 m。 | `Deliverables/CharacterP3/CHARACTER_P3_ACCEPTANCE_REPORT.md`；测试是在隔离副本完成的导入与驱动检查。 |
| BlendShape 盘点 | 现有 FBX 未发现标准 `BlendShape`、`BlendShapeChannel` 或 `DeformPercent` 节点字符串；原始模型的 Unity 导入报告也记录 `blendShapes=0`。新 FBX 的 `.meta` 中 `importBlendShapes: 1` 只表示允许导入，不代表文件里存在形变。当前可用形变名称为 **0 个**；工程没有保存新 FBX 的 Unity `Mesh.blendShapeCount` 输出，后续资源验收时应再做一次导入器级确认。 | `Logs/CharacterCampusImportReport.txt` 对应原始静态网格；新模型文件与 `Assets/Art/Characters/character_campus_front_humanoid.fbx.meta` 已只读检查。 |
| 定制器与新模型 | 导入分支只创建 `CharacterAppearance`，没有给它初始化程序化头部、身体或材质对象；`Apply` 的 `head == null` 条件会直接返回。脸部、肤色、发型、体型、服装等滑块目前都没有新模型映射。 | `Assets/Scripts/Runtime/ProceduralAvatarFactory.cs`、`CharacterAppearance.cs`、`CharacterCreatorUI.cs`。这是对当前源码的静态判断。 |
| 动画与动作 | 已接入 Locomotion Blend Tree 和 Jump、Fall、Land、Interact、Hold 等状态；Character P3 报告覆盖 8 个基础测试片段，确认能驱动蒙皮。动画由简易关键帧生成，属于状态和绑定验证素材。 | `CharacterMotionAnimator.cs`、`CharacterCampusAnimator.controller`、Character P3 验收报告。未完成动作美术评审。 |
| 人工验收 | 报告明确标记最终绑骨角色在原工程 Game View 的键鼠、外观、脚底与物理交互验收未执行。 | 不把原程序化角色或隔离副本的按键结果算作新角色验收。 |
| 资源来源记录 | 工程内 `THIRD_PARTY_LICENSES.md` 日期为 2026-09-19，记录当时没有第三方人物；校园人物于 10-11 后进入工程。Obsidian 中 10-11 的找回记录称模型由混元 3D 流程生成，但工程内的资源来源/使用条款记录尚未同步更新。 | 发布或分享前应补足模型来源与可用范围；目前不据旧许可表推断新人物的权利状态。 |

### 角色后续准备

1. 在 Unity 导入后记录新 FBX 的 `Mesh.blendShapeCount` 和全部名称，形成可复查的正式盘点结果。
2. 由于当前盘点为 0，保留捏人目标需要模型提供相应形变。脸部滑块需要可对应的头部 BlendShape；肤色、发色、发型、服装和体型还需要可独立控制的材质槽、网格变体或骨骼/比例方案。
3. 为现有 22 个参数逐项写明“可由现有模型支持 / 需要新增资源 / 不进入本轮切片”。没有视觉反馈的参数不得标成已完成。
4. 角色资源接通后，人工检查预览与游戏内外观一致、保存后重载一致，以及新 Animator 下移动、跳跃、持物、交互和坐/躺姿势。

## 玩法设计与当前闭环

已经出现的可玩系统包括地图选择、第三人称行走/奔跑/跳跃、第三人称镜头、门和柜体操作、拾取/放置、座椅/床姿态、掉落复位，以及当前工作区中的 NPC 简短对白。`GameBootstrap` 按场景锚点生成静态 NPC，`NpcInteractable` 显示一段问候；这不代表寻路、日常活动或关系系统已经实现。

适合当前自由探索方向的切片循环建议为：

**角色创建并保存 → 选择地图 → 从出生点沿一条短路线到达可识别焦点 → 发现并操作一个环境物件或 NPC → 返回地图选择并进入另一处。**

该循环使用已有功能，不需要任务奖励或成长系统。现在的地图选择是不同场景之间的入口；场景之间没有连续地理连接、时间推进或共享世界状态，设计文档也没有规定这些系统。

工程配置与说明文档有一处入口不一致：`ProjectSettings/EditorBuildSettings.asset` 的首个构建场景为 `Environment_Select_V2`，而 `README.md` 仍把 `Main.unity` 描述为启动场景和首屏捏人入口。下一次文档整理时应以实际 Player 流程校准 README。

## 四张环境的进度与设计角色

环境阶段报告证明了场景结构和交互可用性；以下地图角色是针对“自由探索”提出的设计建议，不是已有定稿设定。

| 地图 | 已有空间与路线 | 已有交互/焦点 | 建议的自由探索角色 | 当前短板 |
|---|---|---|---|---|
| 住宅 | 两层住宅，含客厅、卧室、厨房区、走廊、楼梯及室外落脚区。 | 门、抽屉、椅子、床、拾取与放置；楼梯和跌落复位有历史验收。 | 玩家熟悉的室内起点与生活物件探索。路线可串起入口、客厅/厨房和卧室。 | 外部庭院内容较稀；截图中的房屋与大面积草地更像功能样板，生活痕迹和外部焦点有限。 |
| 学校 | 一个教室、一段走廊和室外落脚区；配置学生桌椅与教师桌。 | 教室门、坐下/起身、拾取和课桌放置；最新工作区代码还加入学生/老师静态问候点。 | 室内空间与人物相遇的探索点。路线可由入口经过走廊进入教室。 | 规模仍是一间教室和一段走廊；2026-09-22 的截图不能代表 10-11 后已修改的学校场景。 |
| 公园 | 草地、十字步道、中央广场、池塘、休息区和凉棚。 | 长椅坐下、拾取/放置；有中心广场和池塘作为焦点。 | 开阔户外漫游与休息。路线可沿步道环绕广场、池塘和长椅。 | 2026-09-21 截图里草坪空间很空、物件分布稀疏；Phase 3 报告还记录较高三角面数，后续需要按统一方法复测性能。 |
| 城市 | 十字道路、人行道、斑马线和街区外立面。 | 一个示例门；当前工作区代码增加店员对白。 | 街角观察与建筑入口探索。路线可经过路口、立面和示例店门。 | 主要是外立面，没有建筑内部和车辆；历史截图中的街区细节和可进入内容有限。 |

### 环境视觉证据

Phase 2 截图日期为 2026-09-21，Phase 3 截图日期为 2026-09-22。代表画面包括 `Deliverables/EnvironmentPhase2/Screenshots/01_SchoolClassroom.png`、`04_ParkOverview.png`、`08_UrbanStreet.png` 和 `Deliverables/EnvironmentPhase1/Screenshots/01_Exterior.png`。这些截图能证明当时的场景构成与大致视觉状态，不能用来确认当前工作区所有改动后的最终画面。截图中住宅外部、教室墙面、公园空地和城市街道均以结构和动线为主，建议下一轮先统一四图的光照/材质基调，并为每图建立一个远近都容易识别的焦点。

环境测试记录中，Phase 2 公园约 1.4M 三角面、学校约 609K、城市约 641K，采样为 1280×720 VSync Game View；Phase 3 报告的公园约 2.60M 三角面，来自显式离屏渲染。两组采样方法不同，不能直接据此判断性能回退。Phase 2 记录四图约 60 FPS，设备为 Intel UHD Graphics；它是历史单机短测，不代表目标设备范围。

### 每图可复用的切片路线草案

- **住宅：**前门 → 客厅/厨房 → 卧室；操作一个门或抽屉，再坐下/躺下或拾放物品。
- **学校：**入口门 → 走廊 → 教室；坐到学生椅或把物件放到课桌，并与一个静态 NPC 交谈。
- **公园：**出生点 → 中央广场/池塘 → 长椅；沿步道移动，坐下或拾放物件。
- **城市：**出生点 → 路口 → 示例商店门；操作门并与店员交谈，再返回地图选择。

这些路线先用现有物件形成可验证的探索体验。若实机试玩发现路线空、焦点不明显，再决定需要补充的环境内容，不预先扩大地图范围。

## 下一阶段工作顺序

1. **记录工作区基线。**保留当前 `environment-phase3` 未提交的角色、动画、摄像机、NPC、场景和工程设置改动；后续实现前按文件核对哪些是当前在制内容，哪些已有隔离测试证据。
2. **完成捏人与模型的资源契约。**正式确认新 FBX 的 BlendShape 数量/名称；把 22 项旧参数映射到可见效果或列为资源待办，保持外观存档兼容。
3. **确定四图身份与动线。**为住宅、学校、公园、城市各写一句体验定位、一个焦点、一条短路线和一个必验交互；标注哪些来自现有资产，哪些需要新增。
4. **整合自由探索切片。**让玩家创建并保存外观、进入地图、完成每图路线、使用交互并能返回地图选择。NPC 继续作为静态对话点，除非后续另行确定 AI 范围。
5. **执行最终人工验收。**在实际 Unity Game View/Windows Player 中检查新角色外观、创建器反馈和存档、移动/镜头/动作、每图交互以及跨地图返回流程；留存本次切片的新截图和日志。
6. **同步项目说明。**按最终可验证状态更新 README 的启动流程、人物能力和地图说明；补录新角色来源与使用范围。性能数据在同一设备、分辨率和采样方法下重新采集。

## 验收清单（下一阶段）

- [ ] BlendShape 数量与名称有 Unity 导入器级记录；每个启用的捏人参数在预览和游戏中都有可见效果，保存/重载一致。
- [ ] 新绑骨角色的方向、比例、脚底位置、材质和碰撞胶囊在 Game View 中确认。
- [ ] WASD、奔跑、跳跃、镜头环绕/缩放、持物和交互在最终角色上人工通过。
- [ ] 四张地图均可从选择界面进入；每张都有一个可辨认焦点、一条短探索路线和至少一个可用交互。
- [ ] NPC 能显示当前设定的静态对白；不把该能力写成 AI 行为。
- [ ] 能从地图返回选择界面并进入另一张地图；全流程截图/日志对应最终工作区状态。
- [ ] README、角色来源记录和本报告中的状态一致。

## 依据文件

- `README.md`、`TEST_REPORT_ROUND2.md`、`THIRD_PARTY_LICENSES.md`
- `Deliverables/EnvironmentPhase1/ENVIRONMENT_PHASE1_CHANGELOG.md`、`ENVIRONMENT_PHASE1_TEST_REPORT.md`
- `ENVIRONMENT_PHASE2_CHANGELOG.md`、`ENVIRONMENT_PHASE2_TEST_REPORT.md`
- `Deliverables/EnvironmentPhase3/ENVIRONMENT_PHASE3_CHANGELOG.md`、`ENVIRONMENT_PHASE3_TEST_REPORT.md`
- `Deliverables/CharacterP3/CHARACTER_P3_ACCEPTANCE_REPORT.md`
- 当前代码：`Assets/Scripts/Runtime/GameBootstrap.cs`、`ProceduralAvatarFactory.cs`、`CharacterAppearance.cs`、`CharacterMotionAnimator.cs`、`NpcInteractable.cs`、`PlayerVisualProfile.cs`；构建场景顺序：`ProjectSettings/EditorBuildSettings.asset`
- Unity 导入记录：`Logs/CharacterCampusImportReport.txt`；角色模型：`Assets/Art/Characters/character_campus_front_humanoid.fbx`
- 环境历史截图：`Deliverables/EnvironmentPhase1/Screenshots/`、`Deliverables/EnvironmentPhase2/Screenshots/`、`Deliverables/EnvironmentPhase3/Screenshots/`
