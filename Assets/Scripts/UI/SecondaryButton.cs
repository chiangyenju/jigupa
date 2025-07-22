using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class SecondaryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Colors")]
    [SerializeField] private Color primaryColor = new Color(0.95f, 0.95f, 0.95f, 1f); // Light gray
    [SerializeField] private Color borderColor = new Color(0.674f, 0.035f, 0.161f, 1f); // #ac0929
    [SerializeField] private Color hoverColor = new Color(1f, 0.98f, 0.98f, 1f); // Slight pink tint
    [SerializeField] private Color textColor = new Color(0.674f, 0.035f, 0.161f, 1f); // #ac0929
    [SerializeField] private Color disabledColor = new Color(0.8f, 0.8f, 0.8f, 0.6f);
    
    [Header("Border Settings")]
    [SerializeField] private bool useBorder = true;
    [SerializeField] private float borderWidth = 3f;
    
    [Header("Animation")]
    [SerializeField] private float scaleOnHover = 1.06f;
    [SerializeField] private float scaleOnPress = 0.96f;
    [SerializeField] private bool pulseOnHover = true;
    
    private Button button;
    private Image buttonImage;
    private TextMeshProUGUI buttonText;
    private RectTransform rectTransform;
    
    // Border element
    private GameObject borderObject;
    private Image borderImage;
    
    private bool isHovering = false;
    private bool isPressed = false;
    private Vector3 originalScale;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = transform.localScale;
        
        SetupStyle();
        CreateBorder();
        UpdateButtonState();
    }
    
    private void SetupStyle()
    {
        buttonImage.color = primaryColor;
        buttonImage.type = Image.Type.Sliced;
        
        if (buttonText != null)
        {
            buttonText.color = textColor;
            buttonText.fontSize = 24;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.alignment = TextAlignmentOptions.Center;
        }
    }
    
    private void CreateBorder()
    {
        if (!useBorder) return;
        
        // Create border behind button at parent level
        borderObject = new GameObject($"{gameObject.name}_Border");
        borderObject.transform.SetParent(transform.parent, false);
        borderObject.transform.SetSiblingIndex(transform.GetSiblingIndex());
        
        var borderRect = borderObject.AddComponent<RectTransform>();
        borderRect.anchorMin = rectTransform.anchorMin;
        borderRect.anchorMax = rectTransform.anchorMax;
        borderRect.anchoredPosition = rectTransform.anchoredPosition;
        borderRect.sizeDelta = rectTransform.sizeDelta + new Vector2(borderWidth * 2, borderWidth * 2);
        
        borderImage = borderObject.AddComponent<Image>();
        borderImage.color = borderColor;
        borderImage.sprite = buttonImage.sprite;
        borderImage.type = Image.Type.Sliced;
        borderImage.raycastTarget = false;
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
        Color targetColor = primaryColor;
        Vector3 targetScale = originalScale;
        float targetBorderWidth = borderWidth;
        
        if (!button.interactable)
        {
            targetColor = disabledColor;
            if (buttonText != null) buttonText.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);
            if (borderImage != null) borderImage.color = new Color(borderColor.r, borderColor.g, borderColor.b, 0.3f);
        }
        else if (isPressed)
        {
            targetScale = originalScale * scaleOnPress;
            targetColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            targetBorderWidth = borderWidth * 1.2f;
        }
        else if (isHovering)
        {
            targetScale = originalScale * scaleOnHover;
            targetColor = hoverColor;
            
            // Cute pulse effect
            if (pulseOnHover)
            {
                float pulse = Mathf.Sin(Time.time * 6f) * 0.02f + 1f;
                targetScale = originalScale * scaleOnHover * pulse;
                
                // Animated border
                float borderPulse = Mathf.Sin(Time.time * 6f) * 0.5f + borderWidth;
                targetBorderWidth = borderPulse;
            }
        }
        else
        {
            if (buttonText != null) buttonText.color = textColor;
            if (borderImage != null) borderImage.color = borderColor;
        }
        
        // Apply changes
        buttonImage.color = targetColor;
        transform.localScale = targetScale;
        
        // Update border size
        if (borderObject != null)
        {
            var borderRect = borderObject.GetComponent<RectTransform>();
            borderRect.sizeDelta = new Vector2(targetBorderWidth * 2, targetBorderWidth * 2);
        }
    }
    
    private void OnEnable()
    {
        UpdateButtonState();
    }
    
    private void OnDisable()
    {
        isHovering = false;
        isPressed = false;
    }
    
    private void OnDestroy()
    {
        if (borderObject != null)
        {
            Destroy(borderObject);
        }
    }
    
    public void SetText(string text)
    {
        if (buttonText != null)
        {
            buttonText.text = text;
        }
    }
    
    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
        UpdateButtonState();
    }
}