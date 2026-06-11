// CSS Custom Properties
export const CSS_VARS = {
  bgPrimary: '#0a0a0a',
  bgSecondary: '#121212',
  textMain: '#ffffff',
  textMuted: 'rgba(255, 255, 255, 0.4)',
  border: 'rgba(255, 255, 255, 0.08)',
  
  // Card-specific colors
  colors: {
    visualizacion: '#3b82f6', // Blue
    interaccion: '#10b981',   // Green
    ingenieria: '#8b5cf6',    // Purple
    unity: '#f59e0b',         // Amber
    webgl: '#ef4444',         // Red
    webassembly: '#06b6d4',   // Cyan
    hlsl: '#ec4899'           // Pink
  }
};

// Breakpoints
export const BREAKPOINTS = {
  mobile: '768px',
  tablet: '1024px',
  desktop: '1440px'
};

// Z-index layers
export const Z_INDEX = {
  base: 1,
  card: 10,
  cardHover: 20,
  overlay: 100,
  modal: 200,
  navigation: 300
};
