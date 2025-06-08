# Prefab Creation Guide - Using Editor Scripts

This guide explains how to use the provided editor scripts to create prefabs and set up your scene in Unity.

---

## Prerequisites

1. Make sure you have imported all required packages:
   - XR Interaction Toolkit
   - Meta Avatar SDK
   - Emerald AI 2024
   - VRIF by BNG
   - Edgar Pro

2. Ensure all scripts are compiled without errors in Unity

---

## Step 1: Creating Prefabs with PrefabCreator

### Opening the Tool
1. In Unity, go to the menu bar
2. Click **DungeonYou → Prefab Creator**
3. A window will open with buttons for each prefab type

### Creating Each Prefab

#### Room Entrance/Exit
1. Click "Create Room Entrance/Exit Prefab"
2. The tool will:
   - Create a GameObject with trigger collider
   - Add the RoomEntranceExit script
   - Save as `Assets/Prefabs/Interactables/RoomEntranceExit.prefab`

#### Door/Gate
1. Click "Create Door/Gate Prefab"
2. The tool will:
   - Create a door with placeholder mesh
   - Add AudioSource and DoorGate script
   - Save as `Assets/Prefabs/Interactables/DoorGate.prefab`

#### Treasure Chest
1. Click "Create Treasure Chest Prefab"
2. The tool will:
   - Create chest with base and lid meshes
   - Add AudioSource, Animator, and TreasureChest script
   - Create loot spawn point
   - Save as `Assets/Prefabs/Interactables/TreasureChest.prefab`

#### Lever/Switch/Button
1. Click "Create Lever/Switch/Button Prefab"
2. The tool will:
   - Create lever with base and handle
   - Add XR interaction components
   - Add LeverSwitchButton script
   - Save as `Assets/Prefabs/Interactables/LeverSwitchButton.prefab`

#### Spatial UI Panel
1. Click "Create Spatial UI Panel Prefab"
2. The tool will:
   - Create world-space canvas
   - Add text elements for health, gold, inventory, feedback
   - Add VRIFUIManager script
   - Save as `Assets/Prefabs/UI/SpatialUIPanel.prefab`

---

## Step 2: Setting Up the Scene with SceneSetupHelper

### Opening the Tool
1. In Unity, go to the menu bar
2. Click **DungeonYou → Scene Setup Helper**

### Complete Setup (Recommended)
1. Click "Setup Complete Game Scene"
2. This will automatically:
   - Create all manager GameObjects
   - Set up XR Rig with controllers
   - Configure MR-friendly lighting
   - Create a sample dungeon room

### Individual Setup Options

#### Core Managers Only
- Click "Setup Core Managers" to create just the manager objects

#### XR Rig Only
- Click "Setup XR Rig" to create the VR player setup

#### MR Lighting Only
- Click "Setup Lighting for MR" to configure scene lighting

#### Sample Dungeon Only
- Click "Create Sample Dungeon Room" to create a test room

---

## Step 3: Manual Configuration After Creation

### For Each Prefab

#### DoorGate
1. Select the prefab
2. In Inspector:
   - Assign open/close audio clips
   - Configure door type (Sliding, Rotating, etc.)
   - Set voice commands in arrays

#### TreasureChest
1. Select the prefab
2. In Inspector:
   - Add items to loot table
   - Assign audio clips
   - Configure gold amounts
   - (Optional) Create open animation in Animator

#### LeverSwitchButton
1. Select the prefab
2. In Inspector:
   - Set interaction type (Lever, Switch, Button)
   - Assign materials for activated/deactivated states
   - Link UnityEvents to doors or other objects

#### SpatialUIPanel
1. Select the prefab
2. In VRIFUIManager component:
   - Drag text elements to corresponding slots
   - Adjust canvas position/scale as needed

### For the Scene

#### Link References
1. Select each manager GameObject
2. Drag references to required fields in Inspector
3. Common references:
   - Hero prefab → various managers
   - UI Panel → VRIFUIManager
   - Dungeon root → DungeonScaler

#### Configure Tags
1. Create and assign these tags:
   - "Hero" - for the tiny hero character
   - "Enemy" - for enemy characters
   - "Interactable" - for interactive objects

#### Set Layers
1. Create layers for:
   - "Hero"
   - "Enemy"
   - "Interactable"
   - "UI"

---

## Step 4: Testing Your Setup

1. **Play Mode Test**:
   - Enter Play mode
   - Verify managers initialize without errors
   - Check console for debug messages

2. **Interaction Test**:
   - Test grabbing levers
   - Test door interactions
   - Test chest opening

3. **Voice Command Test**:
   - Press 'V' key to start listening (or configure for Quest)
   - Test voice commands on doors

---

## Troubleshooting

### "Component not found" errors
- Ensure all packages are imported
- Check script namespaces match

### Prefabs not saving
- Check folder permissions
- Ensure Prefabs folder exists

### XR components missing
- Import XR Interaction Toolkit
- Update XR Plugin Management

---

## Next Steps

1. Replace placeholder meshes with actual models
2. Configure Emerald AI enemies
3. Set up Meta Avatar for hero
4. Integrate Edgar Pro dungeon generation
5. Configure voice recognition for Quest 3

---

Remember: These tools create a starting point. You'll need to customize and configure based on your specific assets and requirements.