using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    [SerializeField] private float _time = 1;
    private void Update()
    {
        Time.timeScale = _time;
    }
}
