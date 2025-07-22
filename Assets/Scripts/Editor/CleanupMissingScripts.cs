using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using Jigupa.UI;

namespace Jigupa.Editor
{
    public class CleanupMissingScripts : EditorWindow
    {
        [MenuItem("Jigupa/Cleanup Missing Scripts")]
        public static void CleanupMissing()
        {
            int removedCount = 0;
            int glareObjectsRemoved = 0;
            
            // Find all GameObjects in the scene
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (GameObject obj in allObjects)
            {
                // Get all components including null (missing) ones
                var components = obj.GetComponents<Component>();
                
                // Remove null components (missing scripts)
                for (int i = components.Length - 1; i >= 0; i--)
                {
                    if (components[i] == null)
                    {
                        var serializedObject = new SerializedObject(obj);
                        var prop = serializedObject.FindProperty("m_Component");
                        
                        for (int j = prop.arraySize - 1; j >= 0; j--)
                        {
                            var componentProp = prop.GetArrayElementAtIndex(j);
                            var componentRef = componentProp.FindPropertyRelative("component");
                            
                            if (componentRef.objectReferenceValue == null)
                            {
                                prop.DeleteArrayElementAtIndex(j);
                                removedCount++;
                            }
                        }
                        
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(obj);
                    }
                }
                
                // Remove old glare objects
                if (obj.name == "TextGlare" || obj.name == "GlareContainer" || 
                    obj.name == "GlareMask" || obj.name == "SharedGlareContainer" ||
                    obj.name == "Glare" || obj.name == "SharedGlareMask")
                {
                    Debug.Log($"Removing glare object: {obj.name}");
                    DestroyImmediate(obj);
                    glareObjectsRemoved++;
                }
            }
            
            if (removedCount > 0 || glareObjectsRemoved > 0)
            {
                Debug.Log($"Cleanup complete: Removed {removedCount} missing scripts and {glareObjectsRemoved} glare objects");
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            else
            {
                Debug.Log("No missing scripts or glare objects found");
            }
        }
        
        [MenuItem("Jigupa/Force Refresh Main Menu")]
        public static void ForceRefreshMainMenu()
        {
            // First cleanup
            CleanupMissing();
            
            // Then rebuild the menu
            MainMenuUISetup menuSetup = Object.FindFirstObjectByType<MainMenuUISetup>();
            if (menuSetup == null)
            {
                GameObject setupObject = new GameObject("MainMenuUISetup");
                menuSetup = setupObject.AddComponent<MainMenuUISetup>();
            }
            
            menuSetup.SetupMainMenu();
            
            // Clean up the setup object
            if (Application.isPlaying == false)
            {
                Object.DestroyImmediate(menuSetup.gameObject);
            }
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("Main Menu refreshed!");
        }
    }
}