using UnityEngine;

public class EducationCheck : MonoBehaviour
{
    [SerializeField] private ShowEducation _showEducation;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerModel = PlayerView.Instance.PlayerModel;
        if (_showEducation.IsMove && playerModel.MoveEducation) return;
        if (_showEducation.IsWallJump && playerModel.WallJumpEducation) return;
        if (_showEducation.IsAttack && playerModel.AttackEducation) return;

        if (collision.CompareTag("Player"))
        {
            _showEducation.gameObject.SetActive(true);
            _showEducation.FadeIn();
            gameObject.SetActive(false);
        }
    }
}
