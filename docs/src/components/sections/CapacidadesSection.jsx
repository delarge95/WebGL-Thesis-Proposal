import React from 'react';
import { motion } from 'framer-motion';
import Card from '../cards/Card';
import { CAPACIDADES_DATA } from '../../constants/data';
import { SCROLL_REVEAL, STAGGER_CONTAINER } from '../../constants/animations';
import './Section.css';

// Import animation variants for each card (will be created later)
import { getCapacidadAnimationVariants } from './animationVariants';

const CapacidadesSection = () => {
  return (
    <section className="section section--capacidades">
      <div className="container">
        <motion.div 
          className="section__header"
          {...SCROLL_REVEAL}
        >
          <h2 className="section__title">Capacidades</h2>
        </motion.div>

        <motion.div 
          className="grid-minimal"
          variants={STAGGER_CONTAINER}
          initial="initial"
          whileInView="animate"
          viewport={{ once: true, amount: 0.3 }}
        >
          {CAPACIDADES_DATA.map((card) => (
            <Card
              key={card.id}
              {...card}
              animationVariants={getCapacidadAnimationVariants(card.animationType)}
            />
          ))}
        </motion.div>
      </div>
    </section>
  );
};

export default CapacidadesSection;
