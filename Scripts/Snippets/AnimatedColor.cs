using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedColor : MonoBehaviour
{
    [SerializeField]
    private Image target;

    [SerializeField]
    private Gradient color;

    [SerializeField]
    private float loopSeconds = 1f;

    void Update()
    {
        target.color = color.Evaluate((Time.time / loopSeconds) % 1f);
    }
}
