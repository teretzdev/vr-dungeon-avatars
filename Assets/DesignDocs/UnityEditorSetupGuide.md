# Unity Editor Setup Guide - VR Dungeon Game

Follow these steps to set up your project in the Unity Editor.

---

## 1. Import Required Packages
- XR Interaction Toolkit (via Package Manager)
- Meta Avatar SDK (from Meta)
- VRIF by BNG (Asset Store)
- Emerald AI 2024 (Asset Store)
- Edgar Pro (Asset Store)
- AR Foundation, Oculus Integration (for Quest 3 MR/voice)

## 2. Project Settings
- Enable XR Plugin Management (OpenXR, Quest 3 features)
- Set up URP pipeline and quality settings for Quest 3

## 3. Scene Setup — Explicit Checklist

### GameManager
- Create Empty, rename to `GameManager`
- Add Component: `GameManager` (custom script)
- Assign references for dungeon, player, UI, etc., as required by your script.

### MRController
- Create Empty, rename to `MRController`
- Add Component: `MRController` (custom script)
- Assign:
  - `arSession` (drag your AR Session GameObject)
  - `anchorManager` (drag ARAnchorManager component from AR Session Origin)
  - `xrOrigin` (drag XR Rig GameObject)

### SaveSystem
- Create Empty, rename to `SaveSystem`
- Add Component: `SaveSystem` (custom script)
- Assign references for save data paths, player data, etc., as required.

### PerformanceMonitor
- Create Empty, rename to `PerformanceMonitor`
- Add Component: `PerformanceMonitor` (custom script)
- Assign references for UI or logging as needed.

### NetworkManager
- Create Empty, rename to `NetworkManager`
- Add Component: `NetworkManager` (Unity Netcode, Mirror, or custom script)
- Assign network settings, player prefab, etc.

### EdgarDungeonController
- Create Empty, rename to `EdgarDungeonController`
- Add Component: `EdgarDungeonController` (Edgar Pro or custom script)
- Assign room templates, generation parameters, etc.

### DungeonScaler
- Create Empty, rename to `DungeonScaler`
- Add Component: `DungeonScaler` (custom script)
- Assign references for dungeon root, scaling factors, etc.

### DungeonRenderer
- Create Empty, rename to `DungeonRenderer`
- Add Component: `DungeonRenderer` (custom script)
- Assign references for mesh, materials, etc.

### XRInteractionManager
- Create Empty, rename to `XRInteractionManager`
- Add Component: `XR Interaction Manager` (XR Interaction Toolkit)
- No extra setup unless your scripts require references.

### VRIFUIManager
- Create Empty, rename to `VRIFUIManager`
- Add Component: `VRIFUIManager` (VRIF or custom script)
- Assign references for UI panels, canvases, etc.

### MetaAvatarHero
- Drag Meta Avatar Hero prefab from `Assets/Prefabs/` into the scene, rename to `MetaAvatarHero`
- Assign references for controllers, scripts, etc.

### EmeraldHeroAI
- Create Empty, rename to `EmeraldHeroAI`
- Add Component: `Emerald AI` (Emerald AI package)
- Assign AI profile, stats, etc.

### HeroVRIFController
- Create Empty, rename to `HeroVRIFController`
- Add Component: `HeroVRIFController` (custom or VRIF script)
- Assign references for VR hands, input, etc.

### MetaAvatarAnimator
- Create Empty, rename to `MetaAvatarAnimator`
- Add Component: `Animator` (Unity built-in)
- Add Component: `MetaAvatarAnimator` (custom script)
- Assign avatar controller, animation clips, etc.

### EmeraldCombatManager
- Create Empty, rename to `EmeraldCombatManager`
- Add Component: `EmeraldCombatManager` (custom or Emerald AI script)
- Assign references for player, enemies, combat settings.

### EmeraldItemSystem
- Create Empty, rename to `EmeraldItemSystem`
- Add Component: `EmeraldItemSystem` (custom or Emerald AI script)
- Assign item prefabs, inventory references, etc.

### CommandInterpreter
- Create Empty, rename to `CommandInterpreter`
- Add Component: `CommandInterpreter` (custom script)
- Assign references for input, command list, etc.

### VRIFVoiceController
- Create Empty, rename to `VRIFVoiceController`
- Add Component: `VRIFVoiceController` (custom or VRIF script)
- Assign microphone, input, and event references.

### MetaAvatarResponseSystem
- Create Empty, rename to `MetaAvatarResponseSystem`
- Add Component: `MetaAvatarResponseSystem` (custom script)
- Assign references for avatar, audio, and response logic.

---

## 4. Assign References
- Drag and drop required prefabs/scripts onto each manager GameObject
- Assign references in the Inspector (e.g., avatar prefabs, dungeon generator, UI panels)
- Set up event listeners (e.g., OnDungeonGenerated, OnGameStateChanged)

## 5. Prefab Placement
- Place interactable prefabs (from checklist) in the scene or as room templates for Edgar Pro

## 6. Test and Iterate
- Enter Play mode and test the voice-to-AI loop, dungeon generation, and interactions
- Refine prefab setups and references as needed

---

**For more details, see the Prefab Assignment Checklist and MVP Interactables docs.** 