using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class ShowScretRoomAfterBrakeWall : MonoBehaviour
{
    [SerializeField] private float _duration = 1f;
    //[SerializeField] private float _speed = 2f;
    private SpriteShapeRenderer _fillSpriteShapeRender;

    private float _fillAlpha = 0f;
    private Color _fillColor;

    private Coroutine _shower;

    private void Awake()
    {
        _fillSpriteShapeRender = GetComponent<SpriteShapeRenderer>();
    }

    private void Start()
    {
        _fillColor = _fillSpriteShapeRender.color;
        _fillSpriteShapeRender.color = _fillColor;
    }

    public void StartShower()
    {
        if (_shower != null)
        {
            StopCoroutine(_shower);
        }

        _shower = StartCoroutine(Shower());
    }

    public void ShowOpenedSecretRoom()
    {
        _fillSpriteShapeRender = GetComponent<SpriteShapeRenderer>();

        _fillColor.a = 0f;
        _fillSpriteShapeRender.color = _fillColor;
    }

    private IEnumerator Shower()
    {
        Debug.Log("Start showing secret room");
        float time = _duration;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            _fillColor.a = Mathf.Lerp( _fillColor.a, 0, elapsedTime / time );
            _fillSpriteShapeRender.color = _fillColor;
            yield return null;
        }

        _fillColor.a = 0;
        _fillSpriteShapeRender.color= _fillColor;
    }
}
