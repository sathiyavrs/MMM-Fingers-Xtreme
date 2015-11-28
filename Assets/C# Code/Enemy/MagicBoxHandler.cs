using UnityEngine;

public class MagicBoxHandler : MonoBehaviour
{
    public GameObject Enemy;
    public BoxCollider2D Bounds;
    public float Speed = 5f;
    public float MinDistanceToAcquireTarget = 0.2f;

    private Vector2 _targetPosition;
    private bool _atPosition;

    public void Start()
    {
        if (Enemy == null)
            Debug.LogError("Enemy is null!");

        if (Bounds == null)
            Debug.LogError("Bounds is null!");

        _atPosition = true;
    }

    public void Update()
    {
        if (Globals.GameOver)
            return;

        CheckPosition();
        HandleMovement(); 
    }

    private void CheckPosition()
    {
        if (!_atPosition)
            return;

        _atPosition = false;

        var leftMostPoint = Bounds.transform.localPosition.x + (Bounds.offset.x - Bounds.size.x / 2) * Bounds.transform.localScale.x;

        var rightMostPoint = Bounds.transform.localPosition.x + (Bounds.offset.x + Bounds.size.x / 2) * Bounds.transform.localScale.x;

        var topMostPoint = Bounds.transform.localPosition.y + (Bounds.offset.y + Bounds.size.y / 2) * Bounds.transform.localScale.y;

        var bottomMostPoint = Bounds.transform.localPosition.y + (Bounds.offset.y - Bounds.size.y / 2) * Bounds.transform.localScale.y;

        _targetPosition = new Vector2(Random.Range(leftMostPoint, rightMostPoint), Random.Range(topMostPoint, bottomMostPoint));

    }

    private void HandleMovement()
    {
        var currentPosition = new Vector2(Enemy.transform.localPosition.x, Enemy.transform.localPosition.y);
        var minDistanceSquared = MinDistanceToAcquireTarget * MinDistanceToAcquireTarget;
        
        if ((currentPosition - _targetPosition).sqrMagnitude < minDistanceSquared)
        {
            _atPosition = true;
            return;
        }

        var direction = (_targetPosition - currentPosition);
        var magnitude = Speed * Time.deltaTime * Globals.GlobalRatio;

        var deltaMovement = direction * magnitude / direction.magnitude;
        
        Enemy.transform.localPosition = new Vector3(Enemy.transform.localPosition.x + deltaMovement.x, Enemy.transform.localPosition.y + deltaMovement.y, Enemy.transform.localPosition.z);

        var vibrationHandler = Enemy.GetComponent<VibrationHandler>();
        if (vibrationHandler == null)
            return;

        vibrationHandler.SetCenterPosition(Enemy.transform.position);
    }
}
