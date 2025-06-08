using UnityEngine;
using UnityEngine.Events;

namespace DungeonYou.Interaction
{
    /// <summary>
    /// Handles room entrance/exit detection for dungeon progression.
    /// Triggers events when the hero enters or exits a room.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class RoomEntranceExit : MonoBehaviour
    {
        [Header("Room Configuration")]
        [SerializeField] private string roomID = "Room_01";
        [SerializeField] private bool isEntrance = true;
        [SerializeField] private bool isExit = true;
        
        [Header("Events")]
        [SerializeField] private UnityEvent<string> onHeroEnter;
        [SerializeField] private UnityEvent<string> onHeroExit;
        [SerializeField] private UnityEvent onRoomCompleted;
        
        private bool heroInside = false;
        private Collider triggerCollider;
        
        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hero") && isEntrance)
            {
                heroInside = true;
                onHeroEnter?.Invoke(roomID);
                
                // Notify EdgarRoomWrapper if available
                var roomWrapper = GetComponentInParent<Dungeon.EdgarRoomWrapper>();
                if (roomWrapper != null)
                {
                    roomWrapper.OnHeroEntered();
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Hero") && isExit)
            {
                heroInside = false;
                onHeroExit?.Invoke(roomID);
                
                // Check if room should be marked as completed
                var roomWrapper = GetComponentInParent<Dungeon.EdgarRoomWrapper>();
                if (roomWrapper != null && roomWrapper.IsRoomCleared())
                {
                    onRoomCompleted?.Invoke();
                }
            }
        }
        
        public void SetRoomID(string id) => roomID = id;
        public string GetRoomID() => roomID;
        public bool IsHeroInside() => heroInside;
    }
}