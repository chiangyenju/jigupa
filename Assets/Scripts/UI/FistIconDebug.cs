using UnityEngine;

namespace Jigupa.UI
{
    public class FistIconDebug : MonoBehaviour
    {
        void Awake()
        {
            // Debug.LogError($"[FIST ICON] I'm alive! GameObject: {gameObject.name}, Active: {gameObject.activeInHierarchy}");
            // Debug.LogError($"[FIST ICON] Parent: {transform.parent?.name ?? "NULL"}");
        }
        
        void Start()
        {
            // Debug.LogError($"[FIST ICON] Start called! Still here!");
            
            // Check visual properties
            var rectTransform = GetComponent<RectTransform>();
            var image = GetComponent<UnityEngine.UI.Image>();
            var canvas = GetComponentInParent<Canvas>();
            
            // Debug.LogError($"[FIST ICON] Position: {transform.position} (World)");
            // Debug.LogError($"[FIST ICON] LocalPosition: {transform.localPosition}");
            // Debug.LogError($"[FIST ICON] AnchoredPosition: {rectTransform.anchoredPosition}");
            // Debug.LogError($"[FIST ICON] Size: {rectTransform.sizeDelta}");
            // Debug.LogError($"[FIST ICON] Scale: {transform.localScale}");
            // Debug.LogError($"[FIST ICON] Image enabled: {image.enabled}");
            // Debug.LogError($"[FIST ICON] Image sprite: {image.sprite?.name ?? "NULL"}");
            // Debug.LogError($"[FIST ICON] Image color: {image.color}");
            // Debug.LogError($"[FIST ICON] Canvas: {canvas?.name ?? "NULL"}");
            // Debug.LogError($"[FIST ICON] Layer: {gameObject.layer} ({LayerMask.LayerToName(gameObject.layer)})");
            
            // Check if it's behind something
            int siblingIndex = transform.GetSiblingIndex();
            // Debug.LogError($"[FIST ICON] Sibling index: {siblingIndex} of {transform.parent.childCount}");
            
            // FIX THE SCALE AND POSITION!
            if (transform.localScale == Vector3.zero)
            {
                // Debug.LogError($"[FIST ICON] FIXING ZERO SCALE!");
                transform.localScale = Vector3.one;
            }
            
            if (rectTransform.anchoredPosition != new Vector2(0, 120))
            {
                // Debug.LogError($"[FIST ICON] FIXING POSITION!");
                rectTransform.anchoredPosition = new Vector2(0, 120);
            }
        }
        
        void OnDestroy()
        {
            // Debug.LogError($"[FIST ICON] I'm being destroyed! Why?!");
            // Debug.LogError(new System.Diagnostics.StackTrace());
        }
        
        void OnDisable()
        {
            // Debug.LogError($"[FIST ICON] I'm being disabled!");
        }
        
        void Update()
        {
            // Check visibility every few seconds
            if (Time.frameCount % 60 == 0) // Every second at 60fps
            {
                var image = GetComponent<UnityEngine.UI.Image>();
                bool isVisible = image.enabled && gameObject.activeInHierarchy;
                var camera = Camera.main;
                
                if (isVisible && camera != null)
                {
                    Vector3 screenPoint = camera.WorldToScreenPoint(transform.position);
                    bool onScreen = screenPoint.z > 0 && 
                                   screenPoint.x >= 0 && screenPoint.x <= Screen.width &&
                                   screenPoint.y >= 0 && screenPoint.y <= Screen.height;
                    
                    // Debug.Log($"[FIST ICON] Update - Visible: {isVisible}, OnScreen: {onScreen}, ScreenPos: {screenPoint}");
                }
            }
        }
    }
}