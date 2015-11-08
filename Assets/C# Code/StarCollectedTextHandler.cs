using UnityEngine;
using UnityEngine.UI;

public class StarCollectedTextHandler : MonoBehaviour
{
    public float TimeToLive = 1f;

    private Text _text;
    private float _timeRemaining;

    public void Start()
    {
        _text = GetComponent<Text>();
        if (_text == null)
            Debug.LogError("Text is null");
        _timeRemaining = -1;
    }
    public void Update()
    {
        if (Globals.StarCollected)
        {
            _timeRemaining = TimeToLive;
            Globals.StarCollected = false;
        }

        if (_timeRemaining > 0)
        {
            _text.text = "+" + Globals.StarReward + "!";
        }
        else
            _text.text = "";

        _timeRemaining -= Time.deltaTime * Globals.GlobalRatio;
    }
}
