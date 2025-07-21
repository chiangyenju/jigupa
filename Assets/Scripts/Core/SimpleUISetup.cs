using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Jigupa.Player;
using Jigupa.UI;
using UnityEngine.SceneManagement;

namespace Jigupa.Core
{
    public class SimpleUISetup : MonoBehaviour
    {
        [ContextMenu("Setup Simple Jigupa Scene")]
        public void SetupSimpleScene()
        {
            // Ensure we're working in the correct scene
            Scene currentScene = gameObject.scene;
            SceneManager.SetActiveScene(currentScene);
            
            // Clean up existing setup
            CleanupExistingSetup();
            
            // Create core managers
            CreateGameManager();
            
            // Create Canvas
            Canvas canvas = CreateCanvas();
            
            // Create simple UI
            CreateSimpleUI(canvas);
            
            // Create player controls
            CreateSimplePlayerControls(canvas);
            
            // Create coin flip UI
            CreateSimpleCoinFlipUI(canvas);
            
            Debug.Log($"Simple Jigupa scene setup complete in scene: {currentScene.name}!");
        }
        
        private void CleanupExistingSetup()
        {
            // Remove ALL game objects that might be from old setup
            var existingManagers = FindObjectsByType<GameStateManager>(FindObjectsSortMode.None);
            foreach (var manager in existingManagers)
            {
                DestroyImmediate(manager.gameObject);
            }
            
            // Remove all canvases
            var canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var canvas in canvases)
            {
                DestroyImmediate(canvas.gameObject);
            }
            
            // Remove event systems
            var eventSystems = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
            foreach (var es in eventSystems)
            {
                DestroyImmediate(es.gameObject);
            }
            
            // Remove any UI managers
            var uiManagers = FindObjectsByType<UIManager>(FindObjectsSortMode.None);
            foreach (var ui in uiManagers)
            {
                DestroyImmediate(ui.gameObject);
            }
            
            // Remove any gesture managers
            var gestureManagers = FindObjectsByType<GestureManager>(FindObjectsSortMode.None);
            foreach (var gm in gestureManagers)
            {
                DestroyImmediate(gm.gameObject);
            }
            
            // Remove any coin flip managers
            var coinFlipManagers = FindObjectsByType<CoinFlipManager>(FindObjectsSortMode.None);
            foreach (var cf in coinFlipManagers)
            {
                DestroyImmediate(cf.gameObject);
            }
        }
        
        private void CreateGameManager()
        {
            GameObject gameManager = new GameObject("GameManager");
            // Ensure it's in the same scene as this component
            SceneManager.MoveGameObjectToScene(gameManager, gameObject.scene);
            gameManager.AddComponent<GameStateManager>();
        }
        
