using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jigupa.UI
{
    public class SceneTransitionManager : MonoBehaviour
    {
        // Call this from the PLAY JIGUPA button
        public static void LoadBattleScene()
        {
            // Option 1: Load scene by name
            SceneManager.LoadScene("BattleScene");
            
            // Option 2: Load scene by build index
            // SceneManager.LoadScene(1);
        }
        
        // Call this from the MENU button in battle
        public static void LoadMainMenu()
        {
            // Option 1: Load scene by name
            SceneManager.LoadScene("MainMenu");
            
            // Option 2: Load scene by build index
            // SceneManager.LoadScene(0);
        }
        
        // For async loading with loading screen
        public static void LoadBattleSceneAsync()
        {
            SceneManager.LoadSceneAsync("BattleScene");
        }
    }
}