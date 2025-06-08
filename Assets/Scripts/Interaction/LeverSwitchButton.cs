using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace DungeonYou.Interaction
{
    /// <summary>
    /// Handles lever/switch/button interactables that can trigger doors, traps, or other events.
    /// Supports XR interaction and voice commands.
    /// </summary>
    public class LeverSwitchButton : MonoBehaviour
    {
        public enum InteractableType { Lever, Switch, Button }
        public enum ActivationType { Toggle, Hold, OneShot }
        
        [Header("Interactable Settings")]
        [SerializeField] private InteractableType interactableType = InteractableType.Lever;
        [SerializeField] private ActivationType activationType = ActivationType.Toggle;
        [SerializeField] private bool startActivated = false;
        [SerializeField] private bool requiresKey = false;
        [SerializeField] private string requiredKeyID = "";
        
        [Header("Visual Settings")]
        [SerializeField] private Transform movingPart;
        [SerializeField] private Vector3 activatedPosition = new Vector3(0, -0.1f, 0);
        [SerializeField] private Vector3 deactivatedPosition = Vector3.zero;
        [SerializeField] private float activatedRotation = 45f;
        [SerializeField] private float deactivatedRotation = -45f;
        [SerializeField] private float moveSpeed = 2f;
        
        [Header("Reset Settings")]
        [SerializeField] private bool autoReset = false;
        [SerializeField] private float resetDelay = 3f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip activateSound;
        [SerializeField] private AudioClip deactivateSound;
        [SerializeField] private AudioClip lockedSound;
        
        [Header("Effects")]
        [SerializeField] private ParticleSystem activateParticles;
        [SerializeField] private Light indicatorLight;
        [SerializeField] private Color activeColor = Color.green;
        [SerializeField] private Color inactiveColor = Color.red;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onActivated = new UnityEvent();
        [SerializeField] private UnityEvent onDeactivated = new UnityEvent();
        [SerializeField] private UnityEvent onLocked = new UnityEvent();
        [SerializeField] private UnityEvent<bool> onStateChanged = new UnityEvent<bool>();
        
        private bool isActivated;
        private bool isMoving;
        private bool hasBeenUsed;
        private XRGrabInteractable grabInteractable;
        private Collider triggerCollider;
        
        private void Awake()
        {
            isActivated = startActivated;
            
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
                
            grabInteractable = GetComponent<XRGrabInteractable>();
            triggerCollider = GetComponent<Collider>();
            
            // Setup XR interaction if component exists
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnXRSelect);
                grabInteractable.selectExited.AddListener(OnXRDeselect);
            }
            
            // Set initial visual state
            UpdateVisualState(true);
        }
        
        /// <summary>
        /// Main interaction method
        /// </summary>
        public void Interact()
        {
            if (requiresKey)
            {
                Debug.Log($"Interactable requires key: {requiredKeyID}");
                PlaySound(lockedSound);
                onLocked?.Invoke();
                return;
            }
            
            switch (activationType)
            {
                case ActivationType.Toggle:
                    Toggle();
                    break;
                    
                case ActivationType.Hold:
                    Activate();
                    break;
                    
                case ActivationType.OneShot:
                    if (!hasBeenUsed)
                    {
                        Activate();
                        hasBeenUsed = true;
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Toggle between activated and deactivated states
        /// </summary>
        public void Toggle()
        {
            if (isActivated)
                Deactivate();
            else
                Activate();
        }
        
        /// <summary>
        /// Activate the interactable
        /// </summary>
        public void Activate()
        {
            if (isActivated && activationType != ActivationType.Hold) return;
            
            isActivated = true;
            PlaySound(activateSound);
            
            // Play particles
            if (activateParticles != null)
                activateParticles.Play();
                
            // Update visuals
            StartCoroutine(UpdateVisualState());
            
            // Fire events
            onActivated?.Invoke();
            onStateChanged?.Invoke(true);
            
            Debug.Log($"{interactableType} activated!");
            
            // Handle auto-reset
            if (autoReset && activationType != ActivationType.Hold)
            {
                CancelInvoke(nameof(AutoReset));
                Invoke(nameof(AutoReset), resetDelay);
            }
        }
        
        /// <summary>
        /// Deactivate the interactable
        /// </summary>
        public void Deactivate()
        {
            if (!isActivated) return;
            
            isActivated = false;
            PlaySound(deactivateSound);
            
            // Update visuals
            StartCoroutine(UpdateVisualState());
            
            // Fire events
            onDeactivated?.Invoke();
            onStateChanged?.Invoke(false);
            
            Debug.Log($"{interactableType} deactivated!");
        }
        
        /// <summary>
        /// Update visual state of the interactable
        /// </summary>
        private System.Collections.IEnumerator UpdateVisualState(bool instant = false)
        {
            isMoving = true;
            
            // Update indicator light
            if (indicatorLight != null)
            {
                indicatorLight.color = isActivated ? activeColor : inactiveColor;
                indicatorLight.enabled = isActivated;
            }
            
            if (movingPart == null)
            {
                isMoving = false;
                yield break;
            }
            
            Vector3 targetPosition = isActivated ? activatedPosition : deactivatedPosition;
            Quaternion targetRotation = Quaternion.Euler(0, 0, isActivated ? activatedRotation : deactivatedRotation);
            
            if (instant)
            {
                if (interactableType == InteractableType.Button)
                    movingPart.localPosition = targetPosition;
                else
                    movingPart.localRotation = targetRotation;
                    
                isMoving = false;
                yield break;
            }
            
            // Animate movement
            float elapsed = 0f;
            Vector3 startPosition = movingPart.localPosition;
            Quaternion startRotation = movingPart.localRotation;
            
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * moveSpeed;
                float t = Mathf.SmoothStep(0, 1, elapsed);
                
                if (interactableType == InteractableType.Button)
                    movingPart.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                else
                    movingPart.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
                    
                yield return null;
            }
            
            // Ensure final position
            if (interactableType == InteractableType.Button)
                movingPart.localPosition = targetPosition;
            else
                movingPart.localRotation = targetRotation;
                
            isMoving = false;
        }
        
        /// <summary>
        /// Auto-reset for buttons
        /// </summary>
        private void AutoReset()
        {
            if (isActivated)
                Deactivate();
        }
        
        /// <summary>
        /// Unlock with key
        /// </summary>
        public bool UnlockWithKey(string keyID)
        {
            if (requiresKey && keyID == requiredKeyID)
            {
                requiresKey = false;
                Debug.Log($"{interactableType} unlocked with key: {keyID}");
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Force set the state
        /// </summary>
        public void SetState(bool activated)
        {
            if (activated)
                Activate();
            else
                Deactivate();
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        #region XR Interaction Callbacks
        
        private void OnXRSelect(SelectEnterEventArgs args)
        {
            if (activationType == ActivationType.Hold)
                Activate();
            else
                Interact();
        }
        
        private void OnXRDeselect(SelectExitEventArgs args)
        {
            if (activationType == ActivationType.Hold)
                Deactivate();
        }
        
        #endregion
        
        #region Voice Command Support
        
        /// <summary>
        /// Handle voice commands
        /// </summary>
        public void OnVoiceCommand(string command)
        {
            command = command.ToLower();
            
            if (command.Contains("activate") || command.Contains("pull") || command.Contains("push"))
            {
                Interact();
            }
            else if (command.Contains("deactivate") || command.Contains("release"))
            {
                if (activationType == ActivationType.Toggle)
                    Deactivate();
            }
        }
        
        #endregion
        
        /// <summary>
        /// Handle direct player collision (optional)
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (interactableType == InteractableType.Button && 
                (other.CompareTag("Player") || other.GetComponent<MetaAvatarHero>() != null))
            {
                // Buttons can be activated by stepping on them
                if (activationType == ActivationType.Hold)
                    Activate();
                else
                    Interact();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (interactableType == InteractableType.Button && 
                activationType == ActivationType.Hold &&
                (other.CompareTag("Player") || other.GetComponent<MetaAvatarHero>() != null))
            {
                Deactivate();
            }
        }
        
        public bool IsActivated() => isActivated;
        public bool RequiresKey() => requiresKey;
    }
}