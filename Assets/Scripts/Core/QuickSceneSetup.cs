using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Jigupa.Player;
using Jigupa.UI;

namespace Jigupa.Core
{
    public class QuickSceneSetup : MonoBehaviour
    {
        [ContextMenu("Setup iPhone 12 Jigupa Scene")]
        public void SetupCompleteScene()
        {
            // Clean up existing setup
            CleanupExistingSetup();
            
            // Create core managers
            CreateGameManager();
            
            // Create Canvas
            Canvas canvas = CreateCanvas();
            
            // Create UI
            CreateGameUI(canvas);
            
            // Create Player Controllers
            CreatePlayerControllers(canvas);
            
            // Create Coin Flip UI
            CreateCoinFlipUI(canvas);
            
            Debug.Log("Jigupa scene setup complete for iPhone 12! Press Play to test.");
        }
        
        private void CleanupExistingSetup()
        {
            // Remove existing GameManager, Canvas, etc.
            var existingManagers = FindObjectsOfType<GameStateManager>();
            foreach (var manager in existingManagers)
            {
                DestroyImmediate(manager.gameObject);
            }
            
            var existingCanvas = FindObjectOfType<Canvas>();
            if (existingCanvas)
            {
                DestroyImmediate(existingCanvas.gameObject);
            }
        }
        
        private void CreateGameManager()
        {
            GameObject gameManager = new GameObject("GameManager");
            gameManager.AddComponent<GameStateManager>();
        }
        
        private Canvas CreateCanvas()
        {
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1170, 2532); // iPhone 12 resolution
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Create EventSystem
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            return canvas;
        }
        
