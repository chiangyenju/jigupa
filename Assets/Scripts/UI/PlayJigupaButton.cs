using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jigupa.UI
{
    /// <summary>
    /// Simple component that handles loading the Battle scene.
    /// Used by BattleFistIcon after the animation completes.
    /// </summary>
    public class PlayJigupaButton : MonoBehaviour
    {
        // Public method to load battle scene - called by BattleFistIcon after animation
        public void LoadBattleScene()
        {
            if (SceneManager.sceneCountInBuildSettings > 1)
            {
                Debug.Log("Loading Battle scene...");
                SceneManager.LoadScene(1);
            }
            else
            {
                Debug.LogError("Battle scene not found in Build Settings!");
                Debug.Log("Please go to File > Build Settings and add both MainMenu and Battle scenes");
            }
        }
    }
}