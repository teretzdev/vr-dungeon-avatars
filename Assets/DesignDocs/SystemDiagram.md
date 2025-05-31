# System Diagram - VR Dungeon Game

This diagram shows the relationships between major classes and systems.

---

```mermaid
flowchart TD
    GameManager --> MRController
    GameManager --> SaveSystem
    GameManager --> PerformanceMonitor
    GameManager --> NetworkManager
    GameManager --> EdgarDungeonController
    GameManager --> XRInteractionManager
    GameManager --> VRIFUIManager
    GameManager --> MetaAvatarHero
    GameManager --> EmeraldHeroAI
    GameManager --> HeroVRIFController
    GameManager --> MetaAvatarAnimator
    GameManager --> EmeraldCombatManager
    GameManager --> EmeraldItemSystem
    GameManager --> CommandInterpreter
    GameManager --> VRIFVoiceController
    GameManager --> MetaAvatarResponseSystem
    EdgarDungeonController --> DungeonScaler
    EdgarDungeonController --> DungeonRenderer
    EdgarDungeonController --> EdgarRoomWrapper
    MetaAvatarHero --> MetaAvatarAnimator
    EmeraldHeroAI --> MetaAvatarHero
    EmeraldHeroAI --> HeroVRIFController
    EmeraldHeroAI --> EmeraldCombatManager
    VRIFVoiceController --> CommandInterpreter
    CommandInterpreter --> EmeraldHeroAI
    CommandInterpreter --> MetaAvatarResponseSystem
    EmeraldCombatManager --> VRIFEnemyController
    EmeraldCombatManager --> VRIFWeaponBridge
    EmeraldCombatManager --> EmeraldItemSystem
    XRInteractionManager --> VRIFUIManager
```

---

**Legend:**
- Arrows indicate primary control or data flow.
- Managers coordinate subsystems and handle events.
- See code comments for more details. 