using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class PrimaryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Colors")]
    [SerializeField] private Color primaryColor = new Color(0.674f, 0.035f, 0.161f, 1f); // #ac0929
    [SerializeField] private Color hoverColor = new Color(0.902f, 0.224f, 0.275f, 1f); // #e63946 - Light red
    [SerializeField] private Color pressedColor = new Color(0.564f, 0.0f, 0.051f, 1f); // Darker
    [SerializeField] private Color disabledColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);
    
    // Glare effect removed
    
    [Header("Border")]
    [SerializeField] private bool showBorder = true;
    [SerializeField] private Color borderColor = new Color(0.674f, 0.035f, 0.161f, 1f); // Primary red
    [SerializeField] private float borderThickness = 2f;
    
    [Header("Animation")]
    [SerializeField] private float scaleOnHover = 1.08f;
    [SerializeField] private float scaleOnPress = 0.95f;
    [SerializeField] private float animationDuration = 0.15f;
    [SerializeField] private bool bounceOnHover = true;
    
    private Button button;
    private Image buttonImage;
    private TextMeshProUGUI buttonText;
    private RectTransform rectTransform;
    
    private bool isHovering = false;
    private bool isPressed = false;
    private Vector3 originalScale;
    private Coroutine animationCoroutine;
    private Coroutine pulseCoroutine;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = transform.localScale;
        
        SetupStyle();
        UpdateButtonState();
    }
    
    private void Start()
    {
        // Start constant pulse animation
        StartCoroutine(ConstantPulseAnimation());
    }
    
    private void SetupStyle()
    {
        // Make button transparent to show only text like menu items
        buttonImage.color = new Color(0, 0, 0, 0);
        buttonImage.type = Image.Type.Sliced;
        
        if (buttonText != null)
        {
            buttonText.color = primaryColor; // Text is colored, not background
            buttonText.fontSize = 60; // Match menu item size
            buttonText.fontWeight = TMPro.FontWeight.Black; // 900 weight
            buttonText.characterSpacing = -15; // Tight spacing like menu
            buttonText.alignment = TextAlignmentOptions.Center;
            
            // Apply Lexend font directly (FontManager is static)
            Jigupa.UI.FontManager.ApplyLexendFont(buttonText, TMPro.FontWeight.Black);
        }
        
        // Add border/outline
        CreateBorder();
    }
    
    private void CreateBorder()
    {
        if (!showBorder) return;
        
        // Create border as a separate image behind the button
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(transform, false);
        borderObj.transform.SetAsFirstSibling(); // Put border behind everything
        
        RectTransform borderRect = borderObj.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(borderThickness * 2, borderThickness * 2); // Border thickness on all sides
        borderRect.anchoredPosition = Vector2.zero;
        
        // Add border image
        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = borderColor;
        borderImage.raycastTarget = false;
        
        // Create inner cutout for transparent center
        GameObject innerCutout = new GameObject("InnerCutout");
        innerCutout.transform.SetParent(borderObj.transform, false);
        
        RectTransform innerRect = innerCutout.AddComponent<RectTransform>();
        innerRect.anchorMin = Vector2.zero;
        innerRect.anchorMax = Vector2.one;
        innerRect.sizeDelta = new Vector2(-borderThickness * 2, -borderThickness * 2);
        innerRect.anchoredPosition = Vector2.zero;
        
        Image innerImage = innerCutout.AddComponent<Image>();
        innerImage.color = new Color(0.961f, 0.949f, 0.91f, 1f); // Ivory background color
        innerImage.raycastTarget = false;
    }
    
    private IEnumerator ConstantPulseAnimation()
    {
        float pulseSpeed = 2f; // Speed of pulsing
        float pulseAmount = 0.1f; // 10% scale variation
        
        while (true)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = originalScale * scale;
            yield return null;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;
        isHovering = true;
        UpdateButtonState();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isPressed = false;
        UpdateButtonState();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        isPressed = true;
        UpdateButtonState();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        UpdateButtonState();
    }
    
    private void UpdateButtonState()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateButtonState());
    }
    
    private IEnumerator AnimateButtonState()
    {
        if (buttonText == null) yield break;
        
        Color startColor = buttonText.color;
        Color targetColor = primaryColor;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = originalScale;
        
        if (!button.interactable)
        {
            targetColor = disabledColor;
        }
        else if (isPressed)
        {
            targetColor = pressedColor;
            targetScale = originalScale * scaleOnPress;
        }
        else if (isHovering)
        {
            targetColor = hoverColor;
            targetScale = originalScale * scaleOnHover;
        }
        
        float elapsed = 0;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            buttonText.color = Color.Lerp(startColor, targetColor, smoothT);
            transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);
            
            // Add bounce effect on hover
            if (isHovering && bounceOnHover && !isPressed)
            {
                float bounce = Mathf.Sin(Time.time * 8f) * 0.02f + 1f;
                transform.localScale = targetScale * bounce;
            }
            
            yield return null;
        }
        
        buttonText.color = targetColor;
        transform.localScale = targetScale;
    }
    
    // Update removed - constant pulse is handled in coroutine
    
    private void OnEnable()
    {
        UpdateButtonState();
    }
    
    private void OnDisable()
    {
        isHovering = false;
        isPressed = false;
        
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
    }
    
    private void OnDestroy()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
    }
    
    public void SetText(string text)
    {
        if (buttonText != null)
        {
            buttonText.text = text;
            Debug.Log($"PrimaryButton.SetText: '{text}' - buttonText component exists: {buttonText != null}");
        }
        else
        {
            Debug.LogWarning($"PrimaryButton.SetText: buttonText is null, cannot set text to '{text}'");
        }
    }
    
    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
        UpdateButtonState();
    }
}