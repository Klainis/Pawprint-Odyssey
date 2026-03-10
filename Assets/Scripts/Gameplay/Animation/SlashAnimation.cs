using UnityEngine;

public class SlashAnimation : MonoBehaviour
{
    private Quaternion _slashRotationInitial;
    private Vector3 _slashScaleInitial;

    private void Awake()
    {
        _slashRotationInitial = GetComponent<Transform>().localRotation;
        _slashScaleInitial = GetComponent<Transform>().localScale;
    }
    public void OnSlashAnimationEnd()
    {
        gameObject.transform.localRotation = _slashRotationInitial;
        gameObject.transform.localScale = _slashScaleInitial;
        gameObject.SetActive(false);
    }

    public void SetActiveSlashObject()
    {
        gameObject.SetActive(true);
    }

    public void SlashAttack()
    {
        PlayerAttack.Instance.AttackDamage();
    }
}
