using UnityEngine;

public class InstantiateParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem _pollenExplosionParticle;

    private ParticleSystem _pollenExplosionInstance;

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
}
