# Scene Setup Guide for Jigupa

## Game Flow
- **Default Scene**: MainMenu (always starts here)
- **Battle Entry**: Click "PLAY JIGUPA" button in Battle tab
- **Return to Menu**: Click "MENU" button in Battle scene

## Quick Setup

### 1. MainMenu Scene (Default Starting Scene)
1. Open MainMenu scene
2. Create empty GameObject named "MenuSetup"
3. Add `MainMenuUISetup` component
4. Right-click component → "Setup Main Menu UI"
5. Save scene
6. **Set as default**: This should be scene index 0 in Build Settings

### 2. Battle Scene (Loaded when playing)
1. Open Battle scene
2. Create empty GameObject named "BattleSetup"
3. Add `BattleSceneSetup` component
4. Check "Auto Setup On Start" in inspector
5. Check "Add Return To Menu Button" in inspector
6. Save scene

### 3. Build Settings Configuration
1. File → Build Settings
2. Add scenes in this order:
   - 0: MainMenu (DEFAULT - game starts here)
   - 1: Battle (loaded when clicking PLAY JIGUPA)

## Alternative Manual Setup

If objects are being created in wrong scene:

### For Battle Scene:
1. Make sure Battle scene is the ACTIVE scene (double-click it)
2. Create empty GameObject IN THE BATTLE SCENE
3. Add `SimpleUISetup` component
4. Right-click → "Setup Simple Jigupa Scene"
5. Verify all objects are in Battle scene hierarchy

### Common Issues:

**Objects created in wrong scene:**
- Double-click the scene you want to work in
- Make sure it's the active scene (bold in hierarchy)
- Create the setup GameObject IN that scene's hierarchy

**Scene not loading:**
- Check Build Settings has both scenes added
- Use scene names exactly as saved ("MainMenu", "Battle")

**Can't find setup components:**
- BattleSceneSetup is in Scripts/Core/
- MainMenuUISetup is in Scripts/UI/
- SimpleUISetup is in Scripts/Core/