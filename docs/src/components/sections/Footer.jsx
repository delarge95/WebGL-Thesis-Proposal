import React from 'react';
import { motion } from 'framer-motion';
import { GITHUB_URL } from '../../constants/data';
import './Footer.css';

const Footer = () => {
  return (
    <footer className="footer">
      <div className="container">
        <motion.div
          initial={{ opacity: 0 }}
          whileInView={{ opacity: 1 }}
          viewport={{ once: true }}
          transition={{ duration: 0.6 }}
        >
          <p className="footer__text">
            TwinSight X500 © 2025 —
            <a
              href={GITHUB_URL}
              target="_blank"
              rel="noopener noreferrer"
              className="footer__link"
            >
              GitHub
            </a>
          </p>
        </motion.div>
      </div>
    </footer>
  );
};

export default Footer;
