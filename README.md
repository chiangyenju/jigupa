# Jigupa - 1v1 Hand Gesture Battle Game

## Game Overview
Jigupa is a strategic turn-based hand gesture battle game where players use Gu (Rock/구), Pa (Paper/파), and Ji (Scissors/지) to eliminate opponent's hands.

## Game Rules

### Initial Setup
- Each player starts with 2 hands (left and right)
- Gu-Pa-Ji determines who attacks first
- First to win 3 rounds wins the match

### Attack Phase
- Attacker chooses gestures for their available hands
- With 2 hands: Can attack with both (e.g., Gu+Pa) or choose single hand
- With 1 hand: Can only perform single-hand attack
- Attack is hidden until defense is submitted

### Defense Phase  
- Defender must choose gestures BEFORE seeing the attack
- Must assign a gesture to each remaining hand
- Defense is blind - pure prediction/strategy

### Resolution
- Attack succeeds when attacker's gesture MATCHES defender's gesture
- When matched, defender loses that entire hand (not just the gesture)
- Single-hand attack can eliminate maximum 1 defending hand
- Double-hand attack can eliminate both defending hands if both match

### Turn Flow
- Players alternate between attacking and defending
- Player with no hands remaining loses the round
- New round starts with both players having 2 hands again

### Victory Condition
- First player to win 3 rounds wins the match

## Technical Features
- **Turn-based Strategy**: Alternating attack/defense phases
- **Blind Defense**: Must predict opponent's attack
- **AI Opponent**: Single-player mode with strategic AI
- **Touch Controls**: Tap to select gestures for each hand

## Development Setup
1. Unity 6.1 or newer
2. Install TextMeshPro when prompted
3. Open project in Unity Hub
4. Run QuickSceneSetup to create game scene
5. Configure build settings for iOS/Android (optimized for iPhone 12)

## Controls
- **Attack Phase**: Select gesture(s) for available hands, then submit
- **Defense Phase**: Select gesture for each hand (blind), then submit
- **UI**: Tap gesture buttons (Gu/Pa/Ji) for each hand

## Build Instructions
1. File > Build Settings
2. Select Android or iOS
3. Configure Player Settings
4. Build and Run