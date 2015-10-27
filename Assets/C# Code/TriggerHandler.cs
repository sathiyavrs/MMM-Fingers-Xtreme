using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public CustomResources.ObjectType Type;

    public void Start()
    {
        if(!GetComponent<Collider2D>())
        {
            Debug.LogError("Collider 2D Component not found!");
        }
    }

    public void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (Globals.GameOver)
            return;

        switch(Type)
        {
            case CustomResources.ObjectType.Friendly:
                FriendlyTrigger(other);
                break;

            case CustomResources.ObjectType.Hostile:

                break;
        }
    }

    private void FriendlyTrigger(Collider2D other)
    {
        if (other.GetComponent<EnemyIdentifier>())
        {
            Globals.GameOver = true;

            var flicker = other.GetComponent<EnemyIdentifier>().WhiteEffect.GetComponent<TextureFlicker>();
            if (flicker == null)
                return;

            flicker.Activate();
        }
    }
}
