using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Jigupa.UI;

namespace Jigupa.Editor
{
    /// <summary>
    /// Helper tools for setting up and managing Jigupa scenes in the Unity Editor
    /// </summary>
    public class SceneSetupHelper : EditorWindow
    {
        // ===== Initial Setup =====
        /// <summary>
        /// One-click setup for the entire project. Configures build settings and creates UI.
        /// </summary>
        [MenuItem("Jigupa/Initial Setup/Setup All Scenes", priority = 1)]
        public static void SetupAllScenes()
        {
            // Check if we're in play mode
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("Cannot setup scenes during play mode. Please exit play mode and try again.");
                return;
            }
            
            // Configure build settings with proper scene order
            ConfigureBuildSettings();
            
            // Open MainMenu as the default working scene
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            
            // Setup MainMenu UI
            SetupMainMenuUI();
        }
        
        private static void ConfigureBuildSettings()
        {
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Battle.unity", true)
            };
            
            EditorBuildSettings.scenes = scenes;
        }
        
        // ===== Scene Navigation =====
        /// <summary>
        /// Quick navigation to MainMenu scene
        /// </summary>
        [MenuItem("Jigupa/Scenes/Open MainMenu", priority = 20)]
        public static void OpenMainMenu()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("Cannot open scenes during play mode. Please exit play mode and try again.");
                return;
            }
            
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
        
        /// <summary>
        /// Quick navigation to Battle scene
        /// </summary>
        [MenuItem("Jigupa/Scenes/Open Battle Scene", priority = 21)]
        public static void OpenBattleScene()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("Cannot open scenes during play mode. Please exit play mode and try again.");
                return;
            }
            
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
        }
        
        // ===== Testing =====
        /// <summary>
        /// Starts play mode from MainMenu to test the full game flow
        /// </summary>
        [MenuItem("Jigupa/Testing/Test Game Flow", priority = 40)]
        public static void TestGameFlow()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("Already in play mode!");
                return;
            }
            
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            EditorApplication.EnterPlaymode();
        }
        
        // ===== Private Helper Methods =====
        private static void SetupMainMenuUI()
        {
            // Ensure fist icon is properly imported as sprite
            EnsureFistIconIsSprite();
            
            // Find or create MainMenuUISetup
            MainMenuUISetup menuSetup = Object.FindFirstObjectByType<MainMenuUISetup>();
            bool wasCreated = false;
            
            if (menuSetup == null)
            {
                GameObject setupObject = new GameObject("MainMenuUISetup");
                menuSetup = setupObject.AddComponent<MainMenuUISetup>();
                wasCreated = true;
            }
            
            // Run the setup
            menuSetup.SetupMainMenu();
            
            // Clean up temporary setup object
            if (wasCreated && Application.isPlaying == false)
            {
                EditorApplication.delayCall += () => {
                    if (menuSetup != null && menuSetup.gameObject != null)
                    {
                        Object.DestroyImmediate(menuSetup.gameObject);
                    }
                };
            }
            
            // Save the scene
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
        
        /// <summary>
        /// Ensures the fist icon is properly imported as a sprite for UI use
        /// </summary>
        private static void EnsureFistIconIsSprite()
        {
            string fistIconPath = "Assets/Resources/Icons/fist.png";
            
            TextureImporter importer = AssetImporter.GetAtPath(fistIconPath) as TextureImporter;
            
            if (importer != null)
            {
                // Configure as UI sprite
                bool needsReimport = false;
                
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    needsReimport = true;
                }
                
                if (needsReimport)
                {
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 100;
                    importer.spritePivot = new Vector2(0.5f, 0.5f);
                    importer.sRGBTexture = true;
                    importer.alphaIsTransparency = true;
                    importer.alphaSource = TextureImporterAlphaSource.FromInput;
                    importer.mipmapEnabled = false;
                    
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                }
            }
        }
    }
}