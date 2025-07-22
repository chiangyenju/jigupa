using UnityEngine;

public static class PrefabLoader
{
    public static GameObject LoadPrimaryButtonPrefab()
    {
        // Try Resources folder first
        GameObject prefab = Resources.Load<GameObject>("PrimaryButton");
        
        if (prefab == null)
        {
            // Try Resources/Prefabs folder
            prefab = Resources.Load<GameObject>("Prefabs/PrimaryButton");
        }
        
        #if UNITY_EDITOR
        if (prefab == null)
        {
            // In editor, try loading from Assets folder
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PrimaryButton.prefab");
        }
        #endif
        
        return prefab;
    }
    
    public static GameObject LoadSecondaryButtonPrefab()
    {
        // Try Resources folder first
        GameObject prefab = Resources.Load<GameObject>("SecondaryButton");
        
        if (prefab == null)
        {
            // Try Resources/Prefabs folder
            prefab = Resources.Load<GameObject>("Prefabs/SecondaryButton");
        }
        
        #if UNITY_EDITOR
        if (prefab == null)
        {
            // In editor, try loading from Assets folder
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SecondaryButton.prefab");
        }
        #endif
        
        return prefab;
    }
}