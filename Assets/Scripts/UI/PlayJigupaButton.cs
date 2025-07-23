using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Jigupa.UI
{
    [RequireComponent(typeof(Button))]
    public class PlayJigupaButton : MonoBehaviour
    {
        private Button button;
        
        void Start()
        {
            button = GetComponent<Button>();
            if (button != null)
            {
                // Don't remove all listeners - just add our listener
                // This allows BattleFistIcon to also add its animation listener
                button.onClick.AddListener(OnPlayJigupaClicked);
                // Debug.Log($"PlayJigupaButton attached to {gameObject.name}");
            }
            else
            {
                // Debug.LogError("No Button component found on PlayJigupaButton!");
            }
        }
        
        void OnPlayJigupaClicked()
        {
            // Debug.Log("=== PLAY JIGUPA CLICKED (from component) ===");
            // Debug.Log($"Scenes in build: {SceneManager.sceneCountInBuildSettings}");
            
            // No visual feedback - let the fist animation handle it
            
            // Delay scene load to allow animation to complete
            StartCoroutine(LoadBattleSceneWithDelay());
        }
        
        System.Collections.IEnumerator LoadBattleSceneWithDelay()
        {
            // Wait for punch animation to complete (0.1s windup + 0.3s punch + 0.15s hold)
            yield return new WaitForSeconds(0.55f);
            
            // Load Battle scene
            if (SceneManager.sceneCountInBuildSettings > 1)
            {
                // Debug.Log("Loading Battle scene...");
                SceneManager.LoadScene(1);
            }
            else
            {
                // Debug.LogError("Battle scene not found in Build Settings!");
                // Debug.Log("Please go to File > Build Settings and add both MainMenu and Battle scenes");
            }
        }
        
        // Test method callable from Inspector
        [ContextMenu("Test Click")]
        public void TestClick()
        {
            OnPlayJigupaClicked();
        }
    }
}