using UnityEngine;

public class EndSlashAnimation : MonoBehaviour
{
    public void OnSlashAnimationEnd()
    {
        gameObject.SetActive(false);
    }

    public void SetActiveSlashObject()
    {
        gameObject.SetActive(true);
    }
}
