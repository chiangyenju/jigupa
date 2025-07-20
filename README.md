# Jigupa - 1v1 Hand Gesture Battle Game

## Game Overview
Jigupa is an intense real-time 1v1 hand gesture battle game where players use Rock (Gu), Paper (Pa), and Scissors (Ji) in strategic combinations.

## Game Rules

### Attack Phase
- Attacker yells their move combination
- Must choose 2 gestures: either identical (e.g., Rock+Rock) or different (e.g., Rock+Paper)
- Voice command triggers visual display for defender

### Defense Phase  
- Defender sees attacker's gestures appear on screen
- Must simultaneously tap two gestures to defend
- If defender has duplicates matching attacker's gesture, they lose that hand
- Remaining hands become defender's attack options

### Victory Condition
Battle continues with alternating attack/defense turns until one player has no remaining gestures.

## Technical Features
- **Voice Recognition**: Attack commands via voice input
- **Real-time Display**: Instant gesture visualization
- **Simultaneous Input**: Multi-touch defense selection
- **Network Play**: 1v1 multiplayer support

## Development Setup
1. Unity 2022.3 LTS or newer
2. Install required packages (see Packages/)
3. Open project in Unity Hub
4. Configure build settings for iOS/Android

## Controls
- **Attacker**: Voice command (e.g., "Rock Paper!")
- **Defender**: Tap two gesture buttons simultaneously

## Build Instructions
1. File > Build Settings
2. Select Android or iOS
3. Configure Player Settings
4. Build and Run