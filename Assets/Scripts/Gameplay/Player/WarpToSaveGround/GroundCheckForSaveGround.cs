using UnityEngine;

public class GroundCheckForSaveGround : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform m_GroundCheck;

    public RaycastHit2D hit {  get; private set; }

    public bool IsSaveGround()
    {
        hit = Physics2D.Raycast(m_GroundCheck.position, Vector2.down, PlayerMove.groundCheckRadius, whatIsGround);
        if (hit && (hit.collider.tag != "Trap"))
            return true;
        else
            return false;
    }
}
