using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jigupa.Core
{
    public class BattleSceneSetup : MonoBehaviour
    {
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool addReturnToMenuButton = true;
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupBattleScene();
            }
        }
        
        [ContextMenu("Setup Battle Scene")]
        public void SetupBattleScene()
        {
            // Make sure we're in the Battle scene
            Scene currentScene = SceneManager.GetActiveScene();
            Debug.Log($"Setting up battle in scene: {currentScene.name}");
            
            // Use SimpleUISetup to create the game
            SimpleUISetup simpleSetup = GetComponent<SimpleUISetup>();
            if (!simpleSetup)
            {
                simpleSetup = gameObject.AddComponent<SimpleUISetup>();
            }
            
            simpleSetup.SetupSimpleScene();
            
            // Add return to menu button if needed
            if (addReturnToMenuButton && SceneManager.sceneCountInBuildSettings > 1)
            {
                AddReturnToMenuButton();
            }
        }
        
        private void AddReturnToMenuButton()
        {
            Canvas gameCanvas = FindObjectOfType<Canvas>();
            if (!gameCanvas) return;
            
            GameObject backBtn = CreateButton(gameCanvas.transform, "BackToMenuButton", "MENU",
                new Vector2(0.1f, 0.95f), new Vector2(100, 60));
            backBtn.GetComponent<UnityEngine.UI.Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            
            UnityEngine.UI.Button backButton = backBtn.GetComponent<UnityEngine.UI.Button>();
            backButton.onClick.AddListener(() => {
                // Load main menu scene
                SceneManager.LoadScene(0); // or "MainMenu"
            });
        }
        
        private GameObject CreateButton(Transform parent, string name, string text, Vector2 anchorPos, Vector2 size)
        {
            GameObject button = new GameObject(name);
            button.transform.SetParent(parent, false);
            
            RectTransform rect = button.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            
            UnityEngine.UI.Image image = button.AddComponent<UnityEngine.UI.Image>();
            image.color = Color.white;
            
            UnityEngine.UI.Button btn = button.AddComponent<UnityEngine.UI.Button>();
            
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(button.transform, false);
            
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            var tmp = textGO.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 16;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = Color.white;
            
            return button;
        }
    }
}