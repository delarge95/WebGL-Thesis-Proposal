import { useState, useEffect } from 'react';

/**
 * Custom hook for managing card hover states with debouncing
 * @param {number} delay - Debounce delay in ms
 * @returns {[boolean, function, function]} - [isHovered, handleMouseEnter, handleMouseLeave]
 */
export function useCardHover(delay = 100) {
  const [isHovered, setIsHovered] = useState(false);
  const [timeoutId, setTimeoutId] = useState(null);

  const handleMouseEnter = () => {
    if (timeoutId) clearTimeout(timeoutId);
    setIsHovered(true);
  };

  const handleMouseLeave = () => {
    const id = setTimeout(() => {
      setIsHovered(false);
    }, delay);
    setTimeoutId(id);
  };

  useEffect(() => {
    return () => {
      if (timeoutId) clearTimeout(timeoutId);
    };
  }, [timeoutId]);

  return [isHovered, handleMouseEnter, handleMouseLeave];
}
