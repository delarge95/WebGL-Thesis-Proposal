# Unity 6 UI Toolkit slider dragger hitbox vs. visual alignment

## Overview

This report analyzes how `BaseSlider<T>` in Unity 6 UI Toolkit positions its dragger, how hit-testing works, what the default USS does, and why a large custom dragger (28×28) can appear visually correct while only part of it is clickable. It also summarizes known issues and provides concrete USS patterns to keep the dragger’s hitbox aligned with its visual representation.[^1][^2]

## How BaseSlider positions the dragger

Unity’s `Slider` and `SliderInt` controls are thin wrappers over `BaseSlider<T>`, which manages low/high/value, direction, and the nested elements: drag container, tracker, and dragger. The dragger visual element gets the USS class `.unity-base-slider__dragger`, and the drag container uses `.unity-base-slider__drag-container`.[^3][^4][^5][^1]

From the public API and documentation:

- `BaseSlider<T>` computes a normalized value from `lowValue`, `highValue`, and `value`, and positions the dragger along the track accordingly.[^6][^1]
- For horizontal sliders, the dragger is moved by setting `dragger.style.left` to a length value (pixels) relative to the drag container.[^2][^1]
- For vertical sliders, a similar calculation is applied along the Y axis (using top/bottom depending on orientation). This is independent of your USS `width`/`height` for the dragger; those only affect the size, not the value-to-position mapping.[^4]

Crucially, the layout engine uses `position`, `left`, `top`, `right`, and `bottom` to compute the element’s layout rect, which is then used for hit-testing. Transforms such as `translate` affect only the final rendered position, not the layout rect that pointer picking uses.[^7][^2]

## Default USS for slider dragger and container

Unity’s default runtime/editor themes define rules for the slider internals using `.unity-base-slider__drag-container`, `.unity-base-slider__tracker`, and `.unity-base-slider__dragger`. While the full USS file is not shown in the official docs, extracted copies of Unity’s default stylesheet and third‑party examples reveal the typical pattern:[^5][^3]

- The drag container is a flex row that fills available width: `.unity-base-slider__drag-container { flex-grow: 1; }`.[^8][^9]
- The tracker is a thin bar (a few pixels high) centered vertically in the container.[^10][^3]
- The dragger is positioned absolutely and vertically offset with a fixed `margin-top` to sit over the tracker:
  - Example from an extracted default stylesheet and tutorial themes: `.unity-base-slider__dragger { position: absolute; border-width: 1px; margin-top: 4px; }`.[^11][^12]
- Custom slider examples that increase dragger size commonly do so by only changing `width`, `height`, `border-radius`, and a small negative `margin-top` relative to the track height, without touching `left` (which is controlled by `BaseSlider`).[^12]

This means Unity’s built-in styling *expects* the dragger to be positioned using `style.left` from C#, with any vertical centering achieved by static offsets (margin or top) in USS, not by changing the positioning model logic.

## Hit-testing and why translate does not help

UI Toolkit’s picking logic is based on each element’s layout rectangle and picking mode, not its visual transform:[^2][^7]

- `position: relative` (default) places the element in the flex layout; `left`/`top` then apply offsets from the computed position.
- `position: absolute` removes the element from the flex flow and positions it relative to the nearest positioned ancestor using `left`/`top`/`right`/`bottom`.
- The pointer picking APIs (`VisualElement.ContainsPoint`, `IPanel.Pick`) and `picking-mode` use this layout rect and visibility/display to decide which element receives pointer events.[^7]
- The `transform`/`translate` properties (and USS transitions/animations on them) apply after layout, affecting only rendering; they do *not* change the layout rect used for hit-testing.[^2]

As a result:

- Using `translate: -50% 0` to center the dragger visually moves the rendered circle but leaves the hitbox where the layout rect is.
- Changing `top`/`margin-top` can change the layout rect, but only if the element’s `position` causes those properties to participate in layout (relative or absolute per USS rules).[^2]

## Why a large 28×28 dragger can have a misaligned hitbox

From Unity’s docs and examples there is no explicit known bug stating that merely increasing the dragger size breaks hit-testing, and custom sliders with larger thumbs are widely demonstrated. Given how layout and picking work, the most plausible causes for the behavior you describe are:[^13][^12]

- The dragger’s layout rect is still anchored near the top-left of the drag container while the visual circle is shifted via properties that do not affect hit-testing (for example, a combination of transforms and layout).
- Parent flex alignment (`align-items`) and container sizing have been overridden so that the dragger’s *logical* origin remains at the top-left of the slider’s input area, while the track and visual thumb appear centered.
- Additional transforms or scaling on ancestor elements (using `transform`/`scale`) visually move or resize the slider without updating layout bounds, reducing the effective clickable area to a fraction of the rendered circle.

