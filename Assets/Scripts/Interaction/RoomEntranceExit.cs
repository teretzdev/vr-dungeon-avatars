using UnityEngine;
using UnityEngine.Events;

namespace DungeonYou.Interaction
{
    /// <summary>
    /// Handles room entrance/exit triggers for dungeon progression.
    /// Attach to trigger colliders at room entrances/exits.
    /// </summary>
    public class RoomEntranceExit : MonoBehaviour
    {
        [Header("Room Settings")]
        [SerializeField] private string roomID = "Room_01";
        [SerializeField] private bool isEntrance = true;
        [SerializeField] private bool isExit = true;
        
        [Header("Events")]
        [SerializeField] private UnityEvent<string> onHeroEnter = new UnityEvent<string>();
        [SerializeField] private UnityEvent<string> onHeroExit = new UnityEvent<string>();
        
        private EdgarRoomWrapper roomWrapper;
        private bool hasTriggered = false;
        
        private void Awake()
        {
            // Ensure we have a trigger collider
            var collider = GetComponent<Collider>();
            if (collider == null)
            {
                Debug.LogError($"RoomEntranceExit on {gameObject.name} requires a Collider component!");
            }
            else if (!collider.isTrigger)
            {
                Debug.LogWarning($"RoomEntranceExit on {gameObject.name}: Collider should be set as trigger!");
                collider.isTrigger = true;
            }
            
            // Try to find the room wrapper
            roomWrapper = GetComponentInParent<EdgarRoomWrapper>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!isEntrance) return;
            
            // Check if it's the hero
            if (other.CompareTag("Player") || other.GetComponent<MetaAvatarHero>() != null)
            {
                if (!hasTriggered)
                {
                    hasTriggered = true;
                    Debug.Log($"Hero entered room: {roomID}");
                    
                    // Notify room wrapper if available
                    if (roomWrapper != null)
                    {
                        roomWrapper.OnRoomEntered();
                    }
                    
                    // Fire Unity event
                    onHeroEnter?.Invoke(roomID);
                    
                    // Notify GameManager
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.OnRoomEntered(roomID);
                    }
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!isExit) return;
            
            // Check if it's the hero
            if (other.CompareTag("Player") || other.GetComponent<MetaAvatarHero>() != null)
            {
                Debug.Log($"Hero exited room: {roomID}");
                
                // Fire Unity event
                onHeroExit?.Invoke(roomID);
                
                // Reset trigger flag when hero exits
                hasTriggered = false;
            }
        }
        
        /// <summary>
        /// Manually trigger room entrance
        /// </summary>
        public void TriggerEnter()
        {
            if (!hasTriggered)
            {
                hasTriggered = true;
                onHeroEnter?.Invoke(roomID);
            }
        }
        
        /// <summary>
        /// Reset the trigger state
        /// </summary>
        public void ResetTrigger()
        {
            hasTriggered = false;
        }
    }
}