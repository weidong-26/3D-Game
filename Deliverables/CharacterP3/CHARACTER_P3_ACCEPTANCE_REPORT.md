# Unity 角色 P3 验收报告

日期：2026-10-11  
工程：`E:\游戏`  
Unity：6000.0.40f1

## 结论

第一阶段输入故障已修复并通过实际 Play Mode 输入检查。前视副本已自动绑骨，Unity Humanoid Avatar、视觉 Prefab、Profile 和基础 Animator 状态已在隔离副本中导入并验证。项目自带的 Phase 3 场景/交互冒烟流程通过。

最终绑骨角色的 WASD、Space、鼠标等人工按键验收尚未在原工程窗口执行；原 Unity 编辑器的场景带有既存未保存标记，自动化环境也无法操作该窗口。本报告将这部分标为“未执行”，不把原模型的输入结果冒充成新模型的按键验收。

## 状态清单

| 项目 | 状态 | 证据/边界 |
|---|---|---|
| Input Manager / Submit 报错 | 通过 | 保留 Legacy 输入架构，只补 `Submit`（Return / joystick button 0）和 `Cancel`（Escape / joystick button 1）。修复后 Play Mode 冒烟日志无 Submit 配置异常。 |
| 程序化人物移动与跳跃 | 通过 | 实际 Play Mode：W 移动到墙体后停止；Shift 跑速约 6 m/s；Space 跳高约 1.329 m 并回到地面。 |
| 原静态角色视觉下的输入 | 通过 | W 前进、S 后退、D 转向；A 在切换英文输入法并聚焦 Game View 后移动约 4.587 m。Space 跳高约 1.272 m，回到 Y≈0.050 并 grounded。右键鼠标转动相机；E 打开 ClassroomDoor，提示文字随门状态变化。 |
| 碰撞胶囊与地面 | 通过 | `CharacterController` 高 2 m、半径 0.34 m、中心 Y=1 m；墙体碰撞有效。`Time.timeScale=1`。Creator 页面禁用移动控制器是现有流程；进入世界后启用。 |
| AccuRig / Blender | 通过 | 本机无 AccuRig。用临时目录中的 Blender 4.5.14 与其自带 Rigify 完成；没有上传模型、登录或购买资源。 |
| 前视副本自动绑骨 | 通过 | 原 FBX 的左侧正面副本单独提取；A-Pose 适配。52 根骨骼，含躯干、四肢和手指。83,529 个顶点全部有权重，0 个未绑定，最多 8 个顶点影响。手臂、下肢和髋部旋转均造成网格形变。 |
| Unity Humanoid / Skinned Mesh | 通过 | 新 FBX 导入类型为 Humanoid，Avatar `isHuman=True`、`isValid=True`；1 个 SkinnedMeshRenderer，52 根骨骼、52 个 bind pose。 |
| 新 Prefab 与 Profile | 通过 | 新 Prefab 上的 Animator 使用新 Avatar 与 `CharacterCampusAnimator`；`PlayerVisualProfile` 已指向新 Prefab。GameBootstrap 在 School 场景实例化的 Player 能找到头部相机锚点、右手锚点与 Humanoid 骨骼。 |
| Idle / Walk / Run / Jump / Fall / Land / Interact / Hold | 通过（基础测试动作） | 8 个片段均可从新 Animator 播放并驱动网格。Speed Blend Tree 阈值为 0 / 3.5 / 6；状态名与 `CharacterMotionAnimator` 使用的名称一致。 |
| 场景交互、拾取与放置 | 通过（自动冒烟） | 项目自带 Phase 3 smoke 在隔离副本中通过 School、Home、Park、Urban 与场景切换；日志含 `PHASE3_PICKUP_10_CYCLES_PASS`、`PHASE3_INTERACTION_PASS`。该 smoke 直接调用现有交互 API，不等同于人工按 E 拾取验收。 |
| 最终绑骨角色的人工键鼠验收 | 未执行 | 未在原工程窗口用最终 FBX 实际按 WASD、Space、E/F、鼠标。需按下方手动步骤完成。 |
| Unity Game View 最终外观截图检查 | 未执行 | 材质已指定为现有 `character_campus_pbr`；未在原 Unity 窗口进行最终屏幕级外观确认。 |

