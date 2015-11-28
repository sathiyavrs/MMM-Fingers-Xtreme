using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public BoxCollider2D Bounds;
    
    public void Start()
    {
        if (Bounds == null)
            Debug.LogError("Bounds is not set!");
    }

    public bool TrySetPosition(Vector2 position)
    {
        var bottomLeftPosition = new Vector2(Bounds.offset.x - Bounds.size.x / 2 * Bounds.transform.localScale.x, Bounds.offset.y - Bounds.size.y / 2 * Bounds.transform.localScale.y);
        var topRightPosition = new Vector2(Bounds.offset.x + Bounds.size.x / 2 * Bounds.transform.localScale.x, Bounds.offset.y + Bounds.size.y / 2 * Bounds.transform.localScale.y);

        if (position.x > topRightPosition.x || position.x < bottomLeftPosition.x)
            return false;

        if (position.y > topRightPosition.y || position.y < bottomLeftPosition.y)
            return false;

        transform.position = new Vector3(position.x, position.y, transform.position.z);
        return true;
    }
}