using UnityEngine;
using UnityEngine.UI;
using Jigupa.Core;
using System;
using System.Collections.Generic;

namespace Jigupa.Player
{
    public class GestureManager : MonoBehaviour
    {
        [Header("Left Hand UI")]
        [SerializeField] private Button leftRockButton;
        [SerializeField] private Button leftPaperButton;
        [SerializeField] private Button leftScissorsButton;

        [Header("Right Hand UI")]
        [SerializeField] private Button rightRockButton;
        [SerializeField] private Button rightPaperButton;
        [SerializeField] private Button rightScissorsButton;

        [Header("Control")]
        [SerializeField] private GameObject gesturePanel;
        [SerializeField] private Button submitButton;
        [SerializeField] private bool isPlayer1 = true;

        private GestureType? selectedLeftGesture;
        private GestureType? selectedRightGesture;
        private PlayerHand myHand;
        private bool isMyTurn = false;

        private void Start()
        {
            SetupButtons();
            
            if (GameStateManager.Instance)
            {
                GameStateManager.Instance.OnStateChanged += OnGameStateChanged;
                GameStateManager.Instance.OnHandsUpdated += OnHandsUpdated;
            }

            if (submitButton)
            {
                submitButton.onClick.AddListener(SubmitGestures);
                submitButton.interactable = false;
            }

            SetPanelActive(false);
        }

        private void SetupButtons()
        {
            if (leftRockButton) leftRockButton.onClick.AddListener(() => SelectLeftGesture(GestureType.Gu));
            if (leftPaperButton) leftPaperButton.onClick.AddListener(() => SelectLeftGesture(GestureType.Pa));
            if (leftScissorsButton) leftScissorsButton.onClick.AddListener(() => SelectLeftGesture(GestureType.Ji));

            if (rightRockButton) rightRockButton.onClick.AddListener(() => SelectRightGesture(GestureType.Gu));
            if (rightPaperButton) rightPaperButton.onClick.AddListener(() => SelectRightGesture(GestureType.Pa));
            if (rightScissorsButton) rightScissorsButton.onClick.AddListener(() => SelectRightGesture(GestureType.Ji));
        }

        private void OnDestroy()
        {
            if (GameStateManager.Instance)
            {
                GameStateManager.Instance.OnStateChanged -= OnGameStateChanged;
                GameStateManager.Instance.OnHandsUpdated -= OnHandsUpdated;
            }
        }

        private void OnHandsUpdated(PlayerHand player1, PlayerHand player2)
        {
            myHand = isPlayer1 ? player1 : player2;
            UpdateButtonAvailability();
        }

        private void OnGameStateChanged(GameState newState)
        {
            bool isAttackPhase = newState == GameState.AttackPhase;
            bool isDefensePhase = newState == GameState.DefensePhase;
            bool isPlayer1Attacking = GameStateManager.Instance.IsPlayer1Attacking();

            isMyTurn = (isPlayer1 && isPlayer1Attacking && isAttackPhase) ||
                      (isPlayer1 && !isPlayer1Attacking && isDefensePhase) ||
                      (!isPlayer1 && !isPlayer1Attacking && isAttackPhase) ||
                      (!isPlayer1 && isPlayer1Attacking && isDefensePhase);

            SetPanelActive(isMyTurn);
            
            if (isMyTurn)
            {
                ResetSelection();
                UpdateButtonAvailability();
                
                // For attack phase, check if we can attack
                if (isAttackPhase && myHand != null)
                {
                    bool canAttack = myHand.HasHandsRemaining();
                    if (!canAttack)
                    {
                        Debug.Log("Cannot attack - no hands remaining!");
                        SetPanelActive(false);
                    }
                }
            }
        }

        private void UpdateButtonAvailability()
        {
            if (myHand == null) return;

            // Left hand buttons - active if hand exists
            bool leftActive = myHand.hasLeftHand;
            SetButtonActive(leftRockButton, leftActive);
            SetButtonActive(leftPaperButton, leftActive);
            SetButtonActive(leftScissorsButton, leftActive);

            // Right hand buttons - active if hand exists
            bool rightActive = myHand.hasRightHand;
            SetButtonActive(rightRockButton, rightActive);
            SetButtonActive(rightPaperButton, rightActive);
            SetButtonActive(rightScissorsButton, rightActive);
        }

        private void SetButtonActive(Button button, bool active)
        {
            if (button)
            {
                button.gameObject.SetActive(active);
                button.interactable = active;
            }
        }

        private void SelectLeftGesture(GestureType gesture)
        {
            selectedLeftGesture = gesture;
            UpdateSubmitButton();
            Debug.Log($"Left hand selected: {gesture}");
            CheckAutoSubmit();
        }

