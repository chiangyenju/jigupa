# Jigupa - 1v1 Hand Gesture Battle Game

## Game Overview
Jigupa (嘰咕帕) is a strategic turn-based hand gesture battle game where players use Gu (Rock/咕), Pa (Paper/帕), and Ji (Scissors/嘰) to eliminate opponent's hands. Unlike traditional rock-paper-scissors, in Jigupa you WIN by MATCHING your opponent's gesture!

## Complete Game Rules

### Initial Setup
- Each player starts with 2 hands (left and right)
- Both players begin with Gu/Gu stance (Rock on both hands)
- A coin flip using traditional rock-paper-scissors determines who attacks first:
  - Rock beats Scissors
  - Paper beats Rock  
  - Scissors beats Paper
  - If tie, flip again
- Match format: First to win 3 rounds wins the entire match

### Persistent Stance System
- **Your gestures persist between turns!**
- Whatever gesture you choose (for attack or defense) becomes your new "stance"
- Your current stance is always visible on the game board
- If you don't select new gestures, you automatically use your current stance
- This adds memory and prediction elements to the game

### Attack Phase
- Attacker selects gestures for their available hands
- **With 2 hands**: Always a double-hand attack
  - Must choose gestures for both hands
  - Can use same gesture on both hands or different gestures
  - Left/right position doesn't matter for matching
- **With 1 hand**: Can only perform single-hand attack
- Attack gestures are HIDDEN from defender until defense is submitted
- **Auto-submit**: Attack automatically submits when you select your second gesture
- Selected gestures become your new stance for future turns

### Defense Phase  
- Defender can adjust their stance while waiting
- Once attack is revealed, defender has only **0.3 seconds** to make final changes!
- Must have a gesture for EACH remaining hand
- **Auto-submit**: Defense automatically submits when you select gestures for all hands
- Selected gestures become your new stance for future turns
- **Timeout**: If time runs out, your current stance is used automatically

### Resolution Rules
- **MATCHING = SUCCESS** (opposite of rock-paper-scissors!)
- Attack succeeds when attacker's gesture MATCHES defender's gesture
- **Position-independent matching**: Any attacking gesture can match any defending gesture
- Examples:
  - Attacker: Left=Gu, Right=Pa | Defender: Left=Pa, Right=Ji
    - Attacker's Right (Pa) matches Defender's Left (Pa) → Defender loses one hand
  - Attacker: Left=Ji, Right=Ji | Defender: Left=Ji, Right=Gu
    - Attacker's Ji matches Defender's Ji → Defender loses one hand
  - If BOTH defender's hands match an attacking gesture, the hand directly across is eliminated:
    - Attacker's Left eliminates Defender's Right (they face each other)
    - Attacker's Right eliminates Defender's Left (they face each other)
- When matched, defender loses the ENTIRE HAND (permanently for that round)
- **With 2 attacking hands**: Can potentially eliminate both defending hands if different gestures match
- **With 1 attacking hand**: Can eliminate maximum 1 defending hand

### Turn Flow
1. Coin flip determines first attacker
2. Attack phase: Attacker chooses gestures
3. Defense phase: Defender chooses gestures (blind)
4. Resolution: Check for matches and eliminate hands
5. If defender has hands remaining, roles swap
6. Continue until one player has no hands left
7. Round ends, winner gets 1 point
8. New round starts with both players at 2 hands each

### Victory Conditions
- **Round Victory**: Eliminate all opponent's hands
- **Match Victory**: First player to win 3 rounds
- No draws possible - rounds continue until someone wins

### Strategic Elements
1. **Memory**: Track opponent's stance patterns
2. **Prediction**: Guess if opponent will change or keep their stance
3. **Bluffing**: Show patterns then break them
4. **Risk Management**: Single vs double-hand attacks
5. **Adaptation**: Adjust strategy based on remaining hands

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