using UnityEngine;

public class GroundCheckForSaveGround : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround;

    const float k_GroundeDistance = 0.2f;
    private RaycastHit2D hit;
    [SerializeField] private Transform m_GroundCheck;

    public bool IsSaveGround()
    {
        hit = Physics2D.Raycast(m_GroundCheck.position, Vector2.down, k_GroundeDistance, whatIsGround);
        if (hit && !(hit.collider.tag == "NotSaveGround"))
        {
            Debug.Log(hit.collider.tag);
            return true;
        }
        else
            return false;
    }
}
