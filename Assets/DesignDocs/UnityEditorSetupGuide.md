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

## 3. Scene Setup
- Create a new scene (e.g., MainScene)
- Add empty GameObjects for each manager:
  - GameManager
  - MRController
  - SaveSystem
  - PerformanceMonitor
  - NetworkManager
  - EdgarDungeonController
  - DungeonScaler
  - DungeonRenderer
  - XRInteractionManager
  - VRIFUIManager
  - MetaAvatarHero
  - EmeraldHeroAI
  - HeroVRIFController
  - MetaAvatarAnimator
  - EmeraldCombatManager
  - EmeraldItemSystem
  - CommandInterpreter
  - VRIFVoiceController
  - MetaAvatarResponseSystem

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