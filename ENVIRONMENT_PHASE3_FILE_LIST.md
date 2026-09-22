# Environment Phase 3 File List

## 新增脚本

- `Assets/Scripts/Interaction/IEnvironmentInteractable.cs`
- `Assets/Scripts/Interaction/InteractionDetector.cs`
- `Assets/Scripts/Interaction/InteractionPromptUI.cs`
- `Assets/Scripts/Interaction/InteractableDoor.cs`
- `Assets/Scripts/Interaction/InteractableCabinet.cs`
- `Assets/Scripts/Interaction/SeatInteractable.cs`
- `Assets/Scripts/Interaction/LieInteractable.cs`
- `Assets/Scripts/Interaction/PickupInteractable.cs`
- `Assets/Scripts/Interaction/PlacementSurface.cs`
- `Assets/Scripts/Interaction/DefaultSpawnPoint.cs`
- `Assets/Scripts/Interaction/PlayerRespawnSystem.cs`
- `Assets/Scripts/Interaction/EnvironmentPhase3Smoke.cs`
- `Assets/Scripts/Interaction/EnvironmentPhase3Capture.cs`

## 新增编辑器工具与测试

- `Assets/Editor/EnvironmentPhase3Builder.cs`
- `Assets/Editor/EnvironmentPhase3ContractTests.cs`
- `Assets/Editor/EnvironmentPhase3SceneTests.cs`

## 修改运行时代码

- `Assets/Scripts/Runtime/GameBootstrap.cs`
- `Assets/Scripts/Runtime/Interactable.cs`
- `Assets/Scripts/Runtime/InteractionController.cs`
- `Assets/Scripts/Runtime/PhysicsGrabber.cs`
- `Assets/Scripts/Runtime/ThirdPersonController.cs`
- `Assets/Scripts/Interaction/EnvironmentSelectionUI.cs`
- `Assets/Scripts/Interaction/EnvironmentPhase2Smoke.cs`
- `Assets/Scripts/Interaction/HomeEnvironmentSmoke.cs`
- `Assets/Editor/WindowsBuild.cs`

## 修改 Prefab

- `Assets/Prefabs/Interactables/FrontDoor.prefab`
- `Assets/Prefabs/Interactables/DrawerCabinet.prefab`
- `Assets/Prefabs/Interactables/DiningSeat.prefab`
- `Assets/Prefabs/Interactables/LoungeSeat.prefab`
- `Assets/Prefabs/Interactables/ModernBed.prefab`
- `Assets/Prefabs/Interactables/CarryBox.prefab`

## 修改场景

- `Assets/Scenes/School_Environment_V1.unity`
- `Assets/Scenes/Home_Environment_V2.unity`
- `Assets/Scenes/Park_Environment_V1.unity`
- `Assets/Scenes/Urban_Environment_V1.unity`

所有新增 Unity 文件均包含对应 `.meta`。完整逐文件变更记录见 Git 提交 `15807ff1135626b4ffb627b039adc70775ad7524`。

