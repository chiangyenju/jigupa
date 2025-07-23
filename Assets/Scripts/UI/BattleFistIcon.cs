using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using System.Collections.Generic;

namespace Jigupa.UI
{
    [RequireComponent(typeof(Image))]
    public class BattleFistIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image fistImage;
        private Vector3 originalScale;
        private Vector2 originalAnchoredPosition = new Vector2(0, 120); // Store original position
        private Color originalColor;
        private Button battleButton;
        private TextMeshProUGUI buttonText;
        private List<GameObject> textFragments = new List<GameObject>();
        private List<GameObject> allEffects = new List<GameObject>(); // Track all effects for cleanup
        
        [Header("Animation Settings")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float clickScale = 4.5f; // Much larger for dramatic punch
        [SerializeField] private float animationDuration = 0.3f; // Faster punch
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Color Settings")]
        [SerializeField] private Color hoverColor = new Color(0.674f, 0.035f, 0.161f); // Primary red
        [SerializeField] private Color clickColor = new Color(0.9f, 0.1f, 0.2f); // Brighter red
        
        private Coroutine currentAnimation;
        private bool isAnimating = false;

        void Awake()
        {
            fistImage = GetComponent<Image>();
            originalColor = fistImage.color;
            
            // Also try to get the button reference early in Awake
            StartCoroutine(SetupButtonListenerDelayed());
        }
        
        void Start()
        {
            // Capture original scale and position after any setup
            originalScale = transform.localScale;
            RectTransform rect = GetComponent<RectTransform>();
            if (rect != null)
            {
                originalAnchoredPosition = rect.anchoredPosition;
            }
            
            // Find the battle button (it's a sibling, not a child of parent)
            Transform parent = transform.parent;
            if (parent != null)
            {
                battleButton = parent.Find("PlayJigupaButton")?.GetComponent<Button>();
                if (battleButton != null)
                {
                    battleButton.onClick.AddListener(OnBattleButtonClick);
                    buttonText = battleButton.GetComponentInChildren<TextMeshProUGUI>();
                    // Debug.Log($"BattleFistIcon: Found button: {battleButton.name}, text: {buttonText?.text}");
                    
                    // Log current listeners count for debugging
                    // Debug.Log($"BattleFistIcon: Button now has {battleButton.onClick.GetPersistentEventCount()} persistent listeners");
                }
                else
                {
                    // Debug.LogWarning("BattleFistIcon: Could not find PlayJigupaButton");
                    
                    // Try to find button in different ways for debugging
                    Button foundButton = parent.GetComponentInChildren<Button>();
                    if (foundButton != null)
                    {
                        // Debug.Log($"BattleFistIcon: Found a button named: {foundButton.name}");
                    }
                    
                    // Log all siblings for debugging
                    // Debug.Log($"BattleFistIcon: Parent has {parent.childCount} children:");
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        // Debug.Log($"  - Child {i}: {parent.GetChild(i).name}");
                    }
                }
            }
        }
        
        private IEnumerator SetupButtonListenerDelayed()
        {
            // Wait a frame to ensure all components are initialized
            yield return null;
            
            // Try to set up the button listener again if it wasn't found in Start
            if (battleButton == null)
            {
                Transform parent = transform.parent;
                if (parent != null)
                {
                    battleButton = parent.Find("PlayJigupaButton")?.GetComponent<Button>();
                    if (battleButton != null)
                    {
                        battleButton.onClick.AddListener(OnBattleButtonClick);
                        // Debug.Log($"BattleFistIcon: Setup button listener in coroutine for: {battleButton.name}");
                    }
                }
            }
        }
        
        void OnEnable()
        {
            // Reset animation state
            isAnimating = false;
            StopAllCoroutines();
            
            // Reset transform but keep original anchored position
            transform.localScale = originalScale;
            GetComponent<RectTransform>().anchoredPosition = originalAnchoredPosition;
            transform.rotation = Quaternion.identity;
            
            // Reset color
            if (fistImage != null)
            {
                fistImage.color = originalColor;
            }
            
            // Reset text visibility when returning to menu
            if (buttonText != null)
            {
                buttonText.enabled = true;
            }
            
            // Clean up any leftover fragments
            CleanupFragments();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log($"[FIST ANIMATION] OnPointerEnter - isAnimating: {isAnimating}");
            
            if (!isAnimating)
            {
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
            }
        }

