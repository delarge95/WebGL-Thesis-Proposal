using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.DevTools
{
    [RequireComponent(typeof(UIDocument))]
    public class UIGalleryLoader : MonoBehaviour
    {
        [Tooltip("Drag the IconGallery.uxml file here from the Assets/UI folder")]
        public VisualTreeAsset galleryUXML;

        void Start()
        {
            var uiDoc = GetComponent<UIDocument>();
            
            if (galleryUXML != null)
            {
                // Instantiate the gallery
                var galleryInstance = galleryUXML.Instantiate();
                galleryInstance.style.flexGrow = 1;
                
                // Add it to the root of the UIDocument
                uiDoc.rootVisualElement.Add(galleryInstance);
                
                Debug.Log("Procedural Icons Gallery Loaded Successfully.");
            }
            else
            {
                Debug.LogError("Error: Make sure the IconGallery.uxml file is assigned in this script's inspector.");
            }
        }
    }
}
