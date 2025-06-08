using UnityEngine;
using Oculus.Avatar2;

/// <summary>
/// Manages Meta Avatar specific animations and expressions.
/// Handles gestures, facial expressions, and combat sequences.
/// </summary>
public class MetaAvatarAnimator : MonoBehaviour
{
    public static MetaAvatarAnimator Instance { get; private set; }

    [Header("Avatar References")]
    private Animator avatarAnimator;
    private OvrAvatarEntity avatarEntity;
    private GameObject avatarGameObject;
    
    [Header("Animation Settings")]
    [SerializeField] private float transitionDuration = 0.2f;
    [SerializeField] private AnimatorOverrideController combatAnimatorOverride;
    
    // Animation parameter names
    private const string SPEED_PARAM = "Speed";
    private const string COMBAT_PARAM = "InCombat";
    private const string ATTACK_TRIGGER = "Attack";
    private const string HIT_TRIGGER = "Hit";
    private const string DEATH_TRIGGER = "Death";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetAvatar(GameObject avatar)
    {
        if (avatar == null) return;
        
        avatarGameObject = avatar;
        avatarAnimator = avatar.GetComponent<Animator>();
        avatarEntity = avatar.GetComponent<OvrAvatarEntity>();
        
        // Set up combat animator override if available
        if (avatarAnimator != null && combatAnimatorOverride != null)
        {
            avatarAnimator.runtimeAnimatorController = combatAnimatorOverride;
        }
        
        Debug.Log("[MetaAvatarAnimator] Avatar animator configured");
    }

    public void SetExpression(string expressionName)
    {
        if (avatarEntity == null) return;
        
        // Set facial expression using Meta Avatar SDK
        switch (expressionName.ToLower())
        {
            case "happy":
                SetFacialExpression(OvrAvatarFacialExpression.Happy);
                break;
            case "sad":
                SetFacialExpression(OvrAvatarFacialExpression.Sad);
                break;
            case "angry":
                SetFacialExpression(OvrAvatarFacialExpression.Angry);
                break;
            case "surprised":
                SetFacialExpression(OvrAvatarFacialExpression.Surprised);
                break;
            default:
                SetFacialExpression(OvrAvatarFacialExpression.Neutral);
                break;
        }
        
        Debug.Log($"[MetaAvatarAnimator] Expression set: {expressionName}");
    }
    
    private void SetFacialExpression(OvrAvatarFacialExpression expression)
    {
        // This would use the actual Meta Avatar API to set expressions
        // For now, we'll use animator parameters as a fallback
        if (avatarAnimator != null)
        {
            avatarAnimator.SetInteger("Expression", (int)expression);
        }
    }

    public void PlayGesture(string gestureName)
    {
        if (avatarAnimator == null) return;
        
        // Play gesture animation
        avatarAnimator.SetTrigger($"Gesture_{gestureName}");
        Debug.Log($"[MetaAvatarAnimator] Gesture played: {gestureName}");
    }

    public void PlayCombatAnimation(string animName)
    {
        if (avatarAnimator == null) return;
        
        switch (animName.ToLower())
        {
            case "attack":
                avatarAnimator.SetTrigger(ATTACK_TRIGGER);
                break;
            case "hit":
                avatarAnimator.SetTrigger(HIT_TRIGGER);
                break;
            case "death":
                avatarAnimator.SetTrigger(DEATH_TRIGGER);
                break;
            case "enterCombat":
                avatarAnimator.SetBool(COMBAT_PARAM, true);
                break;
            case "exitCombat":
                avatarAnimator.SetBool(COMBAT_PARAM, false);
                break;
        }
        
        Debug.Log($"[MetaAvatarAnimator] Combat animation: {animName}");
    }
    
    public void SetMovementSpeed(float speed)
    {
        if (avatarAnimator != null)
        {
            avatarAnimator.SetFloat(SPEED_PARAM, speed);
        }
    }
    
    public void PlayEmote(string emoteName)
    {
        if (avatarEntity == null) return;
        
        // Play Meta Avatar emote
        // This would use the actual Meta Avatar API
        Debug.Log($"[MetaAvatarAnimator] Playing emote: {emoteName}");
    }
    
    public bool IsAnimationPlaying(string animationName)
    {
        if (avatarAnimator == null) return false;
        
        AnimatorStateInfo stateInfo = avatarAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }
}

// Placeholder for Meta Avatar facial expressions enum
public enum OvrAvatarFacialExpression
{
    Neutral = 0,
    Happy = 1,
    Sad = 2,
    Angry = 3,
    Surprised = 4
} 