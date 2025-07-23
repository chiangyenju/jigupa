using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Jigupa.UI;

namespace Jigupa.Editor
{
    public class SceneSetupHelper : EditorWindow
    {
        // ===== Initial Setup =====
        [MenuItem("Jigupa/Initial Setup/Setup All Scenes", priority = 1)]
        public static void SetupAllScenes()
        {
            // Setup Build Settings
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Battle.unity", true)
            };
            
            EditorBuildSettings.scenes = scenes;
            
            Debug.Log("Build settings configured:");
            Debug.Log("- MainMenu (index 0) - Default starting scene");
            Debug.Log("- Battle (index 1)");
            
            // Open MainMenu as the default working scene
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            
            // Setup MainMenu UI
            SetupMainMenuUI();
        }
        
        // ===== Scene Navigation =====
        [MenuItem("Jigupa/Scenes/Open MainMenu", priority = 20)]
        public static void OpenMainMenu()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
        
        [MenuItem("Jigupa/Scenes/Open Battle Scene", priority = 21)]
        public static void OpenBattleScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
        }
        
        // ===== Testing =====
        [MenuItem("Jigupa/Testing/Test Game Flow", priority = 40)]
        public static void TestGameFlow()
        {
            // Save current scene
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            
            // Open MainMenu and play
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
            
            // Delay cleanup to ensure all coroutines complete
            if (wasCreated && Application.isPlaying == false)
            {
                // Use EditorApplication.delayCall to destroy after current frame
                EditorApplication.delayCall += () => {
                    if (menuSetup != null && menuSetup.gameObject != null)
                    {
                        Object.DestroyImmediate(menuSetup.gameObject);
                        Debug.Log("Cleaned up MainMenuUISetup object after setup completed");
                    }
                };
            }
            
            // Mark scene as dirty to save changes
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            // Save the scene
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            
            Debug.Log("MainMenu UI has been configured and saved!");
        }
        
        private static void EnsureFistIconIsSprite()
        {
            string fistIconPath = "Assets/Resources/Icons/fist.png";
            
            // Force refresh first
            AssetDatabase.ImportAsset(fistIconPath, ImportAssetOptions.ForceUpdate);
            
            TextureImporter importer = AssetImporter.GetAtPath(fistIconPath) as TextureImporter;
            
            if (importer != null)
            {
                // Force sprite settings
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 100;
                importer.spritePivot = new Vector2(0.5f, 0.5f);
                importer.sRGBTexture = true;
                importer.alphaIsTransparency = true;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.mipmapEnabled = false;
                
                // Force reimport
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
                
                Debug.Log("Fist icon import settings applied");
                
                // Wait for import to complete
                AssetDatabase.Refresh();
                System.Threading.Thread.Sleep(100);
                
                // Verify the sprite can be loaded
                Sprite testSprite = Resources.Load<Sprite>("Icons/fist");
                Texture2D testTexture = Resources.Load<Texture2D>("Icons/fist");
                
                Debug.Log($"Sprite load test: {testSprite != null} (name: {testSprite?.name})");
                Debug.Log($"Texture load test: {testTexture != null} (name: {testTexture?.name})");
            }
            else
            {
                Debug.LogError($"✗ Could not find fist.png at: {fistIconPath}");
            }
        }
    }
}