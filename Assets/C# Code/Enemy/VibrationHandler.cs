using UnityEngine;

public class VibrationHandler : MonoBehaviour
{
    public float Amplitude;
    public float Multiplier;

    private Vector2 _centerPosition;

    public void Start()
    {
        _centerPosition = transform.position;
    }

    public void SetCenterPosition(Vector2 position)
    {
        _centerPosition = position;
    }

    public void Update()
    {
        if (Globals.GameOver)
            return;

        var amplitude = Amplitude * transform.localScale.x;
        var directionAngle = Random.Range(0, 2 * Mathf.PI);
        var direction = new Vector2(Mathf.Cos(directionAngle), Mathf.Sin(directionAngle));

        direction *= Multiplier * amplitude * Time.deltaTime * Globals.GlobalRatio;

        var finalPosition = new Vector2(transform.position.x, transform.position.y) + direction;
        var distanceSquared = (finalPosition - _centerPosition).sqrMagnitude;

        if (distanceSquared > amplitude)
        {
            direction = (finalPosition - _centerPosition) / Mathf.Sqrt(distanceSquared);
            finalPosition = _centerPosition + direction;
            transform.position = finalPosition;
            return;
        }

        transform.position = finalPosition;
    }
}