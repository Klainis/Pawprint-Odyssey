using UnityEngine;

public class ReceivingDoubleJump : MonoBehaviour
{
    private PlayerView playerView;

    private readonly string doubleJumpItemTag = "DoubleJumpItem";

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
    }

    public void EnableDoubleJump()
    {
        playerView.PlayerModel.SetHasDoubleJump();
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