        private Canvas CreateCanvas()
        {
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1170, 2532);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Create EventSystem
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            return canvas;
        }
        
        private void CreateSimpleUI(Canvas canvas)
        {
            // Create UIManager
            GameObject uiManager = new GameObject("UIManager");
            UIManager uiScript = uiManager.AddComponent<UIManager>();
            
            // Create background panel for better visibility
            GameObject bgPanel = CreatePanel(canvas.transform, "BackgroundPanel",
                new Vector2(0.5f, 0.5f), new Vector2(1170, 2532));
            bgPanel.GetComponent<Image>().color = new Color(0.95f, 0.95f, 0.95f, 1f);
            bgPanel.transform.SetAsFirstSibling();
            
            // Title with background
            GameObject titleBg = CreatePanel(canvas.transform, "TitleBg",
                new Vector2(0.5f, 0.92f), new Vector2(500, 80));
            titleBg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var titleText = CreateText(titleBg.transform, "GameTitle", "JI-GU-PA",
                new Vector2(0.5f, 0.5f), new Vector2(480, 70), 42);
            titleText.color = Color.white;
            
            // Score with background
            GameObject scoreBg = CreatePanel(canvas.transform, "ScoreBg",
                new Vector2(0.5f, 0.84f), new Vector2(400, 50));
            scoreBg.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
            var scoreText = CreateText(scoreBg.transform, "RoundScoreText", "YOU: 0  VS  AI: 0",
                new Vector2(0.5f, 0.5f), new Vector2(380, 40), 22);
            scoreText.color = Color.white;
            
            // Timer with background
            GameObject timerBg = CreatePanel(canvas.transform, "TimerBg",
                new Vector2(0.85f, 0.75f), new Vector2(150, 50));
            timerBg.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f, 1f);
            var timerText = CreateText(timerBg.transform, "TimerText", "",
                new Vector2(0.5f, 0.5f), new Vector2(140, 40), 24);
            timerText.color = Color.white;
            
            // State text with background
            GameObject stateBg = CreatePanel(canvas.transform, "StateBg",
                new Vector2(0.5f, 0.25f), new Vector2(900, 80));
            stateBg.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            var stateText = CreateText(stateBg.transform, "StateText", "Press Start to Begin",
                new Vector2(0.5f, 0.5f), new Vector2(880, 70), 28);
            stateText.color = Color.white;
            
            // Battle area panel
            GameObject battleArea = CreatePanel(canvas.transform, "BattleArea",
                new Vector2(0.5f, 0.55f), new Vector2(1000, 600));
            battleArea.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            
            // Opponent label
            CreateText(battleArea.transform, "OpponentLabel", "OPPONENT",
                new Vector2(0.5f, 0.9f), new Vector2(200, 40), 20);
            
            // Opponent hands with frames
            GameObject p2LeftFrame = CreatePanel(battleArea.transform, "P2LeftFrame",
                new Vector2(0.3f, 0.7f), new Vector2(120, 120));
            var p2LeftText = CreateText(p2LeftFrame.transform, "Player2LeftHandText", "?",
                new Vector2(0.5f, 0.5f), new Vector2(100, 100), 48);
            
            GameObject p2RightFrame = CreatePanel(battleArea.transform, "P2RightFrame",
                new Vector2(0.7f, 0.7f), new Vector2(120, 120));
            var p2RightText = CreateText(p2RightFrame.transform, "Player2RightHandText", "?",
                new Vector2(0.5f, 0.5f), new Vector2(100, 100), 48);
            
            // VS divider
            CreateText(battleArea.transform, "VSText", "- VS -",
                new Vector2(0.5f, 0.5f), new Vector2(100, 40), 24);
            
            // Player label
            CreateText(battleArea.transform, "PlayerLabel", "YOU",
                new Vector2(0.5f, 0.1f), new Vector2(200, 40), 20);
            
            // Player hands with frames
            GameObject p1LeftFrame = CreatePanel(battleArea.transform, "P1LeftFrame",
                new Vector2(0.3f, 0.3f), new Vector2(120, 120));
            var p1LeftText = CreateText(p1LeftFrame.transform, "Player1LeftHandText", "?",
                new Vector2(0.5f, 0.5f), new Vector2(100, 100), 48);
            
            GameObject p1RightFrame = CreatePanel(battleArea.transform, "P1RightFrame",
                new Vector2(0.7f, 0.3f), new Vector2(120, 120));
            var p1RightText = CreateText(p1RightFrame.transform, "Player1RightHandText", "?",
                new Vector2(0.5f, 0.5f), new Vector2(100, 100), 48);
            
            // Start button
            GameObject startButton = CreateButton(canvas.transform, "StartButton", "START",
                new Vector2(0.5f, 0.5f), new Vector2(200, 80));
            
            // Game over panel
            GameObject gameOverPanel = CreatePanel(canvas.transform, "GameOverPanel",
                new Vector2(0.5f, 0.5f), new Vector2(600, 400));
            gameOverPanel.SetActive(false);
            
            var winnerText = CreateText(gameOverPanel.transform, "WinnerText", "",
                new Vector2(0.5f, 0.5f), new Vector2(500, 100), 48);
            
            // Dummy attack overlay
            GameObject attackOverlay = new GameObject("DummyAttackOverlay");
            attackOverlay.transform.SetParent(canvas.transform, false);
            attackOverlay.SetActive(false);
            
            // Link UI elements
            LinkUIManager(uiScript, stateText, timerText, scoreText, startButton.GetComponent<Button>(),
                p1LeftText, p1RightText, p2LeftText, p2RightText,
                attackOverlay, null, null, null, null,
                gameOverPanel, winnerText);
        }
        
        private void CreateSimplePlayerControls(Canvas canvas)
        {
            GameObject player1 = new GameObject("Player1Controller");
            GestureManager p1Manager = player1.AddComponent<GestureManager>();
            
            // Control panel
            GameObject controlPanel = CreatePanel(canvas.transform, "PlayerControlPanel",
                new Vector2(0.5f, 0.1f), new Vector2(900, 400));
            
            // Left hand buttons
            var leftGu = CreateButton(controlPanel.transform, "LeftGuButton", "L-Rock",
                new Vector2(0.1f, 0.7f), new Vector2(120, 60));
            
            var leftPa = CreateButton(controlPanel.transform, "LeftPaButton", "L-Paper",
                new Vector2(0.1f, 0.5f), new Vector2(120, 60));
            
            var leftJi = CreateButton(controlPanel.transform, "LeftJiButton", "L-Scissors",
                new Vector2(0.1f, 0.3f), new Vector2(120, 60));
            
            // Right hand buttons
            var rightGu = CreateButton(controlPanel.transform, "RightGuButton", "R-Rock",
                new Vector2(0.9f, 0.7f), new Vector2(120, 60));
            
            var rightPa = CreateButton(controlPanel.transform, "RightPaButton", "R-Paper",
                new Vector2(0.9f, 0.5f), new Vector2(120, 60));
            
            var rightJi = CreateButton(controlPanel.transform, "RightJiButton", "R-Scissors",
                new Vector2(0.9f, 0.3f), new Vector2(120, 60));
            
            // Submit button (hidden)
            GameObject submitButton = new GameObject("SubmitButton");
            submitButton.transform.SetParent(controlPanel.transform, false);
            submitButton.AddComponent<Button>();
            
            // Link buttons
            LinkGestureManager(p1Manager, true, controlPanel,
                leftGu, leftPa, leftJi,
                rightGu, rightPa, rightJi,
                submitButton);
        }
        
        private void CreateSimpleCoinFlipUI(Canvas canvas)
        {
            GameObject coinFlipManager = new GameObject("CoinFlipManager");
            CoinFlipManager flipScript = coinFlipManager.AddComponent<CoinFlipManager>();
            
            // Coin flip panel
            GameObject coinFlipPanel = CreatePanel(canvas.transform, "CoinFlipPanel",
                new Vector2(0.5f, 0.5f), new Vector2(600, 400));
            coinFlipPanel.SetActive(false);
            
            // Title
            CreateText(coinFlipPanel.transform, "Title", "Choose for coin flip",
                new Vector2(0.5f, 0.8f), new Vector2(500, 60), 32);
            
            // Buttons
            var flipGu = CreateButton(coinFlipPanel.transform, "FlipGu", "Rock",
                new Vector2(0.2f, 0.5f), new Vector2(120, 60));
            
            var flipPa = CreateButton(coinFlipPanel.transform, "FlipPa", "Paper",
                new Vector2(0.5f, 0.5f), new Vector2(120, 60));
            
            var flipJi = CreateButton(coinFlipPanel.transform, "FlipJi", "Scissors",
                new Vector2(0.8f, 0.5f), new Vector2(120, 60));
            
            // Result text
            var resultText = CreateText(coinFlipPanel.transform, "ResultText", "",
                new Vector2(0.5f, 0.2f), new Vector2(500, 60), 24);
            
            // Link to manager
            LinkCoinFlipManager(flipScript, coinFlipPanel,
                flipGu.GetComponent<Button>(),
                flipPa.GetComponent<Button>(),
                flipJi.GetComponent<Button>(),
                resultText);
        }
        
        private GameObject CreatePanel(Transform parent, string name, Vector2 anchorPos, Vector2 size)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            
            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.9f, 0.9f, 0.9f, 0.5f);
            
            return panel;
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
            
            Image image = button.AddComponent<Image>();
            image.color = Color.white;
            
            Button btn = button.AddComponent<Button>();
            
            // Button text
            var btnText = CreateText(button.transform, "Text", text,
                new Vector2(0.5f, 0.5f), size * 0.9f, 16);
            
            return button;
        }
        
        private TextMeshProUGUI CreateText(Transform parent, string name, string text,
            Vector2 anchorPos, Vector2 size, int fontSize)
        {
            GameObject textGO = new GameObject(name);
            textGO.transform.SetParent(parent, false);
            
            RectTransform rect = textGO.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            
            TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.black;
            
            return tmp;
        }
        
        private void LinkUIManager(UIManager uiScript, TextMeshProUGUI stateText,
            TextMeshProUGUI timerText, TextMeshProUGUI scoreText, Button startButton,
            TextMeshProUGUI p1Left, TextMeshProUGUI p1Right,
            TextMeshProUGUI p2Left, TextMeshProUGUI p2Right,
            GameObject attackPanel, TextMeshProUGUI attackLeft, TextMeshProUGUI attackRight,
            GameObject attackLeftLabel, GameObject attackRightLabel,
            GameObject gameOverModal, TextMeshProUGUI winnerText)
        {
            System.Reflection.FieldInfo[] fields = typeof(UIManager).GetFields(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                if (field.Name == "stateText") field.SetValue(uiScript, stateText);
                else if (field.Name == "timerText") field.SetValue(uiScript, timerText);
                else if (field.Name == "roundScoreText") field.SetValue(uiScript, scoreText);
                else if (field.Name == "startButton") field.SetValue(uiScript, startButton);
                else if (field.Name == "player1LeftHandText") field.SetValue(uiScript, p1Left);
                else if (field.Name == "player1RightHandText") field.SetValue(uiScript, p1Right);
                else if (field.Name == "player2LeftHandText") field.SetValue(uiScript, p2Left);
                else if (field.Name == "player2RightHandText") field.SetValue(uiScript, p2Right);
                else if (field.Name == "attackDisplayPanel") field.SetValue(uiScript, attackPanel);
                else if (field.Name == "attackLeftText") field.SetValue(uiScript, attackLeft);
                else if (field.Name == "attackRightText") field.SetValue(uiScript, attackRight);
                else if (field.Name == "attackLeftLabel") field.SetValue(uiScript, attackLeftLabel);
                else if (field.Name == "attackRightLabel") field.SetValue(uiScript, attackRightLabel);
                else if (field.Name == "gameOverPanel") field.SetValue(uiScript, gameOverModal);
                else if (field.Name == "winnerText") field.SetValue(uiScript, winnerText);
            }
        }
        
        private void LinkGestureManager(GestureManager manager, bool isPlayer1, GameObject panel,
            GameObject leftGu, GameObject leftPa, GameObject leftJi,
            GameObject rightGu, GameObject rightPa, GameObject rightJi,
            GameObject submit)
        {
            System.Reflection.FieldInfo[] fields = typeof(GestureManager).GetFields(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                if (field.Name == "leftRockButton") field.SetValue(manager, leftGu.GetComponent<Button>());
                else if (field.Name == "leftPaperButton") field.SetValue(manager, leftPa.GetComponent<Button>());
                else if (field.Name == "leftScissorsButton") field.SetValue(manager, leftJi.GetComponent<Button>());
                else if (field.Name == "rightRockButton") field.SetValue(manager, rightGu.GetComponent<Button>());
                else if (field.Name == "rightPaperButton") field.SetValue(manager, rightPa.GetComponent<Button>());
                else if (field.Name == "rightScissorsButton") field.SetValue(manager, rightJi.GetComponent<Button>());
                else if (field.Name == "submitButton") field.SetValue(manager, submit.GetComponent<Button>());
                else if (field.Name == "gesturePanel") field.SetValue(manager, panel);
                else if (field.Name == "isPlayer1") field.SetValue(manager, isPlayer1);
            }
        }
        
        private void LinkCoinFlipManager(CoinFlipManager flipScript, GameObject panel,
            Button guButton, Button paButton, Button jiButton, TextMeshProUGUI resultText)
        {
            System.Reflection.FieldInfo[] fields = typeof(CoinFlipManager).GetFields(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                if (field.Name == "coinFlipPanel") field.SetValue(flipScript, panel);
                else if (field.Name == "rockButton") field.SetValue(flipScript, guButton);
                else if (field.Name == "paperButton") field.SetValue(flipScript, paButton);
                else if (field.Name == "scissorsButton") field.SetValue(flipScript, jiButton);
                else if (field.Name == "resultText") field.SetValue(flipScript, resultText);
            }
        }
    }
}