using UnityEngine;

public class CoinHandler : MonoBehaviour
{
    public int PointsToAdd;
    public float ShrinkRate;

    private bool _shrinkInitiated;

    public void Update()
    {
        if (Globals.GameOver)
            return;

        if (!_shrinkInitiated)
            return;

        var deltaScale = ShrinkRate * Time.deltaTime * Globals.GlobalRatio * -1;
        transform.localScale = new Vector3(transform.localScale.x + deltaScale, transform.localScale.y + deltaScale, transform.localScale.z);

        if (transform.localScale.x < 0.001f)
        {
            Globals.GameScore += PointsToAdd;
            Globals.ScoreIncrement += PointsToAdd;
            Globals.StarCollected = true;
            Globals.StarReward = PointsToAdd;
            Destroy(gameObject);
        }
    }

    public void InitiateShrink()
    {
        _shrinkInitiated = true;
    }
}
