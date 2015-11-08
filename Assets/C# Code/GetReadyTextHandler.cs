using UnityEngine;
using UnityEngine.UI;

public class GetReadyTextHandler : MonoBehaviour
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
        if (Globals.GameStarting)
            _text.text = "Get Ready! Game begins in five seconds!";
        else
            _text.text = "";
    }
}
