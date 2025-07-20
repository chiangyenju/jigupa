using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Jigupa.Player;
using Jigupa.UI;

namespace Jigupa.Core
{
    public class MinimalUISetup : MonoBehaviour
    {
        // Design System Colors
        private readonly Color primaryRed = new Color(0.675f, 0.035f, 0.161f); // #AC0929
        private readonly Color primaryDark = new Color(0.478f, 0.024f, 0.114f); // #7A061D
        private readonly Color offBlack = new Color(0.102f, 0.102f, 0.102f); // #1A1A1A
        private readonly Color darkGray = new Color(0.251f, 0.251f, 0.251f); // #404040
        private readonly Color mediumGray = new Color(0.502f, 0.502f, 0.502f); // #808080
        private readonly Color lightGray = new Color(0.878f, 0.878f, 0.878f); // #E0E0E0
        private readonly Color offWhite = new Color(0.973f, 0.973f, 0.973f); // #F8F8F8
        private readonly Color playerBlue = new Color(0.145f, 0.388f, 0.922f); // #2563EB
        private readonly Color aiPurple = new Color(0.486f, 0.231f, 0.929f); // #7C3AED
        
        [ContextMenu("Setup Minimal Jigupa Scene")]
        public void SetupCompleteScene()
        {
            // Clean up existing setup
            CleanupExistingSetup();
            
            // Create core managers
            CreateGameManager();
            
            // Create Canvas with minimal design
            Canvas canvas = CreateMinimalCanvas();
            
            // Create minimalist UI
            CreateMinimalGameUI(canvas);
            
            // Create Player Controllers with new design
            CreateMinimalPlayerControls(canvas);
            
            // Create Coin Flip UI with new design
            CreateMinimalCoinFlipUI(canvas);
            
            Debug.Log("Minimal Jigupa scene setup complete! Beautiful, clean design applied.");
        }
        
        private void CleanupExistingSetup()
        {
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
        
        private Canvas CreateMinimalCanvas()
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
            
            // Set canvas background to white
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(canvas.transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = Color.white;
            
            // Create EventSystem
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            return canvas;
        }
        
        private void CreateMinimalGameUI(Canvas canvas)
        {
            // Create UIManager
            GameObject uiManager = new GameObject("UIManager");
            UIManager uiScript = uiManager.AddComponent<UIManager>();
            
            // Top Section - Minimal header with safe area
            GameObject topSection = CreatePanel(canvas.transform, "TopSection",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0, -100), new Vector2(1100, 200));
            topSection.GetComponent<Image>().color = Color.white;
            
            // Game Title
            var titleText = CreateText(topSection.transform, "GameTitle", "嘰咕帕",
                new Vector2(0.5f, 0.7f), new Vector2(400, 80), 40, FontWeight.Light);
            titleText.GetComponent<TextMeshProUGUI>().color = offBlack;
            
            // Score Pills
            GameObject scoreContainer = new GameObject("ScoreContainer");
            scoreContainer.transform.SetParent(topSection.transform, false);
            RectTransform scoreRect = scoreContainer.AddComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0.5f, 0.2f);
            scoreRect.anchorMax = new Vector2(0.5f, 0.2f);
            scoreRect.sizeDelta = new Vector2(300, 48);
            scoreRect.anchoredPosition = Vector2.zero;
            
            GameObject scorePill = CreatePill(scoreContainer.transform, "ScorePill",
                Vector2.zero, new Vector2(300, 48));
            var scoreText = CreateText(scorePill.transform, "RoundScoreText", "You 0 • AI 0",
                new Vector2(0.5f, 0.5f), new Vector2(280, 48), 18, FontWeight.Medium);
            scoreText.GetComponent<TextMeshProUGUI>().color = darkGray;
            
            // Main Game Area
            GameObject mainArea = CreatePanel(canvas.transform, "MainGameArea",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0, 50), new Vector2(1000, 800));
            mainArea.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            
            // State Display Card
            GameObject stateCard = CreateCard(mainArea.transform, "StateCard",
                new Vector2(0.5f, 0.9f), new Vector2(800, 120));
            
            var stateText = CreateText(stateCard.transform, "StateText", "Press Start",
                new Vector2(0.5f, 0.6f), new Vector2(700, 60), 24, FontWeight.SemiBold);
            stateText.GetComponent<TextMeshProUGUI>().color = offBlack;
            
