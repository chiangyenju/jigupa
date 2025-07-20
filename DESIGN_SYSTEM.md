# Jigupa Design System

## Design Philosophy
Minimalist, elegant, and focused on clarity. The design emphasizes the strategic nature of Jigupa through clean typography, generous whitespace, and subtle animations. Every element serves a purpose - nothing is decorative without function.

## Color Palette

### Primary Colors
- **Primary Red**: `#AC0929` - Main brand color, used for important actions and active states
- **Primary Dark**: `#7A061D` - Darker variant for hover states
- **Primary Light**: `#D91F42` - Lighter variant for highlights

### Neutral Colors
- **Pure Black**: `#000000` - Main text on light backgrounds
- **Off Black**: `#1A1A1A` - Softer black for large text areas
- **Dark Gray**: `#404040` - Secondary text
- **Medium Gray**: `#808080` - Disabled states, borders
- **Light Gray**: `#E0E0E0` - Subtle borders, dividers
- **Off White**: `#F8F8F8` - Background alternatives
- **Pure White**: `#FFFFFF` - Primary background

### Semantic Colors
- **Success**: `#22C55E` - Win states, successful actions
- **Warning**: `#F59E0B` - Time warnings, important notices
- **Info**: `#3B82F6` - Informational states
- **Danger**: `#EF4444` - Elimination, loss states

### Game-Specific Colors
- **Player Blue**: `#2563EB` - Player 1 accent
- **AI Purple**: `#7C3AED` - AI/Player 2 accent
- **Attack Pulse**: `#AC0929` with 0.2 opacity - Attack warning overlay

## Typography

### Font Family
- **Primary**: Lexend - Clean, modern, excellent readability
- **Fallback**: -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif
- **Chinese**: "PingFang SC", "Microsoft YaHei", sans-serif

### Type Scale
- **Display**: 56px / 64px line-height / -0.02em letter-spacing
- **Heading 1**: 40px / 48px line-height / -0.02em letter-spacing
- **Heading 2**: 32px / 40px line-height / -0.01em letter-spacing
- **Heading 3**: 24px / 32px line-height / 0em letter-spacing
- **Body Large**: 18px / 28px line-height / 0em letter-spacing
- **Body**: 16px / 24px line-height / 0em letter-spacing
- **Body Small**: 14px / 20px line-height / 0em letter-spacing
- **Caption**: 12px / 16px line-height / 0.01em letter-spacing

### Font Weights
- **Light**: 300 - Large display text only
- **Regular**: 400 - Body text, default
- **Medium**: 500 - Emphasis, buttons
- **Semibold**: 600 - Headings, important labels
- **Bold**: 700 - Critical information, CTAs

## Spacing System
Based on 8px grid system:
- **xs**: 4px
- **sm**: 8px
- **md**: 16px
- **lg**: 24px
- **xl**: 32px
- **2xl**: 48px
- **3xl**: 64px
- **4xl**: 96px

## Layout Principles

### Grid System
- **Mobile**: Single column, edge spacing: 16px
- **Tablet**: Max width 768px, edge spacing: 24px
- **Desktop**: Max width 1024px, edge spacing: 32px

### Safe Areas (iPhone 12)
- **Top**: 47px (notch)
- **Bottom**: 34px (home indicator)
- **Sides**: 16px minimum

### Component Spacing
- **Between sections**: 48px
- **Between related elements**: 24px
- **Between tight groups**: 16px
- **Internal padding**: 16-24px

## Component Design

### Buttons

#### Primary Button
```
Background: #AC0929
Text: #FFFFFF (Medium weight)
Border-radius: 12px
Padding: 16px 32px
Height: 56px
Shadow: 0 4px 16px rgba(172, 9, 41, 0.2)
Hover: Background #7A061D, translate Y -2px
Active: Background #7A061D, translate Y 0px
Disabled: Background #808080, no shadow
```

#### Secondary Button
```
Background: transparent
Border: 2px solid #AC0929
Text: #AC0929 (Medium weight)
Border-radius: 12px
Padding: 16px 32px
Height: 56px
Hover: Background rgba(172, 9, 41, 0.05)
Active: Background rgba(172, 9, 41, 0.1)
Disabled: Border #808080, Text #808080
```

