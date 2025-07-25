using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Jigupa.UI
{
    [RequireComponent(typeof(Image))]
    public class BattleFistIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image fistImage;
        private Vector3 originalScale;
        private Vector2 originalAnchoredPosition = new Vector2(0, 0); // Centered position
        private Color originalColor;
        private Button battleButton;
        private TextMeshProUGUI buttonText;
        private List<GameObject> textFragments = new List<GameObject>();
        private List<GameObject> allEffects = new List<GameObject>(); // Track all effects for cleanup
        
        [Header("Animation Settings")]
        [SerializeField] private float hoverScale = 1.1f;
        
        [Header("Breathing Animation")]
        [SerializeField] private float breathScale = 1.05f;
        [SerializeField] private float breathSpeed = 1.5f;
        private bool isBreathing = true;
        [SerializeField] private float clickScale = 12f; // Grows to almost full screen width
        [SerializeField] private float animationDuration = 0.25f; // Even faster punch
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Color Settings")]
        [SerializeField] private Color hoverColor = new Color(0.674f, 0.035f, 0.161f); // Primary red
        [SerializeField] private Color clickColor = new Color(0.9f, 0.1f, 0.2f); // Brighter red
        
        private Coroutine currentAnimation;
        private bool isAnimating = false;

        void Awake()
        {
            fistImage = GetComponent<Image>();
            if (fistImage != null)
            {
                originalColor = fistImage.color;
                // Ensure the image is enabled
                fistImage.enabled = true;
            }
            
            // Also try to get the button reference early in Awake
            StartCoroutine(SetupButtonListenerDelayed());
        }
        
        void Start()
        {
            // Ensure image is visible
            if (fistImage == null)
            {
                fistImage = GetComponent<Image>();
            }
            
            if (fistImage != null)
            {
                fistImage.enabled = true;
                // Capture color if not already set
                if (originalColor.a == 0)
                {
                    originalColor = fistImage.color;
                }
            }
            
            // Capture original scale
            originalScale = transform.localScale;
            
            // Capture current position (don't override it)
            RectTransform rect = GetComponent<RectTransform>();
            if (rect != null)
            {
                originalAnchoredPosition = rect.anchoredPosition;
            }
            
            // Get the button component on this GameObject (fist is now the button)
            battleButton = GetComponent<Button>();
            if (battleButton != null)
            {
                battleButton.onClick.AddListener(OnBattleButtonClick);
                Debug.Log("BattleFistIcon: Fist is now directly clickable");
            }
            else
            {
                Debug.LogWarning("BattleFistIcon: No Button component found on fist");
            }
            
            // Start breathing animation
            isBreathing = true;
            StartCoroutine(StartBreathingDelayed());
        }
        
        private IEnumerator SetupButtonListenerDelayed()
        {
            // Wait a frame to ensure all components are initialized
            yield return null;
            
            // Try to set up the button listener again if it wasn't found in Start
            if (battleButton == null)
            {
                battleButton = GetComponent<Button>();
                if (battleButton != null)
                {
                    battleButton.onClick.AddListener(OnBattleButtonClick);
                    Debug.Log("BattleFistIcon: Setup button listener in coroutine");
                }
            }
        }
        
        void OnEnable()
        {
            // Reset animation state
            isAnimating = false;
            StopAllCoroutines();
            
            // Make sure we have the image component
            if (fistImage == null)
            {
                fistImage = GetComponent<Image>();
                if (fistImage != null && originalColor == Color.clear)
                {
                    originalColor = fistImage.color;
                }
            }
            
            // Reset transform - but only if we have valid original values
            if (originalScale != Vector3.zero)
            {
                transform.localScale = originalScale;
            }
            else
            {
                // Capture current scale if not set
                originalScale = transform.localScale;
            }
            
            RectTransform rect = GetComponent<RectTransform>();
            if (rect != null)
            {
                // Only reset position if we have a valid original position
                if (originalAnchoredPosition != Vector2.zero || 
                    (originalAnchoredPosition == Vector2.zero && !Application.isPlaying))
                {
                    rect.anchoredPosition = originalAnchoredPosition;
                }
                else
                {
                    // Capture current position
                    originalAnchoredPosition = rect.anchoredPosition;
                }
            }
            
            transform.rotation = Quaternion.identity;
            
            // Reset color and ensure visibility
            if (fistImage != null)
            {
                // Make sure we have a valid original color
                if (originalColor.a > 0)
                {
                    fistImage.color = originalColor;
                }
                else
                {
                    // Default to white with full alpha if color not set
                    fistImage.color = Color.white;
                    originalColor = Color.white;
                }
                fistImage.enabled = true; // Ensure it's visible
            }
            
            // Clean up removed - explosion effects no longer used
            
            // Start breathing animation after delay
            isBreathing = true;
            StartCoroutine(StartBreathingDelayed());
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log($"[FIST ANIMATION] OnPointerEnter - isAnimating: {isAnimating}");
            
            if (!isAnimating)
            {
                // Don't stop breathing, just overlay hover effect
                StopCurrentAnimation();
                currentAnimation = StartCoroutine(AnimateHover());
                
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Debug.Log($"[FIST ANIMATION] OnPointerExit - isAnimating: {isAnimating}");
            
            if (!isAnimating)
            {
                StopCurrentAnimation();
                currentAnimation = StartCoroutine(AnimateToOriginal());
                // Breathing continues automatically
                
            }
        }

        private void OnBattleButtonClick()
        {
            Debug.Log($"[FIST ANIMATION] OnBattleButtonClick called! isAnimating: {isAnimating}");
            
            if (!isAnimating)
            {
                isBreathing = false;
                StopCurrentAnimation();
                currentAnimation = StartCoroutine(AnimatePunch());
                
            }
            else
            {
                // Debug.Log("[FIST ANIMATION] Animation already in progress");
            }
        }

        private void StopCurrentAnimation()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }
        }

        private IEnumerator AnimateHover()
        {
            float elapsed = 0;
            Vector3 startScale = transform.localScale; // Start from current breathing scale
            Color startColor = fistImage.color;
            Vector3 baseScale = originalScale * hoverScale;

            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.2f;
                
                // Smoothly transition to hover color
                fistImage.color = Color.Lerp(startColor, hoverColor, t);
                
                yield return null;
            }

            fistImage.color = hoverColor;
        }

        private IEnumerator AnimateToOriginal()
        {
            float elapsed = 0;
            Color startColor = fistImage.color;

            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.2f;
                
                // Only animate color back, let breathing handle scale
                fistImage.color = Color.Lerp(startColor, originalColor, t);
                
                yield return null;
            }

            fistImage.color = originalColor;
        }

        private IEnumerator AnimatePunch()
        {
            Debug.Log("[FIST ANIMATION] AnimatePunch started!");
            isAnimating = true;
            
            // First hide all UI except the fist
            yield return StartCoroutine(HideAllUIExceptFist());
            
            // Get rect transform for position manipulation
            RectTransform rectTransform = GetComponent<RectTransform>();
            
            // Start from current state - no reset needed
            Vector3 currentScale = transform.localScale;
            Vector2 currentPosition = rectTransform.anchoredPosition;
            Color currentColor = fistImage.color;
            
            yield return new WaitForSeconds(0.1f); // Shorter pause after UI hides
            
            float elapsed = 0;
            Vector3 startScale = currentScale; // Use current scale instead of original
            Vector2 startPosition = currentPosition; // Use current position
            Color startColor = currentColor; // Use current color
            Vector3 targetScale = originalScale * clickScale;
            
            // Quick pull back first (wind up) - from current scale
            float windupDuration = 0.08f; // Slightly faster windup
            Vector3 windupScale = startScale * 0.9f; // Less dramatic pullback
            while (elapsed < windupDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / windupDuration;
                
                // Pull back slightly from current scale
                transform.localScale = Vector3.Lerp(startScale, windupScale, t);
                rectTransform.anchoredPosition = startPosition + Vector2.up * 15f * t; // Less movement
                
                yield return null;
            }
            
            // Reset elapsed for punch forward
            elapsed = 0;
            Vector3 punchStartScale = transform.localScale;
            Vector2 punchStartPos = rectTransform.anchoredPosition;
            
            // Animate punch forward - fast and dramatic with fade out
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = scaleCurve.Evaluate(elapsed / animationDuration);
                
                // Scale up dramatically
                transform.localScale = Vector3.Lerp(punchStartScale, targetScale, t);
                
                // Move forward and down as if coming at the viewer
                rectTransform.anchoredPosition = Vector2.Lerp(punchStartPos, startPosition + Vector2.down * 80f, t);
                
                // Fade out as it gets larger - maintaining current color tone
                Color fadeColor = startColor;
                fadeColor.a = Mathf.Lerp(startColor.a, 0f, t); // Fade from current alpha to transparent
                fistImage.color = fadeColor;
                
                // Add slight rotation for dramatic effect
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(t * Mathf.PI) * -15f);
                
                yield return null;
            }
            
            // Fist is now fully faded out
            transform.localScale = targetScale;
            rectTransform.anchoredPosition = startPosition + Vector2.down * 80f;
            fistImage.color = new Color(clickColor.r, clickColor.g, clickColor.b, 0f);
            
            // Brief pause before scene transition
            yield return new WaitForSeconds(0.2f);
            
            // Trigger scene transition (no black fade needed)
            // PlayJigupaButton is on the same GameObject as the fist
            PlayJigupaButton playButton = GetComponent<PlayJigupaButton>();
            if (playButton != null)
            {
                playButton.LoadBattleScene();
            }
            else
            {
                Debug.LogWarning("PlayJigupaButton not found on fist icon");
            }
            
            // Reset animation state
            isAnimating = false;
        }

        // Explosion methods removed - no longer used
        /*
        private void ExplodeText()
        {
            if (buttonText == null)
            {
                // Debug.LogWarning("BattleFistIcon: buttonText is null, cannot explode text");
                return;
            }
            
            // Debug.Log($"Exploding text: '{buttonText.text}'");
            
            // Hide original text
            buttonText.enabled = false;
            
            // Get text info
            string text = buttonText.text;
            Vector3 textPosition = buttonText.transform.position;
            float fontSize = buttonText.fontSize;
            
            // Create additional particle effects
            CreateExplosionParticles(buttonText.transform.position);
            
            // Create impact rings
            CreateImpactRings(buttonText.transform.position);
            
            // Create individual letter fragments
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ') continue; // Skip spaces
                
                // Create fragment
                GameObject fragment = new GameObject($"Fragment_{text[i]}");
                
                // Parent to canvas to ensure proper UI rendering
                Canvas canvas = buttonText.GetComponentInParent<Canvas>();
                fragment.transform.SetParent(canvas.transform, false);
                
                // Set sibling index to render on top of everything
                fragment.transform.SetAsLastSibling();
                
                // Add RectTransform first
                RectTransform rect = fragment.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(fontSize * 2, fontSize * 2);
                
                // Position relative to button text with Z offset to be in front of fist
                RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
                Vector3 worldPos = buttonTextRect.TransformPoint(new Vector3((i - text.Length/2f) * fontSize * 0.6f, 0, -10));
                rect.position = worldPos;
                
                // Add text component
                TextMeshProUGUI fragmentText = fragment.AddComponent<TextMeshProUGUI>();
                fragmentText.text = text[i].ToString();
                fragmentText.font = buttonText.font;
                fragmentText.fontSize = fontSize * 2.5f; // Much bigger fragments
                fragmentText.color = buttonText.color;
                fragmentText.alignment = TextAlignmentOptions.Center;
                fragmentText.fontStyle = TMPro.FontStyles.Bold; // Make fragments bold
                
                // Add fragment to list
                textFragments.Add(fragment);
                allEffects.Add(fragment); // Track for cleanup
                
                // Start explosion animation
                StartCoroutine(AnimateFragment(fragment, i));
            }
        }
        
        private IEnumerator AnimateFragment(GameObject fragment, int index)
        {
            if (fragment == null) yield break;
            
            Vector3 startPos = fragment.transform.position;
            
            // EXTREME explosion - fragments scatter everywhere
            float baseAngle = Random.Range(0f, Mathf.PI * 2f); // Completely random directions
            float spread = Random.Range(0.7f, 1.3f);
            Vector3 randomDirection = new Vector3(
                Mathf.Cos(baseAngle) * spread,
                Mathf.Sin(baseAngle) * spread + Random.Range(0.2f, 1.5f), // Strong upward bias
                0
            ).normalized;
            
            float explosionForce = Random.Range(2000f, 3500f); // EXTREME force
            float rotationSpeed = Random.Range(-1440f, 1440f); // Super fast rotation
            float lifetime = 2.0f; // Much longer lifetime
            float elapsed = 0;
            
            TextMeshProUGUI fragmentText = fragment.GetComponent<TextMeshProUGUI>();
            Color startColor = fragmentText.color;
            
            // Initial impulse - make fragments BURST
            fragment.transform.localScale = Vector3.one * Random.Range(2f, 3f);
            
            // Random initial rotation
            fragment.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            
            while (elapsed < lifetime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lifetime;
                
                // EXTREME physics - fragments fly way out
                float easeOut = 1f - Mathf.Pow(1f - t, 3f); // Cubic ease out for more punch
                Vector3 displacement = randomDirection * explosionForce * easeOut;
                displacement.y -= 800f * t * t; // Even stronger gravity
                fragment.transform.position = startPos + displacement;
                
                // Rotation with acceleration
                float rotAccel = t * t; // Accelerating rotation
                fragment.transform.rotation = Quaternion.Euler(0, 0, rotationSpeed * rotAccel);
                
                // Chaotic scaling with multiple bounces
                float bounceScale = 1f + Mathf.Sin(t * Mathf.PI * 6f) * 0.4f * (1f - t);
                float wobble = Mathf.Sin(elapsed * 20f) * 0.1f * (1f - t);
                float scale = (1f - (t * 0.6f)) * bounceScale + wobble;
                fragment.transform.localScale = Vector3.one * scale * Random.Range(0.9f, 1.1f);
                
                // Flash and fade effect
                if (fragmentText != null)
                {
                    float alpha = t < 0.8f ? 1f : (1f - ((t - 0.8f) / 0.2f));
                    float flash = Mathf.Sin(elapsed * 30f) * 0.3f + 0.7f; // Flickering effect
                    fragmentText.color = new Color(
                        startColor.r * (1f + flash * 0.3f), 
                        startColor.g * (1f + flash * 0.3f), 
                        startColor.b * (1f + flash * 0.3f), 
                        alpha
                    );
                }
                
                yield return null;
            }
            
            // Destroy fragment
            Destroy(fragment);
        }
        
        private void CleanupFragments()
        {
            foreach (var fragment in textFragments)
            {
                if (fragment != null)
                    Destroy(fragment);
            }
            textFragments.Clear();
            
            // Clean up all effects
            foreach (var effect in allEffects)
            {
                if (effect != null)
                    Destroy(effect);
            }
            allEffects.Clear();
        }
        
        void OnDestroy()
        {
            if (battleButton != null)
            {
                battleButton.onClick.RemoveListener(OnBattleButtonClick);
            }
        }
        
        private IEnumerator ScreenShake()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) yield break;
            
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector3 originalPos = canvasRect.position;
            
            float duration = 0.5f; // Longer shake
            float magnitude = 30f; // More intense shake
            float elapsed = 0;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float strength = 1f - (elapsed / duration);
                
                float x = Random.Range(-1f, 1f) * magnitude * strength;
                float y = Random.Range(-1f, 1f) * magnitude * strength;
                
                canvasRect.position = originalPos + new Vector3(x, y, 0);
                
                yield return null;
            }
            
            canvasRect.position = originalPos;
        }
        
        private void CreateExplosionParticles(Vector3 position)
        {
            Canvas canvas = buttonText.GetComponentInParent<Canvas>();
            if (canvas == null) return;
            
            // Create many more particles for chaos
            int particleCount = 30;
            for (int i = 0; i < particleCount; i++)
            {
                GameObject particle = new GameObject($"Particle_{i}");
                particle.transform.SetParent(canvas.transform, false);
                particle.transform.SetAsLastSibling();
                allEffects.Add(particle); // Track for cleanup
                
                RectTransform rect = particle.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(Random.Range(15f, 40f), Random.Range(15f, 40f)); // Varied particle sizes
                rect.position = position;
                
                // Create a simple colored square as particle
                UnityEngine.UI.Image particleImage = particle.AddComponent<UnityEngine.UI.Image>();
                particleImage.color = Random.Range(0, 2) == 0 ? 
                    new Color(0.674f, 0.035f, 0.161f, 1f) : // Primary red
                    new Color(0.9f, 0.1f, 0.2f, 1f); // Bright red
                
                // Animate particle
                StartCoroutine(AnimateParticle(particle, i));
            }
        }
        
        private IEnumerator AnimateParticle(GameObject particle, int index)
        {
            if (particle == null) yield break;
            
            Vector3 startPos = particle.transform.position;
            float angle = (index / 12f) * Mathf.PI * 2f;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;
            
            float speed = Random.Range(2500f, 4000f); // Much faster particles
            float lifetime = 1.5f; // Longer particle lifetime
            float elapsed = 0;
            
            UnityEngine.UI.Image image = particle.GetComponent<UnityEngine.UI.Image>();
            Color startColor = image.color;
            
            while (elapsed < lifetime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lifetime;
                
                // Move outward
                Vector3 displacement = direction * speed * t;
                particle.transform.position = startPos + displacement;
                
                // Shrink and fade
                float scale = (1f - t) * 0.5f;
                particle.transform.localScale = Vector3.one * scale;
                
                if (image != null)
                {
                    image.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
                }
                
                yield return null;
            }
            
            Destroy(particle);
        }
        
        private void CreateImpactRings(Vector3 position)
        {
            Canvas canvas = buttonText.GetComponentInParent<Canvas>();
            if (canvas == null) return;
            
            // Create expanding rings
            for (int i = 0; i < 3; i++)
            {
                GameObject ring = new GameObject($"ImpactRing_{i}");
                ring.transform.SetParent(canvas.transform, false);
                ring.transform.SetAsFirstSibling(); // Rings behind everything
                
                RectTransform rect = ring.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(100, 100);
                rect.position = position;
                
                UnityEngine.UI.Image ringImage = ring.AddComponent<UnityEngine.UI.Image>();
                ringImage.color = new Color(0.9f, 0.1f, 0.2f, 0.3f); // Semi-transparent red
                ringImage.raycastTarget = false;
                
                // Create a circle sprite (using a white square with rounded corners)
                ringImage.sprite = null; // Will just use the square
                
                StartCoroutine(AnimateRing(ring, i * 0.1f)); // Stagger the rings
            }
        }
        
        private IEnumerator AnimateRing(GameObject ring, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (ring == null) yield break;
            
            float duration = 1f;
            float elapsed = 0;
            RectTransform rect = ring.GetComponent<RectTransform>();
            UnityEngine.UI.Image image = ring.GetComponent<UnityEngine.UI.Image>();
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Expand rapidly
                float scale = Mathf.Lerp(1f, 8f, t);
                rect.sizeDelta = new Vector2(100 * scale, 100 * scale);
                
                // Fade out
                if (image != null)
                {
                    float alpha = Mathf.Lerp(0.3f, 0f, t);
                    image.color = new Color(0.9f, 0.1f, 0.2f, alpha);
                }
                
                yield return null;
            }
            
            Destroy(ring);
        }
        
        private IEnumerator ScreenFlash()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) yield break;
            
            // Create full-screen flash
            GameObject flash = new GameObject("ScreenFlash");
            flash.transform.SetParent(canvas.transform, false);
            flash.transform.SetAsLastSibling();
            
            RectTransform rect = flash.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            UnityEngine.UI.Image flashImage = flash.AddComponent<UnityEngine.UI.Image>();
            flashImage.color = new Color(1f, 1f, 1f, 0.8f); // Bright white flash
            flashImage.raycastTarget = false;
            
            // Animate flash
            float duration = 0.3f;
            float elapsed = 0;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Quick flash in, slow fade out
                float alpha = t < 0.1f ? 
                    Mathf.Lerp(0f, 0.8f, t / 0.1f) : 
                    Mathf.Lerp(0.8f, 0f, (t - 0.1f) / 0.9f);
                    
                flashImage.color = new Color(1f, 1f, 1f, alpha);
                
                yield return null;
            }
            
            Destroy(flash);
        }
        */
        
        private IEnumerator StartBreathingDelayed()
        {
            // Wait a few frames to ensure button text is ready
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(BreathingAnimation());
        }
        
        private IEnumerator BreathingAnimation()
        {
            while (isBreathing)
            {
                // Breathe in
                float elapsed = 0;
                float breathDuration = 1f / breathSpeed;
                Vector3 startScale = transform.localScale;
                Vector3 targetScale = originalScale * breathScale;
                
                while (elapsed < breathDuration && isBreathing)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / breathDuration;
                    // Use sin curve for smooth breathing
                    float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);
                    transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);
                    yield return null;
                }
                
                // Breathe out
                elapsed = 0;
                startScale = transform.localScale;
                
                while (elapsed < breathDuration && isBreathing)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / breathDuration;
                    // Use sin curve for smooth breathing
                    float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);
                    transform.localScale = Vector3.Lerp(startScale, originalScale, smoothT);
                    yield return null;
                }
                
                yield return null;
            }
        }
        
        private IEnumerator HideAllUIExceptFist()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) yield break;
            
            // Find all UI elements
            Image[] allImages = canvas.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] allTexts = canvas.GetComponentsInChildren<TextMeshProUGUI>();
            
            // Fade out all images except fist and background
            foreach (var img in allImages)
            {
                // Skip fist image, self, and background
                bool isBackground = img.gameObject.name == "Background" || 
                                  img.color.r > 0.9f && img.color.g > 0.9f && img.color.b > 0.85f; // Ivory color check
                
                if (img != fistImage && img.gameObject != gameObject && !isBackground)
                {
                    StartCoroutine(FadeGraphic(img, 0f, 0.3f));
                }
            }
            
            // Fade out all texts
            foreach (var text in allTexts)
            {
                StartCoroutine(FadeGraphic(text, 0f, 0.3f));
            }
            
            yield return new WaitForSeconds(0.3f);
        }
        
        private IEnumerator FadeGraphic(Graphic graphic, float targetAlpha, float duration)
        {
            if (graphic == null) yield break;
            
            Color startColor = graphic.color;
            float startAlpha = startColor.a;
            float elapsed = 0;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                Color newColor = startColor;
                newColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
                graphic.color = newColor;
                
                yield return null;
            }
            
            Color finalColor = startColor;
            finalColor.a = targetAlpha;
            graphic.color = finalColor;
        }
        
    }
}