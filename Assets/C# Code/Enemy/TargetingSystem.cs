using UnityEngine;

public class TargetingSystem 
{
    private Transform _target;

    public TargetingSystem()
    {

    }

    public TargetingSystem(Transform target)
    {
        _target = target;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public bool GetTargetPosition(out Vector2 targetPosition)
    {
        targetPosition = new Vector2(0, 0);

        if (_target == null)
            return false;

        targetPosition = new Vector2(_target.position.x, _target.position.y);
        return true;
    }

    public bool GetDirectionToTarget(Vector2 startingPoint, out Vector2 direction)
    {
        direction = new Vector2();
        if (_target == null)
            return false;

        var targetPosition = new Vector2();
        GetTargetPosition(out targetPosition);

        var difference = targetPosition - startingPoint;
        direction = difference / difference.magnitude;

        return true;
    }
}
