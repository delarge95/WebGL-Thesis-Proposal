/**
 * Clamps a value between min and max
 */
export function clamp(value, min, max) {
  return Math.min(Math.max(value, min), max);
}

/**
 * Linear interpolation between two values
 */
export function lerp(start, end, factor) {
  return start + (end - start) * factor;
}

/**
 * Maps a value from one range to another
 */
export function mapRange(value, inMin, inMax, outMin, outMax) {
  return ((value - inMin) * (outMax - outMin)) / (inMax - inMin) + outMin;
}

/**
 * Easing functions for animations
 */
export const easing = {
  // Cubic bezier easing
  smooth: [0.23, 1, 0.32, 1],
  
  // Spring-like easing
  spring: [0.68, -0.55, 0.265, 1.55],
  
  // Elastic easing
  elastic: [0.68, -0.6, 0.32, 1.6],
  
  // Standard easing
  inOut: [0.42, 0, 0.58, 1]
};

/**
 * Generates random value within range
 */
export function randomRange(min, max) {
  return Math.random() * (max - min) + min;
}

/**
 * Converts hex color to RGB
 */
export function hexToRgb(hex) {
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
  return result ? {
    r: parseInt(result[1], 16),
    g: parseInt(result[2], 16),
    b: parseInt(result[3], 16)
  } : null;
}
