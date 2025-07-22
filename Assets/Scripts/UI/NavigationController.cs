using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Jigupa.UI
{
    public class NavigationController : MonoBehaviour
    {
        private Button playerButton;
        private Button guildButton;
        private Button battleButton;
        private Button minigameButton;
        private Button shopButton;
        
        private GameObject playerPanel;
        private GameObject guildPanel;
        private GameObject battlePanel;
        private GameObject minigamePanel;
        private GameObject shopPanel;
        
        private Button currentActiveButton;
        private GameObject currentActivePanel;
        
        private Color normalTextColor = new Color(0.902f, 0.224f, 0.275f, 1f); // #e63946 - Light red
        private Color activeTextColor = new Color(0.674f, 0.035f, 0.161f, 1f); // #ac0929 - Primary red
        
        // New method for 4 buttons (without minigame)
        public void SetupNavigation(Button player, Button guild, Button battle, Button shop)
        {
            SetupNavigation(player, guild, battle, null, shop);
        }
        
        public void SetupNavigation(Button player, Button guild, Button battle, Button minigame, Button shop)
        {
            playerButton = player;
            guildButton = guild;
            battleButton = battle;
            minigameButton = minigame; // Can be null
            shopButton = shop;
            
            // Find panels in the new structure
            Transform contentArea = transform.Find("ContentArea");
            if (contentArea)
            {
                Transform panelContainer = contentArea.Find("PanelContainer");
                if (panelContainer)
                {
                    playerPanel = panelContainer.Find("PlayerPanel")?.gameObject;
                    guildPanel = panelContainer.Find("GuildPanel")?.gameObject;
                    battlePanel = panelContainer.Find("BattlePanel")?.gameObject;
                    minigamePanel = panelContainer.Find("MinigamePanel")?.gameObject;
                    shopPanel = panelContainer.Find("ShopPanel")?.gameObject;
                }
            }
            
            // Setup button clicks
            playerButton.onClick.AddListener(() => OnTabSelected(0));
            guildButton.onClick.AddListener(() => OnTabSelected(1));
            battleButton.onClick.AddListener(() => OnTabSelected(2));
            if (minigameButton != null) minigameButton.onClick.AddListener(() => OnTabSelected(3));
            shopButton.onClick.AddListener(() => OnTabSelected(minigameButton != null ? 4 : 3));
            
            // Start with battle tab active
            OnTabSelected(2);
        }
        
        public void OnTabSelected(int index)
        {
            // Reset all button texts to normal color
            SetButtonTextColor(playerButton, normalTextColor);
            SetButtonTextColor(guildButton, normalTextColor);
            SetButtonTextColor(battleButton, normalTextColor);
            if (minigameButton != null) SetButtonTextColor(minigameButton, normalTextColor);
            SetButtonTextColor(shopButton, normalTextColor);
            
            // Hide all panels
            if (playerPanel) playerPanel.SetActive(false);
            if (guildPanel) guildPanel.SetActive(false);
            if (battlePanel) battlePanel.SetActive(false);
            if (minigamePanel) minigamePanel.SetActive(false);
            if (shopPanel) shopPanel.SetActive(false);
            
            // Activate selected tab
            switch (index)
            {
                case 0: // Player
                    currentActiveButton = playerButton;
                    currentActivePanel = playerPanel;
                    break;
                case 1: // Guild
                    currentActiveButton = guildButton;
                    currentActivePanel = guildPanel;
                    break;
                case 2: // Battle
                    currentActiveButton = battleButton;
                    currentActivePanel = battlePanel;
                    break;
                case 3: // Minigame or Shop
                    if (minigameButton != null)
                    {
                        currentActiveButton = minigameButton;
                        currentActivePanel = minigamePanel;
                    }
                    else
                    {
                        currentActiveButton = shopButton;
                        currentActivePanel = shopPanel;
                    }
                    break;
                case 4: // Shop
                    currentActiveButton = shopButton;
                    currentActivePanel = shopPanel;
                    break;
            }
            
            // Apply active state
            if (currentActiveButton)
            {
                SetButtonTextColor(currentActiveButton, activeTextColor);
            }
            
            if (currentActivePanel)
            {
                currentActivePanel.SetActive(true);
            }
        }
        
        private void SetButtonTextColor(Button button, Color color)
        {
            if (button != null)
            {
                TMPro.TextMeshProUGUI text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                {
                    text.color = color;
                }
            }
        }
    }
}