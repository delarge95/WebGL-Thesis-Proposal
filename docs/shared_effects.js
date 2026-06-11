// ==========================================
// SHARED INTERACTIVE EFFECTS (Scrollytelling)
// ==========================================

document.addEventListener("DOMContentLoaded", () => {
  // --- 1. INJECT NOISE OVERLAY ---
  if (!document.getElementById("noiseOverlay")) {
    const noise = document.createElement("div");
    noise.id = "noiseOverlay";
    noise.className = "noise-overlay";
    // Inline SVG representing fine analog grain filter
    noise.innerHTML = `
      <svg viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg" style="width:100%; height:100%;">
        <filter id="noiseFilter">
          <feTurbulence type="fractalNoise" baseFrequency="0.8" numOctaves="3" stitchTiles="stitch"/>
        </filter>
        <rect width="100%" height="100%" filter="url(#noiseFilter)"/>
      </svg>
    `;
    document.body.appendChild(noise);
  }

  // --- 2. INJECT BACKGROUND MORPHING BLOBS ---
  let blobContainer = document.getElementById("blobContainer");
  if (!blobContainer) {
    blobContainer = document.createElement("div");
    blobContainer.id = "blobContainer";
    blobContainer.className = "blob-container";
    blobContainer.innerHTML = `
      <svg class="blob-svg blob-1" viewBox="0 0 1000 1000" xmlns="http://www.w3.org/2000/svg">
        <defs>
          <linearGradient id="grad1" x1="0%" y1="0%" x2="100%" y2="100%">
            <stop offset="0%" />
            <stop offset="100%" />
          </linearGradient>
        </defs>
        <path id="morphPath1" d="M830,600Q760,700,660,760Q560,820,460,770Q360,720,290,620Q220,520,280,420Q340,320,440,250Q540,180,640,250Q740,320,820,410Q900,500,830,600Z" fill="url(#grad1)" />
      </svg>
      <svg class="blob-svg blob-2" viewBox="0 0 1000 1000" xmlns="http://www.w3.org/2000/svg">
        <defs>
          <linearGradient id="grad2" x1="0%" y1="0%" x2="100%" y2="100%">
            <stop offset="0%" />
            <stop offset="100%" />
          </linearGradient>
        </defs>
        <path id="morphPath2" d="M780,620Q710,740,590,790Q470,840,360,770Q250,700,280,570Q310,440,390,320Q470,200,590,260Q710,320,780,410Q850,500,780,620Z" fill="url(#grad2)" />
      </svg>
    `;
    document.body.prepend(blobContainer);
  }

  // Run morphing animations in Anime.js
  if (window.anime) {
    const blob1Paths = [
      "M830,600Q760,700,660,760Q560,820,460,770Q360,720,290,620Q220,520,280,420Q340,320,440,250Q540,180,640,250Q740,320,820,410Q900,500,830,600Z",
      "M820,580Q720,680,620,780Q520,880,410,800Q300,720,260,600Q220,480,290,360Q360,240,490,210Q620,180,710,290Q800,400,850,490Q900,580,820,580Z",
      "M850,610Q770,720,670,710Q570,700,460,740Q350,780,280,670Q210,560,260,440Q310,320,420,270Q530,220,640,280Q750,340,830,440Q910,540,850,610Z"
    ];
    const blob2Paths = [
      "M780,620Q710,740,590,790Q470,840,360,770Q250,700,280,570Q310,440,390,320Q470,200,590,260Q710,320,780,410Q850,500,780,620Z",
      "M750,590Q650,690,540,740Q430,790,330,710Q230,630,280,500Q330,370,410,290Q490,210,610,250Q730,290,780,390Q830,490,750,590Z",
      "M810,640Q740,710,630,760Q520,810,410,740Q300,670,280,530Q260,390,360,300Q460,210,570,270Q680,330,760,420Q840,510,810,640Z"
    ];

    anime({
      targets: '#morphPath1',
      d: [
        { value: blob1Paths[0] },
        { value: blob1Paths[1] },
        { value: blob1Paths[2] },
        { value: blob1Paths[0] }
      ],
      duration: 18000,
      easing: 'easeInOutQuad',
      loop: true
    });

    anime({
      targets: '#morphPath2',
      d: [
        { value: blob2Paths[0] },
        { value: blob2Paths[1] },
        { value: blob2Paths[2] },
        { value: blob2Paths[0] }
      ],
      duration: 20000,
      easing: 'easeInOutQuad',
      loop: true
    });

    // Drifting motion
    anime({
      targets: '.blob-1',
      translateX: [-40, 40],
      translateY: [-30, 30],
      duration: 11000,
      direction: 'alternate',
      loop: true,
      easing: 'easeInOutSine'
    });

    anime({
      targets: '.blob-2',
      translateX: [50, -50],
      translateY: [40, -40],
      duration: 13000,
      direction: 'alternate',
      loop: true,
      easing: 'easeInOutSine'
    });
  }



  // --- 4. DEEP THEME SYNCHRONIZATION ---
  const body = document.body;
  const themeToggle = document.getElementById("themeToggle");

  const savedTheme = localStorage.getItem("theme");
  if (savedTheme === "light") {
    body.classList.add("light-mode");
  } else {
    body.classList.remove("light-mode");
  }

  if (themeToggle) {
    themeToggle.addEventListener("click", () => {
      body.classList.toggle("light-mode");
      const isLight = body.classList.contains("light-mode");
      localStorage.setItem("theme", isLight ? "light" : "dark");
      
      // Rotate effect
      if (window.gsap) {
        gsap.fromTo(themeToggle, { rotation: 0 }, { rotation: 180, duration: 0.6, ease: "power2.out" });
      }
    });
  }
});
