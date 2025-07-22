using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Jigupa.UI
{
    public class FooterIconGenerator : MonoBehaviour
    {
        public enum IconType
        {
            Player,
            Guild,
            Battle,
            Minigame,
            Shop
        }
        
        [System.Serializable]
        public class IconStyle
        {
            public Color primaryColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            public Color secondaryColor = new Color(0.6f, 0.6f, 0.6f, 1f);
            public Color glowColor = new Color(1f, 1f, 1f, 0.3f);
            public float iconSize = 40f;
            public bool useGlow = true;
            public bool useGradient = true;
        }
        
        public static GameObject CreateIcon(Transform parent, IconType type, IconStyle style = null)
        {
            if (style == null) style = new IconStyle();
            
            GameObject iconContainer = new GameObject($"{type}Icon");
            iconContainer.transform.SetParent(parent, false);
            
            RectTransform containerRect = iconContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(style.iconSize, style.iconSize);
            
            // Add glow/aura effect
            if (style.useGlow)
            {
                AddGlowEffect(iconContainer, style);
            }
            
            // Create the actual icon
            switch (type)
            {
                case IconType.Player:
                    CreatePlayerIcon(iconContainer, style);
                    break;
                case IconType.Guild:
                    CreateGuildIcon(iconContainer, style);
                    break;
                case IconType.Battle:
                    CreateBattleIcon(iconContainer, style);
                    break;
                case IconType.Minigame:
                    CreateMinigameIcon(iconContainer, style);
                    break;
                case IconType.Shop:
                    CreateShopIcon(iconContainer, style);
                    break;
            }
            
            return iconContainer;
        }
        
        private static void AddGlowEffect(GameObject container, IconStyle style)
        {
            GameObject glow = new GameObject("Glow");
            glow.transform.SetParent(container.transform, false);
            glow.transform.SetAsFirstSibling();
            
            RectTransform glowRect = glow.AddComponent<RectTransform>();
            glowRect.anchorMin = Vector2.zero;
            glowRect.anchorMax = Vector2.one;
            glowRect.sizeDelta = new Vector2(20, 20); // Extend beyond icon
            
            Image glowImage = glow.AddComponent<Image>();
            glowImage.color = style.glowColor;
            
            // Create circular glow texture
            Texture2D glowTex = CreateRadialGradientTexture(64, style.glowColor);
            glowImage.sprite = Sprite.Create(glowTex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        }
        
        private static void CreatePlayerIcon(GameObject container, IconStyle style)
        {
            // Create head
            GameObject head = CreateCircle(container.transform, "Head", 
                new Vector2(0.5f, 0.65f), new Vector2(20, 20), style.primaryColor);
            
            // Add inner gradient for depth
            if (style.useGradient)
            {
                GameObject headInner = CreateCircle(head.transform, "HeadInner",
                    new Vector2(0.5f, 0.5f), new Vector2(16, 16), style.secondaryColor);
                headInner.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3f);
            }
            
            // Create body (rounded rectangle)
            GameObject body = CreateRoundedRect(container.transform, "Body",
                new Vector2(0.5f, 0.3f), new Vector2(30, 25), style.primaryColor);
            
            // Add body gradient
            if (style.useGradient)
            {
                GameObject bodyHighlight = CreateRoundedRect(body.transform, "BodyHighlight",
                    new Vector2(0.5f, 0.7f), new Vector2(24, 10), new Color(1f, 1f, 1f, 0.2f));
            }
        }
        
        private static void CreateGuildIcon(GameObject container, IconStyle style)
        {
            // Create house base
            GameObject houseBase = CreateRoundedRect(container.transform, "HouseBase",
                new Vector2(0.5f, 0.3f), new Vector2(35, 25), style.primaryColor);
            
            // Create roof (triangle)
            GameObject roof = CreateTriangle(container.transform, "Roof",
                new Vector2(0.5f, 0.65f), new Vector2(40, 20), style.secondaryColor);
            
            // Add chimney
            GameObject chimney = CreateRoundedRect(container.transform, "Chimney",
                new Vector2(0.75f, 0.75f), new Vector2(8, 12), style.primaryColor);
            
            // Add door
            GameObject door = CreateRoundedRect(houseBase.transform, "Door",
                new Vector2(0.5f, 0.3f), new Vector2(10, 15), style.secondaryColor);
            
            // Add window with glow effect
            GameObject window = CreateCircle(houseBase.transform, "Window",
                new Vector2(0.7f, 0.6f), new Vector2(8, 8), new Color(1f, 0.9f, 0.5f, 1f));
            
            if (style.useGlow)
            {
                GameObject windowGlow = CreateCircle(window.transform, "WindowGlow",
                    new Vector2(0.5f, 0.5f), new Vector2(12, 12), new Color(1f, 0.9f, 0.5f, 0.3f));
            }
        }
        
        private static void CreateBattleIcon(GameObject container, IconStyle style)
        {
            // Create crossed swords
            GameObject sword1 = CreateSword(container.transform, "Sword1",
                new Vector2(0.5f, 0.5f), 45f, style);
            
            GameObject sword2 = CreateSword(container.transform, "Sword2",
                new Vector2(0.5f, 0.5f), -45f, style);
            
            // Add clash effect at intersection
            GameObject clash = CreateCircle(container.transform, "ClashEffect",
                new Vector2(0.5f, 0.5f), new Vector2(15, 15), new Color(1f, 0.9f, 0.3f, 0.6f));
            
            // Add sparks
            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90f;
                Vector2 sparkPos = new Vector2(
                    0.5f + Mathf.Cos(angle * Mathf.Deg2Rad) * 0.2f,
                    0.5f + Mathf.Sin(angle * Mathf.Deg2Rad) * 0.2f
                );
                GameObject spark = CreateCircle(container.transform, $"Spark{i}",
                    sparkPos, new Vector2(4, 4), new Color(1f, 1f, 0.7f, 0.8f));
            }
        }
        
        private static GameObject CreateSword(Transform parent, string name, Vector2 position, float rotation, IconStyle style)
        {
            GameObject sword = new GameObject(name);
            sword.transform.SetParent(parent, false);
            
            RectTransform swordRect = sword.AddComponent<RectTransform>();
            swordRect.anchorMin = position;
            swordRect.anchorMax = position;
            swordRect.sizeDelta = new Vector2(40, 40);
            swordRect.localRotation = Quaternion.Euler(0, 0, rotation);
            
            // Blade
            GameObject blade = CreateRoundedRect(sword.transform, "Blade",
                new Vector2(0.5f, 0.65f), new Vector2(6, 28), style.primaryColor);
            
            // Blade shine
            GameObject bladeShine = CreateRoundedRect(blade.transform, "BladeShine",
                new Vector2(0.3f, 0.5f), new Vector2(2, 24), new Color(1f, 1f, 1f, 0.4f));
            
            // Guard
            GameObject guard = CreateRoundedRect(sword.transform, "Guard",
                new Vector2(0.5f, 0.35f), new Vector2(16, 4), style.secondaryColor);
            
            // Handle
            GameObject handle = CreateRoundedRect(sword.transform, "Handle",
                new Vector2(0.5f, 0.2f), new Vector2(8, 12), style.secondaryColor);
            
            // Pommel
            GameObject pommel = CreateCircle(sword.transform, "Pommel",
                new Vector2(0.5f, 0.1f), new Vector2(8, 8), style.primaryColor);
            
            return sword;
        }
        
        private static void CreateMinigameIcon(GameObject container, IconStyle style)
        {
            // Create dice shape (rotated square)
            GameObject dice = CreateRoundedRect(container.transform, "Dice",
                new Vector2(0.5f, 0.5f), new Vector2(30, 30), style.primaryColor);
            dice.transform.localRotation = Quaternion.Euler(0, 0, 45);
            
            // Add gradient overlay
            if (style.useGradient)
            {
                GameObject diceGradient = CreateRoundedRect(dice.transform, "DiceGradient",
                    new Vector2(0.5f, 0.7f), new Vector2(28, 14), new Color(1f, 1f, 1f, 0.2f));
            }
            
            // Add dice dots with glow
            Vector2[] dotPositions = new Vector2[]
            {
                new Vector2(0.3f, 0.7f),
                new Vector2(0.7f, 0.7f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.3f, 0.3f),
                new Vector2(0.7f, 0.3f)
            };
            
            foreach (var pos in dotPositions)
            {
                GameObject dot = CreateCircle(dice.transform, "Dot", pos, new Vector2(4, 4), style.secondaryColor);
                
                if (style.useGlow)
                {
                    GameObject dotGlow = CreateCircle(dot.transform, "DotGlow",
                        new Vector2(0.5f, 0.5f), new Vector2(6, 6), new Color(1f, 1f, 1f, 0.3f));
                }
            }
        }
        
        private static void CreateShopIcon(GameObject container, IconStyle style)
        {
            // Create cart body
            GameObject cartBody = CreateRoundedRect(container.transform, "CartBody",
                new Vector2(0.5f, 0.4f), new Vector2(30, 20), style.primaryColor);
            
            // Add cart gradient
            if (style.useGradient)
            {
                GameObject cartShine = CreateRoundedRect(cartBody.transform, "CartShine",
                    new Vector2(0.5f, 0.7f), new Vector2(26, 8), new Color(1f, 1f, 1f, 0.2f));
            }
            
            // Create handle
            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(container.transform, false);
            
            // Handle is made of two parts - vertical and diagonal
            GameObject handleVertical = CreateRoundedRect(handle.transform, "HandleVertical",
                new Vector2(0.75f, 0.55f), new Vector2(3, 10), style.secondaryColor);
            
            GameObject handleDiagonal = CreateRoundedRect(handle.transform, "HandleDiagonal",
                new Vector2(0.85f, 0.65f), new Vector2(12, 3), style.secondaryColor);
            handleDiagonal.transform.localRotation = Quaternion.Euler(0, 0, -30);
            
            // Create wheels with metallic effect
            GameObject wheel1 = CreateCircle(container.transform, "Wheel1",
                new Vector2(0.35f, 0.2f), new Vector2(8, 8), style.secondaryColor);
            GameObject wheel1Inner = CreateCircle(wheel1.transform, "WheelInner",
                new Vector2(0.5f, 0.5f), new Vector2(4, 4), style.primaryColor);
            
            GameObject wheel2 = CreateCircle(container.transform, "Wheel2",
                new Vector2(0.65f, 0.2f), new Vector2(8, 8), style.secondaryColor);
            GameObject wheel2Inner = CreateCircle(wheel2.transform, "WheelInner",
                new Vector2(0.5f, 0.5f), new Vector2(4, 4), style.primaryColor);
            
            // Add items in cart (simple shapes)
            GameObject item1 = CreateCircle(cartBody.transform, "Item1",
                new Vector2(0.3f, 0.7f), new Vector2(8, 8), new Color(0.9f, 0.5f, 0.5f, 1f));
            GameObject item2 = CreateRoundedRect(cartBody.transform, "Item2",
                new Vector2(0.6f, 0.7f), new Vector2(10, 8), new Color(0.5f, 0.9f, 0.5f, 1f));
        }
        
        private static GameObject CreateCircle(Transform parent, string name, Vector2 anchorPos, Vector2 size, Color color)
        {
            GameObject circle = new GameObject(name);
            circle.transform.SetParent(parent, false);
            
            RectTransform rect = circle.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            
            Image image = circle.AddComponent<Image>();
            image.color = color;
            
            // Create circular sprite
            Texture2D circleTex = CreateCircleTexture(32);
            image.sprite = Sprite.Create(circleTex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            
            return circle;
        }
        
        private static GameObject CreateRoundedRect(Transform parent, string name, Vector2 anchorPos, Vector2 size, Color color)
        {
            GameObject rect = new GameObject(name);
            rect.transform.SetParent(parent, false);
            
            RectTransform rectTransform = rect.AddComponent<RectTransform>();
            rectTransform.anchorMin = anchorPos;
            rectTransform.anchorMax = anchorPos;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = size;
            
            Image image = rect.AddComponent<Image>();
            image.color = color;
            
            return rect;
        }
        
        private static GameObject CreateTriangle(Transform parent, string name, Vector2 anchorPos, Vector2 size, Color color)
        {
            GameObject triangle = new GameObject(name);
            triangle.transform.SetParent(parent, false);
            
            RectTransform rect = triangle.AddComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            
            Image image = triangle.AddComponent<Image>();
            image.color = color;
            
            // Create triangle texture
            Texture2D triangleTex = CreateTriangleTexture(64);
            image.sprite = Sprite.Create(triangleTex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
            
            return triangle;
        }
        
        private static Texture2D CreateCircleTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size);
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;
            
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance <= radius)
                    {
                        float alpha = 1f - Mathf.Max(0, (distance - radius * 0.8f) / (radius * 0.2f));
                        texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            
            texture.Apply();
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }
        
        private static Texture2D CreateTriangleTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size);
            
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float normalizedX = x / (float)size;
                    float normalizedY = y / (float)size;
                    
                    // Triangle shape: y > 1 - 2*|x - 0.5|
                    float triangleY = 1f - 2f * Mathf.Abs(normalizedX - 0.5f);
                    
                    if (normalizedY > triangleY)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            
            texture.Apply();
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }
        
        private static Texture2D CreateRadialGradientTexture(int size, Color color)
        {
            Texture2D texture = new Texture2D(size, size);
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;
            
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = 1f - Mathf.Clamp01(distance / radius);
                    alpha = Mathf.Pow(alpha, 2f); // Quadratic falloff for softer glow
                    
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * alpha));
                }
            }
            
            texture.Apply();
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }
    }
}