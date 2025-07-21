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
        
        private Color normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        private Color activeColor = new Color(0.8f, 0.2f, 0.2f, 1f);
        
        public void SetupNavigation(Button player, Button guild, Button battle, Button minigame, Button shop)
        {
            playerButton = player;
            guildButton = guild;
            battleButton = battle;
            minigameButton = minigame;
            shopButton = shop;
            
            // Find panels
            Transform contentArea = transform.Find("ContentArea");
            if (contentArea)
            {
                playerPanel = contentArea.Find("PlayerPanel")?.gameObject;
                guildPanel = contentArea.Find("GuildPanel")?.gameObject;
                battlePanel = contentArea.Find("BattlePanel")?.gameObject;
                minigamePanel = contentArea.Find("MinigamePanel")?.gameObject;
                shopPanel = contentArea.Find("ShopPanel")?.gameObject;
            }
            
            // Setup button clicks
            playerButton.onClick.AddListener(() => OnTabSelected(0));
            guildButton.onClick.AddListener(() => OnTabSelected(1));
            battleButton.onClick.AddListener(() => OnTabSelected(2));
            minigameButton.onClick.AddListener(() => OnTabSelected(3));
            shopButton.onClick.AddListener(() => OnTabSelected(4));
            
            // Start with battle tab active
            OnTabSelected(2);
        }
        
        private void OnTabSelected(int index)
        {
            // Reset all buttons to normal color
            playerButton.GetComponent<Image>().color = normalColor;
            guildButton.GetComponent<Image>().color = normalColor;
            battleButton.GetComponent<Image>().color = normalColor;
            minigameButton.GetComponent<Image>().color = normalColor;
            shopButton.GetComponent<Image>().color = normalColor;
            
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
                case 3: // Minigame
                    currentActiveButton = minigameButton;
                    currentActivePanel = minigamePanel;
                    break;
                case 4: // Shop
                    currentActiveButton = shopButton;
                    currentActivePanel = shopPanel;
                    break;
            }
            
            // Apply active state
            if (currentActiveButton)
            {
                currentActiveButton.GetComponent<Image>().color = activeColor;
            }
            
            if (currentActivePanel)
            {
                currentActivePanel.SetActive(true);
            }
        }
    }
}