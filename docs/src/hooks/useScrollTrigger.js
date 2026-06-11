import { useState, useEffect } from 'react';

/**
 * Custom hook for handling scroll-triggered animations
 * @param {number} threshold - Scroll percentage to trigger (0-1)
 * @returns {boolean} - Whether element is in view
 */
export function useScrollTrigger(threshold = 0.3) {
  const [isInView, setIsInView] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      const scrolled = window.scrollY;
      const height = window.innerHeight;
      const shouldTrigger = scrolled > height * threshold;
      setIsInView(shouldTrigger);
    };

    window.addEventListener('scroll', handleScroll);
    handleScroll(); // Check initial state

    return () => window.removeEventListener('scroll', handleScroll);
  }, [threshold]);

  return isInView;
}

/**
 * Custom hook for detecting element intersection
 * @param {Object} options - IntersectionObserver options
 * @returns {[React.RefObject, boolean]} - [ref, isIntersecting]
 */
export function useIntersectionObserver(options = {}) {
  const [isIntersecting, setIsIntersecting] = useState(false);
  const [ref, setRef] = useState(null);

  useEffect(() => {
    if (!ref) return;

    const observer = new IntersectionObserver(([entry]) => {
      setIsIntersecting(entry.isIntersecting);
    }, { threshold: 0.3, ...options });

    observer.observe(ref);

    return () => {
      if (ref) observer.unobserve(ref);
    };
  }, [ref, options]);

  return [setRef, isIntersecting];
}
