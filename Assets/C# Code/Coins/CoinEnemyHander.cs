using UnityEngine;

public class CoinEnemyHander : MonoBehaviour
{
    private CoinIdentifier _identifier;

    public void Start()
    {
        _identifier = GetComponent<CoinIdentifier>();
        if (!_identifier)
            Debug.LogError("No coin identifier attached!");
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<EnemyIdentifier>())
        {
            Destroy(_identifier.Handler.gameObject);
        }
    }
}