        private void CreateGameUI(Canvas canvas)
        {
            // Create UIManager
            GameObject uiManager = new GameObject("UIManager");
            UIManager uiScript = uiManager.AddComponent<UIManager>();
            
            // Top Section - Game Info (Safe area aware - moved down for notch)
            GameObject topPanel = CreatePanel(canvas.transform, "TopPanel", 
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), 
                new Vector2(0, -250), new Vector2(1100, 350));
            topPanel.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.95f);
            
            // State Text
            var stateText = CreateText(topPanel.transform, "StateText", "Press Start to Begin",
                new Vector2(0.5f, 0.7f), new Vector2(800, 80), 42);
            stateText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            
            // Timer
            var timerText = CreateText(topPanel.transform, "TimerText", "",
                new Vector2(0.5f, 0.4f), new Vector2(400, 60), 36);
            timerText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.8f, 0.2f);
            
            // Round Score
            var scoreText = CreateText(topPanel.transform, "RoundScoreText", "You: 0 - AI: 0",
                new Vector2(0.5f, 0.1f), new Vector2(500, 60), 38);
            
            // Middle Section - Player Hands Display
            GameObject middlePanel = CreatePanel(canvas.transform, "HandsDisplayPanel",
                new Vector2(0.5f, 0.62f), new Vector2(0.5f, 0.62f),
                new Vector2(0, 0), new Vector2(1000, 500));
            middlePanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.2f, 0.9f);
            
            // Your Hands
            CreateText(middlePanel.transform, "YourLabel", "YOUR HANDS",
                new Vector2(0.5f, 0.9f), new Vector2(300, 50), 32);
            
            var yourLeftLabel = CreateText(middlePanel.transform, "YourLeftLabel", "Left Hand",
                new Vector2(0.25f, 0.7f), new Vector2(200, 40), 24);
            yourLeftLabel.GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
            
            var yourRightLabel = CreateText(middlePanel.transform, "YourRightLabel", "Right Hand",
                new Vector2(0.75f, 0.7f), new Vector2(200, 40), 24);
            yourRightLabel.GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
            
            var p1LeftText = CreateText(middlePanel.transform, "P1LeftHand", "✊ ✋ ✌️",
                new Vector2(0.25f, 0.5f), new Vector2(400, 80), 48);
            var p1RightText = CreateText(middlePanel.transform, "P1RightHand", "✊ ✋ ✌️",
                new Vector2(0.75f, 0.5f), new Vector2(400, 80), 48);
            
            // AI Hands
            CreateText(middlePanel.transform, "AILabel", "AI HANDS",
                new Vector2(0.5f, 0.35f), new Vector2(300, 50), 32);
            
            var aiLeftLabel = CreateText(middlePanel.transform, "AILeftLabel", "Left Hand",
                new Vector2(0.25f, 0.2f), new Vector2(200, 40), 24);
            aiLeftLabel.GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
            
            var aiRightLabel = CreateText(middlePanel.transform, "AIRightLabel", "Right Hand",
                new Vector2(0.75f, 0.2f), new Vector2(200, 40), 24);
            aiRightLabel.GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f);
            
            var p2LeftText = CreateText(middlePanel.transform, "P2LeftHand", "✊ ✋ ✌️",
                new Vector2(0.25f, 0.05f), new Vector2(400, 80), 48);
            var p2RightText = CreateText(middlePanel.transform, "P2RightHand", "✊ ✋ ✌️",
                new Vector2(0.75f, 0.05f), new Vector2(400, 80), 48);
            
            // Attack Display Panel (overlays middle panel)
            GameObject attackPanel = CreatePanel(middlePanel.transform, "AttackDisplayPanel",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(800, 250));
            attackPanel.GetComponent<Image>().color = new Color(0.9f, 0.2f, 0.2f, 0.95f);
            attackPanel.SetActive(false);
            
            var attackLabel = CreateText(attackPanel.transform, "AttackLabel", "INCOMING ATTACK!",
                new Vector2(0.5f, 0.8f), new Vector2(400, 60), 36);
            
            // For single hand attacks, we'll hide the labels and center the gesture
            var attackLeftLabel = CreateText(attackPanel.transform, "AttackLeftLabel", "Left",
                new Vector2(0.25f, 0.5f), new Vector2(150, 40), 24);
            attackLeftLabel.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0.8f);
            
            var attackRightLabel = CreateText(attackPanel.transform, "AttackRightLabel", "Right",
                new Vector2(0.75f, 0.5f), new Vector2(150, 40), 24);
            attackRightLabel.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0.8f);
            
            var attackLeftText = CreateText(attackPanel.transform, "AttackLeftText", "",
                new Vector2(0.25f, 0.2f), new Vector2(300, 80), 52);
            var attackRightText = CreateText(attackPanel.transform, "AttackRightText", "",
                new Vector2(0.75f, 0.2f), new Vector2(300, 80), 52);
            
            // Start Button
            GameObject startButton = CreateButton(canvas.transform, "StartButton", "START GAME",
                new Vector2(0.5f, 0.5f), new Vector2(500, 120), 42);
            startButton.GetComponent<Image>().color = new Color(0.2f, 0.6f, 1f, 1f);
            
            // Game Over Panel
            GameObject gameOverPanel = CreatePanel(canvas.transform, "GameOverPanel",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(900, 700));
            gameOverPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.95f);
            gameOverPanel.SetActive(false);
            
            var winnerText = CreateText(gameOverPanel.transform, "WinnerText", "",
                new Vector2(0.5f, 0.5f), new Vector2(800, 150), 56);
            winnerText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            
            // Link UI elements to UIManager
            System.Reflection.FieldInfo[] fields = typeof(UIManager).GetFields(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                if (field.Name == "stateText") field.SetValue(uiScript, stateText.GetComponent<TextMeshProUGUI>());
                else if (field.Name == "timerText") field.SetValue(uiScript, timerText.GetComponent<TextMeshProUGUI>());
                else if (field.Name == "roundScoreText") field.SetValue(uiScript, scoreText.GetComponent<TextMeshProUGUI>());
                else if (field.Name == "startButton") field.SetValue(uiScript, startButton.GetComponent<Button>());
                else if (field.Name == "player1LeftHandText") field.SetValue(uiScript, p1LeftText.GetComponent<TextMeshProUGUI>());
                else if (field.Name == "player1RightHandText") field.SetValue(uiScript, p1RightText.GetComponent<TextMeshProUGUI>());
                else if (field.Name == "player2LeftHandText") field.SetValue(uiScript, p2LeftText.GetComponent<TextMeshProUGUI>());
                else if (field.Name == "player2RightHandText") field.SetValue(uiScript, p2RightText.GetComponent<TextMeshProUGUI>());
                else if (field.Name == "attackDisplayPanel") field.SetValue(uiScript, attackPanel);
                else if (field.Name == "attackLeftText") field.SetValue(uiScript, attackLeftText.GetComponent<TextMeshProUGUI>());
                else if (field.Name == "attackRightText") field.SetValue(uiScript, attackRightText.GetComponent<TextMeshProUGUI>());
                else if (field.Name == "attackLeftLabel") field.SetValue(uiScript, attackLeftLabel);
                else if (field.Name == "attackRightLabel") field.SetValue(uiScript, attackRightLabel);
                else if (field.Name == "gameOverPanel") field.SetValue(uiScript, gameOverPanel);
                else if (field.Name == "winnerText") field.SetValue(uiScript, winnerText.GetComponent<TextMeshProUGUI>());
            }
        }
        
        private void CreatePlayerControllers(Canvas canvas)
        {
            // Player 1 Controls Only (vs AI)
            GameObject player1 = new GameObject("Player1Controller");
            GestureManager p1Manager = player1.AddComponent<GestureManager>();
            
            GameObject p1Panel = CreatePanel(canvas.transform, "Player1GesturePanel",
                new Vector2(0.5f, 0), new Vector2(0.5f, 0),
                new Vector2(0, 400), new Vector2(1100, 650));
            p1Panel.GetComponent<Image>().color = new Color(0.1f, 0.2f, 0.3f, 0.95f);
            
            CreateText(p1Panel.transform, "P1ControlLabel", "YOUR CONTROLS",
                new Vector2(0.5f, 0.9f), new Vector2(400, 60), 36);
            
            // Left hand section
            GameObject leftSection = CreatePanel(p1Panel.transform, "LeftSection",
                new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                new Vector2(275, -50), new Vector2(500, 500));
            leftSection.GetComponent<Image>().color = new Color(0.15f, 0.25f, 0.35f, 0.8f);
            
            CreateText(leftSection.transform, "LeftLabel", "LEFT HAND",
                new Vector2(0.5f, 0.9f), new Vector2(300, 50), 28);
            
            var p1LeftRock = CreateGestureButton(leftSection.transform, "P1LeftRock", "✊", "Gu",
                new Vector2(0.5f, 0.7f), new Vector2(150, 150));
            var p1LeftPaper = CreateGestureButton(leftSection.transform, "P1LeftPaper", "✋", "Pa",
                new Vector2(0.5f, 0.45f), new Vector2(150, 150));
            var p1LeftScissors = CreateGestureButton(leftSection.transform, "P1LeftScissors", "✌️", "Ji",
                new Vector2(0.5f, 0.2f), new Vector2(150, 150));
            
            // Right hand section
            GameObject rightSection = CreatePanel(p1Panel.transform, "RightSection",
                new Vector2(1, 0.5f), new Vector2(1, 0.5f),
                new Vector2(-275, -50), new Vector2(500, 500));
            rightSection.GetComponent<Image>().color = new Color(0.15f, 0.25f, 0.35f, 0.8f);
            
            CreateText(rightSection.transform, "RightLabel", "RIGHT HAND",
                new Vector2(0.5f, 0.9f), new Vector2(300, 50), 28);
            
            var p1RightRock = CreateGestureButton(rightSection.transform, "P1RightRock", "✊", "Gu",
                new Vector2(0.5f, 0.7f), new Vector2(150, 150));
            var p1RightPaper = CreateGestureButton(rightSection.transform, "P1RightPaper", "✋", "Pa",
                new Vector2(0.5f, 0.45f), new Vector2(150, 150));
            var p1RightScissors = CreateGestureButton(rightSection.transform, "P1RightScissors", "✌️", "Ji",
                new Vector2(0.5f, 0.2f), new Vector2(150, 150));
            
            // Submit button
            var p1Submit = CreateButton(p1Panel.transform, "P1SubmitButton", "CONFIRM SELECTION",
                new Vector2(0.5f, 0.1f), new Vector2(600, 100), 36);
            p1Submit.GetComponent<Image>().color = new Color(0.2f, 0.7f, 0.3f, 1f);
            
            // Link buttons to GestureManager
            LinkGestureManager(p1Manager, true, p1Panel,
                p1LeftRock, p1LeftPaper, p1LeftScissors,
                p1RightRock, p1RightPaper, p1RightScissors,
                p1Submit);
        }
        
        private void CreateCoinFlipUI(Canvas canvas)
        {
            GameObject coinFlipManager = new GameObject("CoinFlipManager");
            CoinFlipManager flipScript = coinFlipManager.AddComponent<CoinFlipManager>();
            
            // Coin Flip Panel
            GameObject coinFlipPanel = CreatePanel(canvas.transform, "CoinFlipPanel",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(900, 800));
            coinFlipPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.2f, 0.98f);
            coinFlipPanel.SetActive(false);
            
            CreateText(coinFlipPanel.transform, "CoinFlipTitle", "GU PA JI",
                new Vector2(0.5f, 0.85f), new Vector2(700, 80), 42);
            
            CreateText(coinFlipPanel.transform, "CoinFlipSubtitle", "Winner goes first!",
                new Vector2(0.5f, 0.7f), new Vector2(600, 60), 32);
            
            // Coin flip buttons
            var flipRock = CreateGestureButton(coinFlipPanel.transform, "FlipRock", "✊", "Gu",
                new Vector2(0.2f, 0.45f), new Vector2(200, 200));
            var flipPaper = CreateGestureButton(coinFlipPanel.transform, "FlipPaper", "✋", "Pa",
                new Vector2(0.5f, 0.45f), new Vector2(200, 200));
            var flipScissors = CreateGestureButton(coinFlipPanel.transform, "FlipScissors", "✌️", "Ji",
                new Vector2(0.8f, 0.45f), new Vector2(200, 200));
            
            var resultText = CreateText(coinFlipPanel.transform, "CoinFlipResult", "",
                new Vector2(0.5f, 0.15f), new Vector2(800, 120), 36);
            
            // Link to CoinFlipManager
            System.Reflection.FieldInfo[] fields = typeof(CoinFlipManager).GetFields(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                if (field.Name == "coinFlipPanel") field.SetValue(flipScript, coinFlipPanel);
                else if (field.Name == "rockButton") field.SetValue(flipScript, flipRock.GetComponent<Button>());
                else if (field.Name == "paperButton") field.SetValue(flipScript, flipPaper.GetComponent<Button>());
                else if (field.Name == "scissorsButton") field.SetValue(flipScript, flipScissors.GetComponent<Button>());
                else if (field.Name == "resultText") field.SetValue(flipScript, resultText.GetComponent<TextMeshProUGUI>());
            }
        }
        
        private void LinkGestureManager(GestureManager manager, bool isPlayer1, GameObject panel,
            GameObject leftRock, GameObject leftPaper, GameObject leftScissors,
            GameObject rightRock, GameObject rightPaper, GameObject rightScissors,
            GameObject submit)
        {
            System.Reflection.FieldInfo[] fields = typeof(GestureManager).GetFields(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                if (field.Name == "leftRockButton") field.SetValue(manager, leftRock.GetComponent<Button>());
                else if (field.Name == "leftPaperButton") field.SetValue(manager, leftPaper.GetComponent<Button>());
                else if (field.Name == "leftScissorsButton") field.SetValue(manager, leftScissors.GetComponent<Button>());
                else if (field.Name == "rightRockButton") field.SetValue(manager, rightRock.GetComponent<Button>());
                else if (field.Name == "rightPaperButton") field.SetValue(manager, rightPaper.GetComponent<Button>());
                else if (field.Name == "rightScissorsButton") field.SetValue(manager, rightScissors.GetComponent<Button>());
                else if (field.Name == "submitButton") field.SetValue(manager, submit.GetComponent<Button>());
                else if (field.Name == "gesturePanel") field.SetValue(manager, panel);
                else if (field.Name == "isPlayer1") field.SetValue(manager, isPlayer1);
            }
        }
        
        private GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 position, Vector2 size)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            
            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            
            return panel;
        }
        
        private GameObject CreateText(Transform parent, string name, string text,
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
            tmp.color = Color.white;
            
            return textGO;
        }
        
        private GameObject CreateButton(Transform parent, string name, string text,
            Vector2 anchorPos, Vector2 size, int fontSize)
        {
            GameObject buttonGO = new GameObject(name);
            buttonGO.transform.SetParent(parent, false);
            
            RectTransform rect = buttonGO.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            
            Image image = buttonGO.AddComponent<Image>();
            image.color = new Color(0.3f, 0.6f, 0.9f, 1f);
            
            Button button = buttonGO.AddComponent<Button>();
            
            GameObject textGO = CreateText(buttonGO.transform, name + "_Text", text,
                new Vector2(0.5f, 0.5f), size, fontSize);
            
            return buttonGO;
        }
        
        private GameObject CreateGestureButton(Transform parent, string name, string emoji, string label,
            Vector2 anchorPos, Vector2 size)
        {
            GameObject button = CreateButton(parent, name, "", anchorPos, size, 48);
            button.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.5f, 1f);
            
            // Add emoji
            var emojiText = CreateText(button.transform, name + "_Emoji", emoji,
                new Vector2(0.5f, 0.6f), new Vector2(size.x, size.y * 0.5f), 64);
            
            // Add label
            var labelText = CreateText(button.transform, name + "_Label", label,
                new Vector2(0.5f, 0.2f), new Vector2(size.x, size.y * 0.3f), 24);
            labelText.GetComponent<TextMeshProUGUI>().color = new Color(0.9f, 0.9f, 0.9f);
            
            return button;
        }
    }
}