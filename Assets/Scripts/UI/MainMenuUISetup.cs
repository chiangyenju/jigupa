using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Jigupa.Core;
using System.Collections;
using System.Linq;

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
            
            // Create main container with text menu
            CreateMainContainer(canvas);
            
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
            
            // Clean up any objects with missing scripts
            CleanupMissingScripts();
        }
        
        private void CleanupMissingScripts()
        {
            // Find all GameObjects in the scene
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (GameObject obj in allObjects)
            {
                // Check for missing scripts and remove them
                Component[] components = obj.GetComponents<Component>();
                foreach (Component comp in components)
                {
                    if (comp == null)
                    {
                        Debug.Log($"Removing missing script from GameObject: {obj.name}");
                        DestroyImmediate(comp);
                    }
                }
                
                // Specifically look for glare-related objects and remove them
                if (obj.name == "TextGlare" || obj.name == "GlareContainer" || obj.name == "GlareMask" || obj.name == "SharedGlareContainer")
                {
                    Debug.Log($"Removing old glare object: {obj.name}");
                    DestroyImmediate(obj);
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
            var existingEventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
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
            // Background - Ivory Cream
            GameObject bg = CreatePanel(canvas.transform, "Background",
                new Vector2(0.5f, 0.5f), new Vector2(1170, 2532));
            bg.GetComponent<Image>().color = new Color(0.961f, 0.949f, 0.91f, 1f); // #F5F2E8
            bg.transform.SetAsFirstSibling();
            
            // Main content area
            GameObject contentArea = CreatePanel(canvas.transform, "ContentArea",
                new Vector2(0.5f, 0.5f), new Vector2(1170, 2532));
            contentArea.GetComponent<Image>().color = new Color(0, 0, 0, 0); // Transparent
            
            // Create menu title
            TextMeshProUGUI titleText = CreateText(contentArea.transform, "GameTitle", "JIGUPA",
                new Vector2(0.5f, 0.85f), new Vector2(800, 200), 120);
            titleText.color = new Color(0.674f, 0.035f, 0.161f, 1f); // Primary red
            titleText.fontStyle = FontStyles.Bold;
            titleText.characterSpacing = -10; // Tight spacing for title
            FontManager.ApplyLexendFont(titleText, TMPro.FontWeight.Black);
            
            // Create text-based navigation menu at bottom
            CreateTextMenu(contentArea.transform);
            
            // Create content panels for each tab - no background
            GameObject panelContainer = CreatePanel(contentArea.transform, "PanelContainer",
                new Vector2(0.5f, 0.45f), new Vector2(1000, 1200));
            panelContainer.GetComponent<Image>().color = new Color(0, 0, 0, 0); // Transparent
            
            CreatePlayerPanel(panelContainer.transform);
            CreateGuildPanel(panelContainer.transform);
            CreateBattlePanel(panelContainer.transform);
            CreateShopPanel(panelContainer.transform);
        }
        
        private void CreateTextMenu(Transform parent)
        {
            // Create vertical menu container at bottom right
            GameObject menuContainer = CreatePanel(parent, "MenuContainer",
                new Vector2(1f, 0f), new Vector2(300, 400));
            menuContainer.GetComponent<Image>().color = new Color(0, 0, 0, 0); // Transparent
            
            // Adjust the anchor to bottom-right corner
            RectTransform menuRect = menuContainer.GetComponent<RectTransform>();
            menuRect.anchorMin = new Vector2(1f, 0f);
            menuRect.anchorMax = new Vector2(1f, 0f);
            menuRect.pivot = new Vector2(1f, 0f);
            menuRect.anchoredPosition = new Vector2(-50, 100); // 50 pixels from right, 100 from bottom
            
            // We'll handle masking differently to avoid hiding the menu
            
            // We'll create the glare after menu items are set up
            
            // Add navigation controller
            NavigationController navController = parent.GetComponentInParent<Canvas>().gameObject.AddComponent<NavigationController>();
            
            // Menu items - new order: Battle, Player, Guild, Shop (top to bottom)
            string[] menuItems = { "BATTLE", "PLAYER", "GUILD", "SHOP" };
            float spacing = 80f; // Vertical spacing
            float startY = 150f; // Start from top
            
            Button[] buttons = new Button[menuItems.Length];
            
            for (int i = 0; i < menuItems.Length; i++)
            {
                GameObject menuItem = new GameObject(menuItems[i] + "MenuItem");
                menuItem.transform.SetParent(menuContainer.transform, false);
                
                RectTransform rect = menuItem.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(1f, 0.5f); // Right aligned
                rect.anchorMax = new Vector2(1f, 0.5f);
                rect.pivot = new Vector2(1f, 0.5f); // Right pivot
                rect.anchoredPosition = new Vector2(-20, startY - i * spacing); // 20 pixels from right edge
                rect.sizeDelta = new Vector2(250, 80); // Size for text
                
                // Add button component
                Button btn = menuItem.AddComponent<Button>();
                buttons[i] = btn;
                
                // Add text with Lexend 900 weight
                TextMeshProUGUI text = menuItem.AddComponent<TextMeshProUGUI>();
                text.text = menuItems[i];
                text.fontSize = 60; // Size 60 as requested
                text.alignment = TextAlignmentOptions.Right;
                text.fontWeight = TMPro.FontWeight.Black; // 900 weight
                text.characterSpacing = -15; // Much tighter spacing for bold text
                
                // Apply Lexend font if available
                FontManager.ApplyLexendFont(text, TMPro.FontWeight.Black);
                
                // Set initial color - Battle (index 0) is selected by default
                if (i == 0) // Battle is now at index 0
                {
                    text.color = new Color(0.674f, 0.035f, 0.161f, 1f); // #ac0929 - Primary red
                }
                else
                {
                    text.color = new Color(0.902f, 0.224f, 0.275f, 1f); // #e63946 - Light red
                }
                
                
                // Store text component reference for navigation controller
                int index = i;
                btn.onClick.AddListener(() => {
                    UpdateMenuColors(menuContainer, index);
                });
            }
            
            // Setup navigation controller with new order - Battle, Player, Guild, Shop
            navController.SetupNavigation(buttons[1], buttons[2], buttons[0], buttons[3]);
        }
        
        private void UpdateMenuColors(GameObject menuContainer, int selectedIndex)
        {
            TextMeshProUGUI[] texts = menuContainer.GetComponentsInChildren<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++)
            {
                if (i == selectedIndex)
                {
                    texts[i].color = new Color(0.674f, 0.035f, 0.161f, 1f); // Primary red
                }
                else
                {
                    texts[i].color = new Color(0.902f, 0.224f, 0.275f, 1f); // Light red
                }
            }
        }
        
        
        private void CreatePlayerPanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "PlayerPanel",
                new Vector2(0.5f, 0.5f), new Vector2(900, 1000));
            panel.GetComponent<Image>().color = new Color(0, 0, 0, 0); // Transparent
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
                new Vector2(0.5f, 0.5f), new Vector2(900, 1000));
            panel.GetComponent<Image>().color = new Color(0, 0, 0, 0); // Transparent
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
                new Vector2(0.5f, 0.5f), new Vector2(900, 1000));
            panel.GetComponent<Image>().color = new Color(0, 0, 0, 0); // Transparent
            panel.SetActive(true); // Battle panel active by default
            
            // Battle mode buttons - Use PrimaryButton prefab
            GameObject primaryButtonPrefab = PrefabLoader.LoadPrimaryButtonPrefab();
            Debug.Log($"PrimaryButton prefab loaded: {primaryButtonPrefab != null}");
            
            if (primaryButtonPrefab != null)
            {
                GameObject playBtn = GameObject.Instantiate(primaryButtonPrefab, panel.transform);
                playBtn.name = "PlayJigupaButton";
                
                // Position and size the button - centered in panel
                RectTransform rect = playBtn.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(400, 120); // Larger button
                
                // Check components
                PrimaryButton primaryBtn = playBtn.GetComponent<PrimaryButton>();
                Image buttonImage = playBtn.GetComponent<Image>();
                Debug.Log($"PrimaryButton component: {primaryBtn != null}, Image color: {buttonImage?.color}");
                
                // Set text using PrimaryButton component
                if (primaryBtn != null)
                {
                    primaryBtn.SetText("BATTLE");
                    primaryBtn.DisableBorder();
                    Debug.Log("Set BATTLE text using PrimaryButton component");
                }
                else
                {
                    // Fallback: directly set text
                    TextMeshProUGUI immediateText = playBtn.GetComponentInChildren<TextMeshProUGUI>();
                    if (immediateText != null)
                    {
                        immediateText.text = "BATTLE";
                        Debug.Log($"Fallback: set text to: '{immediateText.text}'");
                    }
                }
                
                // Only use coroutine if in play mode
                if (Application.isPlaying)
                {
                    StartCoroutine(SetButtonTextDelayed(playBtn, "BATTLE"));
                }
                
                // Check if PlayJigupaButton already exists (from prefab)
                PlayJigupaButton existingPlayButton = playBtn.GetComponent<PlayJigupaButton>();
                if (existingPlayButton == null)
                {
                    // Add the PlayJigupaButton component for scene loading
                    playBtn.AddComponent<PlayJigupaButton>();
                    Debug.Log("Added PlayJigupaButton component");
                }
                else
                {
                    Debug.Log("PlayJigupaButton component already exists on prefab");
                }
                
                // Add component to manage fist icon
                playBtn.AddComponent<BattleButtonFist>();
                
                // Create fist icon immediately in edit mode
                CreateFistIconForButton(playBtn, panel.transform);
                
                Debug.Log("BATTLE button created with PrimaryButton prefab");
            }
            else
            {
                // Fallback to old method if prefab not found
                GameObject playBtn = CreateButton(panel.transform, "PlayJigupaButton", "BATTLE",
                    new Vector2(0.5f, 0.5f), new Vector2(400, 120));
                Image btnImage = playBtn.GetComponent<Image>();
                btnImage.color = new Color(0.674f, 0.035f, 0.161f, 1f); // #ac0929
                
                // Style the button text
                TextMeshProUGUI btnText = playBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.fontSize = 48;
                    btnText.fontWeight = TMPro.FontWeight.Black;
                    btnText.characterSpacing = -10;
                    FontManager.ApplyLexendFont(btnText, TMPro.FontWeight.Black);
                }
                
                playBtn.AddComponent<PlayJigupaButton>();
                playBtn.AddComponent<BattleButtonFist>();
                
                // Create fist icon immediately in edit mode
                CreateFistIconForButton(playBtn, panel.transform);
                
                Debug.LogWarning("PrimaryButton prefab not found, using fallback button creation");
            }
        }
        
        private void CreateFistIconForButton(GameObject button, Transform parent)
        {
            // Create fist icon as sibling of button
            GameObject fistIcon = new GameObject("FistIcon");
            
            RectTransform fistRect = fistIcon.AddComponent<RectTransform>();
            fistIcon.transform.SetParent(parent, false);
            
            // Set transform properties AFTER parenting
            fistRect.anchorMin = new Vector2(0.5f, 0.5f);
            fistRect.anchorMax = new Vector2(0.5f, 0.5f);
            fistRect.sizeDelta = new Vector2(80, 80);
            fistRect.anchoredPosition = new Vector2(0, 120);
            fistRect.localScale = Vector3.one;
            
            Image fistImage = fistIcon.AddComponent<Image>();
            
            // Load the fist icon
            Sprite fistSprite = Resources.Load<Sprite>("Icons/fist");
            if (fistSprite != null)
            {
                fistImage.sprite = fistSprite;
                fistImage.preserveAspect = true;
                Debug.Log("Fist icon created with sprite in edit mode");
            }
            else
            {
                // Try loading as Texture2D
                Texture2D fistTexture = Resources.Load<Texture2D>("Icons/fist");
                if (fistTexture != null)
                {
                    fistSprite = Sprite.Create(fistTexture, 
                        new Rect(0, 0, fistTexture.width, fistTexture.height), 
                        new Vector2(0.5f, 0.5f));
                    fistImage.sprite = fistSprite;
                    fistImage.preserveAspect = true;
                    Debug.Log("Fist icon created with texture->sprite in edit mode");
                }
                else
                {
                    fistImage.color = Color.red;
                    Debug.LogError("FIST ICON NOT FOUND IN RESOURCES!");
                }
            }
            
            // Add animation component
            fistIcon.AddComponent<BattleFistIcon>();
            
            // Add debug component to track lifecycle
            fistIcon.AddComponent<FistIconDebug>();
            
            Debug.Log($"Fist icon created at position {fistRect.anchoredPosition} with size {fistRect.sizeDelta}");
        }
        
        private void CreateShopPanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "ShopPanel",
                new Vector2(0.5f, 0.5f), new Vector2(900, 1000));
            panel.GetComponent<Image>().color = new Color(0, 0, 0, 0); // Transparent
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
            SimpleUISetup gameSetup = FindFirstObjectByType<SimpleUISetup>();
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
                GameStateManager gameManager = FindFirstObjectByType<GameStateManager>();
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
                    MainMenuUISetup menuSetup = FindFirstObjectByType<MainMenuUISetup>();
                    if (!menuSetup)
                    {
                        GameObject setupGO = new GameObject("MenuSetup");
                        menuSetup = setupGO.AddComponent<MainMenuUISetup>();
                    }
                    menuSetup.SetupMainMenu();
                }
            });
        }
        
        private System.Collections.IEnumerator SetButtonTextDelayed(GameObject button, string text)
        {
            Debug.Log($"SetButtonTextDelayed started for text: '{text}'");
            
            // Wait one frame for components to initialize
            yield return null;
            
            // Log all components
            Debug.Log($"Button components: {string.Join(", ", button.GetComponents<Component>().Select(c => c.GetType().Name))}");
            
            // Try PrimaryButton component first
            PrimaryButton primaryBtn = button.GetComponent<PrimaryButton>();
            if (primaryBtn != null)
            {
                Debug.Log($"Found PrimaryButton component, calling SetText('{text}')");
                primaryBtn.SetText(text);
                
                // Double check the text was set
                yield return null;
                TextMeshProUGUI checkText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (checkText != null)
                {
                    Debug.Log($"Text after SetText: '{checkText.text}'");
                }
            }
            else
            {
                Debug.LogWarning("PrimaryButton component not found!");
                // Fallback: directly set TextMeshProUGUI
                TextMeshProUGUI[] textComponents = button.GetComponentsInChildren<TextMeshProUGUI>();
                Debug.Log($"Found {textComponents.Length} TextMeshProUGUI components");
                if (textComponents.Length > 0)
                {
                    textComponents[0].text = text;
                    Debug.Log($"Set button text via TextMeshProUGUI: '{text}' - Text is now: '{textComponents[0].text}'");
                }
                else
                {
                    Debug.LogWarning("Could not find text component to set button text");
                }
            }
        }
        
    }
    
}