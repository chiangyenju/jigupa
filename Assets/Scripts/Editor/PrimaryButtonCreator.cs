using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class PrimaryButtonCreator : MonoBehaviour
{
    [MenuItem("GameObject/UI/Jigupa/Primary Button", false, 0)]
    public static void CreatePrimaryButton()
    {
        // Get or create canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        GameObject parent = canvas != null ? canvas.gameObject : null;
        
        if (canvas == null)
        {
            // Create canvas if it doesn't exist
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            parent = canvasObject;
        }
        
        // Create button GameObject
        GameObject buttonObject = new GameObject("PrimaryButton");
        buttonObject.transform.SetParent(parent.transform, false);
        
        // Set up RectTransform with cuter proportions
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        float width = 200f;
        float height = 80f; // Cute pill shape
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Add Image component with rounded corners sprite
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.674f, 0.035f, 0.161f, 1f); // #ac0929
        
        // Try to load a rounded rectangle sprite if available
        Sprite roundedSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/RoundedRectangle.png");
        if (roundedSprite != null)
        {
            buttonImage.sprite = roundedSprite;
            buttonImage.type = Image.Type.Sliced;
        }
        
        // Add Button component
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        // Add PrimaryButton component
        buttonObject.AddComponent<PrimaryButton>();
        
        // Create text child
        GameObject textObject = new GameObject("Text (TMP)");
        textObject.transform.SetParent(buttonObject.transform, false);
        
        // Set up text RectTransform
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = new Vector2(-20, -10);
        textRect.anchoredPosition = Vector2.zero;
        
        // Add TextMeshProUGUI component
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = "Button";
        text.fontSize = 32;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;
        
        // Select the created button
        Selection.activeGameObject = buttonObject;
        
        // Mark scene as dirty
        EditorUtility.SetDirty(buttonObject);
        
        Debug.Log("Primary Button created! To save as prefab: drag it from Hierarchy to Assets/Prefabs folder");
    }
    
    [MenuItem("Jigupa/Create UI Sprites/Rounded Rectangle")]
    public static void CreateRoundedRectangleSprite()
    {
        // Create a rounded rectangle texture
        int width = 64;
        int height = 64;
        int cornerRadius = 20; // More rounded for cute look
        
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        
        // Fill with transparent
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        // Draw rounded rectangle
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inRectangle = true;
                
                // Check corners
                if (x < cornerRadius && y < cornerRadius)
                {
                    // Top-left corner
                    float dx = cornerRadius - x;
                    float dy = cornerRadius - y;
                    inRectangle = (dx * dx + dy * dy) <= (cornerRadius * cornerRadius);
                }
                else if (x >= width - cornerRadius && y < cornerRadius)
                {
                    // Top-right corner
                    float dx = x - (width - cornerRadius - 1);
                    float dy = cornerRadius - y;
                    inRectangle = (dx * dx + dy * dy) <= (cornerRadius * cornerRadius);
                }
                else if (x < cornerRadius && y >= height - cornerRadius)
                {
                    // Bottom-left corner
                    float dx = cornerRadius - x;
                    float dy = y - (height - cornerRadius - 1);
                    inRectangle = (dx * dx + dy * dy) <= (cornerRadius * cornerRadius);
                }
                else if (x >= width - cornerRadius && y >= height - cornerRadius)
                {
                    // Bottom-right corner
                    float dx = x - (width - cornerRadius - 1);
                    float dy = y - (height - cornerRadius - 1);
                    inRectangle = (dx * dx + dy * dy) <= (cornerRadius * cornerRadius);
                }
                
                if (inRectangle)
                {
                    pixels[y * width + x] = Color.white;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Ensure the directory exists
        if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
        {
            AssetDatabase.CreateFolder("Assets", "Sprites");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Sprites/UI"))
        {
            AssetDatabase.CreateFolder("Assets/Sprites", "UI");
        }
        
        // Save the texture as PNG
        byte[] pngData = texture.EncodeToPNG();
        string path = "Assets/Sprites/UI/RoundedRectangle.png";
        System.IO.File.WriteAllBytes(path, pngData);
        
        AssetDatabase.Refresh();
        
        // Import settings for the sprite
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spriteBorder = new Vector4(cornerRadius, cornerRadius, cornerRadius, cornerRadius);
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = 256;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
        
        Debug.Log("Rounded Rectangle sprite created at: " + path);
    }
    
    [MenuItem("GameObject/UI/Jigupa/Secondary Button", false, 1)]
    public static void CreateSecondaryButton()
    {
        // Get or create canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        GameObject parent = canvas != null ? canvas.gameObject : null;
        
        if (canvas == null)
        {
            // Create canvas if it doesn't exist
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            parent = canvasObject;
        }
        
        // Create button GameObject
        GameObject buttonObject = new GameObject("SecondaryButton");
        buttonObject.transform.SetParent(parent.transform, false);
        
        // Set up RectTransform with cute proportions
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        float width = 180f;
        float height = 60f; // Smaller than primary
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Add Image component with rounded corners sprite
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.95f, 0.95f, 0.95f, 1f); // Light gray
        
        // Try to load a rounded rectangle sprite if available
        Sprite roundedSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/RoundedRectangle.png");
        if (roundedSprite != null)
        {
            buttonImage.sprite = roundedSprite;
            buttonImage.type = Image.Type.Sliced;
        }
        
        // Add Button component
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        // Add SecondaryButton component
        buttonObject.AddComponent<SecondaryButton>();
        
        // Create text child
        GameObject textObject = new GameObject("Text (TMP)");
        textObject.transform.SetParent(buttonObject.transform, false);
        
        // Set up text RectTransform
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = new Vector2(-20, -10);
        textRect.anchoredPosition = Vector2.zero;
        
        // Add TextMeshProUGUI component
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = "Cancel";
        text.fontSize = 24;
        text.color = new Color(0.674f, 0.035f, 0.161f, 1f); // #ac0929
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;
        
        // Select the created button
        Selection.activeGameObject = buttonObject;
        
        // Mark scene as dirty
        EditorUtility.SetDirty(buttonObject);
        
        Debug.Log("Secondary Button created! To save as prefab: drag it from Hierarchy to Assets/Prefabs folder");
    }
}