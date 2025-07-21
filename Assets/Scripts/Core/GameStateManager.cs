using UnityEngine;
using System;
using Jigupa.Player;

namespace Jigupa.Core
{
    public enum GameState
    {
        WaitingToStart,
        CoinFlip,
        AttackPhase,
        DefensePhase,
        ResolvingTurn,
        RoundOver,
        GameOver
    }

    public enum GestureType
    {
        Gu,    // Rock (咕)
        Pa,    // Paper (帕)
        Ji     // Scissors (嘰)
    }

    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.WaitingToStart;
        [SerializeField] private float turnTimer = 10f;
        [SerializeField] private bool isPlayer1Attacking = true;

        [Header("Players")]
        [SerializeField] private PlayerHand player1;
        [SerializeField] private PlayerHand player2;
        [SerializeField] private int player1RoundWins = 0;
        [SerializeField] private int player2RoundWins = 0;

        [Header("Current Attack")]
        [SerializeField] private GestureType? attackLeftGesture;
        [SerializeField] private GestureType? attackRightGesture;
        [SerializeField] private GestureType defenseLeftGesture;
        [SerializeField] private GestureType defenseRightGesture;
        [SerializeField] private bool defenseSubmitted = false;
        [SerializeField] private int successfulHits = 0;
        [SerializeField] private bool isSingleHandAttack = false;
        [SerializeField] private int defenderHandCountBeforeAttack = 2;
        
        [Header("Current Stance")]
        [SerializeField] private GestureType player1LeftStance = GestureType.Gu;
        [SerializeField] private GestureType player1RightStance = GestureType.Gu;
        [SerializeField] private GestureType player2LeftStance = GestureType.Gu;
        [SerializeField] private GestureType player2RightStance = GestureType.Gu;

        [Header("AI Settings")]
        [SerializeField] private bool isVsAI = true;
        [SerializeField] private float aiThinkTime = 1.5f;

        public event Action<GameState> OnStateChanged;
        public event Action<PlayerHand, PlayerHand> OnHandsUpdated;
        public event Action<GestureType, GestureType> OnAttackDeclared;
        public event Action<int> OnAttackResult; // number of successful hits
        public event Action OnCoinFlipRequired;

