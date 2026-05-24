using UnityEngine;


public class RenderCheck : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _mesh;
    [SerializeField]
    private ParticleSystem[] _particle;

    private void Awake()
    {
        _mesh = gameObject.GetComponent<MeshRenderer>();
        _particle = gameObject.GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        Vector3 viewport = Camera.main.WorldToViewportPoint(transform.position);
        if (viewport.x >= -0.05f && viewport.x <= 1.05 && viewport.y >= -0.05f && viewport.y <= 1.05f)
        {
            Visible();
        }
        else
        {
            Invisible();
        }
    }

    private void Visible()
    {
        if (_mesh != null)
        {
            if (_mesh.enabled == false)
            {
                _mesh.enabled = true;
            }
        }
        if (_particle.Length > 0)
        {
            foreach (var particle in _particle)
            {
                var emission = particle.emission;
                if (emission.enabled == false)
                {
                    emission.enabled = true;
                }
            }
        }
    }

    private void Invisible()
    {
        if (_mesh != null)
        {
            if (_mesh.enabled == true)
            {
                _mesh.enabled = false;
            }
        }
        if (_particle.Length > 0)
        {
            foreach (var particle in _particle)
            {
                var emission = particle.emission;
                if (emission.enabled == true)
                {
                    emission.enabled = false;
                }
            }
        }
    }
}