        private void OnBattleButtonClick()
        {
            // Debug.Log($"[FIST ANIMATION] OnBattleButtonClick called! isAnimating: {isAnimating}");
            
            if (!isAnimating)
            {
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
            Vector3 startScale = transform.localScale;
            Color startColor = fistImage.color;
            Vector3 targetScale = originalScale * hoverScale;

            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.2f;
                
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                fistImage.color = Color.Lerp(startColor, hoverColor, t);
                
                yield return null;
            }

            transform.localScale = targetScale;
            fistImage.color = hoverColor;
        }

        private IEnumerator AnimateToOriginal()
        {
            float elapsed = 0;
            Vector3 startScale = transform.localScale;
            Color startColor = fistImage.color;

            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.2f;
                
                transform.localScale = Vector3.Lerp(startScale, originalScale, t);
                fistImage.color = Color.Lerp(startColor, originalColor, t);
                
                yield return null;
            }

            transform.localScale = originalScale;
            fistImage.color = originalColor;
        }

        private IEnumerator AnimatePunch()
        {
            // Debug.Log("[FIST ANIMATION] AnimatePunch started!");
            isAnimating = true;
            
            // Get rect transform for position manipulation
            RectTransform rectTransform = GetComponent<RectTransform>();
            
            // Don't reset position - keep it where it is
            transform.localScale = originalScale;
            transform.rotation = Quaternion.identity;
            fistImage.color = originalColor;
            
            // Debug.Log($"[FIST ANIMATION] Starting from position: {rectTransform.anchoredPosition}");
            
            yield return null; // Wait a frame
            
            float elapsed = 0;
            Vector3 startScale = transform.localScale;
            Vector2 startPosition = originalAnchoredPosition; // Use stored original position
            Color startColor = fistImage.color;
            Vector3 targetScale = originalScale * clickScale;
            
            // Quick pull back first (wind up)
            float windupDuration = 0.1f;
            while (elapsed < windupDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / windupDuration;
                
                // Pull back slightly
                transform.localScale = Vector3.Lerp(startScale, originalScale * 0.8f, t);
                rectTransform.anchoredPosition = startPosition + Vector2.up * 20f * t;
                
                yield return null;
            }
            
            // Reset elapsed for punch forward
            elapsed = 0;
            Vector3 punchStartScale = transform.localScale;
            Vector2 punchStartPos = rectTransform.anchoredPosition;
            
            // Animate punch forward - fast and dramatic
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = scaleCurve.Evaluate(elapsed / animationDuration);
                
                // Scale up dramatically
                transform.localScale = Vector3.Lerp(punchStartScale, targetScale, t);
                
                // Move forward and down as if coming at the viewer
                rectTransform.anchoredPosition = Vector2.Lerp(punchStartPos, startPosition + Vector2.down * 80f, t);
                
                // Color gets more intense
                fistImage.color = Color.Lerp(startColor, clickColor, t);
                
                // Add slight rotation for dramatic effect
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(t * Mathf.PI) * -15f);
                
                yield return null;
            }

            transform.localScale = targetScale;
            rectTransform.anchoredPosition = startPosition + Vector2.down * 80f;
            fistImage.color = clickColor;
            
            // Explode text when fist hits with a slight delay for impact
            if (buttonText != null)
            {
                yield return new WaitForSeconds(0.05f); // Small delay for impact feel
                ExplodeText();
                
                // Add screen shake effect
                StartCoroutine(ScreenShake());
                
                // Flash the screen
                StartCoroutine(ScreenFlash());
            }
            
            // Hold at maximum size
            yield return new WaitForSeconds(0.3f); // Longer hold for more dramatic effect
            
            // Reset after scene transition starts
            isAnimating = false;
        }

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
            
            CleanupFragments();
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
    }
}