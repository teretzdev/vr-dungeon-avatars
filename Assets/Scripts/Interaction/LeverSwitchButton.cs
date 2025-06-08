using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

namespace DungeonYou.Interaction
{
    /// <summary>
    /// Interactive lever, switch, or button that can trigger events like opening doors or activating traps.
    /// Supports XR interaction and various activation modes.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class LeverSwitchButton : MonoBehaviour
    {
        public enum InteractionType { Lever, Switch, Button, Wheel }
        public enum ActivationMode { Toggle, Hold, OneShot }
        
        [Header("Interaction Configuration")]
        [SerializeField] private InteractionType interactionType = InteractionType.Lever;
        [SerializeField] private ActivationMode activationMode = ActivationMode.Toggle;
        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool requiresHeroOnly = true;
        
        [Header("Lever Settings")]
        [SerializeField] private float leverAngle = 45f;
        [SerializeField] private float leverSpeed = 2f;
        
        [Header("Button Settings")]
        [SerializeField] private float buttonPressDepth = 0.1f;
        [SerializeField] private float buttonReleaseDelay = 0.5f;
        
        [Header("Visual Feedback")]
        [SerializeField] private Transform visualTransform;
        [SerializeField] private Material activatedMaterial;
        [SerializeField] private Material deactivatedMaterial;
        [SerializeField] private GameObject activationEffect;
        
        [Header("Audio")]
        [SerializeField] private AudioClip activateSound;
        [SerializeField] private AudioClip deactivateSound;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onActivated;
        [SerializeField] private UnityEvent onDeactivated;
        [SerializeField] private UnityEvent<bool> onStateChanged; // Passes activation state
        
        private XRGrabInteractable grabInteractable;
        private AudioSource audioSource;
        private MeshRenderer meshRenderer;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Coroutine animationCoroutine;
        private bool isInteracting = false;
        
        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
                
            if (visualTransform == null)
                visualTransform = transform;
                
            meshRenderer = visualTransform.GetComponent<MeshRenderer>();
            
            initialPosition = visualTransform.localPosition;
            initialRotation = visualTransform.localRotation;
            
            // Set initial visual state
            UpdateVisuals();
        }
        
        private void OnEnable()
        {
            grabInteractable.activated.AddListener(OnXRActivated);
            
            // For hold mode, also listen to deactivation
            if (activationMode == ActivationMode.Hold)
            {
                grabInteractable.deactivated.AddListener(OnXRDeactivated);
            }
        }
        
        private void OnDisable()
        {
            grabInteractable.activated.RemoveListener(OnXRActivated);
            grabInteractable.deactivated.RemoveListener(OnXRDeactivated);
        }
        
        private void OnXRActivated(ActivateEventArgs args)
        {
            if (isInteracting) return;
            
            if (requiresHeroOnly && !args.interactorObject.transform.CompareTag("Hero"))
                return;
                
            Activate();
        }
        
        private void OnXRDeactivated(DeactivateEventArgs args)
        {
            if (activationMode == ActivationMode.Hold)
            {
                Deactivate();
            }
        }
        
        public void Activate()
        {
            if (isInteracting) return;
            
            switch (activationMode)
            {
                case ActivationMode.Toggle:
                    Toggle();
                    break;
                case ActivationMode.Hold:
                case ActivationMode.OneShot:
                    SetActivated(true);
                    break;
            }
        }
        
        public void Deactivate()
        {
            if (activationMode == ActivationMode.Hold)
            {
                SetActivated(false);
            }
        }
        
        public void Toggle()
        {
            SetActivated(!isActivated);
        }
        
        private void SetActivated(bool activated)
        {
            if (isActivated == activated && activationMode != ActivationMode.OneShot) 
                return;
                
            isActivated = activated;
            
            if (animationCoroutine != null) 
                StopCoroutine(animationCoroutine);
                
            animationCoroutine = StartCoroutine(AnimateActivation(activated));
            
            // Trigger events
            if (activated)
            {
                onActivated?.Invoke();
                PlaySound(activateSound);
            }
            else
            {
                onDeactivated?.Invoke();
                PlaySound(deactivateSound);
            }
            
            onStateChanged?.Invoke(activated);
            
            // Spawn effect
            if (activationEffect != null && activated)
            {
                var effect = Instantiate(activationEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }
        
        private IEnumerator AnimateActivation(bool activated)
        {
            isInteracting = true;
            float elapsed = 0f;
            float duration = 1f / leverSpeed;
            
            Vector3 startPos = visualTransform.localPosition;
            Quaternion startRot = visualTransform.localRotation;
            Vector3 targetPos = initialPosition;
            Quaternion targetRot = initialRotation;
            
            switch (interactionType)
            {
                case InteractionType.Lever:
                    targetRot = initialRotation * Quaternion.Euler(activated ? leverAngle : 0, 0, 0);
                    break;
                    
                case InteractionType.Switch:
                    targetRot = initialRotation * Quaternion.Euler(0, activated ? 180 : 0, 0);
                    break;
                    
                case InteractionType.Button:
                    targetPos = initialPosition + (activated ? Vector3.down * buttonPressDepth : Vector3.zero);
                    duration = 0.2f;
                    break;
                    
                case InteractionType.Wheel:
                    // Wheel rotates continuously while activated
                    if (activated)
                    {
                        while (isActivated)
                        {
                            visualTransform.Rotate(0, 90 * leverSpeed * Time.deltaTime, 0);
                            yield return null;
                        }
                    }
                    break;
            }
            
            // Animate to target position/rotation
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / duration);
                
                visualTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                visualTransform.localRotation = Quaternion.Lerp(startRot, targetRot, t);
                
                yield return null;
            }
            
            visualTransform.localPosition = targetPos;
            visualTransform.localRotation = targetRot;
            
            // Handle button auto-release
            if (interactionType == InteractionType.Button && activated && activationMode == ActivationMode.OneShot)
            {
                yield return new WaitForSeconds(buttonReleaseDelay);
                SetActivated(false);
            }
            
            UpdateVisuals();
            isInteracting = false;
        }
        
        private void UpdateVisuals()
        {
            if (meshRenderer != null)
            {
                if (isActivated && activatedMaterial != null)
                    meshRenderer.material = activatedMaterial;
                else if (!isActivated && deactivatedMaterial != null)
                    meshRenderer.material = deactivatedMaterial;
            }
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
                audioSource.PlayOneShot(clip);
        }
        
        // Public API for connecting to other systems
        public void ConnectToDoor(DoorGate door)
        {
            onActivated.AddListener(() => door.Open());
            onDeactivated.AddListener(() => door.Close());
        }
        
        public void ConnectToTrap(GameObject trap)
        {
            onActivated.AddListener(() => trap.SetActive(true));
            onDeactivated.AddListener(() => trap.SetActive(false));
        }
        
        public bool IsActivated() => isActivated;
    }
}