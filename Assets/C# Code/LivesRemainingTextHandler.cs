using UnityEngine;
using UnityEngine.UI;

public class LivesRemainingTextHandler : MonoBehaviour
{
    private Text _text;

    public void Start()
    {
        _text = GetComponent<Text>();
        if (_text == null)
            Debug.LogError("Text is null");
    }

    public void Update()
    {
        _text.text = "Lives Remaining : " + Globals.LivesRemaining;
    }

}