#### Gesture Button
```
Background: #FFFFFF
Border: 2px solid #E0E0E0
Border-radius: 20px
Size: 120x120px (mobile), 150x150px (tablet+)
Shadow: 0 2px 8px rgba(0, 0, 0, 0.05)
Hover: Border #AC0929, Shadow 0 4px 16px rgba(172, 9, 41, 0.15)
Selected: Background #AC0929, Border #AC0929, Text #FFFFFF
Disabled: Background #F8F8F8, Border #E0E0E0
Emoji size: 48px
Label: 14px, #404040, margin-top 8px
```

### Cards

#### Game Panel
```
Background: #FFFFFF
Border-radius: 24px
Padding: 32px
Shadow: 0 8px 32px rgba(0, 0, 0, 0.08)
Border: 1px solid rgba(0, 0, 0, 0.05)
```

#### Info Card
```
Background: #F8F8F8
Border-radius: 16px
Padding: 24px
No shadow
Border: 1px solid #E0E0E0
```

### Text Elements

#### Game State Text
```
Font-size: 24px
Font-weight: 600
Color: #1A1A1A
Text-align: center
Letter-spacing: -0.01em
```

#### Score Display
```
Font-size: 18px
Font-weight: 500
Color: #404040
Background: #F8F8F8
Padding: 12px 24px
Border-radius: 12px
```

#### Timer
```
Font-size: 40px
Font-weight: 300
Color: #AC0929
Tabular figures
Animation: Pulse when < 5 seconds
```

### Overlays

#### Attack Warning
```
Background: rgba(172, 9, 41, 0.95)
Backdrop-filter: blur(8px)
Border-radius: 24px
Padding: 48px
Animation: Slide up + fade in (0.3s ease-out)
```

#### Modal
```
Background: rgba(0, 0, 0, 0.5)
Content background: #FFFFFF
Content border-radius: 32px
Content padding: 48px
Max-width: 560px
Animation: Scale in (0.3s ease-out)
```

## Animation Guidelines

### Timing Functions
- **Ease Out**: cubic-bezier(0.0, 0.0, 0.2, 1) - Most UI animations
- **Ease In Out**: cubic-bezier(0.4, 0.0, 0.2, 1) - Page transitions
- **Spring**: cubic-bezier(0.34, 1.56, 0.64, 1) - Playful feedback

### Duration
- **Instant**: 100ms - Hover states
- **Quick**: 200ms - Simple transitions
- **Normal**: 300ms - Most animations
- **Slow**: 500ms - Complex animations

### Common Animations
- **Button hover**: Transform scale(1.02), 100ms
- **Button press**: Transform scale(0.98), 100ms
- **Card enter**: Opacity 0→1, translateY 20px→0, 300ms
- **Attack pulse**: Scale 1→1.05→1, 1000ms, infinite
- **Win celebration**: Confetti particle system, 3000ms

## Accessibility

### Color Contrast
- Normal text: Minimum 4.5:1 ratio
- Large text: Minimum 3:1 ratio
- Interactive elements: Minimum 3:1 ratio against background

### Touch Targets
- Minimum size: 44x44px
- Recommended: 48x48px
- Spacing between targets: 8px minimum

### Visual Feedback
- Focus states: 3px outline, offset 2px, color #AC0929
- Loading states: Skeleton screens with shimmer animation
- Error states: Red border, error message below field

## Responsive Breakpoints
- **Mobile**: 0-639px
- **Tablet**: 640-1023px
- **Desktop**: 1024px+

## Implementation Notes

### CSS Variables
```css
:root {
  /* Colors */
  --color-primary: #AC0929;
  --color-primary-dark: #7A061D;
  --color-primary-light: #D91F42;
  
  /* Typography */
  --font-family: 'Lexend', -apple-system, sans-serif;
  
  /* Spacing */
  --spacing-xs: 4px;
  --spacing-sm: 8px;
  --spacing-md: 16px;
  --spacing-lg: 24px;
  --spacing-xl: 32px;
  
  /* Shadows */
  --shadow-sm: 0 2px 8px rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 16px rgba(0, 0, 0, 0.08);
  --shadow-lg: 0 8px 32px rgba(0, 0, 0, 0.12);
}
```

### Unity-Specific Settings
- Canvas Scaler: Scale with screen size, 0.5 match
- Reference Resolution: 1170 x 2532 (iPhone 12)
- Default material: UI-Default with Pixel Perfect enabled
- Text: TextMeshPro with SDF rendering