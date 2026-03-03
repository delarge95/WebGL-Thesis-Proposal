import codecs

path = r'e:\WebGL_tesis\desarrollo\unity_project\Assets\UI\Styles\Theme.uss'

with open(path, 'rb') as f:
    raw = f.read()

valid_boundary = b'.peek-label {'
idx = raw.find(valid_boundary)
if idx != -1:
    end_idx = raw.find(b'}', idx)
    if end_idx != -1:
        clean = raw[:end_idx+1]
        css = b'''

/* =======================================================
   Sheet Close Button ("X") - Apple HIG / Material 3
   Placed absolute right within the relative header.
   ======================================================= */
.sheet-close-btn {
    position: absolute;
    right: 24px;
    width: 44px;
    height: 44px;
    border-radius: 22px;
    background-color: rgba(255, 255, 255, 0.05);
    border-width: 1px;
    border-color: rgba(255, 255, 255, 0.1);
    color: rgba(255, 255, 255, 0.6);
    font-size: 18px;
    -unity-text-align: middle-center;
    padding: 0;
    margin: 0;
    transition-property: background-color, scale, color;
    transition-duration: 0.2s;
}

.sheet-close-btn:hover {
    background-color: rgba(255, 255, 255, 0.15);
    color: rgb(255, 255, 255);
}

.sheet-close-btn:active {
    scale: 0.92;
    background-color: rgba(255, 255, 255, 0.25);
}
'''
        with open(path, 'wb') as f:
            f.write(clean + css)
            print("CSS fixed successfully.")
