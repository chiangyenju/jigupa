using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Jigupa.UI
{
    // This component goes on the battle button and ensures the fist icon exists
    public class BattleButtonFist : MonoBehaviour
    {
        private GameObject fistIcon;
        
        void Awake()
        {
            // Debug.Log($"[FIST] BattleButtonFist Awake on {gameObject.name}");
            
            // Check immediately if fist exists
            Transform parent = transform.parent;
            if (parent != null)
            {
                Transform fist = parent.Find("FistIcon");
                // Debug.Log($"[FIST] In Awake - Parent: {parent.name}, FistIcon exists: {fist != null}");
                // if (fist != null)
                // {
                //     Debug.Log($"[FIST] FistIcon active: {fist.gameObject.activeInHierarchy}");
                // }
            }
        }
        
        void Start()
        {
            // Debug.Log($"[FIST] BattleButtonFist Start on {gameObject.name}");
            StartCoroutine(CreateFistIconDelayed());
        }
        
        IEnumerator CreateFistIconDelayed()
        {
            // Debug.Log("[FIST] Coroutine started, waiting for end of frame...");
            
            // Wait for UI to be fully initialized
            yield return new WaitForEndOfFrame();
            
            // Debug.Log("[FIST] End of frame reached, calling CreateOrFindFistIcon");
            CreateOrFindFistIcon();
        }
        
        void CreateOrFindFistIcon()
        {
            // Look for existing fist icon as sibling
            Transform parent = transform.parent;
            if (parent != null)
            {
                fistIcon = parent.Find("FistIcon")?.gameObject;
                
                if (fistIcon == null)
                {
                    // Debug.LogError("[FIST] FistIcon not found! It should have been created in edit mode!");
                    // Debug.Log($"[FIST] Parent: {parent.name}, Children: {parent.childCount}");
                    // for (int i = 0; i < parent.childCount; i++)
                    // {
                    //     Debug.Log($"[FIST] Child {i}: {parent.GetChild(i).name}");
                    // }
                }
                else
                {
                    // Debug.Log($"[FIST] Found existing fist icon: {fistIcon.name}, Active: {fistIcon.activeSelf}");
                    
                    // Make sure it's active
                    if (!fistIcon.activeSelf)
                    {
                        fistIcon.SetActive(true);
                        // Debug.Log("[FIST] Activated fist icon");
                    }
                    
                    // Ensure it has animation component
                    if (fistIcon.GetComponent<BattleFistIcon>() == null)
                    {
                        fistIcon.AddComponent<BattleFistIcon>();
                        // Debug.Log("[FIST] Added BattleFistIcon component");
                    }
                }
            }
            else
            {
                // Debug.LogError("[FIST] Button has no parent!");
            }
        }
        
        void OnEnable()
        {
            if (fistIcon != null)
            {
                fistIcon.SetActive(true);
            }
        }
    }
}