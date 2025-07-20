using UnityEngine;
using UnityEngine.UI;
using Jigupa.Core;
using Jigupa.Player;
using TMPro;

namespace Jigupa.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Game State UI")]
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI roundScoreText;
        [SerializeField] private Button startButton;

        [Header("Player 1 Hand Display")]
        [SerializeField] private TextMeshProUGUI player1LeftHandText;
        [SerializeField] private TextMeshProUGUI player1RightHandText;

        [Header("Player 2 Hand Display")]
        [SerializeField] private TextMeshProUGUI player2LeftHandText;
        [SerializeField] private TextMeshProUGUI player2RightHandText;

        [Header("Attack Display")]
        [SerializeField] private GameObject attackDisplayPanel;
        [SerializeField] private TextMeshProUGUI attackLeftText;
        [SerializeField] private TextMeshProUGUI attackRightText;
        [SerializeField] private GameObject attackLeftLabel;
        [SerializeField] private GameObject attackRightLabel;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI winnerText;

        private void Start()
        {
            if (startButton) startButton.onClick.AddListener(StartGame);
            
            if (GameStateManager.Instance)
            {
                GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
                GameStateManager.Instance.OnHandsUpdated += OnHandsUpdated;
                GameStateManager.Instance.OnAttackDeclared += OnAttackDeclared;
                GameStateManager.Instance.OnAttackResult += OnAttackResult;
                GameStateManager.Instance.OnCoinFlipRequired += OnCoinFlipRequired;
            }

            if (gameOverPanel) gameOverPanel.SetActive(false);
            if (attackDisplayPanel) attackDisplayPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameStateManager.Instance)
            {
                GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
                GameStateManager.Instance.OnHandsUpdated -= OnHandsUpdated;
                GameStateManager.Instance.OnAttackDeclared -= OnAttackDeclared;
                GameStateManager.Instance.OnAttackResult -= OnAttackResult;
                GameStateManager.Instance.OnCoinFlipRequired -= OnCoinFlipRequired;
            }
        }

        private void Update()
        {
            if (GameStateManager.Instance)
            {
                UpdateTimer();
                UpdateRoundScore();
            }
        }

        private void StartGame()
        {
            if (GameStateManager.Instance)
            {
                GameStateManager.Instance.StartGame();
                if (startButton) startButton.gameObject.SetActive(false);
                if (gameOverPanel) gameOverPanel.SetActive(false);
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (stateText)
            {
                bool isPlayer1Attacking = GameStateManager.Instance.IsPlayer1Attacking();
                switch (newState)
                {
                    case GameState.WaitingToStart:
                        stateText.text = "Press Start to Begin";
                        break;
                    case GameState.CoinFlip:
                        stateText.text = "Choose Gu, Pa, or Ji! (RPS rules for coin flip)";
                        break;
                    case GameState.AttackPhase:
                        stateText.text = $"{(isPlayer1Attacking ? "Your" : "AI's")} Attack Phase!";
                        break;
                    case GameState.DefensePhase:
                        stateText.text = $"{(!isPlayer1Attacking ? "Your" : "AI's")} Defense - Choose NOW!";
                        break;
                    case GameState.ResolvingTurn:
                        stateText.text = "Resolving Turn...";
                        break;
                    case GameState.RoundOver:
                        stateText.text = "Round Over!";
                        break;
                    case GameState.GameOver:
                        ShowGameOver();
                        break;
                }
            }

            // Show attack panel during defense but hide the actual gestures
            if (newState == GameState.DefensePhase && GameStateManager.Instance.IsPlayer1Attacking())
            {
                if (attackDisplayPanel)
                {
                    attackDisplayPanel.SetActive(true);
                    if (attackLeftText) attackLeftText.text = "???";
                    if (attackRightText) attackRightText.text = "???";
                }
            }
            else if (attackDisplayPanel && newState != GameState.ResolvingTurn)
            {
                attackDisplayPanel.SetActive(false);
            }
        }

        private void OnHandsUpdated(PlayerHand player1, PlayerHand player2)
        {
            UpdateHandDisplay(player1LeftHandText, player1.hasLeftHand);
            UpdateHandDisplay(player1RightHandText, player1.hasRightHand);
            UpdateHandDisplay(player2LeftHandText, player2.hasLeftHand);
            UpdateHandDisplay(player2RightHandText, player2.hasRightHand);
        }

        private void UpdateHandDisplay(TextMeshProUGUI textElement, bool hasHand)
        {
            if (textElement == null) return;

            if (hasHand)
            {
                textElement.text = "✊ ✋ ✌️";
                textElement.color = Color.white;
            }
            else
            {
                textElement.text = "❌";
                textElement.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }

        private void OnAttackDeclared(GestureType leftGesture, GestureType rightGesture)
        {
            if (attackDisplayPanel && GameStateManager.Instance)
            {
                attackDisplayPanel.SetActive(true);
                
                // Check if it's a single hand attack
                GameStateManager.Instance.GetCurrentAttack(out var leftAttack, out var rightAttack);
                
                if (leftAttack.HasValue && rightAttack.HasValue)
                {
                    // Double hand attack - show both with labels
                    if (attackLeftText) 
                    {
                        attackLeftText.text = GetGestureName(leftGesture);
                        // Restore left position
                        var rectTransform = attackLeftText.GetComponent<RectTransform>();
                        rectTransform.anchorMin = new Vector2(0.25f, 0.2f);
                        rectTransform.anchorMax = new Vector2(0.25f, 0.2f);
                        rectTransform.anchoredPosition = Vector2.zero;
                    }
                    if (attackRightText) attackRightText.text = GetGestureName(rightGesture);
                    if (attackLeftLabel) attackLeftLabel.SetActive(true);
                    if (attackRightLabel) attackRightLabel.SetActive(true);
                }
                else if (leftAttack.HasValue || rightAttack.HasValue)
                {
                    // Single hand attack - show only one gesture, centered
                    string gesture = leftAttack.HasValue ? GetGestureName(leftGesture) : GetGestureName(rightGesture);
                    
                    // Use left text position for single gesture display
                    if (attackLeftText) 
                    {
                        attackLeftText.text = gesture;
                        // Move to center
                        var rectTransform = attackLeftText.GetComponent<RectTransform>();
                        rectTransform.anchorMin = new Vector2(0.5f, 0.2f);
                        rectTransform.anchorMax = new Vector2(0.5f, 0.2f);
                        rectTransform.anchoredPosition = Vector2.zero;
                    }
                    if (attackRightText) attackRightText.text = "";
                    
                    // Hide labels for single attack
                    if (attackLeftLabel) attackLeftLabel.SetActive(false);
                    if (attackRightLabel) attackRightLabel.SetActive(false);
                }
            }
        }

        private void OnAttackResult(int successfulHits)
        {
            if (stateText)
            {
                string resultText = "";
                
                // Show what happened
                if (GameStateManager.Instance)
                {
                    bool isPlayer1Attacking = GameStateManager.Instance.IsPlayer1Attacking();
                    
                    // Get attack and defense info
                    GameStateManager.Instance.GetCurrentAttack(out var leftAttack, out var rightAttack);
                    int defenderHandCount = GameStateManager.Instance.GetDefenderHandCountBeforeAttack();
                    
                    if (isPlayer1Attacking)
                    {
                        // Player attacked, show AI's defense
                        resultText = "AI defended with: ";
                        
                        // Check AI's hand count at time of defense (before resolution)
                        if (defenderHandCount == 2)
                        {
                            resultText += $"{GameStateManager.Instance.GetLastDefenseLeft()} + {GameStateManager.Instance.GetLastDefenseRight()}";
                        }
                        else if (defenderHandCount == 1)
                        {
                            // Single hand defense - show only the gesture used
                            resultText += $"{GameStateManager.Instance.GetLastDefenseGesture()}";
                        }
                        resultText += "\n";
                    }
                    else
                    {
                        // AI attacked, show what they attacked with
                        resultText = "AI attacked with: ";
                        if (leftAttack.HasValue && rightAttack.HasValue)
                        {
                            resultText += $"{leftAttack.Value} + {rightAttack.Value}";
                        }
                        else if (leftAttack.HasValue)
                        {
                            resultText += $"{leftAttack.Value}";
                        }
                        else if (rightAttack.HasValue)
                        {
                            resultText += $"{rightAttack.Value}";
                        }
                        resultText += "\n";
                    }
                }
                
                // Add result
                if (successfulHits == 0)
                    resultText += "Defense successful! No hits!";
                else if (successfulHits == 1)
                    resultText += "1 hand eliminated!";
                else
                    resultText += "Both hands eliminated!";
                    
                stateText.text = resultText;
            }
        }

        private void OnCoinFlipRequired()
        {
            // This will be handled by a separate coin flip UI
        }

        private void UpdateTimer()
        {
            if (timerText)
            {
                var state = GameStateManager.Instance.GetCurrentState();
                if (state == GameState.AttackPhase || state == GameState.DefensePhase)
                {
                    float timer = GameStateManager.Instance.GetStateTimer();
                    timerText.text = $"Time: {timer:F1}s";
                }
                else
                {
                    timerText.text = "";
                }
            }
        }

        private void UpdateRoundScore()
        {
            if (roundScoreText)
            {
                int p1Wins = GameStateManager.Instance.GetPlayer1Wins();
                int p2Wins = GameStateManager.Instance.GetPlayer2Wins();
                roundScoreText.text = $"You {p1Wins} • AI {p2Wins}";
            }
        }

        private void ShowGameOver()
        {
            if (gameOverPanel)
            {
                gameOverPanel.SetActive(true);
                
                int p1Wins = GameStateManager.Instance.GetPlayer1Wins();
                int p2Wins = GameStateManager.Instance.GetPlayer2Wins();
                
                if (winnerText)
                {
                    winnerText.text = p1Wins > p2Wins ? "YOU WIN!" : "AI WINS!";
                }
                
                if (startButton) startButton.gameObject.SetActive(true);
            }
        }

        private string GetGestureName(GestureType gesture)
        {
            switch (gesture)
            {
                case GestureType.Gu: return "Gu (咕)";
                case GestureType.Pa: return "Pa (帕)";
                case GestureType.Ji: return "Ji (嘰)";
                default: return gesture.ToString();
            }
        }

        private string GetGestureSymbol(GestureType gesture)
        {
            switch (gesture)
            {
                case GestureType.Gu: return "✊";
                case GestureType.Pa: return "✋";
                case GestureType.Ji: return "✌️";
                default: return "?";
            }
        }
    }
}