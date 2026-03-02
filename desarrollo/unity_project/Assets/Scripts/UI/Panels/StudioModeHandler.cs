using UnityEngine.UIElements;

namespace WebGL.UI.Panels
{
    /// <summary>
    /// Handles the Studio mode UI: Shaders + Environment panels.
    /// Single-level — no sub-panel navigation.
    /// </summary>
    public class StudioModeHandler : BaseModeHandler
    {
        public StudioModeHandler(VisualElement root, VisualElement container) : base(root, container) { }

        public override void Activate() { /* Studio panels display automatically via container visibility */ }

        public override void Deactivate() { /* No sub-panel state to reset */ }
    }
}
