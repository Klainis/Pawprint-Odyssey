using UnityEngine;

public class ClawAnimation : MonoBehaviour
{
    [SerializeField] private PiercingClaw piercingClaw;
    public void SetActiveClawObject()
    {
        gameObject.SetActive(true);
    }

    public void EndAnimation()
    {
        piercingClaw.EndClawAnimation();
    }
}
