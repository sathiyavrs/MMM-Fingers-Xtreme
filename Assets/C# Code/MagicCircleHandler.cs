using UnityEngine;

public class MagicCircleHandler : MonoBehaviour
{
    public GameObject Enemy;
    public CircleCollider2D CircularBounds;
    public float AngularSpeed;
    public float MinRadius;
    public bool Clockwise;
    public float RadialSpeed;

    [Range(0, 1)]
    public float RadiusChangeMultiplier;

    private float _radius;
    private Vector2 _localCenter;
    private float _maxRadiusChangeLower;
    private float _minRadiusChangeUpper;
    private float _angle;

    private bool _radiusIncreasing;

    public float MaxRadius
    {
        get
        {
            // Here I'm assuming CircularBounds is scaled equally in x and y
            return CircularBounds.radius * CircularBounds.transform.localScale.x;
        }
    }

    public void Start()
    {
        if (!Enemy)
            Debug.LogError("Enemy is null!");

        if (!CircularBounds)
            Debug.LogError("Circular Bounds is null!");

        if (MaxRadius < MinRadius)
            Debug.LogError("Max Radius < Min Radius!");

        // Here I'm assuming CircularBounds is scaled equally in x and y.
        _radius = CircularBounds.radius * CircularBounds.transform.localScale.x;

        var centerX = (CircularBounds.offset.x + CircularBounds.transform.localPosition.x) * CircularBounds.transform.localScale.x;

        var centerY = (CircularBounds.offset.y + CircularBounds.transform.localPosition.y) * CircularBounds.transform.localScale.y;

        _localCenter = new Vector2(centerX, centerY);

        _minRadiusChangeUpper = MaxRadius - (MaxRadius - MinRadius) * RadiusChangeMultiplier;
        _maxRadiusChangeLower = MinRadius + (MaxRadius - MinRadius) * RadiusChangeMultiplier;

        _radiusIncreasing = false;
        _angle = 0;
    }

    public void Update()
    {
        if (Globals.GameOver)
            return;

        DetermineRadius();
        HandleMovement();
    }

    private void DetermineRadius()
    {
        var deltaRadius = RadialSpeed * Time.deltaTime * Globals.GlobalRatio;
        deltaRadius *= (_radiusIncreasing) ? 1 : -1;
        deltaRadius *= Random.Range(0.5f, 1);

        _radius += deltaRadius;

        if (_radius > MaxRadius)
            _radius = MaxRadius;

        if (_radius < MinRadius)
            _radius = MinRadius;

        if (_radius > _minRadiusChangeUpper)
        {
            var randomNumber = Random.Range(_radius, MaxRadius);

            // Weighted Average Calculations, unfortunately Hardcoded.
            if (randomNumber > (_minRadiusChangeUpper + 2 * MaxRadius) / 3)
            {
                _radiusIncreasing = false;
                return;
            }
        }

        if (_radius < _maxRadiusChangeLower)
        {
            var randomNumber = Random.Range(MinRadius, _radius);

            // Weighted Average Calculations, unfortunately Hardcoded.
            if (randomNumber < (_minRadiusChangeUpper + 2 * MinRadius) / 3)
            {
                _radiusIncreasing = true;
                return;
            }
        }
    }

    private void HandleMovement()
    {
        var deltaAngle = AngularSpeed * Time.deltaTime * Globals.GlobalRatio;
        deltaAngle *= Clockwise ? 1 : -1;
        deltaAngle *= Random.Range(0.5f, 1);
        _angle += deltaAngle;
        
        var nextPosition = _localCenter + (new Vector2(Mathf.Cos(_angle), Mathf.Sin(_angle))) * _radius;
        Enemy.transform.localPosition = new Vector3(nextPosition.x, nextPosition.y, Enemy.transform.localPosition.z);

        var vibrationHandler = Enemy.GetComponent<VibrationHandler>();

        if (vibrationHandler == null)
            return;

        vibrationHandler.SetCenterPosition(Enemy.transform.position);

    }
}
