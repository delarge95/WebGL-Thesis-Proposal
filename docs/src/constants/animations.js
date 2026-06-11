// Animation variants for Framer Motion
export const FADE_IN_UP = {
  initial: { opacity: 0, y: 40 },
  animate: { opacity: 1, y: 0 },
  transition: { duration: 0.8, ease: [0.23, 1, 0.32, 1] }
};

export const STAGGER_CONTAINER = {
  animate: {
    transition: {
      staggerChildren: 0.1
    }
  }
};

// Base card hover variants (will be extended per card)
export const CARD_BASE_HOVER = {
  initial: { opacity: 0, y: 0 },
  whileHover: { scale: 1.02 },
  transition: { duration: 0.4, ease: [0.23, 1, 0.32, 1] }
};

// Scroll animation trigger settings
export const SCROLL_REVEAL = {
  initial: { opacity: 0, y: 40 },
  whileInView: { opacity: 1, y: 0 },
  viewport: { once: true, amount: 0.3 },
  transition: { duration: 0.8, ease: [0.23, 1, 0.32, 1] }
};
