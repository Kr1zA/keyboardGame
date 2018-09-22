using UnityEngine;

public class AlphaManager : MonoBehaviour
{
    void Update()
    {
        Vector3 cordinates = GameManager.Instance.GameCamera.WorldToViewportPoint(transform.position);

        if (cordinates.x > 1.1 || cordinates.x < -0.1 || cordinates.y < -0.1)
        {
            Destroy(gameObject);
        }
    }
}