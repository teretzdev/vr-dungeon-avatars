using UnityEngine;

/// <summary>
/// Manages hero's limited communication using Meta Avatar expressions.
/// Handles facial expressions, gestures, and acknowledgment animations.
/// </summary>
public class MetaAvatarResponseSystem : MonoBehaviour
{
    public static MetaAvatarResponseSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RespondToCommand(string command)
    {
        if (command.ToLower().Contains("yes") || command.ToLower().Contains("good"))
        {
            MetaAvatarAnimator.Instance.SetExpression("Smile");
            MetaAvatarAnimator.Instance.PlayGesture("ThumbsUp");
        }
        else if (command.ToLower().Contains("no") || command.ToLower().Contains("bad"))
        {
            MetaAvatarAnimator.Instance.SetExpression("Frown");
            MetaAvatarAnimator.Instance.PlayGesture("ShakeHead");
        }
        else
        {
            MetaAvatarAnimator.Instance.SetExpression("Neutral");
        }
        Debug.Log($"Avatar responded to command: {command}");
    }
} 