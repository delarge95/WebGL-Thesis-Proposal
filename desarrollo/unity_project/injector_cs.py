import codecs

# 3. Fix UIDetailsSheet.cs
cs_path = r'e:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\UI\Panels\UIDetailsSheet.cs'
with codecs.open(cs_path, 'r', 'utf-8') as f:
    cstext = f.read()

target_cs1 = 'AddCleanup(() => header.UnregisterCallback(headerClick));\r\n            }'
target_cs2 = 'AddCleanup(() => header.UnregisterCallback(headerClick));\n            }'

rep_cs = 'AddCleanup(() => header.UnregisterCallback(headerClick));\n            }\n\n            var closeBtn = _root.Q<UnityEngine.UIElements.Button>("SheetCloseBtn");\n            if (closeBtn != null)\n            {\n                closeBtn.clicked += () => SetSheetState(false);\n                AddCleanup(() => closeBtn.clicked -= () => SetSheetState(false));\n            }'

if target_cs1 in cstext:
    res_cs = cstext.replace(target_cs1, rep_cs.replace('\n', '\r\n'))
    with codecs.open(cs_path, 'w', 'utf-8') as f:
        f.write(res_cs)
    print("C# INJECTED (CRLF)")
elif target_cs2 in cstext:
    res_cs = cstext.replace(target_cs2, rep_cs)
    with codecs.open(cs_path, 'w', 'utf-8') as f:
        f.write(res_cs)
    print("C# INJECTED (LF)")
else:
    print("TARGET NOT FOUND IN CS")
