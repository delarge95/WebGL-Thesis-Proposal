import React from 'react';
import { motion } from 'framer-motion';
import Card from '../cards/Card';
import { TECNOLOGIA_DATA } from '../../constants/data';
import { SCROLL_REVEAL, STAGGER_CONTAINER } from '../../constants/animations';
import './Section.css';

// Import animation variants for each card (will be created later)
import { getTecnologiaAnimationVariants } from './animationVariants';

const TecnologiaSection = () => {
  return (
    <section className="section section--tecnologia">
      <div className="container">
        <motion.div 
          className="section__header"
          {...SCROLL_REVEAL}
        >
          <h2 className="section__title">Tecnología</h2>
        </motion.div>

        <motion.div 
          className="grid-minimal"
          variants={STAGGER_CONTAINER}
          initial="initial"
          whileInView="animate"
          viewport={{ once: true, amount: 0.3 }}
        >
          {TECNOLOGIA_DATA.map((card) => (
            <Card
              key={card.id}
              {...card}
              animationVariants={getTecnologiaAnimationVariants(card.animationType)}
            />
          ))}
        </motion.div>
      </div>
    </section>
  );
};

export default TecnologiaSection;
