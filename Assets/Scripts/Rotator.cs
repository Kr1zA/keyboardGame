using UnityEngine;

public class Rotator : MonoBehaviour
{
    void Update()
    {
//        Debug.Log(Time.time);
        transform.Rotate(0f, 0f, Time.deltaTime*300f); 
    }
}