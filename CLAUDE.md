# Jigupa Unity Development Guidelines

## Core Rules
1. **NO new files without justification** - Extend existing scripts first
2. **Prioritize Unity patterns** - Use MonoBehaviour, ScriptableObjects, prefabs
3. **Follow Unity conventions** - PascalCase for classes, camelCase for fields
4. **Test commands**: Run in Unity Editor, build for target platform

## Project Structure
- Assets/Scripts/Core/ - Game logic, managers
- Assets/Scripts/Player/ - Player input, gestures
- Assets/Scripts/Network/ - Multiplayer (future)
- Assets/Scripts/UI/ - UI controllers
- Assets/Scripts/Editor/ - Unity editor tools and helpers
- Assets/Scenes/ - MainMenu and Battle scenes
- Assets/Prefabs/ - Reusable game objects
- Assets/Audio/ - Sound effects (future)

## Scene Architecture
1. **MainMenu Scene** - Starting scene with 5-tab navigation
   - Player, Guild, Battle, Minigame, Shop tabs
   - Use `MainMenuUISetup` to create UI
2. **Battle Scene** - Actual gameplay
   - Use `BattleSceneSetup` (auto-setup on start)
   - Or `SimpleUISetup` for manual setup

## Key Systems
1. **GameStateManager** - Core game flow, turn-based logic, win conditions
2. **GestureManager** - Player input for attack/defense gestures
3. **UIManager** - Display game state, hands, attack results
4. **PlayerHand** - Tracks hand states (alive/eliminated)
5. **CoinFlipManager** - Initial RPS to determine first attacker
6. **SimpleUISetup** - Creates battle UI (replaces QuickSceneSetup)
7. **MainMenuUISetup** - Creates main menu with navigation
8. **NavigationController** - Handles tab switching in main menu

## Game Rules Implementation
- **Position-independent matching** - Any attack gesture can match any defense
- **0.3 second reaction window** - Defender can adjust after attack reveal
- **Forced double-hand attacks** - Must use both hands when available
- **Cross-elimination rule** - When both hands match, use facing positions

## Setup Instructions
1. **First Time Setup**:
   - Menu: Jigupa → Setup All Scenes
   - This configures Build Settings automatically
2. **Create Main Menu**:
   - Open MainMenu scene
   - Add GameObject → MainMenuUISetup → Setup Main Menu UI
3. **Create Battle Scene**:
   - Open Battle scene  
   - Add GameObject → BattleSceneSetup (auto-setups on play)

## Testing
- Play Mode tests in Unity Editor
- Start from MainMenu scene (default)
- Test scene transitions (MainMenu → Battle → MainMenu)
- Test single-hand attack/defense scenarios
- Verify UI displays correctly for all hand combinations
- Check win conditions (3 rounds to win match)
- Verify 0.3 second defense reaction window

## Build Commands
- File > Build Settings > Android/iOS
- Ensure scenes are ordered: 0-MainMenu, 1-Battle
- Player Settings: Company Name: "Jigupa", Product Name: "Jigupa"