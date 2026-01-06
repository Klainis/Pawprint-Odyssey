using UnityEngine;

public class InstantiateParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem _pollenExplosionParticle;
    [SerializeField] private ParticleSystem _pollenNodeTreeExplosionParticle;

    private ParticleSystem _pollenExplosionInstance;
    private ParticleSystem _pollenNodeInstance;

    private Vector3 _position;

    private void Start()
    {
        _position = transform.position;
    }

    public void InstantiatePollen()
    {
        //Debug.Log("Obj pos: " + _position);
        //Debug.Log("Obj name: " + gameObject.name);

        if (_pollenExplosionInstance != null)
        {
            return;
        }

        Quaternion _pollenRotation = Quaternion.identity;
        _pollenExplosionInstance = Instantiate(_pollenExplosionParticle, _position, _pollenRotation);
    }

    public void InstantiateNodePollen(Vector3 nodeButtonPosition)
    {
        _pollenNodeInstance = Instantiate(_pollenNodeTreeExplosionParticle, nodeButtonPosition, Quaternion.identity);
    }
}
