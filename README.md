# 3D 轻量游戏原型

> 基于 Unity 6.0 LTS 开发的轻量级 3D 角色与交互游戏原型。

当前项目处于 **第一阶段 Prototype**，主要验证“角色创建 → 保存外貌 → 进入场景 → 第三人称操控 → 与物理物体交互”这一条最基础的游戏流程。

现阶段重点不是制作完整游戏，而是先搭建一个后续可以持续扩展的稳定底层框架。

---

## 1. 项目目标

项目最终希望形成一个以 **自定义角色 + 自由场景 + 轻量生活交互** 为核心的 3D 游戏。

玩家进入游戏后，可以：

1. 在游戏内部创建和调整自己的角色；
2. 保存角色外貌参数；
3. 使用角色进入不同 3D 场景；
4. 自由行走、奔跑、跳跃和观察环境；
5. 拿取、移动和使用场景中的物体；
6. 后续逐步扩展更多生活场景、角色动作和互动玩法。

第一阶段只验证上述体验最基础的一部分。

---

## 2. 当前核心体验

目前已经形成最小可玩的完整流程：

**启动游戏**

↓

**进入角色创建界面**

↓

**调整脸型、五官、肤色和发型**

↓

**Save & Play 保存角色**

↓

**进入 3D 测试场景**

↓

**控制角色行走 / 奔跑 / 跳跃**

↓

**旋转镜头并探索场景**

↓

**推动、抓取和放下物理物体**

↓

**退出游戏后保存外貌参数**

↓

**下次启动自动恢复角色外貌**

---

## 3. 当前功能

### 角色创建

当前使用固定拓扑的程序化角色模型。

头部由 `ProceduralHeadMesh` 按固定顶点顺序生成，因此所有角色共享相同拓扑，可通过 Morph / BlendShape 调整外貌。

已经实现 10 个实际 BlendShape：

* FaceWidth
* FaceLength
* ChinWidth
* ChinLength
* EyeSize
* EyeSpacing
* NoseSize
* NoseWidth
* MouthSize
* LipThickness

同时支持：

* 肤色调整
* 3 种程序化发型
* 实时外貌预览
* 随机角色
* 恢复默认外貌
* 保存角色外貌

当前眼睛、鼻子和嘴唇仍使用轻量程序化占位几何体，以便更明显地展示参数变化。

---

## 4. 角色数据与保存

项目使用 `AppearanceData` 作为角色外貌的唯一数据来源。

角色模型本身不会被整体保存。

保存内容仅包括：

* 11 个 `0–1` 范围的外貌参数
* 发型编号

保存路径：

```text
Application.persistentDataPath/appearance.json
```

下次启动游戏时会自动读取该文件并恢复角色外貌。

这种方式意味着未来即使替换正式人物模型，也可以继续沿用同一套角色参数协议。

---

## 5. 第三人称控制

当前已经实现：

* `WASD`：行走
* `Left Shift`：奔跑
* `Space`：跳跃
* 鼠标：旋转镜头
* 鼠标滚轮：调整镜头距离
* `E`：抓取 / 放下准星指向的刚体
* `Esc`：释放鼠标
* 单击游戏窗口：重新锁定鼠标

角色能够与场景刚体产生物理碰撞，并推动部分物体。

---

## 6. 物理测试地图

当前场景属于 **功能验证地图**，不是正式游戏地图。

地图内提供：

* 方块
* 球体
* 胶囊体
* 堆叠方块
* 可推动刚体
* 可抓取刚体
* 基础碰撞测试区域

主要用于验证：

* 第三人称移动
* Character Controller
* 碰撞
* 刚体物理
* 物体抓取
* 镜头
* 后续玩法接入

---

## 7. 演示

建议后续在这里加入实际运行效果，而不是只通过文字说明项目。

### 角色创建界面

`TODO：加入角色捏脸截图`

### 第三人称场景

`TODO：加入游戏场景截图`

### 物体交互

`TODO：加入抓取 / 推动物体 GIF 或短视频`

后续 GitHub README 可以直接展示：

```text
README Assets/
├── character-creator.png
├── gameplay.png
└── physics-demo.gif
```

