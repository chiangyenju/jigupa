using UnityEngine;
using UnityEditor;

namespace Jigupa.Editor
{
    public class DebugFistIcon : EditorWindow
    {
        [MenuItem("Jigupa/Debug/Check Fist Icon")]
        public static void CheckFistIcon()
        {
            // Find BattlePanel
            GameObject battlePanel = GameObject.Find("BattlePanel");
            if (battlePanel == null)
            {
                Debug.LogError("BattlePanel not found! Run Setup All Scenes first.");
                return;
            }
            
            Debug.Log($"BattlePanel found with {battlePanel.transform.childCount} children:");
            for (int i = 0; i < battlePanel.transform.childCount; i++)
            {
                Transform child = battlePanel.transform.GetChild(i);
                Debug.Log($"  Child {i}: {child.name}");
            }
            
            // Look for FistIcon
            Transform fistIcon = battlePanel.transform.Find("FistIcon");
            if (fistIcon != null)
            {
                Debug.Log($"✓ FistIcon found! Active: {fistIcon.gameObject.activeInHierarchy}");
                
                // Check components
                var image = fistIcon.GetComponent<UnityEngine.UI.Image>();
                if (image != null)
                {
                    Debug.Log($"  - Has Image component");
                    Debug.Log($"  - Sprite: {image.sprite?.name ?? "NULL"}");
                    Debug.Log($"  - Color: {image.color}");
                }
                
                var rectTransform = fistIcon.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    Debug.Log($"  - Position: {rectTransform.anchoredPosition}");
                    Debug.Log($"  - Size: {rectTransform.sizeDelta}");
                }
            }
            else
            {
                Debug.LogError("✗ FistIcon not found in BattlePanel!");
            }
        }
        
        [MenuItem("Jigupa/Debug/Force Create Fist Icon")]
        public static void ForceCreateFistIcon()
        {
            GameObject battlePanel = GameObject.Find("BattlePanel");
            if (battlePanel == null)
            {
                Debug.LogError("BattlePanel not found!");
                return;
            }
            
            // Remove any existing fist icon
            Transform existingFist = battlePanel.transform.Find("FistIcon");
            if (existingFist != null)
            {
                DestroyImmediate(existingFist.gameObject);
                Debug.Log("Removed existing FistIcon");
            }
            
            // Create new fist icon
            GameObject fistIcon = new GameObject("FistIcon");
            fistIcon.transform.SetParent(battlePanel.transform, false);
            
            RectTransform fistRect = fistIcon.AddComponent<RectTransform>();
            fistRect.anchorMin = new Vector2(0.5f, 0.5f);
            fistRect.anchorMax = new Vector2(0.5f, 0.5f);
            fistRect.sizeDelta = new Vector2(80, 80);
            fistRect.anchoredPosition = new Vector2(0, 120);
            
            UnityEngine.UI.Image fistImage = fistIcon.AddComponent<UnityEngine.UI.Image>();
            
            // Load sprite
            Sprite fistSprite = Resources.Load<Sprite>("Icons/fist");
            if (fistSprite == null)
            {
                Texture2D fistTexture = Resources.Load<Texture2D>("Icons/fist");
                if (fistTexture != null)
                {
                    fistSprite = Sprite.Create(fistTexture, 
                        new Rect(0, 0, fistTexture.width, fistTexture.height), 
                        new Vector2(0.5f, 0.5f));
                }
            }
            
            if (fistSprite != null)
            {
                fistImage.sprite = fistSprite;
                fistImage.preserveAspect = true;
                Debug.Log($"✓ Created FistIcon with sprite: {fistSprite.name}");
            }
            else
            {
                fistImage.color = Color.red;
                Debug.LogError("✗ Could not load fist sprite!");
            }
            
            // Mark scene dirty
            EditorUtility.SetDirty(battlePanel);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            
            Debug.Log("Force created FistIcon - remember to save the scene!");
        }
    }
}