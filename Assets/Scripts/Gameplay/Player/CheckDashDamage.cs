using UnityEngine;

public class CheckDashDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"Layaer of collision ob: {LayerMask.LayerToName(collision.gameObject.layer)}");
        //Debug.Log($"My layer: {LayerMask.LayerToName(PlayerView.Instance.gameObject.layer)}");
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemy" &&
            LayerMask.LayerToName(PlayerView.Instance.gameObject.layer) == "PlayerDash" && PlayerInput.Instance.DamageDashActive)
        {
            if (PlayerView.Instance.PlayerModel.HasDamageDash)
            {
                PlayerAttack.Instance.AttackDashDamage();
            }
        }
    }
}