这样别人第一次打开仓库时，可以直接看到游戏目前已经做到什么程度。

---

## 8. 打开与运行

需要安装：

* Unity Hub
* Unity `6000.0`
* 推荐版本：`6000.0.40f1` 或同系列更新版本

在 Unity Hub 中：

1. 选择 **添加 / 打开磁盘中的项目**
2. 选择本项目根目录
3. 打开：

```text
Assets/Scenes/Main.unity
```

4. 点击 `Play`

进入游戏后首先打开角色创建界面。

调整外貌完成后点击：

```text
Save & Play
```

即可进入测试地图。

当前项目主要使用 Unity 自带能力以及运行时生成的占位几何体，不依赖额外下载的正式人物模型。

---

## 9. Windows 独立运行版本

第一次构建需要在 Unity Hub 中安装：

* Unity Editor `6000.0.40f1`
* `Windows Build Support`

然后在项目根目录打开 PowerShell：

```powershell
.\Tools\Build-Windows.ps1
```

如果 Unity 安装在自定义目录：

```powershell
.\Tools\Build-Windows.ps1 -UnityEditor "D:\Unity\6000.0.40f1\Editor\Unity.exe"
```

构建脚本会自动将：

```text
Assets/Scenes/Main.unity
```

设置为唯一启动场景。

构建输出：

```text
Builds/
└── Windows/
    ├── 3DGamePrototype.exe
    ├── 3DGamePrototype_Data/
    ├── UnityPlayer.dll
    └── Unity 生成的其他运行文件
```

构建完成后直接双击：

```text
3DGamePrototype.exe
```

即可运行游戏，无需再次打开 Unity。

> 注意：不能只复制 `.exe` 文件。
> `3DGamePrototype_Data`、`UnityPlayer.dll` 等文件必须与 exe 一起保留。

---

## 10. 技术结构

目前项目主要使用：

```text
Unity 6.0 LTS
C#
Unity UI
Procedural Mesh
BlendShape / Morph
JSON Persistence
Character Controller
Unity Physics
Raycast Interaction
PowerShell Build Scripts
```

当前没有加入：

```text
Multiplayer
AI Character Generation
Combat System
Shop System
Story System
AI NPC
Large Open World
Complex Animation System
```

这些内容不是第一阶段目标。

---

## 11. 项目目录

```text
Assets/
│
├── Editor/
│   └── WindowsBuild.cs
│       Windows 64 位编辑器构建入口
│
├── Scenes/
│   └── Main.unity
│       当前启动场景
│
├── Scripts/
│   ├── Core/
│   │   └── 不依赖 Unity 的外貌参数核心
│   │
│   └── Runtime/
│       ├── 角色
│       ├── UI
│       ├── 移动
│       ├── 镜头
│       ├── 物理交互
│       └── 游戏启动逻辑
│
Packages/
│   └── manifest.json
│
ProjectSettings/
│   ├── Unity 版本
│   ├── Build Scene
│   └── 输入配置
│
Tests/
│   └── CoreTests.cs
│
Tools/
│   ├── verify-phase1.ps1
│   └── Build-Windows.ps1
│
Builds/
│   └── Windows/
│
└── README.md
```

---

## 12. 关键脚本

### `GameBootstrap.cs`

负责游戏启动后创建第一阶段需要的运行时对象，并控制：

```text
角色创建状态
        ↓
游戏状态
```

---

### `AppearanceData.cs`

角色外貌的唯一数据结构。

负责：

* 默认角色参数
* 参数归一化
* 随机外貌
* 外貌参数统一管理

后续角色系统仍应继续使用该结构。

---

### `ProceduralHeadMesh.cs`

负责：

* 创建固定拓扑头模
* 保持相同顶点顺序
* 创建 10 个 BlendShape

---

### `ProceduralAvatarFactory.cs`

负责生成：

* 基础人物
* 身体占位结构
* 三种程序化发型

---

### `CharacterAppearance.cs`

负责把 `AppearanceData` 映射到：

* BlendShape
* 五官
* 肤色
* 发型

---

### `CharacterCreatorUI.cs`

负责角色创建 UI：