Unity’s `VisualElement.pickingMode` documentation notes that hit-testing is disabled entirely when `picking-mode` is `Ignore`, and that elements with `display: none` or `visible = false` are also not pickable. However, because you observe that the bottom-right quadrant *is* clickable, this points to layout/transform interaction rather than picking mode being off.[^7]

No official Unity 6 issue tracker entry or manual page currently documents a slider-specific bug where only part of an enlarged dragger is clickable; recent UI Toolkit release notes talk about preventing the dragger from going out of bounds and other behavior, but not partial hitboxes.[^14][^15]

## Effect of position: absolute vs relative on BaseSlider

`BaseSlider<T>` positions the dragger horizontally by assigning a `Length` to `style.left` on the dragger element. In both `position: relative` and `position: absolute` modes, `left` defines the horizontal offset from the element’s reference edge; the difference is whether that reference is the in-flow position or the containing block’s left edge.[^2]

The default stylesheet uses `position: absolute` for the dragger in at least some variants, and Unity’s documentation does not state that the control requires `position: relative` for correct behavior. This implies:[^11][^12]

- `position: absolute` on the dragger does **not** inherently break `BaseSlider`’s logic, as long as the dragger’s containing block (usually the drag container) is positioned and sized in a way that matches how `BaseSlider` computes `style.left`.
- Changing the dragger to `position: relative` is also legal, but then `left` becomes an offset from the flex layout position, and any additional USS offsets (`top`, `margin-top`) must be coordinated with how the parent lays out children.

In other words, what matters is consistency between:

- The element for which `BaseSlider` writes `style.left` (the dragger), and
- The containing block whose width is used for the value-to-pixel conversion (the drag container/track geometry).

If USS changes the positioning context in a way `BaseSlider` does not expect (for example, changing which ancestor is the containing block, or altering its width with padding/margins), the dragger’s logical hitbox and its visual appearance can diverge.

## Recommended pattern for a large circular dragger

Given Unity’s layout model and default styles, the most robust pattern to create a 28×28 circle dragger on a thin track is:

1. Let `BaseSlider` fully control the dragger’s horizontal position via `style.left`.
2. Keep vertical centering in USS using properties that *do* affect layout (not `translate`).
3. Avoid altering the drag container’s flex flow and alignment in ways that move the logical origin unexpectedly.

A USS pattern that follows these principles:

```css
/* Keep input and drag container unconstrained for hit-testing */
.glass-slider .unity-base-slider__input {
    overflow: visible;
}

.glass-slider .unity-base-slider__drag-container {
    flex-grow: 1;           /* like default */
    align-items: center;    /* center children vertically */
    justify-content: flex-start;
    overflow: visible;
}

/* Thin track, vertically centered via flex alignment instead of top/margin */
.glass-slider .unity-base-slider__tracker {
    height: 3px;
    border-radius: 2px;
    border-width: 0;
    background-color: rgba(255, 255, 255, 0.12);
    align-self: stretch;    /* or center, depending on your overall height */
}

/* Large circular dragger: let BaseSlider control left, only size + radius here */
.glass-slider .unity-base-slider__dragger {
    position: absolute;     /* matches default behavior */
    width: 28px;
    height: 28px;
    border-radius: 50%;
    background-color: rgb(0, 0, 0);
    border-color: rgba(255, 255, 255, 0.8);
    border-width: 2px;

    /* Optional small static vertical tweak if needed, but avoid large top/margin-top */
    top: 50%;
    margin-top: -14px;      /* only if the slider’s total height is stable */
}
```

Key points of this approach:

- Vertical centering is primarily handled by `align-items: center` on the drag container, so large offsets on the dragger aren’t required.
- The dragger remains absolutely positioned with `left` controlled by `BaseSlider`, matching the default theme pattern.[^12][^11]
- `translate` is not used, so the layout rect (and therefore hitbox) moves with the visual.

If your control’s overall height is stable (for example, a fixed height in the slider root), using `top: 50%` with `margin-top: -14px` is safe and will move both the visual and the hitbox together.

## Role of drag container and input styles

Unity’s documentation for `Slider` and `SliderInt` describes the standard USS class structure and encourages using those selectors (`.unity-base-slider__input`, `.unity-base-slider__drag-container`, `.unity-base-slider__tracker`, `.unity-base-slider__dragger`) when customizing.[^9][^16][^3]

For the containing elements:

- The input container (`.unity-base-slider__input`) is the primary layout box for the slider content; changing its `display`, `flex-direction`, or extreme padding can affect how `BaseSlider` interprets available width.[^10]
- The drag container (`.unity-base-slider__drag-container`) is typically a flex row filling available width; its content rect is used for dragger positioning.[^9]
- If you use `position: absolute` on the dragger, ensure that the drag container itself (or another ancestor) is the positioned ancestor that defines the containing block.

