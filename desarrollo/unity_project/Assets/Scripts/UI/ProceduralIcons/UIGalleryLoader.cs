using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.DevTools
{
    [RequireComponent(typeof(UIDocument))]
    public class UIGalleryLoader : MonoBehaviour
    {
        [Tooltip("Arrastra aquí el archivo IconGallery.uxml desde la carpeta Assets/UI")]
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
                Debug.LogError("Error: Asegúrate de asignar el archivo IconGallery.uxml en el inspector de este script.");
            }
        }
    }
}
