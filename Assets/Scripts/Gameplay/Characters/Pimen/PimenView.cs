using UnityEngine;

public class PimenView : MonoBehaviour
{
    private static PimenView _instance;
    public static PimenView Instance {  get { return _instance; } }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;
    }
}
