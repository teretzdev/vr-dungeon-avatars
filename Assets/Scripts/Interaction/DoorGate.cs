using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace DungeonYou.Interaction
{
    /// <summary>
    /// Handles door/gate interactables that can be opened/closed via voice commands or triggers.
    /// Supports animation, physics movement, or simple activation/deactivation.
    /// </summary>
    public class DoorGate : MonoBehaviour
    {
        public enum DoorState { Closed, Opening, Open, Closing }
        public enum DoorType { Animated, Physics, Simple }
        
        [Header("Door Settings")]
        [SerializeField] private DoorType doorType = DoorType.Animated;
        [SerializeField] private DoorState initialState = DoorState.Closed;
        [SerializeField] private bool isLocked = false;
        [SerializeField] private string requiredKeyID = "";
        
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private Vector3 openPosition = new Vector3(0, 3, 0);
        [SerializeField] private Vector3 closedPosition = Vector3.zero;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;
        [SerializeField] private AudioClip lockedSound;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onOpen = new UnityEvent();
        [SerializeField] private UnityEvent onClose = new UnityEvent();
        [SerializeField] private UnityEvent onLocked = new UnityEvent();
        
        private DoorState currentState;
        private Animator animator;
        private Rigidbody rb;
        private Vector3 targetPosition;
        
        private void Awake()
        {
            currentState = initialState;
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            
            if (doorType == DoorType.Physics)
            {
                targetPosition = currentState == DoorState.Open ? openPosition : closedPosition;
                transform.localPosition = targetPosition;
            }
            
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }
        
        /// <summary>
        /// Toggle the door between open and closed states
        /// </summary>
        public void ToggleDoor()
        {
            if (currentState == DoorState.Open || currentState == DoorState.Opening)
            {
                CloseDoor();
            }
            else
            {
                OpenDoor();
            }
        }
        
        /// <summary>
        /// Open the door
        /// </summary>
        public void OpenDoor()
        {
            if (isLocked)
            {
                Debug.Log($"Door is locked. Required key: {requiredKeyID}");
                PlaySound(lockedSound);
                onLocked?.Invoke();
                return;
            }
            
            if (currentState == DoorState.Closed || currentState == DoorState.Closing)
            {
                currentState = DoorState.Opening;
                PlaySound(openSound);
                
                switch (doorType)
                {
                    case DoorType.Animated:
                        if (animator != null)
                        {
                            animator.SetTrigger("Open");
                            animator.SetBool("IsOpen", true);
                        }
                        StartCoroutine(WaitForAnimation(true));
                        break;
                        
                    case DoorType.Physics:
                        targetPosition = openPosition;
                        StartCoroutine(MoveDoor(true));
                        break;
                        
                    case DoorType.Simple:
                        gameObject.SetActive(false);
                        currentState = DoorState.Open;
                        onOpen?.Invoke();
                        break;
                }
            }
        }
        
        /// <summary>
        /// Close the door
        /// </summary>
        public void CloseDoor()
        {
            if (currentState == DoorState.Open || currentState == DoorState.Opening)
            {
                currentState = DoorState.Closing;
                PlaySound(closeSound);
                
                switch (doorType)
                {
                    case DoorType.Animated:
                        if (animator != null)
                        {
                            animator.SetTrigger("Close");
                            animator.SetBool("IsOpen", false);
                        }
                        StartCoroutine(WaitForAnimation(false));
                        break;
                        
                    case DoorType.Physics:
                        targetPosition = closedPosition;
                        StartCoroutine(MoveDoor(false));
                        break;
                        
                    case DoorType.Simple:
                        gameObject.SetActive(true);
                        currentState = DoorState.Closed;
                        onClose?.Invoke();
                        break;
                }
            }
        }
        
        /// <summary>
        /// Unlock the door with a key
        /// </summary>
        public bool UnlockWithKey(string keyID)
        {
            if (isLocked && keyID == requiredKeyID)
            {
                isLocked = false;
                Debug.Log($"Door unlocked with key: {keyID}");
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Force lock/unlock the door
        /// </summary>
        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }
        
        private IEnumerator MoveDoor(bool opening)
        {
            while (Vector3.Distance(transform.localPosition, targetPosition) > 0.01f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            
            transform.localPosition = targetPosition;
            currentState = opening ? DoorState.Open : DoorState.Closed;
            
            if (opening)
                onOpen?.Invoke();
            else
                onClose?.Invoke();
        }
        
        private IEnumerator WaitForAnimation(bool opening)
        {
            // Wait for animation to complete (approximate)
            yield return new WaitForSeconds(1f);
            
            currentState = opening ? DoorState.Open : DoorState.Closed;
            
            if (opening)
                onOpen?.Invoke();
            else
                onClose?.Invoke();
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        /// <summary>
        /// Handle voice commands for the door
        /// </summary>
        public void OnVoiceCommand(string command)
        {
            command = command.ToLower();
            
            if (command.Contains("open"))
            {
                OpenDoor();
            }
            else if (command.Contains("close"))
            {
                CloseDoor();
            }
        }
        
        public DoorState GetState() => currentState;
        public bool IsLocked() => isLocked;
    }
}