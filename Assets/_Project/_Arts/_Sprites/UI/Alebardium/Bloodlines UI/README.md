# Bloodlines UI — Dark/Gothic UI Kit for Unity

A comprehensive interface collection in dark fantasy/gothic aesthetics featuring deep black backgrounds, contrasting red accents, and runic motifs. Ready for quick implementation in Unity through prefabs and textures.

## Features

- **Buttons**: 6 prefabs
  - 3 designs × 2 color themes (Gray/Red)
  - Interactive states for each design: Default, Hover, Pressed, Disabled
  - Textures: `Textures/Button/Button#1..#3`
- **Sliders**: 13 prefabs
  - 5 horizontal (`Slider #1..#5 (Horizontal)`)
  - 4 round (`Slider #1..#2 (Round)` and `With Runes` variants)
  - 4 segmented (`Slider #1..#4 (Segments)`, including handle variant)
  - Additional textures: `Textures/Slider` (rect/rod/diamond, empty/full, toggler)
- **Toggles**: 6 prefabs
  - Round and square variants, 3 styles each (`Toggle #1..#3`)
- **Icons**: 4 prefabs (`Prefabs/Icon`)
  - Additional icon sets in `Textures/Icon`
- **Frames & Backgrounds**: Decorative frame and background elements in `Textures/Frame`
- **Progress Bars** (textures):
  - Rectangular: `Textures/Progress_Bar/Rectangle` (empty/full v1–v5)
  - Round: `Textures/Progress_Bar/Round` (empty/full v1–v4)
- **Fonts**: `Font/`
  - Cloister Black Light (TTF + SDF)
  - Typographer Rotunda (TTF + SDF)
- **Demo Scene**: `Scenes/Demo Scene (Bloodlines UI).unity`
- **Animation**: Preview controllers and clips for sliders in `Animation/`

## Scripts and Components

The kit includes several utility scripts for enhanced functionality:

### Core Components
- **`SoundManager`**: Global audio management with persistent settings
  - Volume control and sound enable/disable
  - Hover and click sound scaling
  - PlayerPrefs integration for settings persistence
- **`SliderTextSynchronizer`**: Synchronizes slider values with text display
  - Real-time percentage display
  - Customizable text format
  - Automatic value monitoring
- **`ToggleSliderController`**: Links toggle state to slider control
  - Stores/restores original slider values
  - Zero-out functionality when toggle is disabled

### UI Enhancement Scripts
- **`ButtonSFX`**: Button sound effect management
  - Hover and click sound support
  - Integration with SoundManager
- **`ToggleSFX`**: Toggle sound effect management
  - Similar to ButtonSFX but for toggle elements
- **`ButtonTextColorState`**: Dynamic text color management for buttons
  - State-based color changes (default, highlighted, pressed, disabled)
  - Automatic interaction detection

### Scene Management
- **`SceneManager`**: Local scene switching (GameObject-based)
  - Loading scene support
  - Scene index management
- **`LoadingSceneManager`**: Loading animation controller
  - Progress bar and text animation
  - Customizable timing and duration
- **`FastDontDestroyOnLoad`**: Quick persistent object utility

## Style and Purpose

- **Aesthetic**: Dark backgrounds, saturated red accents, gothic/vampiric decorative elements
- **Application**: RPG, horror, dark fantasy, dungeon crawler, grim settings

## How to Use

1. Add desired prefabs from `Prefabs/` to your `Canvas`
2. Replace sprites in `Image` components with alternatives from `Textures/` if needed
3. For progress bars, use `Image` type = Filled (linear/radial) with corresponding sprites from `Textures/Progress_Bar`
4. Round sliders can be styled with rune variants or without by replacing sprites/prefabs
5. Add sound effects by attaching `ButtonSFX` or `ToggleSFX` components to interactive elements
6. Use `SoundManager` singleton for global audio control
7. Implement `SliderTextSynchronizer` for automatic slider value display

## Project Structure

```
Bloodlines UI/
├── Prefabs/          # Ready-to-use UI prefabs
├── Textures/         # Sprite assets organized by component type
├── Font/             # TTF and SDF font assets
├── Script/           # Utility scripts and components
├── Animation/        # Animation controllers and clips
└── Scenes/           # Demo scene
```

## Component Dependencies

- TextMeshPro (for text components)
- Unity UI (built-in)
- Unity Audio (for sound effects)

## Performance Notes

- All scripts are optimized for runtime performance
- SoundManager uses singleton pattern for efficient memory usage
- Text synchronization uses value change detection to minimize updates
- Audio clips are played through pooled AudioSource components