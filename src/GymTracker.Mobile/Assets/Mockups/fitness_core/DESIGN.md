---
name: Fitness Core
colors:
  surface: '#f8f9ff'
  surface-dim: '#ccdbf4'
  surface-bright: '#f8f9ff'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#eff4ff'
  surface-container: '#e6eeff'
  surface-container-high: '#dde9ff'
  surface-container-highest: '#d5e3fd'
  on-surface: '#0d1c2f'
  on-surface-variant: '#434656'
  inverse-surface: '#233144'
  inverse-on-surface: '#ebf1ff'
  outline: '#737688'
  outline-variant: '#c3c5d9'
  surface-tint: '#004ced'
  primary: '#003ec7'
  on-primary: '#ffffff'
  primary-container: '#0052ff'
  on-primary-container: '#dfe3ff'
  inverse-primary: '#b7c4ff'
  secondary: '#076e00'
  on-secondary: '#ffffff'
  secondary-container: '#18f900'
  on-secondary-container: '#076d00'
  tertiary: '#5c24c6'
  on-tertiary: '#ffffff'
  tertiary-container: '#7544df'
  on-tertiary-container: '#ebe0ff'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#dde1ff'
  primary-fixed-dim: '#b7c4ff'
  on-primary-fixed: '#001452'
  on-primary-fixed-variant: '#0038b6'
  secondary-fixed: '#78ff5f'
  secondary-fixed-dim: '#16e600'
  on-secondary-fixed: '#012200'
  on-secondary-fixed-variant: '#045300'
  tertiary-fixed: '#e9ddff'
  tertiary-fixed-dim: '#d0bcff'
  on-tertiary-fixed: '#23005c'
  on-tertiary-fixed-variant: '#5516be'
  background: '#f8f9ff'
  on-background: '#0d1c2f'
  surface-variant: '#d5e3fd'
typography:
  display:
    fontFamily: Inter
    fontSize: 48px
    fontWeight: '800'
    lineHeight: 56px
    letterSpacing: -0.02em
  h1:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
    letterSpacing: -0.01em
  h2:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '700'
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
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
    letterSpacing: 0.05em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 8px
  margin-mobile: 24px
  gutter: 16px
  stack-sm: 8px
  stack-md: 16px
  stack-lg: 32px
  section-gap: 48px
---

## Brand & Style

The personality of this design system is performance-driven, energetic, and precise. It targets fitness enthusiasts who value data clarity and high-intensity motivation. The design style combines **Minimalism** with **High-Contrast** elements to create a UI that feels like professional athletic gear—functional, durable, and sleek.

The user experience focuses on reducing cognitive load during workouts by utilizing generous whitespace and a "content-first" architecture. The aesthetic transition between light and dark modes reflects the user's journey from daylight activity to premium, focused recovery or late-night training sessions.

## Colors

This design system utilizes two distinct color environments to match the user's context:

**Light Mode (Active/Day):**
*   **Background:** Pure White (`#FFFFFF`) for maximum clarity and a clean slate.
*   **Text:** Slate Gray (`#334155`) to provide high readability without the harshness of pure black.
*   **Accents:** Electric Blue (`#0052FF`) for primary actions and Lime Green (`#19FF00`) for success states, progress tracking, and "Go" signals.

**Dark Mode (Premium/Night):**
*   **Background:** Obsidian (`#0D0D0D`) with Charcoal (`#1A1A1A`) surface layers.
*   **Text:** Crisp White (`#F8FAFC`) for high-legibility contrast.
*   **Accents:** Neon Cyan (`#00F0FF`) and Electric Violet (`#BF00FF`) to evoke a high-tech, futuristic training atmosphere.

## Typography

This design system relies on **Inter** to deliver a systematic and utilitarian feel. The hierarchy is characterized by significant scale jumps between headlines and body text to guide the eye quickly during physical activity. 

Large "Display" styles are reserved for high-impact metrics (like heart rate or calories), while "Label-caps" are used for metadata and secondary categorization. Generous line-height is applied across all body styles to prevent text-crowding and ensure legibility while the user is in motion.

## Layout & Spacing

The layout philosophy is built on a **Fluid Grid** with a strict 8px rhythmic scale. For mobile contexts, a 4-column grid is used with 24px outer margins to provide a wide, breathable frame for content. 

Horizontal spacing (gutters) is fixed at 16px to maintain structural integrity, while vertical spacing (stacking) utilizes larger gaps (32px+) to separate distinct training modules or data sets. This "breathable" approach ensures that even data-heavy dashboards feel approachable and organized.

## Elevation & Depth

Visual hierarchy is managed differently across the two themes to maintain the minimalist aesthetic:

*   **Light Mode:** Employs **Ambient Shadows**. Surfaces use soft, diffused shadows with a low opacity (4-8%) and a subtle blue tint to lift cards off the white background without creating "muddy" edges.
*   **Dark Mode:** Utilizes **Low-contrast outlines** and **Tonal Layers**. Instead of shadows, surfaces are defined by 1px thin borders in a slightly lighter charcoal shade. Depth is achieved by stepping the background color from Obsidian (base) to Charcoal (elevated cards).

Backdrop blurs (10px–20px) are used sparingly on navigation bars and modals to maintain context while focusing the user on the primary interaction.

## Shapes

The shape language is defined by a consistent **16px (1rem)** corner radius for all primary containers and cards. This large radius softens the high-contrast color palette, making the app feel more engaging and "human."

Buttons follow a **Pill-shaped** convention (full rounding) to clearly distinguish interactive elements from informational cards. Small elements like input checkboxes or tags utilize a 4px radius to maintain a precise, technical look at smaller scales.

## Components

**Buttons**
Action buttons are high-contrast. In Light Mode, primary buttons are solid Electric Blue with white text. In Dark Mode, they transition to Neon Cyan with black text for a "glowing" effect. Secondary buttons use a heavy 2px border with no fill.

**Cards**
Cards are the primary container for workout data. They feature a 16px radius. In Light Mode, they have a subtle shadow; in Dark Mode, they have a 1px border (`#FFFFFF10`). Padding within cards should never be less than 20px.

**Inputs & Fields**
Input fields use a subtle background fill (Slate 50 in Light, Charcoal in Dark) with a bottom-heavy focus state. When focused, the border color shifts to the Primary Accent.

**Progress Elements**
Progress bars and rings use the Secondary Accent (Lime Green or Violet) to visualize completion. Background tracks for these elements should be low-contrast (Slate 100 or Obsidian-Light).

**Activity Lists**
List items are separated by generous whitespace rather than dividers wherever possible. If dividers are necessary, they should be 1px thick and use the lowest contrast neutral color available in the theme.