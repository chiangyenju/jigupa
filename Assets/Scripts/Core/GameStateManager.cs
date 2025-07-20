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
        Rock,    // Gu (구)
        Paper,   // Pa (파)
        Scissors // Ji (지)
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
            if (currentState != GameState.DefensePhase || defenseSubmitted) return;
            
            defenseLeftGesture = leftGesture;
            defenseRightGesture = rightGesture;
            defenseSubmitted = true;
            
            // Store defender's hand count before attack resolution
            PlayerHand defender = isPlayer1Attacking ? player2 : player1;
            defenderHandCountBeforeAttack = defender.GetHandCount();
            
            Debug.Log($"Defense submitted: {leftGesture} + {rightGesture}");
            
            // Now reveal the attack!
            OnAttackDeclared?.Invoke(attackLeftGesture ?? GestureType.Rock, attackRightGesture ?? GestureType.Rock);
            
            // Small delay to show attack before resolving
            Invoke(nameof(ResolveTurn), 1.5f);
        }

        private void ResolveTurn()
        {
            ChangeState(GameState.ResolvingTurn);

            PlayerHand attacker = isPlayer1Attacking ? player1 : player2;
            PlayerHand defender = isPlayer1Attacking ? player2 : player1;
            successfulHits = 0;

            if (isSingleHandAttack)
            {
                // Single hand attack - can only eliminate one defending hand
                GestureType attackGesture = attackLeftGesture ?? attackRightGesture.Value;
                
                // For single hand defense - if defender only has one hand, check if attack matches their defense
                if (defender.GetHandCount() == 1)
                {
                    // Defender chose one gesture to defend with their remaining hand
                    GestureType defenseGesture = defender.hasLeftHand ? defenseLeftGesture : defenseRightGesture;
                    
                    if (CheckAttackSuccess(attackGesture, defenseGesture))
                    {
                        // Attack matches defense - eliminate the last hand
                        if (defender.hasLeftHand)
                        {
                            defender.LoseHand(true);
                            Debug.Log($"Single hand attack eliminates defender's last (left) hand! {attackGesture} matches {defenseGesture}");
                        }
                        else
                        {
                            defender.LoseHand(false);
                            Debug.Log($"Single hand attack eliminates defender's last (right) hand! {attackGesture} matches {defenseGesture}");
                        }
                        successfulHits = 1;
                    }
                    else
                    {
                        Debug.Log($"Defense successful! {defenseGesture} doesn't match {attackGesture}");
                    }
                }
                else
                {
                    // Normal two-hand defense
                    bool beatsLeft = defender.hasLeftHand && CheckAttackSuccess(attackGesture, defenseLeftGesture);
                    bool beatsRight = defender.hasRightHand && CheckAttackSuccess(attackGesture, defenseRightGesture);
                    
                    // For single hand attack against two hands, can only eliminate one
                    if (beatsLeft && beatsRight)
                    {
                        // Choose left hand first by convention
                        defender.LoseHand(true);
                        successfulHits = 1;
                        Debug.Log($"Single hand attack eliminates defender's left hand! {attackGesture} matches {defenseLeftGesture}");
                    }
                    else if (beatsLeft)
                    {
                        defender.LoseHand(true);
                        successfulHits = 1;
                        Debug.Log($"Single hand attack eliminates defender's left hand! {attackGesture} matches {defenseLeftGesture}");
                    }
                    else if (beatsRight)
                    {
                        defender.LoseHand(false);
                        successfulHits = 1;
                        Debug.Log($"Single hand attack eliminates defender's right hand! {attackGesture} matches {defenseRightGesture}");
                    }
                }
            }
            else
            {
                // Double hand attack
                if (defender.GetHandCount() == 1)
                {
                    // Defender has only one hand - check if either attack matches their single defense
                    GestureType defenseGesture = defender.hasLeftHand ? defenseLeftGesture : defenseRightGesture;
                    
                    bool leftAttackHits = attackLeftGesture.HasValue && CheckAttackSuccess(attackLeftGesture.Value, defenseGesture);
                    bool rightAttackHits = attackRightGesture.HasValue && CheckAttackSuccess(attackRightGesture.Value, defenseGesture);
                    
                    if (leftAttackHits || rightAttackHits)
                    {
                        // Either attack hand matches the defense - eliminate the last hand
                        if (defender.hasLeftHand)
                        {
                            defender.LoseHand(true);
                            Debug.Log($"Attack eliminates defender's last (left) hand! {(leftAttackHits ? attackLeftGesture : attackRightGesture)} matches {defenseGesture}");
                        }
                        else
                        {
                            defender.LoseHand(false);
                            Debug.Log($"Attack eliminates defender's last (right) hand! {(leftAttackHits ? attackLeftGesture : attackRightGesture)} matches {defenseGesture}");
                        }
                        successfulHits = 1;
                    }
                    else
                    {
                        Debug.Log($"Defense successful! {defenseGesture} doesn't match either attack");
                    }
                }
                else
                {
                    // Normal two-hand vs two-hand
                    if (attackLeftGesture.HasValue && defender.hasLeftHand)
                    {
                        bool leftHit = CheckAttackSuccess(attackLeftGesture.Value, defenseLeftGesture);
                        if (leftHit)
                        {
                            defender.LoseHand(true);
                            successfulHits++;
                            Debug.Log($"Attack eliminates defender's left hand! {attackLeftGesture} matches {defenseLeftGesture}");
                        }
                    }

                    if (attackRightGesture.HasValue && defender.hasRightHand)
                    {
                        bool rightHit = CheckAttackSuccess(attackRightGesture.Value, defenseRightGesture);
                        if (rightHit)
                        {
                            defender.LoseHand(false);
                            successfulHits++;
                            Debug.Log($"Attack eliminates defender's right hand! {attackRightGesture} matches {defenseRightGesture}");
                        }
                    }
                }
            }

            OnAttackResult?.Invoke(successfulHits);
            OnHandsUpdated?.Invoke(player1, player2);

            Invoke(nameof(CheckRoundEnd), 2f);
        }

        private bool CheckAttackSuccess(GestureType attack, GestureType defense)
        {
            // In Jigupa, attack succeeds when it MATCHES the defense
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
                GestureType randomLeft = (GestureType)UnityEngine.Random.Range(0, 3);
                GestureType randomRight = (GestureType)UnityEngine.Random.Range(0, 3);
                SubmitAttack(randomLeft, randomRight);
            }
            else if (currentState == GameState.DefensePhase && !defenseSubmitted)
            {
                PlayerHand defender = isPlayer1Attacking ? player2 : player1;
                // For timeout, just submit random gestures for remaining hands
                GestureType[] gestures = { GestureType.Rock, GestureType.Paper, GestureType.Scissors };
                GestureType randomLeft = gestures[UnityEngine.Random.Range(0, 3)];
                GestureType randomRight = gestures[UnityEngine.Random.Range(0, 3)];
                SubmitDefense(randomLeft, randomRight);
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
            GestureType[] gestures = { GestureType.Rock, GestureType.Paper, GestureType.Scissors };
            
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
            GestureType[] gestures = { GestureType.Rock, GestureType.Paper, GestureType.Scissors };
            
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
                leftDefense = GestureType.Rock; // Dummy value, won't be checked
            }
            if (!aiHand.hasRightHand)
            {
                rightDefense = GestureType.Rock; // Dummy value, won't be checked
            }
            
            SubmitDefense(leftDefense, rightDefense);
        }

        private GestureType GetCounterGesture(GestureType attackGesture)
        {
            // In Jigupa, we want to AVOID matching the attack gesture
            // So return a different gesture
            GestureType[] gestures = { GestureType.Rock, GestureType.Paper, GestureType.Scissors };
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
    }
}