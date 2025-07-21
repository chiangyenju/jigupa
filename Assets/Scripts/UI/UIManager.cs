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

            // Don't use attack panel anymore - gestures are shown on the board
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
                // Show current stance instead of "READY"
                if (GameStateManager.Instance != null)
                {
                    GestureType stance = GestureType.Gu; // Default
                    
                    // Determine which hand and player we're updating
                    if (textElement == player1LeftHandText)
                        stance = GameStateManager.Instance.GetPlayer1LeftStance();
                    else if (textElement == player1RightHandText)
                        stance = GameStateManager.Instance.GetPlayer1RightStance();
                    else if (textElement == player2LeftHandText)
                        stance = GameStateManager.Instance.GetPlayer2LeftStance();
                    else if (textElement == player2RightHandText)
                        stance = GameStateManager.Instance.GetPlayer2RightStance();
                    
                    textElement.text = GetGestureSymbol(stance);
                    textElement.color = Color.white;
                }
                else
                {
                    textElement.text = "R"; // Default to Rock
                    textElement.color = Color.white;
                }
            }
            else
            {
                textElement.text = "OUT";
                textElement.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }

        private void OnAttackDeclared(GestureType leftGesture, GestureType rightGesture)
        {
            if (GameStateManager.Instance)
            {
                // Instead of showing attack overlay, update the hand displays to show the actual gestures
                bool isPlayer1Attacking = GameStateManager.Instance.IsPlayer1Attacking();
                GameStateManager.Instance.GetCurrentAttack(out var leftAttack, out var rightAttack);
                
                if (isPlayer1Attacking)
                {
                    // Player 1 is attacking - show their gestures with sword
                    if (player1LeftHandText && leftAttack.HasValue)
                        player1LeftHandText.text = GetGestureSymbol(leftGesture) + " ←";
                    if (player1RightHandText && rightAttack.HasValue)
                        player1RightHandText.text = GetGestureSymbol(rightGesture) + " ←";
                    
                    // Player 2 is defending - show shield with current stance
                    if (player2LeftHandText && GameStateManager.Instance.GetPlayer2().hasLeftHand)
                        player2LeftHandText.text = "[" + GetGestureSymbol(GameStateManager.Instance.GetPlayer2LeftStance()) + "]";
                    if (player2RightHandText && GameStateManager.Instance.GetPlayer2().hasRightHand)
                        player2RightHandText.text = "[" + GetGestureSymbol(GameStateManager.Instance.GetPlayer2RightStance()) + "]";
                }
                else
                {
                    // Player 2 is attacking - show their gestures with sword
                    if (player2LeftHandText && leftAttack.HasValue)
                        player2LeftHandText.text = GetGestureSymbol(leftGesture) + " ←";
                    if (player2RightHandText && rightAttack.HasValue)
                        player2RightHandText.text = GetGestureSymbol(rightGesture) + " ←";
                    
                    // Player 1 is defending - show shield with current stance
                    if (player1LeftHandText && GameStateManager.Instance.GetPlayer1().hasLeftHand)
                        player1LeftHandText.text = "[" + GetGestureSymbol(GameStateManager.Instance.GetPlayer1LeftStance()) + "]";
                    if (player1RightHandText && GameStateManager.Instance.GetPlayer1().hasRightHand)
                        player1RightHandText.text = "[" + GetGestureSymbol(GameStateManager.Instance.GetPlayer1RightStance()) + "]";
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
                        
                        // Show defense results on the board
                        var leftDef = GameStateManager.Instance.GetLastDefenseLeft();
                        var rightDef = GameStateManager.Instance.GetLastDefenseRight();
                        
                        if (leftAttack.HasValue && player2LeftHandText && player2LeftHandText.text.Contains("["))
                        {
                            // Just show the defense gesture - we'll determine success from elimination
                            player2LeftHandText.text = GetGestureSymbol(leftDef);
                            player2LeftHandText.color = Color.white;
                        }
                        if (rightAttack.HasValue && player2RightHandText && player2RightHandText.text.Contains("["))
                        {
                            // Just show the defense gesture - we'll determine success from elimination
                            player2RightHandText.text = GetGestureSymbol(rightDef);
                            player2RightHandText.color = Color.white;
                        }
                    }
                    else
                    {
                        // AI attacked, show player's defense results
                        var leftDef = GameStateManager.Instance.GetLastDefenseLeft();
                        var rightDef = GameStateManager.Instance.GetLastDefenseRight();
                        
                        if (leftAttack.HasValue && player1LeftHandText && player1LeftHandText.text.Contains("["))
                        {
                            // Just show the defense gesture - we'll determine success from elimination
                            player1LeftHandText.text = GetGestureSymbol(leftDef);
                            player1LeftHandText.color = Color.white;
                        }
                        if (rightAttack.HasValue && player1RightHandText && player1RightHandText.text.Contains("["))
                        {
                            // Just show the defense gesture - we'll determine success from elimination
                            player1RightHandText.text = GetGestureSymbol(rightDef);
                            player1RightHandText.color = Color.white;
                        }
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
                roundScoreText.text = $"YOU: {p1Wins}  VS  AI: {p2Wins}";
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
                case GestureType.Gu: return "ROCK";
                case GestureType.Pa: return "PAPER";
                case GestureType.Ji: return "SCISSORS";
                default: return gesture.ToString();
            }
        }

        private string GetGestureSymbol(GestureType gesture)
        {
            switch (gesture)
            {
                case GestureType.Gu: return "R";  // Rock
                case GestureType.Pa: return "P";  // Paper
                case GestureType.Ji: return "S";  // Scissors
                default: return "?";
            }
        }
    }
}