        private float stateTimer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            player1 = new PlayerHand("Player 1");
            player2 = new PlayerHand("Player 2");
            ChangeState(GameState.WaitingToStart);
        }

        private void Update()
        {
            if (currentState == GameState.AttackPhase || currentState == GameState.DefensePhase)
            {
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    OnPhaseTimeout();
                }
            }
        }

        public void StartGame()
        {
            player1RoundWins = 0;
            player2RoundWins = 0;
            StartNewRound();
        }

        private void StartNewRound()
        {
            player1.ResetHands();
            player2.ResetHands();
            OnHandsUpdated?.Invoke(player1, player2);
            ChangeState(GameState.CoinFlip);
            OnCoinFlipRequired?.Invoke();
        }

        public void SetFirstAttacker(bool player1First)
        {
            isPlayer1Attacking = player1First;
            Debug.Log($"{(player1First ? "Player 1" : "Player 2")} won coin flip and attacks first!");
            ChangeState(GameState.AttackPhase);
            
            if (isVsAI && !isPlayer1Attacking)
            {
                Invoke(nameof(PerformAIAttack), aiThinkTime);
            }
        }

        public void SubmitAttack(GestureType leftGesture, GestureType rightGesture)
        {
            if (currentState != GameState.AttackPhase) return;
            
            // Validate attacker has both hands
            PlayerHand attacker = isPlayer1Attacking ? player1 : player2;
            if (!attacker.hasLeftHand || !attacker.hasRightHand)
            {
                Debug.LogError("Cannot perform double hand attack without both hands!");
                return;
            }
            
            attackLeftGesture = leftGesture;
            attackRightGesture = rightGesture;
            isSingleHandAttack = false;
            defenseSubmitted = false;
            
            // Update attacker's stance
            if (isPlayer1Attacking)
            {
                player1LeftStance = leftGesture;
                player1RightStance = rightGesture;
            }
            else
            {
                player2LeftStance = leftGesture;
                player2RightStance = rightGesture;
            }
            
            Debug.Log($"Double hand attack declared: {leftGesture} + {rightGesture}");
            
            ChangeState(GameState.DefensePhase);
            // Don't reveal attack until defense is submitted!
            
            if (isVsAI && isPlayer1Attacking)
            {
                Invoke(nameof(PerformAIDefense), aiThinkTime);
            }
        }

        public void SubmitSingleHandAttack(bool useLeftHand, GestureType gesture)
        {
            if (currentState != GameState.AttackPhase) return;
            
            PlayerHand attacker = isPlayer1Attacking ? player1 : player2;
            
            // Validate the attacker has the specified hand
            if (useLeftHand && !attacker.hasLeftHand)
            {
                Debug.LogError("Cannot attack with left hand - it's been eliminated!");
                return;
            }
            else if (!useLeftHand && !attacker.hasRightHand)
            {
                Debug.LogError("Cannot attack with right hand - it's been eliminated!");
                return;
            }
            
            attackLeftGesture = useLeftHand ? gesture : (GestureType?)null;
            attackRightGesture = useLeftHand ? (GestureType?)null : gesture;
            isSingleHandAttack = true;
            defenseSubmitted = false;
            
            // Update attacker's stance for the attacking hand
            if (isPlayer1Attacking)
            {
                if (useLeftHand)
                    player1LeftStance = gesture;
                else
                    player1RightStance = gesture;
            }
            else
            {
                if (useLeftHand)
                    player2LeftStance = gesture;
                else
                    player2RightStance = gesture;
            }
            
            Debug.Log($"Single hand attack declared: {gesture} (using {(useLeftHand ? "left" : "right")} hand)");
            
            ChangeState(GameState.DefensePhase);
            // Don't reveal attack until defense is submitted!
            
            if (isVsAI && isPlayer1Attacking)
            {
                Invoke(nameof(PerformAIDefense), aiThinkTime);
            }
        }

        public void SubmitDefense(GestureType leftGesture, GestureType rightGesture)
        {
            if (currentState != GameState.DefensePhase) return;
            
            defenseLeftGesture = leftGesture;
            defenseRightGesture = rightGesture;
            
            // Update defender's stance immediately
            PlayerHand defender = isPlayer1Attacking ? player2 : player1;
            defenderHandCountBeforeAttack = defender.GetHandCount();
            
            if (isPlayer1Attacking)
            {
                // Player 2 is defending
                if (defender.hasLeftHand)
                    player2LeftStance = leftGesture;
                if (defender.hasRightHand)
                    player2RightStance = rightGesture;
            }
            else
            {
                // Player 1 is defending
                if (defender.hasLeftHand)
                    player1LeftStance = leftGesture;
                if (defender.hasRightHand)
                    player1RightStance = rightGesture;
            }
            
            // If not submitted yet, reveal attack and give 0.3 second window
            if (!defenseSubmitted)
            {
                Debug.Log($"Defense stance set: {leftGesture} + {rightGesture}. Attack revealing...");
                
                // Reveal the attack!
                OnAttackDeclared?.Invoke(attackLeftGesture ?? GestureType.Gu, attackRightGesture ?? GestureType.Gu);
                
                // Give 0.3 second reaction window
                Invoke(nameof(FinalizeDefense), 0.3f);
            }
            else
            {
                // Defense can still be changed during the reaction window
                Debug.Log($"Defense changed to: {leftGesture} + {rightGesture}");
            }
        }
        
        private void FinalizeDefense()
        {
            if (defenseSubmitted) return;
            
            defenseSubmitted = true;
            Debug.Log($"Defense finalized: {defenseLeftGesture} + {defenseRightGesture}");
            
            // Proceed to resolution
            Invoke(nameof(ResolveTurn), 1.2f);
        }

        private void ResolveTurn()
        {
            ChangeState(GameState.ResolvingTurn);

            PlayerHand attacker = isPlayer1Attacking ? player1 : player2;
            PlayerHand defender = isPlayer1Attacking ? player2 : player1;
            successfulHits = 0;

            // Position-independent matching logic
            bool leftDefenseMatched = false;
            bool rightDefenseMatched = false;
            
            // Check if left attack matches any defense
            if (attackLeftGesture.HasValue)
            {
                if (defender.hasLeftHand && CheckAttackSuccess(attackLeftGesture.Value, defenseLeftGesture))
                {
                    leftDefenseMatched = true;
                }
                if (defender.hasRightHand && CheckAttackSuccess(attackLeftGesture.Value, defenseRightGesture))
                {
                    rightDefenseMatched = true;
                }
            }
            
            // Check if right attack matches any defense
            if (attackRightGesture.HasValue)
            {
                if (defender.hasLeftHand && !leftDefenseMatched && CheckAttackSuccess(attackRightGesture.Value, defenseLeftGesture))
                {
                    leftDefenseMatched = true;
                }
                if (defender.hasRightHand && !rightDefenseMatched && CheckAttackSuccess(attackRightGesture.Value, defenseRightGesture))
                {
                    rightDefenseMatched = true;
                }
            }
            
            // Apply damage based on matches
            if (leftDefenseMatched && rightDefenseMatched)
            {
                // Both hands matched - use cross-elimination rule
                // Attacker's left eliminates defender's right (facing each other)
                // Attacker's right eliminates defender's left (facing each other)
                
                // But if it's a single hand attack, only eliminate one
                if (isSingleHandAttack)
                {
                    // Eliminate the first matched hand
                    if (defender.hasLeftHand)
                    {
                        defender.LoseHand(true);
                        successfulHits = 1;
                        Debug.Log($"Single hand attack eliminates defender's left hand (both matched)!");
                    }
                }
                else
                {
                    // Double hand attack - determine which hands to eliminate
                    bool eliminateDefenderLeft = false;
                    bool eliminateDefenderRight = false;
                    
                    // If attacker's left matches defender's right
                    if (attackLeftGesture.HasValue && defender.hasRightHand && 
                        CheckAttackSuccess(attackLeftGesture.Value, defenseRightGesture))
                    {
                        eliminateDefenderRight = true;
                    }
                    
                    // If attacker's right matches defender's left
                    if (attackRightGesture.HasValue && defender.hasLeftHand && 
                        CheckAttackSuccess(attackRightGesture.Value, defenseLeftGesture))
                    {
                        eliminateDefenderLeft = true;
                    }
                    
                    // Apply eliminations
                    if (eliminateDefenderLeft && defender.hasLeftHand)
                    {
                        defender.LoseHand(true);
                        successfulHits++;
                        Debug.Log($"Attacker's right eliminates defender's left hand!");
                    }
                    if (eliminateDefenderRight && defender.hasRightHand)
                    {
                        defender.LoseHand(false);
                        successfulHits++;
                        Debug.Log($"Attacker's left eliminates defender's right hand!");
                    }
                }
            }
            else
            {
                // Normal elimination - any match eliminates that hand
                if (leftDefenseMatched && defender.hasLeftHand)
                {
                    defender.LoseHand(true);
                    successfulHits++;
                    Debug.Log($"Attack eliminates defender's left hand!");
                }
                if (rightDefenseMatched && defender.hasRightHand)
                {
                    defender.LoseHand(false);
                    successfulHits++;
                    Debug.Log($"Attack eliminates defender's right hand!");
                }
            }

            OnAttackResult?.Invoke(successfulHits);
            OnHandsUpdated?.Invoke(player1, player2);

            Invoke(nameof(CheckRoundEnd), 2f);
        }

        private bool CheckAttackSuccess(GestureType attack, GestureType defense)
        {
            // In Jigupa, attack succeeds when it MATCHES the defense
            // This is the opposite of rock-paper-scissors!
            // If attacker shows Gu and defender shows Gu, the attack succeeds
            return attack == defense;
        }

        private void CheckRoundEnd()
        {
            PlayerHand currentDefender = isPlayer1Attacking ? player2 : player1;
            
            if (!currentDefender.HasHandsRemaining())
            {
                if (isPlayer1Attacking)
                {
                    player1RoundWins++;
                    Debug.Log("Player 1 wins the round!");
                }
                else
                {
                    player2RoundWins++;
                    Debug.Log("Player 2 (AI) wins the round!");
                }

                if (player1RoundWins >= 3 || player2RoundWins >= 3)
                {
                    ChangeState(GameState.GameOver);
                }
                else
                {
                    ChangeState(GameState.RoundOver);
                    Invoke(nameof(StartNewRound), 3f);
                }
            }
            else
            {
                isPlayer1Attacking = !isPlayer1Attacking;
                ChangeState(GameState.AttackPhase);
                
                if (isVsAI && !isPlayer1Attacking)
                {
                    Invoke(nameof(PerformAIAttack), aiThinkTime);
                }
            }
        }

        private void OnPhaseTimeout()
        {
            if (currentState == GameState.AttackPhase)
            {
                // Use current stance for timeout attack
                PlayerHand attacker = isPlayer1Attacking ? player1 : player2;
                GestureType leftStance = isPlayer1Attacking ? player1LeftStance : player2LeftStance;
                GestureType rightStance = isPlayer1Attacking ? player1RightStance : player2RightStance;
                
                if (attacker.HasBothHands())
                {
                    SubmitAttack(leftStance, rightStance);
                }
                else if (attacker.hasLeftHand)
                {
                    SubmitSingleHandAttack(true, leftStance);
                }
                else if (attacker.hasRightHand)
                {
                    SubmitSingleHandAttack(false, rightStance);
                }
            }
            else if (currentState == GameState.DefensePhase && !defenseSubmitted)
            {
                // Use current stance for timeout defense
                GestureType leftStance = isPlayer1Attacking ? player2LeftStance : player1LeftStance;
                GestureType rightStance = isPlayer1Attacking ? player2RightStance : player1RightStance;
                SubmitDefense(leftStance, rightStance);
            }
        }

        private void PerformAIAttack()
        {
            if (currentState != GameState.AttackPhase || isPlayer1Attacking) return;
            
            PlayerHand aiHand = player2;
            
            if (!aiHand.HasHandsRemaining())
            {
                Debug.LogError("AI has no hands to attack with!");
                return;
            }
            
            // AI can use any gesture with remaining hands
            GestureType[] gestures = { GestureType.Gu, GestureType.Pa, GestureType.Ji };
            
            // Check if AI can do double hand attack
            if (aiHand.HasBothHands())
            {
                GestureType leftAttack = gestures[UnityEngine.Random.Range(0, 3)];
                GestureType rightAttack = gestures[UnityEngine.Random.Range(0, 3)];
                Debug.Log($"AI attacks with: {leftAttack} + {rightAttack}");
                SubmitAttack(leftAttack, rightAttack);
            }
            else
            {
                // Single hand attack
                bool useLeftHand = aiHand.hasLeftHand;
                GestureType attack = gestures[UnityEngine.Random.Range(0, 3)];
                
                Debug.Log($"AI attacks with single {(useLeftHand ? "left" : "right")} hand: {attack}");
                SubmitSingleHandAttack(useLeftHand, attack);
            }
        }

        private void PerformAIDefense()
        {
            if (currentState != GameState.DefensePhase || !isPlayer1Attacking || defenseSubmitted) return;
            
            PlayerHand aiHand = player2;
            
            // AI must choose defense WITHOUT seeing the attack!
            // This is purely random/strategic
            GestureType[] gestures = { GestureType.Gu, GestureType.Pa, GestureType.Ji };
            
            // AI strategy: sometimes use same gesture on both hands (risky but limits damage)
            // sometimes use different gestures (safer but can lose both)
            bool useSameGesture = UnityEngine.Random.value < 0.3f; // 30% chance
            
            GestureType leftDefense = gestures[UnityEngine.Random.Range(0, 3)];
            GestureType rightDefense = useSameGesture ? leftDefense : gestures[UnityEngine.Random.Range(0, 3)];
            
            // Log AI's defense choice
            if (aiHand.GetHandCount() == 1)
            {
                GestureType singleDefense = aiHand.hasLeftHand ? leftDefense : rightDefense;
                Debug.Log($"AI defends with single hand: {singleDefense}");
            }
            else
            {
                Debug.Log($"AI defends with: {leftDefense} + {rightDefense}");
            }
            
            // Submit defense based on which hands AI still has
            if (!aiHand.hasLeftHand)
            {
                leftDefense = GestureType.Gu; // Dummy value, won't be checked
            }
            if (!aiHand.hasRightHand)
            {
                rightDefense = GestureType.Gu; // Dummy value, won't be checked
            }
            
            SubmitDefense(leftDefense, rightDefense);
        }

        private GestureType GetCounterGesture(GestureType attackGesture)
        {
            // In Jigupa, we want to AVOID matching the attack gesture
            // So return a different gesture
            GestureType[] gestures = { GestureType.Gu, GestureType.Pa, GestureType.Ji };
            GestureType avoid1 = attackGesture;
            
            // Pick a random gesture that's NOT the attack gesture
            GestureType result;
            do
            {
                result = gestures[UnityEngine.Random.Range(0, 3)];
            } while (result == avoid1);
            
            return result;
        }

        private void ChangeState(GameState newState)
        {
            currentState = newState;
            stateTimer = turnTimer;
            OnStateChanged?.Invoke(newState);
            Debug.Log($"Game State: {newState}");
        }

        public GameState GetCurrentState() => currentState;
        public PlayerHand GetPlayer1() => player1;
        public PlayerHand GetPlayer2() => player2;
        public int GetPlayer1Wins() => player1RoundWins;
        public int GetPlayer2Wins() => player2RoundWins;
        public float GetStateTimer() => stateTimer;
        public bool IsPlayer1Attacking() => isPlayer1Attacking;
        public void GetCurrentAttack(out GestureType? left, out GestureType? right)
        {
            left = attackLeftGesture;
            right = attackRightGesture;
        }
        
        public bool IsSingleHandAttack() => isSingleHandAttack;
        
        public GestureType GetLastDefenseLeft() => defenseLeftGesture;
        public GestureType GetLastDefenseRight() => defenseRightGesture;
        public GestureType GetLastDefenseGesture()
        {
            // For single hand defense, return the gesture used
            PlayerHand defender = isPlayer1Attacking ? player2 : player1;
            return defender.hasLeftHand ? defenseLeftGesture : defenseRightGesture;
        }
        
        public int GetDefenderHandCountBeforeAttack() => defenderHandCountBeforeAttack;
        
        // Stance getters
        public GestureType GetPlayer1LeftStance() => player1LeftStance;
        public GestureType GetPlayer1RightStance() => player1RightStance;
        public GestureType GetPlayer2LeftStance() => player2LeftStance;
        public GestureType GetPlayer2RightStance() => player2RightStance;
    }
}