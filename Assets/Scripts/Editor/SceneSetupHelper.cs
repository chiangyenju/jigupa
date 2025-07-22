using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Jigupa.UI;

namespace Jigupa.Editor
{
    public class SceneSetupHelper : EditorWindow
    {
        [MenuItem("Jigupa/Setup All Scenes")]
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
        
        private static void SetupMainMenuUI()
        {
            // Find or create MainMenuUISetup
            MainMenuUISetup menuSetup = Object.FindFirstObjectByType<MainMenuUISetup>();
            
            if (menuSetup == null)
            {
                GameObject setupObject = new GameObject("MainMenuUISetup");
                menuSetup = setupObject.AddComponent<MainMenuUISetup>();
            }
            
            // Run the setup
            menuSetup.SetupMainMenu();
            
            // Clean up the setup object as it's no longer needed
            if (Application.isPlaying == false)
            {
                Object.DestroyImmediate(menuSetup.gameObject);
            }
            
            // Mark scene as dirty to save changes
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log("MainMenu UI has been configured with PrimaryButton prefab!");
        }
        
        [MenuItem("Jigupa/Open MainMenu Scene")]
        public static void OpenMainMenu()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        }
        
        [MenuItem("Jigupa/Open Battle Scene")]
        public static void OpenBattleScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
        }
        
        [MenuItem("Jigupa/Test Game Flow")]
        public static void TestGameFlow()
        {
            // Save current scene
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            
            // Open MainMenu and play
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            EditorApplication.EnterPlaymode();
        }
    }
}