* Slider
* Random
* Reset
* Save & Play

并实时刷新人物外貌。

---

### `ThirdPersonController.cs`

负责：

* 行走
* 奔跑
* 跳跃
* 基础角色控制

---

### `ThirdPersonCamera.cs`

负责：

* 第三人称镜头
* 鼠标旋转
* 镜头距离

---

### `PhysicsGrabber.cs`

通过屏幕中心射线检测刚体，实现：

```text
E
↓
抓取物体
↓
再次 E
↓
放下物体
```

---

### `AppearanceSaveService.cs`

负责：

```text
AppearanceData
        ↓
JSON
        ↓
appearance.json
```

以及重新启动后的参数恢复。

---

### `WindowsBuild.cs`

Unity Editor 的 Windows 64 位自动构建入口。

---

### `Build-Windows.ps1`

负责：

* 定位 Unity
* 检查 Windows Build Support
* 自动执行 Unity Build
* 检查最终游戏文件

---

## 13. 自动验证

在 PowerShell 中运行：

```powershell
.\Tools\verify-phase1.ps1
```

Unity 不在默认路径时：

```powershell
.\Tools\verify-phase1.ps1 -UnityEditor "D:\Unity\6000.0.40f1\Editor\Unity.exe"
```

验证脚本会执行：

```text
AppearanceData 核心测试
        ↓
运行时脚本语法 / 类型检查
        ↓
项目必要文件检查
        ↓
Unity 批处理导入
        ↓
真实 Unity 编译
```

Unity 编译日志：

```text
Logs/unity-compile.log
```

---

## 14. 第一阶段完成情况

### 已完成

* [x] Unity 3D 项目骨架
* [x] Main 启动场景
* [x] 第三人称行走
* [x] 奔跑
* [x] 跳跃
* [x] 镜头旋转
* [x] 固定拓扑人物基础模型
* [x] 10 个脸部 BlendShape / Morph 参数
* [x] 肤色
* [x] 三种程序化发型
* [x] 实时角色预览
* [x] 随机角色
* [x] 恢复默认外貌
* [x] Save & Play
* [x] JSON 外貌保存
* [x] 重启角色恢复
* [x] 简单 3D 测试场景
* [x] 可推动物体
* [x] 可抓取物体
* [x] 刚体碰撞
* [x] Windows 自动 Build
* [x] 基础自动验证脚本

### 尚未开发

* [ ] 正式人物美术模型
* [ ] 骨骼动画
* [ ] Idle / Walk / Run / Jump 正式动画
* [ ] 更精细的角色身体编辑
* [ ] 多人联机
* [ ] AI 自动捏脸
* [ ] 战斗系统
* [ ] 商城
* [ ] 剧情
* [ ] AI NPC
* [ ] 大型地图

---

## 15. 当前原型的主要不足

第一阶段已经证明基础技术链路可以运行，但距离真正具有游戏体验还有明显差距。

### 角色表现

当前人物仍然是程序化占位角色。

主要问题：

* 人物外观比较简陋
* 五官真实感有限
* 身体细节不足
* 缺少正式骨骼
* 缺少自然动作动画

因此目前人物主要用于验证：

```text
捏脸参数
+
角色数据
+
第三人称控制
```

而不是作为最终美术效果。

---

### 场景

目前地图属于测试环境，生活感较弱。

主要用于：

* 跑动
* 跳跃
* 碰撞
* 抓取
* 物理测试

后续需要逐渐增加真正可以产生游戏体验的场景，例如：

```text
房间
公寓
街道
商店
校园
公共空间
家具
生活物品
```

而不是一直停留在测试方块地图。

---

### 移动与镜头

当前已经具备基本第三人称控制，但下一阶段还需要重点优化：

* 镜头上下观察范围
* 镜头碰撞
* 移动手感
* 转向平滑
* 跳跃手感
* 连续跳跃 / 二段跳是否加入
* 掉出地图后的自动重生
* 地图边界保护
* 出生点 / Checkpoint

---

### 交互

目前只实现基础刚体抓取。

后续可以扩展：

```text
拿起物体
放下物体
投掷
开门
坐下
使用家具
拾取道具
切换手持物
简单物品栏
```

