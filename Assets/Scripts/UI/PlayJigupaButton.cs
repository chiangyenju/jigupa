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
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnPlayJigupaClicked);
                Debug.Log($"PlayJigupaButton attached to {gameObject.name}");
            }
            else
            {
                Debug.LogError("No Button component found on PlayJigupaButton!");
            }
        }
        
        void OnPlayJigupaClicked()
        {
            Debug.Log("=== PLAY JIGUPA CLICKED (from component) ===");
            Debug.Log($"Scenes in build: {SceneManager.sceneCountInBuildSettings}");
            
            // Visual feedback
            GetComponent<Image>().color = Color.red;
            
            // Load Battle scene
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
        
        // Test method callable from Inspector
        [ContextMenu("Test Click")]
        public void TestClick()
        {
            OnPlayJigupaClicked();
        }
    }
}