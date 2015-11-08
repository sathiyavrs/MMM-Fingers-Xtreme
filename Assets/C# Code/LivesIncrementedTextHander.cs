using UnityEngine;
using UnityEngine.UI;

public class LivesIncrementedTextHander : MonoBehaviour
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
        if(Globals.LivesIncremented)
        {
            _timeRemaining = TimeToLive;
            Globals.LivesIncremented = false;
        }

        if (_timeRemaining > 0)
        {
            _text.text = "Lives Incremented!";
        }
        else
            _text.text = "";

        _timeRemaining -= Time.deltaTime * Globals.GlobalRatio;

    }
}
