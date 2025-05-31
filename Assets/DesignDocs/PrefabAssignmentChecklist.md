# Prefab Assignment Checklist - VR Dungeon Game

This checklist guides you through creating and wiring up each MVP interactable prefab in Unity.

---

## 1. Room Entrance/Exit Prefab
- **Prefab Name:** RoomEntranceExit
- **Required Components:**
  - Collider (set as trigger)
  - RoomEntranceExit script (custom)
- **Assignment Steps:**
  1. Create an empty GameObject, add a Collider (set as trigger).
  2. Attach the RoomEntranceExit script.
  3. Set up UnityEvents for OnEnter/OnExit to trigger dungeon progression.

## 2. Door/Gate Prefab
- **Prefab Name:** DoorGate
- **Required Components:**
  - MeshRenderer + Collider
  - DoorGate script (custom)
- **Assignment Steps:**
  1. Create a door mesh, add a Collider.
  2. Attach the DoorGate script.
  3. Link to lever/button or voice command triggers.

## 3. Enemy AI Prefab
- **Prefab Name:** EnemyAI
- **Required Components:**
  - Mesh/Animator
  - Emerald AI Agent (Emerald AI 2024)
  - VRIFEnemyController script
- **Assignment Steps:**
  1. Create enemy mesh, add Animator.
  2. Attach Emerald AI Agent and VRIFEnemyController.
  3. Set up enemy stats and behaviors in Emerald AI.

## 4. Weapon Prefab
- **Prefab Name:** Weapon
- **Required Components:**
  - Mesh/Collider
  - VRIF Grabbable script
  - VRIFWeaponBridge script
- **Assignment Steps:**
  1. Create weapon mesh, add Collider.
  2. Attach VRIF Grabbable and VRIFWeaponBridge.
  3. Set up weapon stats and effects.

## 5. Consumable Prefab
- **Prefab Name:** Consumable
- **Required Components:**
  - Mesh/Collider
  - VRIF Grabbable script
  - EmeraldItemSystem script (for pickup/use)
- **Assignment Steps:**
  1. Create consumable mesh, add Collider.
  2. Attach VRIF Grabbable and EmeraldItemSystem.

## 6. Treasure/Chest Prefab
- **Prefab Name:** TreasureChest
- **Required Components:**
  - Mesh/Animator/Collider
  - TreasureChest script (custom)
- **Assignment Steps:**
  1. Create chest mesh, add Animator and Collider.
  2. Attach TreasureChest script.
  3. Set up loot table and open animation.

## 7. Lever/Switch/Button Prefab
- **Prefab Name:** LeverSwitchButton
- **Required Components:**
  - Mesh/Collider
  - XR Grab Interactable (XR Interaction Toolkit)
  - LeverSwitchButton script (custom)
- **Assignment Steps:**
  1. Create lever/button mesh, add Collider.
  2. Attach XR Grab Interactable and custom script.
  3. Link to doors, traps, or events.

## 8. Spatial UI Panel Prefab
- **Prefab Name:** SpatialUIPanel
- **Required Components:**
  - Canvas (World Space)
  - TextMeshProUGUI elements
  - VRIFUIManager script
- **Assignment Steps:**
  1. Create Canvas (World Space), add TextMeshProUGUI for health, inventory, feedback.
  2. Attach VRIFUIManager script.

---

**For each prefab, assign references in the Inspector and test in play mode.** 