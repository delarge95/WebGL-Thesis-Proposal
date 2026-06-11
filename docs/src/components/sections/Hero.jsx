import React, { useState } from 'react';
import { motion } from 'framer-motion';
import './Hero.css';

const Hero = () => {
  const [language, setLanguage] = useState('en');
  const copy = {
    en: {
      subtitle: 'Visual-semantic WebGL viewer',
      cta: 'Open viewer',
    },
    es: {
      subtitle: 'Visor WebGL visual-semántico',
      cta: 'Abrir visor',
    },
  }[language];

  return (
    <section className="hero">
      {/* Hero graphic background */}
      <div className="hero__graphic" id="heroGraphic" />

      <div className="hero__language" aria-label="Language selector">
        <button
          className={language === 'en' ? 'is-active' : ''}
          type="button"
          aria-pressed={language === 'en'}
          onClick={() => setLanguage('en')}
        >
          EN
        </button>
        <button
          className={language === 'es' ? 'is-active' : ''}
          type="button"
          aria-pressed={language === 'es'}
          onClick={() => setLanguage('es')}
        >
          ES
        </button>
      </div>
      
      <div className="hero__content">
        <motion.h1 
          className="hero__title"
          initial={{ opacity: 0, y: 40 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.2, ease: [0.23, 1, 0.32, 1] }}
        >
          TwinSight X500
        </motion.h1>
        
        <motion.p 
          className="hero__subtitle"
          initial={{ opacity: 0, y: 40 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.4, ease: [0.23, 1, 0.32, 1] }}
        >
          {copy.subtitle}
        </motion.p>
        
        <motion.button 
          className="hero__cta btn-start"
          initial={{ opacity: 0, y: 40 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.6, ease: [0.23, 1, 0.32, 1] }}
          whileHover={{ scale: 1.05 }}
          whileTap={{ scale: 0.95 }}
        >
          {copy.cta}
        </motion.button>
      </div>
    </section>
  );
};

export default Hero;
