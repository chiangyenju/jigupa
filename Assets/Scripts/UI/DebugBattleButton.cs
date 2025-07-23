using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;

namespace Jigupa.UI
{
    // Temporary debug component to inspect button listeners
    public class DebugBattleButton : MonoBehaviour
    {
        private Button button;
        
        void Start()
        {
            button = GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("DebugBattleButton: No Button component found!");
                return;
            }
            
            // Add our own debug listener
            button.onClick.AddListener(OnDebugClick);
        }
        
        void OnDebugClick()
        {
            Debug.Log("=== DEBUG BATTLE BUTTON CLICK ===");
            
            if (button == null) return;
            
            // Log persistent listeners
            int persistentCount = button.onClick.GetPersistentEventCount();
            Debug.Log($"Persistent Listeners: {persistentCount}");
            for (int i = 0; i < persistentCount; i++)
            {
                string target = button.onClick.GetPersistentTarget(i)?.name ?? "null";
                string method = button.onClick.GetPersistentMethodName(i);
                Debug.Log($"  [{i}] Target: {target}, Method: {method}");
            }
            
            // Use reflection to check runtime listeners
            var clickEvent = button.onClick;
            var callsField = typeof(UnityEventBase).GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic);
            if (callsField != null)
            {
                var calls = callsField.GetValue(clickEvent);
                var runtimeCallsField = calls.GetType().GetField("m_RuntimeCalls", BindingFlags.Instance | BindingFlags.NonPublic);
                if (runtimeCallsField != null)
                {
                    var runtimeCalls = runtimeCallsField.GetValue(calls);
                    var countProperty = runtimeCalls.GetType().GetProperty("Count");
                    if (countProperty != null)
                    {
                        int runtimeCount = (int)countProperty.GetValue(runtimeCalls);
                        Debug.Log($"Runtime Listeners: {runtimeCount}");
                    }
                }
            }
            
            // Check for animation components
            var fistIcon = transform.parent?.Find("FistIcon");
            if (fistIcon != null)
            {
                Debug.Log($"FistIcon found: {fistIcon.name}, Active: {fistIcon.gameObject.activeInHierarchy}");
                var battleFistIcon = fistIcon.GetComponent<BattleFistIcon>();
                if (battleFistIcon != null)
                {
                    Debug.Log("BattleFistIcon component is attached to FistIcon");
                }
                else
                {
                    Debug.LogWarning("BattleFistIcon component NOT found on FistIcon!");
                }
            }
            else
            {
                Debug.LogWarning("FistIcon not found as sibling!");
            }
            
            Debug.Log("=== END DEBUG ===");
        }
        
        void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnDebugClick);
            }
        }
    }
}