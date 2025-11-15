using UnityEngine;

public class GroundCheckForSaveGround : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform m_GroundCheck;

    public bool IsSaveGround()
    {
        var hit = Physics2D.Raycast(m_GroundCheck.position, Vector2.down, PlayerMove.groundCheckRadius, whatIsGround);
        if (hit && !hit.collider.CompareTag("Trap"))
            return true;
        else
            return false;
    }
}
