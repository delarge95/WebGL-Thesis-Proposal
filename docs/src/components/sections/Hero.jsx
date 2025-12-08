import React from 'react';
import { motion } from 'framer-motion';
import './Hero.css';

const Hero = () => {
  return (
    <section className="hero">
      {/* Hero graphic background */}
      <div className="hero__graphic" id="heroGraphic" />
      
      <div className="hero__content">
        <motion.h1 
          className="hero__title"
          initial={{ opacity: 0, y: 40 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.2, ease: [0.23, 1, 0.32, 1] }}
        >
          Visualizador WebGL
        </motion.h1>
        
        <motion.p 
          className="hero__subtitle"
          initial={{ opacity: 0, y: 40 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.4, ease: [0.23, 1, 0.32, 1] }}
        >
          Exploración interactiva 3D de drones en el navegador
        </motion.p>
        
        <motion.button 
          className="hero__cta btn-start"
          initial={{ opacity: 0, y: 40 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.6, ease: [0.23, 1, 0.32, 1] }}
          whileHover={{ scale: 1.05 }}
          whileTap={{ scale: 0.95 }}
        >
          Iniciar Demo
        </motion.button>
      </div>
    </section>
  );
};

export default Hero;