A safe pattern is to leave `.unity-base-slider__input` and `.unity-base-slider__drag-container` close to the defaults (flex row, flex-grow, align-items center) and only adjust their height and overflow.[^3][^10]

## Known issues, docs, and guidance

The Unity manual for `Slider` and `SliderInt` focuses on usage and USS selectors, not on thumb hitbox issues, and does not mention problems specific to large draggers. Release notes and issue tracker entries reference fixes for draggers going out of bounds or centering under the mouse when clicking, but they don’t describe partial hitbox bugs.[^17][^18][^1][^14][^6][^9]

Practical guidance from official and community tutorials on styling sliders with UI Toolkit includes:[^19][^12]

- Start from Unity’s default USS (`UnityEngine.UIElements.uss` in the UI package) and copy the `.unity-base-slider__*` rules you need, then override only size, colors, and small offsets.
- Keep the dragger element as the hitbox and, if a more complex visual is required, add a child element inside the dragger (with `picking-mode: Ignore`) to render the graphic without affecting hit-testing.
- When using `position: absolute` for auxiliary visuals (value bubbles, shadows), place them as siblings of the dragger and drive their position from the same value computation in C#, rather than overriding the dragger’s positioning.[^20][^21]

This aligns with the idea that you should work *with* `BaseSlider`’s internal layout rather than against it: keep its dragger semantics intact and layer extra visuals if needed.

## Answers to the specific questions

1. **How does BaseSlider position the dragger?**
   - It computes a normalized value from `lowValue`/`highValue`/`value` and sets `dragger.style.left` for horizontal sliders (and a corresponding vertical offset for vertical ones), using the drag container’s geometry as the reference.[^1][^4]
   - This is applied as a `Length` (pixel) inline style, independent of your USS size settings (except insofar as they change the container’s geometry).[^1]

2. **Is there a known issue where changing dragger size breaks the hitbox?**
   - There is no officially documented Unity 6 bug specifically stating that increasing the dragger size causes only part of it to be clickable.[^15][^14]
   - Issues that do exist around sliders concern dragger bounds and centering under the cursor, not partial hit-tests.[^18][^17]

3. **Correct way to create a large 28×28 dragger on a thin track:**
   - Let `BaseSlider` control horizontal positioning via `style.left`.
   - Center vertically using `align-items: center` on `.unity-base-slider__drag-container` and small USS offsets (`top`/`margin-top`) on the dragger if necessary.
   - Avoid using `translate` for centering, as it only moves visuals, not the hitbox.[^2]

4. **Does `position: absolute` break BaseSlider’s logic, or does it expect `relative`?**
   - The default stylesheet itself uses `position: absolute` for the dragger, so `BaseSlider` is designed to work with that configuration.[^11]
   - `BaseSlider` does not require `position: relative`; it only assumes that `left` is applied to the dragger within a well-defined containing block.

5. **Are there Unity forum/posts/docs about this specific hitbox issue?**
   - No dedicated documentation could be found describing a bug where only a portion of an enlarged UI Toolkit slider thumb is clickable; related issues are about dragger bounds and cursor centering.[^14][^17][^18]

6. **Do `#unity-drag-container` or `.unity-base-slider__input` need specific styles?**
   - Yes: they should remain flex containers with predictable geometry, similar to the defaults (`flex-direction: row`, `flex-grow: 1`, sensible `align-items`).[^3][^9]
   - If you change their positioning such that the dragger’s containing block or width no longer match what `BaseSlider` expects, misalignment between hitbox and visuals can occur.

7. **Is the solution to avoid `top`/`margin-top` and instead use flexbox alignment (`align-items: center`) with `position: relative`?**
   - Using flexbox alignment on the drag container is recommended for robust vertical centering.[^2]
   - You can keep the dragger `position: absolute` (as in Unity’s default styles) or `relative`; in both cases, ensure that vertical offsets are minimal and layout‑affecting, not purely visual transforms.

8. **Should you inspect the default Unity 6 slider USS and mimic it?**
   - Yes. Unity’s docs explicitly encourage using the provided USS selectors and default styles as a base when customizing controls.[^5][^3]
   - Extracted default stylesheets and official samples show working patterns for custom sliders, which you can adapt rather than reinventing the positioning from scratch.[^8][^12]

---

## References

1. [Slider - Unity - Manual](https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-uxml-element-Slider.html) - Create a Slider. You can create a Slider with UI Builder, UXML, or C#. The following C# example crea...

2. [USS common properties - Unity 6.3 User Manual](https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-USS-SupportedProperties.html) - Unity's model is equivalent to setting the CSS box-sizing property to border-box . Refer to the MDN ...

