import React from "react";
import { motion } from "framer-motion";
import "./Card.css";

/**
 * Generic Card component with Framer Motion support
 * Each card type will have unique animation variants
 */
const Card = ({
  id,
  title,
  description,
  animationType,
  animationVariants,
  children,
  className = "",
}) => {
  return (
    <motion.div
      className={`card card--${id} ${className}`}
      data-animation-type={animationType}
      initial="initial"
      whileInView="animate"
      whileHover="hover"
      viewport={{ once: true, amount: 0.3 }}
      variants={animationVariants}
    >
      {/* Animation overlay layer */}
      <div className="card__animation-layer" />

      {/* Content */}
      <div className="card__content">
        <h3 className="card__title">{title}</h3>
        <p className="card__description">{description}</p>
        {children}
      </div>

      {/* Border gradient effect */}
      <div className="card__border-gradient" />
    </motion.div>
  );
};

export default Card;