        private void SelectRightGesture(GestureType gesture)
        {
            selectedRightGesture = gesture;
            UpdateSubmitButton();
            Debug.Log($"Right hand selected: {gesture}");
            CheckAutoSubmit();
        }
        
        private void CheckAutoSubmit()
        {
            // Auto-submit when both gestures are selected (or when all available hands have selections)
            if (myHand != null)
            {
                bool shouldAutoSubmit = false;
                
                // If both hands exist and both are selected
                if (myHand.hasLeftHand && myHand.hasRightHand && 
                    selectedLeftGesture.HasValue && selectedRightGesture.HasValue)
                {
                    shouldAutoSubmit = true;
                }
                // If only left hand exists and it's selected
                else if (myHand.hasLeftHand && !myHand.hasRightHand && selectedLeftGesture.HasValue)
                {
                    shouldAutoSubmit = true;
                }
                // If only right hand exists and it's selected
                else if (!myHand.hasLeftHand && myHand.hasRightHand && selectedRightGesture.HasValue)
                {
                    shouldAutoSubmit = true;
                }
                
                if (shouldAutoSubmit)
                {
                    Debug.Log("Auto-submitting gestures!");
                    SubmitGestures();
                }
            }
        }

        private void UpdateSubmitButton()
        {
            if (submitButton)
            {
                bool isAttacking = (isPlayer1 && GameStateManager.Instance.IsPlayer1Attacking()) ||
                                 (!isPlayer1 && !GameStateManager.Instance.IsPlayer1Attacking());
                
                if (isAttacking)
                {
                    // For attack, need at least one gesture selected
                    submitButton.interactable = selectedLeftGesture.HasValue || selectedRightGesture.HasValue;
                }
                else
                {
                    // For defense, need at least one gesture if you have at least one hand
                    if (myHand != null)
                    {
                        bool needsLeft = myHand.hasLeftHand;
                        bool needsRight = myHand.hasRightHand;
                        
                        bool hasRequiredSelections = (!needsLeft || selectedLeftGesture.HasValue) && 
                                                    (!needsRight || selectedRightGesture.HasValue) &&
                                                    (selectedLeftGesture.HasValue || selectedRightGesture.HasValue);
                        
                        submitButton.interactable = hasRequiredSelections;
                    }
                    else
                    {
                        submitButton.interactable = false;
                    }
                }
            }
        }

        private void SubmitGestures()
        {
            bool isAttacking = (isPlayer1 && GameStateManager.Instance.IsPlayer1Attacking()) ||
                              (!isPlayer1 && !GameStateManager.Instance.IsPlayer1Attacking());

            if (isAttacking)
            {
                // With 2 hands, MUST attack with both
                if (myHand.hasLeftHand && myHand.hasRightHand)
                {
                    // Need both gestures selected for double-hand attack
                    if (!selectedLeftGesture.HasValue || !selectedRightGesture.HasValue)
                    {
                        return; // Need both hands selected
                    }
                    GameStateManager.Instance.SubmitAttack(selectedLeftGesture.Value, selectedRightGesture.Value);
                }
                else
                {
                    // Single hand attack (only have one hand)
                    if (selectedLeftGesture.HasValue)
                    {
                        GameStateManager.Instance.SubmitSingleHandAttack(true, selectedLeftGesture.Value);
                    }
                    else if (selectedRightGesture.HasValue)
                    {
                        GameStateManager.Instance.SubmitSingleHandAttack(false, selectedRightGesture.Value);
                    }
                    else
                    {
                        return; // No selection
                    }
                }
            }
            else
            {
                // For defense, submit what we have based on remaining hands
                GestureType leftDef = selectedLeftGesture ?? GestureType.Gu; // Dummy if no left hand
                GestureType rightDef = selectedRightGesture ?? GestureType.Gu; // Dummy if no right hand
                
                // Make sure we have at least one selection for hands we still have
                if ((myHand.hasLeftHand && !selectedLeftGesture.HasValue) ||
                    (myHand.hasRightHand && !selectedRightGesture.HasValue))
                {
                    return; // Missing required selection
                }
                
                GameStateManager.Instance.SubmitDefense(leftDef, rightDef);
            }

            SetPanelActive(false);
        }

        private void ResetSelection()
        {
            selectedLeftGesture = null;
            selectedRightGesture = null;
            UpdateSubmitButton();
        }

        private void SetPanelActive(bool active)
        {
            if (gesturePanel) gesturePanel.SetActive(active);
        }
    }
}