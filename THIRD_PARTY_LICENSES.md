# 第三方资源及 Tafi 调查

调查日期：2026-09-19。此轮没有导入第三方人物、家具、贴图、音效或动画文件。人物、房屋、物品和动作片段由本工程脚本/Unity 基础网格生成。Unity 引擎与 `com.unity.ugui` 按 Unity 自身许可使用，构建包包含 Unity 的运行库。

| 项目 | 当前使用 | 来源与许可 |
| --- | --- | --- |
| Unity Engine、Unity UI、Windows Player | 使用 | Unity `6000.0.40f1` 自带模块；适用 [Unity 条款](https://unity.com/legal/terms-of-service)。 |
| Tafi Avatar / Astra SDK | 未使用 | 不能在没有当前 SDK 包和许可条款时假定集成权。 |
| 第三方人物/家具/动画 | 未使用 | 场景和角色为项目内程序化原型。 |

## Tafi 的事实边界

- Tafi 自己发布的 [2021 年 Astra SDK 公告](https://www.prnewswire.com/news-releases/tafi-launches-new-avatar-astra-sdk-301269831.html) 和 [2022 年公开发布公告](https://www.prnewswire.com/news-releases/tafi-announces-the-public-launch-of-the-game-changing-astra-avatar-creation-engine-301616764.html) 明确描述了供开发者在游戏或应用中提供玩家角色创建/定制的运行时 SDK。它并非只能供开发者在外部做固定角色。
- 2022 年公告称可注册领取 SDK，具备客户端定制、流式资产与 API；这些是当时的公开产品描述。现在打开旧的 `maketafi.com/Astra-SDK` 入口会跳转到 Daz 页面，未找到仍可下载、适配 Unity `6000.0.40f1` 的官方 SDK 包和可执行接入文档。
- **当前授权、收费、账号/API Key、联网/离线条件、资产导出与再分发限制、能否在本游戏给玩家开放编辑**均未获得可核验的现行条款。历史公告中的“游戏内可用”不能替代现行商业许可。未经确认不能把 Tafi 组件加入 Build，也不能把预制 Tafi 模型说成游戏内捏人。

若要正式集成，需要 Tafi/权利方提供：适用 Unity 版本的 SDK 和技术文档、项目/终端用户许可、价格、账号或密钥、离线与服务可用性要求、资产流式传输与导出条款，并提供合法可用的角色/服装资源。届时可把现有 `AppearanceData` 参数及创建界面作为迁移起点，先做授权范围内的最小接入测试。
