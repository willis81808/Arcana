using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineAnimator : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;

    private LineRenderer lineRenderer;

    private float offset = 0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    
    void Update()
    {
        offset += Time.deltaTime * speed;
        lineRenderer.material.SetTextureOffset("_MainTex", Vector2.right * offset);
    }
}
