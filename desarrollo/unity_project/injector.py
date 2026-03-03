import codecs

# 1. Fix MainLayout.uxml
uxml_path = r'e:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\UI\Layouts\MainLayout.uxml'
with codecs.open(uxml_path, 'r', 'utf-8') as f:
    uxmltext = f.read()

target_uxml = '<ui:Label text="SELECT A PART" name="SheetTitle" class="sheet-title" picking-mode="Ignore" />'
if target_uxml in uxmltext and 'name="SheetCloseBtn"' not in uxmltext:
    rep_uxml = target_uxml + '\n                <ui:Button name="SheetCloseBtn" class="sheet-close-btn" text="" tooltip="Close Panel" picking-mode="Position" focusable="false"/>'
    res_uxml = uxmltext.replace(target_uxml, rep_uxml)
    with codecs.open(uxml_path, 'w', 'utf-8') as f:
        f.write(res_uxml)
    print("UXML INJECTED")

# 2. Fix Theme.uss
uss_path = r'e:\WebGL_tesis\desarrollo\unity_project\Assets\UI\Styles\Theme.uss'
with codecs.open(uss_path, 'r', 'utf-8') as f:
    usstext = f.read()

target_uss = '.sheet-header {\r\n    margin-bottom: 4px;\r\n    flex-direction: row;\r\n    justify-content: space-between;\r\n    align-items: center;\r\n}'
target_uss2 = '.sheet-header {\n    margin-bottom: 4px;\n    flex-direction: row;\n    justify-content: space-between;\n    align-items: center;\n}'

rep_uss = '.sheet-header {\n    margin-bottom: 4px;\n    flex-direction: row;\n    position: relative;\n    justify-content: space-between;\n    align-items: center;\n}'

if target_uss in usstext:
    res_uss = usstext.replace(target_uss, rep_uss)
    with codecs.open(uss_path, 'w', 'utf-8') as f:
        f.write(res_uss)
    print("USS INJECTED (CRLF)")
elif target_uss2 in usstext:
    res_uss = usstext.replace(target_uss2, rep_uss)
    with codecs.open(uss_path, 'w', 'utf-8') as f:
        f.write(res_uss)
    print("USS INJECTED (LF)")
