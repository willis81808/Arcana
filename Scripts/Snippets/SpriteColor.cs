using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColor : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] sprites;

    public Color color;
    
    void Start()
    {
        foreach (var s in sprites)
        {
            s.color = color;
        }
    }
}
