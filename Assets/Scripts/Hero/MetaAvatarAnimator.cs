using UnityEngine;

/// <summary>
/// Manages Meta Avatar specific animations and expressions.
/// Handles gestures, facial expressions, and combat sequences.
/// </summary>
public class MetaAvatarAnimator : MonoBehaviour
{
    public static MetaAvatarAnimator Instance { get; private set; }

    // Reference to Meta Avatar animation controller (replace with actual type)
    public Animator avatarAnimator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetExpression(string expressionName)
    {
        // Example: Set facial expression
        // avatarAnimator.SetTrigger(expressionName);
        Debug.Log($"Avatar expression set: {expressionName}");
    }

    public void PlayGesture(string gestureName)
    {
        // Example: Play gesture animation
        // avatarAnimator.SetTrigger(gestureName);
        Debug.Log($"Avatar gesture played: {gestureName}");
    }

    public void PlayCombatAnimation(string animName)
    {
        // Example: Play combat animation
        // avatarAnimator.SetTrigger(animName);
        Debug.Log($"Avatar combat animation: {animName}");
    }
} 