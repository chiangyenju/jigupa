using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Jigupa.Core;

namespace Jigupa.UI
{
    public class MainMenuUISetup : MonoBehaviour
    {
        [ContextMenu("Setup Main Menu UI")]
        public void SetupMainMenu()
        {
            // Clean up existing UI
            CleanupExistingUI();
            
            // Create Canvas
            Canvas canvas = CreateCanvas();
            
            // Create main container
            CreateMainContainer(canvas);
            
            // Create footer navigation
            CreateFooterNavigation(canvas);
            
            Debug.Log("Main Menu UI setup complete!");
        }
        
        private void CleanupExistingUI()
        {
            var existingCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var canvas in existingCanvases)
            {
                if (canvas.name == "MainMenuCanvas")
                {
                    DestroyImmediate(canvas.gameObject);
                }
            }
        }
        
        private Canvas CreateCanvas()
        {
            GameObject canvasGO = new GameObject("MainMenuCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10; // Above game UI
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1170, 2532);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Create EventSystem if needed
            var existingEventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (!existingEventSystem)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("Created EventSystem for UI interaction");
            }
            else
            {
                Debug.Log("EventSystem already exists");
            }
            
            return canvas;
        }
        
        private void CreateMainContainer(Canvas canvas)
        {
            // Background
            GameObject bg = CreatePanel(canvas.transform, "Background",
                new Vector2(0.5f, 0.5f), new Vector2(1170, 2532));
            bg.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
            bg.transform.SetAsFirstSibling();
            
            // Content area (above footer)
            GameObject contentArea = CreatePanel(canvas.transform, "ContentArea",
                new Vector2(0.5f, 0.55f), new Vector2(1170, 2100));
            contentArea.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            // Create content panels for each tab
            CreatePlayerPanel(contentArea.transform);
            CreateGuildPanel(contentArea.transform);
            CreateBattlePanel(contentArea.transform);
            CreateMinigamePanel(contentArea.transform);
            CreateShopPanel(contentArea.transform);
        }
        
        private void CreateFooterNavigation(Canvas canvas)
        {
            // Footer container
            GameObject footer = CreatePanel(canvas.transform, "Footer",
                new Vector2(0.5f, 0.05f), new Vector2(1170, 200));
            footer.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Add navigation controller
            NavigationController navController = canvas.gameObject.AddComponent<NavigationController>();
            
            // Create footer buttons
            float buttonWidth = 234f; // 1170 / 5
            float xStart = 0.1f;
            float xStep = 0.2f;
            
            // Player button
            GameObject playerBtn = CreateFooterButton(footer.transform, "PlayerButton", "PLAYER",
                new Vector2(xStart, 0.5f), new Vector2(buttonWidth, 180));
            
            // Guild button
            GameObject guildBtn = CreateFooterButton(footer.transform, "GuildButton", "GUILD",
                new Vector2(xStart + xStep, 0.5f), new Vector2(buttonWidth, 180));
            
            // Battle button (center, highlighted)
            GameObject battleBtn = CreateFooterButton(footer.transform, "BattleButton", "BATTLE",
                new Vector2(xStart + xStep * 2, 0.5f), new Vector2(buttonWidth, 180));
            battleBtn.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f, 1f);
            
            // Minigame button
            GameObject minigameBtn = CreateFooterButton(footer.transform, "MinigameButton", "MINIGAME",
                new Vector2(xStart + xStep * 3, 0.5f), new Vector2(buttonWidth, 180));
            
            // Shop button
            GameObject shopBtn = CreateFooterButton(footer.transform, "ShopButton", "SHOP",
                new Vector2(xStart + xStep * 4, 0.5f), new Vector2(buttonWidth, 180));
            
            // Link buttons to navigation controller
            navController.SetupNavigation(
                playerBtn.GetComponent<Button>(),
                guildBtn.GetComponent<Button>(),
                battleBtn.GetComponent<Button>(),
                minigameBtn.GetComponent<Button>(),
                shopBtn.GetComponent<Button>()
            );
        }
        
        private GameObject CreateFooterButton(Transform parent, string name, string text,
            Vector2 anchorPos, Vector2 size)
        {
            GameObject button = CreateButton(parent, name, text, anchorPos, size);
            
            // Style the button
            Image img = button.GetComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            // Add icon placeholder
            GameObject icon = CreatePanel(button.transform, "Icon",
                new Vector2(0.5f, 0.65f), new Vector2(60, 60));
            icon.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            
            // Adjust text position
            TextMeshProUGUI btnText = button.GetComponentInChildren<TextMeshProUGUI>();
            btnText.rectTransform.anchorMin = new Vector2(0.5f, 0.2f);
            btnText.rectTransform.anchorMax = new Vector2(0.5f, 0.2f);
            btnText.fontSize = 14;
            
            return button;
        }
        
        private void CreatePlayerPanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "PlayerPanel",
                new Vector2(0.5f, 0.5f), new Vector2(1000, 1800));
            panel.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            panel.SetActive(false);
            
            // Title
            CreateText(panel.transform, "Title", "PLAYER",
                new Vector2(0.5f, 0.95f), new Vector2(800, 100), 48);
            
            // Player info section
            GameObject infoSection = CreatePanel(panel.transform, "InfoSection",
                new Vector2(0.5f, 0.8f), new Vector2(900, 300));
            
            CreateText(infoSection.transform, "NameLabel", "Name: Player123",
                new Vector2(0.5f, 0.8f), new Vector2(800, 50), 24);
            
            CreateText(infoSection.transform, "LevelLabel", "Level: 15",
                new Vector2(0.5f, 0.6f), new Vector2(800, 50), 24);
            
            CreateText(infoSection.transform, "WinsLabel", "Total Wins: 42",
                new Vector2(0.5f, 0.4f), new Vector2(800, 50), 24);
            
            // Equipment section
            CreateText(panel.transform, "EquipTitle", "EQUIPMENT",
                new Vector2(0.5f, 0.5f), new Vector2(800, 60), 32);
            
            GameObject equipGrid = CreatePanel(panel.transform, "EquipGrid",
                new Vector2(0.5f, 0.3f), new Vector2(900, 400));
            
            // Equipment slots
            for (int i = 0; i < 6; i++)
            {
                float x = 0.2f + (i % 3) * 0.3f;
                float y = 0.7f - (i / 3) * 0.5f;
                GameObject slot = CreatePanel(equipGrid.transform, $"EquipSlot{i}",
                    new Vector2(x, y), new Vector2(120, 120));
                slot.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
            }
        }
        
        private void CreateGuildPanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "GuildPanel",
                new Vector2(0.5f, 0.5f), new Vector2(1000, 1800));
            panel.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            panel.SetActive(false);
            
            CreateText(panel.transform, "Title", "GUILD",
                new Vector2(0.5f, 0.95f), new Vector2(800, 100), 48);
            
            CreateText(panel.transform, "GuildName", "No Guild",
                new Vector2(0.5f, 0.8f), new Vector2(800, 60), 32);
            
            CreateButton(panel.transform, "JoinButton", "JOIN GUILD",
                new Vector2(0.5f, 0.5f), new Vector2(300, 100));
        }
        
        private void CreateBattlePanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "BattlePanel",
                new Vector2(0.5f, 0.5f), new Vector2(1000, 1800));
            panel.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            panel.SetActive(true); // Battle panel active by default
            
            CreateText(panel.transform, "Title", "BATTLE",
                new Vector2(0.5f, 0.95f), new Vector2(800, 100), 48);
            
            // Battle mode buttons
            GameObject playBtn = CreateButton(panel.transform, "PlayJigupaButton", "PLAY JIGUPA",
                new Vector2(0.5f, 0.7f), new Vector2(600, 150));
            playBtn.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f, 1f);
            
            // Add the PlayJigupaButton component for more reliable click handling
            playBtn.AddComponent<PlayJigupaButton>();
            Debug.Log("PLAY JIGUPA button created with PlayJigupaButton component");
            
            CreateButton(panel.transform, "RankedButton", "RANKED MATCH",
                new Vector2(0.5f, 0.5f), new Vector2(400, 100));
            
            CreateButton(panel.transform, "PracticeButton", "PRACTICE",
                new Vector2(0.5f, 0.3f), new Vector2(400, 100));
        }
        
        private void CreateMinigamePanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "MinigamePanel",
                new Vector2(0.5f, 0.5f), new Vector2(1000, 1800));
            panel.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            panel.SetActive(false);
            
            CreateText(panel.transform, "Title", "MINIGAMES",
                new Vector2(0.5f, 0.95f), new Vector2(800, 100), 48);
            
            CreateText(panel.transform, "ComingSoon", "Coming Soon!",
                new Vector2(0.5f, 0.5f), new Vector2(800, 100), 32);
        }
        
        private void CreateShopPanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "ShopPanel",
                new Vector2(0.5f, 0.5f), new Vector2(1000, 1800));
            panel.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            panel.SetActive(false);
            
            CreateText(panel.transform, "Title", "SHOP",
                new Vector2(0.5f, 0.95f), new Vector2(800, 100), 48);
            
            // Shop items
            for (int i = 0; i < 4; i++)
            {
                float y = 0.7f - i * 0.2f;
                GameObject item = CreatePanel(panel.transform, $"ShopItem{i}",
                    new Vector2(0.5f, y), new Vector2(800, 120));
                
                CreateText(item.transform, "ItemName", $"Item Pack {i + 1}",
                    new Vector2(0.3f, 0.5f), new Vector2(300, 80), 20);
                
                GameObject buyBtn = CreateButton(item.transform, "BuyButton", $"${(i + 1) * 0.99f:F2}",
                    new Vector2(0.8f, 0.5f), new Vector2(150, 80));
                buyBtn.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.2f, 1f);
            }
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
            
            var btnText = CreateText(button.transform, "Text", text,
                new Vector2(0.5f, 0.5f), size * 0.9f, 16);
            btnText.color = Color.white;
            
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
            tmp.color = Color.white;
            
            return tmp;
        }
        
        private void LaunchBattle()
        {
            // Option 1: Hide main menu and setup game in same scene
            GameObject mainMenu = GameObject.Find("MainMenuCanvas");
            if (mainMenu) mainMenu.SetActive(false);
            
            // Clean up any existing game UI first
            var existingCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var canvas in existingCanvases)
            {
                if (canvas.name == "Canvas") // Game canvas
                {
                    DestroyImmediate(canvas.gameObject);
                }
            }
            
            // Clean up existing game managers
            var existingManagers = FindObjectsByType<GameStateManager>(FindObjectsSortMode.None);
            foreach (var manager in existingManagers)
            {
                DestroyImmediate(manager.gameObject);
            }
            
            // Setup and start the game
            SimpleUISetup gameSetup = FindObjectOfType<SimpleUISetup>();
            if (!gameSetup)
            {
                GameObject setupGO = new GameObject("GameSetup");
                gameSetup = setupGO.AddComponent<SimpleUISetup>();
            }
            gameSetup.SetupSimpleScene();
            
            // Add return to menu functionality
            AddReturnToMenuButton();
        }
        
        private void AddReturnToMenuButton()
        {
            // Find the game canvas
            Canvas gameCanvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
            if (!gameCanvas) return;
            
            // Add a back button
            GameObject backBtn = CreateButton(gameCanvas.transform, "BackToMenuButton", "MENU",
                new Vector2(0.1f, 0.95f), new Vector2(100, 60));
            backBtn.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            
            Button backButton = backBtn.GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                // Destroy game UI
                if (gameCanvas) Destroy(gameCanvas.gameObject);
                
                // Destroy game manager
                GameStateManager gameManager = FindObjectOfType<GameStateManager>();
                if (gameManager) Destroy(gameManager.gameObject);
                
                // Show main menu again
                GameObject mainMenu = GameObject.Find("MainMenuCanvas");
                if (mainMenu) 
                {
                    mainMenu.SetActive(true);
                }
                else
                {
                    // Recreate main menu if it was destroyed
                    MainMenuUISetup menuSetup = FindObjectOfType<MainMenuUISetup>();
                    if (!menuSetup)
                    {
                        GameObject setupGO = new GameObject("MenuSetup");
                        menuSetup = setupGO.AddComponent<MainMenuUISetup>();
                    }
                    menuSetup.SetupMainMenu();
                }
            });
        }
    }
}