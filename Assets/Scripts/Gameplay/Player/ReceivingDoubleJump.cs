using UnityEngine;

public class ReceivingDoubleJump : MonoBehaviour
{
    private readonly string doubleJumpItemTag = "DoubleJumpItem";

    public void EnableDoubleJump()
    {
        PlayerView.Instance.PlayerModel.SetHasDoubleJump();
        SaveSystem.Save();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(doubleJumpItemTag))
        {
            Destroy(collision.gameObject);
            EnableDoubleJump();
        }
    }
}