## 绑骨与动画结果

原始 FBX 有三个互不连接的视角副本；按用户选择只绑定左侧正面副本。源文件 SHA-256 为 `90136F4B7AC581AAD32C1DBC4B3EB87C289DDDC551C060BF3295E5A7827D6550`，工程内原件与检查点哈希一致，没有覆盖。

新模型在 Unity 中烘焙检查的几何尺寸约为宽 1.04 m、高 1.74 m、深 0.42 m，最低点约 Y=0 m；现有 2 m 碰撞胶囊无需改动。Blend Tree 使用 Idle、Walk、Run；Jump、Fall、Land、Interact、Hold 为独立状态。Idle、Walk、Run、Fall、Hold 循环，Jump、Land、Interact 不循环。

动作由 Blender 中的简易测试关键帧生成，不是动作捕捉或精修成品。Play Mode 检查中每个状态都切换到预期片段，网格最大顶点位移均大于 0：Idle 0.0038 m、Walk 0.1692 m、Run 0.3731 m、Jump 0.5592 m、Fall 0.4453 m、Land 0.1229 m、Interact 0.5047 m、Hold 0.5404 m。这些数值用于确认动作确实驱动蒙皮，不评价动作美术质量。

## 修改与新增文件

- 修改 `ProjectSettings/InputManager.asset`：新增 Submit、Cancel 两个按钮映射，原轴配置保留。
- 修改 `Assets/Scripts/Runtime/ProceduralAvatarFactory.cs`：只有 Animator 尚未指定 Controller 时才使用程序化原型 Controller，保留新视觉 Prefab 自带 Controller。
- 新增 `Assets/Art/Characters/character_campus_front_humanoid.fbx` 及 `.meta`。
- 新增 `Assets/Art/Characters/character_campus_front_humanoid.prefab` 及 `.meta`。
- 新增 `Assets/Resources/CharacterCampusAnimator.controller` 及 `.meta`。
- 修改 `Assets/Resources/CharacterModels/PlayerVisualProfile.asset`：引用新 Prefab。
- 新增本验收报告。

修改前检查点：`E:\游戏_codex_checkpoint_20261011_P3`。原始 FBX 未覆盖；运行测试未保存 School 场景。Phase 3 smoke 调用现有 SaveAndEnterWorld，重写了 `C:\Users\牛伟东1\AppData\LocalLow\DefaultCompany\游戏\appearance.json`；其 SHA-256 与检查点一致（文件内容未变）。原 Unity 编辑器当前可能尚未刷新刚复制回工程的资源。

## 尚存问题

- 当前 8 个动画片段只是验证状态机和蒙皮的基础动作，需要后续替换或精修为正式动作资源。
- 最终绑骨角色还缺原工程 Game View 中的人工外观、键鼠与物理交互验收。
- Blender 自动权重已覆盖全网格，但没有逐根手指做独立弯曲美术检查。
- 隔离 Unity 批处理日志包含 UnityConnect 网络超时及无图形环境下的 URP Shader 提示；没有发现 Submit 输入错误或阻止 Avatar/动画/交互测试的异常。

## 需要手动完成

1. 在 `E:\游戏` 的 Unity 编辑器执行 **Assets > Refresh**，或在决定如何处理现有未保存场景标记后重新打开工程。不要为了导入模型而误存不相关的场景改动。
2. 进入 School 场景 Play Mode，先从角色创建界面进入世界；点击 Game View 并切换到英文输入法。
3. 对最终绑骨角色检查正反朝向、材质、脚底与胶囊；按 WASD、Shift、Space、右键鼠标、E（交互/拾取/放置）及 F（使用持有物）验收。
4. 若外观和键鼠检查通过，再将这 8 个测试动作替换为正式授权的动作资源，并做关节与手指美术验收。

