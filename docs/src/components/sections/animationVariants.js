/**
 * Animation variants for Capacidades cards
 * These will be implemented with unique Framer Motion animations
 */

// Card 01 - Visualización: X-ray scan effect
export const visualizacionVariants = {
  initial: { opacity: 0, y: 40 },
  animate: { 
    opacity: 1, 
    y: 0,
    transition: { duration: 0.8, ease: [0.23, 1, 0.32, 1] }
  },
  hover: {
    // TODO: Implement X-ray scan effect
    // - Layer reveal animation
    // - Subtle pulse glow (blue)
    // - Scan line effect
    scale: 1.02,
    transition: { duration: 0.4 }
  }
};

// Card 02 - Interacción: Explosive despiece
export const interaccionVariants = {
  initial: { opacity: 0, y: 40 },
  animate: { 
    opacity: 1, 
    y: 0,
    transition: { duration: 0.8, ease: [0.23, 1, 0.32, 1] }
  },
  hover: {
    // TODO: Implement explosive despiece
    // - Floating particles
    // - Y-axis lift
    // - Stagger child elements
    y: -8,
    transition: { duration: 0.5 }
  }
};

// Card 03 - Ingeniería: Blueprint measurement lines
export const ingenieriaVariants = {
  initial: { opacity: 0, y: 40 },
  animate: { 
    opacity: 1, 
    y: 0,
    transition: { duration: 0.8, ease: [0.23, 1, 0.32, 1] }
  },
  hover: {
    // TODO: Implement blueprint lines
    // - Measurement lines appearing
    // - Precision metrics overlay
    // - Scan line effect (purple)
    transition: { duration: 0.4 }
  }
};

/**
 * Animation variants for Tecnología cards
 */

// Card 04 - Unity: Wireframe → solid render
export const unityVariants = {
  initial: { opacity: 0, y: 40 },
  animate: { 
    opacity: 1, 
    y: 0,
    transition: { duration: 0.8, ease: [0.23, 1, 0.32, 1] }
  },
  hover: {
    // TODO: Implement wireframe render
    // - Wireframe overlay reveal
    // - Glow effect (amber)
    // - Subtle rotation hint
    transition: { duration: 0.6 }
  }
};

// Card 05 - WebGL: RGB glitch + fresnel
export const webglVariants = {
  initial: { opacity: 0, y: 40 },
  animate: { 
    opacity: 1, 
    y: 0,
    transition: { duration: 0.8, ease: [0.23, 1, 0.32, 1] }
  },
  hover: {
    // TODO: Implement RGB glitch
    // - Chromatic aberration
    // - Fresnel gradient edge
    // - Digital glitch effect (red)
    transition: { duration: 0.4 }
  }
};

// Card 06 - WebAssembly: Binary compilation
export const webassemblyVariants = {
  initial: { opacity: 0, y: 40 },
  animate: { 
    opacity: 1, 
    y: 0,
    transition: { duration: 0.8, ease: [0.23, 1, 0.32, 1] }
  },
  hover: {
    // TODO: Implement binary compile
    // - Text → binary transformation
    // - Compile lines flowing
    // - Matrix-style effect (cyan)
    transition: { duration: 0.6 }
  }
};

// Card 07 - HLSL: Chromatic aberration
export const hlslVariants = {
  initial: { opacity: 0, y: 40 },
  animate: { 
    opacity: 1, 
    y: 0,
    transition: { duration: 0.8, ease: [0.23, 1, 0.32, 1] }
  },
  hover: {
    // TODO: Implement chromatic aberration
    // - RGB split effect
    // - Color grading flow
    // - Gradient mesh animation (pink)
    transition: { duration: 0.5 }
  }
};

/**
 * Helper functions to get variants by animation type
 */
export function getCapacidadAnimationVariants(animationType) {
  const variantsMap = {
    'xray-scan': visualizacionVariants,
    'explosive-despiece': interaccionVariants,
    'blueprint-lines': ingenieriaVariants
  };
  return variantsMap[animationType] || visualizacionVariants;
}

export function getTecnologiaAnimationVariants(animationType) {
  const variantsMap = {
    'wireframe-render': unityVariants,
    'rgb-glitch': webglVariants,
    'binary-compile': webassemblyVariants,
    'chromatic-aberration': hlslVariants
  };
  return variantsMap[animationType] || unityVariants;
}
