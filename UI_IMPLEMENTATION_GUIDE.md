# UI Implementation Guide

## Using the New Minimal Design

### Quick Setup
1. Add `MinimalUISetup.cs` to any GameObject in your scene
2. Right-click the component and select "Setup Minimal Jigupa Scene"
3. The entire UI will be created with the new design system

### Font Setup
The design uses Lexend font. To implement:

1. **Download Lexend Font**
   - Visit [Google Fonts - Lexend](https://fonts.google.com/specimen/Lexend)
   - Download the font family
   - Import into Unity: Assets > Fonts > Lexend

2. **Create TextMeshPro Font Asset**
   - Window > TextMeshPro > Font Asset Creator
   - Source Font: Select Lexend-Regular.ttf
   - Font Size: Auto Sizing
   - Atlas Resolution: 4096x4096
   - Character Set: Extended ASCII + Chinese Unicode Range
   - Generate and save as "Lexend-Regular SDF"

3. **Create Font Variants**
   - Repeat for: Lexend-Light, Lexend-Medium, Lexend-SemiBold
   - Name them accordingly: "Lexend-Light SDF", etc.

4. **Set as Default**
   - Edit > Project Settings > TextMeshPro > Settings
   - Default Font Asset: Lexend-Regular SDF

### Color Implementation
The design system uses these primary colors:
- Primary Red: `#AC0929` (172, 9, 41)
- Background: Pure white `#FFFFFF`
- Text: Off-black `#1A1A1A` (26, 26, 26)
- Accents: Player Blue `#2563EB`, AI Purple `#7C3AED`

### Key Design Elements

#### 1. Minimalist Layout
- Clean white background
- Generous spacing (48px between sections)
- Subtle shadows for depth
- No unnecessary borders or decorations

#### 2. Typography Hierarchy
- Display: 40px Light weight for main title
- Headings: 24px Semibold for section titles
- Body: 16-18px Regular for general text
- Labels: 14px Medium with letter-spacing for emphasis

#### 3. Interactive Elements
- Primary buttons: Red background with white text
- Gesture buttons: White circles with subtle borders
- Hover states: Slight scale and shadow changes
- Selected states: Fill with primary color

#### 4. Cards and Panels
- White backgrounds with subtle shadows
- 24px border radius for main cards
- 12px radius for smaller elements
- Consistent 32px padding

### Animation Guidelines

#### Button Interactions
```csharp
// Hover animation
transform.localScale = Vector3.one * 1.02f;
transitionDuration = 0.1f;

// Press animation
transform.localScale = Vector3.one * 0.98f;
transitionDuration = 0.1f;
```

#### Panel Transitions
```csharp
// Fade in
canvasGroup.alpha = 0f;
canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);

// Slide up
transform.localPosition = new Vector3(0, -20, 0);
transform.DOLocalMoveY(0, 0.3f).SetEase(Ease.OutQuad);
```

### Responsive Considerations

#### Safe Areas
The design accounts for device safe areas:
- Top: 100px offset for notch
- Bottom: 320px for controls + home indicator
- Sides: 16px minimum padding

#### Scaling
- Use Canvas Scaler set to "Scale With Screen Size"
- Reference: 1170x2532 (iPhone 12)
- Match: 0.5 (balanced width/height)

### Customization Tips

#### Changing Theme Color
To change from red to another color:
1. Update `primaryRed` in MinimalUISetup.cs
2. Adjust `primaryDark` to a darker shade
3. Update shadow colors to match

#### Adding New Components
Follow these patterns:
- Cards: White background, 24px radius, subtle shadow
- Buttons: 56px height, 12px radius, medium font weight
- Text: Consistent sizing from the type scale
- Spacing: Multiples of 8px

### Performance Notes
- Minimize overdraw with proper canvas sorting
- Use object pooling for gesture buttons
- Batch UI updates when possible
- Avoid transparent overlays except for modals

### Accessibility
- Maintain 4.5:1 contrast ratios
- 44px minimum touch targets
- Clear focus indicators
- Support for larger text sizes