让“场景”从可以走动逐渐变成真正可以互动。

---

## 16. 下一阶段：角色系统升级

第二阶段建议优先提升角色质量，而不是立刻加入联机、AI 或商城。

目标：

```text
当前程序化占位人物
        ↓
正式统一基础人物
        ↓
游戏内角色创建
        ↓
骨骼
        ↓
动画
        ↓
更自然的第三人称角色
```

### Avatar 方案

可以继续研究 Tafi Avatar 或其他合法角色方案作为人物系统候选。

但设计原则保持不变：

**不是开发者提前在外部工具中捏好一个人物，再直接塞进游戏。**

理想方式是：

```text
玩家启动游戏
        ↓
进入游戏自己的角色创建界面
        ↓
玩家自己调整角色
        ↓
保存参数
        ↓
生成 / 更新角色
        ↓
进入游戏
```

也就是说，即使未来使用外部 Avatar 技术，最终体验仍应该是：

**玩家在游戏里面创造自己的角色。**

当前 `AppearanceData` 可以继续作为游戏与 Avatar 系统之间的数据层。

---

## 17. 第二阶段优先级

推荐开发顺序：

```text
1. 修复移动与镜头体验
        ↓
2. 加入掉落重生和地图边界
        ↓
3. 升级正式人物模型
        ↓
4. 加入骨骼与基础动画
        ↓
5. 改进游戏内捏人
        ↓
6. 扩展房间 / 生活场景
        ↓
7. 增加物品交互
        ↓
8. 完成第二阶段 Demo
```

完成这些以后，再决定是否加入：

```text
多人联机
AI 自动捏脸
AI NPC
场景生成
角色关系系统
剧情
商城
```

避免在基础角色体验尚未稳定时同时开发过多大型系统。

---

## 18. 后续代码结构原则

`AppearanceData` 应继续作为角色外貌数据的唯一来源。

新增玩法不要继续全部塞入：

```text
GameBootstrap.cs
```

建议逐渐拆分为：

```text
Assets/Scripts/

Core/
Character/
CharacterCreator/
Movement/
Camera/
Interaction/
World/
UI/
Save/
Systems/
```

每一个系统通过小型入口连接。

例如：

```text
PlayerInteraction
        ↓
DoorInteraction

PlayerInteraction
        ↓
PickupInteraction

PlayerInteraction
        ↓
SeatInteraction
```

而不是把开门、坐下、拿东西等逻辑全部写进角色控制脚本。

---

## 19. 第三方资源与许可证

当前第一阶段主要使用：

* Unity 自带组件
* 自己生成的程序化 Mesh
* 自己的 C# 逻辑
* 程序化占位几何体

当前没有把第三方正式人物素材直接打包进项目。

未来如果加入：

* Tafi Avatar
* 人物模型
* 动作资源
* 音效
* 场景资源
* Texture
* Font
* SDK

应在这里记录：

```text
资源名称
来源
版本
用途
License
是否允许重新分发
```

避免后续项目公开或商业化时出现资源授权问题。

---

## 20. 项目状态

当前版本定位：

**Phase 1 – Technical Prototype**

目前已经证明：

```text
角色参数系统
+
程序化人物
+
游戏内捏脸
+
角色保存
+
第三人称移动
+
基础镜头
+
物理交互
+
Windows 独立运行
```

能够形成一条完整可运行链路。

下一阶段的核心目标不是继续无限增加功能，而是把：

**“能运行”**

逐渐提升为：

**“角色更像人、移动更自然、场景更像生活空间、物体真正可以互动”。**

---

## Roadmap

### Phase 1 — 基础原型 ✅

角色创建、角色保存、第三人称移动、简单地图和物理交互。

### Phase 2 — 角色与基础体验

正式角色、骨骼动画、镜头优化、移动优化、掉落重生、生活场景和基础互动。

### Phase 3 — 世界与玩法

更多场景、人物切换、角色互动、家具与道具、场景编辑。

### Phase 4 — 扩展系统

根据前面阶段效果，再评估多人联机、AI 角色生成、AI NPC、UGC 与其他大型系统。
