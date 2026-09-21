# 住宅环境 V2 第三方资源

本阶段只导入下表用于住宅的资源。原始 FBX/JPG 保存在 `Assets/Art/Furniture/PolyHaven/` 和 `Assets/Art/Materials/ambientCG/`；运行场景使用项目内生成的材质与预制体。Poly Haven 下载文件的官方 URL 和 MD5 记录在 `Assets/Art/Furniture/PolyHaven/download_manifest.csv`。

| 资源 | 来源 | 许可证 | 用途 |
| --- | --- | --- | --- |
| Modern Arm Chair 01 | https://polyhaven.com/a/modern_arm_chair_01 | CC0 | 客厅扶手椅 |
| Modern Coffee Table 01 | https://polyhaven.com/a/modern_coffee_table_01 | CC0 | 客厅茶几 |
| Modern Wooden Cabinet | https://polyhaven.com/a/modern_wooden_cabinet | CC0 | 储物柜 |
| Drawer Cabinet | https://polyhaven.com/a/drawer_cabinet | CC0 | 可开启抽屉柜 |
| Dining Chair 02 | https://polyhaven.com/a/dining_chair_02 | CC0 | 可坐餐椅 |
| Side Table 01 | https://polyhaven.com/a/side_table_01 | CC0 | 边桌 |
| Modern Ceiling Lamp 01 | https://polyhaven.com/a/modern_ceiling_lamp_01 | CC0 | 吊灯 |
| WoodFloor040 | https://ambientcg.com/view?id=WoodFloor040 | CC0 | 木地板与木制物件 |
| Concrete045 | https://ambientcg.com/view?id=Concrete045 | CC0 | 步道、屋顶、台面 |
| Metal029 | https://ambientcg.com/view?id=Metal029 | CC0 | 金属构件 |
| Kloofendal 48d Partly Cloudy（2K HDRI） | https://polyhaven.com/a/kloofendal_48d_partly_cloudy | CC0 | 日间环境光与天空 |

许可证来源：[Poly Haven License](https://polyhaven.com/license)、[ambientCG License](https://docs.ambientcg.com/license/)。模型选用 1K FBX/JPG 版本；材质选用 1K JPG。法线贴图导入为 Normal Map，粗糙度转换到 URP Metallic/Smoothness 贴图 Alpha 通道。所有文件来自上述官方站点，不含用户文档中已下架的 Unity 住宅包。

床、沙发、楼板、墙体、楼梯、门和厨房基础构件由本项目制作；它们的几何网格、材质和预制体位于 `Assets/Art/Buildings/`、`Assets/Art/Materials/Generated/`、`Assets/Prefabs/`。