            var timerText = CreateText(stateCard.transform, "TimerText", "",
                new Vector2(0.5f, 0.2f), new Vector2(200, 40), 14, FontWeight.Regular);
            timerText.GetComponent<TextMeshProUGUI>().color = primaryRed;
            
            // Hands Display Grid
            GameObject handsGrid = new GameObject("HandsGrid");
            handsGrid.transform.SetParent(mainArea.transform, false);
            RectTransform gridRect = handsGrid.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.5f, 0.4f);
            gridRect.anchorMax = new Vector2(0.5f, 0.4f);
            gridRect.sizeDelta = new Vector2(900, 400);
            gridRect.anchoredPosition = Vector2.zero;
            
            // Your Hands Section
            CreateHandsSection(handsGrid.transform, "YourHands", 
                new Vector2(0.5f, 0.75f), "YOUR HANDS", playerBlue,
                out var p1LeftText, out var p1RightText);
            
            // AI Hands Section
            CreateHandsSection(handsGrid.transform, "AIHands",
                new Vector2(0.5f, 0.25f), "AI HANDS", aiPurple,
                out var p2LeftText, out var p2RightText);
            
            // Attack Display Overlay
            GameObject attackOverlay = CreateAttackOverlay(mainArea.transform);
            var attackLeftText = attackOverlay.transform.Find("Content/AttackGestures/LeftGesture/GestureText").GetComponent<TextMeshProUGUI>();
            var attackRightText = attackOverlay.transform.Find("Content/AttackGestures/RightGesture/GestureText").GetComponent<TextMeshProUGUI>();
            var attackLeftLabel = attackOverlay.transform.Find("Content/AttackGestures/LeftGesture").gameObject;
            var attackRightLabel = attackOverlay.transform.Find("Content/AttackGestures/RightGesture").gameObject;
            
            // Start Button
            GameObject startButton = CreatePrimaryButton(canvas.transform, "StartButton", "START GAME",
                new Vector2(0.5f, 0.5f), new Vector2(320, 64));
            
            // Game Over Modal
            GameObject gameOverModal = CreateGameOverModal(canvas.transform);
            var winnerText = gameOverModal.transform.Find("Content/WinnerText").GetComponent<TextMeshProUGUI>();
            
            // Link UI elements to UIManager
            LinkUIManager(uiScript, stateText, timerText, scoreText, startButton,
                p1LeftText, p1RightText, p2LeftText, p2RightText,
                attackOverlay, attackLeftText, attackRightText, attackLeftLabel, attackRightLabel,
                gameOverModal, winnerText);
        }
        
        private void CreateHandsSection(Transform parent, string name, Vector2 anchorPos, 
            string label, Color accentColor, out TextMeshProUGUI leftText, out TextMeshProUGUI rightText)
        {
            GameObject section = new GameObject(name);
            section.transform.SetParent(parent, false);
            RectTransform rect = section.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.sizeDelta = new Vector2(800, 160);
            rect.anchoredPosition = Vector2.zero;
            
            // Label
            var labelText = CreateText(section.transform, "Label", label,
                new Vector2(0.5f, 0.85f), new Vector2(200, 30), 14, FontWeight.Medium);
            labelText.GetComponent<TextMeshProUGUI>().color = accentColor;
            
            // Hand Cards Container
            GameObject handContainer = new GameObject("HandContainer");
            handContainer.transform.SetParent(section.transform, false);
            RectTransform containerRect = handContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.35f);
            containerRect.anchorMax = new Vector2(0.5f, 0.35f);
            containerRect.sizeDelta = new Vector2(400, 100);
            containerRect.anchoredPosition = Vector2.zero;
            
            // Left Hand Card
            GameObject leftCard = CreateHandCard(handContainer.transform, "LeftHand",
                new Vector2(0.25f, 0.5f), "LEFT");
            leftText = leftCard.transform.Find("HandIcon").GetComponent<TextMeshProUGUI>();
            
            // Right Hand Card
            GameObject rightCard = CreateHandCard(handContainer.transform, "RightHand",
                new Vector2(0.75f, 0.5f), "RIGHT");
            rightText = rightCard.transform.Find("HandIcon").GetComponent<TextMeshProUGUI>();
        }
        
        private GameObject CreateHandCard(Transform parent, string name, Vector2 anchorPos, string label)
        {
            GameObject card = CreateCard(parent, name, anchorPos, new Vector2(160, 100));
            
            // Label
            var labelText = CreateText(card.transform, "Label", label,
                new Vector2(0.5f, 0.8f), new Vector2(100, 20), 12, FontWeight.Regular);
            labelText.GetComponent<TextMeshProUGUI>().color = mediumGray;
            
            // Hand Icon
            var handIcon = CreateText(card.transform, "HandIcon", "✊ ✋ ✌️",
                new Vector2(0.5f, 0.35f), new Vector2(140, 60), 32, FontWeight.Regular);
            handIcon.GetComponent<TextMeshProUGUI>().color = offBlack;
            
            return card;
        }
        
        private GameObject CreateAttackOverlay(Transform parent)
        {
            GameObject overlay = new GameObject("AttackOverlay");
            overlay.transform.SetParent(parent, false);
            overlay.SetActive(false);
            
            RectTransform overlayRect = overlay.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.sizeDelta = Vector2.zero;
            overlayRect.anchoredPosition = Vector2.zero;
            
            // Semi-transparent background
            Image overlayBg = overlay.AddComponent<Image>();
            overlayBg.color = new Color(0, 0, 0, 0.3f);
            
            // Content Card
            GameObject content = CreateCard(overlay.transform, "Content",
                new Vector2(0.5f, 0.5f), new Vector2(600, 320));
            content.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.98f);
            
            // Add red accent border
            Outline outline = content.AddComponent<Outline>();
            outline.effectColor = primaryRed;
            outline.effectDistance = new Vector2(2, -2);
            
            // Warning Icon
            var warningIcon = CreateText(content.transform, "WarningIcon", "⚡",
                new Vector2(0.5f, 0.85f), new Vector2(80, 80), 48, FontWeight.Light);
            warningIcon.GetComponent<TextMeshProUGUI>().color = primaryRed;
            
            // Title
            var title = CreateText(content.transform, "Title", "INCOMING ATTACK",
                new Vector2(0.5f, 0.65f), new Vector2(400, 40), 18, FontWeight.SemiBold);
            title.GetComponent<TextMeshProUGUI>().color = primaryRed;
            
            // Attack Gestures Container
            GameObject gesturesContainer = new GameObject("AttackGestures");
            gesturesContainer.transform.SetParent(content.transform, false);
            RectTransform gesturesRect = gesturesContainer.AddComponent<RectTransform>();
            gesturesRect.anchorMin = new Vector2(0.5f, 0.25f);
            gesturesRect.anchorMax = new Vector2(0.5f, 0.25f);
            gesturesRect.sizeDelta = new Vector2(400, 120);
            gesturesRect.anchoredPosition = Vector2.zero;
            
            // Left Gesture
            CreateAttackGestureDisplay(gesturesContainer.transform, "LeftGesture",
                new Vector2(0.25f, 0.5f));
            
            // Right Gesture
            CreateAttackGestureDisplay(gesturesContainer.transform, "RightGesture",
                new Vector2(0.75f, 0.5f));
            
            return overlay;
        }
        
        private void CreateAttackGestureDisplay(Transform parent, string name, Vector2 anchorPos)
        {
            GameObject gesture = new GameObject(name);
            gesture.transform.SetParent(parent, false);
            RectTransform rect = gesture.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.sizeDelta = new Vector2(120, 120);
            rect.anchoredPosition = Vector2.zero;
            
            // Gesture Text
            var gestureText = CreateText(gesture.transform, "GestureText", "",
                new Vector2(0.5f, 0.5f), new Vector2(120, 120), 56, FontWeight.Light);
            gestureText.GetComponent<TextMeshProUGUI>().color = offBlack;
        }
        
        private GameObject CreateGameOverModal(Transform parent)
        {
            GameObject modal = new GameObject("GameOverModal");
            modal.transform.SetParent(parent, false);
            modal.SetActive(false);
            
            RectTransform modalRect = modal.AddComponent<RectTransform>();
            modalRect.anchorMin = Vector2.zero;
            modalRect.anchorMax = Vector2.one;
            modalRect.sizeDelta = Vector2.zero;
            modalRect.anchoredPosition = Vector2.zero;
            
            // Background overlay
            Image modalBg = modal.AddComponent<Image>();
            modalBg.color = new Color(0, 0, 0, 0.7f);
            
            // Content
            GameObject content = CreateCard(modal.transform, "Content",
                new Vector2(0.5f, 0.5f), new Vector2(560, 400));
            
            // Trophy Icon
            var trophyIcon = CreateText(content.transform, "TrophyIcon", "🏆",
                new Vector2(0.5f, 0.75f), new Vector2(120, 120), 80, FontWeight.Light);
            
            // Winner Text
            var winnerText = CreateText(content.transform, "WinnerText", "",
                new Vector2(0.5f, 0.5f), new Vector2(400, 80), 40, FontWeight.SemiBold);
            winnerText.GetComponent<TextMeshProUGUI>().color = offBlack;
            
            // Final Score
            var finalScore = CreateText(content.transform, "FinalScore", "",
                new Vector2(0.5f, 0.35f), new Vector2(300, 40), 18, FontWeight.Regular);
            finalScore.GetComponent<TextMeshProUGUI>().color = darkGray;
            
            // Play Again Button
            CreatePrimaryButton(content.transform, "PlayAgainButton", "PLAY AGAIN",
                new Vector2(0.5f, 0.15f), new Vector2(240, 56));
            
            return modal;
        }
        
        private void CreateMinimalPlayerControls(Canvas canvas)
        {
            GameObject player1 = new GameObject("Player1Controller");
            GestureManager p1Manager = player1.AddComponent<GestureManager>();
            
            // Player Control Panel
            GameObject controlPanel = CreatePanel(canvas.transform, "PlayerControlPanel",
                new Vector2(0.5f, 0), new Vector2(0.5f, 0),
                new Vector2(0, 320), new Vector2(1100, 550));
            controlPanel.GetComponent<Image>().color = offWhite;
            
            // Add subtle top border
            GameObject topBorder = new GameObject("TopBorder");
            topBorder.transform.SetParent(controlPanel.transform, false);
            RectTransform borderRect = topBorder.AddComponent<RectTransform>();
            borderRect.anchorMin = new Vector2(0.5f, 1f);
            borderRect.anchorMax = new Vector2(0.5f, 1f);
            borderRect.sizeDelta = new Vector2(1100, 1);
            borderRect.anchoredPosition = Vector2.zero;
            Image borderImage = topBorder.AddComponent<Image>();
            borderImage.color = lightGray;
            
            // Section Title
            var sectionTitle = CreateText(controlPanel.transform, "SectionTitle", "SELECT YOUR MOVE",
                new Vector2(0.5f, 0.88f), new Vector2(400, 40), 14, FontWeight.Medium);
            sectionTitle.GetComponent<TextMeshProUGUI>().color = mediumGray;
            sectionTitle.GetComponent<TextMeshProUGUI>().letterSpacing = 2f;
            
            // Hand Sections Container
            GameObject handsContainer = new GameObject("HandsContainer");
            handsContainer.transform.SetParent(controlPanel.transform, false);
            RectTransform handsRect = handsContainer.AddComponent<RectTransform>();
            handsRect.anchorMin = new Vector2(0.5f, 0.45f);
            handsRect.anchorMax = new Vector2(0.5f, 0.45f);
            handsRect.sizeDelta = new Vector2(1000, 300);
            handsRect.anchoredPosition = Vector2.zero;
            
            // Left Hand Section
            var leftButtons = CreateHandControlSection(handsContainer.transform, "LeftHandSection",
                new Vector2(0.25f, 0.5f), "LEFT HAND", true);
            
            // Divider
            GameObject divider = new GameObject("Divider");
            divider.transform.SetParent(handsContainer.transform, false);
            RectTransform dividerRect = divider.AddComponent<RectTransform>();
            dividerRect.anchorMin = new Vector2(0.5f, 0.5f);
            dividerRect.anchorMax = new Vector2(0.5f, 0.5f);
            dividerRect.sizeDelta = new Vector2(1, 200);
            dividerRect.anchoredPosition = Vector2.zero;
            Image dividerImage = divider.AddComponent<Image>();
            dividerImage.color = lightGray;
            
            // Right Hand Section
            var rightButtons = CreateHandControlSection(handsContainer.transform, "RightHandSection",
                new Vector2(0.75f, 0.5f), "RIGHT HAND", false);
            
            // Submit Button
            var submitButton = CreatePrimaryButton(controlPanel.transform, "SubmitButton", "CONFIRM",
                new Vector2(0.5f, 0.08f), new Vector2(280, 56));
            
            // Link buttons to GestureManager
            LinkGestureManager(p1Manager, true, controlPanel,
                leftButtons[0], leftButtons[1], leftButtons[2],
                rightButtons[0], rightButtons[1], rightButtons[2],
                submitButton);
        }
        
        private GameObject[] CreateHandControlSection(Transform parent, string name, 
            Vector2 anchorPos, string label, bool isLeft)
        {
            GameObject section = new GameObject(name);
            section.transform.SetParent(parent, false);
            RectTransform rect = section.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.sizeDelta = new Vector2(400, 300);
            rect.anchoredPosition = Vector2.zero;
            
            // Section Label
            var labelText = CreateText(section.transform, "Label", label,
                new Vector2(0.5f, 0.92f), new Vector2(200, 30), 12, FontWeight.Medium);
            labelText.GetComponent<TextMeshProUGUI>().color = mediumGray;
            
            // Gesture Buttons
            GameObject[] buttons = new GameObject[3];
            buttons[0] = CreateMinimalGestureButton(section.transform, 
                isLeft ? "LeftGuButton" : "RightGuButton", "✊", "Gu",
                new Vector2(0.2f, 0.45f));
            buttons[1] = CreateMinimalGestureButton(section.transform,
                isLeft ? "LeftPaButton" : "RightPaButton", "✋", "Pa",
                new Vector2(0.5f, 0.45f));
            buttons[2] = CreateMinimalGestureButton(section.transform,
                isLeft ? "LeftJiButton" : "RightJiButton", "✌️", "Ji",
                new Vector2(0.8f, 0.45f));
            
            return buttons;
        }
        
        private GameObject CreateMinimalGestureButton(Transform parent, string name,
            string emoji, string label, Vector2 anchorPos)
        {
            GameObject button = new GameObject(name);
            button.transform.SetParent(parent, false);
            RectTransform rect = button.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.sizeDelta = new Vector2(100, 100);
            rect.anchoredPosition = Vector2.zero;
            
            // Button component
            Button btn = button.AddComponent<Button>();
            
            // Background
            Image bg = button.AddComponent<Image>();
            bg.color = Color.white;
            
            // Circle mask
            button.AddComponent<Mask>().showMaskGraphic = true;
            
            // Border
            GameObject border = new GameObject("Border");
            border.transform.SetParent(button.transform, false);
            RectTransform borderRect = border.AddComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.sizeDelta = new Vector2(-4, -4);
            borderRect.anchoredPosition = Vector2.zero;
            Outline borderOutline = border.AddComponent<Outline>();
            borderOutline.effectColor = lightGray;
            borderOutline.effectDistance = new Vector2(2, -2);
            
            // Emoji
            var emojiText = CreateText(button.transform, "Emoji", emoji,
                new Vector2(0.5f, 0.6f), new Vector2(80, 80), 40, FontWeight.Regular);
            
            // Label
            var labelText = CreateText(button.transform, "Label", label,
                new Vector2(0.5f, 0.15f), new Vector2(80, 20), 12, FontWeight.Regular);
            labelText.GetComponent<TextMeshProUGUI>().color = darkGray;
            
            // Button animation
            btn.transition = Selectable.Transition.Animation;
            
            return button;
        }
        
        private void CreateMinimalCoinFlipUI(Canvas canvas)
        {
            GameObject coinFlipManager = new GameObject("CoinFlipManager");
            CoinFlipManager flipScript = coinFlipManager.AddComponent<CoinFlipManager>();
            
            // Coin Flip Modal
            GameObject coinFlipModal = new GameObject("CoinFlipModal");
            coinFlipModal.transform.SetParent(canvas.transform, false);
            coinFlipModal.SetActive(false);
            
            RectTransform modalRect = coinFlipModal.AddComponent<RectTransform>();
            modalRect.anchorMin = Vector2.zero;
            modalRect.anchorMax = Vector2.one;
            modalRect.sizeDelta = Vector2.zero;
            modalRect.anchoredPosition = Vector2.zero;
            
            // Background
            Image modalBg = coinFlipModal.AddComponent<Image>();
            modalBg.color = new Color(0, 0, 0, 0.5f);
            
            // Content Card
            GameObject content = CreateCard(coinFlipModal.transform, "Content",
                new Vector2(0.5f, 0.5f), new Vector2(700, 600));
            
            // Title
            var title = CreateText(content.transform, "Title", "咕帕嘰",
                new Vector2(0.5f, 0.85f), new Vector2(400, 60), 40, FontWeight.Light);
            title.GetComponent<TextMeshProUGUI>().color = offBlack;
            
            // Subtitle
            var subtitle = CreateText(content.transform, "Subtitle", 
                "Winner goes first • Rock-Paper-Scissors rules",
                new Vector2(0.5f, 0.72f), new Vector2(500, 30), 14, FontWeight.Regular);
            subtitle.GetComponent<TextMeshProUGUI>().color = mediumGray;
            
            // Gesture Buttons Container
            GameObject buttonsContainer = new GameObject("ButtonsContainer");
            buttonsContainer.transform.SetParent(content.transform, false);
            RectTransform buttonsRect = buttonsContainer.AddComponent<RectTransform>();
            buttonsRect.anchorMin = new Vector2(0.5f, 0.45f);
            buttonsRect.anchorMax = new Vector2(0.5f, 0.45f);
            buttonsRect.sizeDelta = new Vector2(500, 150);
            buttonsRect.anchoredPosition = Vector2.zero;
            
            // Coin flip buttons
            var flipGu = CreateMinimalGestureButton(buttonsContainer.transform,
                "FlipGu", "✊", "Gu", new Vector2(0.2f, 0.5f));
            flipGu.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120);
            
            var flipPa = CreateMinimalGestureButton(buttonsContainer.transform,
                "FlipPa", "✋", "Pa", new Vector2(0.5f, 0.5f));
            flipPa.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120);
            
            var flipJi = CreateMinimalGestureButton(buttonsContainer.transform,
                "FlipJi", "✌️", "Ji", new Vector2(0.8f, 0.5f));
            flipJi.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120);
            
            // Result Text
            var resultText = CreateText(content.transform, "ResultText", "",
                new Vector2(0.5f, 0.2f), new Vector2(600, 80), 18, FontWeight.Regular);
            resultText.GetComponent<TextMeshProUGUI>().color = darkGray;
            
            // Link to CoinFlipManager
            LinkCoinFlipManager(flipScript, coinFlipModal, 
                flipGu.GetComponent<Button>(), 
                flipPa.GetComponent<Button>(), 
                flipJi.GetComponent<Button>(),
                resultText.GetComponent<TextMeshProUGUI>());
        }
        
        // Helper Methods
        private GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, 
            Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            
            Image image = panel.AddComponent<Image>();
            image.color = Color.white;
            
            return panel;
        }
        
        private GameObject CreateCard(Transform parent, string name, Vector2 anchorPos, Vector2 size)
        {
            GameObject card = new GameObject(name);
            card.transform.SetParent(parent, false);
            
            RectTransform rect = card.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            
            Image image = card.AddComponent<Image>();
            image.color = Color.white;
            
            // Add shadow
            Shadow shadow = card.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.08f);
            shadow.effectDistance = new Vector2(0, -8);
            
            return card;
        }
        
        private GameObject CreatePill(Transform parent, string name, Vector2 position, Vector2 size)
        {
            GameObject pill = CreatePanel(parent, name, new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), position, size);
            pill.GetComponent<Image>().color = offWhite;
            
            return pill;
        }
        
        private GameObject CreateText(Transform parent, string name, string text,
            Vector2 anchorPos, Vector2 size, int fontSize, FontWeight weight)
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
            tmp.fontWeight = weight;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = offBlack;
            
            return textGO;
        }
        
        private GameObject CreatePrimaryButton(Transform parent, string name, string text,
            Vector2 anchorPos, Vector2 size)
        {
            GameObject buttonGO = new GameObject(name);
            buttonGO.transform.SetParent(parent, false);
            
            RectTransform rect = buttonGO.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            
            Image image = buttonGO.AddComponent<Image>();
            image.color = primaryRed;
            
            Button button = buttonGO.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            
            ColorBlock colors = button.colors;
            colors.normalColor = primaryRed;
            colors.highlightedColor = primaryDark;
            colors.pressedColor = primaryDark;
            colors.disabledColor = mediumGray;
            button.colors = colors;
            
            // Add shadow
            Shadow shadow = buttonGO.AddComponent<Shadow>();
            shadow.effectColor = new Color(0.675f, 0.035f, 0.161f, 0.2f);
            shadow.effectDistance = new Vector2(0, -4);
            
            // Button text
            var buttonText = CreateText(buttonGO.transform, "Text", text,
                new Vector2(0.5f, 0.5f), size, 16, FontWeight.Medium);
            buttonText.GetComponent<TextMeshProUGUI>().color = Color.white;
            
            return buttonGO;
        }
        
        private void LinkUIManager(UIManager uiScript, TextMeshProUGUI stateText,
            TextMeshProUGUI timerText, TextMeshProUGUI scoreText, GameObject startButton,
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
                else if (field.Name == "startButton") field.SetValue(uiScript, startButton.GetComponent<Button>());
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
        
        private enum FontWeight
        {
            Light = 300,
            Regular = 400,
            Medium = 500,
            SemiBold = 600
        }
    }
}