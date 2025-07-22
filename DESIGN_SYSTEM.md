# Jigupa Design System - School Arena Edition

## Design Philosophy
Bold, vibrant, and energetic! Inspired by East Asian school environments where students duel in hallways and basketball courts. Large, colorful elements capture the excitement of playground battles. Every element is oversized for impact and easy interaction.

## Color Palette

### Core 5-Color System
- **Primary Red**: `#AC0929` - Main brand color, buttons, key actions
- **Light Red**: `#E63946` - Hover states, highlights, secondary actions
- **Deep Blue**: `#1E3A5F` - Contrast elements, secondary actions, depth
- **Ivory Cream**: `#F5F2E8` - Backgrounds, soft surfaces, warmth
- **Pure White**: `#FFFFFF` - Text on dark backgrounds, clean surfaces

### Usage Guidelines
- **Primary Red (#AC0929)**: Main CTAs, active states, selected menu items
- **Light Red (#E63946)**: Hover effects, unselected menu items, emphasis
- **Deep Blue (#1E3A5F)**: Success states, info elements, alternative actions
- **Ivory Cream (#F5F2E8)**: Main background, creates warm atmosphere
- **Pure White (#FFFFFF)**: Primary text on red/blue, cards, overlays

### Color Relationships
- Warm palette with red/cream base and blue accent
- High contrast between deep blue and ivory cream
- Red variations provide hierarchy
- All colors maintain WCAG AA contrast
- Use 80% opacity for disabled states

## Typography

### Font Family
- **Primary**: Lexend - Clean, modern, excellent readability
- **Fallback**: -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif
- **Chinese**: "PingFang SC", "Microsoft YaHei", sans-serif

### Type Scale - BIGGER IS BETTER!
- **MEGA**: 120px / 140px line-height / -0.03em letter-spacing - Game title
- **HUGE**: 96px / 112px line-height / -0.02em letter-spacing - Gestures
- **LARGE**: 72px / 84px line-height / -0.02em letter-spacing - Scores
- **BIG**: 56px / 64px line-height / -0.01em letter-spacing - Buttons
- **MEDIUM**: 40px / 48px line-height / 0em letter-spacing - Labels
- **NORMAL**: 32px / 40px line-height / 0em letter-spacing - Info text
- **SMALL**: 24px / 32px line-height / 0em letter-spacing - Minimum size

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
  /* Core 5-Color System */
  --color-primary: #AC0929;        /* Primary Red */
  --color-primary-light: #E63946;  /* Light Red */
  --color-accent: #1E3A5F;         /* Deep Blue */
  --color-background: #F5F2E8;     /* Ivory Cream */
  --color-white: #FFFFFF;          /* Pure White */
  
  /* Semantic Colors */
  --color-surface: var(--color-white);
  --color-text-primary: var(--color-primary);
  --color-text-secondary: var(--color-accent);
  --color-success: var(--color-accent);
  --color-warning: var(--color-primary-light);
  --color-disabled: rgba(172, 9, 41, 0.4);
  
  /* Typography */
  --font-family: 'Lexend', -apple-system, sans-serif;
  
  /* Spacing */
  --spacing-xs: 4px;
  --spacing-sm: 8px;
  --spacing-md: 16px;
  --spacing-lg: 24px;
  --spacing-xl: 32px;
  
  /* Shadows - using primary color */
  --shadow-sm: 0 2px 8px rgba(172, 9, 41, 0.1);
  --shadow-md: 0 4px 16px rgba(172, 9, 41, 0.15);
  --shadow-lg: 0 8px 32px rgba(172, 9, 41, 0.2);
}
```

### Unity-Specific Settings
- Canvas Scaler: Scale with screen size, 0.5 match
- Reference Resolution: 1170 x 2532 (iPhone 12)
- Default material: UI-Default with Pixel Perfect enabled
- Text: TextMeshPro with SDF rendering