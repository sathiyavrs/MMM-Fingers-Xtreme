using UnityEngine;

public class ScoreOnMovement : MonoBehaviour
{
    public float Multiplier = 2;
    public bool SlowMotionImpact = false;

    private Vector2 _prevPosition;

    public void Start()
    {
        _prevPosition = transform.position;
    }

    public void Update()
    {
        var currentPosition = (Vector2)transform.position;
        var differenceMagnitude = (currentPosition - _prevPosition).magnitude;
        _prevPosition = currentPosition;

        if (Globals.GameOver)
            return;

        //if (Globals.GameStarting)
        //    return;

        Globals.GameScore += SlowMotionImpact ? differenceMagnitude * Multiplier * Globals.GlobalRatio : differenceMagnitude * Multiplier;
    }
}

