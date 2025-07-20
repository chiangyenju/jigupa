using UnityEngine;
using Jigupa.Core;

namespace Jigupa.Player
{
    [System.Serializable]
    public class PlayerHand
    {
        public string playerName;
        public bool hasLeftHand = true;
        public bool hasRightHand = true;

        public PlayerHand(string name)
        {
            playerName = name;
            ResetHands();
        }

        public void ResetHands()
        {
            hasLeftHand = true;
            hasRightHand = true;
        }

        public void LoseHand(bool isLeftHand)
        {
            if (isLeftHand)
            {
                hasLeftHand = false;
                Debug.Log($"{playerName} lost left hand!");
            }
            else
            {
                hasRightHand = false;
                Debug.Log($"{playerName} lost right hand!");
            }
        }

        public bool HasHandsRemaining()
        {
            return hasLeftHand || hasRightHand;
        }

        public bool HasBothHands()
        {
            return hasLeftHand && hasRightHand;
        }

        public int GetHandCount()
        {
            int count = 0;
            if (hasLeftHand) count++;
            if (hasRightHand) count++;
            return count;
        }
    }
}