3. [UXML element Slider - Unity - Manual](https://docs.unity3d.com/2022.1/Documentation/Manual/UIE-uxml-element-Slider.html) - USS class name of dragger elements in elements of this type. draggerBorderUssClassName .unity-base-s...

4. [Unity - Manual: UXML element SliderInt](https://docs.unity.cn/2022.1/Documentation/Manual/UIE-uxml-element-SliderInt.html) - UXML element SliderInt. C# class: SliderInt. Namespace: UnityEngine.UIElements. Base class: BaseSlid...

5. [UXML element Slider - Unity - Manual](https://docs.unity3d.com/2023.2/Documentation/Manual/UIE-uxml-element-Slider.html) - USS class name of dragger elements in elements of this type. draggerBorderUssClassName .unity-base-s...

6. [SliderInt - Unity 6.3 User Manual](https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-uxml-element-SliderInt.html) - The standard practice is to give an element a unique name. picking-mode, UIElements.PickingMode, Det...

7. [Scripting API: UIElements.VisualElement.pickingMode - Unity](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/UIElements.VisualElement-pickingMode.html) - BaseSlider<T0>. Classes. BaseSlider<T0> · BaseTreeView. Classes. UxmlTraits ... Elements with a pick...

8. [com.unity.ui/PackageResources/StyleSheets/Default/UnityEngine ...](https://github.com/needle-mirror/com.unity.ui/blob/master/PackageResources/StyleSheets/Default/UnityEngine.UIElements.uss) - ... USS asset types for building and styling user ... Default/UnityEngine.UIElements.uss at master ....

9. [SliderInt - Unity - Manual](https://docs.unity3d.com/2022.3/Documentation/Manual/UIE-uxml-element-SliderInt.html) - USS class name of elements of this type, when they are displayed vertically. dragContainerUssClassNa...

10. [UXML element Slider - Unity - Manual](https://docs.unity3d.com/2021.3/Documentation/Manual/UIE-uxml-element-Slider.html) - A Slider lets users select a floating-point value from a range of values. You can use a Slider to ad...

11. [Unitys default style sheet extracted · GitHub](https://gist.github.com/Ddemon26/ed9cebf57588b82549070733b00d384f) - position: absolute;. border-width: 1px;. border-color: rgb(50, 50, 50);. margin-top: 4px;. } .unity-...

12. [SliderCartoon09 - Unity UI Designer Pro](https://unityuibuilder.com/posts/sliders/slidercartoon09/) - ... unity-base-slider__dragger { width: 18px; height: 18px; border-radius: 50 ... margin-top: -6px; ...

13. [One Wheel Studio](https://onewheelstudio.com/blog?year=2022) - So I set out to make the “same” ui with UGUI, UI Toolkit, and Nova UI. I wanted to create a slider t...

14. [UnityReleaseNotes-latest/merge_htmls/2022.2.md at main - GitHub](https://github.com/mario206/UnityReleaseNotes-latest/blob/main/merge_htmls/2022.2.md) - UI Toolkit: Prevented the slider dragger from going out of bounds. (1404066). UI Toolkit: Stopped Ba...

15. [Unity 2022.2.0b5](https://unity.com/releases/editor/beta/2022.2.0b5) - ... hit shader names from a shader pass and cause ray tracing dispatches to ... UI Toolkit: Prevente...

16. [Manual: UXML element SliderInt - Unity](https://docs.unity.cn/Manual/UIE-uxml-element-SliderInt.html) - USS class name of dragger elements in elements of this type. draggerBorderUssClassName .unity-base-s...

17. [Unity Issue Tracker](https://issuetracker.unity3d.com/?ampDeviceId=bd73ee3b-a36f-4585-8d3e-967cba74f006&ampSessionId=1766016000339&ampTimestamp=1766102400365&page=6753) - UI Toolkit ... A slider's dragger centers itself under the mouse cursor when clicking it · UI Toolki...

18. [A slider's dragger centers itself under the mouse cursor when ...](https://issuetracker-mig.prd.it.unity3d.com/issues/a-sliders-dragger-centers-itself-under-the-mouse-cursor-when-clicking-it) - A slider's dragger centers itself under the mouse cursor when clicking it. UI Toolkit. -. Jul 03, 20...

19. [Styling elements in UI Toolkit - Unity, huh, how?](https://unity.huh.how/ui-toolkit/styling.html) - This guide describes how to create USS selectors to style complex elements. USS is a query system si...

20. [Unity UI Toolkit Beginner's Guide 5: Customizing Slider 2 - YouTube](https://www.youtube.com/watch?v=4EM3Ccl0u0I) - In this video we'll finish the custom slider that was not completed last time. ( The first part of t...

21. [Unity UI Toolkit Beginner's Guide 4: Customizing Slider 1 - YouTube](https://www.youtube.com/watch?v=pUFG1u6dNQ4) - ... Dragger 2 17:58 Polishing Slider #Unity #UIToolkit #Tutorial. ... Unity UI Toolkit Beginner's Gu...

