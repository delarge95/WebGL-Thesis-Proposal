import codecs

uss_path = r'e:\WebGL_tesis\desarrollo\unity_project\Assets\UI\Styles\Theme.uss'
with codecs.open(uss_path, 'r', 'utf-8') as f:
    usstext = f.read()

target1 = '''/* 
   PRE-PEEK INFO BAR (Option 1)
   Minimal bar that peeks from the bottom
    */'''

target_start_idx = usstext.find(target1)

if target_start_idx != -1:
    end_marker = '''/* =======================================================
   Sheet Close Button ("X") - Apple HIG / Material 3'''
    target_end_idx = usstext.find(end_marker, target_start_idx)
    
    if target_end_idx != -1:
        rep_css = '''/* 
   PRE-PEEK INFO TAB (Semi-circular Option)
   Minimal tab that peeks from the bottom
    */
.info-tab-peek {
    position: absolute;
    bottom: 0;
    left: 50%;
    translate: -50% 100%; /* Hidden off-screen bottom by default */
    width: 64px;
    height: 36px;
    background-color: rgba(0, 0, 0, 0.90);
    border-top-left-radius: 40px;
    border-top-right-radius: 40px;
    border-top-width: 1px;
    border-left-width: 1px;
    border-right-width: 1px;
    border-color: rgba(255, 255, 255, 0.15);
    align-items: center;
    justify-content: center;
    padding: 0;
    margin: 0;
    transition-property: translate, background-color, border-color, scale;
    transition-duration: 0.3s;
    transition-timing-function: ease-out;
}

.info-tab-peek:hover {
    background-color: rgba(20, 20, 20, 0.98);
    border-color: rgba(255, 255, 255, 0.3);
}

.info-tab-peek:active {
    background-color: rgba(30, 30, 30, 1);
    scale: 0.94;
}

.info-tab-peek--visible {
    translate: -50% 0; /* Slide up, fully visible against bottom edge */
}

.info-tab-peek--hidden {
    translate: -50% 100%;
    opacity: 0;
    pointer-events: none;
}

.info-tab-icon {
    font-size: 18px;
    color: rgba(255, 255, 255, 0.75);
    -unity-font-style: italic; /* Traditional info 'i' look */
    -unity-font-definition: initial;
    -unity-font: resource("Fonts/Inter/Inter-Bold"); /* Or Times New Roman if preferred */
    -unity-text-align: middle-center;
    padding: 0;
    margin: 0;
    margin-top: 4px; /* Optical center due to semi-circle cutoff */
}

'''
        # Reemplazar la sección cortada
        res_uss = usstext[:target_start_idx] + rep_css + usstext[target_end_idx:]
        with codecs.open(uss_path, 'w', 'utf-8') as f:
            f.write(res_uss)
        print("CSS REPLACED SUCCESSFULLY")
else:
    print("TARGET STRINGS NOT FOUND")
