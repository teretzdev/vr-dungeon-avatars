using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace DungeonYou.Interaction
{
    /// <summary>
    /// Interactive door/gate that can be opened via voice commands, levers, or other triggers.
    /// Supports various door types and animations.
    /// </summary>
    public class DoorGate : MonoBehaviour
    {
        public enum DoorType { Sliding, Rotating, Portcullis, Magic }
        public enum DoorState { Closed, Opening, Open, Closing }
        
        [Header("Door Configuration")]
        [SerializeField] private DoorType doorType = DoorType.Rotating;
        [SerializeField] private float openSpeed = 2f;
        [SerializeField] private float openAngle = 90f; // For rotating doors
        [SerializeField] private float openDistance = 3f; // For sliding doors
        [SerializeField] private bool isLocked = false;
        [SerializeField] private string requiredKey = "";
        
        [Header("Voice Commands")]
        [SerializeField] private string[] openCommands = { "open door", "open sesame" };
        [SerializeField] private string[] closeCommands = { "close door", "shut door" };
        
        [Header("Audio")]
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;
        [SerializeField] private AudioClip lockedSound;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onDoorOpen;
        [SerializeField] private UnityEvent onDoorClose;
        [SerializeField] private UnityEvent onDoorLocked;
        
        private DoorState currentState = DoorState.Closed;
        private Vector3 closedPosition;
        private Quaternion closedRotation;
        private AudioSource audioSource;
        private Coroutine animationCoroutine;
        
        private void Awake()
        {
            closedPosition = transform.position;
            closedRotation = transform.rotation;
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        private void Start()
        {
            // Register voice commands if VoiceController exists
            var voiceController = FindObjectOfType<Communication.VRIFVoiceController>();
            if (voiceController != null)
            {
                foreach (var cmd in openCommands)
                    voiceController.RegisterCommand(cmd, () => Open());
                foreach (var cmd in closeCommands)
                    voiceController.RegisterCommand(cmd, () => Close());
            }
        }
        
        public void Open()
        {
            if (currentState != DoorState.Closed) return;
            
            if (isLocked)
            {
                PlaySound(lockedSound);
                onDoorLocked?.Invoke();
                return;
            }
            
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(OpenAnimation());
        }
        
        public void Close()
        {
            if (currentState != DoorState.Open) return;
            
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(CloseAnimation());
        }
        
        public void Toggle()
        {
            if (currentState == DoorState.Closed)
                Open();
            else if (currentState == DoorState.Open)
                Close();
        }
        
        public void Unlock(string key = "")
        {
            if (string.IsNullOrEmpty(requiredKey) || requiredKey == key)
            {
                isLocked = false;
            }
        }
        
        private IEnumerator OpenAnimation()
        {
            currentState = DoorState.Opening;
            PlaySound(openSound);
            
            float elapsed = 0f;
            Vector3 targetPosition = closedPosition;
            Quaternion targetRotation = closedRotation;
            
            switch (doorType)
            {
                case DoorType.Sliding:
                    targetPosition = closedPosition + transform.up * openDistance;
                    break;
                case DoorType.Rotating:
                    targetRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
                    break;
                case DoorType.Portcullis:
                    targetPosition = closedPosition + transform.up * openDistance;
                    break;
            }
            
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * openSpeed;
                float t = Mathf.SmoothStep(0, 1, elapsed);
                
                transform.position = Vector3.Lerp(closedPosition, targetPosition, t);
                transform.rotation = Quaternion.Lerp(closedRotation, targetRotation, t);
                
                yield return null;
            }
            
            currentState = DoorState.Open;
            onDoorOpen?.Invoke();
        }
        
        private IEnumerator CloseAnimation()
        {
            currentState = DoorState.Closing;
            PlaySound(closeSound);
            
            float elapsed = 0f;
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;
            
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * openSpeed;
                float t = Mathf.SmoothStep(0, 1, elapsed);
                
                transform.position = Vector3.Lerp(startPosition, closedPosition, t);
                transform.rotation = Quaternion.Lerp(startRotation, closedRotation, t);
                
                yield return null;
            }
            
            currentState = DoorState.Closed;
            onDoorClose?.Invoke();
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
                audioSource.PlayOneShot(clip);
        }
        
        public DoorState GetState() => currentState;
        public bool IsLocked() => isLocked;
    }
}