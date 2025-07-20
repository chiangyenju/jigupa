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
- Assets/Prefabs/ - Reusable game objects
- Assets/Audio/ - Sound effects (future)

## Key Systems
1. **GameStateManager** - Core game flow, turn-based logic, win conditions
2. **GestureManager** - Player input for attack/defense gestures
3. **UIManager** - Display game state, hands, attack results
4. **PlayerHand** - Tracks hand states (alive/eliminated)
5. **CoinFlipManager** - Initial RPS to determine first attacker
6. **QuickSceneSetup** - Automated UI creation for iPhone 12

## Testing
- Play Mode tests in Unity Editor
- Test single-hand attack/defense scenarios
- Verify UI displays correctly for all hand combinations
- Check win conditions (3 rounds to win match)

## Build Commands
- File > Build Settings > Android/iOS
- Player Settings: Company Name: "Jigupa", Product Name: "Jigupa"