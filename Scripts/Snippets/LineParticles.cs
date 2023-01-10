using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class LineParticles : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }
    
    void Update()
    {
        var startPoint = lineRenderer.GetPosition(0);
        var endPoint = lineRenderer.GetPosition(1);
        var line = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);

        var shapeModule = particles.shape;
        shapeModule.radius = Vector3.Distance(startPoint, endPoint) / 2f;
        transform.right = Vector3.Normalize(startPoint - endPoint);
        transform.position = Vector3.Lerp(startPoint, endPoint, 0.5f);
    }
}
