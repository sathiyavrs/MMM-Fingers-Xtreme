using UnityEngine;

public class SimpleTargetHandler : MonoBehaviour
{
    public Transform Player;
    public float Speed = 7f;
    public BoxCollider2D Bounds;
    public bool SelfDestructIfLeavesBounds = true;

    private TargetingSystem _targetingHandler;
    private Vector2 _directionToMove;

    public void Start()
    {
        if (Player == null)
        {
            Debug.LogError("Null player in TargetEnemy");
            return;
        }

        _targetingHandler = new TargetingSystem(Player);
        _targetingHandler.GetDirectionToTarget(transform.position, out _directionToMove);
    }

    public void Initialize(Transform player, float speed)
    {
        Player = player;
        Speed = speed;

        SelfDestructIfLeavesBounds = true;
    }

    public void Update()
    {
        if (Globals.GameStarting)
        {
            _targetingHandler = new TargetingSystem(Player);
            _targetingHandler.GetDirectionToTarget(transform.position, out _directionToMove);
            return;
        }

        if (Globals.GameOver)
            return;
        
        if (SelfDestructIfLeavesBounds)
        {
            CheckForBounds();
        }

        MoveBlock();
    }

    private void MoveBlock()
    {
        var deltaMovement = _directionToMove * Speed * Time.deltaTime * Globals.GlobalRatio;
        transform.position = new Vector3(transform.position.x + deltaMovement.x, transform.position.y + deltaMovement.y, transform.position.z);

        var vibrationHandler = GetComponentInChildren<VibrationHandler>();
        if(vibrationHandler)
        {
            vibrationHandler.SetCenterPosition(transform.position);
        }
    }

    private void CheckForBounds()
    {
        var leftPosition = Bounds.transform.position.x + (Bounds.offset.x - Bounds.size.x / 2) * Bounds.transform.localScale.x;
        var rightPosition = Bounds.transform.position.x + (Bounds.offset.x + Bounds.size.x / 2) * Bounds.transform.localScale.x;

        var topPosition = Bounds.transform.position.y + (Bounds.offset.y + Bounds.size.y / 2) * Bounds.transform.localScale.y;
        var bottomPosition = Bounds.transform.position.y + (Bounds.offset.y - Bounds.size.y / 2) * Bounds.transform.localScale.y;

        if (transform.position.x < leftPosition || transform.position.x > rightPosition || transform.position.y > topPosition || transform.position.y < bottomPosition)
        {
            Kill();
        }
    }

    private void Kill()
    {
        Destroy(gameObject);
    }

}
