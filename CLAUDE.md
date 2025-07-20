# Jigupa Unity Development Guidelines

## Core Rules
1. **NO new files without justification** - Extend existing scripts first
2. **Prioritize Unity patterns** - Use MonoBehaviour, ScriptableObjects, prefabs
3. **Follow Unity conventions** - PascalCase for classes, camelCase for fields
4. **Test commands**: Run in Unity Editor, build for target platform

## Project Structure
- Assets/Scripts/Core/ - Game logic, managers
- Assets/Scripts/Player/ - Player input, gestures
- Assets/Scripts/Network/ - Multiplayer, voice
- Assets/Scripts/UI/ - UI controllers
- Assets/Prefabs/ - Reusable game objects
- Assets/Audio/ - Voice recognition, SFX

## Key Systems
1. **GestureManager** - Handles rock/paper/scissors input
2. **VoiceController** - Voice recognition for attacks
3. **GameStateManager** - Turn-based logic
4. **NetworkManager** - Real-time multiplayer

## Testing
- Play Mode tests in Unity Editor
- Device testing for voice/gesture input

## Build Commands
- File > Build Settings > Android/iOS
- Player Settings: Company Name: "Jigupa", Product Name: "Jigupa"