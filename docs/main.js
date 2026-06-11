/* ==========================================
   DRONE VIEWER - GSAP ANIMATIONS
   Scrollytelling + Interactions
   ========================================== */

// Wait for DOM
document.addEventListener('DOMContentLoaded', () => {
  // Register GSAP plugins
  gsap.registerPlugin(ScrollTrigger);

  // Preloader
  const preloader = document.getElementById('preloader');
  const progress = document.querySelector('.preloader-progress');
  
  let loadProgress = 0;
  const loadInterval = setInterval(() => {
    loadProgress += Math.random() * 30;
    if (loadProgress >= 100) {
      loadProgress = 100;
      clearInterval(loadInterval);
      setTimeout(() => {
        gsap.to(preloader, {
          opacity: 0,
          duration: 0.5,
          onComplete: () => {
            preloader.style.display = 'none';
            initAnimations();
          }
        });
      }, 300);
    }
    progress.style.width = loadProgress + '%';
  }, 200);

  // Custom Cursor
  const cursor = document.getElementById('cursor');
  const follower = document.getElementById('cursorFollower');
  let mouseX = 0, mouseY = 0;
  let cursorX = 0, cursorY = 0;
  let followerX = 0, followerY = 0;

  document.addEventListener('mousemove', (e) => {
    mouseX = e.clientX;
    mouseY = e.clientY;
  });

  // Cursor animation loop
  function animateCursor() {
    cursorX += (mouseX - cursorX) * 0.2;
    cursorY += (mouseY - cursorY) * 0.2;
    followerX += (mouseX - followerX) * 0.1;
    followerY += (mouseY - followerY) * 0.1;

    cursor.style.left = cursorX + 'px';
    cursor.style.top = cursorY + 'px';
    follower.style.left = followerX + 'px';
    follower.style.top = followerY + 'px';

    requestAnimationFrame(animateCursor);
  }
  animateCursor();

  // Hover effects
  document.querySelectorAll('[data-hover]').forEach(el => {
    el.addEventListener('mouseenter', () => follower.classList.add('hover'));
    el.addEventListener('mouseleave', () => follower.classList.remove('hover'));
  });

  // Main animations
  function initAnimations() {
    // Hero animations
    gsap.from('.hero-tag', { opacity: 0, y: 30, duration: 0.8, delay: 0.2 });
    gsap.from('.title-line', { opacity: 0, y: 50, duration: 0.8, stagger: 0.1, delay: 0.3 });
    gsap.from('.hero-desc', { opacity: 0, y: 30, duration: 0.8, delay: 0.6 });
    gsap.from('.hero-actions', { opacity: 0, y: 30, duration: 0.8, delay: 0.7 });
    gsap.from('.hero-stats', { opacity: 0, y: 30, duration: 0.8, delay: 0.8 });
    gsap.from('.hero-visual', { opacity: 0, scale: 0.9, duration: 1, delay: 0.5, ease: 'power2.out' });
    gsap.from('.scroll-cue', { opacity: 0, duration: 0.8, delay: 1.2 });

    // Counter animation
    document.querySelectorAll('[data-count]').forEach(el => {
      const target = parseInt(el.dataset.count);
      gsap.to(el, {
        innerText: target,
        duration: 2,
        delay: 1,
        snap: { innerText: 1 },
        ease: 'power2.out',
        scrollTrigger: {
          trigger: el,
          start: 'top 80%',
          once: true
        }
      });
    });

    // Nav hide/show on scroll
    let lastScroll = 0;
    const nav = document.getElementById('nav');
    window.addEventListener('scroll', () => {
      const currentScroll = window.scrollY;
      if (currentScroll > lastScroll && currentScroll > 100) {
        nav.style.transform = 'translateY(-100%)';
      } else {
        nav.style.transform = 'translateY(0)';
      }
      lastScroll = currentScroll;
    });

    // Features section
    gsap.from('.features-header', {
      opacity: 0, y: 50, duration: 0.8,
      scrollTrigger: { trigger: '.features-header', start: 'top 80%' }
    });

    gsap.from('.feature-card', {
      opacity: 0, y: 50, duration: 0.6, stagger: 0.1,
      scrollTrigger: { trigger: '.features-track', start: 'top 80%' }
    });

    // Scrollytelling
    const storyPanels = document.querySelectorAll('.story-panel');
    const storyDrone = document.querySelector('.story-drone');

    storyPanels.forEach((panel, i) => {
      ScrollTrigger.create({
        trigger: panel,
        start: 'top center',
        end: 'bottom center',
        onEnter: () => {
          storyPanels.forEach(p => p.classList.remove('active'));
          panel.classList.add('active');
          updateDroneState(i + 1);
        },
        onEnterBack: () => {
          storyPanels.forEach(p => p.classList.remove('active'));
          panel.classList.add('active');
          updateDroneState(i + 1);
        }
      });
    });

    function updateDroneState(step) {
      storyDrone.className = 'story-drone';
      if (step === 2) storyDrone.classList.add('exploded');
      if (step === 3) storyDrone.classList.add('xray');
      if (step === 4) storyDrone.classList.add('tools');
    }

    // Tech cards
    gsap.from('.tech-card', {
      opacity: 0, y: 30, duration: 0.5, stagger: 0.05,
      scrollTrigger: { trigger: '.tech-grid', start: 'top 80%' }
    });

    // Doc cards
    gsap.from('.doc-card', {
      opacity: 0, y: 30, duration: 0.5, stagger: 0.05,
      scrollTrigger: { trigger: '.docs-grid', start: 'top 80%' }
    });

    // Timeline
    gsap.from('.timeline-item', {
      opacity: 0, x: -30, duration: 0.6, stagger: 0.15,
      scrollTrigger: { trigger: '.timeline-track', start: 'top 80%' }
    });

    // CTA
    gsap.from('.cta-content', {
      opacity: 0, y: 50, duration: 0.8,
      scrollTrigger: { trigger: '.cta', start: 'top 80%' }
    });

    // Parallax orbs
    gsap.to('.hero-orb-1', {
      y: -100,
      scrollTrigger: {
        trigger: '.hero',
        start: 'top top',
        end: 'bottom top',
        scrub: 1
      }
    });

    gsap.to('.hero-orb-2', {
      y: -150,
      scrollTrigger: {
        trigger: '.hero',
        start: 'top top',
        end: 'bottom top',
        scrub: 1
      }
    });
  }

  // Smooth scroll for anchor links
  document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function(e) {
      e.preventDefault();
      const target = document.querySelector(this.getAttribute('href'));
      if (target) {
        gsap.to(window, {
          scrollTo: target,
          duration: 1,
          ease: 'power2.inOut'
        });
      }
    });
  });
});
