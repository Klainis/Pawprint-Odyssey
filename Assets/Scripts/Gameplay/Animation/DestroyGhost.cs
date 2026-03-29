using UnityEngine;
using UnityEngine.Rendering;

public class DestroyGhost : MonoBehaviour
{
    public void DestroyGhostInstance()
    {
        Destroy(gameObject);
    }
}
