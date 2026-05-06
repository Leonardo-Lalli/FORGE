---
name: Cyber-Athletic Elite
colors:
  surface: '#131313'
  surface-dim: '#131313'
  surface-bright: '#3a3939'
  surface-container-lowest: '#0e0e0e'
  surface-container-low: '#1c1b1b'
  surface-container: '#201f1f'
  surface-container-high: '#2a2a2a'
  surface-container-highest: '#353534'
  on-surface: '#e5e2e1'
  on-surface-variant: '#bac9cc'
  inverse-surface: '#e5e2e1'
  inverse-on-surface: '#313030'
  outline: '#849396'
  outline-variant: '#3b494c'
  surface-tint: '#00daf3'
  primary: '#c3f5ff'
  on-primary: '#00363d'
  primary-container: '#00e5ff'
  on-primary-container: '#00626e'
  inverse-primary: '#006875'
  secondary: '#ffffff'
  on-secondary: '#283500'
  secondary-container: '#c3f400'
  on-secondary-container: '#556d00'
  tertiary: '#efeceb'
  on-tertiary: '#313030'
  tertiary-container: '#d2d0cf'
  on-tertiary-container: '#5a5959'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#9cf0ff'
  primary-fixed-dim: '#00daf3'
  on-primary-fixed: '#001f24'
  on-primary-fixed-variant: '#004f58'
  secondary-fixed: '#c3f400'
  secondary-fixed-dim: '#abd600'
  on-secondary-fixed: '#161e00'
  on-secondary-fixed-variant: '#3c4d00'
  tertiary-fixed: '#e5e2e1'
  tertiary-fixed-dim: '#c8c6c5'
  on-tertiary-fixed: '#1c1b1b'
  on-tertiary-fixed-variant: '#474746'
  background: '#131313'
  on-background: '#e5e2e1'
  surface-variant: '#353534'
typography:
  display-hero:
    fontFamily: Space Grotesk
    fontSize: 72px
    fontWeight: '700'
    lineHeight: 72px
    letterSpacing: -0.04em
  metric-xl:
    fontFamily: Space Grotesk
    fontSize: 48px
    fontWeight: '600'
    lineHeight: 48px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Space Grotesk
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
  headline-md:
    fontFamily: Space Grotesk
    fontSize: 24px
    fontWeight: '500'
    lineHeight: 32px
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  label-caps:
    fontFamily: Lexend
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
    letterSpacing: 0.1em
  label-sm:
    fontFamily: Lexend
    fontSize: 11px
    fontWeight: '400'
    lineHeight: 14px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 4px
  container-margin: 24px
  gutter: 16px
  stack-sm: 8px
  stack-md: 16px
  stack-lg: 32px
  section-gap: 48px
---

## Brand & Style

This design system targets high-performance athletes and tech-forward fitness enthusiasts. The brand personality is aggressive, precise, and elite, evoking the feeling of a high-tech training lab. 

The aesthetic is a fusion of **Glassmorphism** and **High-Contrast Minimalism**. It relies on deep obsidian depths to make neon telemetry data "pop," creating a sense of infinite digital space. The interface should feel like a heads-up display (HUD), emphasizing real-time data and physical momentum through sleek, translucent layers and energetic glowing accents.

## Colors

The palette is strictly dark-mode to maximize the luminosity of functional accents. 

- **Primary (Electric Blue):** Used for primary actions, progress indicators, and "active" states. It represents connectivity and flow.
- **Secondary (Cyber Lime):** Reserved for peak performance metrics, high-intensity goals, and success states. It represents energy and kinetic power.
- **Backgrounds:** A mix of pure black (#000000) for deep contrast and Deep Charcoal (#0A0A0A) for structural surfaces.
- **Accents:** Vibrant glows are derived from the Primary and Secondary colors at 20-40% opacity to create "light leaks" and depth.

## Typography

This design system utilizes a high-tech typography stack to balance data density with editorial style.

- **Headlines & Metrics:** `Space Grotesk` provides a geometric, futuristic feel. For key metrics (e.g., heart rate, pace), use tight tracking and bold weights to mimic digital instrumentation.
- **Body Text:** `Inter` is used for readability in descriptions and settings, providing a neutral balance to the aggressive display type.
- **Labels:** `Lexend` is employed for captions and small UI labels to maintain an "athletic" and clear readability even at small scales. 

Incorporate "Typography Hybrids" by pairing large `Space Grotesk` numbers with small `Lexend` uppercase units (e.g., "145 BPM") to create visual hierarchy within data points.

## Layout & Spacing

The layout follows a **fluid grid** model with generous outer margins to frame the content like a premium editorial. 

- **Grid:** Use a 12-column grid for tablet/desktop and a 4-column grid for mobile. 
- **Rhythm:** An 8px base grid governs all spatial relationships. 
- **Visual Breathing Room:** While the content is high-contrast, large gaps (`section-gap`) are used between different workout modules to prevent cognitive overload. 
- **Alignment:** Consistent left-alignment for text, while metrics are often centered within "organic" card containers to create focal points.

## Elevation & Depth

Depth is achieved through **Glassmorphism** rather than traditional drop shadows.

- **Surface Layers:** Surfaces use a background blur (16px to 32px) and a semi-transparent charcoal fill (e.g., `rgba(26, 26, 26, 0.6)`).
- **Edge Definition:** Instead of shadows, use 1px "inner glows" or borders with a linear gradient (top-left to bottom-right) from white at 15% opacity to transparent.
- **Glow Accents:** High-priority elements (like active workout cards) should have a soft, diffused outer glow matching the element's primary color, with a spread of 20px and 20% opacity.
- **Z-Axis:** Lower levels are pure black; interactive cards sit on the glass layer; floating action buttons sit on the highest layer with the strongest glow.

## Shapes

The shape language is "organic-tech." While the grid is rigid, the containers are soft and fluid.

- **Organic Cards:** Use `rounded-xl` (1.5rem) for main content cards. For a more "organic" feel, consider asymmetrical rounding (e.g., top-left and bottom-right at 2rem, others at 1rem) for featured modules.
- **Interactive Elements:** Buttons and input fields use `rounded-lg` (1rem) to maintain a modern, approachable feel.
- **Pills:** Use for status indicators, chips, and tags to differentiate them from structural cards.

## Components

- **Buttons:** High-contrast backgrounds. Primary buttons use a solid Electric Blue fill with black text and a 10px soft blue outer glow. Secondary buttons use a ghost style with a Cyber Lime border.
- **Glass Cards:** Semi-transparent containers with a subtle 1px border. Background blur is mandatory to maintain legibility over moving workout videos or gradients.
- **Input Fields:** Minimalist lines. Only the bottom border is visible by default; on focus, the border turns Electric Blue with a soft neon "under-glow."
- **Progress Rings:** Use thick strokes with rounded caps. The "track" is a dark grey, while the "progress" is a gradient from Electric Blue to Cyber Lime.
- **Line-Art Icons:** Ultra-thin (1.5pt) stroke icons. Icons should be monochrome (white) unless they are active, in which case they take the Primary Electric Blue color.
- **Metric Widgets:** Large-scale typography paired with a sparkline (mini-graph) that uses a glow effect on the data path.