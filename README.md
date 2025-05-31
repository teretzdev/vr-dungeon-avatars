# VR Dungeon Game (Unity 6000/2025)

## Overview
A next-gen VR/MR dungeon crawler for Quest 3, where you guide a tiny Meta Avatar hero through procedurally generated dungeons using your voice. Integrates Meta Avatar System, Emerald AI 2024, VRIF by BNG, XR Interaction Toolkit, and Edgar Pro.

## Core Gameplay Loop
- Speak commands to guide your hero (Meta Avatar) through dungeons.
- Dungeons are procedurally generated and scaled to your real-world space.
- Hero responds to your voice, navigates, fights, and interacts with the world.

## Key Systems
- **GameManager**: Orchestrates game state, scene flow, and system coordination.
- **MRController**: Handles MR/VR mode, passthrough, and spatial anchoring.
- **EdgarDungeonController**: Generates dungeons using Edgar Pro.
- **DungeonScaler/Renderer**: Fits dungeons to your space and manages visuals.
- **MetaAvatarHero/EmeraldHeroAI**: Spawns and controls your hero using Meta Avatar and Emerald AI.
- **VRIFVoiceController/CommandInterpreter**: Captures and interprets your voice commands.
- **Combat/Interaction/UI**: VRIF and XR Interaction Toolkit for weapons, enemies, and UI.

## Setup Instructions
1. **Unity Version**: Use Unity 6000 (2025) or later.
2. **Import Packages**:
   - XR Interaction Toolkit (via Package Manager)
   - Meta Avatar SDK (from Meta)
   - VRIF by BNG (Asset Store)
   - Emerald AI 2024 (Asset Store)
   - Edgar Pro (Asset Store)
   - AR Foundation, Oculus Integration (for Quest 3 MR/voice)
3. **Folder Structure**: Scripts are organized by system (Core, Dungeon, Hero, Combat, Communication, Interaction).
4. **Scenes**: Create a main scene and add GameManager, MRController, and other managers as singletons.
5. **Prefabs**: Set up prefabs for Meta Avatar, dungeon rooms, enemies, and UI.
6. **Configure XR**: Enable OpenXR, set up Quest 3 features (passthrough, hand tracking).

## Implementation Directions
- **Voice-to-AI Loop**: VRIFVoiceController captures voice → CommandInterpreter maps to Emerald AI behaviors → Hero acts.
- **Dungeon Generation**: EdgarDungeonController generates layout, DungeonScaler fits to space, DungeonRenderer handles visuals.
- **Hero System**: MetaAvatarHero spawns avatar, EmeraldHeroAI controls movement/combat, MetaAvatarAnimator handles expressions.
- **Combat/Interaction**: EmeraldCombatManager and VRIF systems manage fighting and item use.
- **UI**: VRIFUIManager displays health, inventory, and command feedback in MR/VR space.

## Design Notes
- Modular, event-driven architecture for easy extension.
- All systems are ready to integrate with their respective Unity packages.
- Focused on hands-free, voice-driven gameplay for maximum immersion.

## Requirements
- Unity 6000 (2025) or later
- Quest 3 hardware
- All listed Unity packages

## Credits
- Meta, BNG, Edgar Pro, Emerald AI, Unity Technologies

---
For more details, see the code comments and each system's documentation. 