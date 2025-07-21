# Scene Organization

This folder contains all Unity scenes for the Jigupa project.

## Scene Structure

- **MainMenu.unity** - Main menu with 5-tab navigation (Player, Guild, Battle, Minigame, Shop)
- **Battle.unity** - The actual Jigupa game scene
- **Loading.unity** (optional) - Loading screen between scenes

## How to Create Scenes

1. **MainMenu Scene:**
   - File → New Scene
   - Save as `Assets/Scenes/MainMenu.unity`
   - Add empty GameObject → Add `MainMenuUISetup` component
   - Right-click component → "Setup Main Menu UI"

2. **Battle Scene:**
   - File → New Scene
   - Save as `Assets/Scenes/Battle.unity`
   - Add empty GameObject → Add `SimpleUISetup` component
   - Right-click component → "Setup Simple Jigupa Scene"

## Build Settings

Add scenes to build in this order:
1. MainMenu (index 0)
2. Battle (index 1)

File → Build Settings → Add Open Scenes