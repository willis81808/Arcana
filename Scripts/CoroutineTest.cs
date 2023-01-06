using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Foo());
    }

    private IEnumerator Foo()
    {
        UnityEngine.Debug.Log("Start");
        yield return Bar();
        UnityEngine.Debug.Log("End");
    }

    private IEnumerator Bar()
    {
        yield break;
    }
}