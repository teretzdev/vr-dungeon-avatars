# VR Dungeon Game - MVP Interactables List

To achieve a Minimum Viable Product (MVP) for your voice-guided VR dungeon game, the following interactables are required:

---

## MVP Interactables

### 1. Dungeon Room Interactables
- **Room Entrances/Exits**
  - Trigger dungeon progression or completion when the hero enters/exits.
- **Doors/Gates**
  - Open/close via hero action or voice command.

### 2. Enemy Interactables
- **Enemy AI Characters**
  - Can be targeted, attacked, and defeated by the hero.
  - Respond to hero proximity and combat commands.

### 3. Item Interactables
- **Weapons**
  - Can be picked up, equipped, and used by the hero (VRIF grabbable).
- **Consumables (Potions, etc.)**
  - Can be picked up and used to affect hero stats.
- **Treasure/Chests**
  - Can be opened to grant loot or items.

### 4. Puzzle/Environmental Interactables
- **Levers/Switches/Buttons**
  - Can be activated by the hero (via command or proximity) to open doors, trigger traps, etc.
- **Sockets/Keyholes**
  - Accept specific items to unlock progression. (Optional for MVP)

### 5. UI Interactables
- **Spatial UI Panels**
  - Health, inventory, and command feedback panels that can be interacted with (e.g., via pointer or voice).

### 6. Voice Command Interactables
- **Voice-Activated Objects**
  - Objects that respond directly to specific voice commands (e.g., "Open sesame" for a secret door). (Optional for MVP)

---

## Summary Table

| Interactable Type      | Example(s)                | Required For MVP? |
|----------------------- |--------------------------|-------------------|
| Room Entrances/Exits   | Doorways, portals         | ✅                |
| Enemy AI              | Goblins, skeletons        | ✅                |
| Weapons               | Swords, bows              | ✅                |
| Consumables           | Health potions            | ✅                |
| Treasure/Chests       | Loot chests               | ✅                |
| Levers/Switches       | Door levers, buttons      | ✅                |
| Sockets/Keyholes      | Puzzle locks              | Optional          |
| Spatial UI Panels     | Health/inventory displays | ✅                |
| Voice-Activated Obj.  | Secret doors, traps       | Optional          |

---

**For each interactable, create a prefab and assign the appropriate scripts/components in the Unity Editor.** 