using UnityEngine;
using UnityEngine.UI;
using Jigupa.Core;

namespace Jigupa.Core
{
    public class CoinFlipManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject coinFlipPanel;
        [SerializeField] private Button rockButton;
        [SerializeField] private Button paperButton;
        [SerializeField] private Button scissorsButton;
        [SerializeField] private TMPro.TextMeshProUGUI resultText;

        private GestureType? playerChoice;
        private bool isWaitingForAI = false;

        private void Start()
        {
            if (rockButton) rockButton.onClick.AddListener(() => SelectGesture(GestureType.Gu));
            if (paperButton) paperButton.onClick.AddListener(() => SelectGesture(GestureType.Pa));
            if (scissorsButton) scissorsButton.onClick.AddListener(() => SelectGesture(GestureType.Ji));

            if (GameStateManager.Instance)
            {
                GameStateManager.Instance.OnCoinFlipRequired += ShowCoinFlip;
                GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
            }

            if (coinFlipPanel) coinFlipPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameStateManager.Instance)
            {
                GameStateManager.Instance.OnCoinFlipRequired -= ShowCoinFlip;
                GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            // Hide the coin flip panel when we move to any state other than CoinFlip
            if (newState != GameState.CoinFlip && coinFlipPanel && coinFlipPanel.activeSelf)
            {
                coinFlipPanel.SetActive(false);
                CancelInvoke(); // Cancel any pending invokes
            }
        }

        private void ShowCoinFlip()
        {
            if (coinFlipPanel) coinFlipPanel.SetActive(true);
            if (resultText) resultText.text = "Choose to see who goes first!";
            playerChoice = null;
            isWaitingForAI = false;
            SetButtonsInteractable(true);
        }

        private void SelectGesture(GestureType gesture)
        {
            if (isWaitingForAI) return;

            playerChoice = gesture;
            isWaitingForAI = true;
            SetButtonsInteractable(false);

            if (resultText) resultText.text = $"You chose {gesture}...";

            // AI makes choice after a delay
            Invoke(nameof(ResolveCoinFlip), 1.5f);
        }

        private void ResolveCoinFlip()
        {
            GestureType aiChoice = (GestureType)Random.Range(0, 3);
            bool playerWins = CheckWinner(playerChoice.Value, aiChoice);

            if (resultText)
            {
                resultText.text = $"You: {playerChoice}\nAI: {aiChoice}\n";
                
                if (playerChoice.Value == aiChoice)
                {
                    resultText.text += "Draw! Choose again...";
                    Invoke(nameof(ResetCoinFlip), 2f);
                    return;
                }
                
                resultText.text += playerWins ? "You go first!" : "AI goes first!";
            }

            if (GameStateManager.Instance)
            {
                GameStateManager.Instance.SetFirstAttacker(playerWins);
            }

            Invoke(nameof(HideCoinFlip), 2f);
        }

        private bool CheckWinner(GestureType player, GestureType ai)
        {
            if (player == ai) return false;

            // For coin flip, we still use traditional RPS rules
            // Gu (Rock) beats Ji (Scissors)
            // Pa (Paper) beats Gu (Rock)  
            // Ji (Scissors) beats Pa (Paper)
            return (player == GestureType.Gu && ai == GestureType.Ji) ||
                   (player == GestureType.Pa && ai == GestureType.Gu) ||
                   (player == GestureType.Ji && ai == GestureType.Pa);
        }

        private void ResetCoinFlip()
        {
            if (resultText) resultText.text = "Choose again!";
            playerChoice = null;
            isWaitingForAI = false;
            SetButtonsInteractable(true);
        }

        private void HideCoinFlip()
        {
            if (coinFlipPanel) coinFlipPanel.SetActive(false);
        }

        private void SetButtonsInteractable(bool interactable)
        {
            if (rockButton) rockButton.interactable = interactable;
            if (paperButton) paperButton.interactable = interactable;
            if (scissorsButton) scissorsButton.interactable = interactable;
        }
    }
}