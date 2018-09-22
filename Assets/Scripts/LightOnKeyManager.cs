using UnityEngine;
using System.Collections;

public class LightOnKeyManager : MonoBehaviour
{
    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime > 0.08)
            Destroy(gameObject);
    }
}