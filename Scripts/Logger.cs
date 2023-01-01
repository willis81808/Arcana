using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour
{
    [SerializeField, TextArea]
    private string message;

    public void Log()
    {
        UnityEngine.Debug.Log(message);
    }
}
