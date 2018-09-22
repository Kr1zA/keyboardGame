using UnityEngine;

public class FloorCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        GameManager.Instance.RemoveFallingAlpha(other.gameObject, true);
        GameManager.Instance.LoseLife();
    }
}