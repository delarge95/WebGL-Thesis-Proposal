# WebGL Deployment Guide
## Hosting the Drone Viewer on the Web

---

## Deployment Options

| Platform | Cost | Difficulty | Best For |
|----------|------|------------|----------|
| **GitHub Pages** | Free | Easy | Static hosting, academic demos |
| **Vercel** | Free tier | Easy | Custom domains, CDN |
| **Netlify** | Free tier | Easy | Continuous deployment |
| **AWS S3 + CloudFront** | Pay-per-use | Medium | Production, global CDN |
| **University Server** | Free | Medium | Academic requirement |

---

## Option 1: GitHub Pages (Recommended)

### Step 1: Build in Unity

```
File > Build Settings
├── Platform: WebGL
├── Build: Build
└── Output: /docs folder in repo root
```

### Player Settings
```
Edit > Project Settings > Player > WebGL
├── Resolution: Default Canvas Width/Height
├── WebGL Template: Default
├── Compression Format: Brotli (production) or Disabled (testing)
├── Decompression Fallback: Enabled
├── Publishing Settings:
│   ├── Compression Format: Brotli
│   └── Data Caching: Enabled
```

### Step 2: Configure Repository

1. Build output to `/docs` folder:
```
WebGL_tesis/
├── docs/              ← WebGL build output
│   ├── Build/
│   ├── TemplateData/
│   └── index.html
├── desarrollo/
└── ...
```

2. Commit and push:
```bash
git add docs/
git commit -m "build: Add WebGL build for GitHub Pages"
git push
```

### Step 3: Enable GitHub Pages

1. Go to repository Settings > Pages
2. Source: **Deploy from a branch**
3. Branch: **master** (or main)
4. Folder: **/docs**
5. Click Save

### Step 4: Access URL

```
https://delarge95.github.io/WebGL_tesis/
```

Wait 2-5 minutes for first deployment.

---

## Option 2: Vercel

### Step 1: Build Locally

Build Unity WebGL to a `webgl-build/` folder.

### Step 2: Deploy via CLI

```bash
# Install Vercel CLI
npm install -g vercel

# Navigate to build folder
cd webgl-build

# Deploy
vercel

# Follow prompts:
# - Link to existing project? No
# - Project name: webgl-drone-viewer
# - Directory: ./
```

### Step 3: Production Deploy

```bash
vercel --prod
```

URL: `https://webgl-drone-viewer.vercel.app`

---

## Option 3: Netlify

### Step 1: Drag and Drop

1. Go to [netlify.com](https://netlify.com)
2. Sign up with GitHub
3. Drag `webgl-build/` folder to the drop zone

### Step 2: Configure

- Site name: `webgl-drone-viewer`
- URL: `webgl-drone-viewer.netlify.app`

### Step 3: Custom Domain (Optional)

```
Site Settings > Domain Management > Add custom domain
```

---

## WebGL Build Optimization

### Recommended Settings

```
Player Settings > Publishing Settings:
├── Compression Format: Brotli
├── Name Files as Hashes: Enabled
├── Data Caching: Enabled
└── Decompression Fallback: Enabled

Player Settings > Other Settings:
├── Strip Engine Code: Enabled
├── Managed Stripping Level: High
└── Script Call Optimization: Fast but no exceptions
```

### Build Size Targets

| Component | Target Size |
|-----------|-------------|
| .wasm | < 15 MB |
| .data | < 20 MB |
| .js | < 2 MB |
| **Total** | < 40 MB |

---

## Custom Loading Screen

### Create custom template

```
Assets/WebGLTemplates/DroneViewer/
├── index.html
├── style.css
├── TemplateData/
│   ├── favicon.ico
│   ├── fullscreen-button.png
│   ├── progress-bar-empty-dark.png
│   ├── progress-bar-full-dark.png
│   ├── unity-logo-dark.png
│   └── webgl-logo.png
└── thumbnail.png
```

### Custom index.html template

```html
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>WebGL Drone Viewer</title>
  <link rel="stylesheet" href="style.css">
</head>
<body>
  <div id="unity-container">
    <canvas id="unity-canvas"></canvas>
    <div id="unity-loading-bar">
      <div id="unity-logo"></div>
      <div id="unity-progress-bar-empty">
        <div id="unity-progress-bar-full"></div>
      </div>
    </div>
  </div>
  <script src="Build/{{{ LOADER_FILENAME }}}"></script>
  <script>
    createUnityInstance(document.querySelector("#unity-canvas"), {
      dataUrl: "Build/{{{ DATA_FILENAME }}}",
      frameworkUrl: "Build/{{{ FRAMEWORK_FILENAME }}}",
      codeUrl: "Build/{{{ CODE_FILENAME }}}",
      streamingAssetsUrl: "StreamingAssets",
      companyName: "UNAD",
      productName: "WebGL Drone Viewer",
      productVersion: "1.0.0",
    });
  </script>
</body>
</html>
```

---

## Server Configuration

### Required MIME Types

For servers that need manual configuration:

```
.wasm    application/wasm
.data    application/octet-stream
.js      application/javascript
.br      (Brotli compressed files)
.gz      (Gzip compressed files)
```

### .htaccess (Apache)

```apache
AddType application/wasm .wasm
AddType application/octet-stream .data

<IfModule mod_mime.c>
  AddEncoding br .br
</IfModule>

<FilesMatch "\.data\.br$">
  AddType application/octet-stream .data
  AddEncoding br .br
</FilesMatch>

<FilesMatch "\.wasm\.br$">
  AddType application/wasm .wasm
  AddEncoding br .br
</FilesMatch>

<FilesMatch "\.js\.br$">
  AddType application/javascript .js
  AddEncoding br .br
</FilesMatch>
```

### nginx.conf

```nginx
location / {
  types {
    application/wasm wasm;
    application/octet-stream data;
  }
  
  gzip_static on;
  brotli_static on;
}
```

---

## CORS Configuration

If loading external assets:

```html
<script>
  // In index.html before Unity loader
  window.unityWebRequestOverrides = {
    ModifyUrl: function(url) { return url; },
    ModifyHeaders: function(headers) {
      headers['Access-Control-Allow-Origin'] = '*';
      return headers;
    }
  };
</script>
```

---

## Performance Monitoring

### Add Analytics

```javascript
// After Unity loads
unityInstance.Module.printErr = function(message) {
  console.error(message);
  // Send to analytics
};
```

### Lighthouse Targets

| Metric | Target |
|--------|--------|
| First Contentful Paint | < 3s |
| Time to Interactive | < 10s |
| Performance Score | > 60 |

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Blank screen | Check browser console for MIME type errors |
| Loading stuck | Verify all .br files are served correctly |
| Low FPS | Reduce quality settings, check GPU support |
| Memory error | Increase memory size in Player Settings |
| Mobile issues | Test WebGL support at webglreport.com |

---

## SSL/HTTPS Requirement

WebGL requires HTTPS for:
- SharedArrayBuffer (for threading)
- Service Workers
- Certain APIs

GitHub Pages and Vercel provide HTTPS automatically.

---

## Sharing the Demo

### Academic Presentation
```
https://delarge95.github.io/WebGL_tesis/
```

### QR Code Generator
Use [qr-code-generator.com](https://www.qr-code-generator.com/) to create QR for the URL.

### Embed in Slides
For presentations, consider a screen recording backup in case of network issues.

---

*Deployment Guide Version: 1.0*
*Last Updated: December 2024*
